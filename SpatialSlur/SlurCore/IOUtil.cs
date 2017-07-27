using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Utility class for stray constants and static methods.
    /// </summary>
    public static class IOUtil
    {
        /// <summary>
        /// Simple binary serialization
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
        /// Simple binary deserialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T DeserializeBinary<T>(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var formatter = new BinaryFormatter();
                return (T) formatter.Deserialize(stream);
            }
        }



        /// <summary>
        /// Simple binary deserialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
    }
}
