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
    public class HeFace:HeElement
    {
        private Halfedge _first;
   

        /// <summary>
        /// 
        /// </summary>
        internal HeFace() { }

        
        /// <summary>
        /// Returns the first halfedge in the face.
        /// </summary>
        public Halfedge First
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
        /// </summary>
        internal override bool IsValid
        {
            get { return !_first.IsInDegenerate; }
        }


        /// <summary>
        /// Returns true if the face has at least 1 boundary edge.
        /// </summary>
        public override bool IsBoundary
        {
            get
            {
                Halfedge he = _first;
          
                do
                {
                    if (he.Twin.Face == null) return true;
                    he = he.Next;
                } while (he != _first);

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
        /// Returns the number of edges in the face.
        /// </summary>
        public int Degree
        {
            get
            {
                Halfedge he = _first;
                int count = 0;

                do
                {
                    count++;
                    he = he.Next;
                } while (he != _first);

                return count;
            }
        }


        /// <summary>
        /// Iterates over the face's vertices.
        /// </summary>
        public IEnumerable<HeVertex> Vertices
        {
            get
            {
                Halfedge he = _first;

                do
                {
                    yield return he.Start;
                    he = he.Next;
                } while (he != _first);
            }
        }


        /// <summary>
        /// Iterates over the face's halfedges.
        /// </summary>
        public IEnumerable<Halfedge> Halfedges
        {
            get
            {
                Halfedge he = _first;

                do
                {
                    yield return he;
                    he = he.Next;
                } while (he != _first);
            }
        }


        /// <summary>
        /// Iterates over adjacent faces.
        /// Note that null faces are skipped.
        /// Also if mutliple edges are shared with an adjacent face, that face will be returned multiple times.
        /// </summary>
        public IEnumerable<HeFace> AdjacentFaces
        {
            get
            {
                Halfedge he = _first;

                do
                {
                    HeFace f = he.Twin.Face;
                    if (f != null) yield return f;
                    he = he.Next;
                } while (he != _first);
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
        [Obsolete("Use Degree property instead.")]
        public int CountEdges()
        {
            Halfedge he = _first;
            int count = 0;

            do
            {
                count++;
                he = he.Next;
            } while (he != _first);

            return count;
        }


        /// <summary>
        /// Returns the number of boundary edges in the face.
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryEdges()
        {
            Halfedge he = _first;
            int count = 0;

            do
            {
                if(he.Twin.Face == null) count++;
                he = he.Next;
            } while (he != _first);

            return count;
        }


        /// <summary>
        /// Returns the number of boundary vertices in the face.
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryVertices()
        {
            Halfedge he = _first;
            int count = 0;

            do
            {
                if (he.Start.IsBoundary) count++;
                he = he.Next;
            } while (he != _first);

            return count;
        }


        /// <summary>
        /// Finds the edge between this face and another.
        /// Returns the halfedge adjacent to this face or null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Halfedge FindEdgeBetween(HeFace other)
        {
            foreach (Halfedge he in Halfedges)
                if (he.Twin.Face == other) return he;

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
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetNormal()
        {
            Vec3d result;

            if(this.IsTri)
            {
                result = _first.GetNormal();
            }
            else
            {
                result = new Vec3d();
                foreach (Halfedge he in Halfedges)
                    result += he.GetNormal();
            }
        
            result.Unitize();
            return result;
        }


        /// <summary>
        /// Returns the circumcenter of a triangular face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetCircumcenter()
        {
            var next = _first.Next;
            return next.Start.Position + GeometryUtil.GetCurvatureVector(_first.Twin.Span, next.Span);
        }
    }
}
