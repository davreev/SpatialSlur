
/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Drawing;

using Rhino.Geometry;
using SpatialSlur.Collections;
using SpatialSlur.Meshes;

using Vec3d = Rhino.Geometry.Vector3d;
using Vec3f = Rhino.Geometry.Vector3f;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class MeshExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeGraph3d ToHeGraph(this Mesh mesh)
        {
            return HeGraph3d.Factory.CreateFromVertexTopology(mesh);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh3d ToHeMesh(this Mesh mesh)
        {
            return HeMesh3d.Factory.CreateFromMesh(mesh);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToPolySoup(this Mesh mesh)
        {
            return RhinoFactory.Mesh.CreatePolySoup(mesh);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="getColor"></param>
        /// <param name="parallel"></param>
        public static void ColorVertices(this Mesh mesh, Func<int, Color> getColor, bool parallel = false)
        {
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            colors.Count = verts.Count;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                    colors[i] = getColor(i);
            }
        }


        /// <summary>
        /// Assumes the mesh is polygon soup (i.e. vertices aren't shared between faces).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getColor"></param>
        /// <param name="parallel"></param>
        public static void ColorFaces<T>(this Mesh mesh, Func<int, Color> getColor, bool parallel = false)
        {
            var faces = mesh.Faces;
            var colors = mesh.VertexColors;
            colors.Count = mesh.Vertices.Count;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    Color c = getColor(i);

                    int n = (f.IsQuad) ? 4 : 3;
                    for (int j = 0; j < n; j++) colors[f[j]] = c;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexValues"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Mesh IsoTrim(this Mesh mesh, IReadOnlyList<double> vertexValues, Intervald interval)
        {
            return RhinoFactory.Mesh.CreateIsoTrim(mesh, vertexValues, interval);
        }


        /// <summary>
        /// Returns the entries of the cotangent-weighted Laplacian matrix in row-major order.
        /// Based on symmetric derivation of the Laplace-Beltrami operator detailed in http://www.cs.jhu.edu/~misha/ReadingSeminar/Papers/Vallet08.pdf.
        /// Assumes triangle mesh.
        /// </summary>
        /// <returns></returns>
        public static void GetLaplacianMatrix(this Mesh mesh, double[] result)
        {
            GetLaplacianMatrix(mesh, result, new double[mesh.Vertices.Count]);
        }


        /// <summary>
        /// Returns the entries of the cotangent-weighted Laplacian matrix in row-major order.
        /// Based on symmetric derivation of the Laplace-Beltrami operator detailed in http://www.cs.jhu.edu/~misha/ReadingSeminar/Papers/Vallet08.pdf.
        /// Also returns the barycentric dual area of each vertex.
        /// Assumes triangle mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="entriesOut"></param>
        /// <param name="areasOut"></param>
        public static void GetLaplacianMatrix(this Mesh mesh, double[] entriesOut, double[] areasOut)
        {
            var verts = mesh.Vertices;
            int n = verts.Count;
            double t = 1.0 / 6.0;

            Array.Clear(entriesOut, 0, n * n);
            Array.Clear(areasOut, 0, n);

            // iterate faces to collect weights and vertex areas (upper triangular only)
            foreach (MeshFace mf in mesh.Faces)
            {
                // circulate verts in face
                for (int i = 0; i < 3; i++)
                {
                    int i0 = mf[i];
                    int i1 = mf[(i + 1) % 3];
                    int i2 = mf[(i + 2) % 3];

                    Vec3d v0 = verts[i0] - verts[i2];
                    Vec3d v1 = verts[i1] - verts[i2];

                    // add to vertex area
                    double a = Vec3d.CrossProduct(v0, v1).Length;
                    areasOut[i0] += a * t;

                    // add to edge cotangent weights (assumes consistent face orientation)
                    if (i1 < i0)
                        entriesOut[i0 * n + i1] += 0.5 * v0 * v1 / a;
                    else
                        entriesOut[i1 * n + i0] += 0.5 * v0 * v1 / a;
                }
            }

            // normalize weights with areas and sum along diagonals
            for (int i = 0; i < n; i++)
            {
                int ii = i * n + i;

                for (int j = i + 1; j < n; j++)
                {
                    double w = entriesOut[i * n + j];
                    w /= Math.Sqrt(areasOut[i] * areasOut[j]);

                    // set symmetric entries
                    entriesOut[i * n + j] = entriesOut[j * n + i] = w;

                    // sum along diagonal entries
                    entriesOut[j * n + j] -= w;
                    entriesOut[ii] -= w;
                }
            }
        }
    }
}

#endif