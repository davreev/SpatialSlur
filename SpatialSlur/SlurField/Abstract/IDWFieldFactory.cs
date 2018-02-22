using System;
using System.Linq;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class IDWFieldFactory<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDWField3d<T> Create(double power, double epsilon = SlurMath.ZeroTolerance);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IDWField3d<T> CreateCopy(IDWField3d<T> other)
        {
            var result = Create(other.Power, other.Epsilon);
            result.DefaultValue = other.DefaultValue;
            result.DefaultWeight = other.DefaultWeight;
            result.Points.AddRange(other.Points.Select(p => IDWPoint3d.Create(p)));
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IDWField3d<T> CreateCopy<U>(IDWField3d<U> other, Func<U, T> converter)
        {
            var result = Create(other.Power, other.Epsilon);
            result.DefaultValue = converter(other.DefaultValue);
            result.DefaultWeight = other.DefaultWeight;
            result.Points.AddRange(other.Points.Select(p => IDWPoint3d.Create(p, converter)));
            return result;
        }
    }
}
