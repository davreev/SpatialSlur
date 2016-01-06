using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// represents a domain defined by two numerical extremes
    /// </summary>
    public struct Domain
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Domain Unit
        {
            get { return new Domain(0.0, 1.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double Remap(double t, Domain from, Domain to)
        {
            if (!from.IsValid) 
                throw new InvalidOperationException("Can't remap from an invalid domain");

            return SlurMath.Remap(t, from.t0, from.t1, to.t0, to.t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Domain Intersect(Domain d0, Domain d1)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Domain Union(Domain d0, Domain d1)
        {
            throw new NotImplementedException();
        }

        #endregion


        public double t0, t1;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        public Domain(double t0, double t1)
        {
            this.t0 = t0;
            this.t1 = t1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public Domain(IList<double> values)
        {
            double min = values[0];
            double max = min;
            Object locker = new Object();

            Parallel.ForEach(Partitioner.Create(1, values.Count), range =>
            {
                double myMin = min;
                double myMax = max;

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double t = values[i];
                    if (t < myMin) myMin = t;
                    else if (t > myMax) myMax = t;
                }

                lock (locker)
                {
                    if (myMin < min) min = myMin;
                    if (myMax > max) max = myMax;
                }
            });

            t0 = min;
            t1 = max;
        }


        /// <summary>
        /// Returns true if t1 is greater than or equal to t0.
        /// </summary>
        public bool IsIncreasing
        {
            get { return t1 >= t0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsValid
        {
            get { return t0 != t1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Span
        {
            get { return t1 - t0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Mid
        {
            get { return (t0 + t1) * 0.5; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Min
        {
            get { return Math.Min(t0, t1); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Max
        {
            get { return Math.Max(t0, t1); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} to {1}", t0, t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        public void Set(double t0, double t1)
        {
            this.t0 = t0;
            this.t1 = t1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(Domain other, double epsilon)
        {
            return Math.Abs(other.t0 - t0) < epsilon && Math.Abs(other.t1 - t1) < epsilon;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public double Evaluate(double t)
        {
            return SlurMath.Lerp(t0, t1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <exception cref="DivideByZeroException">
        /// thrown if the domain is invalid </exception>
        public double Normalize(double t)
        {
            return SlurMath.Normalize(t, t0, t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Clamp(double t)
        {
            if (IsIncreasing)
                return SlurMath.Clamp(t, t0, t1);
            else
                return SlurMath.Clamp(t, t1, t0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Nearest(double t)
        {
            return SlurMath.Nearest(t, t0, t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Ramp(double t)
        {
            return SlurMath.Ramp(t, t0, t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SmoothStep(double t)
        {
            return SlurMath.SmoothStep(t, t0, t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SmootherStep(double t)
        {
            return SlurMath.SmootherStep(t, t0, t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException">
        /// thrown if the domain is invalid </exception>
        public double Wrap(double t)
        {
            return SlurMath.Wrap(t, t0, t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Contains(double t)
        {
            if (IsIncreasing)
                return SlurMath.Contains(t, t0, t1);
            else
                return SlurMath.Contains(t, t1, t0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ContainsIncl(double t)
        {
            if (IsIncreasing)
                return SlurMath.ContainsIncl(t, t0, t1);
            else
                return SlurMath.ContainsIncl(t, t1, t0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void Translate(double t)
        {
            t0 += t;
            t1 += t;
        }


        /// <summary>
        /// expands the domain on both sides by t
        /// </summary>
        /// <param name="t"></param>
        public void Expand(double t)
        {
            if (IsIncreasing)
            {
                t0 -= t;
                t1 += t;
            }
            else
            {
                t0 += t;
                t1 -= t;
            }
        }


        /// <summary>
        /// expands the domain to include t
        /// </summary>
        /// <param name="t"></param>
        public void Include(double t)
        {
            if (IsIncreasing)
            {
                if (t > t1) 
                    t1 = t;
                else if (t < t0)
                    t0 = t;
            }
            else
            {
                if (t > t0)
                    t0 = t;
                else if (t < t1)
                    t1 = t;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Reverse()
        {
            double t = t0;
            t0 = t1;
            t1 = t;
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeIncreasing()
        {
            if (!IsIncreasing)
                Reverse();
        }
    }
}
