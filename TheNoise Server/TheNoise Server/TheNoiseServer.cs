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
    internal class AudioPacketEventArgs : EventArgs
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

        public void ContinueStream()
        {
        }

        public void BeginStreaming(TrackStreamRequest audioTrack)
        {

        }

        public void EndStreaming()
        {

        }

        public void RequestUpdateList()
        {
            // Hack to prevent firing of this event too quickly.
            if (DateTime.UtcNow.Ticks > (lastUpdatedMusicList + 50000000)) // 5 seconds
            {
                lastUpdatedMusicList = DateTime.UtcNow.Ticks;

                string[] files = System.IO.Directory.GetFiles(audioPath);
                Track[] tracks = new Track[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    tracks[i] = new Track(System.IO.Path.GetFileNameWithoutExtension(files[i]), 180, TrackType.Unspecified);
                }

                trackListUpdated.Invoke(tcpEndPoint, new TrackList(tracks));
            }
        }
    }

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

    public sealed class TheNoiseServer : TcpTransmission.Server.ServerManager
    {
        public event EventHandler<ClientAuthEventArgs> clientAuthenticated = delegate { };

        // Contains a list of active authenticated connections. TheNoiseClientHandler processes authenticated client requests.
        Dictionary<IPEndPoint, TheNoiseClientHandler> authenticatedConnections = new Dictionary<IPEndPoint, TheNoiseClientHandler>();

        // Extends authenticatedConnections so that usernames can be paired with their connection.
        Dictionary<string, IPEndPoint> usernameLookup = new Dictionary<string, IPEndPoint>();

        // Watches the file system for changes. Clients are updated accordingly.
        private System.IO.FileSystemWatcher watcher;

        // Base path to store/retreive user files.
        private readonly string audioPath;

        // Database connection details.
        private readonly string databaseAddress;
        private readonly string databaseName;

        public TheNoiseServer(IPAddress bindIP, int port, string audioPath, string usersDatabaseAddress, string usersDatabaseName)
            : base(bindIP, port)
        {
            databaseAddress = usersDatabaseAddress;
            databaseName = usersDatabaseName;
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
                        authenticatedConnections[sender.RemoteEndPoint].RequestUpdateList();
                        break;
                    case PacketType.StartAudioStream:
                        byte[] requestReply = null;
                        BeginStreamingAudio(sender.RemoteEndPoint, e.Message, out requestReply);
                        SendData(sender.RemoteEndPoint, requestReply, (byte)PacketType.StartAudioStream);
                        break;
                    case PacketType.StopAudioStream:
                        break;
                    default:
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
                        RegisterNewUser(e.Message, out send);
                        SendData(sender.RemoteEndPoint, send, (byte)PacketType.Register);
                        break;
                    default:
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
        }

        private void TheNoiseServer_audioPacketReady(IPEndPoint sender, AudioPacketEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AuthenticateClient(IPEndPoint clientIPE, byte[] message, out byte[] send)
        {
            // Create a database access object to validate this user against.
            using (TheNoise_DatabaseControl.DataAccessLayer databaseAccess = new TheNoise_DatabaseControl.DataAccessLayer(databaseAddress, databaseName))
            {
                LoginData credentials = (LoginData)ObjectSerialization.Deserialize(message, typeof(LoginData));

                UserAuthenticationResult result = databaseAccess.validateUser(credentials);

                if (result == UserAuthenticationResult.Success)
                {
                    // Add the user to the authenticated list.
                    authenticatedConnections.Add(clientIPE, new TheNoiseClientHandler(credentials.username, audioPath + credentials.username + "\\", clientIPE));
                    usernameLookup.Add(credentials.username, clientIPE);

                    authenticatedConnections[clientIPE].audioPacketReady += TheNoiseServer_audioPacketReady;
                    authenticatedConnections[clientIPE].trackListUpdated += TheNoiseServer_trackListUpdated;

                    // Fire event that a user is authenticated.
                    clientAuthenticated.Invoke(this, new ClientAuthEventArgs(clientIPE, credentials.username));
                }

                // Generate response for client.
                ObjectSerialization.Serialize(result, out send);
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
                if (System.IO.File.Exists(audioPath + request.Track.TrackName))
                {
                    // Ask the client to being streaming.
                    authenticatedConnections[sender].BeginStreaming(request);
                    result = TrackStreamRequestResult.Success;
                }
                else
                {
                    result = TrackStreamRequestResult.InvalidFileName;
                }
            }
            catch
            {
                result = TrackStreamRequestResult.UnknownResult;
            }

            // Give back the reply to the client.
            ObjectSerialization.Serialize(result, out send);
        }

        private void RegisterNewUser(byte[] message, out byte[] send)
        {
            // Create a database access object to register new user with database.
            using (TheNoise_DatabaseControl.DataAccessLayer databaseAccess = new TheNoise_DatabaseControl.DataAccessLayer(databaseAddress, databaseName))
            {
                LoginData credentials = (LoginData)ObjectSerialization.Deserialize(message, typeof(LoginData));

                UserAddResult result = databaseAccess.addUser(credentials);
                ObjectSerialization.Serialize(result, out send);
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
