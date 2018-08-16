
/*
 * Notes
 * 
 * References
 * http://webstaff.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
 */

using System;
using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class Noise
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Perlin
        {
            private const double _offsetY = 10.0;
            private const double _offsetZ = 20.0;

            private const double _delta = 1.0e-8;
            private const double _d2Inv = 1.0 / (_delta * 2.0);

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
            static Perlin()
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
            public static double ValueAt(Vector2d point)
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
            public static Vector2d VectorAt(Vector2d point)
            {
                return VectorAt(point.X, point.Y);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Vector2d VectorAt(double x, double y)
            {
                return new Vector2d(
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
            public static Vector2d GradientAt(Vector2d point)
            {
                return GradientAt(point.X, point.Y);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Vector2d GradientAt(double x, double y)
            {
                return new Vector2d(
                    (ValueAt(x + _delta, y) - ValueAt(x - _delta, y)) * _d2Inv,
                    (ValueAt(x, y + _delta) - ValueAt(x, y - _delta)) * _d2Inv
                    );
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public static Vector2d CurlAt(Vector2d point)
            {
                return CurlAt(point.X, point.Y);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Vector2d CurlAt(double x, double y)
            {
                // impl ref
                // http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph2007-curlnoise.pdf

                return new Vector2d(
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
            public static double ValueAt(Vector3d point)
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
            public static Vector3d VectorAt(Vector3d point)
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
            public static Vector3d VectorAt(double x, double y, double z)
            {
                return new Vector3d(
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
            public static Vector3d GradientAt(Vector3d point)
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
            public static Vector3d GradientAt(double x, double y, double z)
            {
                return new Vector3d(
                  (ValueAt(x + _delta, y, z) - ValueAt(x - _delta, y, z)) * _d2Inv,
                  (ValueAt(x, y + _delta, z) - ValueAt(x, y - _delta, z)) * _d2Inv,
                  (ValueAt(x, y, z + _delta) - ValueAt(x, y, z - _delta)) * _d2Inv
                  );
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public static Vector3d CurlAt(Vector3d point)
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
            public static Vector3d CurlAt(double x, double y, double z)
            {
                // impl ref
                // http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph2007-curlnoise.pdf

                var dz_y = GetZ(x, y + _delta, z) - GetZ(x, y - _delta, z);
                var dy_z = GetY(x, y, z + _delta) - GetY(x, y, z - _delta);

                var dx_z = GetX(x, y, z + _delta) - GetX(x, y, z - _delta);
                var dz_x = GetZ(x + _delta, y, z) - GetZ(x - _delta, y, z);

                var dy_x = GetY(x + _delta, y, z) - GetY(x - _delta, y, z);
                var dx_y = GetX(x, y + _delta, z) - GetX(x, y - _delta, z);

                return new Vector3d(
                   (dz_y - dy_z) * _d2Inv,
                   (dx_z - dz_x) * _d2Inv,
                   (dy_x - dx_y) * _d2Inv
                   );
            }

            #endregion
        }

        
        /// <summary>
        /// 
        /// </summary>
        public static class Simplex
        {
            private const double _offsetY = 10.0;
            private const double _offsetZ = 20.0;

            private const double _delta = 1.0e-8;
            private const double _d2Inv = 1.0 / (_delta * 2.0);

            // skew constants for 2 dimensions
            private static readonly double _skew2 = (Math.Sqrt(3.0) - 1.0) / 2.0;
            private static readonly double _skew2Inv = (3.0 - Math.Sqrt(3.0)) / 6.0;

            // skew constants for 3 dimensions
            private const double _skew3 = 1.0 / 3.0;
            private const double _skew3Inv = 1.0 / 6.0;

            // permutation tables
            private static readonly int[] _perm = new int[512]; // double size to remove need for wrapping
            private static readonly int[] _permMod12 = new int[512]; // double size to remove need for wrapping

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
            static Simplex()
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
            public static double ValueAt(Vector2d point)
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
                // find unit grid cell containing point in skewed coordinates
                double s = (x + y) * _skew2;
                int i = (int)Math.Floor(x + s);
                int j = (int)Math.Floor(y + s);

                // unskew and make relative to origin
                double t = (i + j) * _skew2Inv;
                double x0 = x - i + t;
                double y0 = y - j + t;

                // collect offsets for remaining corners of simplex
                int i1, j1;

                if (x0 > y0)
                {
                    // point is in first simplex
                    i1 = 1;
                    j1 = 0;
                }
                else
                {
                    // point is in second simplex
                    i1 = 0;
                    j1 = 1;
                }

                double a = 2.0 * _skew2Inv - 1.0;

                var x1 = x0 - i1 + _skew2Inv;
                var y1 = y0 - j1 + _skew2Inv;

                var x2 = x0 + a;
                var y2 = y0 + a;

                // wrap to perm table
                i &= 255;
                j &= 255;

                int g0 = ToIndex(i, j);
                int g1 = ToIndex(i + i1, j + j1);
                int g2 = ToIndex(i + 1, j + 1);

                var n0 = GetNoise(g0, x0, y0);
                var n1 = GetNoise(g1, x1, y1);
                var n2 = GetNoise(g2, x2, y2);

                // add contributions from each corner to get final value between -1 and 1
                return 70.0 * (n0 + n1 + n2);
            }


            /// <summary>
            /// 
            /// </summary>
            private static double GetNoise(int index, double x, double y)
            {
                var t = 0.5 - x * x - y * y;

                if (t < 0.0)
                    return 0.0;

                t *= t;
                return t * t * GradDot(index, x, y);
            }


            /// <summary>
            /// Returns the gradient table index for the given coordinates.
            /// </summary>
            private static int ToIndex(int i, int j)
            {
                return _perm[i + _perm[j]] & 7;
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
            /// Returns a vector composed of offset noise values.
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public static Vector2d VectorAt(Vector2d point)
            {
                return VectorAt(point.X, point.Y);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Vector2d VectorAt(double x, double y)
            {
                return new Vector2d(
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
            public static Vector2d GradientAt(Vector2d point)
            {
                return GradientAt(point.X, point.Y);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Vector2d GradientAt(double x, double y)
            {
                return new Vector2d(
                     (ValueAt(x + _delta, y) - ValueAt(x - _delta, y)) * _d2Inv,
                     (ValueAt(x, y + _delta) - ValueAt(x, y - _delta)) * _d2Inv
                     );
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public static Vector2d CurlAt(Vector2d point)
            {
                return CurlAt(point.X, point.Y);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Vector2d CurlAt(double x, double y)
            {
                // impl ref
                // http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph2007-curlnoise.pdf

                return new Vector2d(
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
            public static double ValueAt(Vector3d point)
            {
                return ValueAt(point.X, point.Y, point.Z);
            }


            /// <summary>
            /// Returns the noise value at the given coordinates.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            public static double ValueAt(double x, double y, double z)
            {
                // find unit grid cell containing point in skewed coordinates
                double s = (x + y + z) * _skew3;
                int i = (int)Math.Floor(x + s);
                int j = (int)Math.Floor(y + s);
                int k = (int)Math.Floor(z + s);

                // unskew and make relative to origin
                double t = (i + j + k) * _skew3Inv;
                double x0 = x - i + t;
                double y0 = y - j + t;
                double z0 = z - k + t;

                // collect offsets for remaining corners of simplex
                int i1, j1, k1;
                int i2, j2, k2;

                if (x0 >= y0)
                {
                    if (y0 >= z0)
                    {
                        // point is in the 1st simplex
                        i1 = 1; j1 = 0; k1 = 0;
                        i2 = 1; j2 = 1; k2 = 0;
                    }
                    else if (x0 >= z0)
                    {
                        // point is in the 2nd simplex
                        i1 = 1; j1 = 0; k1 = 0;
                        i2 = 1; j2 = 0; k2 = 1;
                    }
                    else
                    {
                        // point is in the 3rd simplex
                        i1 = 0; j1 = 0; k1 = 1;
                        i2 = 1; j2 = 0; k2 = 1;
                    }
                }
                else
                {
                    if (y0 < z0)
                    {
                        // point is in the 4th simplex
                        i1 = 0; j1 = 0; k1 = 1;
                        i2 = 0; j2 = 1; k2 = 1;
                    }
                    else if (x0 < z0)
                    {
                        // point is in the 5th simplex
                        i1 = 0; j1 = 1; k1 = 0;
                        i2 = 0; j2 = 1; k2 = 1;
                    }
                    else
                    {
                        // point is in the 6th simplex
                        i1 = 0; j1 = 1; k1 = 0;
                        i2 = 1; j2 = 1; k2 = 0;
                    }
                }

                const double a0 = 2.0 * _skew3Inv;
                const double a1 = 3.0 * _skew3Inv - 1.0;

                var x1 = x0 - i1 + _skew3Inv;
                var y1 = y0 - j1 + _skew3Inv;
                var z1 = z0 - k1 + _skew3Inv;

                var x2 = x0 - i2 + a0;
                var y2 = y0 - j2 + a0;
                var z2 = z0 - k2 + a0;

                var x3 = x0 + a1;
                var y3 = y0 + a1;
                var z3 = z0 + a1;

                i &= 255;
                j &= 255;
                k &= 255;

                int g0 = ToIndex(i, j, k);
                var g1 = ToIndex(i + i1, j + j1, k + k1);
                var g2 = ToIndex(i + i2, j + j2, k + k2);
                var g3 = ToIndex(i + 1, j + 1, k + 1);

                // calculate noise contributions from each corner
                var n0 = GetNoise(g0, x0, y0, z0);
                var n1 = GetNoise(g1, x1, y1, z1);
                var n2 = GetNoise(g2, x2, y2, z2);
                var n3 = GetNoise(g3, x3, y3, z3);

                // add contributions from each corner to get final value between -1 and 1
                return 32.0 * (n0 + n1 + n2 + n3);
            }


            /// <summary>
            /// 
            /// </summary>
            private static double GetNoise(int index, double x, double y, double z)
            {
                var t = 0.5 - x * x - y * y - z * z;

                if (t < 0.0)
                    return 0.0;

                t *= t;
                return t * t * GradDot(index, x, y, z);
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
            /// Returns a vector composed of offset noise values.
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public static Vector3d VectorAt(Vector3d point)
            {
                return VectorAt(point.X, point.Y, point.Z);
            }


            /// <summary>
            /// Returns a vector composed of offset noise values.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Vector3d VectorAt(double x, double y, double z)
            {
                return new Vector3d(
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
            public static Vector3d GradientAt(Vector3d point)
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
            public static Vector3d GradientAt(double x, double y, double z)
            {
                return new Vector3d(
                  (ValueAt(x + _delta, y, z) - ValueAt(x - _delta, y, z)) * _d2Inv,
                  (ValueAt(x, y + _delta, z) - ValueAt(x, y - _delta, z)) * _d2Inv,
                  (ValueAt(x, y, z + _delta) - ValueAt(x, y, z - _delta)) * _d2Inv
                  );
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public static Vector3d CurlAt(Vector3d point)
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
            public static Vector3d CurlAt(double x, double y, double z)
            {
                // impl ref
                // http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph2007-curlnoise.pdf

                var dz_y = GetZ(x, y + _delta, z) - GetZ(x, y - _delta, z);
                var dy_z = GetY(x, y, z + _delta) - GetY(x, y, z - _delta);

                var dx_z = GetX(x, y, z + _delta) - GetX(x, y, z - _delta);
                var dz_x = GetZ(x + _delta, y, z) - GetZ(x - _delta, y, z);

                var dy_x = GetY(x + _delta, y, z) - GetY(x - _delta, y, z);
                var dx_y = GetX(x, y + _delta, z) - GetX(x, y - _delta, z);

                return new Vector3d(
                    (dz_y - dy_z) * _d2Inv,
                    (dx_z - dz_x) * _d2Inv,
                    (dy_x - dx_y) * _d2Inv
                    );
            }

            #endregion
        }
    }
}
