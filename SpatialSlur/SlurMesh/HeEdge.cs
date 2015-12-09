using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    public class HeEdge:HeElement
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        internal static void MakeConsecutive(HeEdge e0, HeEdge e1)
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
        internal static bool AreConsecutive(HeEdge e0, HeEdge e1)
        {
            return (e0.Next == e1 || e1.Next == e0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        internal static void MakeTwins(HeEdge e0, HeEdge e1)
        {
            e0.Twin = e1;
            e1.Twin = e0;
        }

        #endregion


        private HeVertex _start;
        private HeEdge _prev;
        private HeEdge _next;
        private HeEdge _twin;
        private HeFace _face;


        /// <summary>
        ///
        /// </summary>
        internal HeEdge() { }


        /// <summary>
        /// Returns the vertex at the start of the half edge.
        /// </summary>
        public HeVertex Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <summary>
        /// Returns the vertex at the end of the half edge.
        /// </summary>
        public HeVertex End
        {
            get { return _twin._start; }
        }


        [Obsolete("use Prev property instead")]
        /// <summary>
        /// Returns the previous edge in the face.
        /// </summary>
        public HeEdge Previous
        {
            get { return _prev; }
            internal set { _prev = value; }
        }

        
        /// <summary>
        /// Returns the previous edge in the face.
        /// </summary>
        public HeEdge Prev
        {
            get { return _prev; }
            internal set { _prev = value; }
        }
       

        /// <summary>
        /// Returns the next edge in the face.
        /// </summary>
        public HeEdge Next
        {
            get { return _next; }
            internal set { _next = value; }
        }

        
        /// <summary>
        /// oppositely oriented adjacent edge
        /// </summary>
        public HeEdge Twin
        {
            get { return _twin; }
            internal set { _twin = value; }
        }


        /// <summary>
        /// Returns the face to which this edge belongs.
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
        /// Returns true if the edge and its twin have different faces.
        /// </summary>
        internal override bool IsValid
        {
            get { return _face != _twin._face; }
        }


        /// <summary>
        /// Returns true if the edge or its twin has no adjacent face.
        /// </summary>
        public override bool IsBoundary
        {
            get { return _face == null || _twin._face == null; }
        }


        /// <summary>
        /// Returns a vector which spans between the edge's vertices.
        /// </summary>
        public Vec3d Span
        {
            get { return _start.VectorTo(_twin._start); }
        }


        /// <summary>
        /// Returns the length of the edge.
        /// </summary>
        public double Length
        {
            get { return _start.Position.DistanceTo(_twin._start.Position); }
        }


        /// <summary>
        /// Returns true if the edge starts at a degree 1 vertex.
        /// </summary>
        internal bool IsFromDeg1
        {
            get { return _twin == _prev; }
        }


        /// <summary>
        /// Returns true if the edge starts at a degree 2 vertex
        /// </summary>
        public bool IsFromDeg2
        {
            get { return _twin._next == _prev._twin; }
        }


        /// <summary>
        /// Returns true if the edge starts at a degree 3 vertex.
        /// </summary>
        public bool IsFromDeg3
        {
            get { return _twin._next._twin == _prev._twin._prev; }
        }


        /// <summary>
        /// Returns true if the edge is the outgoing edge of its start vertex.
        /// </summary>
        public bool IsOutgoing
        {
            get { return this == _start.Outgoing; }
        }


        /// <summary>
        /// Returns true if the edge is the first in its face.
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
        /// Makes the edge the first in its face.
        /// </summary>
        public void MakeFirst()
        {
            if (_face != null) _face.First = this;
        }


        /// <summary>
        /// Makes the edge the outgoing edge from its start vertex.
        /// </summary>
        internal void MakeOutgoing()
        {
            _start.Outgoing = this;
        }


        /// <summary>
        /// Returns the first boundary edge encountered when circulating around the start vertex.
        /// Returns null if no boundary edge is found.
        /// </summary>
        /// <returns></returns>
        internal HeEdge FindBoundary()
        {
            HeEdge e = this;
            do
            {
                if (e.Face == null) return e;
                e = e.Twin.Next;
            } while (e != this);

            return null;
        }


        /// <summary>
        /// Circulates the face starting from this edge.
        /// </summary>
        public IEnumerable<HeEdge> CirculateFace
        {
            get
            {
                // can't circulate an unused vertex
                if (IsUnused) yield break;

                // start on this edge
                HeEdge e = this;

                // circulate to the next edge until back at the first
                do
                {
                    yield return e;
                    e = e.Next;
                } while (e != this);
            }
        }


        /// <summary>
        /// Circulates the start vertex starting from this edge.
        /// </summary>
        public IEnumerable<HeEdge> CirculateVertex
        {
            get
            {
                // can't circulate an unused vertex
                if (IsUnused) yield break;

                // start on this edge
                HeEdge e = this;

                // circulate to the next edge until back at the first
                do
                {
                    yield return e;
                    e = e.Twin.Next;
                } while (e != this);
            }
        }


        /// <summary>
        /// Returns an linearly interpolated point along the edge.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vec3d PointAt(double t)
        {
            return Vec3d.Lerp(_start.Position, _twin._start.Position, t);
        }
    }
}
