using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Represents a double precision numerical domain in 2 dimensions.
    /// </summary>
    [Obsolete("Renamed to Interval2d")]
    [Serializable]
    public struct Domain2d
    {
        #region Static

        /// <summary></summary>
        public static readonly Domain2d Zero = new Domain2d();
        /// <summary></summary>
        public static readonly Domain2d Unit = new Domain2d(0.0, 1.0, 0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        public static implicit operator Interval2d(Domain2d domain)
        {
            return new Interval2d(domain.X, domain.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Domain2d operator +(Domain2d d, Vec2d v)
        {
            d.X.Translate(v.X);
            d.Y.Translate(v.Y);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Domain2d operator -(Domain2d d, Vec2d v)
        {
            d.X.Translate(-v.X);
            d.Y.Translate(-v.Y);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Domain2d operator *(Domain2d d, double t)
        {
            d.X.Scale(t);
            d.Y.Scale(t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Domain2d operator *(double t, Domain2d d)
        {
            d.X.Scale(t);
            d.Y.Scale(t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Domain2d operator /(Domain2d d, double t)
        {
            t = 1.0 / t;
            d.X.Scale(t);
            d.Y.Scale(t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vec2d Remap(Vec2d point, Domain2d from, Domain2d to)
        {
            point.X = Domain1d.Remap(point.X, from.X, to.X);
            point.Y = Domain1d.Remap(point.Y, from.Y, to.Y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Domain2d Intersect(Domain2d d0, Domain2d d1)
        {
            d0.X = Domain1d.Intersect(d0.X, d1.X);
            d0.Y = Domain1d.Intersect(d0.Y, d1.Y);
            return d0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Domain2d Union(Domain2d a, Domain2d b)
        {
            a.X = Domain1d.Union(a.X, b.X);
            a.Y = Domain1d.Union(a.Y, b.Y);
            return a;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator Domain2d(Domain3d d)
        {
            return new Domain2d(d.X, d.Y);
        }

        #endregion


        /// <summary></summary>
        public Domain1d X;
        /// <summary></summary>
        public Domain1d Y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public Domain2d(Vec2d point)
        {
            X = new Domain1d(point.X);
            Y = new Domain1d(point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Domain2d(Domain1d x, Domain1d y)
        {
            this.X = x;
            this.Y = y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Domain2d(Vec2d from, Vec2d to)
        {
            X = new Domain1d(from.X, to.X);
            Y = new Domain1d(from.Y, to.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offset"></param>
        public Domain2d(Vec2d center, double offset)
           : this(center, offset, offset)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public Domain2d(Vec2d center, double offsetX, double offsetY)
        {
            X = new Domain1d(center.X - offsetX, center.X + offsetX);
            Y = new Domain1d(center.Y - offsetY, center.Y + offsetY);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        public Domain2d(double x0, double x1, double y0, double y1)
        {
            X = new Domain1d(x0, x1);
            Y = new Domain1d(y0, y1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public Domain2d(IEnumerable<Vec2d> points)
            : this()
        {
            var p = points.First();
            X = new Domain1d(p.X);
            Y = new Domain1d(p.Y);

            Include(points.Skip(1));
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsIncreasing
        {
            get { return X.IsIncreasing && Y.IsIncreasing; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsDecreasing
        {
            get { return X.IsDecreasing && Y.IsDecreasing; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsValid
        {
            get { return X.IsValid && Y.IsValid; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d From
        {
            get { return new Vec2d(X.T0, Y.T0); }
            set
            {
                X.T0 = value.X;
                Y.T0 = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d To
        {
            get { return new Vec2d(X.T1, Y.T1); }
            set
            {
                X.T1 = value.X;
                Y.T1 = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Span
        {
            get { return new Vec2d(X.Span, Y.Span); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Mid
        {
            get { return new Vec2d(X.Mid, Y.Mid); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Min
        {
            get { return new Vec2d(X.Min, Y.Min); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Max
        {
            get { return new Vec2d(X.Max, Y.Max); }
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("({0} to {1}, {2} to {3})", X.T0, X.T1, Y.T0, Y.T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Domain2d other, double tolerance = SlurMath.ZeroTolerance)
        {
            return X.ApproxEquals(other.X, tolerance) && Y.ApproxEquals(other.Y, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d Evaluate(Vec2d point)
        {
            point.X = X.Evaluate(point.X);
            point.Y = Y.Evaluate(point.Y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d Normalize(Vec2d point)
        {
            point.X = X.Normalize(point.X);
            point.Y = Y.Normalize(point.Y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d Clamp(Vec2d point)
        {
            point.X = X.Clamp(point.X);
            point.Y = Y.Clamp(point.Y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d Wrap(Vec2d point)
        {
            point.X = X.Wrap(point.X);
            point.Y = Y.Wrap(point.Y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vec2d point)
        {
            return X.Contains(point.X) && Y.Contains(point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsIncl(Vec2d point)
        {
            return X.ContainsIncl(point.X) && Y.ContainsIncl(point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vec2d delta)
        {
            X.Translate(delta.X);
            Y.Translate(delta.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Expand(double delta)
        {
            X.Expand(delta);
            Y.Expand(delta);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Expand(Vec2d delta)
        {
            X.Expand(delta.X);
            Y.Expand(delta.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void Include(Vec2d point)
        {
            X.Include(point.X);
            Y.Include(point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Include(Domain2d other)
        {
            X.Include(other.X);
            Y.Include(other.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void Include(IEnumerable<Vec2d> points)
        {
            X.Include(points.Select(p => p.X));
            Y.Include(points.Select(p => p.Y));
        }
  

        /// <summary>
        /// 
        /// </summary>
        public void Reverse()
        {
            X.Reverse();
            Y.Reverse();
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeIncreasing()
        {
            X.MakeIncreasing();
            Y.MakeIncreasing();
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeDecreasing()
        {
            X.MakeDecreasing();
            Y.MakeDecreasing();
        }
    }
}
