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

                public Track(string trackName, long trackLength, TrackType type)
                {
                    this.trackName = trackName;
                    this.trackLength = trackLength;
                    this.type = type;
                }
            }

            [KnownType(typeof(TrackStreamRequest))]
            [DataContract]
            public class TrackStreamRequest
            {
                [DataMember]
                private string trackName;
                public string TrackName
                {
                    get { return trackName; }
                    set { }
                }

                [DataMember]
                private long startPos;
                public long StartPos
                {
                    get { return StartPos; }
                    set { }
                }

                public TrackStreamRequest(string trackName, long startPos)
                {
                    this.trackName = trackName;
                    this.startPos = startPos;
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