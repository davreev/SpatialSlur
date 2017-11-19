using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class HeElement
    {
        #region Static

        /// <summary></summary>
        protected const string UnusedCheckMessage = "This element has been flagged for removal. The operation cannot be performed.";

 
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
        /// Returns the index of this element within its parent list.
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
    /// <typeparam name="TSelf"></typeparam>
    [Serializable]
    public abstract class HeElement<TSelf>: HeElement
        where TSelf : HeElement<TSelf>
    {
        private TSelf _self; // cached downcasted ref of this instance (TODO test performance impact)


        /// <summary>
        /// 
        /// </summary>
        public HeElement()
        {
            _self = (TSelf)this;
        }


        /// <summary>
        /// 
        /// </summary>
        internal TSelf Self
        {
            get { return _self; }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class HeElement<TSelf, E> : HeElement<TSelf>
       where TSelf : HeElement<TSelf, E>
       where E : Halfedge<E>
    {
        private E _first;


        /// <summary>
        /// 
        /// </summary>
        public E First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <summary>
        /// 
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
            if (IsUnused)
                throw new ArgumentException(UnusedCheckMessage);
        }
    }
}
