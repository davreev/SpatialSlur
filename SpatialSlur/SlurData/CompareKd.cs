using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurData
{
    public class CompareKd : IComparer<VecKd>
    {
        private int _k;
        private double _epsilon;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        public CompareKd(int k, double epsilon)
        {
            K = k;
            Epsilon = epsilon;
        }


        /// <summary>
        /// The dimension used for vector comparison.
        /// </summary>
        public int K
        {
            get { return _k; }
            set 
            {
                if (value < 0)
                    throw new ArgumentException("The sort dimension cannot be less than 0.");

                _k = value; 
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Epsilon
        {
            get { return _epsilon; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Epsilon must be greater than zero.");

                _epsilon = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(VecKd x, VecKd y)
        {
            double d = x[_k] - y[_k];
            return (Math.Abs(d) < _epsilon) ? 0 : Math.Sign(d);
        }
    }
}
