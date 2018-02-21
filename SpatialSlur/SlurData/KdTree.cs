using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;

using static SpatialSlur.SlurData.ArrayMath;

/*
 * Notes
 * 
 * Left subtree is strictly less than
 * Right subtree is equal or greater than
 * 
 * KdTree search performance degrades at higher dimensions.
 * In general the number of nodes should be larger than 2^k for decent search performance.
 * 
 * References
 * https://www.cs.umd.edu/class/spring2008/cmsc420/L19.kd-trees.pdf
 * http://www.cs.umd.edu/~meesh/420/Notes/MountNotes/lecture18-kd2.pdf
 * 
 * TODO
 * Compare to array-based implementation
 * Would be more efficient for rebalancing/updating points
 * Doesn't improve cache locality though since nodes are heap allocated.
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Constains type-inferred static creation methods.
    /// </summary>
    public static class KdTree
    {
        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        public static KdTree<T> CreateBalanced<T>(IEnumerable<double[]> points, IEnumerable<T> values)
        {
            return KdTree<T>.CreateBalanced(points, values);
        }


        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        public static KdTree<T> CreateBalanced<T>(double[][] points, T[] values)
        {
            return KdTree<T>.CreateBalanced(points, values);
        }


        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        public static KdTree<int> CreateBalanced(IEnumerable<double[]> points)
        {
            return KdTree<int>.CreateBalanced(points, Sequences.CountFrom(0));
        }


        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        public static KdTree<int> CreateBalanced(double[][] points)
        {
            return KdTree<int>.CreateBalanced(points, Enumerable.Range(0, points.Length).ToArray());
        }
    }

    
    /// <summary>
    /// Generic implementation of a k-dimensional binary search tree.
    /// </summary>
    [Serializable]
    public class KdTree<T>
    {
        #region Static

        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        internal static KdTree<T> CreateBalanced(IEnumerable<double[]> points, IEnumerable<T> values)
        {
            KdTree<T> result = new KdTree<T>(points.First().Length);
            Node[] nodes = points.Zip(values, (p, v) => new Node(p, v)).ToArray();

            result._root = result.InsertBalanced(nodes, 0, nodes.Length - 1, 0);
            result._count = nodes.Length;
            return result;
        }


        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        internal static KdTree<T> CreateBalanced(double[][] points, T[] values)
        {
            KdTree<T> result = new KdTree<T>(points[0].Length);
            Node[] nodes = points.Convert((p, i) => new Node(p, values[i]));

            result._root = result.InsertBalanced(nodes, 0, nodes.Length - 1, 0);
            result._count = nodes.Length;
            return result;
        }

        
        /// <summary>
        /// Returns the node with the smallest value in the given dimension.
        /// If equal, n0 is returned.
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static Node Min(Node n0, Node n1, int i)
        {
            return (n1.Point[i] < n0.Point[i]) ? n1 : n0;
        }

        #endregion


        private Node _root;
        private double _tolerance = SlurMath.ZeroTolerance;
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
        /// Returns the number of dimensions used by the tree (i.e. K)
        /// </summary>
        public int Dimension
        {
            get { return _k; }
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
        /// </summary>
        public double Tolerance
        {
            get { return _tolerance; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("The value must be greater than zero.");

                _tolerance = value;
            }
        }


        /// <summary>
        ///
        /// </summary>
        public bool IsEmpty
        {
            get { return _root == null; }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountDebug()
        {
            int count = 0;
            CountDebug(_root, ref count);
            return count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="count"></param>
        private void CountDebug(Node node, ref int count)
        {
            if (node == null)
                return;

            count++;

            CountDebug(node.Left, ref count);
            CountDebug(node.Right, ref count);
        }
        */


        /// <summary>
        /// Creates a shallow copy of the tree
        /// </summary>
        /// <returns></returns>
        public KdTree<T> Duplicate()
        {
            var copy = new KdTree<T>(_k);
            copy._root = Duplicate(_root);
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private Node Duplicate(Node other)
        {
            var copy = new Node(other.Point, other.Value);

            if (other.Left != null)
                copy.Left = Duplicate(other.Left);

            if (other.Right != null)
                copy.Right = Duplicate(other.Right);

            return copy;
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
        private int GetMaxDepth(Node node, int i)
        {
            if (node == null) return i;
            return Math.Max(GetMaxDepth(node.Left, i + 1), GetMaxDepth(node.Right, i + 1));
        }


        /// <summary>
        /// 
        /// </summary>
        public int GetMinDepth()
        {
            var q0 = new Queue<Node>();
            var q1 = new Queue<Node>();
            int depth = 0;

            q0.Enqueue(_root);

            while(true)
            {
                // iterate through nodes at the current depth and return if leaf is found
                while (q0.Count > 0)
                {
                    var node = q0.Dequeue();
                    var left = node.Left;
                    var right = node.Right;

                    if(left == null)
                    {
                        if (right == null) return depth; // reached leaf
                        q1.Enqueue(right);
                    }
                    else
                    {
                        q1.Enqueue(left);
                        if (right != null) q1.Enqueue(right);
                    }
                }

                // swap queues and continue on the next level
                CoreUtil.Swap(ref q0, ref q1);
                depth++;
            }
        }


        /// <summary>
        /// Returns true if the tree contains the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(double[] point)
        {
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
            Node n = Find(_root, point, 0);

            if (n == null)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = n.Value;
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
        private Node Find(Node node, double[] point, int i)
        {
            if (node == null) 
                return null;

            if (ApproxEquals(point, node.Point, _tolerance))
                return node;

            // wrap dimension
            if (i == _k) i = 0;

            if (point[i] < node.Point[i])
                return Find(node.Left, point, i + 1);
            else
                return Find(node.Right, point, i + 1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void Insert(double[] point, T value)
        {
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
        private Node Insert(Node node, double[] point, T value, int i)
        {
            if (node == null)
                return new Node(point, value);
   
            // wrap dimension
            if (i == _k) i = 0;

            // left or right of node?
            if (point[i] < node.Point[i])
                node.Left = Insert(node.Left, point, value, i + 1);
            else
                node.Right = Insert(node.Right, point, value, i + 1);

            return node;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private Node InsertBalanced(Node[] nodes, int from, int to, int i)
        {
            // stopping conditions
            if (from > to)
                return null;
            else if (from == to)
                return nodes[from];

            // wrap dimension
            if (i == _k) i = 0;

            // sort the median element
            int mid = ((to - from) >> 1) + from;
            var midValue = nodes.QuickSelect(mid, from, to, (n0, n1) => n0.Point[i].CompareTo(n1.Point[i])).Point[i];

            // ensure no duplicate elements to the left of the median
            int j = from;
            while (j < mid)
            {
                if (nodes[j].Point[i] == midValue)
                    nodes.Swap(j, --mid);
                else
                    j++;
            }
            
            // recall on left and right children
            var midNode = nodes[mid];
            midNode.Left = InsertBalanced(nodes, from, mid - 1, i + 1);
            midNode.Right = InsertBalanced(nodes, mid + 1, to, i + 1);
            return midNode;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="values"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private Node InsertBalanced(double[][] points, T[] values, int from, int to, int i)
        {
            // stopping conditions
            if (from > to)
                return null;
            else if (from == to)
                return new Node(points[from], values[from]);

            // wrap dimension
            if (i == _k) i = 0;

            // sort the median element
            int mid = ((to - from) >> 1) + from;
            var midPoint = points.QuickSelect(values, mid, from, to, (p0, p1) => p0[i].CompareTo(p1[i]));

            // ensure no duplicate elements to the left of the median
            int j = from;
            double t = midPoint[i];

            while (j < mid)
            {
                if (points[j][i] == t)
                {
                    points.Swap(j, --mid);
                    values.Swap(j, mid);
                }
                else
                {
                    j++;
                }
            }

            // create node and recall on left and right children
            Node node = new Node(points[mid], values[mid]);
            node.Left = InsertBalanced(points, values, from, mid - 1, i + 1);
            node.Right = InsertBalanced(points, values, mid + 1, to, i + 1);
            return node;
        }
        */
      

        /// <summary> 
        /// Removes the first point in the tree which is equal to the given point.
        /// Note that repeated node removal can result in unbalanced trees which degrades search performance.
        /// </summary>
        /// <param name="point"></param>
        public bool Remove(double[] point)
        {
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
        private Node Remove(Node node, double[] point, int i)
        {
            if (node == null)
                return null;

            // wrap dimension
            if (i == _k) i = 0;

            if (ApproxEquals(point, node.Point, _tolerance))
            {
                // found the node to remove, find replacement if it has children
                if (node.Right != null)
                {
                    Node min = FindMin(node.Right, i, i + 1); // search the right sub-tree for a replacement node (min in the current dimension)
                    min.Right = Remove(node.Right, min.Point, i + 1); // remove the replacement node from right subtree
                    min.Left = node.Left;
                    node = min;
                }
                else if (node.Left != null)
                {
                    Node min = FindMin(node.Left, i, i + 1); // search the left sub-tree for a replacement node (min in the current dimension)
                    min.Right = Remove(node.Left, min.Point, i + 1); // remove the replacement node from the left sub tree
                    min.Left = null;
                    node = min;
                }
                else
                {
                    node = null; // node is a leaf, can safely remove
                    _count--; // decrement node count
                }
            }
            else if (point[i] < node.Point[i])
            {
                // node not found, continue search to the left
                node.Left = Remove(node.Left, point, i + 1);
            }
            else
            {
                // node not found, continue search to the right
                node.Right = Remove(node.Right, point, i + 1);
            }

            return node;
        }


        /// <summary>
        /// Returns the node in the subtree with the smallest value in the specified dimension.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dim"></param>
        /// <param name="i"></param>
        private Node FindMin(Node node, int dim, int i)
        {
            // wrap dimension
            if (i == _k) i = 0;

            if (dim == i)
            {
                // recall on left subtree tree only
                if (node.Left == null)
                    return node;
                else
                    return FindMin(node.Left, dim, i + 1);
            }
            else
            {
                // recall on both subtrees if they exist
                if (node.IsLeaf)
                    return node;
                else if (node.Right == null)
                    return Min(node, FindMin(node.Left, dim, i + 1), dim);
                else if (node.Left == null)
                    return Min(node, FindMin(node.Right, dim, i + 1), dim);
                else
                    return Min(node, Min(FindMin(node.Left, dim, i + 1), FindMin(node.Right, dim, i + 1), dim), dim);
            }
        }


        /// <summary>
        /// Calls the given delegate on each found value.
        /// The search can be aborted by returning false from the given callback. 
        /// If aborted, this function will also return false.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool BoxSearch(double[] point, double[] range, Func<T, bool> callback)
        {
            return BoxSearchImpl(_root, point, range, callback, 0);
        }


        /// <summary>
        /// Returns all found values by appending them to the given list.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="result"></param>
        public void BoxSearch(double[] point, double[] range, List<T> result)
        {
            BoxSearch(point, range, t =>
            {
                result.Add(t);
                return true;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="callback"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool BoxSearchImpl(Node node, double[] point, double[] range, Func<T, bool> callback, int i)
        {
            if (node == null) return true;

            // wrap dimension
            if (i == _k) i = 0;

            // callback on node if within range
            if (ApproxEquals(point, node.Point, range))
                if (!callback(node.Value)) return false;
            
            // signed distance to cut plane
            double d = point[i] - node.Point[i];

            if (d < 0.0)
            {
                // point is to the left, recall in left subtree first
                BoxSearchImpl(node.Left, point, range, callback, i + 1);

                // recall in right subtree only if necessary
                if (Math.Abs(d) < range[i])
                    BoxSearchImpl(node.Right, point, range, callback, i + 1);
            }
            else
            {
                // point is to the right, recall in right subtree first
                BoxSearchImpl(node.Right, point, range, callback, i + 1);

                // recall in left subtree only if necessary
                if (Math.Abs(d) < range[i])
                    BoxSearchImpl(node.Left, point, range, callback, i + 1);
            }

            return true;
        }


        /// <summary>
        /// Calls the given delegate on each found value.
        /// The search can be aborted by returning false from the given callback. 
        /// If aborted, this function will also return false.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="callback"></param>
        public bool RangeSearchL2(double[] point, double range, Func<T, bool> callback)
        {
            return RangeSearchL2Impl(_root, point, range * range, callback, 0);
        }


        /// <summary>
        /// Returns all found values by appending them to the given list.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="result"></param>
        public void RangeSearchL2(double[] point, double range, List<T> result)
        {
            RangeSearchL2(point, range, t =>
             {
                 result.Add(t);
                 return true;
             });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="squareRange"></param>
        /// <param name="callback"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool RangeSearchL2Impl(Node node, double[] point, double squareRange, Func<T, bool> callback, int i)
        {
            if (node == null) return true;

            // wrap dimension
            if (i == _k) i = 0;

            // enqueue value if node is within range
            if (SquareDistanceL2(point, node.Point) < squareRange)
                if(!callback(node.Value)) return false;

            // signed distance to cut plane
            double d = point[i] - node.Point[i];

            if (d < 0.0)
            {
                // point is to the left, recall in left subtree first
                RangeSearchL2Impl(node.Left, point, squareRange, callback, i + 1);

                // recall in right subtree only if necessary
                if (d * d < squareRange)
                    RangeSearchL2Impl(node.Right, point, squareRange, callback, i + 1);
            }
            else
            {
                // point is to the right, recall in right subtree first
                RangeSearchL2Impl(node.Right, point, squareRange, callback, i + 1);

                // recall in left subtree only if necessary
                if (d * d < squareRange)
                    RangeSearchL2Impl(node.Left, point, squareRange, callback, i + 1);
            }

            return true;
        }


        /// <summary>
        /// Calls the given delegate on each found value.
        /// The search can be aborted by returning false from the given callback. 
        /// If aborted, this function will also return false.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="callback"></param>
        public bool RangeSearchL1(double[] point, double range, Func<T, bool> callback)
        {
            return RangeSearchL1Impl(_root, point, range, callback, 0);
        }


        /// <summary>
        /// Returns all found values by appending them to the given list.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="result"></param>
        public void RangeSearchL1(double[] point, double range, List<T> result)
        {
            RangeSearchL1(point, range, t =>
            {
                result.Add(t);
                return true;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <param name="callback"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool RangeSearchL1Impl(Node node, double[] point, double range, Func<T, bool> callback, int i)
        {
            if (node == null) return true;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if within range
            if (DistanceL1(point, node.Point) < range)
                if (!callback(node.Value)) return false;

            // signed distance to cut plane
            double d = point[i] - node.Point[i];

            if (d < 0.0)
            {
                // point is to the left, recall in left subtree first
                RangeSearchL1Impl(node.Left, point, range, callback, i + 1);

                // recall in right subtree only if necessary
                if (Math.Abs(d) < range)
                    RangeSearchL1Impl(node.Right, point, range, callback, i + 1);
            }
            else
            {
                // point is to the right, recall in right subtree first
                RangeSearchL1Impl(node.Right, point, range, callback, i + 1);

                // recall in left subtree only if necessary
                if (Math.Abs(d) < range)
                    RangeSearchL1Impl(node.Left, point, range, callback, i + 1);
            }

            return true;
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Euclidean distance metric.
        /// If the tree is empty the default value of T is returned.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T NearestL2(double[] point)
        {
            return NearestL2(point, out double d);
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Euclidean distance metric.
        /// If the tree is empty the default value of T is returned.
        /// Also returns the square distance to the nearest value as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public T NearestL2(double[] point, out double distance)
        {
            T nearest = default(T);
            distance = double.MaxValue;
            NearestL2(_root, point, 0, ref nearest, ref distance);
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
        private void NearestL2(Node node, double[] point, int i, ref T nearest, ref double distance)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // update nearest if necessary
            double d = SquareDistanceL2(point, node.Point);
            if (d < distance)
            {
                nearest = node.Value;
                distance = d;
            }

            // signed distance to cut plane
            d = point[i] - node.Point[i];

            if (d < 0.0)
            {
                // point is to the left, recall in left subtree first
                NearestL2(node.Left, point, i + 1, ref nearest, ref distance);

                // recall in right subtree only if necessary
                if (d * d < distance)
                    NearestL2(node.Right, point, i + 1, ref nearest, ref distance);
            }
            else
            {
                // point is to the right, recall in right subtree first
                NearestL2(node.Right, point, i + 1, ref nearest, ref distance);

                // recall in left subtree only if necessary
                if (d * d < distance)
                    NearestL2(node.Left, point, i + 1, ref nearest, ref distance);
            }
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Manhattan distance metric.
        /// If the tree is empty the default value of T is returned.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T NearestL1(double[] point)
        {
            return NearestL1(point, out double d);
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Manhattan distance metric.
        /// If the tree is empty the default value of T is returned.
        /// Also returns the distance to the nearest value as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public T NearestL1(double[] point, out double distance)
        {
            T nearest = default(T);
            distance = Double.MaxValue;
            NearestL1(_root, point, 0, ref nearest, ref distance);
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
        private void NearestL1(Node node, double[] point, int i, ref T nearest, ref double distance)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // update nearest if necessary
            double d = DistanceL1(point, node.Point);
            if (d < distance)
            {
                nearest = node.Value;
                distance = d;
            }

            // signed distance to cut plane
            d = point[i] - node.Point[i];

            if (d < 0.0)
            {
                // point is to the left, recall in left subtree first
                NearestL1(node.Left, point, i + 1, ref nearest, ref distance);

                // recall in right subtree only if necessary
                if (Math.Abs(d) < distance)
                    NearestL1(node.Right, point, i + 1, ref nearest, ref distance);
            }
            else
            {
                // point is to the right, recall in right subtree first
                NearestL1(node.Right, point, i + 1, ref nearest, ref distance);

                // recall in left subtree only if necessary
                if (Math.Abs(d) < distance)
                    NearestL1(node.Left, point, i + 1, ref nearest, ref distance); 
            }
        }


        /// <summary>
        /// Returns the nearest k values and their squared Euclidean distances.
        /// Note distances will be negative as the search uses a max priority queue.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public PriorityQueue<double, T> KNearestL2(double[] point, int k)
        {
            var result = new PriorityQueue<double, T>(k);
            KNearestL2(_root, point, k, 0, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="k"></param>
        /// <param name="i"></param>
        /// <param name="result"></param>
        private void KNearestL2(Node node, double[] point, int k, int i, PriorityQueue<double, T> result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // use negative square distance for max priority queue
            double d = -SquareDistanceL2(point, node.Point); 

            // insert node if closer than nth element
            if (result.Count < k)
                result.Insert(d, node.Value);
            else if (d > result.Min.Key)
                result.ReplaceMin(d, node.Value);

            // signed distance to cut plane
            d = point[i] - node.Point[i];

            if (d < 0.0)
            {
                // point is to the left, recall in left subtree first
                KNearestL2(node.Left, point, k, i + 1, result);

                // recall in right subtree only if necessary
                if (result.Count < k || d * d < -result.Min.Key)
                    KNearestL2(node.Right, point, k, i + 1, result);
            }
            else
            {
                // point is to the right, recall in right subtree first
                KNearestL2(node.Right, point, k, i + 1, result);

                // recall in left subtree only if necessary
                if (result.Count < k || d * d < -result.Min.Key)
                    KNearestL2(node.Left, point, k, i + 1, result);
            }
        }


        /// <summary>
        /// Returns the nearest k values and their Manhattan distances.
        /// Note distances will be negative as the search uses a max priority queue.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public PriorityQueue<double, T> KNearestL1(double[] point, int k)
        {
            var result = new PriorityQueue<double, T>(k);
            KNearestL1(_root, point, k, 0, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="k"></param>
        /// <param name="i"></param>
        /// <param name="result"></param>
        private void KNearestL1(Node node, double[] point, int k, int i, PriorityQueue<double, T> result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // use negative distance for max priority queue
            double d = -DistanceL1(point, node.Point);

            // insert node if closer than nth element
            if (result.Count < k)
                result.Insert(d, node.Value);
            else if (d > result.Min.Key)
                result.ReplaceMin(d, node.Value);

            // signed distance to cut plane
            d = point[i] - node.Point[i];

            if (d < 0.0)
            {
                // point is to the left, recall in left subtree first
                KNearestL1(node.Left, point, k, i + 1, result);

                // recall in right subtree only if necessary
                if (result.Count < k || Math.Abs(d) < -result.Min.Key)
                    KNearestL1(node.Right, point, k, i + 1, result); 
            }
            else
            {
                // point is to the right, recall in right subtree first
                KNearestL1(node.Right, point, k, i + 1, result);

                // recall in left subtree only if necessary
                if (result.Count < k || Math.Abs(d) < -result.Min.Key)
                    KNearestL1(node.Left, point, k, i + 1, result); 
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private class Node
        {
            /// <summary></summary>
            public Node Left;
            /// <summary></summary>
            public Node Right;
            /// <summary></summary>
            public readonly double[] Point;
            /// <summary></summary>
            public readonly T Value;


            /// <summary>
            /// 
            /// </summary>
            public bool IsLeaf
            {
                get { return Left == null && Right == null; }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="point"></param>
            /// <param name="value"></param>
            public Node(double[] point, T value)
            {
                Point = point;
                Value = value;
            }
        }
    }
}
