
/*
 * Notes 
 */

using System;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    internal static partial class Delegates
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal static class Position3d<T>
            where T : IPosition3d
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly Func<T, Vector3d> Get = v => v.Position;

            /// <summary>
            /// 
            /// </summary>
            public static readonly Action<T, Vector3d> Set = (v, p) => v.Position = p;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal static class Normal3d<T>
            where T : INormal3d
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly Func<T, Vector3d> Get = v => v.Normal;

            /// <summary>
            /// 
            /// </summary>
            public static readonly Action<T, Vector3d> Set = (v, n) => v.Normal = n;
        }
    }
}
