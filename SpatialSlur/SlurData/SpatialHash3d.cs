using System;
using SpatialSlur.SlurCore;

/*
 * Notes
 * 
 * Region search methods may return the contents of the same bin multiple times since different indices (i, j, k) may hash to the same index.
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
    [Serializable]
    public class SpatialHash3d<T> : SpatialMap3d<T>
    {
        private const int P1 = 73856093; // used in hash function
        private const int P2 = 19349663; // used in hash function
        private const int P3 = 83492791; // used in hash function
        private double _scale, _scaleInv; // scale of implicit grid


        /// <summary>
        /// 
        /// </summary>
        public SpatialHash3d(int binCount, double binScale)
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
        protected sealed override (int, int, int) Discretize(Vec3d point)
        {
            return (
                (int)Math.Floor(point.X * _scaleInv),
                (int)Math.Floor(point.Y * _scaleInv),
                (int)Math.Floor(point.Z * _scaleInv));
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        internal override void Discretize(Vec3d point, out int i, out int j, out int k)
        {
            i = (int)Math.Floor(point.x * _scaleInv);
            j = (int)Math.Floor(point.y * _scaleInv);
            k = (int)Math.Floor(point.z * _scaleInv);
        }
        */


        /// <summary>
        /// http://cybertron.cg.tu-berlin.de/eitz/pdf/2007_hsh.pdf
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        protected sealed override int ToIndex(int i, int j, int k)
        {
            return SlurMath.Mod2(i * P1 ^ j * P2 ^ k * P3, BinCount);
        }
    }
}
