using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Provides an alternate view of the halfedge list which exposes the first halfedge in each pair.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public class HeEdgeList<E> : IHeElementList<E>
        where E : HeElement, IHalfedge<E>
    {
        private HalfedgeList<E> _hedges;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedges"></param>
        internal HeEdgeList(HalfedgeList<E> halfedges)
        {
            _hedges = halfedges;
        }


        /// <inheritdoc/>
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


        /// <inheritdoc/>
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


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountRemoved()
        {
            int result = 0;

            for (int i = 0; i < _hedges.Count; i += 2)
                if (_hedges[i].IsRemoved) result++;

            return result;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public void CompactAttributes<U>(List<U> attributes)
        {
            int marker = SwimAttributes(attributes);
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public int SwimAttributes<U>(IList<U> attributes)
        {
            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                if (_hedges[i].IsRemoved) continue; // skip unused elements
                attributes[marker++] = attributes[i >> 1];
            }

            return marker;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public bool Contains(E hedge)
        {
            return _hedges.Contains(hedge);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <returns></returns>
        public IEnumerable<E> GetDistinct(IEnumerable<E> hedges)
        {
            foreach (var he in hedges)
                _hedges.ContainsCheck(he);

            return GetDistinctImpl(hedges);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <returns></returns>
        internal IEnumerable<E> GetDistinctImpl(IEnumerable<E> hedges)
        {
            int currTag = _hedges.NextTag;

            foreach (var he in hedges.Select(he => he.Older))
            {
                if (he.Tag == currTag) continue;
                he.Tag = currTag;
                yield return he;
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        public IEnumerable<E> GetUnion(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            return GetDistinct(hedgesA.Concat(hedgesB));
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        public IEnumerable<E> GetDifference(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            foreach (var he in hedgesA.Concat(hedgesB))
                _hedges.ContainsCheck(he);

            return GetDifferenceImpl(hedgesA, hedgesB);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        internal IEnumerable<E> GetDifferenceImpl(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            int currTag = _hedges.NextTag;

            // tag elements in A
            foreach (var heB in hedgesB.Select(he => he.Older))
                heB.Tag = currTag;

            // return untagged elements in B
            foreach (var heA in hedgesA.Select(he => he.Older))
                if (heA.Tag != currTag) yield return heA;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        public IEnumerable<E> GetIntersection(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            foreach (var he in hedgesA.Concat(hedgesB))
                _hedges.ContainsCheck(he);

            return GetIntersectionImpl(hedgesA, hedgesB);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgesA"></param>
        /// <param name="hedgesB"></param>
        /// <returns></returns>
        internal IEnumerable<E> GetIntersectionImpl(IEnumerable<E> hedgesA, IEnumerable<E> hedgesB)
        {
            int currTag = _hedges.NextTag;

            // tag elements in A
            foreach (var heA in hedgesA.Select(he => he.Older))
                heA.Tag = currTag;

            // return tagged elements in B
            foreach (var heB in hedgesB.Select(he => he.Older))
                if (heB.Tag == currTag) yield return heB;
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
