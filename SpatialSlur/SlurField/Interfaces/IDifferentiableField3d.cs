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
    /// <typeparam name="T"></typeparam>
    public interface IDifferentiableField3d<T>
    {
        /// <summary>
        /// Returns the the gradient at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T GradientAt(Vec3d point);
    }
}
