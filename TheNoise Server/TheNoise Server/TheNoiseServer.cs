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
    /// Contains event data for large packets of audio.
    /// </summary>
    internal sealed class AudioPacketEventArgs : EventArgs
    {
        public byte[] data;

        public AudioPacketEventArgs(byte[] data)
        {
            this.data = data;
        }
        public AudioPacketEventArgs()
        {
        }
    }

    /// <summary>
    /// Handles TheNoise server specific client serving capability.
    /// </summary>
    internal sealed class TheNoiseClientHandler
    {
        public delegate void AudioPacketEventHandler(IPEndPoint sender, AudioPacketEventArgs e);
        public event AudioPacketEventHandler audioPacketReady = delegate { };

        public delegate void TrackListUpdatedEventHandler(IPEndPoint sender, TrackList e);
        public event TrackListUpdatedEventHandler trackListUpdated = delegate { };

        public delegate void ClientSendBackHandler(IPEndPoint sender, IncomingMessageEventArgs e);
        public event ClientSendBackHandler ClientSendBack = delegate { };

        private string username; // The username this client authenticated with.
        public string Username
        {
            get { return username; }
            set { }
        }

        private string audioPath;
        public string AudioPath
        {
            get { return audioPath; }
            set { }
        }

        private IPEndPoint tcpEndPoint;
        private IPEndPoint udpEndPoint;

        private AwesomeAudio.UDPSender audioSender;

        private long lastUpdatedMusicList;

        public TheNoiseClientHandler(string username, string audioPath, IPEndPoint ipe)
        {
            this.username = username;
            this.audioPath = audioPath;
            tcpEndPoint = ipe;

            if (!System.IO.Directory.Exists(audioPath))
            {
                System.IO.Directory.CreateDirectory(audioPath);
            }
        }


        /// <summary>
        /// Deprecated. Use BeginStreaming only.
        /// </summary>
        public void ContinueStream()
        {

        }

        /// <summary>
        /// Begins the streaming.
        /// </summary>
        /// <param name="audioTrack">The audio track.</param>
        public void BeginStreaming(TrackStreamRequest audioTrack)
        {
            this.udpEndPoint = audioTrack.Connection;
            string trackLocation = audioPath + audioTrack.Track.TrackName + audioTrack.Track.TrackExtension;

            audioSender = new AwesomeAudio.UDPSender(udpEndPoint.Address.ToString(), trackLocation);
            audioSender.StartFileSend();
        }

        public void EndStreaming()
        {
            if (audioSender != null)
            {
                audioSender.StopFileSend();
            }

            audioSender = null;
        }

        public void RequestUpdateList()
        {
            // Hack to prevent firing of this event too quickly.
            if (DateTime.UtcNow.Ticks > (lastUpdatedMusicList + 50000000)) // 5 seconds
            {
                lastUpdatedMusicList = DateTime.UtcNow.Ticks;

                // Get every file in the user's directory and list them.
                AwesomeAudio.WaveFileManager audioInfo = new AwesomeAudio.WaveFileManager();
                string[] files = System.IO.Directory.GetFiles(audioPath);
                Track[] tracks = new Track[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    audioInfo.ReadWaveFile(files[i]);
                    int secondsLong = audioInfo.NumTenthOfSeconds / 10;
                    string trackName = System.IO.Path.GetFileNameWithoutExtension(files[i]);
                    string trackExt = System.IO.Path.GetExtension(files[i]);
                    tracks[i] = new Track(trackName, trackExt, secondsLong, TrackType.Unspecified);
                }

                trackListUpdated.Invoke(tcpEndPoint, new TrackList(tracks));
            }
        }
    }

    /// <summary>
    /// Provides a method of delivering event data regarding client connect or disconnect.
    /// </summary>
    public sealed class ClientAuthEventArgs : EventArgs
    {
        private IPEndPoint clientIPE;
        public IPEndPoint ClientIPE
        {
            get { return clientIPE; }
            set { }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { }
        }

        public ClientAuthEventArgs(IPEndPoint clientIPE, string clientUsername)
        {
            this.clientIPE = clientIPE;
            this.username = clientUsername;
        }
    }

    /// <summary>
    /// Class for general info events regarding internal server activity.
    /// </summary>
    public sealed class GeneralEventArgs : EventArgs
    {
        public IPEndPoint SubjectIPE { get; private set; }
        public string SubjectUsername { get; private set; }
        public string EventMessage { get; private set; }

        public GeneralEventArgs(string subjectUsername, IPEndPoint subjectIPE, string eventMessage)
        {
            SubjectIPE = subjectIPE;
            SubjectUsername = subjectUsername;
            EventMessage = eventMessage;
        }

        public override string ToString()
        {
            if (EventMessage == null)
            {
                return base.ToString();
            }

            if (SubjectUsername == null && SubjectIPE == null)
            {
                return "General Event: " + EventMessage;
            }
            else if (SubjectUsername != null)
            {
                return String.Format("Client {0} at {1} {2}", SubjectUsername, SubjectIPE, EventMessage);
            }
            else if (SubjectIPE != null)
            {
                return String.Format("Client at {0} {1}", SubjectIPE, EventMessage);
            }


            return base.ToString(); // Why would this ever be hit?
        }
    }

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
                    result = databaseAccess.validateUser(credentials);

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
                    UserAddResult result = databaseAccess.addUser(credentials);
                    ObjectSerialization.Serialize(result, out send);

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
