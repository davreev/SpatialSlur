using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{
    using M = HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F>;
    using G = HeGraph<HeGraph3d.V, HeGraph3d.E>;


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
        public static G ToHeGraph(this Mesh mesh)
        {
            return HeGraph3d.Factory.CreateFromVertexTopology(mesh);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static M ToHeMesh(this Mesh mesh)
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
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            Mesh newMesh = new Mesh();
            var newVerts = newMesh.Vertices;
            var newFaces = newMesh.Faces;

            // add verts and faces
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                int nv = newVerts.Count;

                if (f.IsTriangle)
                {
                    for (int j = 0; j < 3; j++)
                        newVerts.Add(verts[f[j]]);

                    newFaces.AddFace(nv, nv + 1, nv + 2);
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                        newVerts.Add(verts[f[j]]);

                    newFaces.AddFace(nv, nv + 1, nv + 2, nv + 3);
                }
            }

            return newMesh;
        }


        /// <summary>
        /// Assumes the number of vertices in the mesh equals the number of given values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="values"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public static void PaintByVertexValue<T>(this Mesh mesh, IReadOnlyList<T> values, Func<T, Color> mapper, bool parallel = false)
        {
            var colors = mesh.VertexColors;
            colors.Count = values.Count;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    colors[i] = mapper(values[i]);
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, values.Count), func);
            else
                func(Tuple.Create(0, values.Count));
        }


        /// <summary>
        /// Assumes the number of vertices in the mesh equals the number of given values.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public static void PaintByVertexPosition(this Mesh mesh, Func<Point3f, Color> mapper, bool parallel = false)
        {
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            colors.Count = verts.Count;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    colors[i] = mapper(verts[i]);
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), func);
            else
                func(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Assumes the number of vertices in the mesh equals the number of given values.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public static void PaintByVertexNormal(this Mesh mesh, Func<Vector3f, Color> mapper, bool parallel = false)
        {
            var norms = mesh.Normals;
            var colors = mesh.VertexColors;
            colors.Count = norms.Count;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    colors[i] = mapper(norms[i]);
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, norms.Count), func);
            else
                func(Tuple.Create(0, norms.Count));
        }


        /// <summary>
        /// Assumes the mesh is polygon soup (i.e. vertices aren't shared between faces).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="values"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public static void PaintByFaceValue<T>(this Mesh mesh, IReadOnlyList<T> values, Func<T, Color> mapper, bool parallel = false)
        {
            var faces = mesh.Faces;
            var colors = mesh.VertexColors;
            colors.Count = mesh.Vertices.Count;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (!f.IsValid()) continue; // skip invalid faces

                    Color c = mapper(values[i]);
                    int n = (f.IsQuad) ? 4 : 3;
                    for (int j = 0; j < n; j++) colors[f[j]] = c;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexValues"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Mesh IsoTrim(this Mesh mesh, IReadOnlyList<double> vertexValues, Domain domain)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the entries of the cotangent-weighted Laplacian matrix in column-major order.
        /// Based on symmetric derivation of the Laplace-Beltrami operator detailed in http://www.cs.jhu.edu/~misha/ReadingSeminar/Papers/Vallet08.pdf.
        /// Assumes triangle mesh.
        /// </summary>
        /// <returns></returns>
        public static void GetLaplacianMatrix(this Mesh mesh, double[] result)
        {
            GetLaplacianMatrix(mesh, result, new double[mesh.Vertices.Count]);
        }


        /// <summary>
        /// Returns the entries of the cotangent-weighted Laplacian matrix in column-major order.
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

            // iterate faces to collect weights and vertex areas (lower triangular only)
            foreach (MeshFace mf in mesh.Faces)
            {
                // circulate verts in face
                for (int i = 0; i < 3; i++)
                {
                    int i0 = mf[i];
                    int i1 = mf[(i + 1) % 3];
                    int i2 = mf[(i + 2) % 3];

                    Vector3d v0 = verts[i0] - verts[i2];
                    Vector3d v1 = verts[i1] - verts[i2];

                    // add to vertex area
                    double a = Vector3d.CrossProduct(v0, v1).Length;
                    areasOut[i0] += a * t;

                    // add to edge cotangent weights (assumes consistent face orientation)
                    if (i1 > i0)
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
                    entriesOut[i * n + j] = w;
                    entriesOut[j * n + i] = w;

                    // sum along diagonal entries
                    entriesOut[ii] -= w;
                    entriesOut[j * n + j] -= w;
                }
            }
        }
    }
}
