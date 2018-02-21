using System;
using System.Drawing;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class ColorUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public static Vec3d ToVec3d(this Color color)
        {
            return new Vec3d(color.R,color.G,color.B);
        }


        /// <summary>
        /// Components of the returned vector are between 0 and 1.
        /// </summary>
        /// <param name="color"></param>
        public static Vec3d ToVec3dNorm(this Color color)
        {
            const double t = 1.0 / 255.0;
            return new Vec3d(color.R * t, color.G * t, color.B * t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Color ToColor(this Vec3d vector)
        {
            int r = SlurMath.Clamp((int)vector.X, 255);
            int g = SlurMath.Clamp((int)vector.Y, 255);
            int b = SlurMath.Clamp((int)vector.Z, 255);
            return Color.FromArgb(r, g, b);
        }


        /// <summary>
        /// Assumes components of the given vector are between 0 and 1.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Color ToColorNorm(Vec3d vector)
        {
            return ToColor(vector * 256.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d LCHtoRGB(Vec3d color)
        {
            color = LCHtoLAB(color);
            color = LABtoXYZ(color);
            return XYZtoRGB(color);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d LABtoRGB(Vec3d color)
        {
            color = LABtoXYZ(color);
            return XYZtoRGB(color);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d LCHtoLAB(Vec3d color)
        {
            double l = color.X;
            double c = color.Y;
            double h = SlurMath.ToRadians(color.Z);

            double a = Math.Cos(h) * c;
            double b = Math.Sin(h) * c;

            return new Vec3d(l, a, b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d LABtoXYZ(Vec3d color)
        {
            const double t0 = 1.0 / 116.0;
            const double t1 = 16.0 / 116.0;
            const double t2 = 1.0 / 7.787;

            double y = (color.X + 16.0) * t0;
            double x = color.Y * 0.002 + y;
            double z = y - color.Z * 0.005;

            double Map(double c)
            {
                double ccc = c * c * c;
                return (ccc > 0.008856) ? ccc : (x - t1) * t2;
            }

            return new Vec3d(Map(x) * 95.047, Map(y) * 100.00, Map(z) * 108.883);
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d XYZtoRGB(Vec3d color)
        {
            const double t = 1.0 / 2.4;

            double x = color.X * 0.01;
            double y = color.Y * 0.01;
            double z = color.Z * 0.01;

            double r = x * 3.2406 + y * -1.5372 + z * -0.4986;
            double g = x * -0.9689 + y * 1.8758 + z * 0.0415;
            double b = x * 0.0557 + y * -0.2040 + z * 1.0570;

            double Map(double c)
            {
                return ((c > 0.0031308) ? 1.055 * Math.Pow(c, t) - 0.055 : c * 12.92) * 255.0;
            }

            return new Vec3d(Map(r), Map(g), Map(b));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d RGBtoLCH(Vec3d color)
        {
            color = RGBtoXYZ(color);
            color = XYZtoLAB(color);
            return LABtoLCH(color);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d RGBtoLAB(Vec3d color)
        {
            color = RGBtoXYZ(color);
            return XYZtoLAB(color);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d RGBtoXYZ(Vec3d color)
        {
            const double t0 = 1.0 / 255.0;
            const double t1 = 1.0 / 1.055;
            const double t2 = 1.0 / 12.92;

            double r = Map(color.X);
            double g = Map(color.Y);
            double b = Map(color.Z);

            double Map(double c)
            {
                c *= t0;
                return ((c > 0.04045) ? Math.Pow((c + 0.055) * t1, 2.4) : c * t2) * 100.0;
            }

            return new Vec3d(
                r * 0.4124 + g * 0.3576 + b * 0.1805,
                r * 0.2126 + g * 0.7152 + b * 0.0722,
                r * 0.0193 + g * 0.1192 + b * 0.9505
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d XYZtoLAB(Vec3d color)
        {
            const double t0 = 1.0 / 95.047;
            const double t1 = 1.0 / 108.883;
            const double t2 = 1.0 / 3.0;
            const double t3 = 16.0 / 116.0;

            double x = Map(color.X * t0);
            double y = Map(color.Y * 0.01);
            double z = Map(color.Z * t1);

            double Map(double c)
            {
                return (c > 0.008856) ? Math.Pow(c, t2) : 7.787 * c + t3;
            }

            return new Vec3d(
                (y > 0.008856) ? 116.0 * y - 16.0 : 903.3 * y,
                500.0 * (x - y),
                200.0 * (y - z)
                );
        }
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d LABtoLCH(Vec3d color)
        {
            double l = color.X;
            double a = color.Y;
            double b = color.Z;

            double c = Math.Sqrt(a * a + b * b);
            double h = Math.Atan2(b, a);
            h = (h > 0.0) ? SlurMath.ToDegrees(h) : SlurMath.ToDegrees(360.0 - Math.Abs(h));

            return new Vec3d(l, c, h);
        }
    }
}
