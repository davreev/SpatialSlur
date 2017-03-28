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
    public interface IField<T>
    {
        /// <summary>
        /// Returns the array of field values.
        /// </summary>
        T[] Values { get; }

        /// <summary>
        /// Returns the number of values in the field.
        /// This may be less than the length of the value array depending on the implementation.
        /// </summary>
        int Count { get; }
    }
}
