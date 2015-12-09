using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    public class HeVertex : HeElement
    {
        private Vec3d _position;
        private HeEdge _outgoing;


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
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        internal HeVertex(Vec3d position)
        {
            _position = position;
        }


        /// <summary>
        /// Returns one of the edges starting at this vertex.
        /// Note that if the vertex is on the mesh boundary, the outgoing edge will be as well (i.e. it will have a null face).
        /// </summary>
        public HeEdge Outgoing
        {
            get { return _outgoing; }
            internal set { _outgoing = value; }
        }


        /// <summary>
        /// Returns true if the vertex has no outgoing edge.
        /// </summary>
        public override bool IsUnused
        {
            get { return _outgoing == null; }
        }


        /// <summary>
        /// Returns true if the vertex has at least 2 outgoing edges.
        /// Note that this assumes the vertex is used.
        /// </summary>
        internal override bool IsValid
        {
            get { return !_outgoing.IsFromDeg1; }
        }


        /// <summary>
        /// Returns true if the vertex lies on the mesh boundary.
        /// Note that if this is true, the outgoing edge will also be on the boundary (i.e. it will have a null face).
        /// </summary>
        public override bool IsBoundary
        {
            get { return _outgoing.Face == null; }
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
        /// Returns true if the vertex is has 2 outgoing edges.
        /// </summary>
        public bool IsDeg2
        {
            get { return _outgoing.IsFromDeg2; }
        }


        /// <summary>
        /// Returns true if the vertex is has 3 outgoing edges.
        /// </summary>
        public bool IsDeg3
        {
            get { return _outgoing.IsFromDeg3; }
        }


        /// <summary>
        ///
        /// </summary>
        internal override void MakeUnused()
        {
            _outgoing = null;
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
                HeEdge e = _outgoing;

                // circulate to the next edge until back at the first
                do
                {
                    e = e.Twin;
                    yield return e.Start;
                    e = e.Next;
                } while (e != _outgoing);
            }
        }


        /// <summary>
        /// Iterates over all outgoing edges.
        /// </summary>
        public IEnumerable<HeEdge> OutgoingEdges
        {
            get
            {
                if (IsUnused) yield break;
                HeEdge e = _outgoing;

                // circulate to the next edge until back at the first
                do
                {
                    yield return e;
                    e = e.Twin.Next;
                } while (e != _outgoing);
            }
        }


        /// <summary>
        /// Iterates over all incoming edges.
        /// </summary>
        public IEnumerable<HeEdge> IncomingEdges
        {
            get
            {
                if (IsUnused) yield break;
                HeEdge e = _outgoing;

                // circulate to the next edge until back at the first
                do
                {
                    e = e.Twin;
                    yield return e;
                    e = e.Next;
                } while (e != _outgoing);
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
                HeEdge e = _outgoing;
                HeFace f = e.Face;

                // circulate to the next edge until back at the first
                do
                {
                    if (f != null) yield return f;
                    e = e.Twin.Next;
                    f = e.Face;
                } while (e != _outgoing);
            }
        }


        /// <summary>
        /// Returns the number of connected vertices.
        /// </summary>
        /// <returns></returns>
        public int GetDegree()
        {
            if (IsUnused) return 0; // no nieghbours if unused
            HeEdge e = _outgoing;
            int count = 0;

            // circulate to the next edge until back at the first
            do
            {
                count++;
                e = e.Twin.Next;
            } while (e != _outgoing);

            return count;
        }


        /// <summary>
        /// Finds the outgoing edge connecting this vertex to another. 
        /// Returns null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public HeEdge FindEdgeTo(HeVertex other)
        {
            if (IsUnused) return null;

            foreach (HeEdge e in OutgoingEdges)
                if (e.End == other) return e;

            return null;
        }

    }
}
