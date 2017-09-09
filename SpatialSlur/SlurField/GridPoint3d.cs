
using System;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Intermediate representation of a 3d grid query which stores the indices of the 8 nearest values along with weights used for interpolation.
    /// </summary>
    [Serializable]
    public class GridPoint3d
    {
        private readonly double[] _weights;
        private readonly int[] _corners;


        /// <summary>
        /// 
        /// </summary>
        public GridPoint3d()
        {
            _weights = new double[8];
            _corners = new int[8];
            Unset();
        }


        /// <summary>
        /// 
        /// </summary>
        public double[] Weights
        {
            get { return _weights; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int[] Corners
        {
            get { return _corners; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnset
        {
            get { return _corners[0] == -1; }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        public void SetWeights(double u, double v, double w)
        {
            double u1 = 1.0 - u;
            double v1 = 1.0 - v;
            double w1 = 1.0 - w;

            _weights[0] = u1 * v1 * w1;
            _weights[1] = u * v1 * w1;
            _weights[2] = u1 * v * w1;
            _weights[3] = u * v * w1;

            _weights[4] = u1 * v1 * w;
            _weights[5] = u * v1 * w;
            _weights[6] = u1 * v * w;
            _weights[7] = u * v * w;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Unset()
        {
            _corners[0] = -1;
        }
    }
}
