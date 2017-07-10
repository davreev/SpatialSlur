using System;
using SpatialSlur.SlurCore;

/*
 * Notes
 * 
 * Region search methods may return the contents of the same bin multiple times since different indices (i, j) may hash to the same index.
 * Similarly, region insertion methods may add the given item to the same bin multiple times.
 * 
 * References
 * http://www.beosil.com/download/CollisionDetectionHashing_VMV03.pdf
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Spatial hash for broad phase collision detection between dynamic objects.
    /// </summary>
    public class SpatialHash2d<T> : SpatialMap2d<T>
    {
        private const int P1 = 73856093; // used in hash function
        private const int P2 = 19349663; // used in hash function
        private double _scale, _scaleInv; // scale of implicit grid


        /// <summary>
        ///
        /// </summary>
        public SpatialHash2d(int binCount, double binScale)
        {
            Init(binCount);
            BinScale = binScale;
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
                    throw new ArgumentOutOfRangeException("The value must be larger than zero.");

                _scale = value;
                _scaleInv = 1.0 / _scale;
                Clear();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override (int, int) Discretize(Vec2d point)
        {
            return (
                (int)Math.Floor(point.X * _scaleInv),
                (int)Math.Floor(point.Y * _scaleInv));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected sealed override int ToIndex(int i, int j)
        {
            return SlurMath.Mod2(i * P1 ^ j * P2, BinCount);
        }
    }
}
