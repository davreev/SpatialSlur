/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDiscreteField<T>
    {
        /// <summary>
        /// Returns a reference to the internal array of values.
        /// </summary>
        T[] Values { get; }


        /// <summary>
        /// Gets or sets the field value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; set; }


        /// <summary>
        /// Returns the number of values in the field.
        /// This may be less than the length of the value array depending on the implementation.
        /// </summary>
        int Count { get; }

        
        /// <summary>
        /// Returns a copy of this field.
        /// The value array of the returned field is a deep copy but other fields may be shallow depending on the implementation.
        /// </summary>
        /// <returns></returns>
        IDiscreteField<T> Duplicate(bool setValues);
    }
}
