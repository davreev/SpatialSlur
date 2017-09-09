using System;
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
    public interface IDiscreteField<TValue>
    {
        /// <summary>
        /// Returns a reference to the internal array of values.
        /// </summary>
        TValue[] Values { get; }


        /// <summary>
        /// Gets or sets the field value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        TValue this[int index] { get; set; }


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
        IDiscreteField<TValue> Duplicate(bool copyValues);
    }
}
