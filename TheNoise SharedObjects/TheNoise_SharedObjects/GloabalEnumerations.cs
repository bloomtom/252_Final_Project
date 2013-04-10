using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TheNoise_SharedObjects
{
    namespace GlobalEnumerations
    {
        [DataContract]
        public enum UserAddResult : byte
        {
            [EnumMember]
            UnknownResult = 0,
            [EnumMember]
            Success = 1,
            [EnumMember]
            AlreadyExists = 2,
            [EnumMember]
            UsernameTooLong = 3,
            [EnumMember]
            InvalidPassword = 4
        }

        [DataContract]
        public enum UserAuthenticationResult : byte
        {
            [EnumMember]
            UnknownResult = 0,
            [EnumMember]
            Success = 1,
            [EnumMember]
            InvalidUser = 2,
            [EnumMember]
            InvalidPassword = 3
        }

        public enum PacketType : byte
        {
            // Login types
            Authenticate = 0,

            // Registration types
            Register = 2,

            // Audio streaming types
            RequestList = 4,
            StartAudioStream = 5,
            StopAudioStream = 6,
            AudioSegment = 7,

            // The default packet type within TcpNetworking
            UnknownPacket = 90
        }
    }
}
