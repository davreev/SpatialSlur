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
        /// By default the table is set to seed 0.
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
            // get whole and franctional componenets
            int i, j;
            x = SlurMath.Fract(x, out i);
            y = SlurMath.Fract(y, out j);

            // calculate noise contributions from each corner
            double n00 = Grad(i, j, x, y);
            double n01 = Grad(i, j + 1, x, y - 1.0);
            double n10 = Grad(i + 1, j, x - 1.0, y);
            double n11 = Grad(i + 1, j + 1, x - 1.0, y - 1.0);

            // eased values for x and y
            x = SlurMath.SmootherStep(x);
            y = SlurMath.SmootherStep(y);

            // bilinear interpolation
            return SlurMath.Lerp(
                SlurMath.Lerp(n00,n10,x),
                SlurMath.Lerp(n01,n11,x),
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
        /// returns noise value at given coordinates
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
            double n000 = Grad(i, j, k, x, y, z);
            double n001 = Grad(i, j, k + 1, x, y, z - 1.0);
            double n010 = Grad(i, j + 1, k, x, y - 1.0, z);
            double n011 = Grad(i, j + 1, k + 1, x, y - 1.0, z - 1.0);
            double n100 = Grad(i + 1, j, k, x - 1.0, y, z);
            double n101 = Grad(i + 1, j, k + 1, x - 1.0, y, z - 1.0);
            double n110 = Grad(i + 1, j + 1, k, x - 1.0, y - 1.0, z);
            double n111 = Grad(i + 1, j + 1, k + 1, x - 1.0, y - 1.0, z - 1.0);
            
            // eased values for each dimension
            x = SlurMath.SmootherStep(x);
            y = SlurMath.SmootherStep(y);
            z = SlurMath.SmootherStep(z);

            // trilinear interpolation
            return SlurMath.Lerp(
                SlurMath.Lerp(
                    SlurMath.Lerp(n000, n100, x),
                    SlurMath.Lerp(n001, n101, x),
                    y),
                SlurMath.Lerp(
                    SlurMath.Lerp(n010, n110, x),
                    SlurMath.Lerp(n011, n111, x),
                    y),
                z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double Grad(int i, int j, double x, double y)
        {
            var g = _grad3[PermAt(i + PermAt(j)) & 7];
            return g[0] * x + g[1] * y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static double Grad(int i, int j, int k, double x, double y, double z)
        {
            var g = _grad3[PermAt(i + PermAt(j + PermAt(k))) % 12];
            return g[0] * x + g[1] * y + g[2] * z;
        }


        /// <summary>
        /// returns permutation at wrapped index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int PermAt(int index)
        {
            return _perm[index & 255];
        }
    }
}
