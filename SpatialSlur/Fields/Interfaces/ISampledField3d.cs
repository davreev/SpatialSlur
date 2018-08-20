
/*
 * Notes
 */

using System.Collections.Generic;

using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISampledField3d<T> : ISampledField<T>, IField3d<T>
    {
        /// <summary>
        /// Returns all sample points used by this field.
        /// </summary>
        IEnumerable<Vector3d> Points { get; }


        /// <summary>
        /// Returns the sample point at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Vector3d PointAt(int index);


        /// <summary>
        /// Returns a copy of this field.
        /// Note that that sample value array of the returned field is a deep copy but other fields may be shallow depending on the implementation.
        /// </summary>
        /// <returns></returns>
        new ISampledField3d<T> Duplicate(bool setValues);
    }
}
