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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        public CompareKd(int k)
        {
            if (k < 0) 
                throw new ArgumentException("The sort dimension cannot be less than 0");

            _k = k;
        }


        /// <summary>
        /// The dimension used for vector comparison.
        /// </summary>
        public int K
        {
            get { return _k; }
            set { _k = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(VecKd x, VecKd y)
        {
            return x[_k].CompareTo(y[_k]);
        }
    }
}
