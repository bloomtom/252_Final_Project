/*
 * Name:    Thomas Bloom
 * Class:   CP-252
 * Project: 252 Final Project
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TcpTransmission.Client;

namespace TcpTransmission
{

    namespace Server
    {
        /// <summary>
        /// An event based multi-client TCP server.
        /// </summary>
        /// <remarks>
        /// Implements IDisposable.
        /// 
        /// Uses instances of ThreadedNetworking.Client.ClientConnection to keep track of it's client connections.
        /// </remarks>
        /// <seealso cref="EventDrivenNetworking.Client.ClientConnection"/>
        /// <seealso cref="EventDrivenNetworking.IncomingMessageEventArgs"/>
        public class ServerManager : IDisposable
        {
            public delegate void DataReceivedEventHandler(object sender, IncomingMessageEventArgs e);
            public event DataReceivedEventHandler dataReceived = delegate { }; // Fired when data is received from a client.

            public delegate void ClientConnectEventHandler(object sender, IPEndPoint clientIP);
            public event ClientConnectEventHandler clientConnected = delegate { }; // Fired when a client connects.

            public delegate void ClientDisconnectEventHandler(object sender, IPEndPoint clientIP);
            public event ClientDisconnectEventHandler clientDisconnected = delegate { }; // Fired when a client disconnects.

            private volatile bool runServer = false; // When set to false, the server listen loop ends.
            public bool Running
            {
                get { return runServer; }
                set { }
            }

            private TcpListener listener;

            private readonly IPAddress bindIP;
            public IPAddress IP
            {
                get { return bindIP; }
                set { }
            }

            private readonly int port;
            public int Port
            {
                get { return port; }
                set { }
            }

            private Dictionary<IPEndPoint, ClientConnection> clientList = new Dictionary<IPEndPoint, ClientConnection>();

            ~ServerManager()
            {
                Dispose(true);
            }

            public ServerManager(IPAddress bindIP, int port)
            {
                this.bindIP = bindIP;
                this.port = port;
            }

            // Wrap the client events so the server owner can catch them.
            private void client_dataReceived(object sender, IncomingMessageEventArgs e)
            {
                // Check to see if the client is still connected.
                ClientConnection client = (ClientConnection)sender;
                if (!client.Connected || e == null)
                {
                    // The client has disconnected.
                    // Fire the disconnect event and remove it from the client list.
                    clientDisconnected.Invoke(sender, client.RemoteEndPoint);
                    lock (clientList) { clientList.Remove(client.RemoteEndPoint); }

                    if (e == null) { return; } // Don't give listeners of dataReceived a null IncomingMessageEventArgs.
                }

                dataReceived.Invoke(sender, e);
            }

            /// <summary>
            /// Closes all client connections and stops listening. Blocks until all clients are closed.
            /// </summary>
            public void Stop()
            {
                // Politely request that the server stop
                runServer = false;

                // Stop the listener.
                if (listener != null) { listener.Stop(); }

                // Close all client connections and remove the entries from clientList.
                lock (clientList)
                {
                    // Check to see if there are any clients to disconnect.
                    if (clientList.Count == 0)
                    {
                        return;
                    }

                    // Create a list of all keys to remove from the list.
                    IPEndPoint[] removeList = new IPEndPoint[clientList.Keys.Count];
                    clientList.Keys.CopyTo(removeList, 0);

                    // Iterate over clientList with removeList.
                    foreach (IPEndPoint key in removeList)
                    {
                        clientList[key].Close();
                    }
                }
            }

            /// <summary>
            /// Starts listening for incoming client connections on a seperate thread.
            /// </summary>
            public void Start()
            {
                listener = new TcpListener(bindIP, port);
                listener.Start();
                runServer = true;
                ConnectionListen();
            }

            // Starts the listen callback loop.
            private void ConnectionListen()
            {
                listener.BeginAcceptTcpClient(new AsyncCallback(ConnectionListenCallback), listener);
            }
            // This is the callback for a client connecting. If runServer is true, then it calls ConnectionListen() at the end.
            private void ConnectionListenCallback(IAsyncResult ar)
            {
                try
                {
                    if (runServer)
                    {
                        // Cast the callback object to a TcpListener.
                        TcpListener callbackListener = (TcpListener)ar.AsyncState;
                        // Accept pending request.
                        TcpClient client = listener.EndAcceptTcpClient(ar);

                        // Get the remote address to use as a key in the client dictionary.
                        IPEndPoint clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;

                        // Aquire exclusive access to the clientlist as we are on the callback thread.
                        lock (clientList)
                        {
                            //Add the connected client to a new ClientConnection inside clientList.
                            if (clientList.ContainsKey(clientEndPoint))
                            {
                                // First remove if necessary.
                                clientList.Remove(clientEndPoint);
                            }
                            clientList.Add(clientEndPoint, new ClientConnection(client));
                        } // Release here or risk deadlock

                        clientList[clientEndPoint].dataReceived += client_dataReceived; // Attach to the client data event.
                        clientList[clientEndPoint].Connect(); // Start receiving data.
                        clientConnected.Invoke(this, clientEndPoint); // Fire the client connected event.

                        // Begin listening again.
                        ConnectionListen();
                    }
                }
                catch (Exception ex)
                {
                    dataReceived.Invoke(this, new IncomingMessageEventArgs(ex));
                }

            }

            /// <summary>
            /// Closes the specified client connection.
            /// </summary>
            /// <param name="clientIP">The client IP to drop.</param>
            public void DropClient(IPEndPoint clientIP)
            {
                lock (clientList)
                {
                    if (clientList.ContainsKey(clientIP))
                    {
                        clientList[clientIP].Close();
                    }
                }
            }

            /// <summary>
            /// Sends data to the specified client
            /// </summary>
            /// <param name="client">The connected client's IP address.</param>
            /// <param name="message">The byte array to send.</param>
            /// <exception cref="System.ArgumentNullException">The message or client parameter is null.</exception>
            /// <exception cref="System.InvalidArgumentException">The specified client does not exist.</exception>
            /// <exception cref="System.IOException">There was a problem with the network stream.</exception>
            public void SendData(IPEndPoint client, byte[] message)
            {
                if (client == null) { throw new ArgumentNullException("client"); }
                if (message == null) { throw new ArgumentNullException("message"); }

                lock (clientList)
                {
                    if (clientList.ContainsKey(client))
                    {
                        clientList[client].SendDataPacket(ref message);
                    }
                    else
                    {
                        throw new ArgumentException("The specified client does not exist.", "client");
                    }
                }
            }

            /// <summary>
            /// Broadcasts the specified message to all connected clients.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <exception cref="System.ArgumentNullException">The message parameter is null.</exception>
            /// <exception cref="System.IOException">There was a problem with the network stream.</exception>
            public void Broadcast(byte[] message)
            {
                Broadcast(message, 90);
            }
            public void Broadcast(byte[] message, byte messageType)
            {
                // Check this first, or every client will thow one.
                if (message == null) { throw new ArgumentNullException("message"); }

                // Do nothing if no clients.
                if (clientList.Count == 0)
                {
                    return;
                }

                lock (clientList)
                {
                    // Send the message to all clients.
                    System.Threading.Tasks.Parallel.ForEach(clientList, item =>
                    {
                        if (item.Value.Connected)
                        {
                            item.Value.SendDataPacket(ref message, messageType);
                        }
                    });
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <exception cref="System.Net.Sockets.SocketException">An exception was generated while closing the TcpListener.</exception>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // Dispose all managed resources.
                    Stop();
                }

                // Dispose of unmanaged resources here if any.
            }
        }
    }

}
