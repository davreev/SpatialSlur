using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Represents a double precision numerical domain.
    /// </summary>
    [Serializable]
    public struct Domain1d
    {
        #region Static
        
        /// <summary></summary>
        public static readonly Domain1d Zero = new Domain1d();
        /// <summary></summary>
        public static readonly Domain1d Unit = new Domain1d(0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Domain1d operator +(Domain1d d, double t)
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
        public static Domain1d operator -(Domain1d d, double t)
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
        public static Domain1d operator *(Domain1d d, double t)
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
        public static Domain1d operator *(double t, Domain1d d)
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
        public static Domain1d operator /(Domain1d d, double t)
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
        public static double Remap(double t, Domain1d from, Domain1d to)
        {
            if (!from.IsValid) 
                throw new InvalidOperationException("Can't remap from an invalid domain");

            return SlurMath.Remap(t, from.T0, from.T1, to.T0, to.T1);
        }

        /// <summary>
        /// Returns the union of a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Domain1d Union(Domain1d a, Domain1d b)
        {
            a.Include(b.T0);
            a.Include(b.T1);
            return a;
        }


        /// <summary>
        /// Returns the region of a that is also in b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Domain1d Intersect(Domain1d a, Domain1d b)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the region of a that is not in b.
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Domain1d Difference(Domain1d d0, Domain1d d1)
        {
            throw new NotImplementedException();
        }

        #endregion


        /// <summary></summary>
        public double T0;
        /// <summary></summary>
        public double T1;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public Domain1d(double t)
        {
            T0 = T1 = t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        public Domain1d(double t0, double t1)
        {
            this.T0 = t0;
            this.T1 = t1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public Domain1d(IEnumerable<double> values)
            : this()
        {
            T0 = T1 = values.First();
            Include(values.Skip(1), ref T0, ref T1);
        }


        /// <summary>
        /// Returns true if t1 is greater than t0.
        /// </summary>
        public bool IsIncreasing
        {
            get { return T1 > T0; }
        }



        /// <summary>
        /// Returns true if t1 is less than t0
        /// </summary>
        public bool IsDecreasing
        {
            get { return T1 < T0; }
        }


        /// <summary>
        /// Returns true if t0 equals t1.
        /// </summary>
        public bool IsValid
        {
            get { return T0 != T1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Span
        {
            get { return T1 - T0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Mid
        {
            get { return (T0 + T1) * 0.5; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Min
        {
            get { return Math.Min(T0, T1); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Max
        {
            get { return Math.Max(T0, T1); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} to {1}", T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void Set(double t)
        {
            T0 = T1 = t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        public void Set(double t0, double t1)
        {
            this.T0 = t0;
            this.T1 = t1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Domain1d other, double tolerance)
        {
            return Math.Abs(other.T0 - T0) < tolerance && Math.Abs(other.T1 - T1) < tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public double Evaluate(double t)
        {
            return SlurMath.Lerp(T0, T1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <exception cref="DivideByZeroException">
        /// thrown if the domain is invalid </exception>
        public double Normalize(double t)
        {
            return SlurMath.Normalize(t, T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Clamp(double t)
        {
            if (IsDecreasing)
                return SlurMath.Clamp(t, T1, T0);
            else
                return SlurMath.Clamp(t, T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Nearest(double t)
        {
            return SlurMath.Nearest(t, T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Ramp(double t)
        {
            return SlurMath.Ramp(t, T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SmoothStep(double t)
        {
            return SlurMath.SmoothStep(t, T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SmootherStep(double t)
        {
            return SlurMath.SmootherStep(t, T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Wrap(double t)
        {
            return SlurMath.Wrap(t, T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Contains(double t)
        {
            if (IsDecreasing)
                return SlurMath.Contains(t, T1, T0);
            else
                return SlurMath.Contains(t, T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ContainsIncl(double t)
        {
            if (IsDecreasing)
                return SlurMath.ContainsIncl(t, T1, T0);
            else
                return SlurMath.ContainsIncl(t, T0, T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void Scale(double t)
        {
            T0 *= t;
            T1 *= t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void Translate(double t)
        {
            T0 += t;
            T1 += t;
        }


        /// <summary>
        /// Epands the domain on both sides by the given value
        /// </summary>
        /// <param name="t"></param>
        public void Expand(double t)
        {
            if (IsDecreasing)
            {
                T0 += t;
                T1 -= t;
            }
            else
            {
                T0 -= t;
                T1 += t;
            }
        }


        /// <summary>
        /// Expands the domain to include the given value.
        /// </summary>
        /// <param name="t"></param>
        public void Include(double t)
        {
            if (IsDecreasing)
            {
                if (t > T0) T0 = t;
                else if (t < T1) T1 = t;
            }
            else
            {
                if (t > T1) T1 = t;
                else if (t < T0) T0 = t;
            }
        }


        /// <summary>
        /// Expands this domain to include another
        /// </summary>
        /// <param name="other"></param>
        public void Include(Domain1d other)
        {
            Include(other.T0);
            Include(other.T1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Include(IEnumerable<double> values)
        {
            if (IsDecreasing)
                Include(values, ref T1, ref T0);
            else
                Include(values, ref T0, ref T1);
        }


        /// <summary>
        /// 
        /// </summary>
        private void Include(IEnumerable<double> values, ref double min, ref double max)
        {
            foreach (double t in values)
            {
                if (t < min) min = t;
                else if (t > max) max = t;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Reverse()
        {
            double t = T0;
            T0 = T1;
            T1 = t;
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
    }
}
