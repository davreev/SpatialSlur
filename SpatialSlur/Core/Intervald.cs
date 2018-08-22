
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
    /// Represents a double precision interval.
    /// </summary>
    [Serializable]
    public partial struct Intervald
    {
        #region Static Members
        
        /// <summary></summary>
        public static readonly Intervald Zero = new Intervald();
        /// <summary></summary>
        public static readonly Intervald Unit = new Intervald(0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public static implicit operator string(Intervald interval)
        {
            return interval.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Intervald operator +(Intervald d, double t)
        {
            d.Translate(t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Intervald operator -(Intervald d, double t)
        {
            d.Translate(-t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Intervald operator *(Intervald d, double t)
        {
            d.Scale(t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Intervald operator *(double t, Intervald d)
        {
            d.Scale(t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Intervald operator /(Intervald d, double t)
        {
            d.Scale(1.0 / t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double Remap(double t, Intervald from, Intervald to)
        {
            return SlurMath.Remap(t, from.A, from.B, to.A, to.B);
        }


 #if OBSOLETE
        /// <summary>
        /// Returns the union of a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Intervald Union(Intervald a, Intervald b)
        {
            a.Include(b.A);
            a.Include(b.B);
            return a;
        }


        /// <summary>
        /// Returns the region of a that is also in b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Intervald Intersect(Intervald a, Intervald b)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the region of a that is not in b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Intervald Difference(Intervald a, Intervald b)
        {
            throw new NotImplementedException();
        }
#endif

        #endregion


        /// <summary>The start of the interval</summary>
        public double A;

        /// <summary>The end of the interval</summary>
        public double B;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ab"></param>
        public Intervald(double ab)
        {
            A = B = ab;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Intervald(double a, double b)
        {
            A = a;
            B = b;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public Intervald(IEnumerable<double> values)
        {
            A = B = values.First();

            foreach (var t in values.Skip(1))
                IncludePos(t);
        }


        /// <summary>
        /// Returns true if A is less than B.
        /// </summary>
        public bool IsIncreasing
        {
            get { return A < B; }
        }


        /// <summary>
        /// Returns true if A is greater than B.
        /// </summary>
        public bool IsDecreasing
        {
            get { return A > B; }
        }


        /// <summary>
        /// Returns true if A equals B.
        /// </summary>
        public bool IsValid
        {
            get { return A != B; }
        }


        /// <summary>
        /// B - A
        /// </summary>
        public double Delta
        {
            get { return B - A; }
        }


        /// <summary>
        /// Returns positive if this interval is increasing, negative if it's decreasing, and zero if it's invalid.
        /// </summary>
        /// <returns></returns>
        public int Sign
        {
            get { return Math.Sign(B - A); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Mid
        {
            get { return (A + B) * 0.5; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Min
        {
            get { return Math.Min(A, B); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Max
        {
            get { return Math.Max(A, B); }
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("{0} to {1}", A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ab"></param>
        public void Set(double ab)
        {
            A = B = ab;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Set(double a, double b)
        {
            A = a;
            B = b;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Intervald other, double epsilon = D.ZeroTolerance)
        {
            return 
                SlurMath.ApproxEquals(A, other.A, epsilon) &&
                SlurMath.ApproxEquals(B, other.B, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public double Evaluate(double t)
        {
            return SlurMath.Lerp(A, B, t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Normalize(double t)
        {
            return SlurMath.Normalize(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Clamp(double t)
        {
            if (IsDecreasing)
                return SlurMath.Clamp(t, B, A);
            else
                return SlurMath.Clamp(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Nearest(double t)
        {
            return SlurMath.Nearest(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Ramp(double t)
        {
            return SlurMath.Ramp(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SmoothStep(double t)
        {
            return SlurMath.SmoothStep(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SmootherStep(double t)
        {
            return SlurMath.SmootherStep(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Repeat(double t)
        {
            return SlurMath.Repeat(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Contains(double t)
        {
            if (IsDecreasing)
                return SlurMath.Contains(t, B, A);
            else
                return SlurMath.Contains(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ContainsIncl(double t)
        {
            if (IsDecreasing)
                return SlurMath.ContainsIncl(t, B, A);
            else
                return SlurMath.ContainsIncl(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void Scale(double t)
        {
            A *= t;
            B *= t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void Translate(double t)
        {
            A += t;
            B += t;
        }


        /// <summary>
        /// Expands the interval on both sides by the given value
        /// </summary>
        /// <param name="t"></param>
        public void Expand(double t)
        {
            if (IsDecreasing)
            {
                A += t;
                B -= t;
            }
            else
            {
                A -= t;
                B += t;
            }
        }


        /// <summary>
        /// Expands the interval to include the given value.
        /// </summary>
        /// <param name="t"></param>
        public void Include(double t)
        {
            if (IsDecreasing)
                IncludeNeg(t);
            else
                IncludePos(t);
        }


        /// <summary>
        /// Expands this interval to include another
        /// </summary>
        /// <param name="other"></param>
        public void Include(Intervald other)
        {
            if(IsDecreasing)
            {
                IncludeNeg(other.A);
                IncludeNeg(other.B);
            }
            else
            {
                IncludePos(other.A);
                IncludePos(other.B);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        internal void IncludeNeg(double t)
        {
            if (t > A) A = t;
            else if (t < B) B = t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        internal void IncludePos(double t)
        {
            if (t > B) B = t;
            else if (t < A) A = t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Include(IEnumerable<double> values)
        {
            Include(new Intervald(values));
        }


        /// <summary>
        /// 
        /// </summary>
        public void Reverse()
        {
            double tmp = A;
            A = B;
            B = tmp;
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeIncreasing()
        {
            if (IsDecreasing) Reverse();
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeDecreasing()
        {
            if (IsIncreasing) Reverse();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Deconstruct(out double a, out double b)
        {
            a = A;
            b = B;
        }
    }
}
