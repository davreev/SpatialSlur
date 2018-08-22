
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur;
using SpatialSlur.Meshes.Impl;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Meshes
{
    using G = HeGraph3d;
    using V = HeGraph3d.Vertex;
    using E = HeGraph3d.Halfedge;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeGraph3dFactory : HeGraphFactory<G, V, E>
    {
        /// <inheritdoc />
        public sealed override G Create(int vertexCapacity, int hedgeCapacity)
        {
            return new G(vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoints"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public G CreateFromLineSegments(IReadOnlyList<Vector3d> endPoints, double tolerance = D.ZeroTolerance, bool allowMultiEdges = false, bool allowLoops = false)
        {
            return CreateFromLineSegments(endPoints, (v, p) => v.Position = p, tolerance, allowMultiEdges, allowLoops);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public G CreateFromVertexTopology(HeMesh<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
            var graph = Create(mesh.Vertices.Count, mesh.Halfedges.Count);
            graph.AppendVertexTopology(mesh);
            return graph;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public G CreateFromFaceTopology(HeMesh<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
            var graph = Create(mesh.Faces.Count, mesh.Halfedges.Count);
            graph.AppendFaceTopology(mesh);
            return graph;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public G CreateFromJson(string path)
        {
            var result = Create();
            Interop.Meshes.ReadFromJson(path, result);
            return result;
        }
    }
}
