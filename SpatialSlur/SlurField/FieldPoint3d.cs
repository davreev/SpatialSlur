using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public class FieldPoint3d
    {
        private readonly int[] _corners = new int[8]; // indices of cell corners
        private readonly double[] _weights = new double[8]; // weights of each corner


        /// <summary>
        /// 
        /// </summary>
        public FieldPoint3d()
        {
            Unset();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        public FieldPoint3d(double u, double v, double w)
            : this()
        {
            SetWeights(u, v, w);
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
        public double[] Weights
        {
            get { return _weights; }
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
        internal void SetWeights(double u, double v, double w)
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
