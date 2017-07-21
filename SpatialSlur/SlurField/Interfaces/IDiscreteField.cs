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
    public interface IDiscreteField<T> : IList<T>, IReadOnlyList<T>
    {
        /// <summary>
        /// Returns the internal array of field values.
        /// </summary>
        T[] Values { get; }

        
        /// <summary>
        /// Gets or sets the field value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new T this[int index] { get; set; }


        /// <summary>
        /// Returns the number of values in the field.
        /// This may be less than the length of the value array depending on the implementation.
        /// </summary>
        new int Count { get; }
      
        
        /// <summary>
        /// Returns a deep copy of this field.
        /// </summary>
        /// <returns></returns>
        IDiscreteField<T> Duplicate();
    }
}
