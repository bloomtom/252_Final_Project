using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TheNoiseHLC
{
    namespace CommunicationObjects
    {
        namespace AudioTrack
        {
            [DataContract]
            public enum TrackType
            {
                [EnumMember]
                Unspecified = 0,
                [EnumMember]
                GsmEncoding = 1,
                [EnumMember]
                Uncompressed = 2
            }

            [KnownType(typeof(Track))]
            [DataContract]
            public class Track
            {
                [DataMember]
                private string trackName;
                public string TrackName
                {
                    get { return trackName; }
                    set { }
                }

                [DataMember]
                private string trackExtension;
                public string TrackExtension
                {
                    get { return trackExtension; }
                    set {  }
                }

                [DataMember]
                private long trackLength;
                public long TrackLength
                {
                    get { return trackLength; }
                    set { }
                }

                [DataMember]
                private TrackType type;
                public TrackType Type
                {
                    get { return type; }
                    set { }
                }

                public Track(string trackName, string trackExtension, long trackLength, TrackType type)
                {
                    this.trackName = trackName;
                    this.trackExtension = trackExtension;
                    this.trackLength = trackLength;
                    this.type = type;
                }

                public override string ToString()
                {
                    TimeSpan t = TimeSpan.FromSeconds(trackLength);
                    string trackTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
                    return trackName + " (" + trackTime.ToString() + ")";
                }
            }

            [KnownType(typeof(TrackStreamRequest))]
            [DataContract]
            public class TrackStreamRequest
            {
                [DataMember]
                private Track track;
                public Track Track
                {
                    get { return track; }
                    set { }
                }

                [DataMember]
                private long startPos;
                public long StartPos
                {
                    get { return StartPos; }
                    set { }
                }

                [DataMember]
                private System.Net.IPEndPoint connection;
                public System.Net.IPEndPoint Connection
                {
                    get { return connection; }
                    set { }
                }

                public TrackStreamRequest(Track trackName, long startPos, System.Net.IPEndPoint connection)
                {
                    this.track = trackName;
                    this.startPos = startPos;
                    this.connection = connection;
                }
            }

            [KnownType(typeof(TrackList))]
            [DataContract]
            public class TrackList : EventArgs
            {
                [DataMember()]
                private Track[] tracks;
                public Track[] Tracks
                {
                    get { return tracks; }
                    set { }
                }

                public TrackList(Track[] tracks)
                {
                    this.tracks = tracks;
                }
            }
        }
    }
}