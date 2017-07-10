using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;

using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class RhinoExtensions
    {
        #region Point3d


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="other"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Point3d LerpTo(this Point3d point, Point3d other, double t)
        {
            return point + (other - point) * t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double SquareDistanceTo(this Point3d point, Point3d other)
        {
            Vector3d v = other - point;
            return v.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="other"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3d LerpTo(this Vector3d vector, Vector3d other, double t)
        {
            return vector + (other - vector) * t;
        }


        #endregion


        #region Plane


        /// <summary>
        /// Returns the transform matrix described by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform ToWorld(this Plane plane)
        {
            Point3d o = plane.Origin;
            Vector3d x = plane.XAxis;
            Vector3d y = plane.YAxis;
            Vector3d z = plane.ZAxis;

            Transform m = new Transform();

            m[0, 0] = x.X;
            m[0, 1] = y.X;
            m[0, 2] = z.X;
            m[0, 3] = o.X;

            m[1, 0] = x.Y;
            m[1, 1] = y.Y;
            m[1, 2] = z.Y;
            m[1, 3] = o.Y;

            m[2, 0] = x.Z;
            m[2, 1] = y.Z;
            m[2, 2] = z.Z;
            m[2, 3] = o.Z;

            m[3, 3] = 1.0;

            return m;
        }


        /// <summary>
        /// Returns the inverse transformation matrix described by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform ToLocal(this Plane plane)
        {
            Vector3d d = new Vector3d(plane.Origin);
            Vector3d x = plane.XAxis;
            Vector3d y = plane.YAxis;
            Vector3d z = plane.ZAxis;

            Transform m = new Transform();

            m[0, 0] = x.X;
            m[0, 1] = x.Y;
            m[0, 2] = x.Z;
            m[0, 3] = -(d * x);

            m[1, 0] = y.X;
            m[1, 1] = y.Y;
            m[1, 2] = y.Z;
            m[1, 3] = -(d * y);

            m[2, 0] = z.X;
            m[2, 1] = z.Y;
            m[2, 2] = z.Z;
            m[2, 3] = -(d * z);

            m[3, 3] = 1.0;

            return m;
        }


        #endregion


        #region Mesh


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
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="values"></param>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="values"></param>
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


        #endregion


        #region IReadOnlyList<Point3d>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, double tolerance)
        {
            return RemoveDuplicatePoints(points, tolerance, out int[] indexMap, out RTree tree);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, double tolerance, out int[] indexMap)
        {
            return RemoveDuplicatePoints(points, tolerance, out indexMap, out RTree tree);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <param name="indexMap"></param>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, double tolerance, out int[] indexMap, out RTree tree)
        {
            indexMap = new int[points.Count];
            tree = new RTree();

            List<Point3d> result = new List<Point3d>();
            Vector3d span = new Vector3d(tolerance, tolerance, tolerance);

            // for each point, search for coincident points in the tree
            for (int i = 0; i < points.Count; i++)
            {
                Point3d pt = points[i];
                var index = -1;
                tree.Search(new BoundingBox(pt - span, pt + span), (s, e) =>
                {
                    index = e.Id; // cache index of found object
                    e.Cancel = true; // abort search
                });

                // if no coincident point was found...
                if (index == -1)
                {
                    index = result.Count; // set id of point
                    tree.Insert(pt, index); // insert point in tree
                    result.Add(pt); // add point to results
                }

                indexMap[i] = index;
            }

            return result;
        }


        #endregion


        #region IEnumerable<Point3d>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point3d Mean(this IEnumerable<Point3d> points)
        {
            var sum = new Point3d();
            int count = 0;

            foreach (Point3d p in points)
            {
                sum += p;
                count++;
            }

            return sum / count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static Vector3d Mean(this IEnumerable<Vector3d> vectors)
        {
            var sum = new Vector3d();
            int count = 0;

            foreach (Vector3d v in vectors)
            {
                sum += v;
                count++;
            }

            return sum / count;
        }


        /// <summary>
        /// Returns the the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static void GetCovarianceMatrix(this IEnumerable<Vector3d> vectors, double[] result)
        {
            GetCovarianceMatrix(vectors, Mean(vectors), result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static void GetCovarianceMatrix(this IEnumerable<Vector3d> vectors, Vector3d mean, double[] result)
        {
            Array.Clear(result, 0, 9);

            // calculate lower triangular covariance matrix
            foreach (Vector3d v in vectors)
            {
                Vector3d d = v - mean;
                result[0] += d.X * d.X;
                result[1] += d.X * d.Y;
                result[2] += d.X * d.Z;
                result[4] += d.Y * d.Y;
                result[5] += d.Y * d.Z;
                result[8] += d.Z * d.Z;
            }

            // set symmetric values
            result[3] = result[1];
            result[6] = result[2];
            result[7] = result[5];
        }


        /// <summary>
        /// Returns the the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static void GetCovarianceMatrix(this IEnumerable<Point3d> points, double[] result)
        {
            GetCovarianceMatrix(points, Mean(points), result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static void GetCovarianceMatrix(this IEnumerable<Point3d> points, Point3d mean, double[] result)
        {
            Array.Clear(result, 0, 9);

            // calculate lower triangular covariance matrix
            foreach (Point3d p in points)
            {
                Vector3d d = p - mean;
                result[0] += d.X * d.X;
                result[1] += d.X * d.Y;
                result[2] += d.X * d.Z;
                result[4] += d.Y * d.Y;
                result[5] += d.Y * d.Z;
                result[8] += d.Z * d.Z;
            }

            // set symmetric values
            result[3] = result[1];
            result[6] = result[2];
            result[7] = result[5];
        }


        #endregion
    }
}
