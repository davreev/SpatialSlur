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
    public static class RhinoExtensions
    {
        #region SlurCore Extensions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point2d ToPoint2d(this Vec2d vector)
        {
            return new Point2d(vector.x, vector.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point2f ToPoint2f(this Vec2d vector)
        {
            return new Point2f((float)vector.x, (float)vector.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2d ToVector2d(this Vec2d vector)
        {
            return new Vector2d(vector.x, vector.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec2d ToVec2d(this Point2d point)
        {
            return new Vec2d(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec2d ToVec2d(this Vector2d vector)
        {
            return new Vec2d(vector.X, vector.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec2d ToVec2d(this Point2f point)
        {
            return new Vec2d(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec2d ToVec2d(this Vector2f vector)
        {
            return new Vec2d(vector.X, vector.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="xform"></param>
        /// <returns></returns>
        public static Vec3d TransformDirection(this Vec3d v, Transform xform)
        {
            return new Vec3d(
                v.x * xform.M00 + v.y * xform.M01 + v.z * xform.M02,
                v.x * xform.M10 + v.y * xform.M11 + v.z * xform.M12,
                v.x * xform.M20 + v.y * xform.M21 + v.z * xform.M22
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="xform"></param>
        /// <returns></returns>
        public static Vec3d TransformPosition(this Vec3d p, Transform xform)
        {
            return new Vec3d(
               p.x * xform.M00 + p.y * xform.M01 + p.z * xform.M02 + xform.M03,
               p.x * xform.M10 + p.y * xform.M11 + p.z * xform.M12 + xform.M13,
               p.x * xform.M20 + p.y * xform.M21 + p.z * xform.M22 + xform.M23
               );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="xform"></param>
        /// <returns></returns>
        public static Vec4d Transform(this Vec4d p, Transform xform)
        {
            return new Vec4d(
              p.x * xform.M00 + p.y * xform.M01 + p.z * xform.M02 + p.w * xform.M03,
              p.x * xform.M10 + p.y * xform.M11 + p.z * xform.M12 + p.w * xform.M13,
              p.x * xform.M20 + p.y * xform.M21 + p.z * xform.M22 + p.w * xform.M23,
              p.w
              );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point3d ToPoint3d(this Vec3d vector)
        {
            return new Point3d(vector.x, vector.y, vector.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point3f ToPoint3f(this Vec3d vector)
        {
            return new Point3f((float)vector.x, (float)vector.y, (float)vector.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3d ToVector3d(this Vec3d vector)
        {
            return new Vector3d(vector.x, vector.y, vector.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3f ToVector3f(this Vec3d vector)
        {
            return new Vector3f((float)vector.x, (float)vector.y, (float)vector.z);
        }


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
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d ToVec3d(this Vector3d vector)
        {
            return new Vec3d(vector.X, vector.Y, vector.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec3d ToVec3d(this Point3f point)
        {
            return new Vec3d(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d ToVec3d(this Vector3f vector)
        {
            return new Vec3d(vector.X, vector.Y, vector.Z);
        }

  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Domain ToDomain(this Interval interval)
        {
            return new Domain(interval.T0, interval.T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Interval ToInterval(this Domain domain)
        {
            return new Interval(domain.t0, domain.t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Domain2d domain)
        {
            Domain x = domain.x;
            Domain y = domain.y;
            return new BoundingBox(x.t0, y.t0, 0.0, x.t1, y.t1, 0.0);
        }


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
        /// <param name="domain"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Domain3d domain)
        {
            Domain x = domain.x;
            Domain y = domain.y;
            Domain z = domain.z;
            return new BoundingBox(x.t0, y.t0, z.t0, x.t1, y.t1, z.t1);
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Domain3d ToDomain3d(this Line line)
        {
            return new Domain3d(line.From.ToVec3d(), line.To.ToVec3d());
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


        /// <summary>
        /// Returns the transform matrix for this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform LocalToWorld(this Plane plane)
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
        /// Returns the inverse transform matrix for this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform WorldToLocal(this Plane plane)
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
        /// Assumes the number of vertices in the mesh equals the number of given values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="values"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public static void PaintByVertexValue<T>(this Mesh mesh, IReadOnlyList<T> values, Func<T, Color> mapper, bool parallel = false)
        {
            var vc = new Color[values.Count];

            Action<Tuple<int, int>> func = range =>
             {
                 for (int i = range.Item1; i < range.Item2; i++)
                     vc[i] = mapper(values[i]);
             };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, values.Count), func);
            else
                func(Tuple.Create(0, values.Count));

            mesh.VertexColors.SetColors(vc);
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
            var v = mesh.Vertices;
            var vc = new Color[v.Count];

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    vc[i] = mapper(v[i]);
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, v.Count), func);
            else
                func(Tuple.Create(0, v.Count));

            mesh.VertexColors.SetColors(vc);
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
            var vn = mesh.Normals;
            var vc = new Color[vn.Count];

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    vc[i] = mapper(vn[i]);
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vn.Count), func);
            else
                func(Tuple.Create(0, vn.Count));

            mesh.VertexColors.SetColors(vc);
        }


        /// <summary>
        /// Assumes the mesh is polygon soup (i.e. vertices aren't shared between faces).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="values"></param>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public static void PaintByFaceValue<T>(this Mesh mesh, IReadOnlyList<T> values, Func<T, Color> mapper)
        {
            Color[] vc = new Color[mesh.Vertices.Count];
            var faces = mesh.Faces;
 
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (!f.IsValid()) continue; // skip invalid faces

                Color c = mapper(values[i]);
                int n = (f.IsQuad) ? 4 : 3;
                for (int j = 0; j < n; j++) vc[f[j]] = c;
            }

            mesh.VertexColors.SetColors(vc);
        }

        #endregion

        #region SlurField Extensions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static BoundingBox GetBoundingBox(this Field2d field)
        {
            return field.Domain.ToBoundingBox();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static BoundingBox GetBoundingBox(this Field3d field)
        {
            return field.Domain.ToBoundingBox();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static PointCloud ToPointCloud<T>(this Field3d<T> field, Func<T, Color> mapper)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="result"></param>
        public static void ToPointCloud<T>(this Field3d<T> field, Func<T, Color> mapper, PointCloud result)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static Mesh ToMesh<T>(this Field2d<T> field, Func<T, Color> mapper)
        {
            Mesh mesh = new Mesh();
            var verts = mesh.Vertices;
            var colors = mesh.VertexColors;
            var faces = mesh.Faces;

            var values = field.Values;
            int index = 0;

            for(int i = 0; i < field.CountY; i++)
            {
                for(int j = 0; j < field.CountX; j++)
                {
                    Vec2d p = field.CoordinateAt(j, i);
                    verts.Add(p.x, p.y, 0.0);
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
        /// Returns the interpolated value at a given point in the field.
        /// Assumes triangular faces in the queried mesh.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static T ValueAt<T>(this MeshField<T> field, MeshPoint point)
        {
            MeshFace f = point.Mesh.Faces[point.FaceIndex];
            double[] t = point.T;
            return field.ValueAt(f[0], f[1], f[2], t[0], t[1], t[2]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this MeshScalarField field, MeshPoint point, double amount)
        {
            MeshFace f = point.Mesh.Faces[point.FaceIndex];
            double[] t = point.T;
            field.IncrementAt(f[0], f[1], f[2], t[0], t[1], t[2], amount);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void IncrementAt(this MeshVectorField field, MeshPoint point, Vec3d amount)
        {
            MeshFace f = point.Mesh.Faces[point.FaceIndex];
            double[] t = point.T;
            field.IncrementAt(f[0], f[1], f[2], t[0], t[1], t[2], amount);
        }

        #endregion

        #region SlurMesh Extensions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh ToHeMesh(this Mesh mesh)
        {
            return RhinoFactory.CreateHeMesh(mesh);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh ToHeMesh(this Mesh mesh, out Vec3d[] vertexPositions)
        {
            return RhinoFactory.CreateHeMesh(mesh, out vertexPositions);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh ToHeMesh(this Mesh mesh, out Vec3d[] vertexPositions, out Vec3d[] vertexNormals)
        {
            return RhinoFactory.CreateHeMesh(mesh, out vertexPositions, out vertexNormals);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh ToHeMesh(this Mesh mesh, out Vec3d[] vertexPositions, out Vec3d[] vertexNormals, out Vec2d[] textureCoords)
        {
            return RhinoFactory.CreateHeMesh(mesh, out vertexPositions, out vertexNormals, out textureCoords);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public static Line ToLine<E, V>(this Halfedge<E, V> halfedge, IReadOnlyList<Vec3d> vertexPositions)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Vec3d p0 = vertexPositions[halfedge.Start.Index];
            Vec3d p1 = vertexPositions[halfedge.End.Index];
            return new Line(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Polyline ToPolyline<E, V, F>(this HeFace<E, V, F> face, IReadOnlyList<Vec3d> vertexPositions)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Polyline result = new Polyline();

            foreach (var v in face.Vertices)
            {
                var p = vertexPositions[v.Index];
                result.Add(p.x, p.y, p.z);
            }

            result.Add(result.First);
            return result;
        }


        /// <summary>
        /// Returns the circumcircle of a triangular face.
        /// Assumes the face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Circle GetCircumcircle<E,V,F>(this HeFace<E, V, F> face, IReadOnlyList<Vec3d> vertexPositions)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            var he = face.First;
            var p0 = vertexPositions[he.Previous.Start.Index];
            var p1 = vertexPositions[he.Previous.Start.Index];
            var p2 = vertexPositions[he.Next.Start.Index];

            return new Circle(p0.ToPoint3d(), p0.ToPoint3d(), p0.ToPoint3d());
        }


        /*
        /// <summary>
        /// Returns the circumcircle of a triangular face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Circle GetIncircle(this HeFace face)
        {
            // TODO
            Vec3d p0 = _first.Previous.Start.Position;
            Vec3d p1 = _first.Start.Position;
            Vec3d p2 = _first.Next.Start.Position;

            double d01 = p0.DistanceTo(p1);
            double d12 = p1.DistanceTo(p2);
            double d20 = p2.DistanceTo(p0);

            double p = (d01 + d12 + d20) * 0.5; // semiperimeter
            double pInv = 1.0 / p; // inverse semiperimeter
            radius = Math.Sqrt(p * (p - d01) * (p - d12) * (p - d20)) * pInv; // triangle area (Heron's formula) / semiperimeter

            pInv *= 0.5; // inverse perimeter
            return p0 * (d12 * pInv) + p1 * (d20 * pInv) + p2 * (d01 * pInv);
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetEdgeLines<S, EE, VV, E, V>(this HalfedgeList<S, EE, VV, E, V> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Line> result, bool parallel = false)
            where S : HeStructure<S, EE, VV, E, V>
            where EE : HalfedgeList<S, EE, VV, E, V>
            where VV : HeVertexList<S, EE, VV, E, V>
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i << 1];
                    if (he.IsUnused) continue;
                    result[i] = he.ToLine(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count >> 1), func);
            else
                func(Tuple.Create(0, halfedges.Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeLines<S, EE, VV, E, V>(this HalfedgeList<S, EE, VV, E, V> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Line> result, bool parallel = false)
            where S : HeStructure<S, EE, VV, E, V>
            where EE : HalfedgeList<S, EE, VV, E, V>
            where VV : HeVertexList<S, EE, VV, E, V>
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he = halfedges[j];
                    if (he.IsUnused) continue;

                    var ln = he.ToLine(vertexPositions);
                    result[j] = ln;

                    ln.Flip();
                    result[j + 1] = ln;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count >> 1), func);
            else
                func(Tuple.Create(0, halfedges.Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeLines<E, V>(this IReadOnlyList<Halfedge<E, V>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Line> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused) continue;
                    result[i] = he.ToLine(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFacePolylines<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Polyline> result, bool parallel = false)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.ToPolyline(vertexPositions);
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
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceCircumcircles<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Circle> result, bool parallel = false)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.GetCircumcircle(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Note that unused and n-gon faces are skipped.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoMesh(this HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions)
        {
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            Mesh result = new Mesh();
            var newVerts = result.Vertices;
            var newFaces = result.Faces;

            // add vertices
            for (int i = 0; i < verts.Count; i++)
            {
                var p = vertexPositions[i];
                newVerts.Add(p.x, p.y, p.z);
            }

            // add faces
            foreach (var f in mesh.Faces)
            {
                if (f.IsUnused) continue;

                var he = f.First;
                int ne = f.Degree;

                if (ne == 3)
                    newFaces.AddFace(he.Start.Index, he.Next.Start.Index, he.Previous.Start.Index);
                else if (ne == 4)
                    newFaces.AddFace(he.Start.Index, he.Next.Start.Index, he.Next.Next.Start.Index, he.Previous.Start.Index);
            }

            return result;
        }


        /// <summary>
        /// Note that unused faces are skipped.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoMesh(this HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, QuadrangulateMode mode = QuadrangulateMode.Strip)
        {
            Mesh result = new Mesh();
            var newVerts = result.Vertices;
            var newFaces = result.Faces;

            // add vertices
            for (int i = 0; i < newVerts.Count; i++)
            {
                var p = vertexPositions[i];
                newVerts.Add(p.x, p.y, p.z);
            }

            // add faces
            foreach (var f in mesh.Faces)
            {
                if (f.IsUnused) continue;

                var he = f.First;
                int ne = f.Degree;

                if (ne == 3)
                    newFaces.AddFace(he.Start.Index, he.Next.Start.Index, he.Previous.Start.Index);
                else if (ne == 4)
                    newFaces.AddFace(he.Start.Index, he.Next.Start.Index, he.Next.Next.Start.Index, he.Previous.Start.Index);
                else
                {
                    // TODO support different quadrangulation schemes for n-gons
                    throw new NotImplementedException();

                    /*
                    int last = verts.Count;
                    verts.Add(f.GetBarycenter().ToPoint3d());

                    foreach (var he in f.Halfedges)
                        faces.AddFace(he.Start.Index, he.End.Index, last);
                    */
                }
            }

            return result;
        }


        /// <summary>
        /// Vertices of the resulting mesh are not shared between faces.
        /// Note that unused and n-gon faces are skipped.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoPolySoup(this HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions)
        {
            Mesh result = new Mesh();
            var newVerts = result.Vertices;
            var newFaces = result.Faces;

            // add vertices per face
            foreach (var f in mesh.Faces)
            {
                if (f.IsUnused) continue;

                int v0 = newVerts.Count;
                int deg = 0;

                // add face vertices
                foreach (var v in f.Vertices)
                {
                    var p = vertexPositions[v.Index];
                    newVerts.Add(p.x,p.y,p.z);
                    deg++;
                }

                // add face(s)
                if (deg == 3)
                    newFaces.AddFace(v0, v0 + 1, v0 + 2);
                else if (deg == 4)
                    newFaces.AddFace(v0, v0 + 1, v0 + 2, v0 + 3);
            }

            return result;
        }


        /// <summary>
        /// Vertices of the resulting mesh are not shared between faces.
        /// Note that unused faces are skipped.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoPolySoup(this HeMesh mesh, IReadOnlyList<Vec3d> vertexPositions, QuadrangulateMode mode)
        {
            Mesh result = new Mesh();
            var newVerts = result.Vertices;
            var newFaces = result.Faces;

            // add vertices per face
            foreach (var f in mesh.Faces)
            {
                if (f.IsUnused) continue;

                int v0 = newVerts.Count;
                int deg = 0;

                // add face vertices
                foreach (var v in f.Vertices)
                {
                    var p = vertexPositions[v.Index];
                    newVerts.Add(p.x, p.y, p.z);
                    deg++;
                }

                // add face(s)
                if (deg == 3)
                    newFaces.AddFace(v0, v0 + 1, v0 + 2);
                else if (deg == 4)
                    newFaces.AddFace(v0, v0 + 1, v0 + 2, v0 + 3);
                else
                {
                    // TODO support different triangulation schemes for n-gons
                    throw new NotImplementedException();

                    /*
                    int last = verts.Count;
                    verts.Add(f.GetBarycenter().ToPoint3d());

                    int offset = last - ne;
                    for (int i = 0; i < ne; i++)
                    {
                        int j = (i + 1) % ne;
                        faces.AddFace(offset + i, offset + j, last);
                    }
                    */
                }
            }

            return result;
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

            // add verts
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                int v0 = newVerts.Count;

                if (f.IsTriangle)
                {
                    for (int j = 0; j < 3; j++)
                        newVerts.Add(verts[f[j]]);

                    newFaces.AddFace(v0, v0 + 1, v0 + 2);
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                        newVerts.Add(verts[f[j]]);

                    newFaces.AddFace(v0, v0 + 1, v0 + 2, v0 + 3);
                }
            }

            return newMesh;
        }


        /// <summary>
        /// Reverses face windings and flips normals.
        /// </summary>
        /// <param name="mesh"></param>
        public static void Reverse(this Mesh mesh)
        {
            // reverse face windings
            var faces = mesh.Faces;
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];

                if (f.IsQuad)
                    faces.SetFace(i, f.D, f.C, f.B, f.A);
                else
                    faces.SetFace(i, f.C, f.B, f.A);
            }

            // reverse normals
            var normals = mesh.Normals;
            for (int i = 0; i < normals.Count; i++)
                normals[i] *= -1.0f;
        }

        #endregion

        #region RhinoCommon Extensions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, double tolerance)
        {
            int[] indexMap;
            RTree tree;
            return RemoveDuplicatePoints(points, tolerance, out indexMap, out tree);
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
            RTree tree;
            return RemoveDuplicatePoints(points, tolerance, out indexMap, out tree);
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
            List<Point3d> result = new List<Point3d>();
            indexMap = new int[points.Count];
            tree = new RTree();

            SearchHelper helper = new SearchHelper();
            Vector3d span = new Vector3d(tolerance, tolerance, tolerance);

            // for each point, search for coincident points in the tree
            for (int i = 0; i < points.Count; i++)
            {
                Point3d pt = points[i];
                helper.Reset();
                tree.Search(new BoundingBox(pt - span, pt + span), helper.Callback);
                int index = helper.Id;

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


        /// <summary>
        /// Simple helper class for searching an RTree for duplicate points.
        /// </summary>
        private class SearchHelper
        {
            public int _id = -1;

            /// <summary>
            /// 
            /// </summary>
            public int Id
            {
                get { return _id; }
            }


            /// <summary>
            /// 
            /// </summary>
            public void Reset()
            {
                _id = -1;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void Callback(Object sender, RTreeEventArgs e)
            {
                _id = e.Id; // cache index of found object
                e.Cancel = true; // abort search
            }
        }


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
        /// Also rerturns barycentric vertex areas calculated in the process.
        /// Assumes triangle mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="entriesOut"></param>
        /// <param name="vertexAreasOut"></param>
        public static void GetLaplacianMatrix(this Mesh mesh, double[] entriesOut, double[] vertexAreasOut)
        {
            var verts = mesh.Vertices;
            int n = verts.Count;
            double t = 1.0 / 6.0;

            Array.Clear(entriesOut, 0, n * n);
            Array.Clear(vertexAreasOut, 0, n);

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
                    vertexAreasOut[i0] += a * t;

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
                    w /= Math.Sqrt(vertexAreasOut[i] * vertexAreasOut[j]);
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
