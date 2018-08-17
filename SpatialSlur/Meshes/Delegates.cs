
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
            public static readonly Func<T, Vector3d> Get = t => t.Position;

            /// <summary>
            /// 
            /// </summary>
            public static readonly Action<T, Vector3d> Set = (t, p) => t.Position = p;
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
            public static readonly Func<T, Vector3d> Get = t => t.Normal;

            /// <summary>
            /// 
            /// </summary>
            public static readonly Action<T, Vector3d> Set = (t, n) => t.Normal = n;
        }
    }
}
