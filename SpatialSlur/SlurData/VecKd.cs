using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;


namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    public class VecKd
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(VecKd v0, VecKd v1)
        {
            double result = 0.0;

            for (int i = 0; i < v0.K; i++)
                result += v0[i] * v1[i];

            return result;
        }

        #endregion


        private readonly double[] _values;
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        public VecKd(int k)
        {
            if (k < 2)
                throw new System.ArgumentOutOfRangeException("The vector must have at least 2 dimensions");

            _values = new double[k];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VecKd(VecKd other)
            : this(other.K)
        {
            Array.Copy(other._values, _values, K);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public VecKd(IList<double> values)
            : this(values.Count)
        {
            _values.Set(values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VecKd(Vec2d other)
            : this(2)
        {
            this[0] = other.x;
            this[1] = other.y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VecKd(Vec3d other)
            : this(3)
        {
            this[0] = other.x;
            this[1] = other.y;
            this[2] = other.z;
        }


        /// <summary>
        /// Returns the number of dimensions in this vector.
        /// </summary>
        public int K
        {
            get { return _values.Length; }
        }


        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double this[int i]
        {
            get { return _values[i]; }
            set { _values[i] = value; }
        }


        /// <summary>
        /// Returns the underlying array of values.
        /// </summary>
        public IList<double> Values
        {
            get { return _values; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Set(VecKd other)
        {
            Array.Copy(other._values, _values, K);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(VecKd other, double epsilon)
        {
            for (int i = 0; i < K; i++)
                if (Math.Abs(other[i] - this[i]) >= epsilon) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(VecKd other, VecKd epsilon)
        {
            for (int i = 0; i < K; i++)
                if (Math.Abs(other[i] - this[i]) >= epsilon[i]) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            return Math.Sqrt(GetSquareLength());
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public double GetSquareLength()
        {
            double result = 0.0;
            for (int i = 0; i < K; i++)
            {
                double x = this[i];
                result += x * x;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetManhattanLength()
        {
            double result = 0.0;
            for (int i = 0; i < K; i++)
                result += Math.Abs(this[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(VecKd other)
        {
            return Math.Sqrt(SquareDistanceTo(other));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(VecKd other)
        {
            double result = 0.0;
            for (int i = 0; i < K; i++)
            {
                double d = other[i] - this[i];
                result += d * d;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(VecKd other)
        {
            double result = 0.0;
            for (int i = 0; i < K; i++)
                result += Math.Abs(other[i] - this[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Add(VecKd other)
        {
            for (int i = 0; i < K; i++)
                this[i] += other[i];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Add(VecKd other, VecKd result)
        {
            for (int i = 0; i < K; i++)
                result[i] = this[i] + other[i];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Subtract(VecKd other)
        {
            for (int i = 0; i < K; i++)
                this[i] -= other[i];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Subtract(VecKd other, VecKd result)
        {
            for (int i = 0; i < K; i++)
                result[i] = this[i] - other[i];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public void Scale(double factor)
        {
            for (int i = 0; i < K; i++)
                this[i] *= factor;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="result"></param>
        public void Scale(double factor, VecKd result)
        {
            for (int i = 0; i < K; i++)
                result[i] = this[i] * factor;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public void AddScaled(VecKd other, double factor)
        {
            for (int i = 0; i < K; i++)
                this[i] += other[i] * factor;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <param name="result"></param>
        public void AddScaled(VecKd other, double factor, VecKd result)
        {
            for (int i = 0; i < K; i++)
                result[i] = this[i] + other[i] * factor;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool Unitize()
        {
            double d = GetSquareLength();
            if (d == 0.0) return false;

            Scale(1.0 / Math.Sqrt(d));
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Negate()
        {
            Scale(-1.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void LerpTo(VecKd other, double factor)
        {
            for (int i = 0; i < K; i++)
            {
                double x = this[i];
                this[i] = x + (other[i] - x) * factor;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <param name="result"></param>
        public void LerpTo(VecKd other, double factor, VecKd result)
        {
            for (int i = 0; i < K; i++)
            {
                double x = this[i];
                result[i] = x + (other[i] - x) * factor;
            }
        }

    }
}
