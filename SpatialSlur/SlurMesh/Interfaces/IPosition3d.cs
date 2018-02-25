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
    public interface IPosition3d
    {
        /// <summary></summary>
        Vec3d Position { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class IPosition3d<T>
        where T : IPosition3d
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Func<T, Vec3d> Get = v => v.Position;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Action<T, Vec3d> Set = (v, p) => v.Position = p; 
    }
}
