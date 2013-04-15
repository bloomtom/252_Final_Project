using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TheNoiseHLC
{
    namespace CommunicationObjects
    {
        [KnownType(typeof(LoginData))]
        [DataContract()] // A data contract makes this class available to the DataContractSerializer for serialization.
        public class LoginData
        {
            [DataMember()]
            public string username;
            [DataMember()]
            public string password;

            public LoginData(string username, string password)
            {
                this.username = username;
                this.password = password;
            }
        }
    }
}
