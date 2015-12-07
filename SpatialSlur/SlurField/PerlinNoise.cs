using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// http://webstaff.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
    /// </summary>
    public static class PerlinNoise
    {
        // permutation table
        private static readonly int[] _perm = new int[256];

        // gradient table
        private static readonly double[][] _grad = 
        {
          new double[]{1,1,0},
          new double[]{-1,1,0},
          new double[]{1,-1,0},
          new double[]{-1,-1,0},
          new double[]{1,0,1},
          new double[]{-1,0,1},
          new double[]{1,0,-1},
          new double[]{-1,0,-1},
          new double[]{0,1,1},
          new double[]{0,-1,1},
          new double[]{0,1,-1},
          new double[]{0,-1,-1}
        };

     
        /// <summary>
        /// static constructor initializes default permutation table
        /// </summary>
        static PerlinNoise()
        {
            SetPermutation(0);
        }


        /// <summary>
        /// shuffles the permutation table
        /// the same seed value will always produce the same table
        /// </summary>
        /// <param name="seed"></param>
        public static void SetPermutation(int seed)
        {
            // assign permutation table and shuffle
            for (int i = 0; i < 256; i++)
                _perm[i] = i;

            _perm.Shuffle(seed);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double Evaluate(Vec2d point)
        {
            return Evaluate(point.x, point.y);
        }


        /// <summary>
        /// returns noise value at given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Evaluate(double x, double y)
        {
            // get whole portion of the coordinates
            int i = (int)Math.Floor(x);
            int j = (int)Math.Floor(y);

            // get fractional portion of coordinates
            x -= i;
            y -= j;

            // calculate noise contributions from each corner
            double n00 = Dot(_grad[ToGradIndex(i, j)], x, y);
            double n01 = Dot(_grad[ToGradIndex(i, j + 1)], x, y - 1);
            double n10 = Dot(_grad[ToGradIndex(i + 1, j)], x - 1, y);
            double n11 = Dot(_grad[ToGradIndex(i + 1, j + 1)], x - 1, y - 1);

            // Compute eased value for x, y, and z respectively
            x = SlurMath.SmootherStep(x);
            y = SlurMath.SmootherStep(y);

            // Interpolate along x
            double nx0 = SlurMath.Lerp(n00, n10, x);
            double nx1 = SlurMath.Lerp(n01, n11, x);

            // Interpolate along y
            return SlurMath.Lerp(nx0, nx1, y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double Evaluate(Vec3d point)
        {
            return Evaluate(point.x, point.y, point.z);
        }


        /// <summary>
        /// returns noise value at given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double Evaluate(double x, double y, double z)
        {
            // get whole portion of the coordinates
            int i = (int)Math.Floor(x);
            int j = (int)Math.Floor(y);
            int k = (int)Math.Floor(z);

            // get fractional portion of coordinates
            x -= i;
            y -= j;
            z -= k;

            // calculate noise contributions from each corner
            double n000 = Dot(_grad[ToGradIndex(i, j, k)], x, y, z);
            double n001 = Dot(_grad[ToGradIndex(i, j, k + 1)], x, y, z - 1);
            double n010 = Dot(_grad[ToGradIndex(i, j + 1, k)], x, y - 1, z);
            double n011 = Dot(_grad[ToGradIndex(i, j + 1, k + 1)], x, y - 1, z - 1);
            double n100 = Dot(_grad[ToGradIndex(i + 1, j, k)], x - 1, y, z);
            double n101 = Dot(_grad[ToGradIndex(i + 1, j, k + 1)], x - 1, y, z - 1);
            double n110 = Dot(_grad[ToGradIndex(i + 1, j + 1, k)], x - 1, y - 1, z);
            double n111 = Dot(_grad[ToGradIndex(i + 1, j + 1, k + 1)], x - 1, y - 1, z - 1);
            
            // Compute eased value for each dimension
            x = SlurMath.SmootherStep(x);
            y = SlurMath.SmootherStep(y);
            z = SlurMath.SmootherStep(z);

            // Interpolate along x
            double nx00 = SlurMath.Lerp(n000, n100, x);
            double nx01 = SlurMath.Lerp(n001, n101, x);
            double nx10 = SlurMath.Lerp(n010, n110, x);
            double nx11 = SlurMath.Lerp(n011, n111, x);

            // Interpolate along y
            double ny0 = SlurMath.Lerp(nx00, nx10, y);
            double ny1 = SlurMath.Lerp(nx01, nx11, y);

            // Interpolate along z
            return SlurMath.Lerp(ny0, ny1, z);
        }


        /// <summary>
        /// returns gradient table index at given coordinates
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private static int ToGradIndex(int i, int j)
        {
            return  GetPermAt(i +  GetPermAt(j)) % 12;
        }


        /// <summary>
        /// returns gradient table index at given coordinates
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private static int ToGradIndex(int i, int j, int k)
        {
           return GetPermAt(i +  GetPermAt(j +  GetPermAt(k))) % 12;
        }


        /// <summary>
        /// returns permutation at wrapped index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int GetPermAt(int index)
        {
            return _perm[index & 255]; // bitwise wrapping
        }


        /// <summary>
        /// custom dot product for compatibility with gradient table
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double Dot(double[] xy, double x, double y)
        {
            return xy[0] * x + xy[1] * y;
        }


        /// <summary>
        /// custom dot product for compatibility with gradient table
        /// </summary>
        /// <param name="xyz"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static double Dot(double[] xyz, double x, double y, double z)
        {
            return xyz[0] * x + xyz[1] * y + xyz[2] * z;
        }

    }
}
