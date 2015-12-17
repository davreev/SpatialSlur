using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    ///
    /// </summary>
    public static class ColorUtil
    {
        // Various constants used in colour conversion
        private static readonly double _t0 = 16.0 / 116.0;
        private static readonly double _t1 = 1.0 / 2.4;
        private static readonly double _t2 = 1.0 / 3.0;
        private static double _invPi = 1.0 / Math.PI;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Color LerpSpectrum(IList<Color> colors, double t)
        {
            int last = colors.Count - 1;
            t *= last;
            int i = (int)Math.Floor(t);

            if (i < 0) return colors[0];
            else if (i >= last) return colors[last];

            return colors[i].LerpTo(colors[i + 1], t - i);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d LerpSpectrum(IList<Vec3d> colors, double t)
        {
            int last = colors.Count - 1;
            t *= last;
            int i = (int)Math.Floor(t);

            if (i < 0) return colors[0];
            else if (i >= last) return colors[last];

            return Vec3d.Lerp(colors[i], colors[i + 1], t - i);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public static Vec3d ToVec3d(this Color color)
        {
            return new Vec3d(color.R,color.G,color.B);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Color ToColor(Vec3d vector)
        {
            int r = SlurMath.Clamp((int)vector.x, 255);
            int g = SlurMath.Clamp((int)vector.y, 255);
            int b = SlurMath.Clamp((int)vector.z, 255);
            return Color.FromArgb(r, g, b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Color NormToColor(Vec3d vector)
        {
            int r = (int)(SlurMath.Saturate(vector.x) * 255.0);
            int g = (int)(SlurMath.Saturate(vector.y) * 255.0);
            int b = (int)(SlurMath.Saturate(vector.z) * 255.0);
            return Color.FromArgb(r, g, b);
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
            double l = color.x;
            double c = color.y;
            double h = color.z * (Math.PI / 180.0); // convert h to radians

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
            double l = color.x;
            double a = color.y;
            double b = color.z;

            double y = (l + 16.0) / 116.0;
            double x = a / 500.0 + y;
            double z = y - b / 200.0;

            double xxx = x * x * x;
            double yyy = y * y * y;
            double zzz = z * z * z;

            x = (xxx > 0.008856) ? xxx : (x - _t0) / 7.787;
            y = (yyy > 0.008856) ? yyy : (y - _t0) / 7.787;
            z = (zzz > 0.008856) ? zzz : (z - _t0) / 7.787;

            return new Vec3d(x * 95.047, y * 100.00, z * 108.883);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d XYZtoRGB(Vec3d color)
        {
            double x = color.x * 0.01;
            double y = color.y * 0.01;
            double z = color.z * 0.01;

            double r = x * 3.2406 + y * -1.5372 + z * -0.4986;
            double g = x * -0.9689 + y * 1.8758 + z * 0.0415;
            double b = x * 0.0557 + y * -0.2040 + z * 1.0570;

            r = (r > 0.0031308) ? 1.055 * Math.Pow(r, _t1) - 0.055 : r * 12.92;
            g = (g > 0.0031308) ? 1.055 * Math.Pow(g, _t1) - 0.055 : g * 12.92;
            b = (b > 0.0031308) ? 1.055 * Math.Pow(b, _t1) - 0.055 : b * 12.92;

            return new Vec3d(r * 255.0, g * 255.0, b * 255.0);
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
            double r = color.x / 255.0;
            double g = color.y / 255.0;
            double b = color.z / 255.0;

            r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
            g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
            b = (b > 0.04045) ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92;

            r *= 100.0;
            g *= 100.0;
            b *= 100.0;

            double x = r * 0.4124 + g * 0.3576 + b * 0.1805;
            double y = r * 0.2126 + g * 0.7152 + b * 0.0722;
            double z = r * 0.0193 + g * 0.1192 + b * 0.9505;

            return new Vec3d(x, y, z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d XYZtoLAB(Vec3d color)
        {
            double x = color.x / 95.047;
            double y = color.y * 0.01;
            double z = color.z / 108.883;

            x = (x > 0.008856) ? Math.Pow(x, _t2) : 7.787 * x + _t0;
            y = (y > 0.008856) ? Math.Pow(y, _t2) : 7.787 * y + _t0;
            z = (z > 0.008856) ? Math.Pow(z, _t2) : 7.787 * z + _t0;

            double l = (y > 0.008856) ? 116.0 * y - 16.0 : 903.3 * y;
            double a = 500.0 * (x - y);
            double b = 200.0 * (y - z);

            return new Vec3d(l, a, b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vec3d LABtoLCH(Vec3d color)
        {
            double l = color.x;
            double a = color.y;
            double b = color.z;

            double c = Math.Sqrt(a * a + b * b);
            double h = Math.Atan2(b, a);
            h = (h > 0.0) ? h * _invPi * 180.0 : 360.0 - Math.Abs(h) * _invPi * 180.0;

            return new Vec3d(l, c, h);
        }
    }
}
