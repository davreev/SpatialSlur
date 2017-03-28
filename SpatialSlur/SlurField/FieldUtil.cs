using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class FieldUtil
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Func<int, int, int>[] _wrapFuncs =
        {
            Clamp,
            Repeat,
            MirrorRepeat
        };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="wrapMode"></param>
        /// <returns></returns>
        public static Func<int, int, int> SelectWrapFunc(FieldWrapMode wrapMode)
        {
            return _wrapFuncs[(int)wrapMode];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Clamp(int i, int n)
        {
            return (i < 0) ? 0 : (i < n) ? i : n - 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Repeat(int i, int n)
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
        public static int MirrorRepeat(int i, int n)
        {
            i %= n + n;
            if (i < 0) i += n + n;
            return (i < n) ? i : n + n - i - 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nx"></param>
        /// <returns></returns>
        public static int FlattenIndex(int x, int y, int nx)
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
        public static int FlattenIndex(int x, int y, int z, int nx, int nxy)
        {
            return x + y * nx + z * nxy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nx"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void ExpandIndex(int index, int nx, out int x, out int y)
        {
            y = index / nx;
            x = index - y * nx;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nx"></param>
        /// <param name="nxy"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void ExpandIndex(int index, int nx, int nxy, out int x, out int y, out int z)
        {
            z = index / nxy;
            x = index - z * nxy; // store remainder in x temporarily
            y = x / nx;
            x -= y * nx;
        }
    }
}
