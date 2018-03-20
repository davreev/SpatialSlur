

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
    public static class IDWPoint3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <param name="influence"></param>
        /// <returns></returns>
        public static IDWPoint3d<T> Create<T>(Vec3d point, T value, double influence = 1.0)
        {
            return new IDWPoint3d<T>()
            {
                Point = point,
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
        public static IDWPoint3d<T> Create<T>(IDWPoint3d<T> other)
        {
            return new IDWPoint3d<T>()
            {
                Point = other.Point,
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
        public static IDWPoint3d<T> Create<T, U>(IDWPoint3d<U> other, Func<U, T> converter)
        {
            return new IDWPoint3d<T>()
            {
                Point = other.Point,
                Value = converter(other.Value),
                Influence = other.Influence
            };
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IDWPoint3d<T> : IDWObject3d<T>
    {
        private Vec3d _point;


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Point
        {
            get => _point;
            set => _point = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double DistanceTo(Vec3d point)
        {
            return _point.DistanceTo(point);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDWObject3d<T> Duplicate()
        {
            return IDWPoint3d.Create(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public override IDWObject3d<U> Convert<U>(Func<T, U> converter)
        {
            return IDWPoint3d.Create(this, converter);
        }
    }
}
