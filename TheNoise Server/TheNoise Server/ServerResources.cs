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
            else if (SubjectUsername != null && SubjectIPE != null)
            {
                return String.Format("Client {0} at {1} {2}", SubjectUsername, SubjectIPE, EventMessage);
            }
            else if (SubjectUsername != null)
            {
                return String.Format("{0}: {1}", SubjectUsername, EventMessage);
            }
            else if (SubjectIPE != null)
            {
                return String.Format("Client at {0} {1}", SubjectIPE, EventMessage);
            }


            return base.ToString(); // Why would this ever be hit?
        }
    }
}
