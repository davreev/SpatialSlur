using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes 
 */
 
namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDiscreteField3d<TValue> : IDiscreteField<TValue>
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
        new IDiscreteField3d<TValue> Duplicate(bool copyValues);
    }
}
