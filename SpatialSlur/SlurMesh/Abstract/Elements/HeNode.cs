using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class HeNode<T, E> : HeElement<T>
       where T : HeNode<T, E>
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
            const string errorMessage = "This element is not in use. The operation cannot be performed.";

            if (IsUnused)
                throw new ArgumentException(errorMessage);
        }
    }
}
