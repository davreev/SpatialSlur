
/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Drawing;

using Rhino.Geometry;
using SpatialSlur;
using SpatialSlur.Meshes;
using SpatialSlur.Meshes.Impl;

using Vec3d = Rhino.Geometry.Vector3d;
using Vec3f = Rhino.Geometry.Vector3f;
using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeGraphFactoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="lines"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public static G CreateFromLineSegments<G, V, E>(this HeGraphFactory<G, V, E> factory, IEnumerable<Line> lines, double tolerance = D.ZeroTolerance, bool allowMultiEdges = false, bool allowLoops = false)
            where G : HeGraph<V, E>
            where V : HeGraph<V, E>.Vertex, IPosition3d
            where E : HeGraph<V, E>.Halfedge
        {
            return factory.CreateFromLineSegments(lines, (v, p) => v.Position = p, tolerance, allowMultiEdges, allowLoops);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="factory"></param>
        /// <param name="lines"></param>
        /// <param name="setPosition"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public static G CreateFromLineSegments<G, V, E>(this HeGraphFactory<G, V, E> factory, IEnumerable<Line> lines, Action<V, Vector3d> setPosition, double tolerance = D.ZeroTolerance, bool allowMultiEdges = false, bool allowLoops = false)
            where G : HeGraph<V, E>
            where V : HeGraph<V, E>.Vertex
            where E : HeGraph<V, E>.Halfedge
        {
            var pts = new List<Vector3d>();
            foreach (var ln in lines)
            {
                pts.Add(ln.From);
                pts.Add(ln.To);
            }

            return factory.CreateFromLineSegments(pts, setPosition, tolerance, allowMultiEdges, allowLoops);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="factory"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static G CreateFromVertexTopology<G, V, E>(this HeGraphFactory<G, V, E> factory, Mesh mesh)
            where G : HeGraph<V, E>
            where V : HeGraph<V, E>.Vertex, IPosition3d, INormal3d
            where E : HeGraph<V, E>.Halfedge
        {
            return factory.CreateFromVertexTopology(
                mesh,
                (v, p) => v.Position = (Vector3f)p,
                (v, n) => v.Normal = (Vector3f)n
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="factory"></param>
        /// <param name="mesh"></param>
        /// <param name="setPosition"></param>
        /// <param name="setNormal"></param>
        /// <param name="setTexture"></param>
        /// <param name="setColor"></param>
        /// <returns></returns>
        public static G CreateFromVertexTopology<G, V, E>(this HeGraphFactory<G, V, E> factory, Mesh mesh, Action<V, Point3f> setPosition, Action<V, Vec3f> setNormal = null, Action<V, Point2f> setTexture = null, Action<V, Color> setColor = null)
            where G : HeGraph<V, E>
            where V : HeGraph<V, E>.Vertex
            where E : HeGraph<V, E>.Halfedge
        {
            // TODO implement
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="factory"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static G CreateFromFaceTopology<G, V, E>(this HeGraphFactory<G, V, E> factory, Mesh mesh)
            where G : HeGraph<V, E>
            where V : HeGraph<V, E>.Vertex, IPosition3d, INormal3d
            where E : HeGraph<V, E>.Halfedge
        {
            return factory.CreateFromFaceTopology(
                mesh,
                (v, p) => v.Position = (Vector3f)p,
                (v, n) => v.Normal = (Vector3f)n
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="factory"></param>
        /// <param name="mesh"></param>
        /// <param name="setPosition"></param>
        /// <param name="setNormal"></param>
        /// <param name="setTexture"></param>
        /// <param name="setColor"></param>
        /// <returns></returns>
        public static G CreateFromFaceTopology<G, V, E>(this HeGraphFactory<G, V, E> factory, Mesh mesh, Action<V, Point3f> setPosition, Action<V, Vec3f> setNormal = null, Action<V, Point2f> setTexture = null, Action<V, Color> setColor = null)
            where G : HeGraph<V, E>
            where V : HeGraph<V, E>.Vertex
            where E : HeGraph<V, E>.Halfedge
        {
            // TODO implement
            throw new NotImplementedException();
        }
    }
}

#endif