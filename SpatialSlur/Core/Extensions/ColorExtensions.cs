
/*
 * Notes
 */

using System.Drawing;


namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="other"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Color LerpTo(this Color c, Color other, double t)
        {
            int a = (int)(c.A + (other.A - c.A) * t);
            int r = (int)(c.R + (other.R - c.R) * t);
            int g = (int)(c.G + (other.G - c.G) * t);
            int b = (int)(c.B + (other.B - c.B) * t);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
