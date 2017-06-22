using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;

using Rhino.Geometry;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMeshField
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        MeshPoint MeshPointAt(Vec3d point);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMeshField<T> : IMeshField, IField3d<T>, IDiscreteField<T>
    {
        /// <summary>
        /// Returns the interpolated value at the given <see cref="MeshPoint"/>.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T ValueAt(MeshPoint point);
    }
}
