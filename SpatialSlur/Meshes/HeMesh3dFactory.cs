
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
    using M = HeMesh3d;
    using V = HeMesh3d.Vertex;
    using E = HeMesh3d.Halfedge;
    using F = HeMesh3d.Face;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMesh3dFactory : HeMeshFactory<M, V, E, F>
    {
        /// <inheritdoc />
        public sealed override M Create(int vertexCapacity, int halfedgeCapacity, int faceCapacity)
        {
            return new M(vertexCapacity, halfedgeCapacity, faceCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M CreateFromFaceVertexData<T>(IEnumerable<Vector3d> vertices, IEnumerable<T> faces)
            where T : IEnumerable<int>
        {
            return CreateFromFaceVertexData(vertices, faces, (v, p) => v.Position = p);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M CreateFromPolygons<T>(IEnumerable<T> polygons, double tolerance = D.ZeroTolerance)
            where T : IEnumerable<Vector3d>
        {
            return CreateFromPolygons(polygons, (v, p) => v.Position = p, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M CreateFromOBJ(string path)
        {
            return CreateFromObj(path, (v, p) => v.Position = p);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public M CreateFromJson(string path)
        {
            var result = Create();
            Interop.Meshes.ReadFromJson(path, result);
            return result;
        }
    }
}
