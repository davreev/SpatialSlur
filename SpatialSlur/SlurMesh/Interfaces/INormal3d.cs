using System;

using SpatialSlur.SlurCore;

/*
 * Notes 
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public interface INormal3d
    {
        /// <summary></summary>
        Vec3d Normal { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class INormal3d<T>
        where T : INormal3d
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Func<T, Vec3d> Get = v => v.Normal;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Action<T, Vec3d> Set = (v, n) => v.Normal = n;
    }
}
