
/*
 * Notes
 */

using System;
using SpatialSlur;
using SpatialSlur.Fields;

namespace SpatialSlur.Fields
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
        public static IDWPoint3d<T> Create<T>(Vector3d point, T value, double influence = 1.0)
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
        /// <typeparam name="U"></typeparam>
        /// <param name="other"></param>
        /// <param name="converter"></param>
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
        private Vector3d _point;


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Point
        {
            get => _point;
            set => _point = value;
        }


        /// <inheritdoc />
        public override double DistanceTo(Vector3d point)
        {
            return _point.DistanceTo(point);
        }


        /// <inheritdoc />
        public override IDWObject3d<T> Duplicate()
        {
            return IDWPoint3d.Create(this);
        }


        /// <inheritdoc />
        public override IDWObject3d<U> Convert<U>(Func<T, U> converter)
        {
            return IDWPoint3d.Create(this, converter);
        }
    }
}
