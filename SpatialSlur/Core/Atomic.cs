using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpatialSlur
{
    /// <summary>
    /// Extends atomic operations from Interlocked class to SpatialSlur primitive types 
    /// </summary>
    public static class Atomic
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public static void Exchange(ref Vector2i v0, Vector2i v1)
        {
            Interlocked.Exchange(ref v0.X, v1.X);
            Interlocked.Exchange(ref v0.Y, v1.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public static void Exchange(ref Vector2d v0, Vector2d v1)
        {
            Interlocked.Exchange(ref v0.X, v1.X);
            Interlocked.Exchange(ref v0.Y, v1.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public static void Exchange(ref Vector3i v0, Vector3i v1)
        {
            Interlocked.Exchange(ref v0.X, v1.X);
            Interlocked.Exchange(ref v0.Y, v1.Y);
            Interlocked.Exchange(ref v0.Z, v1.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public static void Exchange(ref Vector3f v0, Vector3f v1)
        {
            Interlocked.Exchange(ref v0.X, v1.X);
            Interlocked.Exchange(ref v0.Y, v1.Y);
            Interlocked.Exchange(ref v0.Z, v1.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public static void Exchange(ref Vector3d v0, Vector3d v1)
        {
            Interlocked.Exchange(ref v0.X, v1.X);
            Interlocked.Exchange(ref v0.Y, v1.Y);
            Interlocked.Exchange(ref v0.Z, v1.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public static void Exchange(ref Vector4d v0, Vector4d v1)
        {
            Interlocked.Exchange(ref v0.X, v1.X);
            Interlocked.Exchange(ref v0.Y, v1.Y);
            Interlocked.Exchange(ref v0.Z, v1.Z);
            Interlocked.Exchange(ref v0.W, v1.W);
        }
    }
}
