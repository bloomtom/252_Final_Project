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
        /// <summary>
        /// Serializes the specified object into an array of bytes.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="serialized">A byte array containing the serialized object.</param>
        public void Serialize(object obj, out byte[] serialized) // Serializes an employee[] object into a string
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType()); // Create a serialization object.
                serializer.WriteObject(ms, obj); // Serialize the passed array of employee.
                ms.Seek(0, SeekOrigin.Begin); // Seek the stream to the start.
                serialized = ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the specified serialized object array.
        /// </summary>
        /// <param name="serialized">The serialized object array</param>
        /// <param name="type">The type of object the serilized array represents</param>
        /// <returns>The uncasted deserialized object.</returns>
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