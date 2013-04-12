using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheNoise_SharedObjects;
using TheNoise_SharedObjects.GlobalEnumerations;
using TcpTransmission;
using TcpTransmission.Client;
using System.Net;

namespace TheNoise_Server
{
    /// <summary>
    /// Handles TheNoise server specific client serving capability.
    /// </summary>
    internal sealed class TheNoiseClientHandler
    {
        private string username; // The username this client authenticated with.
        public string Username
        {
            get { return username; }
            set { }
        }

        public TheNoiseClientHandler(string username)
        {
            this.username = username;
        }
    }

    public sealed class ClientAuthEventArgs : EventArgs
    {
        private IPEndPoint clientIPE;
        public IPEndPoint ClientIPE
        {
            get { return clientIPE; }
            set {  }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set {  }
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

        private readonly string databaseAddress;
        private readonly string databaseName;

        public TheNoiseServer(IPAddress bindIP, int port, string usersDatabaseAddress, string usersDatabaseName)
            : base(bindIP, port)
        {
            databaseAddress = usersDatabaseAddress;
            databaseName = usersDatabaseName;
        }

        // Wrap the client events so the server owner can catch them.
        protected override void DataReceivedHandle(ClientConnection sender, IncomingMessageEventArgs e)
        {
            if (authenticatedConnections.ContainsKey(sender.RemoteEndPoint))
            {
                // Don't continue if the user has already been authenticated.
                return;
            }

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
            
            //OnDataReceived(e);
        }

        private void AuthenticateClient(IPEndPoint clientIPE, byte[] message, out byte[] send)
        {
            ObjectSerialization serializer = new ObjectSerialization();
            // Create a database access object to validate this user against.
            using (TheNoise_DatabaseControl.DataAccessLayer databaseAccess = new TheNoise_DatabaseControl.DataAccessLayer(databaseAddress, databaseName))
            {
                LoginData credentials = (LoginData)serializer.Deserialize(message, typeof(LoginData));

                UserAuthenticationResult result = databaseAccess.validateUser(credentials);

                if (result == UserAuthenticationResult.Success)
                {
                    // Add the user to the authenticated list.
                    authenticatedConnections.Add(clientIPE, new TheNoiseClientHandler(credentials.username));
                    // 
                    clientAuthenticated.Invoke(this, new ClientAuthEventArgs(clientIPE, credentials.username));
                }

                // Generate response for client.
                serializer.Serialize(result, out send);
            }
        }

        private void RegisterNewUser(byte[] message, out byte[] send)
        {
            ObjectSerialization serializer = new ObjectSerialization();
            // Create a database access object to register new user with database.
            using (TheNoise_DatabaseControl.DataAccessLayer databaseAccess = new TheNoise_DatabaseControl.DataAccessLayer(databaseAddress, databaseName))
            {
                LoginData credentials = (LoginData)serializer.Deserialize(message, typeof(LoginData));

                UserAddResult result = databaseAccess.addUser(credentials);
                serializer.Serialize(result, out send);
            }
        }

        /// <summary>
        /// Closes the specified client connection and releases any open streaming handles.
        /// </summary>
        /// <param name="clientIP">The client IP to drop.</param>
        public override void DropClient(IPEndPoint clientIP)
        {
            base.DropClient(clientIP); // Drop the client from the clientlist.

            // Also take care of additional client resources.
        }
    }
}
