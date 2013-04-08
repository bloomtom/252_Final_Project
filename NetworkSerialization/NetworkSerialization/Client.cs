/*
 * Name:    Thomas Bloom
 * Class:   CP-252
 * Project: 252 Final Project
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace EventDrivenNetworking
{
    namespace Client
    {
        /// <summary>
        /// An event based interface for a TCP connection.
        /// </summary>
        /// <remarks>
        /// Implements IDisposable.
        /// 
        /// Instances of this class are used by ThreadedNetworking.Server.ServerManager to keep track of it's connections.
        /// </remarks>
        /// <example>
        /// // Create a new connection.
        /// ClientConnection client = new ClientConnection(127.0.0.1, 8000);
        /// 
        /// // Attach to the received data event.
        /// client.dataReceived += client_dataReceived;
        /// 
        /// // Connect to the client.
        /// client.Connect();
        /// 
        /// // Send some data.
        /// client.SendData(someBuffer);
        /// </example>
        /// <seealso cref="EventDrivenNetworking.Server.ServerManager"/>
        /// <seealso cref="EventDrivenNetworking.IncomingMessageEventArgs"/>
        public class ClientConnection : IDisposable
        {
            public delegate void DataReceivedEventHandler(object sender, IncomingMessageEventArgs e);
            public event DataReceivedEventHandler dataReceived = delegate { };

            private TcpClient client; // The internal client.
            NetworkStream tcpStream; // Access to the underlying stream of client.

            public bool Connected
            {
                get { return client.Connected; }
                set { }
            }

            const int readSize = 2048; // Size of the read buffer
            byte[] readBuffer;

            int packetFilled; // The amount of data currently filled into packetBuffer.
            byte[] packetBuffer; // Storage for an entire (up to 64k in length) object packet.

            private readonly IPEndPoint remoteEndPoint;
            public IPEndPoint RemoteEndPoint
            {
                get { return remoteEndPoint; }
                set { }
            }

            // When this is set to false, the read loop is ended on the next read completion.
            private volatile bool runClient = false;

            ~ClientConnection()
            {
                Dispose(false);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ClientConnection"/> class using an already accepted TcpClient connection.
            /// </summary>
            /// <param name="client">An accepted client connection.</param>
            public ClientConnection(TcpClient client)
            {
                this.client = client;
                remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ClientConnection"/> class.
            /// </summary>
            /// <param name="ip">IP address of the target machine.</param>
            /// <param name="port">Port to use for the connection.</param>
            /// <exception cref="System.Net.Sockets.SocketException">Could not connect to the target machine.</exception>
            public ClientConnection(IPAddress ip, int port)
            {
                remoteEndPoint = new IPEndPoint(ip, port);

                // Create a new client from the passed address and port.
                this.client = new TcpClient();
                client.ReceiveTimeout = 1500;
                client.SendTimeout = 1500;
                client.Connect(ip, port);
            }

            /// <summary>
            /// Opens a connection to the remote machine and begins listening to it.
            /// </summary>
            public void Connect()
            {
                if (!runClient)
                {
                    // Raise run flag
                    runClient = true;

                    // Initialize the buffer.
                    readBuffer = new byte[readSize];

                    // If the client is not connected, connect it.
                    if (!client.Connected) { client.Connect(remoteEndPoint); }
                    // Then get the client's network stream.
                    tcpStream = client.GetStream();

                    // Start the callback listen loop.
                    ClientListen();
                }
            }

            /// <summary>
            /// Closes the connection and cleans up resources.
            /// </summary>
            public void Close()
            {
                // Tell the read loop to stop.
                runClient = false;

                // Cleanup connections.
                if (tcpStream != null) { tcpStream.Close(); }
                if (client != null) { client.Close(); }

                //Cleanup other resources.
                readBuffer = null;

                // Give listeners the disconnect event.
                if (dataReceived != null) { dataReceived.Invoke(this, null); }
            }

            // Calling this begins the callback listen loop.
            private void ClientListen()
            {
                if (tcpStream.CanRead)
                {
                    // Start an async read with ClientListenCallback.
                    tcpStream.BeginRead(readBuffer, 0, readSize, new AsyncCallback(ClientListenCallback), tcpStream);
                }
            }

            // This is called when a BeginRead is completed.
            // If runClient is true, data is read and another read is started.
            private void ClientListenCallback(IAsyncResult ar)
            {
                try
                {
                    if (runClient)
                    {
                        // Get the actual size of data read.
                        System.IO.Stream resultStream = (System.IO.Stream)ar.AsyncState;
                        int bytesRead = resultStream.EndRead(ar);

                        // This indicates the connection is being closed.
                        if (bytesRead == 0)
                        {
                            Close();
                            return;
                        }

                        if (packetBuffer == null) // Check to see if start of new packet.
                        {
                            if (bytesRead < 4) { return; } // Not enough data for a packet header. Wait for the next go.


                        }

                        //// Quick check to see if the buffer was filled completely.
                        //else if (bytesRead != readSize)
                        //{
                        //    // Create a properly sized buffer and copy data to it.
                        //    byte[] actualBuffer = new byte[bytesRead];
                        //    Array.Copy(readBuffer, actualBuffer, bytesRead);
                        //    dataReceived.Invoke(this, new IncomingMessageEventArgs(actualBuffer)); // Fire the data received event.
                        //}
                        //else
                        //{
                        //    // No need to copy data to a new buffer since the original was filled.
                        //    dataReceived.Invoke(this, new IncomingMessageEventArgs(readBuffer)); // Fire the data received event.
                        //}
                        ClientListen(); // Listen for more data.
                    }
                }
                catch (Exception ex)
                {
                    // Tell anyone listening that something bad happened.
                    dataReceived.Invoke(this, new IncomingMessageEventArgs(ex));
                }
            }

            /// <summary>
            /// Synchronously sends data over the client network stream.
            /// This method is deprecated in the public API. Consider using SendDataPacket instead.
            /// </summary>
            /// <param name="data">The data buffer to send</param>
            /// <exception cref="System.ArgumentNullException">The message parameter is null.</exception>
            /// <exception cref="System.IOException">There was a problem with the network stream.</exception>
            private void SendData(ref byte[] data)
            {
                // Attempt to send the passed data over the tcp connection.
                try
                {
                    if (!runClient) { return; } // Silently return if we are stopping.
                    if (PollConnection()) // Check the connection before sending data across it.
                    {
                        tcpStream.Write(data, 0, data.Length); // Send data
                    }
                }
                catch (Exception ex)
                {
                    // Pass error up the ladder.
                    dataReceived.Invoke(this, new IncomingMessageEventArgs(ex));
                }
            }

            /// <summary>
            /// Synchronously sends a formatted packet of data over the network stream.
            /// </summary>
            /// <param name="data">The data buffer to send</param>
            /// <exception cref="System.ArgumentNullException">The message parameter is null.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The message parameter is greater than 65535 in length.</exception>
            /// <exception cref="System.IOException">There was a problem with the network stream.</exception>
            public void SendDataPacket(ref byte[] data)
            {
                if (data.Length > 65535) { throw new ArgumentOutOfRangeException("data", "Parameter cannot be greater than 65535 in length."); }
                ushort dataLen = (ushort)data.Length;

                byte[] header = new byte[4];
                header[0] = 1; // SOH
                header[1] = 90; // Reserved packet type
                header[2] = (byte)(dataLen >> 8); // Store the top byte
                header[3] = (byte)(dataLen & 0x00FF); // Store the bottom byte

                SendData(ref header);
                SendData(ref data);
            }

            /// <summary>
            /// Polls the connection for close signals.
            /// </summary>
            /// <returns>True if connected.</returns>
            public bool PollConnection()
            {
                try
                {
                    return !(client.Client.Poll(1, SelectMode.SelectRead) && client.Client.Available == 0);
                }
                catch (SocketException) { return false; }
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
                    Close();
                }

                // Dispose of unmanaged resources here if any.
            }
        }
    }
}
