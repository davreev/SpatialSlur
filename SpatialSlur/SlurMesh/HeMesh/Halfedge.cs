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
    public class Halfedge<TV, TE, TF> : HeElement, IHalfedge<TV, TE, TF>
        where TV : HeVertex<TV, TE, TF>
        where TE : Halfedge<TV, TE, TF>
        where TF : HeFace<TV, TE, TF>
    {
        private TE _self; // cached downcasted ref of this instance (TODO test impact)
        private TE _prev;
        private TE _next;
        private TE _twin;
        private TV _start;
        private TF _face;


        /// <summary>
        /// 
        /// </summary>
        public Halfedge()
        {
            _self = (TE)this;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE Twin
        {
            get { return _twin; }
            internal set { _twin = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE PrevInFace
        {
            get { return _prev; }
            internal set { _prev = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE NextInFace
        {
            get { return _next; }
            internal set { _next = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE PrevAtStart
        {
            get { return _prev._twin; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE NextAtStart
        {
            get { return _twin._next; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TV Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TV End
        {
            get { return _twin._start; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TF Face
        {
            get { return _face; }
            internal set { _face = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsRemoved
        {
            get { return _start == null; }
        }


        /// <summary>
        /// Returns true if the halfedge and its twin have different faces.
        /// This should always return true in client side code.
        /// </summary>
        internal bool IsValid
        {
            get { return _face != _twin._face; }
        }


        /// <summary>
        /// Returns true if this halfedge starts at a degree 1 vertex.
        /// Note this should always return false in client side code.
        /// </summary>
        internal bool IsAtDegree1
        {
            get { return this == NextAtStart; }
        }


        /// <summary>
        /// Returns true if this halfedge starts at a degree 2 vertex.
        /// </summary>
        public bool IsAtDegree2
        {
            get { return this == NextAtStart.NextAtStart; }
        }


        /// <summary>
        /// Returns true if this halfedge starts at a degree 3 vertex.
        /// </summary>
        public bool IsAtDegree3
        {
            get { return this == NextAtStart.NextAtStart.NextAtStart; }
        }


        /// <summary>
        /// Returns true if this halfedge is in a one-sided face/hole.
        /// Note this should always return false in client side code.
        /// </summary>
        internal bool IsInDegree1
        {
            get { return this == _next; }
        }


        /// <summary>
        /// Returns true if this halfedge is in a two-sided face/hole.
        /// Note this should always return false in client side code for halfedges with faces.
        /// </summary>
        public bool IsInDegree2
        {
            get { return this == _next._next; }
        }


        /// <summary>
        /// Returns true if this halfedge is in a three-sided face/hole.
        /// </summary>
        public bool IsInDegree3
        {
            get { return this == _next._next._next; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsBoundary
        {
            get { return _face == null || _twin._face == null; }
        }


        /// <summary>
        /// Returns true this halfedge is interior and connects two boundary vertices.
        /// </summary>
        public bool IsBridge
        {
            get { return _start.IsBoundary && _twin._start.IsBoundary && _face != null && _twin._face != null; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsInHole
        {
            get { return _face == null; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsFirstAtStart
        {
            get { return this == _start.FirstOut; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsFirstInFace
        {
            get { return this == _face.First; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TE> CirculateStart
        {
            get
            {
                var he = _self;

                do
                {
                    yield return he;
                    he = he.Twin.NextInFace;
                } while (he != this);
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TE> CirculateEnd
        {
            get
            {
                var he = _self;

                do
                {
                    yield return he;
                    he = he.NextInFace.Twin;
                } while (he != this);
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TE> CirculateFace
        {
            get
            {
                var he = _self;

                do
                {
                    yield return he;
                    he = he.NextInFace;
                } while (he != this);
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TE> ConnectedPairs
        {
            get
            {
                yield return PrevInFace;
                yield return NextInFace;
                yield return Twin.PrevInFace;
                yield return Twin.NextInFace;
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TE NextBoundaryAtStart
        {
            get
            {
                var he = NextAtStart;

                do
                {
                    if (he.Face == null) return he;
                    he = he.NextAtStart;
                } while (he != this);

                return null;
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TE NextBoundaryInFace
        {
            get
            {
                var he1 = NextInFace;

                do
                {
                    if (he1.Twin.Face == null) return he1;
                    he1 = he1.NextInFace;
                } while (he1 != this);

                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void Remove()
        {
            _start = _twin._start = null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        internal void MakeConsecutive(TE other)
        {
            _next = other;
            other._prev = _self;
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeFirstInFace()
        {
            _face.First = _self;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountEdgesAtStart()
        {
            var he = this;
            int count = 0;

            do
            {
                count++;
                he = he.NextAtStart;
            } while (he != this);

            return count;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountEdgesInFace()
        {
            var he = this;
            int count = 0;

            do
            {
                count++;
                he = he.NextInFace;
            } while (he != this);

            return count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public TE OffsetAtStart(int count)
        {
            var he = _self;

            if (count < 0)
            {
                for (int i = 0; i < count; i++)
                    he = he.PrevAtStart;
            }
            else
            {
                for (int i = 0; i < count; i++)
                    he = he.NextAtStart;
            }

            return he;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public TE OffsetInFace(int count)
        {
            var he = _self;

            if (count < 0)
            {
                for (int i = 0; i < count; i++)
                    he = he.PrevInFace;
            }
            else
            {
                for (int i = 0; i < count; i++)
                    he = he.NextInFace;
            }

            return he;
        }


        /// <summary>
        /// 
        /// </summary>
        internal void Bypass()
        {
            if (IsAtDegree1)
            {
                Start.Remove();
                return;
            }

            var he = NextAtStart;

            if (Start.FirstOut == this)
                Start.FirstOut = he;

            PrevInFace.MakeConsecutive(he);
        }
    }
}
