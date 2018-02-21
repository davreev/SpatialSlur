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
    public interface IDiscreteField2d<T> : IDiscreteField<T>
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<Vec2d> Coordinates { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Vec2d CoordinateAt(int index);


        /// <summary>
        /// Returns a copy of this field.
        /// The value array of the returned field is a deep copy but other fields may be shallow depending on the implementation.
        /// </summary>
        /// <returns></returns>
        new IDiscreteField2d<T> Duplicate(bool setValues);
    }
}
