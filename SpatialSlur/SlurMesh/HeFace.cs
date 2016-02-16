using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public class HeFace:HeElement
    {
        private HalfEdge _first;
   

        /// <summary>
        /// 
        /// </summary>
        internal HeFace() { }

        
        /// <summary>
        /// Returns the first half-edge in the face.
        /// </summary>
        public HalfEdge First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsUnused
        {
            get { return _first == null; }
        }


        /// <summary>
        /// Returns true if the face has at least 3 edges.
        /// This method assumes that the face is in use.
        /// </summary>
        internal override bool IsValid
        {
            get { return !_first.IsInDegenerate; }
        }


        /// <summary>
        /// Returns true if the face has at least 1 boundary vertex.
        /// </summary>
        public override bool IsBoundary
        {
            get
            {
                foreach (HalfEdge e in HalfEdges)
                    if (e.Start.IsBoundary) return true;
                return false;
            }
        }


        /// <summary>
        /// Returns true if the face has 3 edges.
        /// </summary>
        public bool IsTri
        {
            get { return _first.IsInTri; }
        }


        /// <summary>
        /// Returns true if the face has 4 edges.
        /// </summary>
        public bool IsQuad
        {
            get { return _first.IsInQuad; }
        }


        /// <summary>
        /// Iterates over the face's vertices.
        /// </summary>
        public IEnumerable<HeVertex> Vertices
        {
            get
            {
                if (IsUnused) yield break;
                HalfEdge e = _first;

                // advance to the next edge until back at the first
                do
                {
                    yield return e.Start;
                    e = e.Next;
                } while (e != _first);
            }
        }


        /// <summary>
        /// Iterates over the face's half-edges.
        /// </summary>
        public IEnumerable<HalfEdge> HalfEdges
        {
            get
            {
                if (IsUnused) yield break;
                HalfEdge e = _first;

                // advance to the next edge until back at the first
                do
                {
                    yield return e;
                    e = e.Next;
                } while (e != _first);
            }
        }


        /// <summary>
        /// Iterates over adjacent faces.
        /// Null faces are skipped.
        /// Note that if mutliple edges are shared with an adjacent face, that face will be returned multiple times.
        /// </summary>
        public IEnumerable<HeFace> AdjacentFaces
        {
            get
            {
                if (IsUnused) yield break;
                HalfEdge e = _first;
          
                // advance to the next edge until back at the first
                do
                {
                    HeFace f = e.Twin.Face;
                    if (f != null) yield return f;

                    e = e.Next;
                } while (e != _first);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal override void MakeUnused()
        {
            _first = null;
        }


        /// <summary>
        /// Returns the number of edges in the face.
        /// </summary>
        /// <returns></returns>
        public int CountEdges()
        {
            int count = 0;

            foreach (HalfEdge e in HalfEdges)
                count++;

            return count;
        }


        /// <summary>
        /// Returns the number of boundary edges in the face.
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryEdges()
        {
            int count = 0;

            foreach (HalfEdge e in HalfEdges)
                if (e.Twin.Face == null) count++;

            return count;
        }


        /// <summary>
        /// Returns the number of boundary vertices in the face.
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryVertices()
        {
            int count = 0;

            foreach (HeVertex v in Vertices)
                if (v.IsBoundary) count++;

            return count;
        }


        /// <summary>
        /// Finds the edge between this face and another.
        /// Returns the half-edge adjacent to this face or null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public HalfEdge FindEdgeBetween(HeFace other)
        {
            if (IsUnused) return null;

            foreach (HalfEdge e in HalfEdges)
                if (e.Twin.Face == other) return e;

            return null;
        }


        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetCenter()
        {
            Vec3d result = new Vec3d();
            int count = 0;

            foreach (HeVertex v in Vertices)
            {
                result += v.Position;
                count++;
            }
            result /= count;

            return result;
        }


        /// <summary>
        /// Returns the circumcenter of a triangular face.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetCircumcenter()
        {
            if (!IsTri)
                throw new InvalidOperationException("the face must be triangular");

            throw new NotImplementedException();
        }

    }
}
