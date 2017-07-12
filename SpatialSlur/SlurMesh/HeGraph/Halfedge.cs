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
    public class Halfedge<TV, TE> : HeElement, IHalfedge<TV, TE>
        where TV : HeVertex<TV, TE>
        where TE : Halfedge<TV, TE>
    {
        private TE _self; // cached downcasted ref of this instance (TODO test impact)
        private TE _prev;
        private TE _next;
        private TE _twin;
        private TV _start;


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
        public TE Older
        {
            get { return _self < _twin ? _self : _twin; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE PrevAtStart
        {
            get { return _prev; }
            internal set { _prev = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE NextAtStart
        {
            get { return _next; }
            internal set { _next = value; }
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
        public sealed override bool IsRemoved
        {
            get { return _start == null; }
        }


        /// <summary>
        /// Returns true if this halfedge starts at a degree 1 vertex.
        /// </summary>
        public bool IsAtDegree1
        {
            get { return this == _next; }
        }


        /// <summary>
        /// Returns true if this halfedge starts at a degree 2 vertex.
        /// </summary>
        public bool IsAtDegree2
        {
            get
            {
                var he = _next;
                return this != he && this == he._next;
            }
        }


        /// <summary>
        /// Returns true if this halfedge starts at a degree 3 vertex.
        /// </summary>
        public bool IsAtDegree3
        {
            get
            {
                var he = _next;
                return this != he && this == he._next._next;
            }
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
        public IEnumerable<TE> CirculateStart
        {
            get
            {
                var he = _self;

                do
                {
                    yield return he;
                    he = he.NextAtStart;
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
                var he = Twin;

                do
                {
                    yield return he.Twin;
                    he = he.NextAtStart;
                } while (he != Twin);
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
                yield return PrevAtStart;
                yield return NextAtStart;
                yield return Twin.PrevAtStart;
                yield return Twin.NextAtStart;
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
        internal void Bypass()
        {
            var v = _start;

            if (IsAtDegree1)
            {
                _start.Remove();
                return;
            }

            if (IsFirstAtStart) _start.FirstOut = NextAtStart;
            PrevAtStart.MakeConsecutive(NextAtStart);
        }
    }
}