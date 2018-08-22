
/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Collections.Concurrent;
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
    public static class QuadStripExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="strip"></param>
        /// <returns></returns>
        public static Mesh ToMesh<V, E, F>(this QuadStrip<V, E, F> strip)
            where V : HeMesh<V, E, F>.Vertex, IPosition3d
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return ToMesh(strip, v => v.Position.As3f);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="strip"></param>
        /// <param name="getPosition"></param>
        /// <param name="getNormal"></param>
        /// <param name="getTexture"></param>
        /// <param name="getColor"></param>
        /// <returns></returns>
        public static Mesh ToMesh<V, E, F>(this QuadStrip<V, E, F> strip, Func<V, Point3f> getPosition, Func<V, Vec3f> getNormal = null, Func<V, Point2f> getTexture = null, Func<V, Color> getColor = null)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return RhinoFactory.Mesh.CreateFromQuadStrip(strip, getPosition, getNormal, getTexture, getColor);
        }
    }
}

#endif