
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Represents a double precision interval in 2 dimensions.
    /// </summary>
    [Serializable]
    public struct Interval2d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Interval2d Zero = new Interval2d();
        /// <summary></summary>
        public static readonly Interval2d Unit = new Interval2d(0.0, 1.0, 0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public static implicit operator string(Interval2d interval)
        {
            return $"({interval.X}, {interval.Y})";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Interval2d operator +(Interval2d d, Vector2d v)
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
        public static Interval2d operator -(Interval2d d, Vector2d v)
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
        public static Vector2d Remap(Vector2d point, Interval2d from, Interval2d to)
        {
            point.X = Intervald.Remap(point.X, from.X, to.X);
            point.Y = Intervald.Remap(point.Y, from.Y, to.Y);
            return point;
        }

        #endregion


        /// <summary></summary>
        public Intervald X;
        /// <summary></summary>
        public Intervald Y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Interval2d(Intervald x, Intervald y)
        {
            X = x;
            Y = y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ab"></param>
        public Interval2d(Vector2d ab)
        {
            X = new Intervald(ab.X);
            Y = new Intervald(ab.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Interval2d(Vector2d a, Vector2d b)
        {
            X = new Intervald(a.X, b.X);
            Y = new Intervald(a.Y, b.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offset"></param>
        public Interval2d(Vector2d center, double offset)
           : this(center, offset, offset)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public Interval2d(Vector2d center, double offsetX, double offsetY)
        {
            X = new Intervald(center.X - offsetX, center.X + offsetX);
            Y = new Intervald(center.Y - offsetY, center.Y + offsetY);
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
            X = new Intervald(x0, x1);
            Y = new Intervald(y0, y1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public Interval2d(IEnumerable<Vector2d> points)
            : this()
        {
            (var x, var y) = points.First();
            X = new Intervald(x);
            Y = new Intervald(y);

            foreach(var p in points.Skip(1))
            {
                X.IncludePos(p.X);
                Y.IncludePos(p.Y);
            }
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
        public Vector2d A
        {
            get { return new Vector2d(X.A, Y.A); }
            set
            {
                X.A = value.X;
                Y.A = value.Y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d B
        {
            get { return new Vector2d(X.B, Y.B); }
            set
            {
                X.B = value.X;
                Y.B = value.Y;
            }
        }


        /// <summary>
        /// B - A
        /// </summary>
        public Vector2d Delta
        {
            get { return new Vector2d(X.Delta, Y.Delta); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2i Sign
        {
            get { return new Vector2i(X.Sign, Y.Sign); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d Mid
        {
            get { return new Vector2d(X.Mid, Y.Mid); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d Min
        {
            get { return new Vector2d(X.Min, Y.Min); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d Max
        {
            get { return new Vector2d(X.Max, Y.Max); }
        }

        
        /// <summary>
        /// Returns the area of the interval.
        /// </summary>
        public double Area
        {
            get { return Math.Abs(X.Delta * Y.Delta); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Interval3d As3d
        {
            get => new Interval3d(X, Y, Intervald.Zero);
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("({0} to {1}, {2} to {3})", X.A, X.B, Y.A, Y.B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Interval2d other, double epsilon = D.ZeroTolerance)
        {
            return 
                X.ApproxEquals(other.X, epsilon) && 
                Y.ApproxEquals(other.Y, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2d Evaluate(Vector2d point)
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
        public Vector2d Normalize(Vector2d point)
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
        public Vector2d Clamp(Vector2d point)
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
        public Vector2d Repeat(Vector2d point)
        {
            point.X = X.Repeat(point.X);
            point.Y = Y.Repeat(point.Y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector2d point)
        {
            return 
                X.Contains(point.X) && 
                Y.Contains(point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsIncl(Vector2d point)
        {
            return 
                X.ContainsIncl(point.X) && 
                Y.ContainsIncl(point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vector2d delta)
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
        public void Expand(Vector2d delta)
        {
            X.Expand(delta.X);
            Y.Expand(delta.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void Include(Vector2d point)
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
        public void Include(IEnumerable<Vector2d> points)
        {
            Include(new Interval2d(points));
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Deconstruct(out Intervald x, out Intervald y)
        {
            x = X;
            y = Y;
        }
    }
}
