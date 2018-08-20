/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISampledField<T>
    {
        /// <summary>
        /// Returns the field's array of sample values.
        /// </summary>
        ArrayView<T> Values { get; }


        /// <summary>
        /// Returns the number of samples in the field.
        /// </summary>
        int Count { get; }


        /// <summary>
        /// Returns a copy of this field.
        /// Note that that value array of the returned field is a deep copy but other fields may be shallow depending on the implementation.
        /// </summary>
        /// <returns></returns>
        ISampledField<T> Duplicate(bool setValues);
    }
}
