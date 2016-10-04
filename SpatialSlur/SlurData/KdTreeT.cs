using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 * Left subtree is strictly less than
 * Right subtree is equal or greater than
 * 
 * KdTree search performance degrades at higher dimensions
 * In general the number of nodes should be larger than 2^k for decent performance.
 * 
 * TODO 
 * Compare to array-based implementation
 * 
 * References
 * https://www.cs.umd.edu/class/spring2008/cmsc420/L19.kd-trees.pdf
 * http://www.cs.umd.edu/~meesh/420/Notes/MountNotes/lecture18-kd2.pdf
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Generic implementation of a k-dimensional binary search tree.
    /// </summary>
    public class KdTree<T>
    {
        #region Static

        /// <summary>
        /// Returns the node with the smallest value in the specified dimension.
        /// If equal, n0 is returned.
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <param name="dim"></param>
        /// <returns></returns>
        private static KdNode Min(KdNode n0, KdNode n1, int dim)
        {
            return (n1.point[dim] < n0.point[dim]) ? n1 : n0;
        }


        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        public static KdTree<T> CreateBalanced(IList<Vecd> points, IList<T> values)
        {
            return CreateBalanced(points.ConvertAll(p => p.Values), values);
        }


        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        public static KdTree<T> CreateBalanced(IList<double[]> points, IList<T> values)
        {
            if (points.Count != values.Count)
                throw new ArgumentException("Must provide an equal number of points and values.");

            // create tree using dimension of first point
            int k = points[0].Length;
            KdTree<T> result = new KdTree<T>(k);

            // verify dimension of remaining points
            for (int i = 1; i < points.Count; i++)
                if (points[i].Length != k) throw new ArgumentException("All given points must be of the same dimension.");

            result._root = result.InsertBalanced(points, values, 0, points.Count - 1, 0);
            result._n = points.Count;
            return result;
        }

        #endregion


        private KdNode _root;
        private double _epsilon = 1.0e-8;
        private readonly int _k;
        private int _n;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        public KdTree(int k)
        {
            if (k < 2)
                throw new System.ArgumentException("The tree must have at least 2 dimensions.");

            _k = k;
        }


        /// <summary>
        /// Returns the number of dimensions used by the tree.
        /// </summary>
        public int K
        {
            get{return _k;}
        }


        /// <summary>
        /// Returns the number of nodes in the tree.
        /// </summary>
        public int Count
        {
            get { return _n; }
        }


        /// <summary>
        /// Sets the tolerance used for finding equal points in the tree.
        /// By default, this is set to 1.0e-8.
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
        public bool IsEmpty
        {
            get { return _root == null; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int GetMaxDepth()
        {
            return GetMaxDepth(_root, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private int GetMaxDepth(KdNode node, int i)
        {
            if (node == null) return i;
            return Math.Max(GetMaxDepth(node.left, i + 1), GetMaxDepth(node.right, i + 1));
        }


        /// <summary>
        /// 
        /// </summary>
        public int GetMinDepth()
        {
            var queue = new Queue<KeyValuePair<KdNode, int>>();
            queue.Enqueue(new KeyValuePair<KdNode, int>(_root,0));

            while (queue.Count > 0)
            {
                var t = queue.Dequeue();
                var node = t.Key;
                int depth = t.Value;

                if (node.IsLeaf)
                    return depth;

                queue.Enqueue(new KeyValuePair<KdNode, int>(node.left, depth + 1));
                queue.Enqueue(new KeyValuePair<KdNode, int>(node.right, depth + 1));
            }

            return -1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private void DimCheck(Vecd point)
        {
            if (_k != point.Count)
                throw new System.ArgumentException("The given point must have the same number of dimensions as this tree.");
        }


        /// <summary>
        /// Returns true if the tree contains the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vecd point)
        {
            DimCheck(point);
            return Find(_root, point.Values, 0) != null;
        }


        /// <summary>
        /// Returns true if the tree contains the given point.
        /// The value associated with the given point is also returned in the out parameter on success.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(Vecd point, out T value)
        {
            DimCheck(point);
            KdNode n = Find(_root, point.Values, 0);

            if (n == null)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = n.value;
                return true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private KdNode Find(KdNode node, double[] point, int i)
        {
            if (node == null) 
                return null;

            if (VecMath.Equals(point,node.point,_epsilon,_k))
                return node;

            // wrap dimension
            if (i == _k) i = 0;

            if (point[i] < node.point[i])
                return Find(node.left, point, i + 1);
            else
                return Find(node.right, point, i + 1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void Insert(Vecd point, T value)
        {
            DimCheck(point);
            _root = Insert(_root, point.Values, value, 0);
            _n++;
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <param name="i"></param>
        private KdNode Insert(KdNode node, double[] point, T value, int i)
        {
            if (node == null)
                return new KdNode(point, value);
   
            // wrap dimension
            if (i == _k) i = 0;

            // left or right of node?
            if (point[i] < node.point[i])
                node.left = Insert(node.left, point, value, i + 1);
            else
                node.right = Insert(node.right, point, value, i + 1);

            return node;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="values"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private KdNode InsertBalanced(IList<double[]> points, IList<T> values, int from, int to, int i)
        {
            // stopping conditions
            if (from > to)
                return null;
            else if (from == to)
                return new KdNode(points[from], values[from]);

            // wrap dimension
            if (i == _k) i = 0;

            // sort the median element
            int mid = ((to - from) >> 1) + from;
            KdComparer comparer = new KdComparer(i, _epsilon);
            double[] midPt = points.QuickSelect(mid, from, to, comparer, values);

            // make sure there's no duplicate elements to the left of the median
            int j = from;
            while(j < mid)
            {
                if (comparer.Compare(points[j], midPt) == 0)
                    points.Swap(j, --mid);
                else
                    j++;
            }

            // create node and recurse on left and right children
            KdNode node = new KdNode(points[mid], values[mid]);
            node.left = InsertBalanced(points, values, from, mid - 1, i + 1);
            node.right = InsertBalanced(points, values, mid + 1, to, i + 1);
            return node;
        }


        /*
        /// <summary> 
        /// Finds the first node in the tree which is equal to the given point and flags it for removal.
        /// </summary>
        /// <param name="point"></param>
        public bool Remove(Vecdd point)
        {
            // TODO implement compact method to remove flagged nodes
            DimCheck(point);
            KdNode n = Find(_root, point, 0);

            if (n != null)
            {
                n.isRemoved = true;
                _n--;
            }
        }
        */
    

        /// <summary> 
        /// Removes the first point in the tree which is equal to the given point.
        /// Note that node removal can result in unbalanced trees which degrades search performance.
        /// </summary>
        /// <param name="point"></param>
        public bool Remove(Vecd point)
        {
            DimCheck(point);

            int n = _n;
            _root = Remove(_root, point.Values, 0);
            return n != _n;
        }


        /// <summary>
        /// http://www.cs.umd.edu/~meesh/420/Notes/MountNotes/lecture18-kd2.pdf
        /// http://www.cs.umd.edu/class/spring2002/cmsc420-0401/pbasic.pdf
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private KdNode Remove(KdNode node, double[] point, int i)
        {
            if (node == null)
                return null;

            // wrap dimension
            if (i == _k) i = 0;

            if (VecMath.Equals(point, node.point, _epsilon, _k))
            {
                // found the node to delete
                if (node.right != null)
                {
                    KdNode min = FindMin(node.right, i, i + 1); // search the right sub-tree for a replacement node (min in the current dimension)
                    min.right = Remove(node.right, min.point, i + 1); // remove the replacement node from right subtree
                    min.left = node.left;
                    node = min;
                }
                else if (node.left != null)
                {
                    KdNode min = FindMin(node.left, i, i + 1); // search the left sub-tree for a replacement node (min in the current dimension)
                    min.right = Remove(node.left, min.point, i + 1); // remove the replacement node from the left sub tree
                    min.left = null;
                    node = min;
                }
                else
                {
                    node = null; // node is a leaf, can safely delete
                    _n--; // decrement node count
                }
            }
            else if (point[i] < node.point[i])
            {
                // node not found, continue search to the left
                node.left = Remove(node.left, point, i + 1);
            }
            else
            {
                // node not found, continue search to the right
                node.right = Remove(node.right, point, i + 1);
            }

            return node;
        }


        /// <summary>
        /// Returns the node in the subtree with the smallest value in the specified dimension.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dim"></param>
        /// <param name="i"></param>
        private KdNode FindMin(KdNode node, int dim, int i)
        {
            // wrap dimension
            if (i == _k) i = 0;

            if (dim == i)
            {
                // recurse on left subtree tree only
                if (node.left == null)
                    return node;
                else
                    return FindMin(node.left, dim, i + 1);
            }
            else
            {
                // recurse on both subtrees if they exist
                if (node.IsLeaf)
                    return node;
                else if (node.right == null)
                    return Min(node, FindMin(node.left, dim, i + 1), dim);
                else if (node.left == null)
                    return Min(node, FindMin(node.right, dim, i + 1), dim);
                else
                    return Min(node, Min(FindMin(node.left, dim, i + 1), FindMin(node.right, dim, i + 1), dim), dim);
            }
        }


        /// <summary>
        /// Returns all values within the given Euclidean distance.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        public List<T> EuclideanSearch(Vecd point, double range)
        {
            DimCheck(point);

            List<T> result = new List<T>();
            EuclideanSearch(_root, point.Values, range * range, result, 0);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="result"></param>
        /// <param name="i"></param>
        private void EuclideanSearch(KdNode node, double[] point, double range, List<T> result, int i)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // enqueue value if node is within range
            if (VecMath.SquareDistance(point, node.point, _k) < range)
                result.Add(node.value);

            double d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                EuclideanSearch(node.left, point, range, result, i + 1); // recurse in left subtree
                if (d * d < range) EuclideanSearch(node.right, point, range, result, i + 1); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                EuclideanSearch(node.right, point, range, result, i + 1); // recurse in right subtree
                if (d * d < range) EuclideanSearch(node.left, point, range, result, i + 1); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns all values within the given box.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        public List<T> BoxSearch(Vecd point, Vecd range)
        {
            DimCheck(point);
            DimCheck(range);

            List<T> result = new List<T>();
            BoxSearch(_root, point.Values, range.Values, result, 0);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="result"></param>
        /// <param name="i"></param>
        private void BoxSearch(KdNode node, double[] point, double[] range, List<T> result, int i)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if within range
            if (VecMath.Equals(point, node.point, range, _k))
                result.Add(node.value);

            // if left side of tree
            double d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                BoxSearch(node.left, point, range, result, i + 1); // recurse in left subtree
                if (Math.Abs(d) < range[i]) BoxSearch(node.right, point, range, result, i + 1); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                BoxSearch(node.right, point, range, result, i + 1); // recurse in right subtree
                if (Math.Abs(d) < range[i]) BoxSearch(node.left, point, range, result, i + 1); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns all values within the given Manhattan distance.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        public List<T> ManhattanSearch(Vecd point, double range)
        {
            DimCheck(point);

            List<T> result = new List<T>();
            ManhattanSearch(_root, point.Values, range, result, 0);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="result"></param>
        /// <param name="i"></param>
        private void ManhattanSearch(KdNode node, double[] point, double range, List<T> result, int i)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if within range
            if (VecMath.ManhattanDistance(point, node.point, _k) < range)
                result.Add(node.value);

            double d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                ManhattanSearch(node.left, point, range, result, i + 1); // recurse in left subtree
                if (Math.Abs(d) < range) ManhattanSearch(node.right, point, range, result, i + 1); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                ManhattanSearch(node.right, point, range, result, i + 1); // recurse in right subtree
                if (Math.Abs(d) < range) ManhattanSearch(node.left, point, range, result, i + 1); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Euclidean distance metric.
        /// If the tree is empty the default value of T is returned.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T EuclideanNearest(Vecd point)
        {
            DimCheck(point);

            NearestHelper result = new NearestHelper();
            EuclideanNearest(_root, point.Values, 0, result);

            return result.value;
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Euclidean distance metric.
        /// If the tree is empty the default value of T is returned.
        /// Also returns the distance to the nearest value as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public T EuclideanNearest(Vecd point, out double distance)
        {
            DimCheck(point);

            NearestHelper result = new NearestHelper();
            EuclideanNearest(_root, point.Values, 0, result);

            distance = Math.Sqrt(result.distance);
            return result.value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="result"></param>
        /// <param name="i"></param>
        private void EuclideanNearest(KdNode node, double[] point, int i, NearestHelper result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // update nearest if necessary
            double d = VecMath.SquareDistance(point, node.point, _k);
            if (d < result.distance)
            {
                result.value = node.value;
                result.distance = d;
            }
            
            d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                EuclideanNearest(node.left, point, i + 1, result); // recurse in left subtree first
                if (d * d < result.distance) 
                    EuclideanNearest(node.right, point, i + 1, result); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                EuclideanNearest(node.right, point, i + 1, result); // recurse in right subtree first
                if (d * d < result.distance) 
                    EuclideanNearest(node.left, point, i + 1, result); // recurse in left subtree only if necessary
            }
        }

         
        /// <summary>
        /// Returns the nearest n values in the tree using a Euclidean distance metric.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public T[] EuclideanNearestN(Vecd point, int n)
        {
            DimCheck(point);

            if (n < 1 || n > _n)
                throw new ArgumentException("n must be greater than 0 and less than or equal the number of points in the tree.");

            SortedList<double, T> result = new SortedList<double, T>(new DupComparer<double>());
            EuclideanNearestN(_root, point.Values, n, 0, result);
            
            // return first n values
            return result.Values.SubArray(0, n);
        }


        /// <summary>
        /// Returns the nearest n values in the tree using a Euclidean distance metric.
        /// Also returns the respective distances as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <param name="distances"></param>
        /// <returns></returns>
        public T[] EuclideanNearestN(Vecd point, int n, out double[] distances)
        {
            DimCheck(point);

            if (n < 1 || n > _n)
                throw new ArgumentException("n must be greater than 0 and less than or equal the number of points in the tree.");

            SortedList<double, T> result = new SortedList<double, T>(new DupComparer<double>());
            EuclideanNearestN(_root, point.Values, n, 0, result);

            // get first n distances
            distances = result.Keys.SubArray(0, n);
            for (int i = 0; i < n; i++) distances[i] = Math.Sqrt(distances[i]);

            // return first n values
            return result.Values.SubArray(0, n);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <param name="i"></param>
        /// <param name="result"></param>
        private void EuclideanNearestN(KdNode node, double[] point, int n, int i, SortedList<double, T> result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if closer than nth element
            double d = VecMath.SquareDistance(point, node.point, _k);
            if (result.Count < n || d < result.Keys[n-1])
                result.Add(d, node.value);

            d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                EuclideanNearestN(node.left, point, n, i + 1, result); // recurse in left subtree first
                if (result.Count < n || d * d < result.Keys[n - 1]) 
                    EuclideanNearestN(node.right, point, n, i + 1, result); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                EuclideanNearestN(node.right, point, n, i + 1, result); // recurse in right subtree first
                if (result.Count < n || d * d < result.Keys[n - 1]) 
                    EuclideanNearestN(node.left, point, n, i + 1, result); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Manhattan distance metric.
        /// If the tree is empty the default value of T is returned.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ManhattanNearest(Vecd point)
        {
            DimCheck(point);

            NearestHelper result = new NearestHelper();
            ManhattanNearest(_root, point.Values, 0, result);

            return result.value;
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Manhattan distance metric.
        /// If the tree is empty the default value of T is returned.
        /// Also returns the distance to the nearest value as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public T ManhattanNearest(Vecd point, out double distance)
        {
            DimCheck(point);

            NearestHelper result = new NearestHelper();
            ManhattanNearest(_root, point.Values, 0, result);

            distance = result.distance;
            return result.value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="result"></param>
        /// <param name="i"></param>
        private void ManhattanNearest(KdNode node, double[] point, int i, NearestHelper result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // update nearest if necessary
            double d = VecMath.ManhattanDistance(point, node.point, _k); 
            if (d < result.distance)
            {
                result.value = node.value;
                result.distance = d;
            }

            d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                ManhattanNearest(node.left, point, i + 1, result); // recurse in left subtree first
                if (Math.Abs(d) < result.distance) 
                    ManhattanNearest(node.right, point, i + 1, result); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                ManhattanNearest(node.right, point,  i + 1, result); // recurse in right subtree first
                if (Math.Abs(d) < result.distance) 
                    ManhattanNearest(node.left, point, i + 1, result); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns the nearest n values in the tree using a Manhattan distance metric.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public T[] ManhattanNearestN(Vecd point, int n)
        {
            DimCheck(point);

            if (n < 1 || n > _n)
                throw new ArgumentException("n must be greater than 0 and less than or equal the number of points in the tree");

            SortedList<double, T> result = new SortedList<double, T>(new DupComparer<double>());
            ManhattanNearestN(_root, point.Values, n, 0, result);

            // return first n values
            return result.Values.SubArray(0, n);
        }


        /// <summary>
        /// Returns the nearest n values in the tree using a Manhattan distance metric.
        /// Also returns the respective distances as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <param name="distances"></param>
        /// <returns></returns>
        public T[] ManhattanNearestN(Vecd point, int n, out double[] distances)
        {
            DimCheck(point);

            if (n < 1 || n > _n)
                throw new ArgumentException("n must be greater than 0 and less than or equal the number of points in the tree");

            SortedList<double, T> result = new SortedList<double, T>(new DupComparer<double>());
            ManhattanNearestN(_root, point.Values, n, 0, result);

            // get first n distances
            distances = result.Keys.SubArray(0, n);
            for (int i = 0; i < n; i++) distances[i] = Math.Sqrt(distances[i]);

            // return first n values
            return result.Values.SubArray(0, n);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <param name="i"></param>
        /// <param name="result"></param>
        private void ManhattanNearestN(KdNode node, double[] point, int n, int i, SortedList<double, T> result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if closer than nth element
            double d = VecMath.ManhattanDistance(node.point, point, _k);
            if (result.Count < n || d < result.Keys[n - 1])
                result.Add(d, node.value);

            d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                ManhattanNearestN(node.left, point, n, i + 1, result); // recurse in left subtree first
                if (result.Count < n || Math.Abs(d) < result.Keys[n - 1]) 
                    ManhattanNearestN(node.right, point, n, i + 1, result); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                ManhattanNearestN(node.right, point, n, i + 1, result); // recurse in right subtree first
                if (result.Count < n || Math.Abs(d) < result.Keys[n - 1]) 
                    ManhattanNearestN(node.left, point, n, i + 1, result); // recurse in left subtree only if necessary
            }
        }

       
        /// <summary>
        /// 
        /// </summary>
        private class KdNode
        {
            public KdNode left, right;
            public readonly double[] point;
            public readonly T value;
            // public int depth; // handy to store depth if want to avoid recursion
            // public bool isRemoved; // flag nodes as removed


            /// <summary>
            /// 
            /// </summary>
            public bool IsLeaf
            {
                get { return left == null && right == null; }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="point"></param>
            /// <param name="value"></param>
            public KdNode(double[] point, T value)
            {
                this.point = point;
                this.value = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private class KdComparer : IComparer<double[]>
        {
            public int k; // dimension used for comparison
            public double epsilon; // equality tolerance


            /// <summary>
            /// 
            /// </summary>
            /// <param name="k"></param>
            /// <param name="epsilon"></param>
            public KdComparer(int k, double epsilon)
            {
                this.k = k;
                this.epsilon = epsilon;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="p0"></param>
            /// <param name="p1"></param>
            /// <returns></returns>
            public int Compare(double[] p0, double[] p1)
            {
                double d = p0[k] - p1[k];
                return (Math.Abs(d) < epsilon) ? 0 : Math.Sign(d);
            }
        }


        /// <summary>
        /// Comparer that treats equality as greater than.
        /// Used to avoid errors related to duplicate keys.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        private class DupComparer<U> : IComparer<U> 
            where U : IComparable
        {
            public int Compare(U x, U y)
            {
                int result = x.CompareTo(y);
                return (result == 0) ? 1 : result;
            }
        }


        /// <summary>
        /// Used to cache relevant information during a nearest search.
        /// </summary>
        private class NearestHelper
        {
            public T value;
            public double distance = Double.PositiveInfinity;
        }
    }
}
