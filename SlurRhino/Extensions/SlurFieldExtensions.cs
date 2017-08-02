using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;

using Rhino.Geometry;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class SlurFieldExtensions
    {
        #region GridField2d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        static Mesh MeshSelection(this GridField2d field, IEnumerable<int> selection)
        {
            var mesh = new Mesh();
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            double dx = field.ScaleX * 0.5;
            double dy = field.ScaleY * 0.5;

            // add vertices
            foreach (int index in selection)
            {
                var p = field.CoordinateAt(index);
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

        #endregion


        #region GridField3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        static Mesh MeshSelection(this GridField3d field, IEnumerable<int> selection)
        {
            var mesh = new Mesh();
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            double dx = field.ScaleX * 0.5;
            double dy = field.ScaleY * 0.5;

            // add vertices
            foreach (int index in selection)
            {
                var p = field.CoordinateAt(index);
                verts.Add(p.X - dx, p.Y - dy, p.Z);
                verts.Add(p.X + dx, p.Y - dy, p.Z);
                verts.Add(p.X - dx, p.Y + dy, p.Z);
                verts.Add(p.X + dx, p.Y + dy, p.Z);
            }

            // add faces
            for (int i = 0; i < verts.Count; i += 4)
                faces.AddFace(i, i + 1, i + 3, i + 2);

            return mesh;
        }

        #endregion


        #region GridField2d<T>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static PointCloud ToPointCloud<T>(this GridField2d<T> field, Func<T, Color> mapper, bool parallel = false)
            where T : struct
        {
            var cloud = new PointCloud(field.Coordinates.Select(p => p.ToPoint3d()));
            var vals = field.Values;

            Action<Tuple<int, int>> body = range =>
             {
                 for (int i = range.Item1; i < range.Item2; i++)
                     cloud[i].Color = mapper(vals[i]);
             };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), body);
            else
                body(Tuple.Create(0, field.Count));

            return cloud;
        }
   

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static Mesh ToMesh<T>(this GridField2d<T> field, Func<T, Color> mapper)
            where T : struct
        {
            Mesh mesh = new Mesh();
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            var faces = mesh.Faces;

            var values = field.Values;
            int index = 0;

            for (int i = 0; i < field.CountY; i++)
            {
                for (int j = 0; j < field.CountX; j++)
                {
                    Vec2d p = field.CoordinateAt(j, i);
                    verts.Add(p.X, p.Y, 0.0);
                    colors.Add(mapper(values[index++]));
                }
            }

            int nx = field.CountX;
            int ny = field.CountY;

            for (int j = 0; j < ny - 1; j++)
            {
                for (int i = 0; i < nx - 1; i++)
                {
                    index = field.IndexAtUnchecked(i, j);
                    faces.AddFace(index, index + 1, index + 1 + nx, index + nx);
                }
            }

            return mesh;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static Mesh ToPolySoup<T>(this GridField2d<T> field, Func<T, Color> mapper)
            where T : struct
        {
            Mesh mesh = new Mesh();
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            var faces = mesh.Faces;

            double dx = field.ScaleX * 0.5;
            double dy = field.ScaleY * 0.5;

            var values = field.Values;
            int index = 0;

            // add vertices
            for (int i = 0; i < field.CountY; i++)
            {
                for (int j = 0; j < field.CountX; j++)
                {
                    Vec2d p = field.CoordinateAt(j, i);
                    verts.Add(p.X - dx, p.Y - dy, 0.0);
                    verts.Add(p.X + dx, p.Y - dy, 0.0);
                    verts.Add(p.X - dx, p.Y + dy, 0.0);
                    verts.Add(p.X + dx, p.Y + dy, 0.0);

                    var c = mapper(values[index++]);
                    for (int k = 0; k < 4; k++) colors.Add(c);
                }
            }

            // add faces
            for (int i = 0; i < verts.Count; i+= 4)
                faces.AddFace(i, i+1, i+3, i+2);
         
            return mesh;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        static Mesh MeshSelection<T>(this GridField2d<T> field, Func<T, Color> mapper, IEnumerable<int> selection)
            where T : struct
        {
            var mesh = new Mesh();
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            var faces = mesh.Faces;

            double dx = field.ScaleX * 0.5;
            double dy = field.ScaleY * 0.5;

            var values = field.Values;

            // add vertices
            foreach (int index in selection)
            {
                var p = field.CoordinateAt(index);
     
                verts.Add(p.X - dx, p.Y - dy, 0.0);
                verts.Add(p.X + dx, p.Y - dy, 0.0);
                verts.Add(p.X - dx, p.Y + dy, 0.0);
                verts.Add(p.X + dx, p.Y + dy, 0.0);

                var c = mapper(values[index]);
                for (int k = 0; k < 4; k++) colors.Add(c);
            }

            // add faces
            for (int i = 0; i < verts.Count; i += 4)
                faces.AddFace(i, i + 1, i + 3, i + 2);

            return mesh;
        }

        #endregion


        #region GridField3d<T>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static PointCloud ToPointCloud<T>(this GridField3d<T> field, Func<T, Color> mapper, bool parallel = false)
            where T : struct
        {
            var cloud = new PointCloud(field.Coordinates.Select(p => p.ToPoint3d()));
            var vals = field.Values;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    cloud[i].Color = mapper(vals[i]);
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), body);
            else
                body(Tuple.Create(0, field.Count));

            return cloud;
        }

        #endregion
    }
}
