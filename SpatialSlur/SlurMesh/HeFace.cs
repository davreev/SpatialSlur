using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    public class HeFace:HeElement
    {
        private HeEdge _first; // first edge in face loop (doesn't matter which one)
   

        /// <summary>
        /// 
        /// </summary>
        internal HeFace() { }

        
        /// <summary>
        /// first edge the face loop (doesn't matter which one)
        /// </summary>
        public HeEdge First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <summary>
        /// true if element has been flagged for removal
        /// </summary>
        public override bool IsUnused
        {
            get { return _first == null; }
        }


        /// <summary>
        /// true if the face has at least 3 edges
        /// assumes the face is used
        /// </summary>
        internal override bool IsValid
        {
            get { return _first.Prev != _first.Next; }
        }


        /// <summary>
        /// true if the face has at least 1 boundary edge
        /// </summary>
        public override bool IsBoundary
        {
            get
            {
                foreach (HeEdge e in Edges)
                    if (e.Twin.Face == null) return true;
                return false;
            }
        }


        /// <summary>
        /// true if face has three edges
        /// </summary>
        public bool IsTri
        {
            get { return _first.Prev == _first.Next.Next; }
        }


        /// <summary>
        /// true is the face has 4 edges
        /// </summary>
        public bool IsQuad
        {
            get { return _first.Prev.Prev == _first.Next.Next; }
        }


        /// <summary>
        /// enumerates the face's vertices
        /// </summary>
        public IEnumerable<HeVertex> Vertices
        {
            get
            {
                // can't enumerate an unused face
                if (IsUnused) yield break;

                // start on the face's first edge
                HeEdge e = _first;

                // advance to the next edge until back at the first
                do
                {
                    yield return e.Start;
                    e = e.Next;
                } while (e != _first);
            }
        }


        /// <summary>
        /// enumerates the face's edges
        /// </summary>
        public IEnumerable<HeEdge> Edges
        {
            get
            {
                // can't enumerate an unused face
                if (IsUnused) yield break;

                // start on the face's first edge
                HeEdge e = _first;

                // advance to the next edge until back at the first
                do
                {
                    yield return e;
                    e = e.Next;
                } while (e != _first);
            }
        }


        /// <summary>
        /// enumerates all faces adjacent to this one
        /// null faces are not returned
        /// if mutliple edges are shared with an adjacent face, that face will be returned multiple times
        /// </summary>
        public IEnumerable<HeFace> AdjacentFaces
        {
            get
            {
                // can't enumerate an unused face
                if (IsUnused) yield break;

                // start on the face's first edge
                HeEdge e = _first;
          
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
        /// flags element for removal
        /// </summary>
        internal override void MakeUnused()
        {
            _first = null;
        }


        /// <summary>
        /// returns the number of edges in the face
        /// this is also equal to the number of vertices in the face
        /// </summary>
        /// <returns></returns>
        public int CountEdges()
        {
            int count = 0;

            foreach (HeEdge e in Edges)
                count++;

            return count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryEdges()
        {
            int count = 0;

            foreach (HeEdge e in Edges)
                if (e.Twin.Face == null) count++;

            return count;
        }


        /// <summary>
        /// 
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
        /// Finds the edge between this face and another. Returns null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public HeEdge FindEdgeBetween(HeFace other)
        {
            if (IsUnused) return null;

            foreach (HeEdge e in Edges)
                if (e.Twin.Face == other) return e;

            return null;
        }


        /// <summary>
        /// returns the centroid of the face
        /// </summary>
        /// <param name="f"></param>
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
        /// returns the circumcenter of a triangular face
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
