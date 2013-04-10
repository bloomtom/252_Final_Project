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
    public class TheNoiseServer : TcpTransmission.Server.ServerManager
    {
        public TheNoiseServer(IPAddress bindIP, int port)
            : base(bindIP, port)
        {

        }

        // Wrap the client events so the server owner can catch them.
        protected override void client_dataReceived(object sender, IncomingMessageEventArgs e)
        {
            // Check to see if the client is still connected.
            ClientConnection client = (ClientConnection)sender;
            if (!client.Connected || e == null)
            {
                // The client has disconnected.
                // Fire the disconnect event and remove it from the client list.
                OnClientDisconnect(client.RemoteEndPoint);
                lock (clientList) { clientList.Remove(client.RemoteEndPoint); }

                if (e == null) { return; } // Don't give listeners of dataReceived a null IncomingMessageEventArgs.
            }

            switch ((PacketType)e.PacketType)
            {
                case PacketType.AudioSegment:
                    // Ready for next segment.
                    break;
                case PacketType.Authenticate:
                    // Authenticate this client.
                    break;
                case PacketType.Register:
                    // Register new credentials.
                    break;
                case PacketType.StartAudioStream:
                    // Begin streaming the specified file to this client.
                    break;
                case PacketType.StopAudioStream:
                    // Stop streaming to this client.
                    break;
                case PacketType.RequestList:
                    // Send a copy of this client's audio.
                    break;
                default:
                    break;
            }
            OnDataReceived(e);
        }
    }
}
