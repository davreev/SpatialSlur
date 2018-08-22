
/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Linq;
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
    public static class HeMeshFactoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="factory"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static M CreateFromMesh<M, V, E, F>(this HeMeshFactory<M, V, E, F> factory, Mesh mesh)
            where M : HeMesh<V, E, F>
            where V : HeMesh<V, E, F>.Vertex, IPosition3d, INormal3d
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return factory.CreateFromMesh(
                mesh,
                (v, p) => v.Position = (Vector3f)p,
                (v, n) => v.Normal = (Vector3f)n
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="factory"></param>
        /// <param name="mesh"></param>
        /// <param name="setPosition"></param>
        /// <param name="setNormal"></param>
        /// <param name="setTexture"></param>
        /// <param name="setColor"></param>
        /// <returns></returns>
        public static M CreateFromMesh<M, V, E, F>(this HeMeshFactory<M, V, E, F> factory, Mesh mesh, Action<V, Point3f> setPosition, Action<V, Vec3f> setNormal = null, Action<V, Point2f> setTexture = null, Action<V, Color> setColor = null)
            where M : HeMesh<V, E, F>
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            var verts = mesh.Vertices;
            int nv = verts.Count;

            var faces = mesh.Faces;
            int nf = faces.Count;

            var norms = mesh.Normals;
            var coords = mesh.TextureCoordinates;
            var colors = mesh.VertexColors;

            int mask = 0;
            if (setNormal != null && norms.Count == nv) mask |= 1;
            if (setTexture != null && coords.Count == nv) mask |= 2;
            if (setColor != null && colors.Count == nv) mask |= 4;

            var result = factory.Create(nv, nv << 3, nf);

            // add vertices
            for (int i = 0; i < nv; i++)
            {
                var v = result.AddVertex();
                setPosition(v, verts[i]);

                if ((mask & 1) > 0)
                    setNormal(v, norms[i]);

                if ((mask & 2) > 0)
                    setTexture(v, coords[i]);

                if ((mask & 4) > 0)
                    setColor(v, colors[i]);
            }

            // add faces
            for (int i = 0; i < nf; i++)
            {
                MeshFace f = faces[i];
                if (f.IsQuad)
                    result.AddFace(f.A, f.B, f.C, f.D);
                else
                    result.AddFace(f.A, f.B, f.C);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="factory"></param>
        /// <param name="polylines"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static M CreateFromPolylines<M, V, E, F>(this HeMeshFactory<M, V, E, F> factory, IEnumerable<Polyline> polylines, double tolerance = D.ZeroTolerance)
            where M : HeMesh<V, E, F>
            where V : HeMesh<V, E, F>.Vertex, IPosition3d
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return factory.CreateFromPolylines(polylines, (v, p) => v.Position = p, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="factory"></param>
        /// <param name="polylines"></param>
        /// <param name="setPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static M CreateFromPolylines<M, V, E, F>(this HeMeshFactory<M, V, E, F> factory, IEnumerable<Polyline> polylines, Action<V, Vector3d> setPosition, double tolerance = D.ZeroTolerance)
            where M : HeMesh<V, E, F>
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            var vertices = Flatten().RemoveCoincident(out List<int> indexMap, tolerance);
            var faces = polylines.Select(p => Count(p));

            return factory.CreateFromFaceVertexData(vertices, indexMap.Batch(faces), setPosition);

            IEnumerable<Vector3d> Flatten()
            {
                foreach (Polyline p in polylines)
                {
                    int n = Count(p);
                    for (int i = 0; i < n; i++)
                        yield return p[i];
                }
            }

            int Count(Polyline polyline)
            {
                return polyline.IsClosed ? polyline.Count - 1 : polyline.Count;
            }
        }
    }
}

#endif