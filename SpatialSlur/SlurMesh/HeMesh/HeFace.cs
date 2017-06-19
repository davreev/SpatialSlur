using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeFace<TV, TE, TF> : HeElement, IHeFace<TV, TE, TF>
        where TV : HeVertex<TV, TE, TF>
        where TE : Halfedge<TV, TE, TF>
        where TF : HeFace<TV, TE, TF>
    {
        private TE _first;


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsRemoved
        {
            get { return _first == null; }
        }


        /// <summary>
        /// Returns true if this face has 1 edge.
        /// Note this is an invalid state and should always return false in client side code.
        /// </summary>
        internal bool IsDegree1
        {
            get { return _first.IsInDegree1; }
        }


        /// <summary>
        /// Returns true if this face has 2 edges.
        /// Note this is an invalid state and should always return false in client side code.
        /// </summary>
        internal bool IsDegree2
        {
            get { return _first.IsInDegree2; }
        }


        /// <summary>
        /// Returns true if this face has 3 edges.
        /// </summary>
        public bool IsDegree3
        {
            get { return _first.IsInDegree3; }
        }


        /// <summary>
        /// Returns true if this face has 3 edges. Same as IsDegree3.
        /// </summary>
        public bool IsTriangle
        {
            get { return _first.IsInDegree3; }
        }
        

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public int Degree
        {
            get { return _first.CountEdgesInFace(); }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsBoundary
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    if (he1.Twin.Face == null) return true;
                    he1 = he1.NextInFace;
                } while (he1 != he0);

                return false;
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TE> Halfedges
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    yield return he1;
                    he1 = he1.NextInFace;
                } while (he1 != he0);
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TV> Vertices
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    yield return he1.Start;
                    he1 = he1.NextInFace;
                } while (he1 != he0);
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TF> AdjacentFaces
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    var f = he1.Twin.Face;
                    if (f != null) yield return f;
                    he1 = he1.NextInFace;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Flags the face for removal.
        /// </summary>
        internal void Remove()
        {
            _first = null;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SetFirstToBoundary()
        {
            var he0 = First;
            var he1 = he0;

            do
            {
                if (he1.Twin.Face == null)
                {
                    First = he1;
                    return true;
                }

                he1 = he1.NextInFace;
            } while (he1 != he0);

            return false;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public TE FindHalfedge(TF other)
        {
            var he0 = First;
            var he1 = he0;

            do
            {
                if (he1.Twin.Face == other) return he1;
                he1 = he1.NextInFace;
            } while (he1 != he0);

            return null;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryEdges()
        {
            var he0 = First;
            var he1 = he0;
            int count = 0;

            do
            {
                if (he1.Twin.Face == null) count++;
                he1 = he1.NextInFace;
            } while (he1 != he0);

            return count;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryVertices()
        {
            var he0 = First;
            var he1 = he0;
            int count = 0;

            do
            {
                if (he1.Start.IsBoundary) count++;
                he1 = he1.NextInFace;
            } while (he1 != he0);

            return count;
        }
    }
}
