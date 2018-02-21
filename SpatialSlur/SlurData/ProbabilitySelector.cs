using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;

using static SpatialSlur.SlurData.ArrayMath;

/*
 * Notes
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ProbabilitySelector
    {
        #region Static

        /// <summary>
        /// Returns the index of the first element larger than the given value.
        /// If all elements are smaller than the given value, returns the length of the array.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static int BinarySearch(double[] values, double x)
        {
            int lo = 0;
            int hi = values.Length;
            int rng = hi - lo;

            while (rng > 1)
            {
                var mid = lo + (rng >> 1);

                if (x < values[mid])
                    hi = mid;
                else
                    lo = mid;

                rng = hi - lo;
            }

            return (x < values[lo]) ? lo : hi;
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
            // cumulative sum
            double sum = 0.0;
            for (int i = 0; i < _weights.Length; i++)
            {
                sum += _weights[i];
                _weights[i] = sum;
            }

            // normalize
            Scale(_weights, 1.0 / sum, _weights);
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
