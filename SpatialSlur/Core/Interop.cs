
/*
 * Notes
 */

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;

namespace SpatialSlur
{
    /// <summary>
    /// Static methods for importing from and exporting to external formats.
    /// </summary>
    public static partial class Interop
    {
        /// <summary>
        /// Binary serialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="path"></param>
        public static void SerializeBinary<T>(T item, string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, item);
            }
        }


        /// <summary>
        /// Binary deserialization
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object DeserializeBinary(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }


        /// <summary>
        /// Binary deserialization
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T DeserializeBinary<T>(string path)
        {
            return (T)DeserializeBinary(path);
        }


        /// <summary>
        /// Json serialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="path"></param>
        public static void SerializeJson<T>(T item, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, item);
            }
        }


        /// <summary>
        /// Json deserialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T DeserializeJson<T>(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}