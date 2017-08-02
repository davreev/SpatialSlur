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
    public class Property<T, V>
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
    }


    /*
    /// <summary>
    /// Wrapper class containing a pair of delegates for getting/setting a value V via an item of T.
    /// </summary>
    public class Property<T, V> : IProperty<T, V>
    {
        private Func<T, V> _get;
        private Action<T, V> _set;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="get"></param>
        /// <param name="set"></param>
        public Property(Func<T, V> get, Action<T, V> set)
        {
            _get = get ?? throw new ArgumentNullException();
            _set = set ?? throw new ArgumentNullException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public V Get(T item)
        {
            return _get(item);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        public void Set(T item, V value)
        {
            _set(item, value);
        }
    }
    */
}
