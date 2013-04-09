using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TcpTransmission
{
    public class ObjectSerialization
    {
        public byte[] Serialize(object obj) // Serializes an employee[] object into a string
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType()); // Create a serialization object.
                serializer.WriteObject(ms, obj); // Serialize the passed array of employee.
                ms.Seek(0, SeekOrigin.Begin); // Seek the stream to the start.
                return ms.ToArray();
            }
        }

        public object Deserialize(byte[] serialized, Type type)
        {
            using (MemoryStream ms = new MemoryStream(serialized))
            {
                DataContractSerializer serializer = new DataContractSerializer(type); // Create the serialization object.
                return serializer.ReadObject(ms); // Deserialize and return result.
            }
        }
    }
}