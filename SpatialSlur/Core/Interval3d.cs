
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
    /// Represents a double precision interval in 3 dimensions.
    /// </summary>
    [Serializable]
    public partial struct Interval3d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Interval3d Zero = new Interval3d();
        /// <summary></summary>
        public static readonly Interval3d Unit = new Interval3d(0.0, 1.0, 0.0, 1.0, 0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public static implicit operator string(Interval3d interval)
        {
            return $"({interval.X}, {interval.Y}, {interval.Z})";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Interval3d operator +(Interval3d d, Vector3d v)
        {
            d.X.Translate(v.X);
            d.Y.Translate(v.Y);
            d.Z.Translate(v.Z);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Interval3d operator -(Interval3d d, Vector3d v)
        {
            d.X.Translate(-v.X);
            d.Y.Translate(-v.Y);
            d.Z.Translate(-v.Z);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Interval3d operator *(Interval3d d, double t)
        {
            d.X.Scale(t);
            d.Y.Scale(t);
            d.Z.Scale(t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Interval3d operator *(double t, Interval3d d)
        {
            d.X.Scale(t);
            d.Y.Scale(t);
            d.Z.Scale(t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Interval3d operator /(Interval3d d, double t)
        {
            t = 1.0 / t;
            d.X.Scale(t);
            d.Y.Scale(t);
            d.Z.Scale(t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector3d Remap(Vector3d point, Interval3d from, Interval3d to)
        {
            point.X = Intervald.Remap(point.X, from.X, to.X);
            point.Y = Intervald.Remap(point.Y, from.Y, to.Y);
            point.Y = Intervald.Remap(point.Z, from.Z, to.Z);
            return point;
        }

        #endregion


        /// <summary></summary>
        public Intervald X;
        /// <summary></summary>
        public Intervald Y;
        /// <summary></summary>
        public Intervald Z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Interval3d(Intervald x, Intervald y, Intervald z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ab"></param>
        public Interval3d(Vector3d ab)
        {
            X = new Intervald(ab.X);
            Y = new Intervald(ab.Y);
            Z = new Intervald(ab.Z);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Interval3d(Vector3d a, Vector3d b)
        {
            X = new Intervald(a.X, b.X);
            Y = new Intervald(a.Y, b.Y);
            Z = new Intervald(a.Z, b.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offset"></param>
        public Interval3d(Vector3d center, double offset)
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
        public Interval3d(Vector3d center, double offsetX, double offsetY, double offsetZ)
        {
            X = new Intervald(center.X - offsetX, center.X + offsetX);
            Y = new Intervald(center.Y - offsetY, center.Y + offsetY);
            Z = new Intervald(center.Z - offsetZ, center.Z + offsetZ);
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
        public Interval3d(double x0, double x1, double y0, double y1, double z0, double z1)
        {
            X = new Intervald(x0, x1);
            Y = new Intervald(y0, y1);
            Z = new Intervald(z0, z1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public Interval3d(IEnumerable<Vector3d> points)
            : this()
        {
            (var x, var y, var z) = points.First();
            X = new Intervald(x);
            Y = new Intervald(y);
            Z = new Intervald(z);

            foreach (var p in points.Skip(1))
            {
                X.IncludePos(p.X);
                Y.IncludePos(p.Y);
                Z.IncludePos(p.Z);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Interval2d XY
        {
            get { return new Interval2d(X, Y); }
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
        public Vector3d A
        {
            get { return new Vector3d(X.A, Y.A, Z.A); }
            set
            {
                X.A = value.X;
                Y.A = value.Y;
                Z.A = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d B
        {
            get { return new Vector3d(X.B, Y.B, Z.B); }
            set
            {
                X.B = value.X;
                Y.B = value.Y;
                Z.B = value.Z;
            }
        }


        /// <summary>
        /// B - A
        /// </summary>
        public Vector3d Delta
        {
            get { return new Vector3d(X.Delta, Y.Delta, Z.Delta); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3i Sign
        {
            get { return new Vector3i(X.Sign, Y.Sign, Z.Sign); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Mid
        {
            get { return new Vector3d(X.Mid, Y.Mid, Z.Mid); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Min
        {
            get { return new Vector3d(X.Min, Y.Min, Z.Min); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Max
        {
            get { return new Vector3d(X.Max, Y.Max, Z.Max); }
        }


        /// <summary>
        /// Returns the area of the interval.
        /// </summary>
        public double Area
        {
            get { return Math.Abs(X.Delta * Y.Delta * Z.Delta); }
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("({0} to {1}, {2} to {3}, {4} to {5})", X.A, X.B, Y.A, Y.B, Z.A, Z.B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Interval3d other, double epsilon = D.ZeroTolerance)
        {
            return
                X.ApproxEquals(other.X, epsilon) && 
                Y.ApproxEquals(other.Y, epsilon) && 
                Z.ApproxEquals(other.Z, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3d Evaluate(Vector3d point)
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
        public Vector3d Normalize(Vector3d point)
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
        public Vector3d Clamp(Vector3d point)
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
        public Vector3d Repeat(Vector3d point)
        {
            point.X = X.Repeat(point.X);
            point.Y = Y.Repeat(point.Y);
            point.Z = Z.Repeat(point.Z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector3d point)
        {
            return 
                X.Contains(point.X) && 
                Y.Contains(point.Y) && 
                Z.Contains(point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsIncl(Vector3d point)
        {
            return 
                X.ContainsIncl(point.X) && 
                Y.ContainsIncl(point.Y) && 
                Z.ContainsIncl(point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vector3d delta)
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
        public void Expand(Vector3d delta)
        {
            X.Expand(delta.X);
            Y.Expand(delta.Y);
            Z.Expand(delta.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void Include(Vector3d point)
        {
            X.Include(point.X);
            Y.Include(point.Y);
            Z.Include(point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Include(Interval3d other)
        {
            X.Include(other.X);
            Y.Include(other.Y);
            Z.Include(other.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void Include(IEnumerable<Vector3d> points)
        {
            Include(new Interval3d(points));
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out Intervald x, out Intervald y, out Intervald z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
