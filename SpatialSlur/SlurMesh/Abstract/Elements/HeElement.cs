

/*
 * Notes
 * 
 * Abstract/virtual implementations are avoided on element level methods and properties to allow for inline optimization.
 */

using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class HeElement
    {
        #region Static

        /// <summary>
        /// Implicitly converts an element to its index for convenience.
        /// </summary>
        /// <param name="element"></param>
        public static implicit operator int(HeElement element)
        {
            return element.Index;
        }

        #endregion


        private int _index = -1;
        private int _tag;


        /// <summary>
        /// Returns the position of this element within the corresponding element list.
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }


        /// <summary>
        /// General purpose tag used internally for topological searches and validation.
        /// </summary>
        internal int Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class HeElement<T> : HeElement
        where T : HeElement<T>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<T, V> CreateProperty<V>(V[] values)
        {
            return new Property<T, V>(t => values[t], (t, v) => values[t] = v);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<T, V> CreateProperty<V>(List<V> values)
        {
            return new Property<T, V>(t => values[t], (t, v) => values[t] = v);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<T, V> CreateProperty<V>(IList<V> values)
        {
            return new Property<T, V>(t => values[t], (t, v) => values[t] = v);
        }

        #endregion


        private T _self; // cached downcasted ref of this instance (TODO test performance impact)


        /// <summary>
        /// 
        /// </summary>
        public HeElement()
        {
            _self = (T)this;
        }


        /// <summary>
        /// 
        /// </summary>
        internal T Self
        {
            get { return _self; }
        }
    }
}
