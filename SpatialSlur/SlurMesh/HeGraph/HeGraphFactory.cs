using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */
 
namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeGraphFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="vertexProvider"></param>
        /// <param name="hedgeProvider"></param>
        /// <returns></returns>
        public static HeGraphFactory<TV, TE> Create<TV, TE>(Func<TV> vertexProvider, Func<TE> hedgeProvider)
            where TV : HeVertex<TV, TE>
            where TE : Halfedge<TV, TE>
        {
            return new HeGraphFactory<TV, TE>(vertexProvider, hedgeProvider);
        }


        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static HeGraphFactory<TV, TE> Create<TV, TE>(HeGraph<TV, TE> graph)
            where TV : HeVertex<TV, TE>
            where TE : Halfedge<TV, TE>
        {
            return new HeGraphFactory<TV, TE>(graph.VertexProvider, graph.HalfedgeProvider);
        }
    }


    /// <summary>
    /// Class for creation of HeGraphs
    /// </summary>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    [Serializable]
    public class HeGraphFactory<TV, TE> : IFactory<HeGraph<TV,TE>>
        where TV : HeVertex<TV, TE>
        where TE : Halfedge<TV, TE>
    {
        private Func<TV> _newTV;
        private Func<TE> _newTE;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexProvider"></param>
        /// <param name="hedgeProvider"></param>
        public HeGraphFactory(Func<TV> vertexProvider, Func<TE> hedgeProvider)
        {
            _newTV = vertexProvider ?? throw new ArgumentNullException();
            _newTE = hedgeProvider ?? throw new ArgumentNullException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeGraph<TV, TE> Create()
        {
            return Create(4, 4);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public HeGraph<TV, TE> Create(int vertexCapacity, int hedgeCapacity)
        {
            return HeGraph.Create(_newTV, _newTE, vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public HeGraph<TV, TE> CreateCopy<UV, UE>(HeGraph<UV, UE> graph, Action<TV, UV> setVertex, Action<TE, UE> setHedge)
               where UV : HeVertex<UV, UE>
               where UE : Halfedge<UV, UE>
        {
            var copy = Create(graph.Vertices.Capacity, graph.Halfedges.Capacity);
            copy.Append(graph, setVertex, setHedge);
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="getHandle"></param>
        /// <param name="setHandle"></param>
        /// <returns></returns>
        public HeGraph<TV, TE>[] CreateConnectedComponents<UV, UE>(HeGraph<UV, UE> graph, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Func<UE, SplitDisjointHandle> getHandle, Action<UE, SplitDisjointHandle> setHandle)
            where UV : HeVertex<UV, UE>
            where UE : Halfedge<UV, UE>
        {
            return graph.SplitDisjoint(this, setVertex, setHedge, getHandle, setHandle);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoints"></param>
        /// <param name="setPosition"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public HeGraph<TV, TE> CreateFromLineSegments(IReadOnlyList<Vec3d> endPoints, Action<TV, Vec3d> setPosition, double tolerance = 1.0e-8, bool allowMultiEdges = false, bool allowLoops = false)
        {
            var vertPos = endPoints.RemoveDuplicates(out int[] indexMap, tolerance);

            var result = Create(vertPos.Count, endPoints.Count >> 1);
            var verts = result.Vertices;
            var hedges = result.Halfedges;

            // add vertices
            for(int i = 0; i < vertPos.Count; i++)
            {
                var v = result.AddVertex();
                setPosition(v, vertPos[i]);
            }

            // add edges
            int mask = 0;
            if (allowMultiEdges) mask |= 1;
            if (allowLoops) mask |= 2;

            // 0 - neither allowed
            // 1 - no loops allowed
            // 2 - no multi-edges allowed
            // 3 - both allowed
            switch (mask)
            {
                case 0:
                    {
                        // no multi-edges or loops allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0 != v1 && (v0.IsRemoved || !v0.IsConnectedTo(v1))) result.AddEdgeImpl(v0, v1);
                        }
                        break;
                    }
                case 1:
                    {
                        // no loops allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0 != v1) result.AddEdgeImpl(v0, v1);
                        }
                        break;
                    }
                case 2:
                    {
                        // no multi-edges allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0.IsRemoved || !v0.IsConnectedTo(v1)) result.AddEdgeImpl(v0, v1);
                        }
                        break;
                    }
                case 3:
                    {
                        // both allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                            result.AddEdge(indexMap[i], indexMap[i + 1]);
                        break;
                    }
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public HeGraph<TV, TE> CreateFromVertexTopology<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UV> setVertex, Action<TE, UE> setHedge)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
            var result = Create(mesh.Vertices.Count, mesh.Halfedges.Count);
            result.AppendVertexTopology(mesh, setVertex, setHedge);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="setHedge"></param>
        /// <param name="setVertex"></param>
        /// <returns></returns>
        public HeGraph<TV, TE> CreateFromFaceTopology<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UF> setVertex, Action<TE, UE> setHedge)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
            var result = Create(mesh.Faces.Count, mesh.Halfedges.Count);
            result.AppendFaceTopology(mesh, setVertex, setHedge);
            return result;
        }
    }
}
