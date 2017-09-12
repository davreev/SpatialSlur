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
    public abstract class HeVertex<TV, TE, TF> : HeElement, IHeVertex<TV, TE, TF>
        where TV : HeVertex<TV, TE, TF>
        where TE : Halfedge<TV, TE, TF>
        where TF : HeFace<TV, TE, TF>
    {
        private TE _first;


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE FirstOut
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE FirstIn
        {
            get { return _first.Twin; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public sealed override bool IsRemoved
        {
            get { return _first == null; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public int Degree
        {
            get { return _first.CountEdgesAtStart(); }
        }


        /// <summary>
        /// Returns true if the vertex has 1 outgoing halfedge.
        /// Note this is an invalid state.
        /// </summary>
        internal bool IsDegree1
        {
            get { return _first.IsAtDegree1; }
        }


        /// <summary>
        /// Returns true if the vertex has 2 outgoing halfedges.
        /// </summary>
        public bool IsDegree2
        {
            get { return _first.IsAtDegree2; }
        }


        /// <summary>
        /// Returns true if the vertex has 3 outgoing halfedges.
        /// </summary>
        public bool IsDegree3
        {
            get { return _first.IsAtDegree3; }
        }


        /// <summary>
        /// Returns true if the vertex has 4 outgoing halfedges.
        /// </summary>
        public bool IsDegree4
        {
            get { return _first.IsAtDegree4; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsBoundary
        {
            get { return _first.Face == null; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsCorner
        {
            get { return IsBoundary && IsDegree2; }
        }


        /// <summary>
        /// Returns false if the vertex is incident to multiple boundary edges.
        /// </summary>
        public bool IsManifold
        {
            get
            {
                var he = FirstOut;

                // interior vertex, can assume manifold
                if (he.Face != null)
                    return true;

                // boundary vertex, check for second boundary
                he = he.NextAtStart;
                do
                {
                    if (he.Face == null) return false;
                } while (he != FirstOut);

                return true;
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TE> OutgoingHalfedges
        {
            get { return _first.CirculateStart; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TE> IncomingHalfedges
        {
            get { return _first.Twin.CirculateEnd; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TV> ConnectedVertices
        {
            get
            {
                var he0 = FirstIn;
                var he1 = he0;

                do
                {
                    yield return he1.Start;
                    he1 = he1.NextInFace.Twin;
                } while (he1 != he0);
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TF> SurroundingFaces
        {
            get
            {
                var he0 = FirstOut;
                var he1 = he0;

                do
                {
                    var f = he1.Face;
                    if (f != null) yield return f;
                    he1 = he1.NextAtStart;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Flags the vertex for removal.
        /// </summary>
        internal void Remove()
        {
            _first = null;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsConnectedTo(TV other)
        {
            return FindHalfedge(other) != null;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public TE FindHalfedge(TV other)
        {
            var he0 = FirstOut;
            var he1 = he0;

            do
            {
                if (he1.End == other) return he1;
                he1 = he1.NextAtStart;
            } while (he1 != he0);

            return null;
        }


        /// <summary>
        /// Sets the first halfedge of this vertex to the first boundary halfedge encountered during circulation.
        /// Returns true if a boundary halfedge was found.
        /// </summary>
        /// <returns></returns>
        internal bool SetFirstToBoundary()
        {
            var he0 = FirstOut;
            var he1 = he0;

            do
            {
                if (he1.Face == null)
                {
                    FirstOut = he1;
                    return true;
                }

                he1 = he1.NextAtStart;
            } while (he1 != he0);

            return false;
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        bool IHeVertex<TV, TE>.IsDegree1
        {
            get { return IsDegree1; }
        }

        #endregion
    }
}
