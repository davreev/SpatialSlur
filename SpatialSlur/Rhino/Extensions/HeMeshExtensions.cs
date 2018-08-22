
/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Drawing;

using Rhino.Geometry;
using SpatialSlur.Meshes;
using SpatialSlur.Meshes.Impl;

using Vec3d = Rhino.Geometry.Vector3d;
using Vec3f = Rhino.Geometry.Vector3f;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeMeshExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="quadrangulator"></param>
        /// <returns></returns>
        public static Mesh ToMesh<V, E, F>(this HeMesh<V, E, F> mesh, IFaceQuadrangulator quadrangulator = null)
            where V : HeMesh<V, E, F>.Vertex, IPosition3d
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return RhinoFactory.Mesh.CreateFromHeMesh(mesh, v => v.Position.As3f, null, null, null, quadrangulator);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="quadrangulator"></param>
        /// <param name="getPosition"></param>
        /// <param name="getNormal"></param>
        /// <param name="getTexture"></param>
        /// <param name="getColor"></param>
        /// <returns></returns>
        public static Mesh ToMesh<V, E, F>(this HeMesh<V, E, F> mesh, Func<V, Point3f> getPosition, Func<V, Vec3f> getNormal = null, Func<V, Point2f> getTexture = null, Func<V, Color> getColor = null, IFaceQuadrangulator quadrangulator = null)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return RhinoFactory.Mesh.CreateFromHeMesh(mesh, getPosition, getNormal, getTexture, getColor, quadrangulator);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getColor"></param>
        /// <param name="quadrangulator"></param>
        /// <returns></returns>
        public static Mesh ToPolySoup<V, E, F>(this HeMesh<V, E, F> mesh, Func<F, Color> getColor = null, IFaceQuadrangulator quadrangulator = null)
            where V : HeMesh<V, E, F>.Vertex, IPosition3d
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return RhinoFactory.Mesh.CreatePolySoup(mesh, v => v.Position.As3f, getColor, quadrangulator);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getPosition"></param>
        /// <param name="getColor"></param>
        /// <param name="quadrangulator"></param>
        /// <returns></returns>
        public static Mesh ToPolySoup<V, E, F>(this HeMesh<V, E, F> mesh, Func<V, Point3f> getPosition, Func<F, Color> getColor = null, IFaceQuadrangulator quadrangulator = null)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return RhinoFactory.Mesh.CreatePolySoup(mesh, getPosition, getColor, quadrangulator);
        }
    }
}

#endif