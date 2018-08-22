
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using F = SpatialSlur.SlurMath.Constantsf;

namespace SpatialSlur
{
    /// <summary>
    /// Represents a single precision interval.
    /// </summary>
    [Serializable]
    public partial struct Intervalf
    {
        #region Static Members
        
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
            return SlurMath.Remap(t, from.A, from.B, to.A, to.B);
        }


#if OBSOLETE
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
#endif

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
        public float Delta
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


        /// <inheritdoc />
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
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Intervalf other, float epsilon = F.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(A, other.A, epsilon) &&
                SlurMath.ApproxEquals(B, other.B, epsilon);
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

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float Repeat(float t)
        {
            return SlurMath.Repeat(t, A, B);
        }


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
                IncludeNeg(t);
            else
                IncludePos(t);
        }


        /// <summary>
        /// Expands this interval to include another
        /// </summary>
        /// <param name="other"></param>
        public void Include(Intervalf other)
        {
            if (IsDecreasing)
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
        internal void IncludeNeg(float t)
        {
            if (t > A) A = t;
            else if (t < B) B = t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        internal void IncludePos(float t)
        {
            if (t > B) B = t;
            else if (t < A) A = t;
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
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Deconstruct(out float a, out float b)
        {
            a = A;
            b = B;
        }
    }
}
