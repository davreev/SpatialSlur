
/*
 * Notes
 */

using System;
using System.Collections.Generic;

using SpatialSlur;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Meshes.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TG"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    [Serializable]
    public abstract class HeGraphFactory<TG, TV, TE>
        where TG : HeGraph<TV, TE>
        where TV : HeGraph<TV, TE>.Vertex
        where TE : HeGraph<TV, TE>.Halfedge
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TG Create()
        {
            return Create(4, 4);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public abstract TG Create(int vertexCapacity, int hedgeCapacity);
        

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public TG CreateCopy<UV, UE>(HeGraph<UV, UE> graph, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraph<UV, UE>.Vertex
            where UE : HeGraph<UV, UE>.Halfedge
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
        /// <returns></returns>
        public TG[] CreateConnectedComponents<UV, UE>(HeGraph<UV, UE> graph, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraph<UV, UE>.Vertex
            where UE : HeGraph<UV, UE>.Halfedge
        {
            return CreateConnectedComponents(graph, out int[] compIds, out int[] edgeIds, setVertex, setHedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public TG[] CreateConnectedComponents<UV, UE>(HeGraph<UV, UE> graph, out int[] componentIndices, out int[] edgeIndices, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraph<UV, UE>.Vertex
            where UE : HeGraph<UV, UE>.Halfedge
        {
            int ne = graph.Edges.Count;
            componentIndices = new int[ne];
            edgeIndices = new int[ne];
            
            return CreateConnectedComponents(
                graph,
                HeGraph<UV, UE>.Edge.CreateProperty(componentIndices),
                HeGraph<UV, UE>.Edge.CreateProperty(edgeIndices),
                setVertex,
                setHedge
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public TG[] CreateConnectedComponents<UV, UE>(HeGraph<UV, UE> graph, Property<UE, int> componentIndex, Property<UE, int> edgeIndex, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraph<UV, UE>.Vertex
            where UE : HeGraph<UV, UE>.Halfedge
        {
            var vertices = graph.Vertices;
            var hedges = graph.Halfedges;

            int ncomp = graph.GetEdgeComponentIndices(componentIndex.Set);
            var comps = new TG[ncomp];

            // initialize components
            for (int i = 0; i < comps.Length; i++)
                comps[i] = Create();

            // create component halfedges
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var heA = hedges[i];
                if (heA.IsUnused) continue;

                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.AddEdge();
                edgeIndex.Set(heA, heB.Index >> 1);
            }

            // set component halfedge->halfedge refs
            for (int i = 0; i < hedges.Count; i++)
            {
                var heA0 = hedges[i];
                if (heA0.IsUnused) continue;

                // the component to which heA0 was copied
                var compHedges = comps[componentIndex.Get(heA0)].Halfedges;
                var heA1 = heA0.NextAtStart;

                // set refs
                var heB0 = compHedges[(edgeIndex.Get(heA0) << 1) + (i & 1)];
                var heB1 = compHedges[(edgeIndex.Get(heA1) << 1) + (heA1.Index & 1)];
                heB0.MakeConsecutive(heB1);

                // transfer attributes
                setHedge?.Invoke(heB0, heA0);
            }

            // create component vertices
            for (int i = 0; i < vertices.Count; i++)
            {
                var vA = vertices[i];
                if (vA.IsUnused) continue;

                var heA = vA.First;
                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.Halfedges[(edgeIndex.Get(heA) << 1) + (heA.Index & 1)];

                // set vertex refs
                var vB = comp.AddVertex();
                vB.First = heB;

                foreach (var he in heB.CirculateStart)
                    he.Start = vB;

                // transfer attributes
                setVertex?.Invoke(vB, vA);
            }

            return comps;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="setPosition"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public TG CreateFromLineSegments(IEnumerable<Vector3d> points, Action<TV, Vector3d> setPosition, double tolerance = D.ZeroTolerance, bool allowMultiEdges = false, bool allowLoops = false)
        {
            var positions = points.RemoveCoincident(out List<int> indexMap, tolerance);
            var result = Create(positions.Count, indexMap.Count >> 1);
            var verts = result.Vertices;
            var hedges = result.Halfedges;

            // add vertices
            for (int i = 0; i < positions.Count; i++)
                setPosition(result.AddVertex(), positions[i]);

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
                        for (int i = 0; i < indexMap.Count; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0 != v1 && (v0.IsUnused || !v0.IsConnectedTo(v1))) result.AddEdgeImpl(v0, v1);
                        }
                        break;
                    }
                case 1:
                    {
                        // no loops allowed
                        for (int i = 0; i < indexMap.Count; i += 2)
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
                        for (int i = 0; i < indexMap.Count; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0.IsUnused || !v0.IsConnectedTo(v1)) result.AddEdgeImpl(v0, v1);
                        }
                        break;
                    }
                case 3:
                    {
                        // both allowed
                        for (int i = 0; i < indexMap.Count; i += 2)
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
        public TG CreateFromVertexTopology<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
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
        public TG CreateFromFaceTopology<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UF> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            var result = Create(mesh.Faces.Count, mesh.Halfedges.Count);
            result.AppendFaceTopology(mesh, setVertex, setHedge);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="setVertexAttributes"></param>
        /// <param name="setHedgeAttributes"></param>
        /// <returns></returns>
        public TG CreateFromJson(string path, Action<TV, object[]> setVertexAttributes = null, Action<TE, object[]> setHedgeAttributes = null)
        {
            var result = Create();
            Interop.Meshes.ReadFromJson(path, result, setVertexAttributes, setHedgeAttributes);
            return result;
        }
    }
}