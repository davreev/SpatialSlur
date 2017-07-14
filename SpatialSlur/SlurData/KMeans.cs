using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    public class KMeans
    {
        private double[][] _points;
        private int[] _nearestClusters;

        private double[][] _clusterCenters;
        private int[] _clusterSizes;
        private int[] _clusterIndices;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="clusterCount"></param>
        /// <param name="seed"></param>
        public KMeans(double[][] points, int clusterCount, int seed = 1)
        {
            _points = points;
            _nearestClusters = new int[points[0].Length];

            InitClusters(clusterCount, seed);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="seed"></param>
        private void InitClusters(int count, int seed)
        {
            _clusterCenters = new double[count][];
            _clusterSizes = new int[count];
            _clusterIndices = Enumerable.Range(0, count).ToArray();

            var shuffled = _points.ShallowCopy();
            shuffled.Shuffle(seed);

            for (int i = 0; i < count; i++)
                _clusterCenters[i] = shuffled[i].ShallowCopy();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointIndex"></param>
        public int GetNearestCluster(int pointIndex)
        {
            return _nearestClusters[pointIndex];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clusterIndex"></param>
        /// <returns></returns>
        public double[] GetClusterCenter(int clusterIndex)
        {
            return _clusterCenters[clusterIndex];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clusterIndex"></param>
        /// <returns></returns>
        public int GetClusterSize(int clusterIndex)
        {
            return _clusterSizes[clusterIndex];
        }


        /// <summary>
        /// Returns the number of steps taken to converge.
        /// Returns -1 if convergence was not reached in the maximum number of steps.
        /// </summary>
        /// <param name="maxSteps"></param>
        /// <returns></returns>
        public int Cluster(int maxSteps)
        {
            int count = 0;

            while (count++ < maxSteps)
                if (UpdateNearest() == 0) return count;

            return -1;
        }


        /// <summary>
        /// Returns the number of data points that changed clusters.
        /// </summary>
        private int UpdateNearest()
        {
            // create balanced tree of cluster centers
            var tree = KdTree<int>.CreateBalanced(_clusterCenters, _clusterIndices);

            // for each data point, find nearest cluster center
            int count = 0;
            for (int i = 0; i < _points.Length; i++)
            {
                var nearest = tree.NearestL2(_points[i]);
                // var nearest = tree.EuclideanNearest(_dataPoints[i]);

                if (nearest != _nearestClusters[i])
                {
                    _nearestClusters[i] = nearest;
                    count++;
                }
            }

            return count;
        }


        /// <summary>
        ///
        /// </summary>
        void UpdateClusterCenters()
        {
            _clusterSizes.Clear();

            for (int i = 0; i < _clusterCenters.Length; i++)
                _clusterCenters[i].Clear();

            // aggregate
            for (int i = 0; i < _points.Length; i++)
            {
                var point = _points[i];
                int nearest = _nearestClusters[i];

                var center = _clusterCenters[nearest];
                ArrayMath.Add(center, point, center);
                _clusterSizes[nearest]++;
            }

            // average
            for (int i = 0; i < _clusterCenters.Length; i++)
            {
                var size = _clusterSizes[i];
                if (size == 0) continue;

                var center = _clusterCenters[i];
                ArrayMath.Scale(center, 1.0 / size, center);
            }
        }
    }
}
