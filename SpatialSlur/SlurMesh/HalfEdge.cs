using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    public class HalfEdge:HeElement
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        internal static void MakeConsecutive(HalfEdge e0, HalfEdge e1)
        {
            e0.Next = e1;
            e1.Prev = e0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        /// <returns></returns>
        internal static bool AreConsecutive(HalfEdge e0, HalfEdge e1)
        {
            return (e0.Next == e1 || e1.Next == e0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        internal static void MakeTwins(HalfEdge e0, HalfEdge e1)
        {
            e0.Twin = e1;
            e1.Twin = e0;
        }

        #endregion


        private HeVertex _start;
        private HalfEdge _prev;
        private HalfEdge _next;
        private HalfEdge _twin;
        private HeFace _face;


        /// <summary>
        ///
        /// </summary>
        internal HalfEdge() { }


        /// <summary>
        /// Returns the vertex at the start of the half-edge.
        /// </summary>
        public HeVertex Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <summary>
        /// Returns the vertex at the end of the half-edge.
        /// </summary>
        public HeVertex End
        {
            get { return _twin._start; }
        }

        
        /// <summary>
        /// Returns the previous half-edge in the face.
        /// </summary>
        public HalfEdge Prev
        {
            get { return _prev; }
            internal set { _prev = value; }
        }
       

        /// <summary>
        /// Returns the next half-edge in the face.
        /// </summary>
        public HalfEdge Next
        {
            get { return _next; }
            internal set { _next = value; }
        }

        
        /// <summary>
        /// Returns the oppositely oriented adjacent half-edge
        /// </summary>
        public HalfEdge Twin
        {
            get { return _twin; }
            internal set { _twin = value; }
        }


        /// <summary>
        /// Returns the face to which the half-edge belongs.
        /// </summary>
        public HeFace Face
        {
            get { return _face; }
            internal set { _face = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsUnused
        {
            get { return _start == null; }
        }


        /// <summary>
        /// Returns true if the half-edge and its twin have different faces.
        /// </summary>
        internal override bool IsValid
        {
            get { return _face != _twin._face; }
        }


        /// <summary>
        /// Returns true if the half-edge or its twin has no adjacent face.
        /// </summary>
        public override bool IsBoundary
        {
            get { return _face == null || _twin._face == null; }
        }


        /// <summary>
        /// Returns a vector which spans from the start to the end of the half-edge.
        /// </summary>
        public Vec3d Span
        {
            get { return _start.VectorTo(_twin._start); }
        }


        /// <summary>
        /// Returns the length of the half-edge.
        /// </summary>
        public double Length
        {
            get { return _start.Position.DistanceTo(_twin._start.Position); }
        }


        /// <summary>
        /// Returns true if the half-edge starts at a degree 1 vertex.
        /// </summary>
        internal bool IsFromDeg1
        {
            get { return _twin == _prev; }
        }


        /// <summary>
        /// Returns true if the half-edge starts at a degree 2 vertex
        /// </summary>
        public bool IsFromDeg2
        {
            get { return _twin._next == _prev._twin; }
        }


        /// <summary>
        /// Returns true if the half-edge starts at a degree 3 vertex.
        /// </summary>
        public bool IsFromDeg3
        {
            get { return _twin._next._twin == _prev._twin._prev; }
        }


        /// <summary>
        /// Returns true if the half-edge is the first outgoing from its start vertex.
        /// </summary>
        public bool IsOutgoing
        {
            get { return this == _start.Outgoing; }
        }


        /// <summary>
        /// Returns true if the half-edge is the first in its face.
        /// </summary>
        public bool IsFirst
        {
            get { return _face != null && this == _face.First; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal override void MakeUnused()
        {
            _start = null;
        }


        /// <summary>
        /// Makes the half-edge the first in its face.
        /// </summary>
        public void MakeFirst()
        {
            if (_face != null) _face.First = this;
        }


        /// <summary>
        /// Makes the half-edge the first outgoing from its start vertex.
        /// </summary>
        internal void MakeOutgoing()
        {
            _start.Outgoing = this;
        }


        /// <summary>
        /// Returns the first boundary half-edge encountered when circulating the start vertex.
        /// Returns null if no boundary half-edge is found.
        /// </summary>
        /// <returns></returns>
        internal HalfEdge FindBoundary()
        {
            HalfEdge e = this;
            do
            {
                if (e.Face == null) return e;
                e = e.Twin.Next;
            } while (e != this);

            return null;
        }


        /// <summary>
        /// Circulates the face starting from this half-edge.
        /// </summary>
        public IEnumerable<HalfEdge> CirculateFace
        {
            get
            {
                // can't circulate an unused vertex
                if (IsUnused) yield break;

                // start on this edge
                HalfEdge e = this;

                // circulate to the next edge until back at the first
                do
                {
                    yield return e;
                    e = e.Next;
                } while (e != this);
            }
        }


        /// <summary>
        /// Circulates the start vertex starting from this half-edge.
        /// </summary>
        public IEnumerable<HalfEdge> CirculateVertex
        {
            get
            {
                // can't circulate an unused vertex
                if (IsUnused) yield break;

                // start on this edge
                HalfEdge e = this;

                // circulate to the next edge until back at the first
                do
                {
                    yield return e;
                    e = e.Twin.Next;
                } while (e != this);
            }
        }


        /// <summary>
        /// Returns an linearly interpolated point along the half-edge.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vec3d PointAt(double t)
        {
            return Vec3d.Lerp(_start.Position, _twin._start.Position, t);
        }
    }
}
