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
    public interface IDiscreteField3d<T> : IDiscreteField<T>
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<Vec3d> Coordinates { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Vec3d CoordinateAt(int index);


        /// <summary>
        /// Returns a copy of this field.
        /// The value array of the returned field is a deep copy but other fields may be shallow depending on the implementation.
        /// </summary>
        /// <returns></returns>
        new IDiscreteField3d<T> Duplicate(bool setValues);
    }
}
