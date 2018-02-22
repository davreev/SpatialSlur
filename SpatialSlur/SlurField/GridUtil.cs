using System;
using System.Runtime.CompilerServices;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Utility class for related constants and static methods.
    /// </summary>
    public static class GridUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Func<int, int, int> SelectWrapFunction(WrapMode mode)
        {
            switch (mode)
            {
                case WrapMode.Clamp:
                    return Clamp;
                case WrapMode.Repeat:
                    return Repeat;
                case WrapMode.Mirror:
                    return Mirror;
            }

            throw new NotSupportedException();
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static int Wrap(int index, int range, WrapMode mode)
        {
            switch (mode)
            {
                case WrapMode.Clamp:
                    return Clamp(index, range);
                case WrapMode.Repeat:
                    return Repeat(index, range);
                case WrapMode.Mirror:
                    return Mirror(index, range);
            }

            throw new NotSupportedException();
        }
        */
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static int Wrap(int index, int range, WrapMode mode)
        {
            // conditional is used instead of switch for inline optimization
            return mode == WrapMode.Repeat ?
                Repeat(index, range) : mode == WrapMode.Mirror ?
                Mirror(index, range) : Clamp(index, range);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static int Clamp(int i, int n)
        {
            return (i < 0) ? 0 : (i < n) ? i : n - 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static int Repeat(int i, int n)
        {
            i %= n;
            return (i < 0) ? i + n : i;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static int Mirror(int i, int n)
        {
            var n2 = n + n;
            i %= n2;
            if (i < 0) i += n2;
            return (i < n) ? i : n2 - i - 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nx"></param>
        /// <returns></returns>
        public static int FlattenIndices(int x, int y, int nx)
        {
            return x + y * nx;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="nx"></param>
        /// <param name="nxy"></param>
        /// <returns></returns>
        public static int FlattenIndices(int x, int y, int z, int nx, int nxy)
        {
            return x + y * nx + z * nxy;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nx"></param>
        /// <returns></returns>
        public static (int, int) ExpandIndex(int index, int nx)
        {
            int y = index / nx;
            return (index - y * nx, y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nx"></param>
        /// <param name="nxy"></param>
        /// <returns></returns>
        public static (int, int, int) ExpandIndex(int index, int nx, int nxy)
        {
            int z = index / nxy;
            index -= z * nxy;
            int y = index / nx;
            return (index - y * nx, y, z);
        }
    }
}
