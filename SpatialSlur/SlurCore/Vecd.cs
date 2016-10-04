using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public class Vecd
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vecd v0, Vecd v1)
        {
            return VecMath.Dot(v0._values, v1._values, v0.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="result"></param>
        public static void Add(Vecd v0, Vecd v1, Vecd result)
        {
            VecMath.Add(v0._values, v1._values, v0.Count, result._values);
        }

        #endregion


        private readonly double[] _values;
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        public Vecd(int count)
        {
            _values = new double[count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Vecd(Vecd other)
            : this(other.Count)
        {
            Set(other);
        }


        /// <summary>
        /// Binds to an existing double array.
        /// </summary>
        /// <param name="values"></param>
        public Vecd(double[] values)
        {
            _values = values;
        }


        /// <summary>
        /// Copies values from an existing ICollection.
        /// </summary>
        /// <param name="values"></param>
        public Vecd(ICollection<double> values)
            : this(values.Count)
        {
            values.CopyTo(_values, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Vecd(Vec2d other)
            : this(2)
        {
            this[0] = other.x;
            this[1] = other.y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Vecd(Vec3d other)
            : this(3)
        {
            this[0] = other.x;
            this[1] = other.y;
            this[2] = other.z;
        }


        /// <summary>
        /// Returns the number of dimensions in this vector.
        /// </summary>
        public int Count
        {
            get { return _values.Length; }
        }


        /// <summary>
        /// Returns the value at the given index.
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
        public double[] Values
        {
            get { return _values; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Set(Vecd other)
        {
            other._values.CopyTo(_values, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(Vecd other, double epsilon)
        {
            return VecMath.Equals(_values, other._values, epsilon, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(Vecd other, Vecd epsilon)
        {
            return VecMath.Equals(_values, other._values, epsilon._values, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double Dot(Vecd other)
        {
            return VecMath.Dot(_values, other._values, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            return Math.Sqrt(VecMath.Dot(_values, _values, Count));
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public double GetSquareLength()
        {
            return VecMath.Dot(_values, _values, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetManhattanLength()
        {
            return VecMath.ManhattanLength(_values, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vecd other)
        {
            return Math.Sqrt(VecMath.SquareDistance(_values, other._values, Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vecd other)
        {
            return VecMath.SquareDistance(_values, other._values, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(Vecd other)
        {
            return VecMath.ManhattanDistance(_values, other._values, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Add(Vecd other)
        {
            VecMath.Add(_values, other._values, Count, _values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Add(Vecd other, Vecd result)
        {
            VecMath.Add(_values, other._values, Count, result._values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Subtract(Vecd other)
        {
            VecMath.Subtract(_values, other._values, Count, _values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Subtract(Vecd other, Vecd result)
        {
            VecMath.Subtract(_values, other._values, Count, result._values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public void Scale(double factor)
        {
            VecMath.Scale(_values, factor, Count, _values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="result"></param>
        public void Scale(double factor, Vecd result)
        {
            VecMath.Scale(_values, factor, Count, result._values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public void AddScaled(Vecd other, double factor)
        {
            VecMath.AddScaled(_values, other._values, factor, Count, _values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <param name="result"></param>
        public void AddScaled(Vecd other, double factor, Vecd result)
        {
            VecMath.AddScaled(_values, other._values, factor, Count, result._values);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool Unitize()
        {
            return VecMath.Unitize(_values, Count, _values);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool Unitize(Vecd result)
        {
            return VecMath.Unitize(_values, Count, result._values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void LerpTo(Vecd other, double factor)
        {
            VecMath.Lerp(_values, other._values, factor, Count, _values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <param name="result"></param>
        public void LerpTo(Vecd other, double factor, Vecd result)
        {
            VecMath.Lerp(_values, other._values, factor, Count, result._values);
        }
    }
}
