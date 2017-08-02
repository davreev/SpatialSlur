using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

using static SpatialSlur.SlurMesh.HeGraph3d;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class HeGraphExtensions
    {
        /// <summary> </summary>
        private static Func<IVertex3d, Vec3d> _getPosition = v => v.Position;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        public static HeGraph<V, E> Duplicate(this HeGraph<V, E> graph)
        {
            return graph.Duplicate(Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="other"></param>
        public static void Append(this HeGraph<V, E> graph, HeGraph<V, E> other)
        {
            graph.Append(other, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static HeGraph<V, E>[] SplitDisjoint(this HeGraph<V, E> graph)
        {
            return graph.SplitDisjoint(Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public static HeGraph<V, E>[] SplitDisjoint(this HeGraph<V, E> graph, out int[] componentIndices, out int[] edgeIndices)
        {
            return graph.SplitDisjoint(Set, Set, out componentIndices, out edgeIndices);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public static HeGraph<V, E>[] SplitDisjoint(this HeGraph<V, E> graph, Property<E, int> componentIndex, Property<E, int> edgeIndex)
        {
            return graph.SplitDisjoint(Set, Set, componentIndex, edgeIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="mesh"></param>
        public static void AppendVertexTopology(this HeGraph<V, E> graph, HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F> mesh)
        {
            graph.AppendVertexTopology(mesh, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="mesh"></param>
        public static void AppendFaceTopology(this HeGraph<V, E> graph, HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F> mesh)
        {
            graph.AppendFaceTopology(mesh, Set, Set);
        }


        /// <summary> </summary>
        private static void Set(V v0, V v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary> </summary>
        private static void Set(E he0, E he1)
        {
            he0.Weight = he1.Weight;
        }


        /// <summary> </summary>
        private static void Set(V v0, HeMesh3d.V v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary> </summary>
        private static void Set(V v, HeMesh3d.F f)
        {
            v.Position = f.Vertices.Mean(_getPosition);
            v.Normal = f.Normal;
        }


        /// <summary> </summary>
        private static void Set(E he0, HeMesh3d.E he1)
        {
            he0.Weight = he1.Weight;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public static partial class HeGraphFactoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="endPoints"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public static HeGraph<V, E> CreateFromLineSegments(this HeGraphFactory<V, E> factory, IReadOnlyList<Vec3d> endPoints, double tolerance = 1.0e-8, bool allowMultiEdges = false, bool allowLoops = false)
        {
            return factory.CreateFromLineSegments(endPoints, (v, p) => v.Position = p, tolerance, allowMultiEdges, allowLoops);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeGraph<V, E> CreateFromVertexTopology(this HeGraphFactory<V, E> factory, HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F> mesh)
        {
            var graph = factory.Create(mesh.Vertices.Count, mesh.Halfedges.Count);
            graph.AppendVertexTopology(mesh);
            return graph;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeGraph<V, E> CreateFromFaceTopology(this HeGraphFactory<V, E> factory, HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F> mesh)
        {
            var graph = factory.Create(mesh.Faces.Count, mesh.Halfedges.Count);
            graph.AppendFaceTopology(mesh);
            return graph;
        }
    }
}
