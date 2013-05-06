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
    /// Handles TheNoise server specific client serving capability.
    /// </summary>
    internal sealed class TheNoiseClientHandler : IDisposable
    {
        public delegate void AudioPacketEventHandler(IPEndPoint sender, AudioPacketEventArgs e);
        public event AudioPacketEventHandler AudioPacketReady = delegate { };

        public delegate void TrackListUpdatedEventHandler(IPEndPoint sender, TrackList e);
        public event TrackListUpdatedEventHandler TrackListUpdated = delegate { };

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

        private bool streamedAudio = false;
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
            streamedAudio = true;
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

                TrackListUpdated.Invoke(tcpEndPoint, new TrackList(tracks));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Close all managed resources.
                if (streamedAudio) { EndStreaming(); }
            }

            // Dispose of unmanaged resources here if any.
        }
    }
}
