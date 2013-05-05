using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheNoiseHLC;
using TheNoiseHLC.CommunicationObjects;
using TheNoiseHLC.CommunicationObjects.AudioTrack;
using TheNoiseHLC.CommunicationObjects.GlobalEnumerations;
using TcpTransmission;
using TcpTransmission.Client;
using System.Net;

namespace TheNoise_Server
{
    /// <summary>
    /// Extends ServerManager to handle authentication and audio streaming requests.
    /// </summary>
    public sealed class TheNoiseServer : TcpTransmission.Server.ServerManager
    {
        public event EventHandler<ClientAuthEventArgs> ClientAuthenticated = delegate { };
        public event EventHandler<GeneralEventArgs> GeneralEvent = delegate { };

        // Contains a list of active authenticated connections. TheNoiseClientHandler processes authenticated client requests.
        Dictionary<IPEndPoint, TheNoiseClientHandler> authenticatedConnections = new Dictionary<IPEndPoint, TheNoiseClientHandler>();

        // Extends authenticatedConnections so that usernames can be paired with their connection.
        Dictionary<string, IPEndPoint> usernameLookup = new Dictionary<string, IPEndPoint>();

        // Watches the file system for changes. Clients are updated accordingly.
        private System.IO.FileSystemWatcher watcher;

        // Sets debugging mode. If true the database will not be queried, requests will simply be accepted.
        private bool debugging = false;
        public bool Debugging
        {
            get { return debugging; }
            set
            {
                if (value && value != debugging)
                {
                    GeneralEvent.Invoke(this, new GeneralEventArgs("Server", null, "Debug mode ON. All auth requests will be accepted."));
                }
                else if (!value && value != debugging)
                {
                    GeneralEvent.Invoke(this, new GeneralEventArgs("Server", null, "Debug mode OFF. The database will be queried for auth requests."));
                }
                debugging = value;
            }
        }

        // Base path to store/retreive user files.
        private readonly string audioPath;

        // Database connection details.
        private IPAddress databaseAddress;
        public IPAddress DatabaseAddress
        {
            get { return databaseAddress; }
            set
            {
                if (!base.Running)
                {
                    databaseAddress = value;
                }
            }
        }

        private int databasePort;
        public int DatabasePort
        {
            get { return databasePort; }
            set
            {
                if (!base.Running)
                {
                    databasePort = value;
                }
            }
        }

        private bool databaseUseIntegratedSecurity;
        public bool DatabaseUseIntegratedSecurity
        {
            get { return databaseUseIntegratedSecurity; }
            set
            {
                if (!base.Running)
                {
                    databaseUseIntegratedSecurity = value;
                }
            }
        }

        private string databaseUsername;
        public string DatabaseUsername
        {
            get { return databaseUsername; }
            set
            {
                if (!base.Running)
                {
                    databaseUsername = value;
                }
            }
        }

        private string databasePassword;
        public string DatabasePassword
        {
            get { return databasePassword; }
            set
            {
                if (!base.Running)
                {
                    databasePassword = value;
                }
            }
        }

        private string databaseName;
        public string DatabaseName
        {
            get { return databaseName; }
            set
            {
                if (!base.Running)
                {
                    databaseName = value;
                }
            }
        }

        public TheNoiseServer(IPAddress bindIP, int port, string audioPath)
            : base(bindIP, port)
        {
            this.audioPath = audioPath;

            if (!System.IO.Directory.Exists(audioPath))
            {
                System.IO.Directory.CreateDirectory(audioPath);
            }

            watcher = new System.IO.FileSystemWatcher(audioPath);
            watcher.NotifyFilter = System.IO.NotifyFilters.DirectoryName;
            watcher.Changed += watcher_Changed;
        }

        private void watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (usernameLookup.ContainsKey(e.Name))
            {
                authenticatedConnections[usernameLookup[e.Name]].RequestUpdateList();
            }
        }

