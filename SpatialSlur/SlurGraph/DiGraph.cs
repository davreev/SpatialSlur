using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// Adjacency list implementation of a directed graph.
    /// </summary>
    [Serializable]
    public class DiGraph
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointPairs"></param>
        /// <param name="epsilon"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <param name="nodePositions"></param>
        /// <returns></returns>
        public static DiGraph CreateFromLineSegments(IList<Vec3d> pointPairs, double epsilon, bool allowMultiEdges, bool allowLoops, out List<Vec3d> nodePositions)
        {
            int[] indexMap;
            nodePositions = Vec3d.RemoveDuplicates(pointPairs, epsilon, out indexMap);
            
            DiGraph result = new DiGraph(nodePositions.Count, pointPairs.Count >> 1);
            var nodes = result.Nodes;
            var edges = result.Edges;

            // add nodes
            nodes.AddMany(nodePositions.Count);

            // add edges
            int mask = 0;
            if (allowMultiEdges) mask |= 1;
            if (allowLoops) mask |= 2;

            // 0 - no constraints
            // 1 - no duplicate edges
            // 2 - no loops
            // 3 - no duplicate edges or loops
            switch (mask)
            {
                case 0:
                    {
                        // no constraints
                        for (int i = 0; i < indexMap.Length; i += 2)
                            edges.Add(indexMap[i], indexMap[i + 1]);
                        break;
                    }
                case 1:
                    {
                        // no duplicate edges
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            DiNode n0 = nodes[indexMap[i]];
                            DiNode n1 = nodes[indexMap[i + 1]];
                            if (!n0.IsConnectedTo(n1)) edges.AddImpl(n0, n1);
                        }
                        break;
                    }
                case 2:
                    {
                        // no loops
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            DiNode n0 = nodes[indexMap[i]];
                            DiNode n1 = nodes[indexMap[i + 1]];
                            if (n0 != n1) edges.AddImpl(n0, n1);
                        }
                        break;
                    }
                case 3:
                    {
                        // no duplicate edges or loops
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            DiNode n0 = nodes[indexMap[i]];
                            DiNode n1 = nodes[indexMap[i + 1]];
                            if (!(n0 == n1 || n0.IsConnectedTo(n1))) edges.AddImpl(n0, n1);
                        }
                        break;
                    }
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static DiGraph CreateFromVertexTopology(HeMesh mesh)
        {
            var verts = mesh.Vertices;
            var hedges = mesh.Halfedges;

            DiGraph result = new DiGraph(verts.Count, hedges.Count);
            var nodes = result.Nodes;
            var edges = result.Edges;

            // add nodes
            nodes.AddMany(verts.Count);

            // add edges
            for (int i = 0; i < hedges.Count; i++)
            {
                Halfedge he = hedges[i];

                if (he.IsUnused)
                    edges.Add(new DiEdge());
                else
                    edges.Add(he.Start.Index, he.End.Index);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static DiGraph CreateFromFaceTopology(HeMesh mesh)
        {
            var faces = mesh.Faces;
            var hedges = mesh.Halfedges;

            DiGraph result = new DiGraph(faces.Count, hedges.Count);
            var nodes = result.Nodes;
            var edges = result.Edges;

            // add nodes
            nodes.AddMany(faces.Count);

            // add edges
            for (int i = 0; i < hedges.Count; i++)
            {
                Halfedge he = hedges[i];

                if (he.IsUnused || he.IsBoundary)
                    edges.Add(new DiEdge());
                else
                    edges.Add(he.Face.Index, he.Twin.Face.Index);
            }

            return result;
        }

        #endregion


        private readonly DiNodeList _nodes;
        private readonly DiEdgeList _edges;


        /// <summary>
        /// 
        /// </summary>
        public DiGraph()
        {
            _nodes = new DiNodeList(this);
            _edges = new DiEdgeList(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeCapacity"></param>
        /// <param name="edgeCapacity"></param>
        public DiGraph(int nodeCapacity, int edgeCapacity)
        {
            _nodes = new DiNodeList(this, nodeCapacity);
            _edges = new DiEdgeList(this, edgeCapacity);
        }


        /// <summary>
        /// Creates a deep copy of the given graph.
        /// </summary>
        /// <param name="other"></param>
        public DiGraph(DiGraph other)
            : this(other._nodes.Count, other._edges.Count)
        {
            var otherNodes = other._nodes;
            var otherEdges = other._edges;

            _nodes = new DiNodeList(this, otherNodes.Count);
            _edges = new DiEdgeList(this, otherEdges.Count);
  
            // add all nodes
            for (int i = 0; i < otherNodes.Count; i++)
            {
                var n = otherNodes[i];
                _nodes.Add(n.InEdgeCapacity, n.OutEdgeCapacity);
            }

            // add all edges
            for (int i = 0; i < otherEdges.Count; i++)
            {
                DiEdge e = otherEdges[i];

                if (e.IsUnused)
                    _edges.Add(new DiEdge());
                else
                    _edges.AddImpl(_nodes[e.Start.Index], _nodes[e.End.Index]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public DiNodeList Nodes
        {
            get { return _nodes; }
        }


        /// <summary>
        /// 
        /// </summary>
        public DiEdgeList Edges
        {
            get { return _edges; }
        }


        /// <summary>
        /// Removes all flagged nodes and edges from the graph.
        /// </summary>
        public void Compact()
        {
            _nodes.Compact();
            _edges.Compact();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("DiGraph (N:{0} E:{1})", _nodes.Count, _edges.Count);
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public double[] GetLaplacianMatrix()
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public double[] GetLaplacianMatrix(IList<double> edgeWeights)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
