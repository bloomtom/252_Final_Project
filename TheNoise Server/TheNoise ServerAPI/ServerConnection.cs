using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TcpTransmission;
using TcpTransmission.Client;

namespace TheNoiseAPI
{
    public class ServerConnection : IDisposable
    {
        private ClientConnection client;

        public ServerConnection(IPAddress ip, int port)
        {
            client = new ClientConnection(ip, port);
        }

        public int Register()
        {
            return 0;
        }

        public int Authenticate()
        {
            return 0;
        }

        public void RequestAudioList()
        {
            return;
        }

        public void RequestBeginAudioStream(string trackName)
        {
            return;
        }

        public void RequestEndAudioStream()
        {
            return;
        }

        public void OpenConnection()
        {
            client.Connect();
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
