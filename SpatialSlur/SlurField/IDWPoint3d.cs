using System;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IDWPoint3d<T>
    {
        private Vec3d _point;
        private T _value;
        private double _influence = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Point { get => _point; set => _point = value; }


        /// <summary>
        /// 
        /// </summary>
        public T Value { get => _value; set => this._value = value; }


        /// <summary>
        /// 
        /// </summary>
        public double Influence { get => _influence; set => _influence = value; }
    }


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
        /// <param name="data"></param>
        /// <param name="influence"></param>
        /// <returns></returns>
        public static IDWPoint3d<T> Create<T>(Vec3d point, T data, double influence = 1.0)
        {
            return new IDWPoint3d<T>() {
                Point = point,
                Value = data,
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
            return new IDWPoint3d<T>() {
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
}
