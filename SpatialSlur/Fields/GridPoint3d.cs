
/*
 * Notes
 */

using System;
using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// Barycentric representation of position in a three-dimensional grid.
    /// </summary>
    [Serializable]
    public struct GridPoint3d
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

        /// <summary></summary>
        public double Weight4;
        /// <summary></summary>
        public int Index4;

        /// <summary></summary>
        public double Weight5;
        /// <summary></summary>
        public int Index5;

        /// <summary></summary>
        public double Weight6;
        /// <summary></summary>
        public int Index6;

        /// <summary></summary>
        public double Weight7;
        /// <summary></summary>
        public int Index7;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void SetWeights(Vector3d point)
        {
            (var u0, var v0,  var w0) = point;

            double u1 = 1.0 - u0;
            double v1 = 1.0 - v0;
            double w1 = 1.0 - w0;

            Weight0 = u1 * v1 * w1;
            Weight1 = u0 * v1 * w1;
            Weight2 = u1 * v0 * w1;
            Weight3 = u0 * v0 * w1;

            Weight4 = u1 * v1 * w0;
            Weight5 = u0 * v1 * w0;
            Weight6 = u1 * v0 * w0;
            Weight7 = u0 * v0 * w0;
        }
    }
}
