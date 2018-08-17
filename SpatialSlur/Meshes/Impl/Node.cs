

/*
 * Notes
 * 
 * Abstract/virtual methods are avoided at the element level to allow for inline optimization.
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SpatialSlur.Collections;

namespace SpatialSlur.Meshes.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class Node
    {
        #region Static Members

        /// <summary>
        /// Implicitly converts an element to its index for convenience.
        /// </summary>
        /// <param name="element"></param>
        public static implicit operator int(Node element)
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
    public abstract class Node<T> : Node
        where T : Node<T>
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<T, U> CreateProperty<U>(U[] values)
        {
            return new Property<T, U>(t => values[t], (t, u) => values[t] = u);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<T, U> CreateProperty<U>(ArrayView<U> values)
        {
            return new Property<T, U>(t => values[t], (t, u) => values[t] = u);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<T, U> CreateProperty<U>(IList<U> values)
        {
            return new Property<T, U>(t => values[t], (t, u) => values[t] = u);
        }

        #endregion


        private T _self; // cached downcasted ref of this instance


        /// <summary>
        /// 
        /// </summary>
        public Node()
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


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class Node<T, E> : Node<T>
       where T : Node<T, E>
       where E : Halfedge<E>
    {
        private E _first;


        /// <summary>
        /// Returns the first halfedge at this node.
        /// </summary>
        public E First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <summary>
        /// Returns true if this element is not being used by the halfedge structure i.e. its first halfedge is null.
        /// </summary>
        public bool IsUnused
        {
            get { return _first == null; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void MakeUnused()
        {
            _first = null;
        }


        /// <summary>
        /// 
        /// </summary>
        internal void UnusedCheck()
        {
            const string errorMessage = "This node is not in use. The operation cannot be performed.";

            if (IsUnused)
                throw new ArgumentException(errorMessage);
        }
    }
}
