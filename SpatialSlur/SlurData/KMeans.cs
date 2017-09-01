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
    [Serializable]
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
        /// <param name="k"></param>
        /// <param name="seed"></param>
        public KMeans(IEnumerable<double[]> points, int k, int seed = 1)
            : this(points.ToArray(), k, seed)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="k"></param>
        /// <param name="seed"></param>
        public KMeans(double[][] points, int k, int seed = 1)
        {
            _points = points;
            _nearestClusters = new int[points.Length];
            InitClusters(k, seed);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="seed"></param>
        private void InitClusters(int k, int seed)
        {
            _clusterCenters = new double[k][];
            _clusterSizes = new int[k];
            _clusterIndices = Enumerable.Range(0, k).ToArray();

            var shuffled = _points.ShallowCopy();
            shuffled.Shuffle(seed);

            for (int i = 0; i < k; i++)
                _clusterCenters[i] = shuffled[i].ShallowCopy();
        }


        /// <summary>
        /// Returns the number of clusters.
        /// </summary>
        public int K
        {
            get { return _clusterSizes.Length; }
        }


        /// <summary>
        /// Returns the number of clusters (i.e. K)
        /// </summary>
        public int ClusterCount
        {
            get { return _clusterSizes.Length; }
        }


        /// <summary>
        /// Returns the number of points being clustered
        /// </summary>
        public int PointCount
        {
            get { return _points.Length; }
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
            {
                if (UpdateNearest() == 0) return count;
                UpdateClusterCenters();
            }

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

                // update if new nearest is different than previous
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
