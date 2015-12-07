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
        private HeEdge _outgoing; // one of the half edges starting at the vertex - if the vertex is on a boundary the outgoing edge must have a null face reference


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
        /// Gets one of the edges starting at this vertex.
        /// Note that if a vertex lies on a boundary, the face of the outgoing edge will also be null.
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
        /// Returns true if this vertex has at least 2 outgoing edges.
        /// Note that this assumes the vertex is used.
        /// </summary>
        internal override bool IsValid
        {
            get { return !_outgoing.IsFromDeg1; }
        }


        /// <summary>
        /// Returns true if the vertex lies on the mesh boundary.
        /// Note that if a vertex lies on a boundary, the face of the outgoing edge will also be null.
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
        /// Returns true if the vertex is has exactly 2 outgoing edges.
        /// </summary>
        public bool IsDeg2
        {
            get { return _outgoing.IsFromDeg2; }
        }


        /// <summary>
        /// Returns true if the vertex is has exactly 3 outgoing edges.
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
        /// Circulates all neighbouring vertices.
        /// </summary>
        public IEnumerable<HeVertex> ConnectedVertices
        {
            get
            {
                // can't circulate an unused vertex
                if (IsUnused) yield break;

                // start on the vertex's outgoing edge
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
        /// Circulates all outgoing edges.
        /// </summary>
        public IEnumerable<HeEdge> OutgoingEdges
        {
            get
            {
                // can't circulate an unused vertex
                if (IsUnused) yield break;

                // start on the vertex's outgoing edge
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
        /// Circulates all incoming edges.
        /// </summary>
        public IEnumerable<HeEdge> IncomingEdges
        {
            get
            {
                // can't circulate an unused vertex
                if (IsUnused) yield break;

                // start on the vertex's outgoing edge
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
        /// Circulates all surrounding faces.
        /// </summary>
        public IEnumerable<HeFace> SurroundingFaces
        {
            get
            {
                // can't circulate an unused vertex
                if (IsUnused) yield break;

                // start on the vertex's outgoing edge
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
        /// Returns the number of neighbouring vertices.
        /// </summary>
        /// <returns></returns>
        public int GetDegree()
        {
            // no nieghbours if unused
            if (IsUnused) return 0;

            // start on the vertex's outgoing edge
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
