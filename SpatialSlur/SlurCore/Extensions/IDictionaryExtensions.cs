using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Assigns the contents of another dictionary to this one.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <param name="other"></param>
        public static void Set<K, V>(this IDictionary<K, V> source, IDictionary<K, V> other)
        {
            foreach (var pair in other)
                source[pair.Key] = pair.Value;
        }
    }
}
