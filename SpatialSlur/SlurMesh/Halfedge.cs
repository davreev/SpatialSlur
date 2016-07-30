using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 * 
 * Unused checks are not performed within element level methods to avoid redundancy.
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Halfedge:HeElement
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        internal static bool AreConsecutive(Halfedge he0, Halfedge he1)
        {
            return (he0.Next == he1 || he1.Next == he0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        internal static void MakeConsecutive(Halfedge he0, Halfedge he1)
        {
            he0.Next = he1;
            he1.Previous = he0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        internal static void MakeTwins(Halfedge he0, Halfedge he1)
        {
            he0.Twin = he1;
            he1.Twin = he0;
        }

        #endregion

        private Halfedge _prev;
        private Halfedge _next;
        private Halfedge _twin;
        private HeVertex _start;
        private HeFace _face;


        /// <summary>
        ///
        /// </summary>
        internal Halfedge() { }

        
        /// <summary>
        /// Returns the previous halfedge in the face.
        /// </summary>
        public Halfedge Previous
        {
            get { return _prev; }
            internal set { _prev = value; }
        }
       

        /// <summary>
        /// Returns the next halfedge in the face.
        /// </summary>
        public Halfedge Next
        {
            get { return _next; }
            internal set { _next = value; }
        }

        
        /// <summary>
        /// Returns the oppositely oriented adjacent halfedge
        /// </summary>
        public Halfedge Twin
        {
            get { return _twin; }
            internal set { _twin = value; }
        }


        /// <summary>
        /// Returns the vertex at the start of the halfedge.
        /// </summary>
        public HeVertex Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <summary>
        /// Returns the vertex at the end of the halfedge.
        /// </summary>
        public HeVertex End
        {
            get { return _twin._start; }
        }


        /// <summary>
        /// Returns the face to which the halfedge belongs.
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
        /// Returns true if the halfedge and its twin have different faces.
        /// </summary>
        internal override bool IsValid
        {
            get { return _face != _twin._face; }
        }


        /// <summary>
        /// Returns true if the halfedge or its twin has no adjacent face.
        /// </summary>
        public override bool IsBoundary
        {
            get { return _face == null || _twin._face == null; }
        }


        /// <summary>
        /// Returns a vector which spans from the start to the end of the halfedge.
        /// </summary>
        public Vec3d Span
        {
            get { return _start.VectorTo(_twin._start); }
        }


        /// <summary>
        /// Returns the length of the halfedge.
        /// </summary>
        public double Length
        {
            get { return _start.Position.DistanceTo(_twin._start.Position); }
        }


        /// <summary>
        /// Returns true if the halfedge starts at a degree 1 vertex.
        /// </summary>
        internal bool IsFromDegree1
        {
            get { return _twin == _prev; }
        }


        /// <summary>
        /// Returns true if the halfedge starts at a degree 2 vertex
        /// </summary>
        public bool IsFromDegree2
        {
            get { return _twin._next == _prev._twin; }
        }


        /// <summary>
        /// Returns true if the halfedge starts at a degree 3 vertex.
        /// </summary>
        public bool IsFromDegree3
        {
            get { return _twin._next._twin == _prev._twin._prev; }
        }


        /// <summary>
        /// Returns true if the halfedge is in a degenerate face.
        /// </summary>
        internal bool IsInDegenerate
        {
            get { return _next == _prev; }
        }


        /// <summary>
        /// Returns true if the halfedge is in a triangular face.
        /// </summary>
        public bool IsInTri
        {
            get { return _next._next == _prev; }
        }


        /// <summary>
        /// Returns true if the halfedge is in a quadrilateral face.
        /// </summary>
        public bool IsInQuad
        {
            get { return _next._next == _prev._prev; }
        }
   

        /// <summary>
        /// Returns true if the halfedge is the first from its start vertex.
        /// </summary>
        public bool IsFirstFromStart
        {
            get { return this == _start.First; }
        }


        /// <summary>
        /// Returns true if the halfedge is the first in its face.
        /// </summary>
        public bool IsFirstInFace
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
        /// Makes the halfedge the first outgoing from its start vertex.
        /// </summary>
        internal void MakeFirstFromStart()
        {
            _start.First = this;
        }


        /// <summary>
        /// Makes the halfedge the first in its face.
        /// </summary>
        public void MakeFirstInFace()
        {
            if (_face != null) _face.First = this;
        }


        /// <summary>
        /// Returns the first boundary halfedge encountered when circulating the start vertex.
        /// Returns null if no boundary halfedge is found.
        /// </summary>
        /// <returns></returns>
        internal Halfedge FindBoundary()
        {
            Halfedge he = this;

            do
            {
                if (he.Face == null) return he;
                he = he.Twin.Next;
            } while (he != this);

            return null;
        }


        /// <summary>
        /// Circulates the face starting from this halfedge.
        /// </summary>
        public IEnumerable<Halfedge> CirculateFace
        {
            get
            {
                Halfedge he = this;

                do
                {
                    yield return he;
                    he = he.Next;
                } while (he != this);
            }
        }


        /// <summary>
        /// Circulates the start vertex starting from this halfedge.
        /// </summary>
        public IEnumerable<Halfedge> CirculateStart
        {
            get
            {
                Halfedge he = this;

                do
                {
                    yield return he;
                    he = he.Twin.Next;
                } while (he != this);
            }
        }


        /// <summary>
        /// Returns a linearly interpolated point along the halfedge.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vec3d PointAt(double t)
        {
            return Vec3d.Lerp(_start.Position, _twin._start.Position, t);
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetNormal()
        {
            return Vec3d.Cross(_prev.Span, Span);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetAngle()
        {
            // TODO consistently define as exterior angle between edges (should be in range [0-Tau])
            return Vec3d.Angle(_prev.Span, Span);
        }
    }
}
