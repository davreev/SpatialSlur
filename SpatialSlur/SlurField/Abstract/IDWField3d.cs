using System;
using System.Collections.Generic;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDWField3d
    {
        /// <summary></summary>
        public static readonly IDWFieldFactory<double> Double = new IDWField3dDouble.Factory();

        /// <summary></summary>
        public static readonly IDWFieldFactory<Vec3d> Vec3d = new IDWField3dVec3d.Factory();
    }


    /// <summary>
    /// Field type that uses inverse distance weighting to interpolate between known spatial values.
    /// https://en.wikipedia.org/wiki/Inverse_distance_weighting
    /// </summary>
    [Serializable]
    public abstract class IDWField3d<T> : IField2d<T>, IField3d<T>, IGradient2d<T>, IGradient3d<T>
    {
        private List<IDWPoint3d<T>> _valuePoints = new List<IDWPoint3d<T>>();
        private T _defaultValue;
        private double _defaultWeight;
        private double _power;
        private double _epsilon = SlurMath.ZeroTolerance;


        /// <summary>
        /// 
        /// </summary>
        public IDWField3d(double power, double epsilon = SlurMath.ZeroTolerance)
        {
            Power = power;
            Epsilon = epsilon;
        }


        /// <summary>
        /// 
        /// </summary>
        public List<IDWPoint3d<T>> Points
        {
            get { return _valuePoints; }
        }


        /// <summary>
        /// 
        /// </summary>
        public T DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public double DefaultWeight
        {
            get => _defaultWeight;
            set => _defaultWeight = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public double Power
        {
            get => _power;
            set => _power = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public double Epsilon
        {
            get { return _epsilon; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value can not be negative.");

                _epsilon = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDWField3d<T> Duplicate();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T ValueAt(Vec3d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="gx"></param>
        /// <param name="gy"></param>
        /// <param name="gz"></param>
        public abstract void GradientAt(Vec3d point, out T gx, out T gy, out T gz);


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T IField2d<T>.ValueAt(Vec2d point)
        {
            return ValueAt(new Vec3d(point.X, point.Y, 0.0));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="gx"></param>
        /// <param name="gy"></param>
        void IGradient2d<T>.GradientAt(Vec2d point, out T gx, out T gy)
        {
            GradientAt(point, out gx, out gy, out T gz);
        }

        #endregion
    }
}
