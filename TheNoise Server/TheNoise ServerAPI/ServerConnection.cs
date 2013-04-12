using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TcpTransmission;
using TcpTransmission.Client;
using TheNoise_SharedObjects;
using TheNoise_SharedObjects.GlobalEnumerations;
using TheNoise_SharedObjects.AudioTrack;

namespace TheNoiseAPI
{
    /// <summary>
    /// Provides a simple API to the 
    /// </summary>
    public class ServerConnection : IDisposable
    {
        IPAddress ip;
        int port;

        private ClientConnection client;
        ObjectSerialization serializer= new ObjectSerialization();

        public delegate void TrackListEventHandler(object sender, TrackList e);
        public event TrackListEventHandler AudioListReceived = delegate { }; // Fired when data is received from a client.

        public delegate void DataReceivedEventHandler(object sender, IncomingMessageEventArgs e);
        public event DataReceivedEventHandler AudioPacketReceived = delegate { }; // Fired when data is received from a client.

        public ServerConnection(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        volatile bool startedRequest = false;
        byte expectedResponse = 90;
        byte[] response;
        private void client_dataReceived(object sender, IncomingMessageEventArgs e)
        {
            // Check to see if the client is still connected.
            if (!Connected())
            {
                // The client has disconnected.

                if (e == null) { return; } // Don't give listeners of dataReceived a null IncomingMessageEventArgs.
            }

            if (startedRequest && e.PacketType == expectedResponse)
            {
                // This packet is a response to an open request.
                response = e.Message;
                startedRequest = false; // Lower the unfulfilled flag.
                return;
            }

            switch ((PacketType)e.PacketType)
            {
                case PacketType.RequestList:
                    // Deserialize the tracklist and pass it up.
                    ObjectSerialization deserializer = new ObjectSerialization();
                    AudioListReceived.Invoke(this, (TrackList)deserializer.Deserialize(e.Message, typeof(TrackList)));
                    break;
                case PacketType.AudioSegment:
                    // TODO: Implement audio streaming.
                    break;
                default:
                    break;
            }

            //dataReceived.Invoke(sender, e);
        }

        /// <summary>
        /// Tests the connection to the server. Returns true if connected.
        /// </summary>
        /// <returns>True if connected, else false.</returns>
        public bool Connected()
        {
            return (client.Connected);
        }

        // Makes a blocking request to the server. This returns a serialized object or null (timeout/disconnect).
        private bool MakeBlockingRequest(ref byte[] send, PacketType packetType, PacketType expectedResponse)
        {
            this.expectedResponse = (byte)expectedResponse;
            startedRequest = true;
            client.SendDataPacket(ref send, (byte)packetType);

            // Wait until the request has been fulfilled.
            int timeoutCounter = 30;
            while (startedRequest)
            {
                if ((timeoutCounter > 0) && client.Connected)
                {
                    System.Threading.Thread.Sleep(100);
                    timeoutCounter--;
                    continue;
                }
                return false; // The request timed out or client disconnect.
            }
            return true;
        }

        /// <summary>
        /// Attempt to register the specified credentials with the server.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The returned status code from the server in form of a UserAddResult</returns>
        public UserAddResult Register(string username, string password)
        {
            if (Connected())
            {
                byte[] send;
                serializer.Serialize(new LoginData(username, password), out send);

                // Make a request to the server to register the user. 
                if (MakeBlockingRequest(ref send, PacketType.Register, PacketType.Register))
                {
                    // Deserialize the response.
                    UserAddResult result = new UserAddResult();
                    return (UserAddResult)serializer.Deserialize(response, result.GetType());
                }
            }
            return UserAddResult.UnknownResult; // Who knows what happened.
        }

        /// <summary>
        /// Authenticates the specified credentials with the server, allowing subsequent privileged operations.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The returned status code from the server in form of a UserAuthenticationResult</returns>
        public UserAuthenticationResult Authenticate(string username, string password)
        {
            if (Connected())
            {
                byte[] send;
                serializer.Serialize(new LoginData(username, password), out send);

                // Make a request to the server to register the user. 
                if (MakeBlockingRequest(ref send, PacketType.Authenticate, PacketType.Authenticate))
                {
                    // Deserialize the response.
                    UserAuthenticationResult result = new UserAuthenticationResult();
                    return (UserAuthenticationResult)serializer.Deserialize(response, result.GetType());
                }
            }
            return UserAuthenticationResult.UnknownResult; // Who knows what happened.
        }

        /// <summary>
        /// Requests the list of available audio from the server.
        /// Typically, the server will automatically send the audio list when necessary.
        /// Only make this request if you no longer have the audio list.
        /// </summary>
        public void RequestAudioList()
        {
            if (Connected())
            {
                byte[] send = { 0 };
                client.SendDataPacket(ref send, (byte)PacketType.RequestList);
            }
        }

        /// <summary>
        /// Requests that the server being streaming the specified track.
        /// </summary>
        /// <param name="track">The track to stream</param>
        public void StartAudioStream(TheNoise_SharedObjects.AudioTrack.TrackStreamRequest track)
        {
            if (Connected())
            {
                byte[] send;
                serializer.Serialize(track, out send);
                client.SendDataPacket(ref send, (byte)PacketType.StopAudioStream);
            }
        }

        /// <summary>
        /// Requests that the server end all streaming of audio.
        /// </summary>
        public void StopAudioStream()
        {
            if (Connected())
            {
                byte[] send = { 0 };
                client.SendDataPacket(ref send, (byte)PacketType.StopAudioStream);
            }
        }

        public bool OpenConnection()
        {
            try
            {
                client = new ClientConnection(ip, port);
                client.dataReceived += client_dataReceived;
                client.Connect();
            }
            catch (Exception)
            {
            }
            return client.Connected;
        }

        public void CloseConnection()
        {
            client.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Close all managed resources.
                CloseConnection();
            }

            // Dispose of unmanaged resources here if any.
        }
    }
}
