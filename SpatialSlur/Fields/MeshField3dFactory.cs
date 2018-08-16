
/*
 * Notes
 */

#if USING_RHINO

using System;
using SpatialSlur.Meshes;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MeshField3dFactory<T>
        where T : struct
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public abstract MeshField3d<T> Create(HeMesh3d mesh);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract MeshField3d<T> Create(MeshField3d other);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public MeshField3d<T> CreateCopy(MeshField3d<T> other)
        {
            var result = Create(other);
            result.Set(other);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="other"></param>
        /// <param name="converter"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshField3d<T> CreateCopy<U>(MeshField3d<U> other, Func<U, T> converter, bool parallel = false)
            where U : struct
        {
            var result = Create(other);
            other.Convert(converter, result, parallel);
            return result;
        }
    }
}

#endif