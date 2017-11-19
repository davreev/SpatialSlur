#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using Rhino.Geometry;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurField
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
        MeshPoint ClosestMeshPoint(Vec3d point);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMeshField<T> : IMeshField
    {
        /// <summary>
        /// Returns the interpolated value at the given MeshPoint.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T ValueAt(MeshPoint point);
    }
}

#endif