using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Represents a double precision interval in 2 dimensions.
    /// </summary>
    [Serializable]
    public struct Interval2d
    {
        #region Static

        /// <summary></summary>
        public static readonly Interval2d Zero = new Interval2d();
        /// <summary></summary>
        public static readonly Interval2d Unit = new Interval2d(0.0, 1.0, 0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Interval2d operator +(Interval2d d, Vec2d v)
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
        public static Interval2d operator -(Interval2d d, Vec2d v)
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
        public static Interval2d operator *(Interval2d d, double t)
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
        public static Interval2d operator *(double t, Interval2d d)
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
        public static Interval2d operator /(Interval2d d, double t)
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
        public static Vec2d Remap(Vec2d point, Interval2d from, Interval2d to)
        {
            point.X = Interval1d.Remap(point.X, from.X, to.X);
            point.Y = Interval1d.Remap(point.Y, from.Y, to.Y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Interval2d Intersect(Interval2d d0, Interval2d d1)
        {
            d0.X = Interval1d.Intersect(d0.X, d1.X);
            d0.Y = Interval1d.Intersect(d0.Y, d1.Y);
            return d0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Interval2d Union(Interval2d a, Interval2d b)
        {
            a.X = Interval1d.Union(a.X, b.X);
            a.Y = Interval1d.Union(a.Y, b.Y);
            return a;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator Interval2d(Interval3d d)
        {
            return new Interval2d(d.X, d.Y);
        }

        #endregion


        /// <summary></summary>
        public Interval1d X;
        /// <summary></summary>
        public Interval1d Y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Interval2d(Interval1d x, Interval1d y)
        {
            this.X = x;
            this.Y = y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ab"></param>
        public Interval2d(Vec2d ab)
        {
            X = new Interval1d(ab.X);
            Y = new Interval1d(ab.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Interval2d(Vec2d a, Vec2d b)
        {
            X = new Interval1d(a.X, b.X);
            Y = new Interval1d(a.Y, b.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offset"></param>
        public Interval2d(Vec2d center, double offset)
           : this(center, offset, offset)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public Interval2d(Vec2d center, double offsetX, double offsetY)
        {
            X = new Interval1d(center.X - offsetX, center.X + offsetX);
            Y = new Interval1d(center.Y - offsetY, center.Y + offsetY);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        public Interval2d(double x0, double x1, double y0, double y1)
        {
            X = new Interval1d(x0, x1);
            Y = new Interval1d(y0, y1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public Interval2d(IEnumerable<Vec2d> points)
            : this()
        {
            var p = points.First();
            X = new Interval1d(p.X);
            Y = new Interval1d(p.Y);

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
        /// <returns></returns>
        public Vec2i Orientation
        {
            get { return new Vec2i(X.Orientation, Y.Orientation); }
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
        public Vec2d A
        {
            get { return new Vec2d(X.A, Y.A); }
            set
            {
                X.A = value.X;
                Y.A = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d B
        {
            get { return new Vec2d(X.B, Y.B); }
            set
            {
                X.B = value.X;
                Y.B = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public (Interval1d, Interval1d) Components
        {
            get { return (X, Y); }
        }


        /// <summary>
        /// Defined as B - A
        /// </summary>
        public Vec2d Length
        {
            get { return new Vec2d(X.Length, Y.Length); }
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0} to {1}, {2} to {3})", X.A, X.B, Y.A, Y.B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Interval2d other, double tolerance = SlurMath.ZeroTolerance)
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
        public void Include(Interval2d other)
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
