using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurGraph;
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
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vecd ToVecd(this Vector2d vector)
        {
            Vecd result = new Vecd(2);
            result[0] = vector.X;
            result[1] = vector.Y;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vecd ToVecd(this Vector3d vector)
        {
            Vecd result = new Vecd(3);
            result[0] = vector.X;
            result[1] = vector.Y;
            result[2] = vector.Z;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vecd ToVecd(this Point2d point)
        {
            Vecd result = new Vecd(2);
            result[0] = point.X;
            result[1] = point.Y;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vecd ToVecd(this Point3d point)
        {
            Vecd result = new Vecd(3);
            result[0] = point.X;
            result[1] = point.Y;
            result[2] = point.Z;
            return result;
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
        public static void PaintByValue<T>(this Mesh mesh, IList<T> values, Func<T, Color> mapper, bool parallel = false)
        {
            var vc = new Color[values.Count];

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, values.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        vc[i] = mapper(values[i]);
                });
            }
            else
            {
                for (int i = 0; i < values.Count; i++)
                    vc[i] = mapper(values[i]);
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
        public static void UpdatePointCloud<T>(this Field3d<T> field, Func<T, Color> mapper, PointCloud result)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the interpolated value at a given point in the field.
        /// Assumes triangular faces in the queried mesh.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static T Evaluate<T>(this MeshField<T> field, MeshPoint point)
        {
            MeshFace f = point.Mesh.Faces[point.FaceIndex];
            double[] t = point.T;
            return field.Evaluate(f[0], f[1], f[2], t[0], t[1], t[2]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public static void DepositAt(this DynamicMeshScalarField field, MeshPoint point, double amount)
        {
            MeshFace f = point.Mesh.Faces[point.FaceIndex];
            double[] t = point.T;
            field.DepositAt(f[0], f[1], f[2], t[0], t[1], t[2], amount);
        }

        #endregion

        #region SlurGraph Extensions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Line[] GetEdgeLines(this EdgeList edges, IList<Vec3d> nodePositions, bool parallel = false)
        {
            Line[] result = new Line[edges.Count >> 1];
            edges.UpdateEdgeLines(nodePositions, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateEdgeLines(this EdgeList edges, IList<Vec3d> nodePositions, IList<Line> result, bool parallel = false)
        {
            edges.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count >> 1), range =>
                    edges.UpdateEdgeLines(nodePositions, result, range.Item1, range.Item2));
            else
                edges.UpdateEdgeLines(nodePositions, result, 0, edges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateEdgeLines(this EdgeList edges, IList<Vec3d> nodePositions, IList<Line> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var e = edges[i << 1];
                if (e.IsUnused) continue;

                Vec3d p0 = nodePositions[e.Start.Index];
                Vec3d p1 = nodePositions[e.End.Index];
                result[i] = new Line(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Line[] GetEdgeLines(this DiEdgeList edges, IList<Vec3d> nodePositions, bool parallel = false)
        {
            Line[] result = new Line[edges.Count >> 1];
            edges.UpdateEdgeLines(nodePositions, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateEdgeLines(this DiEdgeList edges, IList<Vec3d> nodePositions, IList<Line> result, bool parallel = false)
        {
            edges.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count >> 1), range =>
                    edges.UpdateEdgeLines(nodePositions, result, range.Item1, range.Item2));
            else
                edges.UpdateEdgeLines(nodePositions, result, 0, edges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateEdgeLines(this DiEdgeList edges, IList<Vec3d> nodePositions, IList<Line> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var e = edges[i << 1];
                if (e.IsUnused) continue;

                Vec3d p0 = nodePositions[e.Start.Index];
                Vec3d p1 = nodePositions[e.End.Index];
                result[i] = new Line(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z);
            }
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
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public static Line ToLine(this Halfedge halfedge)
        {
            Vec3d p0 = halfedge.Start.Position;
            Vec3d p1 = halfedge.End.Position;
            return new Line(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Polyline ToPolyline(this HeFace face)
        {
            Polyline result = new Polyline();

            foreach (HeVertex v in face.Vertices)
                result.Add(v.Position.ToPoint3d());

            result.Add(result.First);
            return result;
        }


        /*
       /// <summary>
       /// Returns the circumcircle of a triangular face.
       /// Assumes face is triangular.
       /// http://mathworld.wolfram.com/Incenter.html
       /// </summary>
       /// <returns></returns>
       public static Circle GetCircumcircle(this HeFace face)
       {
           // TODO
       }
       */


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
        /// <param name="hedges"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Line[] GetEdgeLines(this HalfedgeList hedges, bool parallel = false)
        {
            Line[] result = new Line[hedges.Count >> 1];
            hedges.UpdateEdgeLines(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateEdgeLines(this HalfedgeList hedges, IList<Line> result, bool parallel = false)
        {
            hedges.HalfSizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.UpdateEdgeLines(result, range.Item1, range.Item2));
            else
                hedges.UpdateEdgeLines(result, 0, hedges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateEdgeLines(this HalfedgeList hedges, IList<Line> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i << 1];
                if (he.IsUnused) continue;
                result[i] = he.ToLine();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Polyline[] GetFacePolylines(this HeFaceList faces, bool parallel = false)
        {
            Polyline[] result = new Polyline[faces.Count];
            faces.UpdateFacePolylines(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFacePolylines(this HeFaceList faces, IList<Polyline> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFacePolylines(result, range.Item1, range.Item2));
            else
                faces.UpdateFacePolylines(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFacePolylines(this HeFaceList faces, IList<Polyline> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;
                result[i] = f.ToPolyline();
            }
        }


        /// <summary>
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Circle[] GetFaceCircumcircles(this HeFaceList faces, bool parallel = false)
        {
            Circle[] result = new Circle[faces.Count];
            faces.UpdateFaceCircumcircles(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceCircumcircles(this HeFaceList faces, IList<Circle> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFaceCircumcircles(result, range.Item1, range.Item2));
            else
                faces.UpdateFaceCircumcircles(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceCircumcircles(this HeFaceList faces, IList<Circle> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                Halfedge he = f.First;
                HeVertex v0 = he.Start;
                he = he.Next;
                HeVertex v1 = he.Start;
                he = he.Next;
                HeVertex v2 = he.Start;

                result[i] = new Circle(v0.Position.ToPoint3d(), v1.Position.ToPoint3d(), v2.Position.ToPoint3d());
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Circle[] GetFaceIncircles(this HeFaceList list, IList<double> edgeLengths)
        {
            if (edgeLengths.Count != list.Mesh.Edges.Count)
                throw new ArgumentException("the number of edge lengths provided must match the number of edges in the associated mesh.");

            int nf = list.Count;
            Circle[] result = new Circle[nf];

            for (int i = 0; i < nf; i++)
            {
                HeFace f = list[i];
                if (f.IsUnused) continue;

                HeEdge e0 = f.First;
                HeEdge e1 = e0.Next;
                HeEdge e2 = e1.Next;

                ///result[i] = Circle.TryFitCircleTTT();
            }

            throw new System.NotImplementedException();    
            return result;
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static BoundingBox GetBoundingBox(this HeMesh mesh)
        {
            List<Point3d> points = new List<Point3d>();

            foreach (HeVertex v in mesh.Vertices)
            {
                if (v.IsUnused) continue;
                points.Add(v.Position.ToPoint3d());
            }

            return new BoundingBox(points);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="xform"></param>
        /// <param name="parallel"></param>
        public static void Transform(this HeMesh mesh, Transform xform, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, mesh.Vertices.Count), range =>
                   mesh.Transform(xform, range.Item1, range.Item2));
            else
                mesh.Transform(xform, 0, mesh.Vertices.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void Transform(this HeMesh mesh, Transform xform, int i0, int i1)
        {
            HeVertexList verts = mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                Point3d p = xform * v.Position.ToPoint3d();
                v.Position = p.ToVec3d();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoMesh(this HeMesh mesh)
        {
            Mesh result = new Mesh();
            var verts = result.Vertices;
            var faces = result.Faces;

            // add vertices
            foreach (HeVertex v in mesh.Vertices)
                verts.Add(v.Position.ToPoint3d());

            // add faces
            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused)
                {
                    faces.AddFace(new MeshFace()); // add placeholder for unused faces
                    continue;
                }

                int ne = f.EdgeCount;

                if (ne == 3)
                {
                    Halfedge he = f.First;
                    faces.AddFace(he.Start.Index, he.Next.Start.Index, he.Previous.Start.Index);
                }
                else if (ne == 4)
                {
                    Halfedge he = f.First;
                    faces.AddFace(he.Start.Index, he.Next.Start.Index, he.Next.Next.Start.Index, he.Previous.Start.Index);
                }
                else
                {
                    // TODO support different triangulation schemes for n-gons
                    int last = verts.Count;
                    verts.Add(f.GetBarycenter().ToPoint3d());

                    foreach (Halfedge he in f.Halfedges)
                        faces.AddFace(he.Start.Index, he.End.Index, last);
                }
            }

            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoMeshUnwelded(this HeMesh mesh)
        {
            Mesh result = new Mesh();
            var verts = result.Vertices;
            var faces = result.Faces;

            // add vertices per face
            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused)
                {
                    faces.AddFace(new MeshFace()); // add placeholder for unused faces
                    continue;
                }

                int ne = 0;

                // add face vertices
                foreach (HeVertex v in f.Vertices)
                {
                    verts.Add(v.Position.ToPoint3d());
                    ne++;
                }

                // add face(s)
                if (ne == 3)
                {
                    int n = result.Vertices.Count;
                    faces.AddFace(n - 3, n - 2, n - 1);
                }
                else if (ne == 4)
                {
                    int n = result.Vertices.Count;
                    faces.AddFace(n - 4, n - 3, n - 2, n - 1);
                }
                else
                {
                    // TODO support different triangulation schemes for n-gons.
                    int last = verts.Count;
                    verts.Add(f.GetBarycenter().ToPoint3d());

                    int offset = last - ne;
                    for (int i = 0; i < ne; i++)
                    {
                        int j = (i + 1) % ne;
                        faces.AddFace(offset + i, offset + j, last);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
