using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Represents a single precision interval.
    /// https://en.wikipedia.org/wiki/Interval_(mathematics)
    /// </summary>
    [Serializable]
    public partial struct Intervalf
    {
        #region Static
        
        /// <summary></summary>
        public static readonly Intervalf Zero = new Intervalf();
        /// <summary></summary>
        public static readonly Intervalf Unit = new Intervalf(0.0f, 1.0f);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Intervalf operator +(Intervalf d, float t)
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
        public static Intervalf operator -(Intervalf d, float t)
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
        public static Intervalf operator *(Intervalf d, float t)
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
        public static Intervalf operator *(float t, Intervalf d)
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
        public static Intervalf operator /(Intervalf d, float t)
        {
            d.Scale(1.0f / t);
            return d;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float Remap(float t, Intervalf from, Intervalf to)
        {
            if (!from.IsValid) 
                throw new InvalidOperationException("Can't remap from an invalid interval");

            return SlurMath.Remap(t, from.A, from.B, to.A, to.B);
        }


        /// <summary>
        /// Returns the union of a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Intervalf Union(Intervalf a, Intervalf b)
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
        public static Intervalf Intersect(Intervalf a, Intervalf b)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the region of a that is not in b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Intervalf Difference(Intervalf a, Intervalf b)
        {
            throw new NotImplementedException();
        }

        #endregion


        /// <summary></summary>
        public float A;
        /// <summary></summary>
        public float B;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ab"></param>
        public Intervalf(float ab)
        {
            A = B = ab;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Intervalf(float a, float b)
        {
            A = a;
            B = b;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public Intervalf(IEnumerable<float> values)
        {
            A = B = values.First();

            foreach (var t in values.Skip(1))
                IncludeIncreasing(t);
        }


        /// <summary>
        /// Returns true if A is greater than B.
        /// </summary>
        public bool IsIncreasing
        {
            get { return B > A; }
        }



        /// <summary>
        /// Returns true if A is less than B.
        /// </summary>
        public bool IsDecreasing
        {
            get { return B < A; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Orientation
        {
            get { return Math.Sign(B - A); }
        }


        /// <summary>
        /// Returns true if A equals B.
        /// </summary>
        public bool IsValid
        {
            get { return A != B; }
        }


        /// <summary>
        /// Defined as B - A
        /// </summary>
        public float Length
        {
            get { return B - A; }
        }


        /// <summary>
        /// 
        /// </summary>
        public float Mid
        {
            get { return (A + B) * 0.5f; }
        }


        /// <summary>
        /// 
        /// </summary>
        public float Min
        {
            get { return Math.Min(A, B); }
        }


        /// <summary>
        /// 
        /// </summary>
        public float Max
        {
            get { return Math.Max(A, B); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} to {1}", A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ab"></param>
        public void Set(float ab)
        {
            A = B = ab;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Set(float a, float b)
        {
            this.A = a;
            this.B = b;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Intervalf other, float tolerance = SlurMath.ZeroTolerancef)
        {
            return
                SlurMath.ApproxEquals(A, other.A, tolerance) &&
                SlurMath.ApproxEquals(B, other.B, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public float Evaluate(float t)
        {
            return SlurMath.Lerp(A, B, t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float Normalize(float t)
        {
            return SlurMath.Normalize(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float Clamp(float t)
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
        public float Nearest(float t)
        {
            return SlurMath.Nearest(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float Ramp(float t)
        {
            return SlurMath.Ramp(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float SmoothStep(float t)
        {
            return SlurMath.SmoothStep(t, A, B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float SmootherStep(float t)
        {
            return SlurMath.SmootherStep(t, A, B);
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float Wrap(float t)
        {
            return SlurMath.Wrap(t, A, B);
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Contains(float t)
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
        public bool ContainsIncl(float t)
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
        public void Scale(float t)
        {
            A *= t;
            B *= t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void Translate(float t)
        {
            A += t;
            B += t;
        }


        /// <summary>
        /// Expands the interval on both sides by the given value
        /// </summary>
        /// <param name="t"></param>
        public void Expand(float t)
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
        public void Include(float t)
        {
            if (IsDecreasing)
                IncludeDecreasing(t);
            else
                IncludeIncreasing(t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        internal void IncludeDecreasing(float t)
        {
            if (t > A) A = t;
            else if (t < B) B = t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        internal void IncludeIncreasing(float t)
        {
            if (t > B) B = t;
            else if (t < A) A = t;
        }


        /// <summary>
        /// Expands this interval to include another
        /// </summary>
        /// <param name="other"></param>
        public void Include(Intervalf other)
        {
            Include(other.A);
            Include(other.B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Include(IEnumerable<float> values)
        {
            Include(new Intervalf(values));
        }


        /// <summary>
        /// 
        /// </summary>
        public void Reverse()
        {
            float t = A;
            A = B;
            B = t;
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
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out float a, out float b)
        {
            a = A;
            b = B;
        }
    }
}
