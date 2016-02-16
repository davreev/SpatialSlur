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
    public class HeVertex : HeElement
    {
        private Vec3d _position;
        private HalfEdge _first;


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
        /// Returns the first half-edge starting at this vertex.
        /// Note that if the vertex is on the mesh boundary, the first half-edge must have a null face reference.
        /// </summary>
        public HalfEdge First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <summary>
        /// Returns true if the vertex has no outgoing half-edge.
        /// </summary>
        public override bool IsUnused
        {
            get { return _first == null; }
        }


        /// <summary>
        /// Returns true if the vertex has at least 2 outgoing half-edges.
        /// Note that this assumes the vertex is used.
        /// </summary>
        internal override bool IsValid
        {
            get { return !_first.IsFromDegree1; }
        }


        /// <summary>
        /// Returns true if the vertex lies on the mesh boundary.
        /// Note that if this is true, the outgoing half-edge must have a null face reference.
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
        /// 
        /// </summary>
        public int Degree
        {
            get
            {
                if (IsUnused) return 0; // no nieghbours if unused
                HalfEdge e = _first;
                int count = 0;

                // circulate to the next edge until back at the first
                do
                {
                    count++;
                    e = e.Twin.Next;
                } while (e != _first);

                return count;
            }
        }


        /// <summary>
        /// Returns true if the vertex has 2 outgoing half-edges.
        /// </summary>
        public bool IsDegree2
        {
            get { return _first.IsFromDegree2; }
        }


        /// <summary>
        /// Returns true if the vertex has 3 outgoing half-edges.
        /// </summary>
        public bool IsDegree3
        {
            get { return _first.IsFromDegree3; }
        }


        /// <summary>
        ///
        /// </summary>
        internal override void MakeUnused()
        {
            _first = null;
        }

     
        /// <summary>
        /// Returns a vector spanning from this vertex to another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Vec3d VectorTo(HeVertex other)
        {
            return other._position - _position;
        }


        /// <summary>
        /// Iterates over all connected vertices.
        /// </summary>
        public IEnumerable<HeVertex> ConnectedVertices
        {
            get
            {
                if (IsUnused) yield break;
                HalfEdge e = _first;

                // circulate to the next edge until back at the first
                do
                {
                    e = e.Twin;
                    yield return e.Start;
                    e = e.Next;
                } while (e != _first);
            }
        }


        /// <summary>
        /// Iterates over all outgoing half-edges.
        /// </summary>
        public IEnumerable<HalfEdge> OutgoingHalfEdges
        {
            get
            {
                if (IsUnused) yield break;
                HalfEdge e = _first;

                // circulate to the next edge until back at the first
                do
                {
                    yield return e;
                    e = e.Twin.Next;
                } while (e != _first);
            }
        }


        /// <summary>
        /// Iterates over all incoming half-edges.
        /// </summary>
        public IEnumerable<HalfEdge> IncomingHalfEdges
        {
            get
            {
                if (IsUnused) yield break;
                HalfEdge e = _first;

                // circulate to the next edge until back at the first
                do
                {
                    e = e.Twin;
                    yield return e;
                    e = e.Next;
                } while (e != _first);
            }
        }


        /// <summary>
        /// Iterates over all surrounding faces.
        /// Null faces are skipped.
        /// </summary>
        public IEnumerable<HeFace> SurroundingFaces
        {
            get
            {
                if (IsUnused) yield break;
                HalfEdge e = _first;
                HeFace f = e.Face;

                // circulate to the next edge until back at the first
                do
                {
                    if (f != null) yield return f;
                    e = e.Twin.Next;
                    f = e.Face;
                } while (e != _first);
            }
        }


        /// <summary>
        /// Finds the outgoing half-edge connecting this vertex to another. 
        /// Returns null if no such half-edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public HalfEdge FindHalfEdgeTo(HeVertex other)
        {
            if (IsUnused) return null;

            foreach (HalfEdge e in OutgoingHalfEdges)
                if (e.End == other) return e;

            return null;
        }

    }
}
