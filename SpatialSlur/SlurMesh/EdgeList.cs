

/*
 * Notes
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Provides an alternate view of the halfedge list which only considers the first halfedge in each pair.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class EdgeList<E> : IReadOnlyList<E>
        where E : Halfedge<E>
    {
        private HalfedgeList<E> _hedges;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedges"></param>
        internal EdgeList(HalfedgeList<E> halfedges)
        {
            _hedges = halfedges;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _hedges.Count >> 1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get { return _hedges.Capacity >> 1; }
        }


        /// <summary>
        /// Returns the first halfedge of the edge at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public E this[int index]
        {
            get { return _hedges[index << 1]; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<E> GetEnumerator()
        {
            for (int i = 0; i < _hedges.Count; i += 2)
                yield return _hedges[i];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountUnused()
        {
            return _hedges.CountUnused() >> 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="attributes"></param>
        public void CompactAttributes<A>(List<A> attributes)
        {
            int marker = SwimAttributes(attributes);
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="attributes"></param>
        public int SwimAttributes<A>(IList<A> attributes)
        {
            int marker = 0;

            for (int i = 0; i < Count; i++)
            {
                if (this[i].IsUnused) continue; // skip unused elements
                attributes[marker++] = attributes[i];
            }

            return marker;
        }


        /// <summary>
        /// Returns true if the given edge belongs to this mesh.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public bool Owns(E hedge)
        {
            return _hedges.Owns(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        internal void OwnsCheck(E hedge)
        {
            _hedges.OwnsCheck(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <returns></returns>
        public IEnumerable<E> GetDistinct(IEnumerable<E> hedges)
        {
            return _hedges.GetDistinct(hedges.Select(he => he.Edge));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <returns></returns>
        internal IEnumerable<E> GetDistinctImpl(IEnumerable<E> hedges)
        {
            return _hedges.GetDistinctImpl(hedges.Select(he => he.Edge));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        public IEnumerable<E> GetUnion(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            return _hedges.GetUnion(
                hedgesA.Select(he => he.Edge), 
                hedgesB.Select(he => he.Edge)
                );
        }

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        public IEnumerable<E> GetDifference(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            return _hedges.GetDifference(
                hedgesA.Select(he => he.Edge),
                hedgesB.Select(he => he.Edge)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        internal IEnumerable<E> GetDifferenceImpl(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            return _hedges.GetDifferenceImpl(
                hedgesA.Select(he => he.Edge),
                hedgesB.Select(he => he.Edge)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        public IEnumerable<E> GetIntersection(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            return _hedges.GetIntersection(
                hedgesA.Select(he => he.Edge),
                hedgesB.Select(he => he.Edge)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        internal IEnumerable<E> GetIntersectionImpl(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            return _hedges.GetIntersectionImpl(
                hedgesA.Select(he => he.Edge),
                hedgesB.Select(he => he.Edge)
                );
        }


        /// <summary>
        /// Reorders edges based on the given key.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="getKey"></param>
        public void Sort<K>(Func<E, K> getKey)
            where K : IComparable<K>
        {
            _hedges.Sort(getKey);
        }


        #region Explicit interface implementations

        /// <summary>
        /// Explicit implementation of non-generic method.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // call generic version
        }

        #endregion
    }
}