        // Wrap the client events so the server owner can catch them.
        protected override void DataReceivedHandle(ClientConnection sender, IncomingMessageEventArgs e)
        {
            // If client already authenticated.
            if (authenticatedConnections.ContainsKey(sender.RemoteEndPoint))
            {
                switch ((PacketType)e.PacketType)
                {
                    case PacketType.AudioSegment:
                        authenticatedConnections[sender.RemoteEndPoint].ContinueStream();
                        break;
                    case PacketType.RequestList:
                        GeneralEvent.Invoke(this, new GeneralEventArgs(authenticatedConnections[sender.RemoteEndPoint].Username,
                            sender.RemoteEndPoint, ("Requested a new track list.")));
                        authenticatedConnections[sender.RemoteEndPoint].RequestUpdateList();
                        break;
                    case PacketType.StartAudioStream:
                        byte[] requestReply = null;
                        BeginStreamingAudio(sender.RemoteEndPoint, e.Message, out requestReply);
                        SendData(sender.RemoteEndPoint, requestReply, (byte)PacketType.StartAudioStream);
                        break;
                    case PacketType.StopAudioStream:
                        authenticatedConnections[sender.RemoteEndPoint].EndStreaming();
                        break;
                    case PacketType.UnknownPacket:
                        GeneralEvent.Invoke(this, new GeneralEventArgs(authenticatedConnections[sender.RemoteEndPoint].Username,
                            sender.RemoteEndPoint, "Made a request to close the connection."));
                        DropClient(sender.RemoteEndPoint);
                        break;
                    default:
                        GeneralEvent.Invoke(this, new GeneralEventArgs(authenticatedConnections[sender.RemoteEndPoint].Username,
                            sender.RemoteEndPoint, ("Made a request with packet type " + ((PacketType)e.PacketType).ToString() + " which is invalid for this context.")));
                        break;
                }
            }
            else
            {
                // See if the client is trying to authenticate or register.
                byte[] send = null;
                switch ((PacketType)e.PacketType)
                {
                    case PacketType.Authenticate:
                        // Authenticate this client.
                        AuthenticateClient(sender.RemoteEndPoint, e.Message, out send);
                        SendData(sender.RemoteEndPoint, send, (byte)PacketType.Authenticate);
                        break;
                    case PacketType.Register:
                        // Register new credentials.
                        RegisterNewUser(sender.RemoteEndPoint, e.Message, out send);
                        SendData(sender.RemoteEndPoint, send, (byte)PacketType.Register);
                        break;
                    case PacketType.UnknownPacket:
                        GeneralEvent.Invoke(this, new GeneralEventArgs(null, sender.RemoteEndPoint, "Made a request to close the connection."));
                        DropClient(sender.RemoteEndPoint);
                        break;
                    default:
                        GeneralEvent.Invoke(this, new GeneralEventArgs(null, sender.RemoteEndPoint,
                            ("Made a request with packet type " + ((PacketType)e.PacketType).ToString() + " which is invalid for this context.")));
                        break;
                }
                if (send != null) { sender.SendDataPacket(ref send, e.PacketType); } // Reply with the same header and some data.
            }
        }

        private void TheNoiseServer_trackListUpdated(IPEndPoint sender, TrackList e)
        {
            byte[] send;
            ObjectSerialization.Serialize(e, out send);
            clientList[sender].SendDataPacket(ref send, (byte)PacketType.RequestList);
            GeneralEvent.Invoke(this, new GeneralEventArgs(authenticatedConnections[sender].Username,
                sender, ("Was sent a track list with " + e.Tracks.Length + " elements.")));
        }

