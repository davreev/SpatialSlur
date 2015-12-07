using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace SpatialSlur.SlurCore
{
    public static class DisplayUtil
    {
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
    }
}
