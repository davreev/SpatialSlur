using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 * 
 * References
 * http://webstaff.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class PerlinNoise
    {
        // permutation table
        private static readonly int[] _perm = new int[256];

        // 2d gradient table
        private static readonly double[][] _grad2 = 
        {
          new double[]{1, 1},
          new double[]{-1, 1},
          new double[]{1, -1},
          new double[]{-1, -1},
          new double[]{1, 0},
          new double[]{-1, 0},
          new double[]{0, 1},
          new double[]{0, -1},
        };

        // 3d gradient table
        private static readonly double[][] _grad3 = 
        {
          new double[]{1, 1, 0},
          new double[]{-1, 1, 0},
          new double[]{1, -1, 0},
          new double[]{-1, -1, 0},
          new double[]{1, 0, 1},
          new double[]{-1, 0, 1},
          new double[]{1, 0, -1},
          new double[]{-1, 0, -1},
          new double[]{0, 1, 1},
          new double[]{0, -1, 1},
          new double[]{0, 1, -1},
          new double[]{0, -1, -1}
        };
      
     
        /// <summary>
        /// 
        /// </summary>
        static PerlinNoise()
        {
            SetPermutation(0);
        }


        /// <summary>
        /// Sets the permutation table.
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
        /// Returns the noise value at the given coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Evaluate(double x, double y)
        {
            // get whole and franctional componenets
            int i, j;
            x = SlurMath.Fract(x, out i);
            y = SlurMath.Fract(y, out j);

            // calculate noise contributions from each corner
            double n00 = Dot(_grad2[ToIndex(i, j)], x, y);
            double n10 = Dot(_grad2[ToIndex(i + 1, j)], x - 1.0, y);
            double n01 = Dot(_grad2[ToIndex(i, j + 1)], x, y - 1.0);
            double n11 = Dot(_grad2[ToIndex(i + 1, j + 1)], x - 1.0, y - 1.0);

            // eased values for x and y
            x = SlurMath.HermiteC2(x);
            y = SlurMath.HermiteC2(y);

            // bilinear interpolation
            return SlurMath.Lerp(
              SlurMath.Lerp(n00, n10, x),
              SlurMath.Lerp(n01, n11, x),
              y);
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
        /// Returns the noise value at the given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double Evaluate(double x, double y, double z)
        {
            // get whole and franctional componenets
            int i, j, k;
            x = SlurMath.Fract(x, out i);
            y = SlurMath.Fract(y, out j);
            z = SlurMath.Fract(z, out k);

            // calculate noise contributions from each corner
            double n000 = Dot(_grad3[ToIndex(i, j, k)], x, y, z);
            double n100 = Dot(_grad3[ToIndex(i + 1, j, k)], x - 1.0, y, z);
            double n010 = Dot(_grad3[ToIndex(i, j + 1, k)], x, y - 1.0, z);
            double n110 = Dot(_grad3[ToIndex(i + 1, j + 1, k)], x - 1.0, y - 1.0, z);

            double n001 = Dot(_grad3[ToIndex(i, j, k + 1)], x, y, z - 1.0);
            double n101 = Dot(_grad3[ToIndex(i + 1, j, k + 1)], x - 1.0, y, z - 1.0);
            double n011 = Dot(_grad3[ToIndex(i, j + 1, k + 1)], x, y - 1.0, z - 1.0);
            double n111 = Dot(_grad3[ToIndex(i + 1, j + 1, k + 1)], x - 1.0, y - 1.0, z - 1.0);

            // eased values for each dimension
            x = SlurMath.HermiteC2(x);
            y = SlurMath.HermiteC2(y);
            z = SlurMath.HermiteC2(z);

            // trilinear interpolation
            return SlurMath.Lerp(
              SlurMath.Lerp(
              SlurMath.Lerp(n000, n100, x),
              SlurMath.Lerp(n010, n110, x),
              y),
              SlurMath.Lerp(
              SlurMath.Lerp(n001, n101, x),
              SlurMath.Lerp(n011, n111, x),
              y),
              z);
        }


        /// <summary>
        /// Returns the gradient table index for the given coordinates.
        /// </summary>
        private static int ToIndex(int i, int j)
        {
            return _perm[(i + _perm[j & 255]) & 255] & 7;
        }


        /// <summary>
        /// Returns the gradient table index for the given coordinates.
        /// </summary>
        private static int ToIndex(int i, int j, int k)
        {
            return _perm[(i + _perm[(j + _perm[k & 255]) & 255]) & 255] % 12;
        }


        /// <summary>
        /// 
        /// </summary>
        private static double Dot(double[] vec, double x, double y)
        {
            return vec[0] * x + vec[1] * y;
        }


        /// <summary>
        /// 
        /// </summary>
        private static double Dot(double[] vec, double x, double y, double z)
        {
            return vec[0] * x + vec[1] * y + vec[2] * z;
        }
    }
}
