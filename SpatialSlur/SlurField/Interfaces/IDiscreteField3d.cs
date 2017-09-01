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
    }
}
