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
    /// <summary>
    /// Extension methods for classes in the Rhino.Geometry namespace
    /// </summary>
    public static class GeometryExtensions
    {
        #region Interval

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Domain ToDomain(this Interval interval)
        {
            return new Domain(interval.T0, interval.T1);
        }

        #endregion


        #region BoundingBox

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public static Domain2d ToDomain2d(this BoundingBox bbox)
        {
            Vec3d p0 = bbox.Min.ToVec3d();
            Vec3d p1 = bbox.Max.ToVec3d();
            return new Domain2d(p0, p1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public static Domain3d ToDomain3d(this BoundingBox bbox)
        {
            Vec3d p0 = bbox.Min.ToVec3d();
            Vec3d p1 = bbox.Max.ToVec3d();
            return new Domain3d(p0, p1);
        }

        #endregion


        #region Line

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Domain3d ToDomain3d(this Line line)
        {
            return new Domain3d(line.From.ToVec3d(), line.To.ToVec3d());
        }

        #endregion


        #region Plane

        /// <summary>
        /// Returns the transform matrix given by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform ToTransform(this Plane plane)
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
        /// Returns the inverse transformation matrix given by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform ToInverseTransform(this Plane plane)
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


        /// <summary>
        /// Returns the transform matrix given by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        [Obsolete("")]
        public static Transform ToWorld(this Plane plane)
        {
            return ToTransform(plane);
        }


        /// <summary>
        /// Returns the inverse transformation matrix given by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        [Obsolete("")]
        public static Transform ToLocal(this Plane plane)
        {
            return ToInverseTransform(plane);
        }

        #endregion


        #region Point2d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec2d ToVec2d(this Point2d point)
        {
            return new Vec2d(point.X, point.Y);
        }

        #endregion


        #region Point2f

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec2d ToVec2d(this Point2f point)
        {
            return new Vec2d(point.X, point.Y);
        }

        #endregion


        #region Point3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec3d ToVec3d(this Point3d point)
        {
            return new Vec3d(point.X, point.Y, point.Z);
        }


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

        #endregion


        #region Point3f

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec3d ToVec3d(this Point3f point)
        {
            return new Vec3d(point.X, point.Y, point.Z);
        }

        #endregion


        #region Vector2d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec2d ToVec2d(this Vector2d vector)
        {
            return new Vec2d(vector.X, vector.Y);
        }

        #endregion


        #region Vector2f

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec2d ToVec2d(this Vector2f vector)
        {
            return new Vec2d(vector.X, vector.Y);
        }

        #endregion


        #region Vector3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d ToVec3d(this Vector3d vector)
        {
            return new Vec3d(vector.X, vector.Y, vector.Z);
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


        #region Vector3f

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d ToVec3d(this Vector3f vector)
        {
            return new Vec3d(vector.X, vector.Y, vector.Z);
        }

        #endregion

    
        #region Transform

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vec4d Apply(this Transform xform, Vec4d vector)
        {
            return new Vec4d(
             vector.X * xform.M00 + vector.Y * xform.M01 + vector.Z * xform.M02 + vector.W * xform.M03,
             vector.X * xform.M10 + vector.Y * xform.M11 + vector.Z * xform.M12 + vector.W * xform.M13,
             vector.X * xform.M20 + vector.Y * xform.M21 + vector.Z * xform.M22 + vector.W * xform.M23,
             vector.W
             );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xform"></param>
        /// <param name="vector"></param>
        /// <param name="isPosition"></param>
        /// <returns></returns>
        public static Vec3d Apply(this Transform xform, Vec3d vector, bool isPosition = false)
        {
            if (isPosition)
            {
                return new Vec3d(
                    vector.X * xform.M00 + vector.Y * xform.M01 + vector.Z * xform.M02 + xform.M03,
                    vector.X * xform.M10 + vector.Y * xform.M11 + vector.Z * xform.M12 + xform.M13,
                    vector.X * xform.M20 + vector.Y * xform.M21 + vector.Z * xform.M22 + xform.M23
                    );
            }

            return new Vec3d(
               vector.X * xform.M00 + vector.Y * xform.M01 + vector.Z * xform.M02,
               vector.X * xform.M10 + vector.Y * xform.M11 + vector.Z * xform.M12,
               vector.X * xform.M20 + vector.Y * xform.M21 + vector.Z * xform.M22
               );
        }

        #endregion


        #region SpaceMorph

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmorph"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d Apply(this SpaceMorph xmorph, Vec3d vector)
        {
            return xmorph.MorphPoint(vector.ToPoint3d()).ToVec3d();
        }

        #endregion


        #region Mesh

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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="values"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public static void PaintByVertexValue<T>(this Mesh mesh, IReadOnlyList<T> values, Func<T, Color> mapper, bool parallel = false)
        {
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            colors.Count = verts.Count;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    colors[i] = mapper(values[i]);
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public static void PaintByVertexPosition(this Mesh mesh, Func<Point3f, Color> mapper, bool parallel = false)
        {
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            colors.Count = verts.Count;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    colors[i] = mapper(verts[i]);
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public static void PaintByVertexNormal(this Mesh mesh, Func<Vector3f, Color> mapper, bool parallel = false)
        {
            var norms = mesh.Normals;
            var colors = mesh.VertexColors;
            colors.Count = norms.Count;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    colors[i] = mapper(norms[i]);
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, norms.Count), body);
            else
                body(Tuple.Create(0, norms.Count));
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

            Action<Tuple<int, int>> body = range =>
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
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
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
    }
}
