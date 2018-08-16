
/*
 * Notes
 */

using System;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// Barycentric representation of position in a two-dimensional grid.
    /// </summary>
    [Serializable]
    public struct GridPoint2d
    {
        /// <summary></summary>
        public double Weight0;
        /// <summary></summary>
        public int Index0;

        /// <summary></summary>
        public double Weight1;
        /// <summary></summary>
        public int Index1;

        /// <summary></summary>
        public double Weight2;
        /// <summary></summary>
        public int Index2;

        /// <summary></summary>
        public double Weight3;
        /// <summary></summary>
        public int Index3;


        /// <summary>
        /// Assumes components of the given point are between 0 and 1 inclusive.
        /// </summary>
        /// <param name="point"></param>
        public void SetWeights(Vector2d point)
        {
            (var u0, var v0) = point;

            double u1 = 1.0 - u0;
            double v1 = 1.0 - v0;

            Weight0 = u1 * v1;
            Weight1 = u0 * v1;
            Weight2 = u1 * v0;
            Weight3 = u0 * v0;
        }
    }
}
