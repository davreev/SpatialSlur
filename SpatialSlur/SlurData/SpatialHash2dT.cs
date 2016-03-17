using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 * Search methods may return the contents of the same bin multiple times since different points may hash to the same index.
 * Similarly, insertion methods may add the given item to the same bin multiple times.
 * 
 * References
 * http://www.beosil.com/download/CollisionDetectionHashing_VMV03.pdf
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Spatial hash for broad phase collision detection between dynamic objects.
    /// </summary>
    public class SpatialHash2d<T>:Spatial2d<T>
    {
        // prime numbers used in hash function
        private const int P1 = 73856093; 
        private const int P2 = 19349663;

        private double _scale, _scaleInv; // scale of implicit grid
  

        /// <summary>
        ///
        /// </summary>
        public SpatialHash2d(int binCount, double scale)
            :base(binCount)
        {
            BinScale = scale;
        }


        /// <summary>
        /// Gets or sets the scale of the implicit grid used to discretize coordinates.
        /// Note that setting the scale clears the map.
        /// </summary>
        public double BinScale
        {
            get { return _scale; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentOutOfRangeException("The scale must be larger than zero");

                _scale = value;
                _scaleInv = 1.0 / _scale;
                Clear();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        protected override void Discretize(Vec2d point, out int i, out int j)
        {
            i = (int)Math.Floor(point.x * _scaleInv);
            j = (int)Math.Floor(point.y * _scaleInv);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected override int ToIndex(int i, int j)
        {
            return SlurMath.Mod2(i * P1 ^ j * P2, BinCount);
        }
    }
}
