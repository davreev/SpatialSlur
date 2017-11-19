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
        private const double _offsetY = 10.0;
        private const double _offsetZ = 20.0;

        private const double _delta = 1.0e-8;
        private const double _d2Inv = 1.0 / (_delta * 2.0);
        
        // permutation table
        //private static readonly int[] _perm = new int[256];


        // permutation tables
        private static readonly int[] _perm = new int[512]; // double size to remove need for wrapping
        private static readonly int[] _permMod12 = new int[512];


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
            const int n = 256;

            // fill in 1st half
            for (int i = 0; i < n; i++)
                _perm[i] = i;

            // shuffle
            _perm.Shuffle(seed, 0, n);

            // copy to 2nd half
            for (int i = 0; i < n; i++)
            {
                _perm[i + n] = _perm[i];
                _permMod12[i] = _permMod12[i + n] = _perm[i] % 12;
            }
        }


        #region 2d operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double ValueAt(Vec2d point)
        {
            return ValueAt(point.X, point.Y);
        }


        /// <summary>
        /// Returns the noise value at the given coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double ValueAt(double x, double y)
        {
            // separate whole and fractional components
            x = SlurMath.Fract(x, out int i);
            y = SlurMath.Fract(y, out int j);

            // wrap to perm table
            i &= 255;
            j &= 255;

            // calculate noise contributions from each corner
            double n00 = GradDot(ToIndex(i, j), x, y);
            double n10 = GradDot(ToIndex(i + 1, j), x - 1.0, y);
            double n01 = GradDot(ToIndex(i, j + 1), x, y - 1.0);
            double n11 = GradDot(ToIndex(i + 1, j + 1), x - 1.0, y - 1.0);

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
        /// Returns the gradient table index for the given coordinates.
        /// </summary>
        private static int ToIndex(int i, int j)
        {
            return _perm[(i + _perm[j])] & 7;
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GradDot(int index, double x, double y)
        {
            index <<= 1;
            return _grad2[index] * x + _grad2[index + 1] * y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec2d VectorAt(Vec2d point)
        {
            return VectorAt(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vec2d VectorAt(double x, double y)
        {
            return new Vec2d(
                GetX(x, y),
                GetY(x, y)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GetX(double x, double y)
        {
            return ValueAt(x, y);
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GetY(double x, double y)
        {
            return ValueAt(x + _offsetY, y + _offsetY);
        }


        /// <summary>
        /// Returns the gradient of noise values.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec2d GradientAt(Vec2d point)
        {
            return GradientAt(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vec2d GradientAt(double x, double y)
        {
            return new Vec2d(
                (ValueAt(x + _delta, y) - ValueAt(x - _delta, y)) * _d2Inv,
                (ValueAt(x, y + _delta) - ValueAt(x, y - _delta)) * _d2Inv
                );
        }


        /// <summary>
        /// http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph2007-curlnoise.pdf
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec2d CurlAt(Vec2d point)
        {
            return CurlAt(point.X, point.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vec2d CurlAt(double x, double y)
        {
            return new Vec2d(
                (ValueAt(x + _delta, y) - ValueAt(x - _delta, y)) * _d2Inv,
                (ValueAt(x, y + _delta) - ValueAt(x, y - _delta)) * _d2Inv
                );
        }

        #endregion


        #region 3d operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double ValueAt(Vec3d point)
        {
            return ValueAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// Returns the noise value at the given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double ValueAt(double x, double y, double z)
        {
            // separate whole and fractional components
            x = SlurMath.Fract(x, out int i);
            y = SlurMath.Fract(y, out int j);
            z = SlurMath.Fract(z, out int k);

            // wrap to perm table
            i &= 255;
            j &= 255;
            k &= 255;

            // calculate noise contributions from each corner
            double n000 = GradDot(ToIndex(i, j, k), x, y, z);
            double n100 = GradDot(ToIndex(i + 1, j, k), x - 1.0, y, z);
            double n010 = GradDot(ToIndex(i, j + 1, k), x, y - 1.0, z);
            double n110 = GradDot(ToIndex(i + 1, j + 1, k), x - 1.0, y - 1.0, z);

            double n001 = GradDot(ToIndex(i, j, k + 1), x, y, z - 1.0);
            double n101 = GradDot(ToIndex(i + 1, j, k + 1), x - 1.0, y, z - 1.0);
            double n011 = GradDot(ToIndex(i, j + 1, k + 1), x, y - 1.0, z - 1.0);
            double n111 = GradDot(ToIndex(i + 1, j + 1, k + 1), x - 1.0, y - 1.0, z - 1.0);

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
        private static int ToIndex(int i, int j, int k)
        {
            return _permMod12[i + _perm[j + _perm[k]]];
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GradDot(int index, double x, double y, double z)
        {
            index *= 3;
            return _grad3[index] * x + _grad3[index + 1] * y + _grad3[index + 2] * z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec3d VectorAt(Vec3d point)
        {
            return VectorAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vec3d VectorAt(double x, double y, double z)
        {
            return new Vec3d(
                GetX(x, y, z),
                GetY(x, y, z),
                GetZ(x, y, z)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GetX(double x, double y, double z)
        {
            return ValueAt(x, y, z);
        }

        
        /// <summary>
        /// 
        /// </summary>
        private static double GetY(double x, double y, double z)
        {
            return ValueAt(x + _offsetY, y + _offsetY, z + _offsetY);
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GetZ(double x, double y, double z)
        {
            return ValueAt(x + _offsetZ, y + _offsetZ, z + _offsetZ);
        }


        /// <summary>
        /// Returns the gradient of noise values.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec3d GradientAt(Vec3d point)
        {
            return GradientAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vec3d GradientAt(double x, double y, double z)
        {
            return new Vec3d(
              (ValueAt(x + _delta, y, z) - ValueAt(x - _delta, y, z)) * _d2Inv,
              (ValueAt(x, y + _delta, z) - ValueAt(x, y - _delta, z)) * _d2Inv,
              (ValueAt(x, y, z + _delta) - ValueAt(x, y, z - _delta)) * _d2Inv
              );
        }


        /// <summary>
        /// http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph2007-curlnoise.pdf
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vec3d CurlAt(Vec3d point)
        {
            return CurlAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vec3d CurlAt(double x, double y, double z)
        {
            var dz_y = GetZ(x, y + _delta, z) - GetZ(x, y - _delta, z);
            var dy_z = GetY(x, y, z + _delta) - GetY(x, y, z - _delta);

            var dx_z = GetX(x, y, z + _delta) - GetX(x, y, z - _delta);
            var dz_x = GetZ(x + _delta, y, z) - GetZ(x - _delta, y, z);

            var dy_x = GetY(x + _delta, y, z) - GetY(x - _delta, y, z);
            var dx_y = GetX(x, y + _delta, z) - GetX(x, y - _delta, z);

            return new Vec3d(
               (dz_y - dy_z) * _d2Inv,
               (dx_z - dz_x) * _d2Inv,
               (dy_x - dx_y) * _d2Inv
               );
        }

        #endregion
    }
}
