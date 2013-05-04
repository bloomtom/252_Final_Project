/*
 * Name:    Thomas Bloom
 * Class:   CP-252
 * Project: 252 Final Project
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace TcpTransmission
{
    namespace Client
    {
        /// <summary>
        /// An event based interface for a TCP connection that supports indication of privilege through IsAuthenticated.
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

            /// <summary>
            /// Checks the Connected property of the underlying TcpClient
            /// </summary>
            public bool Connected
            {
                get { return (client.Client != null && client.Connected); }
                set { }
            }

            const int readSize = 4096; // Size of the read buffer
            byte[] readBuffer;

            int packetFilled; // The amount of data currently filled into packetBuffer.
            byte packetType;
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
                InternalConnect(3000);
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
                    if (!client.Connected)
                    {
                        InternalConnect(3000);
                    }
                    // Then get the client's network stream.
                    tcpStream = client.GetStream();

                    // Start the callback listen loop.
                    ClientListen();
                }
            }

            private void InternalConnect(int timeout)
            {
                // Attempt to connect with a 3 second timeout.
                IAsyncResult result = client.BeginConnect(remoteEndPoint.Address, remoteEndPoint.Port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(timeout, true);
                if (!success)
                {
                    client.Close();
                    throw new TimeoutException("Connection to the remote endpoint timed out.");
                }
                //client.Connect(remoteEndPoint);
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
                        int streamLength = resultStream.EndRead(ar);

                        // This indicates the connection is being closed.
                        if (streamLength == 0)
                        {
                            Close();
                            return;
                        }

                        using (System.IO.MemoryStream incomingData = new System.IO.MemoryStream(readBuffer))
                        {
                            // Check to see if start of new packet.
                            if (packetBuffer == null)
                            {
                                if (StartPacket(incomingData, streamLength) == false) { return; }
                            }

                            ReadPackets(incomingData, streamLength);
                        }

                        ClientListen(); // Listen for more data.
                    }
                }
                catch (Exception ex)
                {
                    // Tell anyone listening that something bad happened.
                    dataReceived.Invoke(this, new IncomingMessageEventArgs(ex));
                }
            }

            private bool StartPacket(System.IO.Stream netStream, int streamLength)
            {
                if ((streamLength - netStream.Position) < 4) { return false; } // Not enough data for a packet header.

                // Check header for validity
                int Soh = netStream.ReadByte();
                if (Soh != 1)
                {
                    // Invalid packet or synchronization error.
                    // Sync to nearest correct header byte.
                    while (Soh != -1 || Soh != 1) // -1 is end of stream
                    {
                        Soh = netStream.ReadByte();
                    }

                    dataReceived.Invoke(this, new IncomingMessageEventArgs(new FormatException("Header corruption detected.")));

                    if (Soh != 1) { return false; } // End of stream reached without getting a header byte.
                }

                // Read rest of the header from the stream
                byte[] packetHeader = new byte[3];
                netStream.Read(packetHeader, 0, 3);

                // Get packet type and packet size
                packetType = packetHeader[0];
                int packetSize = (packetHeader[1] << 8) + packetHeader[2]; // Merge the two length bytes into one.

                if (packetSize == 0)
                {
                    // Received a packet with no data payload. Fire event with packet type specified.
                    dataReceived.Invoke(this, new IncomingMessageEventArgs(null, packetType));
                    return false; // The caller doesn't need to know a packet was processed.
                }
                else
                {
                    // Create the packet buffer. The caller will proceed to read the data packet.
                    packetBuffer = new byte[packetSize];
                }

                return true;
            }

            private void ReadPackets(System.IO.Stream netStream, int streamLength)
            {
                // Check to see if the entire packet can be read at once.
                if ((streamLength - netStream.Position) >= (packetBuffer.Length - packetFilled))
                {
                    // Read to the end of packet.
                    Array.ConstrainedCopy(readBuffer, (int)netStream.Position, packetBuffer, packetFilled, (packetBuffer.Length - packetFilled));

                    dataReceived.Invoke(this, new IncomingMessageEventArgs(packetBuffer, packetType));

                    int packetLengthOffset = packetBuffer.Length;
                    packetBuffer = null;
                    packetType = 90;

                    // Start reading another packet if available.
                    if (StartPacket(netStream, streamLength - packetLengthOffset))
                    {
                        ReadPackets(netStream, streamLength - packetLengthOffset);
                    }
                }
                else
                {
                    // Read available into the packet buffer.
                    Array.ConstrainedCopy(readBuffer, (int)netStream.Position, packetBuffer, packetFilled, (int)(streamLength - netStream.Position));
                    packetFilled += (int)(streamLength - netStream.Position);
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
                    if (!runClient) { throw new InvalidOperationException("Cannot send data because the client connection is not open."); } // Silently return if we are stopping.
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
            /// Synchronously sends a typed and formatted packet of data over the network stream.
            /// </summary>
            /// <param name="data">The data buffer to send</param>
            /// <param name="packetType">Number indicating to the received what type the packet is.</param>
            /// <exception cref="System.ArgumentNullException">The message parameter is null.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The message parameter is greater than 65535 in length.</exception>
            /// <exception cref="System.IOException">There was a problem with the network stream.</exception>
            public void SendDataPacket(ref byte[] data, byte packetType)
            {
                ushort dataLen = 0;
                if (data != null)
                {
                    if (data.Length > 65535) { throw new ArgumentOutOfRangeException("data", "Parameter cannot be greater than 65535 in length."); }
                    dataLen = (ushort)data.Length;
                }

                byte[] header = new byte[4];
                header[0] = 1; // SOH
                header[1] = packetType;

                if (dataLen != 0)
                {
                    // There is data to send. Store data length in header and send header+data.
                    header[2] = (byte)(dataLen >> 8); // Store the top byte
                    header[3] = (byte)(dataLen & 0x00FF); // Store the bottom byte

                    SendData(ref header); // Send the header.
                    SendData(ref data);
                }
                else
                {
                    // No data to send, only send header.
                    header[2] = 0; // Store the top byte
                    header[3] = 0; // Store the bottom byte

                    SendData(ref header); // Send the header.
                }
            }

            /// <summary>
            /// Synchronously sends an untyped formatted packet of data over the network stream.
            /// </summary>
            /// <param name="data">The data buffer to send</param>
            /// <exception cref="System.ArgumentNullException">The message parameter is null.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">The message parameter is greater than 65535 in length.</exception>
            /// <exception cref="System.IOException">There was a problem with the network stream.</exception>
            public void SendDataPacket(ref byte[] data) { SendDataPacket(ref data, 90); }

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
