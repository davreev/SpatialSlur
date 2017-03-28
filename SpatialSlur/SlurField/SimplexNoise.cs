using System;
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
    public static class SimplexNoise
    {
        // Skew and unskew constants for 2 and 3 dimensions
        private static readonly double Skew2 = (Math.Sqrt(3.0) - 1.0) / 2.0;
        private static readonly double Unskew2 = (3.0 - Math.Sqrt(3.0)) / 6.0;
        private static readonly double Skew3 = 1.0 / 3.0;
        private static readonly double Unskew3 = 1.0 / 6.0;

         // permutation table
        private static readonly int[] _perm = new int[256];

        /*
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
        */


        // 2d gradient table
        private static readonly double[] _grad2 =
        {
          1, 1,
          -1, 1,
          1, -1,
          -1, -1,
          1, 0,
          -1, 0,
          0, 1,
          0, -1,
        };


        // 3d gradient table
        private static readonly double[] _grad3 =
        {
          1, 1, 0,
          -1, 1, 0,
          1, -1, 0,
          -1, -1, 0,
          1, 0, 1,
          -1, 0, 1,
          1, 0, -1,
          -1, 0, -1,
          0, 1, 1,
          0, -1, 1,
          0, 1, -1,
          0, -1, -1
        };


        /// <summary>
        ///
        /// </summary>
        static SimplexNoise()
        {
            SetPermutation(0);
        }


        /// <summary>
        /// Sets the permutation table.
        /// </summary>
        /// <param name="seed"></param>
        public static void SetPermutation(int seed)
        {
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
            // find unit grid cell containing point in skewed coordinates
            double t = (x + y) * Skew2;
            int i = (int)Math.Floor(x + t);
            int j = (int)Math.Floor(y + t);

            // offset and gradient index for first corner
            t = (i + j) * Unskew2;
            double x0 = x - i + t;
            double y0 = y - j + t;
            int g0 = ToIndex(i, j);

            // get offset and gradient index for the second corner (depends on which simplex the point lies in)
            double x1, y1;
            int g1;
            if (y0 < x0)
            {
                // point is in the 1st simplex
                x1 = x0 - 1.0 + Unskew2;
                y1 = y0 + Unskew2;
                g1 = ToIndex(i + 1, j);
            }
            else
            {
                // point is in the 2nd simplex
                x1 = x0 + Unskew2;
                y1 = y0 - 1.0 + Unskew2;
                g1 = ToIndex(i, j + 1);
            }

            // get offset and gradient index for third corner
            t = 2.0 * Unskew2 - 1.0;
            double x2 = x0 + t;
            double y2 = y0 + t;
            int g2 = ToIndex(i + 1, j + 1);

            // calculate noise contributions from each corner
            t = 0.5 - x0 * x0 - y0 * y0;
            double n0 = (t < 0.0) ? 0.0 : t * t * t * t * GradDot(g0, x0, y0);
      
            t = 0.5 - x1 * x1 - y1 * y1;
            double n1 = (t < 0.0) ? 0.0 : t * t * t * t * GradDot(g1, x1, y1);

            t = 0.5 - x2 * x2 - y2 * y2;
            double n2 = (t < 0.0) ? 0.0 : t * t * t * t * GradDot(g2, x2, y2);

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
        /// Returns the noise value at the given coordinates.
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
            int g0 = ToIndex(i, j, k);

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
                    g1 = ToIndex(i + 1, j, k);

                    x2 = x0 - 1.0 + t;
                    y2 = y0 - 1.0 + t;
                    z2 = z0 + t;
                    g2 = ToIndex(i + 1, j + 1, k);
                }
                else if (z0 < x0)
                {
                    // point is in the 2nd simplex
                    x1 = x0 - 1.0 + Unskew3;
                    y1 = y0 + Unskew3;
                    z1 = z0 + Unskew3;
                    g1 = ToIndex(i + 1, j, k);

                    x2 = x0 - 1.0 + t;
                    y2 = y0 + t;
                    z2 = z0 - 1.0 + t;
                    g2 = ToIndex(i + 1, j, k + 1);
                }
                else
                {
                    // point is in the 3rd simplex
                    x1 = x0 + Unskew3;
                    y1 = y0 + Unskew3;
                    z1 = z0 - 1.0 + Unskew3;
                    g1 = ToIndex(i, j, k + 1);

                    x2 = x0 - 1.0 + t;
                    y2 = y0 + t;
                    z2 = z0 - 1.0 + t;
                    g2 = ToIndex(i + 1, j, k + 1);
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
                    g1 = ToIndex(i, j, k + 1);

                    x2 = x0 + t;
                    y2 = y0 - 1.0 + t;
                    z2 = z0 - 1.0 + t;
                    g2 = ToIndex(i, j + 1, k + 1);
                }
                else if (x0 < z0)
                {
                    // point is in the 5th simplex
                    x1 = x0 + Unskew3;
                    y1 = y0 - 1.0 + Unskew3;
                    z1 = z0 + Unskew3;
                    g1 = ToIndex(i, j + 1, k);

                    x2 = x0 + t;
                    y2 = y0 - 1.0 + t;
                    z2 = z0 - 1.0 + t;
                    g2 = ToIndex(i, j + 1, k + 1);
                }
                else
                {
                    // point is in the 6th simplex
                    x1 = x0 + Unskew3;
                    y1 = y0 - 1.0 + Unskew3;
                    z1 = z0 + Unskew3;
                    g1 = ToIndex(i, j + 1, k);

                    x2 = x0 - 1.0 + t;
                    y2 = y0 - 1.0 + t;
                    z2 = z0 + t;
                    g2 = ToIndex(i + 1, j + 1, k);
                }
            }

            // get offset for last corner in unskewed coordinates
            t = 3.0 * Unskew3 - 1.0;
            double x3 = x0 + t;
            double y3 = y0 + t;
            double z3 = z0 + t;
            int g3 = ToIndex(i + 1, j + 1, k + 1);

            // calculate noise contributions from each corner
            t = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
            double n0 = (t < 0.0) ? 0.0 : t * t * t * t * GradDot(g0, x0, y0, z0);

            t = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
            double n1 = (t < 0.0) ? 0.0 : t * t * t * t * GradDot(g1, x1, y1, z1);
         
            t = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
            double n2 = (t < 0.0) ? 0.0 : t * t * t * t * GradDot(g2, x2, y2, z2);
         
            t = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
            double n3 = (t < 0.0) ? 0.0 : t * t * t * t * GradDot(g3, x3, y3, z3);

            // add contributions from each corner to get final value between -1 and 1
            return 32.0 * (n0 + n1 + n2 + n3);
        }


        /// <summary>
        /// Returns the gradient table index for the given coordinates.
        /// </summary>
        private static int ToIndex(int i, int j)
        {
            return (_perm[(i + _perm[j & 255]) & 255] & 7) << 1;
        }


        /// <summary>
        /// Returns the gradient table index for the given coordinates.
        /// </summary>
        private static int ToIndex(int i, int j, int k)
        {
            return _perm[(i + _perm[(j + _perm[k & 255]) & 255]) & 255] % 12 * 3;
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GradDot(int index, double x, double y)
        {
            return _grad2[index] * x + _grad2[index + 1] * y;
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GradDot(int index, double x, double y, double z)
        {
            return _grad3[index] * x + _grad3[index + 1] * y + _grad3[index + 2] * z;
        }
    }
}
