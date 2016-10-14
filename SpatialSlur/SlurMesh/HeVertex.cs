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
    public class HeVertex : HeElement
    {
        private Vec3d _position;
        private Halfedge _first;


        /// <summary>
        /// 
        /// </summary>
        internal HeVertex() { }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        internal HeVertex(double x, double y, double z)
        {
            _position.Set(x, y, z);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        internal HeVertex(Vec3d position)
        {
            _position = position;
        }


        /// <summary>
        /// Returns the first halfedge starting at this vertex.
        /// Note that if the vertex is on the mesh boundary, the first halfedge must have a null face reference.
        /// </summary>
        public Halfedge First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <summary>
        /// Returns true if the vertex has no outgoing halfedges.
        /// </summary>
        public override bool IsUnused
        {
            get { return _first == null; }
        }


        /// <summary>
        /// Returns true if the vertex has at least 2 outgoing halfedges.
        /// </summary>
        internal override bool IsValid
        {
            get { return !_first.IsAtDegree1; }
        }


        /// <summary>
        /// Returns true if the vertex lies on the mesh boundary.
        /// Note that if this is true, the first halfedge must have a null face reference.
        /// </summary>
        public override bool IsBoundary
        {
            get { return _first.Face == null; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Position
        {
            get { return _position; }
            set { _position = value; }
        }


        /// <summary>
        /// Returns the number of halfedges starting from this vertex.
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
                    he = he.Twin.Next;
                } while (he != _first);

                return count;
            }
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
        ///
        /// </summary>
        internal override void MakeUnused()
        {
            _first = null;
        }

     
        /// <summary>
        /// Iterates over all connected vertices.
        /// </summary>
        public IEnumerable<HeVertex> ConnectedVertices
        {
            get
            {
                Halfedge he = _first;

                do
                {
                    he = he.Twin;
                    yield return he.Start;
                    he = he.Next;
                } while (he != _first);
            }
        }


        /// <summary>
        /// Iterates over all halfedges starting from this vertex.
        /// </summary>
        public IEnumerable<Halfedge> OutgoingHalfedges
        {
            get
            {
                Halfedge he = _first;

                do
                {
                    yield return he;
                    he = he.Twin.Next;
                } while (he != _first);
            }
        }


        /// <summary>
        /// Iterates over all halfedges ending at this vertex.
        /// </summary>
        public IEnumerable<Halfedge> IncomingHalfedges
        {
            get
            {
                Halfedge he = _first;

                do
                {
                    he = he.Twin;
                    yield return he;
                    he = he.Next;
                } while (he != _first);
            }
        }


        /// <summary>
        /// Iterates over all surrounding faces.
        /// Note that null faces are skipped.
        /// </summary>
        public IEnumerable<HeFace> SurroundingFaces
        {
            get
            {
                Halfedge he = _first;

                do
                {
                    HeFace f = he.Face;
                    if (f != null) yield return f;
                    he = he.Twin.Next;
                } while (he != _first);
            }
        }


        /// <summary>
        /// Finds the outgoing halfedge connecting this vertex to another. 
        /// Returns null if no such halfedge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Halfedge FindHalfedgeTo(HeVertex other)
        {
            foreach (Halfedge he in OutgoingHalfedges)
                if (he.End == other) return he;

            return null;
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetNormal()
        {
            Vec3d result = new Vec3d();

            foreach (Halfedge he in OutgoingHalfedges)
            {
                if (he.Face == null) continue;
                result += he.GetNormal();
            }

            result.Unitize();
            return result;
        }

    }
}
