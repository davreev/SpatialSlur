using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct Domain3d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Domain3d Unit
        {
            get { return new Domain3d(Domain.Unit, Domain.Unit, Domain.Unit); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vec3d Remap(Vec3d point, Domain3d from, Domain3d to)
        {
            point.X = Domain.Remap(point.X, from.X, to.X);
            point.Y = Domain.Remap(point.Y, from.Y, to.Y);
            point.Y = Domain.Remap(point.Z, from.Z, to.Z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Domain3d Intersect(Domain3d d0, Domain3d d1)
        {
            d0.X = Domain.Intersect(d0.X, d1.X);
            d0.Y = Domain.Intersect(d0.Y, d1.Y);
            d0.Z = Domain.Intersect(d0.Z, d1.Z);
            return d0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Domain3d Union(Domain3d a, Domain3d b)
        {
            a.X = Domain.Union(a.X, b.X);
            a.Y = Domain.Union(a.Y, b.Y);
            a.Z = Domain.Union(a.Z, b.Z);
            return a;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator Domain3d(Domain2d d)
        {
            return new Domain3d(d.X, d.Y, new Domain());
        }

        #endregion


        /// <summary></summary>
        public Domain X;
        /// <summary></summary>
        public Domain Y;
        /// <summary></summary>
        public Domain Z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public Domain3d(Vec3d point)
        {
            X = new Domain(point.X);
            Y = new Domain(point.Y);
            Z = new Domain(point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Domain3d(Domain x, Domain y, Domain z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Domain3d(Vec3d from, Vec3d to)
        {
            X = new Domain(from.X, to.X);
            Y = new Domain(from.Y, to.Y);
            Z = new Domain(from.Z, to.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offset"></param>
        public Domain3d(Vec3d center, double offset)
           : this(center, offset, offset, offset)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="offsetZ"></param>
        public Domain3d(Vec3d center, double offsetX, double offsetY, double offsetZ)
        {
            X = new Domain(center.X - offsetX, center.X + offsetX);
            Y = new Domain(center.Y - offsetY, center.Y + offsetY);
            Z = new Domain(center.Z - offsetZ, center.Z + offsetZ);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        /// <param name="z0"></param>
        /// <param name="z1"></param>
        public Domain3d(double x0, double x1, double y0, double y1, double z0, double z1)
        {
            X = new Domain(x0, x1);
            Y = new Domain(y0, y1);
            Z = new Domain(z0, z1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public Domain3d(IEnumerable<Vec3d> points)
            : this()
        {
            var p = points.First();
            X = new Domain(p.X);
            Y = new Domain(p.Y);
            Z = new Domain(p.Z);

            Include(points.Skip(1));
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsIncreasing
        {
            get { return X.IsIncreasing && Y.IsIncreasing && Z.IsIncreasing; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsDecreasing
        {
            get { return X.IsDecreasing && Y.IsDecreasing && Z.IsDecreasing; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsValid
        {
            get { return X.IsValid && Y.IsValid && Z.IsValid; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d From
        {
            get { return new Vec3d(X.T0, Y.T0, Z.T0); }
            set
            {
                X.T0 = value.X;
                Y.T0 = value.Y;
                Z.T0 = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d To
        {
            get { return new Vec3d(X.T1, Y.T1, Z.T1); }
            set
            {
                X.T1 = value.X;
                Y.T1 = value.Y;
                Z.T1 = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Span
        {
            get { return new Vec3d(X.Span, Y.Span, Z.Span); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Mid
        {
            get { return new Vec3d(X.Mid, Y.Mid, Z.Mid); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Min
        {
            get { return new Vec3d(X.Min, Y.Min, Z.Min); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Max
        {
            get { return new Vec3d(X.Max, Y.Max, Z.Max); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0} to {1}, {2} to {3}, {4} to {5})", X.T0, X.T1, Y.T0, Y.T1, Z.T0, Z.T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Domain3d other, double tolerance)
        {
            return X.ApproxEquals(other.X, tolerance) && Y.ApproxEquals(other.Y, tolerance) && Z.ApproxEquals(other.Z, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d Evaluate(Vec3d point)
        {
            point.X = X.Evaluate(point.X);
            point.Y = Y.Evaluate(point.Y);
            point.Z = Z.Evaluate(point.Z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d Normalize(Vec3d point)
        {
            point.X = X.Normalize(point.X);
            point.Y = Y.Normalize(point.Y);
            point.Z = Z.Normalize(point.Z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d Clamp(Vec3d point)
        {
            point.X = X.Clamp(point.X);
            point.Y = Y.Clamp(point.Y);
            point.Z = Z.Clamp(point.Z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d Wrap(Vec3d point)
        {
            point.X = X.Wrap(point.X);
            point.Y = Y.Wrap(point.Y);
            point.Z = Z.Wrap(point.Z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vec3d point)
        {
            return X.Contains(point.X) && Y.Contains(point.Y) && Z.Contains(point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsIncl(Vec3d point)
        {
            return X.ContainsIncl(point.X) && Y.ContainsIncl(point.Y) && Z.ContainsIncl(point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vec3d delta)
        {
            X.Translate(delta.X);
            Y.Translate(delta.Y);
            Z.Translate(delta.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Expand(double delta)
        {
            X.Expand(delta);
            Y.Expand(delta);
            Z.Expand(delta);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Expand(Vec3d delta)
        {
            X.Expand(delta.X);
            Y.Expand(delta.Y);
            Z.Expand(delta.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void Include(Vec3d point)
        {
            X.Include(point.X);
            Y.Include(point.Y);
            Z.Include(point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Include(Domain3d other)
        {
            X.Include(other.X);
            Y.Include(other.Y);
            Z.Include(other.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void Include(IEnumerable<Vec3d> points)
        {
            X.Include(points.Select(p => p.X));
            Y.Include(points.Select(p => p.Y));
            Z.Include(points.Select(p => p.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        public void Reverse()
        {
            X.Reverse();
            Y.Reverse();
            Z.Reverse();
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeIncreasing()
        {
            X.MakeIncreasing();
            Y.MakeIncreasing();
            Z.MakeIncreasing();
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeDecreasing()
        {
            X.MakeDecreasing();
            Y.MakeDecreasing();
            Z.MakeDecreasing();
        }
    }
}
