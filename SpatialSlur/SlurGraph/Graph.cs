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
    /// Adjacency list implementation of an undirected graph.
    /// </summary>
    [Serializable]
    public class Graph
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoints"></param>
        /// <param name="epsilon"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <param name="nodePositions"></param>
        /// <returns></returns>
        public static Graph CreateFromLineSegments(IList<Vec3d> endPoints, double epsilon, bool allowMultiEdges, bool allowLoops, out List<Vec3d> nodePositions)
        {
            int[] indexMap;
            nodePositions = Vec3d.RemoveDuplicates(endPoints, epsilon, out indexMap);

            Graph result = new Graph(nodePositions.Count, endPoints.Count >> 1);
            var nodes = result._nodes;
            var edges = result._edges;

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
                            Node n0 = nodes[indexMap[i]];
                            Node n1 = nodes[indexMap[i + 1]];
                            if (!n0.IsConnectedTo(n1)) edges.AddImpl(n0, n1);
                        }
                        break;
                    }
                case 2:
                    {
                        // no loops
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            Node n0 = nodes[indexMap[i]];
                            Node n1 = nodes[indexMap[i + 1]];
                            if (n0 != n1) edges.AddImpl(n0, n1);
                        }
                        break;
                    }
                case 3:
                    {
                        // no duplicate edges or loops
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            Node n0 = nodes[indexMap[i]];
                            Node n1 = nodes[indexMap[i + 1]];
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
        public static Graph CreateFromVertexTopology(HeMesh mesh)
        {
            var verts = mesh.Vertices;
            var hedges = mesh.Halfedges;

            Graph result = new Graph(verts.Count, hedges.Count >> 1);
            var nodes = result._nodes;
            var edges = result._edges;
   
            // add nodes
            nodes.AddMany(verts.Count);

            // add edges
            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge2 he = hedges[i];

                if (he.IsUnused)
                    edges.Add(new Edge());
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
        public static Graph CreateFromFaceTopology(HeMesh mesh)
        {
            var faces = mesh.Faces;
            var hedges = mesh.Halfedges;

            Graph result = new Graph(faces.Count, hedges.Count >> 1);
            var nodes = result._nodes;
            var edges = result._edges;

            // add nodes
            nodes.AddMany(faces.Count);

            // add edges
            for (int i = 0; i < hedges.Count; i+=2)
            {
                Halfedge2 he = hedges[i];
         
                if (he.IsUnused)
                    edges.Add(new Edge());
                else
                    edges.AddImpl(nodes[he.Face.Index], nodes[he.Twin.Face.Index]);
            }

            return result;
        }

        #endregion


        private readonly NodeList _nodes;
        private readonly EdgeList _edges;


        /// <summary>
        /// 
        /// </summary>
        public Graph()
        {
            _nodes = new NodeList(this);
            _edges = new EdgeList(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeCapacity"></param>
        /// <param name="edgeCapacity"></param>
        public Graph(int nodeCapacity, int edgeCapacity)
        {
            _nodes = new NodeList(this, nodeCapacity);
            _edges = new EdgeList(this, edgeCapacity);
        }


        /// <summary>
        /// Creates a deep copy of the given graph.
        /// </summary>
        /// <param name="other"></param>
        public Graph(Graph other)
        {
            var otherNodes = other._nodes;
            var otherEdges = other._edges;

            _nodes = new NodeList(this, otherNodes.Count);
            _edges = new EdgeList(this, otherEdges.Count);

            // add all nodes
            for (int i = 0; i < otherNodes.Count; i++)
                _nodes.Add(otherNodes[i].EdgeCapacity);

            // add all edges
            for (int i = 0; i < otherEdges.Count; i++)
            {
                Edge e = otherEdges[i];

                if (e.IsUnused)
                    _edges.Add(new Edge());
                else
                    _edges.AddImpl(_nodes[e.Start.Index], _nodes[e.End.Index]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public NodeList Nodes
        {
            get { return _nodes; }
        }


        /// <summary>
        /// 
        /// </summary>
        public EdgeList Edges
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
            return String.Format("Graph (N:{0} E:{1})", _nodes.Count, _edges.Count);
        }

    }
}
