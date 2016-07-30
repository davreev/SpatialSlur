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
    public class FieldPoint2d
    {
        private readonly int[] _corners = new int[4]; // indices of cell corners
        private readonly double[] _weights = new double[4]; // weights of each corner


        /// <summary>
        /// 
        /// </summary>
        public FieldPoint2d()
        {
            Unset();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public FieldPoint2d(double u, double v)
            : this()
        {
            SetWeights(u, v);
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
        public void SetWeights(double u, double v)
        {
            double u1 = 1.0 - u;
            double v1 = 1.0 - v;
        
            _weights[0] = u1 * v1;
            _weights[1] = u * v1;
            _weights[2] = u1 * v;
            _weights[3] = u * v;
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
