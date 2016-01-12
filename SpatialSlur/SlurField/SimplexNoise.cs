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
    public static class SimplexNoise
    {
        // Skew and unskew constants for 2 and 3 dimensions
        private static readonly double Skew2 = (Math.Sqrt(3.0) - 1.0) / 2.0;
        private static readonly double Unskew2 = (3.0 - Math.Sqrt(3.0)) / 6.0;
        private static readonly double Skew3 = 1.0 / 3.0;
        private static readonly double Unskew3 = 1.0 / 6.0;


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
        /// static constructor initializes default permutation table
        /// </summary>
        static SimplexNoise()
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
            // find unit grid cell containing point in skewed coordinates
            double t = (x + y) * Skew2;
            int i = (int)Math.Floor(x + t);
            int j = (int)Math.Floor(y + t);

            // offset and gradient index for first corner
            t = (i + j) * Unskew2;
            double x0 = x - i + t;
            double y0 = y - j + t;
            int g0 = ToGradIndex(i, j);

            // get offset and gradient index for the second corner (depends on which simplex the point lies in)
            double x1, y1;
            int g1;
            if(y0 < x0)
            {
                // point is in the 1st simplex
                x1 = x0 - 1.0 + Unskew2;
                y1 = y0 + Unskew2;
                g1 = ToGradIndex(i + 1, j);
            }
            else
            {
                // point is in the 2nd simplex
                x1 = x0 + Unskew2;
                y1 = y0 - 1.0 + Unskew2;
                g1 = ToGradIndex(i, j + 1);
            }

            // get offset and gradient index for third corner
            t = 2.0 * Unskew2 - 1.0;
            double x2 = x0 + t;
            double y2 = y0 + t;
            int g2 = ToGradIndex(i + 1, j + 1);

            double n0, n1, n2;

            // calculate noise contributions from each corner
            t = 0.5 - x0 * x0 - y0 * y0;
            if (t < 0.0) 
                n0 = 0.0;
            else
            {
                t *= t;
                n0 = t * t * Dot(_grad2[g0], x0, y0);
            }

            t = 0.5 - x1 * x1 - y1 * y1;
            if (t < 0.0) 
                n1 = 0.0;
            else
            {
                t *= t;
                n1 = t * t * Dot(_grad2[g1], x1, y1);
            }

            t = 0.5 - x2 * x2 - y2 * y2;
            if (t < 0.0) 
                n2 = 0.0;
            else
            {
                t *= t;
                n2 = t * t * Dot(_grad2[g2], x2, y2);
            }
         
            // add contributions from each corner to get final value between -1 and 1
            return 70.0 * (n0 + n1 + n2);
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
            // find unit grid cell containing point in skewed coordinates
            double s = (x + y + z) * Skew3;
            int i = (int)Math.Floor(x + s);
            int j = (int)Math.Floor(y + s);
            int k = (int)Math.Floor(z + s);

            // get offset and gradient index for first corner
            double t = (i + j + k) * Unskew3;
            double x0 = x - i + t;
            double y0 = y - j + t;
            double z0 = z - k + t;
            int g0 = ToGradIndex(i, j, k);

            // get gradient table indices and offsets for second and third corner (depends on which simplex the point lies in)
            double x1, y1, z1, x2, y2, z2;
            int g1, g2;
            t = 2.0 * Unskew3;

            if (y0 < x0)
            {
                if (z0 < y0)
                {
                    // point is in the 1st simplex
                    x1 = x0 - 1.0 + Unskew3;
                    y1 = y0 + Unskew3;
                    z1 = z0 + Unskew3;
                    g1 = ToGradIndex(i + 1, j, k);

                    x2 = x0 - 1.0 + t;
                    y2 = y0 - 1.0 + t;
                    z2 = z0 + t;
                    g2 = ToGradIndex(i + 1, j + 1, k);
                }
                else if (z0 < x0)
                {
                    // point is in the 2nd simplex
                    x1 = x0 - 1.0 + Unskew3;
                    y1 = y0 + Unskew3;
                    z1 = z0 + Unskew3;
                    g1 = ToGradIndex(i + 1, j, k);

                    x2 = x0 - 1.0 + t;
                    y2 = y0 + t;
                    z2 = z0 - 1.0 + t;
                    g2 = ToGradIndex(i + 1, j, k + 1);
                }
                else
                {
                    // point is in the 3rd simplex
                    x1 = x0 + Unskew3;
                    y1 = y0 + Unskew3;
                    z1 = z0 - 1.0 + Unskew3;
                    g1 = ToGradIndex(i, j, k + 1);

                    x2 = x0 - 1.0 + t;
                    y2 = y0 + t;
                    z2 = z0 - 1.0 + t;
                    g2 = ToGradIndex(i + 1, j, k + 1);
                }
            }
            else
            {
                if (y0 < z0)
                {
                    // point is in the 4th simplex
                    x1 = x0 + Unskew3;
                    y1 = y0 + Unskew3;
                    z1 = z0 - 1.0 + Unskew3;
                    g1 = ToGradIndex(i, j, k + 1);

                    x2 = x0 + t;
                    y2 = y0 - 1.0 + t;
                    z2 = z0 - 1.0 + t;
                    g2 = ToGradIndex(i, j + 1, k + 1);
                }
                else if (x0 < z0)
                {
                    // point is in the 5th simplex
                    x1 = x0 + Unskew3;
                    y1 = y0 - 1.0 + Unskew3;
                    z1 = z0 + Unskew3;
                    g1 = ToGradIndex(i, j + 1, k);

                    x2 = x0 + t;
                    y2 = y0 - 1.0 + t;
                    z2 = z0 - 1.0 + t;
                    g2 = ToGradIndex(i, j + 1, k + 1);
                }
                else
                {
                    // point is in the 6th simplex
                    x1 = x0 + Unskew3;
                    y1 = y0 - 1.0 + Unskew3;
                    z1 = z0 + Unskew3;
                    g1 = ToGradIndex(i, j + 1, k);

                    x2 = x0 - 1.0 + t;
                    y2 = y0 - 1.0 + t;
                    z2 = z0 + t;
                    g2 = ToGradIndex(i + 1, j + 1, k);
                }
            }

            // get offset for last corner in unskewed coordinates
            t = 3.0 * Unskew3 - 1.0;
            double x3 = x0 + t;
            double y3 = y0 + t;
            double z3 = z0 + t;
            int g3 = ToGradIndex(i + 1, j + 1, k + 1);

            // calculate noise contributions from each corner
            double n0, n1, n2, n3;

            t = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
            if (t < 0) 
                n0 = 0.0;
            else
            {
                t *= t;
                n0 = t * t * Dot(_grad3[g0], x0, y0, z0);
            }

            t = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
            if (t < 0) 
                n1 = 0.0;
            else
            {
                t *= t;
                n1 = t * t * Dot(_grad3[g1], x1, y1, z1);
            }

            t = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
            if (t < 0) 
                n2 = 0.0;
            else
            {
                t *= t;
                n2 = t * t * Dot(_grad3[g2], x2, y2, z2);
            }

            t = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
            if (t < 0) 
                n3 = 0.0;
            else
            {
                t *= t;
                n3 = t * t * Dot(_grad3[g3], x3, y3, z3);
            }

            // add contributions from each corner to get final value between -1 and 1
            return 32.0 * (n0 + n1 + n2 + n3);
        }


        /// <summary>
        /// returns gradient table index at given coordinates
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private static int ToGradIndex(int i, int j)
        {
            return GetPermAt(i + GetPermAt(j)) & 7;
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
            return GetPermAt(i + GetPermAt(j + GetPermAt(k))) % 12;
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
