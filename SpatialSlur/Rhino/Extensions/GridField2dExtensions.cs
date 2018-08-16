
/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Drawing;
using Rhino.Geometry;
using SpatialSlur;
using SpatialSlur.Fields;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField2dExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="getColor"></param>
        /// <returns></returns>
        public static Mesh ToMesh<T>(this GridField2d<T> field, Func<T, Color> getColor)
            where T : struct
        {
            return ToMesh(field, getColor, t => 0.0);
        }


        /// <summary>
        /// Converts the field to a colored heightfield mesh.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="getColor"></param>
        /// <param name="getHeight"></param>
        /// <returns></returns>
        public static Mesh ToMesh<T>(this GridField2d<T> field, Func<T, Color> getColor, Func<T, double> getHeight)
            where T : struct
        {
            var mesh = new Mesh();
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            var faces = mesh.Faces;

            var values = field.Values;
            (var nx, var ny) = field.Count;
            int index = 0;

            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    var t = values[index++];

                    Vector2d p = field.ToWorldSpace(new Vector2d(x, y));
                    verts.Add(p.X, p.Y, getHeight(t));
                    colors.Add(getColor(t));
                }
            }
            
            for (int y = 0; y < ny - 1; y++)
            {
                for (int x = 0; x < nx - 1; x++)
                {
                    index = field.ToIndexUnsafe(new Vector2i(x, y));
                    faces.AddFace(index, index + 1, index + 1 + nx, index + nx);
                }
            }

            return mesh;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public static Mesh ToPolySoup(this Grid2d field, IEnumerable<int> selection)
        {
            var mesh = new Mesh();
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            (var dx, var dy) = (field.Scale * 0.5);

            // add vertices
            foreach (int index in selection)
            {
                var p = field.ToWorldSpace(index);
                verts.Add(p.X - dx, p.Y - dy, 0.0);
                verts.Add(p.X + dx, p.Y - dy, 0.0);
                verts.Add(p.X - dx, p.Y + dy, 0.0);
                verts.Add(p.X + dx, p.Y + dy, 0.0);
            }

            // add faces
            for (int i = 0; i < verts.Count; i += 4)
                faces.AddFace(i, i + 1, i + 3, i + 2);

            return mesh;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="getColor"></param>
        /// <returns></returns>
        public static Mesh ToPolySoup<T>(this GridField2d<T> field, Func<T, Color> getColor)
            where T : struct
        {
            var mesh = new Mesh();
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            var faces = mesh.Faces;

            var values = field.Values;
            (var nx, var ny) = field.Count;
            (var dx, var dy) = (field.Scale * 0.5);

            int index = 0;

            // add vertices
            for (int x = 0; x < ny; x++)
            {
                for (int y = 0; y < nx; y++)
                {
                    var p = field.ToWorldSpace(new Vector2d(y, x));
                    verts.Add(p.X - dx, p.Y - dy, 0.0);
                    verts.Add(p.X + dx, p.Y - dy, 0.0);
                    verts.Add(p.X - dx, p.Y + dy, 0.0);
                    verts.Add(p.X + dx, p.Y + dy, 0.0);

                    var c = getColor(values[index++]);
                    for (int k = 0; k < 4; k++) colors.Add(c);
                }
            }

            // add faces
            for (int i = 0; i < verts.Count; i += 4)
                faces.AddFace(i, i + 1, i + 3, i + 2);

            return mesh;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="getColor"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public static Mesh ToPolySoup<T>(this GridField2d<T> field, Func<T, Color> getColor, IEnumerable<int> selection)
            where T : struct
        {
            var mesh = new Mesh();
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            var faces = mesh.Faces;

            var values = field.Values;
            (var dx, var dy) = (field.Scale * 0.5);

            // add vertices
            foreach (int index in selection)
            {
                var p = field.ToWorldSpace(index);

                verts.Add(p.X - dx, p.Y - dy, 0.0);
                verts.Add(p.X + dx, p.Y - dy, 0.0);
                verts.Add(p.X - dx, p.Y + dy, 0.0);
                verts.Add(p.X + dx, p.Y + dy, 0.0);

                var c = getColor(values[index]);
                for (int k = 0; k < 4; k++) colors.Add(c);
            }

            // add faces
            for (int i = 0; i < verts.Count; i += 4)
                faces.AddFace(i, i + 1, i + 3, i + 2);

            return mesh;
        }
    }
}

#endif