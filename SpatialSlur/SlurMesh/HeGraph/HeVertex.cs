using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class HeVertex<TV, TE> : HeElement, IHeVertex<TV, TE>
        where TV : HeVertex<TV, TE>
        where TE : Halfedge<TV, TE>
    {
        private TV _self; // cached downcasted ref of this instance (TODO test impact)
        private TE _first;


        /// <summary>
        /// 
        /// </summary>
        public HeVertex()
        {
            _self = (TV)this;
        }


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
        /// Returns true if this vertex has 1 outgoing halfedge.
        /// </summary>
        public bool IsDegree1
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
                var he = FirstOut;

                do
                {
                    yield return he.End;
                    he = he.NextAtStart;
                } while (he != FirstOut);
            }
        }


        /// <summary>
        /// 
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
            var he = FirstOut;

            do
            {
                if (he.End == other) return he;
                he = he.NextAtStart;
            } while (he != FirstOut);

            return null;
        }


        /// <summary>
        /// Inserts the given halfedge at this vertex
        /// </summary>
        /// <param name="hedge"></param>
        internal void Insert(TE hedge)
        {
            hedge.Start = _self;

            if (IsRemoved)
            {
                FirstOut = hedge;
                hedge.MakeConsecutive(hedge);
            }
            else
            {
                var he = FirstOut;
                he.PrevAtStart.MakeConsecutive(hedge);
                hedge.MakeConsecutive(he);
            }
        }


        /// <summary>
        /// Circulates the given halfedge inserting each connected halfedge at this vertex.
        /// </summary>
        /// <param name="hedge"></param>
        internal void InsertRange(TE hedge)
        {
            InsertRange(hedge, hedge);
        }


        /// <summary>
        /// Circulates the given halfedges (exclusive) inserting each connected halfedge at this vertex.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        internal void InsertRange(TE he0, TE he1)
        {
            // set start vertices
            {
                var he2 = he0;

                do
                {
                    he2.Start = _self;
                    he2 = he2.NextAtStart;
                } while (he2 != he1);
            }

            // link into any existing halfedges
            if (IsRemoved)
            {
                FirstOut = he0;
            }
            else
            {
                var he2 = FirstOut;
                var he3 = he1.PrevAtStart; // cache in case he0 and he1 are the same

                he2.PrevAtStart.MakeConsecutive(he0);
                he3.MakeConsecutive(he2);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="compare"></param>
        public void SortOutgoing(Comparison<TE> compare)
        {
            var hedges = OutgoingHalfedges.ToArray();
            Array.Sort(hedges, compare);

            int last = hedges.Length - 1;

            for (int i = 0; i < last; i++)
                hedges[i].MakeConsecutive(hedges[i + 1]);

            hedges[last].MakeConsecutive(hedges[0]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="getPosition"></param>
        /// <param name="normal"></param>
        public void SortOutgoingRadial(Func<TV, Vec3d> getPosition, Vec3d normal)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
