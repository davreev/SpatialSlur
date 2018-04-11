
/*
 * Notes 
 */

using System;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDWConstant3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="distance"></param>
        /// <param name="value"></param>
        /// <param name="influence"></param>
        /// <returns></returns>
        public static IDWConstant3d<T> Create<T>(double distance, T value, double influence = 1.0)
        {
            return new IDWConstant3d<T>()
            {
                Distance = distance,
                Value = value,
                Influence = influence
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IDWConstant3d<T> Create<T>(IDWConstant3d<T> other)
        {
            return new IDWConstant3d<T>()
            {
                Distance = other.Distance,
                Value = other.Value,
                Influence = other.Influence
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IDWConstant3d<T> Create<T, U>(IDWConstant3d<U> other, Func<U, T> converter)
        {
            return new IDWConstant3d<T>()
            {
                Distance = other.Distance,
                Value = converter(other.Value),
                Influence = other.Influence
            };
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IDWConstant3d<T> : IDWObject3d<T>
    {
        private double _distance = 0.0;


        /// <summary>
        /// 
        /// </summary>
        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }


        /// <inheritdoc />
        public override double DistanceTo(Vec3d point)
        {
            return _distance;
        }


        /// <inheritdoc />
        public override IDWObject3d<T> Duplicate()
        {
            return IDWConstant3d.Create(this);
        }


        /// <inheritdoc />
        public override IDWObject3d<U> Convert<U>(Func<T, U> converter)
        {
            return IDWConstant3d.Create(this, converter);
        }
    }
}
