using System;
using System.Collections.Generic;
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


        #endregion
    }
}
