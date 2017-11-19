using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    [Serializable]
    public abstract class Halfedge<TSelf> : HeElement<TSelf>
        where TSelf : Halfedge<TSelf>
    {
        private TSelf _twin;
        private TSelf _previous;
        private TSelf _next;


        /// <summary>
        /// 
        /// </summary>
        public TSelf Twin
        {
            get { return _twin; }
            internal set { _twin = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal TSelf Previous
        {
            get { return _previous; }
            set { _previous = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal TSelf Next
        {
            get { return _next; }
            set { _next = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public TSelf Edge
        {
            get { return Self < _twin ? Self : _twin; }
        }
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class Halfedge<V, E> : Halfedge<E>
        where V : HeElement<V, E>
        where E : Halfedge<V, E>
    {
        private V _start;


        /// <summary>
        /// 
        /// </summary>
        public V Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public V End
        {
            get { return Twin._start; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsUnused
        {
            get { return _start == null; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void MakeUnused()
        {
            _start = Twin._start = null;
        }


        /// <summary>
        /// 
        /// </summary>
        internal void UnusedCheck()
        {
            if (IsUnused)
                throw new ArgumentException(UnusedCheckMessage);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class Halfedge<V, E, F> : Halfedge<V, E>
        where V : HeElement<V, E>
        where E : Halfedge<V, E, F>
        where F : HeElement<F, E>
    {
        private F _face;


        /// <summary>
        /// 
        /// </summary>
        public F Face
        {
            get { return _face; }
            internal set { _face = value; }
        }
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class Halfedge<V, E, F, C> : Halfedge<V, E, F>
        where V : HeElement<V, E>
        where E : Halfedge<V, E, F, C>
        where F : HeElement<F, E>
        where C : HeElement<C, E>
    {
        private E _adjacent;
        private E _bundle;
        private C _cell;


        /// <summary>
        /// 
        /// </summary>
        public E Adjacent
        {
            get { return _adjacent; }
            internal set { _adjacent = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public E Bundle
        {
            get { return _bundle; }
            internal set { _bundle = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public C Cell
        {
            get { return _cell; }
            internal set { _cell = value; }
        }
    }
}
