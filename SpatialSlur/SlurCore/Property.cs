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
    public static class Property
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="get"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public static Property<T, V> Create<T, V>(Func<T, V> get, Action<T, V> set)
        {
            return new Property<T, V>(get, set);
        }
    }


    /// <summary>
    /// Compound delegate for getting/setting a value V via an instance of T.
    /// </summary>
    public struct Property<T, V> : IProperty<T, V>
    {
        /// <summary></summary>
        public readonly Func<T, V> Get;
        /// <summary></summary>
        public readonly Action<T, V> Set;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="get"></param>
        /// <param name="set"></param>
        public Property(Func<T, V> get, Action<T, V> set)
        {
            Get = get ?? throw new ArgumentNullException();
            Set = set ?? throw new ArgumentNullException();
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        V IReadOnlyProperty<T, V>.Get(T item)
        {
            return Get(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        void IProperty<T, V>.Set(T item, V value)
        {
            Set(item, value);
        }

        #endregion
    }
}
