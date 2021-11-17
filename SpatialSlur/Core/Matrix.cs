
/*
 * Notes
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Matrix
    {
        /// <summary>
        /// Returns the entries of the covariance matrix in row-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="mean"></param>
        public static void GetCovariance(IEnumerable<double[]> vectors, double[] result, out double[] mean)
        {
            mean = new double[vectors.First().Length];
            vectors.Mean(mean);
            GetCovariance(vectors, mean, result);
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in row-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <param name="result"></param>
        public static void GetCovariance(IEnumerable<double[]> vectors, double[] mean, double[] result)
        {
            int dim = mean.Length;
            int n = 0;

            Array.Clear(result, 0, dim * dim);

            // calculate uppper triangular values
            foreach (double[] v in vectors)
            {
                for (int i = 0; i < dim; i++)
                {
                    double di = v[i] - mean[i];

                    for (int j = i; j < dim; j++)
                        result[i * dim + j] += di * (v[j] - mean[j]);
                }

                n++;
            }

            var t = 1.0 / n;

            // average and set lower triangular values
            for (int i = 0; i < dim; i++)
            {
                result[i * dim + i] *= t; // diagonal entry

                // lower triangular
                for (int j = i + 1; j < dim; j++)
                    result[j * dim + i] = result[i * dim + j] *= t;
            }
        }
    }
}
