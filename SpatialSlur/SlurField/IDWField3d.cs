using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Field type that uses inverse distance weighting to interpolate between known spatial values.
    /// https://en.wikipedia.org/wiki/Inverse_distance_weighting
    /// </summary>
    [Serializable]
    public abstract class IDWField3d<T> : IField2d<T>, IField3d<T>
    {
        /// <summary></summary>
        public T DefaultValue;
        /// <summary></summary>
        public double DefaultWeight;
        /// <summary></summary>
        public double Power = 3.0;


        private List<IDWPoint3d<T>> _valuePoints = new List<IDWPoint3d<T>>();
        private double _epsilon = 1.0e-4;


        /// <summary>
        /// 
        /// </summary>
        public IDWField3d(double power)
        {
            Power = power;
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
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T ValueAt(Vec3d point);


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


        #endregion
    }
}