        private void TheNoiseServer_audioPacketReady(IPEndPoint sender, AudioPacketEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AuthenticateClient(IPEndPoint sender, byte[] message, out byte[] send)
        {
            try
            {
                UserAuthenticationResult result;
                // Create a database access object to validate this user against.
                using (TheNoise_DatabaseControl.DataAccessLayer databaseAccess =
                    new TheNoise_DatabaseControl.DataAccessLayer(databaseAddress.ToString(), databaseName, databaseUsername, databasePassword, databaseUseIntegratedSecurity))
                {

                    // Deserialize the auth request.
                    LoginData credentials = (LoginData)ObjectSerialization.Deserialize(message, typeof(LoginData));
                    // Attempt to validate this user.
                    if (!debugging)
                    {
                        result = databaseAccess.validateUser(credentials);
                    }
                    else
                    {
                        // Debug mode, simply accept the auth request.
                        result = UserAuthenticationResult.Success;
                    }

                    if (result == UserAuthenticationResult.Success)
                    {
                        // Add the user to the authenticated list.
                        authenticatedConnections.Add(sender, new TheNoiseClientHandler(credentials.username, audioPath + credentials.username + "\\", sender));
                        usernameLookup.Add(credentials.username, sender);

                        authenticatedConnections[sender].audioPacketReady += TheNoiseServer_audioPacketReady;
                        authenticatedConnections[sender].trackListUpdated += TheNoiseServer_trackListUpdated;

                        // Fire event that a user is authenticated.
                        ClientAuthenticated.Invoke(this, new ClientAuthEventArgs(sender, credentials.username));
                    }
                    else
                    {
                        GeneralEvent.Invoke(this, new GeneralEventArgs(null, sender,
                            "Tried to authenticate, but the database gave us: " + result.ToString()));
                    }
                }

                // Generate response for client.
                ObjectSerialization.Serialize(result, out send);
            }
            catch (Exception ex)
            {
                GeneralEvent.Invoke(this, new GeneralEventArgs(null, sender, "Tried to authenticate, but something failed: " + ex.Message));
                ObjectSerialization.Serialize(UserAuthenticationResult.UnknownResult, out send);
            }
        }

        private void BeginStreamingAudio(IPEndPoint sender, byte[] message, out byte[] send)
        {
            TheNoiseHLC.CommunicationObjects.GlobalEnumerations.TrackStreamRequestResult result;

            try
            {
                // Deserialize the message.
                TrackStreamRequest request = (TrackStreamRequest)ObjectSerialization.Deserialize(message, typeof(TrackStreamRequest));
                // Check to see if the requested file exists on the server.
                string username = authenticatedConnections[sender].Username;
                string trackPath = audioPath + username + "\\" + request.Track.TrackName + request.Track.TrackExtension;
                if (System.IO.File.Exists(trackPath))
                {
                    // Ask the client to being streaming.
                    authenticatedConnections[sender].BeginStreaming(request);
                    GeneralEvent.Invoke(this, new GeneralEventArgs(authenticatedConnections[sender].Username, sender, ("Started streaming \"" + request.Track.TrackName + "\".")));
                    result = TrackStreamRequestResult.Success;
                }
                else
                {
                    GeneralEvent.Invoke(this, new GeneralEventArgs(authenticatedConnections[sender].Username, sender, ("Tried to stream \"" + request.Track.TrackName + "\" but it wasn't found.")));
                    result = TrackStreamRequestResult.InvalidFileName;
                }
            }
            catch (Exception ex)
            {
                GeneralEvent.Invoke(this, new GeneralEventArgs(authenticatedConnections[sender].Username, sender, ("Tried to stream something, but this happened instead: " + ex.Message)));
                result = TrackStreamRequestResult.UnknownResult;
            }

            // Give back the reply to the client.
            ObjectSerialization.Serialize(result, out send);
        }

        private void RegisterNewUser(IPEndPoint sender, byte[] message, out byte[] send)
        {
            try
            {
                // Create a database access object to register new user with database.
                using (TheNoise_DatabaseControl.DataAccessLayer databaseAccess =
                    new TheNoise_DatabaseControl.DataAccessLayer(databaseAddress.ToString(), databaseName, databaseUsername, databasePassword, databaseUseIntegratedSecurity))
                {
                    // Deserialize the login request.
                    LoginData credentials = (LoginData)ObjectSerialization.Deserialize(message, typeof(LoginData));

                    // Attempt to register with the database.
                    UserAddResult result;
                    if (!debugging)
                    {
                        result = databaseAccess.addUser(credentials);
                    }
                    else
                    {
                        result = UserAddResult.Success;
                    }

                    ObjectSerialization.Serialize(result, out send);

                    GeneralEvent.Invoke(this, new GeneralEventArgs(null, sender,
                        "Issued a request for registration and the result was: " + result.ToString()));
                }
            }
            catch (Exception ex)
            {
                GeneralEvent.Invoke(this, new GeneralEventArgs(null, sender, "Tried to register a new user, but something failed: " + ex.Message));
                ObjectSerialization.Serialize(UserAuthenticationResult.UnknownResult, out send);
            }
        }

        protected override void ClientRemove(IPEndPoint clientIPE)
        {
            base.ClientRemove(clientIPE);

            if (authenticatedConnections.ContainsKey(clientIPE))
            {
                usernameLookup.Remove(authenticatedConnections[clientIPE].Username);
                authenticatedConnections.Remove(clientIPE);
            }
        }

        /// <summary>
        /// Closes the specified client connection and releases any open streaming handles.
        /// </summary>
        /// <param name="clientIPE">The client IP to drop.</param>
        public override void DropClient(IPEndPoint clientIPE)
        {
            base.DropClient(clientIPE); // Drop the client from the clientlist.

            // Also take care of additional client resources.
            if (authenticatedConnections.ContainsKey(clientIPE))
            {
                usernameLookup.Remove(authenticatedConnections[clientIPE].Username);
                authenticatedConnections.Remove(clientIPE);
            }
        }
    }
}
