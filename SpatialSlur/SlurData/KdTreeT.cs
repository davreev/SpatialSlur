using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 * Left subtree is strictly less than
 * Right subtree is equal or greater than
 * 
 * KdTree search performance degrades at higher dimensions
 * In general the number of nodes should be larger than 2^k for decent performance.
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
        public static KdTree<T> CreateBalanced(IList<double[]> points, IList<T> values)
        {
            if (points is double[][] && values is T[])
                return CreateBalanced((double[][])points, (T[])values);

            if (points.Count != values.Count)
                throw new ArgumentException("Must provide an equal number of points and values.");

            // create tree using dimension of first point
            int k = points[0].Length;
            KdTree<T> result = new KdTree<T>(k);

            // verify dimension of remaining points
            for (int i = 1; i < points.Count; i++)
                if (points[i].Length != k) throw new ArgumentException("All given points must be of the same dimension.");

            result._root = result.InsertBalanced(points, values, 0, points.Count - 1, 0);
            result._count = points.Count;
            return result;
        }


        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// TODO Compare performance to IList implementation.
        /// </summary>
        public static KdTree<T> CreateBalanced(double[][] points, T[] values)
        {
            if (points.Length != values.Length)
                throw new ArgumentException("Must provide an equal number of points and values.");

            // create tree using dimension of first point
            int k = points[0].Length;
            KdTree<T> result = new KdTree<T>(k);

            // verify dimension of remaining points
            for (int i = 1; i < points.Length; i++)
                if (points[i].Length != k) throw new ArgumentException("All given points must be of the same dimension.");

            result._root = result.InsertBalanced(points, values, 0, points.Length - 1, 0);
            result._count = points.Length;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        private static int ReverseCompareKeys<K>(KeyValuePair<K, T> item1, KeyValuePair<K, T> item2)
            where K : IComparable<K>
        {
            return item2.Key.CompareTo(item1.Key);
        }

        #endregion


        private KdNode _root;
        private double _epsilon = 1.0e-8;
        private readonly int _k;
        private int _count;


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
            get { return _count; }
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
            var q0 = new Queue<KdNode>();
            var q1 = new Queue<KdNode>();
            int depth = 0;

            q0.Enqueue(_root);

            while(true)
            {
                // iterate through nodes at the current depth and return if leaf is found
                while (q0.Count > 0)
                {
                    var node = q0.Dequeue();
                    if (node.IsLeaf) return depth;
                    q1.Enqueue(node.left);
                    q1.Enqueue(node.right);
                }

                // swap queues and continue on the next level
                var temp = q0;
                q0 = q1;
                q1 = temp;
                depth++;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private void DimCheck(double[] point)
        {
            if (_k != point.Length)
                throw new System.ArgumentException("The given point must have the same number of dimensions as this tree.");
        }


        /// <summary>
        /// Returns true if the tree contains the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(double[] point)
        {
            DimCheck(point);
            return Find(_root, point, 0) != null;
        }


        /// <summary>
        /// Returns true if the tree contains the given point.
        /// The value associated with the given point is also returned in the out parameter on success.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(double[] point, out T value)
        {
            DimCheck(point);
            KdNode n = Find(_root, point, 0);

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

            if (point.ApproxEquals(node.point, _epsilon))
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
        public void Insert(double[] point, T value)
        {
            DimCheck(point);
            _root = Insert(_root, point, value, 0);
            _count++;
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
            double[] midPt = points.QuickSelect(values, mid, from, to, comparer.Compare);

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="values"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private KdNode InsertBalanced(double[][] points, T[] values, int from, int to, int i)
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
            double[] midPt = points.QuickSelect(values, mid, from, to, comparer.Compare);

            // make sure there's no duplicate elements to the left of the median
            int j = from;
            while (j < mid)
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


        /// <summary> 
        /// Removes the first point in the tree which is equal to the given point.
        /// Note that repeated node removal can result in unbalanced trees which degrades performance.
        /// </summary>
        /// <param name="point"></param>
        public bool Remove(double[] point)
        {
            DimCheck(point);

            int n = _count;
            _root = Remove(_root, point, 0);
            return n != _count;
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

            if (point.ApproxEquals(node.point, _epsilon))
            {
                // found the node to remove
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
                    node = null; // node is a leaf, can safely remove
                    _count--; // decrement node count
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
        /// Returns all values within the given box.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        public List<T> BoxSearch(double[] point, double[] range)
        {
            DimCheck(point);
            DimCheck(range);

            List<T> result = new List<T>();
            BoxSearch(_root, point, range, result, 0);
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
            if (point.ApproxEquals(node.point, range))
                result.Add(node.value);

            // if left side of tree
            double d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is to the left so recurse in left subtree first
                BoxSearch(node.left, point, range, result, i + 1);

                // recurse in right subtree only if necessary
                if (Math.Abs(d) < range[i])
                    BoxSearch(node.right, point, range, result, i + 1);
            }
            else
            {
                // point is to the right so recurse in right subtree first
                BoxSearch(node.right, point, range, result, i + 1);

                // recurse in left subtree only if necessary
                if (Math.Abs(d) < range[i])
                    BoxSearch(node.left, point, range, result, i + 1);
            }
        }


        /// <summary>
        /// Returns all values within the given Euclidean distance.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        public List<T> L2RangeSearch(double[] point, double range)
        {
            DimCheck(point);

            List<T> result = new List<T>();
            L2RangeSearch(_root, point, range * range, result, 0);
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
        private void L2RangeSearch(KdNode node, double[] point, double range, List<T> result, int i)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // enqueue value if node is within range
            if (point.L2DistanceToSqr(node.point) < range)
                result.Add(node.value);

            double d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is to the left so recurse in left subtree first
                L2RangeSearch(node.left, point, range, result, i + 1);

                // recurse in right subtree only if necessary
                if (d * d < range)
                    L2RangeSearch(node.right, point, range, result, i + 1);
            }
            else
            {
                // point is to the right so recurse in right subtree first
                L2RangeSearch(node.right, point, range, result, i + 1);

                // recurse in left subtree only if necessary
                if (d * d < range)
                    L2RangeSearch(node.left, point, range, result, i + 1);
            }
        }


        /// <summary>
        /// Returns all values within the given Manhattan distance.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        public List<T> L1RangeSearch(double[] point, double range)
        {
            DimCheck(point);

            List<T> result = new List<T>();
            L1RangeSearch(_root, point, range, result, 0);
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
        private void L1RangeSearch(KdNode node, double[] point, double range, List<T> result, int i)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if within range
            if (point.L1DistanceTo(node.point) < range)
                result.Add(node.value);

            double d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is to the left so recurse in left subtree first
                L1RangeSearch(node.left, point, range, result, i + 1);

                // recurse in right subtree only if necessary
                if (Math.Abs(d) < range)
                    L1RangeSearch(node.right, point, range, result, i + 1);
            }
            else
            {
                // point is to the right so recurse in right subtree first
                L1RangeSearch(node.right, point, range, result, i + 1);

                // recurse in left subtree only if necessary
                if (Math.Abs(d) < range)
                    L1RangeSearch(node.left, point, range, result, i + 1); 
            }
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Euclidean distance metric.
        /// If the tree is empty the default value of T is returned.
        /// Also returns the square distance to the nearest value as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T L2Nearest(double[] point)
        {
            double d;
            return L2Nearest(point, out d);
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Euclidean distance metric.
        /// If the tree is empty the default value of T is returned.
        /// Also returns the square distance to the nearest value as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public T L2Nearest(double[] point, out double distance)
        {
            DimCheck(point);

            T nearest = default(T);
            distance = double.MaxValue;
            L2Nearest(_root, point, 0, ref nearest, ref distance);

            return nearest;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="nearest"></param>
        /// <param name="distance"></param>
        private void L2Nearest(KdNode node, double[] point, int i, ref T nearest, ref double distance)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // update nearest if necessary
            double d = point.L2DistanceToSqr(node.point);
            if (d < distance)
            {
                nearest = node.value;
                distance = d;
            }

            d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is to the left so recurse in left subtree first
                L2Nearest(node.left, point, i + 1, ref nearest, ref distance);

                // recurse in right subtree only if necessary
                if (d * d < distance)
                    L2Nearest(node.right, point, i + 1, ref nearest, ref distance);
            }
            else
            {
                // point is to the right so recurse in right subtree first
                L2Nearest(node.right, point, i + 1, ref nearest, ref distance);

                // recurse in left subtree only if necessary
                if (d * d < distance)
                    L2Nearest(node.left, point, i + 1, ref nearest, ref distance);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T L1Nearest(double[] point)
        {
            double d;
            return L1Nearest(point, out d);
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Manhattan distance metric.
        /// If the tree is empty the default value of T is returned.
        /// Also returns the distance to the nearest value as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public T L1Nearest(double[] point, out double distance)
        {
            DimCheck(point);

            T nearest = default(T);
            distance = Double.MaxValue;
            L1Nearest(_root, point, 0, ref nearest, ref distance);

            return nearest;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="nearest"></param>
        /// <param name="distance"></param>
        private void L1Nearest(KdNode node, double[] point, int i, ref T nearest, ref double distance)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // update nearest if necessary
            double d = point.L1DistanceTo(node.point);
            if (d < distance)
            {
                nearest = node.value;
                distance = d;
            }

            d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is to the left so recurse in left subtree first
                L1Nearest(node.left, point, i + 1, ref nearest, ref distance);

                // recurse in right subtree only if necessary
                if (Math.Abs(d) < distance)
                    L1Nearest(node.right, point, i + 1, ref nearest, ref distance);
            }
            else
            {
                // point is to the right so recurse in right subtree first
                L1Nearest(node.right, point, i + 1, ref nearest, ref distance);

                // recurse in left subtree only if necessary
                if (Math.Abs(d) < distance)
                    L1Nearest(node.left, point, i + 1, ref nearest, ref distance); 
            }
        }


        /// <summary>
        /// Returns the nearest n values and their Euclidean distances.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public PriorityQueue<KeyValuePair<double, T>> L2NearestN(double[] point, int n)
        {
            DimCheck(point);

            var result = new PriorityQueue<KeyValuePair<double, T>>(ReverseCompareKeys, n);
            L2NearestN(_root, point, n, 0, result);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <param name="i"></param>
        /// <param name="result"></param>
        private void L2NearestN(KdNode node, double[] point, int n, int i, PriorityQueue<KeyValuePair<double, T>> result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if closer than nth element
            double d = point.L2DistanceToSqr(node.point);
        
            if (result.Count < n)
                result.Insert(new KeyValuePair<double, T>(d, node.value));
            else if (d < result.Min.Key)
                result.ReplaceMin(new KeyValuePair<double, T>(d, node.value));

            d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is to the left so recurse in left subtree first
                L2NearestN(node.left, point, n, i + 1, result);

                // recurse in right subtree only if necessary
                if (result.Count < n || d * d < result.Min.Key)
                    L2NearestN(node.right, point, n, i + 1, result);
            }
            else
            {
                // point is to the right so recurse in right subtree first
                L2NearestN(node.right, point, n, i + 1, result);

                // recurse in left subtree only if necessary
                if (result.Count < n || d * d < result.Min.Key)
                    L2NearestN(node.left, point, n, i + 1, result);
            }
        }


        /// <summary>
        /// Returns the nearest n values and their Manhattan distances.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public PriorityQueue<KeyValuePair<double, T>> L1NearestN(double[] point, int n)
        {
            DimCheck(point);

            var result = new PriorityQueue<KeyValuePair<double, T>>(ReverseCompareKeys, n);
            L1NearestN(_root, point, n, 0, result);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <param name="i"></param>
        /// <param name="result"></param>
        private void L1NearestN(KdNode node, double[] point, int n, int i, PriorityQueue<KeyValuePair<double, T>> result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if closer than nth element
            double d = point.L1DistanceTo(node.point);

            if (result.Count < n)
                result.Insert(new KeyValuePair<double, T>(d, node.value));
            else if (d < result.Min.Key)
                result.ReplaceMin(new KeyValuePair<double, T>(d, node.value));

            d = point[i] - node.point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is to the left so recurse in left subtree first
                L1NearestN(node.left, point, n, i + 1, result);

                // recurse in right subtree only if necessary
                if (result.Count < n || Math.Abs(d) < result.Min.Key)
                    L1NearestN(node.right, point, n, i + 1, result); 
            }
            else
            {
                // point is to the right so recurse in right subtree first
                L1NearestN(node.right, point, n, i + 1, result);

                // recurse in left subtree only if necessary
                if (result.Count < n || Math.Abs(d) < result.Min.Key)
                    L1NearestN(node.left, point, n, i + 1, result); 
            }
        }


        /*
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

            SortedList<double, T> result = new SortedList<double, T>(new NoEqualComparer<double>()); // TODO use PriorityQueue instead of sorted list
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

            SortedList<double, T> result = new SortedList<double, T>(new NoEqualComparer<double>());
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

            SortedList<double, T> result = new SortedList<double, T>(new NoEqualComparer<double>());
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

            SortedList<double, T> result = new SortedList<double, T>(new NoEqualComparer<double>());
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
        */


        /// <summary>
        /// 
        /// </summary>
        private class KdNode
        {
            public KdNode left, right;
            public readonly double[] point;
            public readonly T value;
           

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
    }
}
