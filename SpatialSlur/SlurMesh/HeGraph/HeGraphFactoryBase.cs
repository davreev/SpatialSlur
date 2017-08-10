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
    /// <typeparam name="TG"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    [Serializable]
    public abstract class HeGraphFactoryBase<TG, TV, TE> : IFactory<TG>
        where TG : HeGraphBase<TV, TE>
        where TV : HeVertex<TV, TE>
        where TE : Halfedge<TV, TE>
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
        public TG CreateCopy<UV, UE>(HeGraphBase<UV, UE> graph, Action<TV, UV> setVertex, Action<TE, UE> setHedge)
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
        /// <returns></returns>
        public TG[] CreateConnectedComponents<UV, UE>(HeGraphBase<UV, UE> graph, Action<TV, UV> setVertex, Action<TE, UE> setHedge)
            where UV : HeVertex<UV, UE>
            where UE : Halfedge<UV, UE>
        {
            return CreateConnectedComponents(graph, setVertex, setHedge, out int[] compIds, out int[] edgeIds);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public TG[] CreateConnectedComponents<UV, UE>(HeGraphBase<UV, UE> graph, Action<TV, UV> setVertex, Action<TE, UE> setHedge, out int[] componentIndices, out int[] edgeIndices)
          where UV : HeVertex<UV, UE>
          where UE : Halfedge<UV, UE>
        {
            int ne = graph.Edges.Count;
            componentIndices = new int[ne];
            edgeIndices = new int[ne];

            return CreateConnectedComponents(graph, setVertex, setHedge, ToProp(componentIndices), ToProp(edgeIndices));

            Property<UE, T> ToProp<T>(T[] values)
            {
                return Property.Create<UE, T>(he => values[he >> 1], (he, i) => values[he >> 1] = i);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public TG[] CreateConnectedComponents<UV, UE>(HeGraphBase<UV, UE> graph, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Property<UE, int> componentIndex, Property<UE, int> edgeIndex)
            where UV : HeVertex<UV, UE>
            where UE : Halfedge<UV, UE>
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
                if (heA.IsRemoved) continue;

                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.AddEdge();
                edgeIndex.Set(heA, heB.Index >> 1);
            }

            // set component halfedge->halfedge refs
            for (int i = 0; i < hedges.Count; i++)
            {
                var heA0 = hedges[i];
                if (heA0.IsRemoved) continue;

                // the component to which heA0 was copied
                var compHedges = comps[componentIndex.Get(heA0)].Halfedges;
                var heA1 = heA0.NextAtStart;

                // set refs
                var heB0 = compHedges[(edgeIndex.Get(heA0) << 1) + (i & 1)];
                var heB1 = compHedges[(edgeIndex.Get(heA1) << 1) + (heA1.Index & 1)];
                heB0.MakeConsecutive(heB1);
                setHedge(heB0, heA0);
            }

            // create component vertices
            for (int i = 0; i < vertices.Count; i++)
            {
                var vA = vertices[i];
                if (vA.IsRemoved) continue;

                var heA = vA.FirstOut;
                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.Halfedges[(edgeIndex.Get(heA) << 1) + (heA.Index & 1)];

                // set vertex refs
                var vB = comp.AddVertex();
                vB.FirstOut = heB;

                foreach (var he in heB.CirculateStart)
                    he.Start = vB;

                setVertex(vB, vA);
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
        public TG CreateFromLineSegments(IReadOnlyList<Vec3d> points, Action<TV, Vec3d> setPosition, double tolerance = 1.0e-8, bool allowMultiEdges = false, bool allowLoops = false)
        {
            var vertPos = points.RemoveCoincident(out int[] indexMap, tolerance);

            var result = Create(vertPos.Count, points.Count >> 1);
            var verts = result.Vertices;
            var hedges = result.Halfedges;

            // add vertices
            for (int i = 0; i < vertPos.Count; i++)
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
        public TG CreateFromVertexTopology<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UV> setVertex, Action<TE, UE> setHedge)
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
        public TG CreateFromFaceTopology<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UF> setVertex, Action<TE, UE> setHedge)
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
