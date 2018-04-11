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
        private List<IDWObject3d<T>> _objects = new List<IDWObject3d<T>>();
       
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
        public List<IDWObject3d<T>> Objects
        {
            get { return _objects; }
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


        /// <inheritdoc />
        public abstract T ValueAt(Vec3d point);


        /// <inheritdoc />
        public abstract void GradientAt(Vec3d point, out T gx, out T gy, out T gz);


        #region Explicit interface implementations

        /// <inheritdoc />
        T IField2d<T>.ValueAt(Vec2d point)
        {
            return ValueAt(new Vec3d(point.X, point.Y, 0.0));
        }


        /// <inheritdoc />
        void IGradient2d<T>.GradientAt(Vec2d point, out T gx, out T gy)
        {
            GradientAt(point, out gx, out gy, out T gz);
        }

        #endregion
    }
}
