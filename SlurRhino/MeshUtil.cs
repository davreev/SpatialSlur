using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class MeshUtil
    {
        /// <summary>
        /// 
        /// </summary>
        public static Mesh Extrude(Polyline polyline, Vector3d direction)
        {
            if (polyline.IsClosed)
                return ExtrudeClosed(polyline, direction);
            else
                return ExtrudeOpen(polyline, direction);
        }


        /// <summary>
        /// 
        /// </summary>
        private static Mesh ExtrudeOpen(Polyline polyline, Vector3d direction)
        {
            Mesh result = new Mesh();
            var verts = result.Vertices;
            var faces = result.Faces;

            int n = polyline.Count;

            // add vertices
            for (int i = 0; i < n; i++)
                verts.Add(polyline[i]);

            for (int i = 0; i < n; i++)
                verts.Add(polyline[i] + direction);

            // add faces
            for (int i = 0; i < n - 1; i++)
                faces.AddFace(i, i + 1, i + n + 1, i + n);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static Mesh ExtrudeClosed(Polyline polyline, Vector3d direction)
        {
            Mesh result = new Mesh();
            var verts = result.Vertices;
            var faces = result.Faces;

            int n = polyline.Count - 1;

            // add verts
            for(int i = 0; i < n; i++)
                verts.Add(polyline[i]);

            for (int i = 0; i < n; i++)
                verts.Add(polyline[i] + direction);

            // add faces
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                faces.AddFace(i, j, j + n, i + n);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Mesh Loft(Polyline polylineA, Polyline polylineB)
        {
            if (polylineA.IsClosed && polylineB.IsClosed)
                return LoftClosed(polylineA, polylineB);
            else
                return LoftOpen(polylineA, polylineB);
        }


        /// <summary>
        /// 
        /// </summary>
        private static Mesh LoftOpen(Polyline polylineA, Polyline polylineB)
        {
            Mesh result = new Mesh();
            var verts = result.Vertices;
            var faces = result.Faces;

            int n = polylineA.Count;

            // add vertices
            verts.AddVertices(polylineA);
            verts.AddVertices(polylineB);

            // add faces
            for (int i = 0; i < n - 1; i++)
                faces.AddFace(i, i + 1, i + n + 1, i + n);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static Mesh LoftClosed(Polyline polylineA, Polyline polylineB)
        {
            Mesh result = new Mesh();
            var verts = result.Vertices;
            var faces = result.Faces;

            int n = polylineA.Count - 1;

            // add verts
            for (int i = 0; i < n; i++)
                verts.Add(polylineA[i]);

            for (int i = 0; i < n; i++)
                verts.Add(polylineB[i]);

            // add faces
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                faces.AddFace(i, j, j + n, i + n);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Mesh Loft(IList<Polyline> polylines)
        {
            if (Enumerable.All(polylines, p => p.IsClosed))
                return LoftClosed(polylines);
            else
                return LoftOpen(polylines);
        }


        /// <summary>
        /// 
        /// </summary>
        private static Mesh LoftOpen(IList<Polyline> polylines)
        {
            Mesh result = new Mesh();
            var verts = result.Vertices;
            var faces = result.Faces;

            int ny = polylines.Count;
            int nx = Enumerable.Min(polylines, p => p.Count);
            int n;

            // add vertices
            for (int i = 0; i < ny; i++)
            {
                var poly = polylines[i];

                for (int j = 0; j < nx; j++)
                    verts.Add(poly[j]);
            }

            // add faces
            for (int i = 0; i < ny - 1; i++)
            {
                n = i * nx;

                for (int j = 0; j < nx - 1; j++)
                    faces.AddFace(n + j, n + j + 1, n + j + nx + 1, n + j + nx);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static Mesh LoftClosed(IList<Polyline> polylines)
        {
            Mesh result = new Mesh();
            var verts = result.Vertices;
            var faces = result.Faces;

            int ny = polylines.Count;
            int nx = Enumerable.Min(polylines, p => p.Count) - 1;
            int n;

            // add vertices
            for (int i = 0; i < ny; i++)
            {
                var poly = polylines[i];

                for (int j = 0; j < nx; j++)
                    verts.Add(poly[j]);
            }

            // add faces
            for (int i = 0; i < ny - 1; i++)
            {
                n = i * nx;

                for (int j0 = 0; j0 < nx; j0++)
                {
                    int j1 = (j0 + 1) % nx;
                    faces.AddFace(n + j0, n + j1, n + j1 + nx, n + j0 + nx);
                }
            }

            return result;
        }
    }
}
