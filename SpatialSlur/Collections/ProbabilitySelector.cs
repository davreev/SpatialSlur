
/*
 * Notes
 * 
 * TODO move to more appropriate namespace
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ProbabilitySelector
    {
        #region Static Members

        /// <summary>
        /// Returns the index of the first element larger than the given value.
        /// If all elements are smaller than the given value, returns the length of the array.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int BinarySearch(double[] source, double value)
        {
            int lo = 0;
            int hi = source.Length;
            int rng = hi - lo;

            while (rng > 1)
            {
                var mid = lo + (rng >> 1);

                if (value < source[mid])
                    hi = mid;
                else
                    lo = mid;

                rng = hi - lo;
            }

            return (value < source[lo]) ? lo : hi;
        }

        #endregion


        private double[] _weights;
        private Random _random;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="random"></param>
        public ProbabilitySelector(IEnumerable<double> weights, Random random)
        {
            _weights = weights.ToArray();
            _random = random;
            NormalizeWeights();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="random"></param>
        public ProbabilitySelector(double[] weights, Random random)
        {
            _weights = weights.ShallowCopy();
            _random = random;
            NormalizeWeights();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weights"></param>
        public void SetWeights(IEnumerable<double> weights)
        {
            _weights.Set(weights);
            NormalizeWeights();
        }


        /// <summary>
        /// 
        /// </summary>
        private void NormalizeWeights()
        {
            double sum = 0.0;
            
            for (int i = 0; i < _weights.Length; i++)
                _weights[i] = sum += _weights[i];

            double t = 1.0 / sum;

            for (int i = 0; i < _weights.Length; i++)
                _weights[i] *= t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Next()
        {
            return BinarySearch(_weights, _random.NextDouble());
        }
    }
}
