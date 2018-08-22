
/*
 * Notes
 * 
 * Left subtree is strictly less than
 * Right subtree is equal or greater than
 * 
 * KdTree search performance degrades at higher dimensions.
 * In general, the number of nodes should be larger than 2^k for decent search performance.
 * 
 * Impl ref
 * https://www.cs.umd.edu/class/spring2008/cmsc420/L19.kd-trees.pdf
 * http://www.cs.umd.edu/~meesh/420/Notes/MountNotes/lecture18-kd2.pdf
 * 
 * TODO compare to array-based implementation
 * Could be more efficient for rebalancing/updating points
 * Will only improve cache locality if nodes are stack allocated
 */

using System;
using System.Collections.Generic;
using System.Linq;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
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
    /// 
    /// </summary>
    [Serializable]
    public class KdTree<T>
    {
        #region Nested Types

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


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        private class Pair<T0, T1>
        {
            public T0 Item1;
            public T1 Item2;
        }

        #endregion


        #region Static Members

        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        internal static KdTree<T> CreateBalanced(IEnumerable<double[]> points, IEnumerable<T> values)
        {
            KdTree<T> result = new KdTree<T>(points.First().Length);
            Node[] nodes = points.Zip(values, (p, v) => new Node(p, v)).ToArray();

            result._root = result.InsertBalanced(nodes, 0, nodes.Length, 0);
            result._count = nodes.Length;
            return result;
        }


        /// <summary>
        /// Inserts point value pairs in a way that produces a balanced tree.
        /// </summary>
        internal static KdTree<T> CreateBalanced(double[][] points, T[] values)
        {
            KdTree<T> result = new KdTree<T>(points[0].Length);
            Node[] nodes = new Node[points.Length];

            for (int i = 0; i < points.Length; i++)
                nodes[i] = new Node(points[i], values[i]);

            result._root = result.InsertBalanced(nodes, 0, nodes.Length, 0);
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
        private double _tolerance = D.ZeroTolerance;
        private readonly int _dimension;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dimension"></param>
        public KdTree(int dimension)
        {
            if (dimension < 2)
                throw new System.ArgumentException("The tree must have at least 2 dimensions.");

            _dimension = dimension;
        }


        /// <summary>
        /// Returns the number of dimensions used by the tree (i.e. K)
        /// </summary>
        public int Dimension
        {
            get { return _dimension; }
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


        /// <summary>
        /// 
        /// </summary>
        private int NextIndex(int i)
        {
            return (++i == _dimension) ? 0 : i;
        }


#if DEBUG
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
#endif


        /// <summary>
        /// Creates a shallow copy of the tree
        /// </summary>
        /// <returns></returns>
        public KdTree<T> Duplicate()
        {
            var copy = new KdTree<T>(_dimension);
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
            if (node == null)
                return i;

            i++;
            return Math.Max(
                GetMaxDepth(node.Left, i), 
                GetMaxDepth(node.Right, i));
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
                Utilities.Swap(ref q0, ref q1);
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
            return Find(_root, point) != null;
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
            Node n = Find(_root, point);

            if (n == null)
            {
                value = default;
                return false;
            }

            value = n.Value;
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private Node Find(Node node, double[] point)
        {
            int n = _dimension;
            int i = 0;
            
            while(node != null)
            {
                var p = node.Point;

                if (Vector.ApproxEquals(point, p, _tolerance))
                    return node;

                node = (point[i] < p[i]) ? node.Left : node.Right;
                i = (++i == n) ? 0 : i;
            }

            return null;
        }


#if OBSOLETE
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
                value = default;
                return false;
            }

            value = n.Value;
            return true;
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

            if (Vector.ApproxEquals(point, node.Point, _tolerance))
                return node;
            
            if (i == _dim) i = 0;

            if (point[i] < node.Point[i])
                return Find(node.Left, point, i + 1);
            else
                return Find(node.Right, point, i + 1);
        }
#endif


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void Insert(double[] point, T value)
        {
            if (_root == null)
                _root = new Node(point, value);
            else
                Insert(_root, point, value);

            _count++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        private void Insert(Node node, double[] point, T value)
        {
            int n = _dimension;
            int i = 0;

            while (true)
            {
                if (point[i] < node.Point[i])
                {
                    if (node.Left == null)
                    {
                        node.Left = new Node(point, value);
                        return;
                    }

                    node = node.Left;
                }
                else
                {
                    if (node.Right == null)
                    {
                        node.Right = new Node(point, value);
                        return;
                    }

                    node = node.Right;
                }

                i = (++i == n) ? 0 : i;
            }
        }


#if OBSOLETE
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
            if (i == _dim) i = 0;

            // left or right of node?
            if (point[i] < node.Point[i])
                node.Left = Insert(node.Left, point, value, i + 1);
            else
                node.Right = Insert(node.Right, point, value, i + 1);

            return node;
        }
#endif


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
            int n = to - from;

            if (n == 1)
                return nodes[from];
            else if(n < 1)
                return null;

            // find the median node in the current dimension
            int median = (n >> 1) + from;
            var node = nodes.QuickSelect(median, from, to, (n0, n1) => n0.Point[i].CompareTo(n1.Point[i]));
            var value = node.Point[i];

            // ensure no equal elements to the left of the median
            int j = from;
            while (j < median)
            {
                if (nodes[j].Point[i] == value)
                    nodes.Swap(j, --median);
                else
                    j++;
            }

            // recall on left and right children
            node = nodes[median];
            i = NextIndex(i);

            node.Left = InsertBalanced(nodes, from, median, i);
            node.Right = InsertBalanced(nodes, median + 1, to, i);

            return node;
        }


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
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private Node Remove(Node node, double[] point, int i)
        {
            if (node == null) return null;
            var p = node.Point;

            // found node to remove, return replacement node
            if (Vector.ApproxEquals(point, p, _tolerance))
                return FindReplacement(node, i);

            int j = NextIndex(i);

            // node not found, continue search
            if (point[i] < p[i])
                node.Left = Remove(node.Left, point, j);
            else
                node.Right = Remove(node.Right, point, j);
            
            return node;
        }


        /// <summary>
        /// Searches the sub-tree of the given node for a valid replacement node.
        /// </summary>
        private Node FindReplacement(Node node, int i)
        {
            // impl refs
            // http://www.cs.umd.edu/~meesh/420/Notes/MountNotes/lecture18-kd2.pdf
            // http://www.cs.umd.edu/class/spring2002/cmsc420-0401/pbasic.pdf

            var right = node.Right;
            var left = node.Left;
            
            if (right != null)
            {
                int j = NextIndex(i);
                var min = FindMin(right, i, j);
                min.Right = Remove(right, min.Point, j);
                min.Left = left;
                return min;
            }
            else if (left != null)
            {
                int j = NextIndex(i);
                var min = FindMin(left, i, j);
                min.Right = Remove(left, min.Point, j);
                min.Left = null;
                return min;
            }

            // node is leaf, no need to find replacement
            _count--;
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        private Node FindMin(Node node, int dim, int i)
        {
            var left = node.Left;
            var right = node.Right;
            int j = NextIndex(i);

            if (left != null)
                node = Min(FindMin(left, dim, j), node, dim);

            if (right != null && i != dim)
                node = Min(FindMin(right, dim, j), node, dim);

            return node;
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

            // callback on node if within range
            if (Vector.ApproxEquals(point, node.Point, range))
                if (!callback(node.Value)) return false;
            
            // signed distance to cut plane
            double d = point[i] - node.Point[i];
            int j = NextIndex(i);

            // recall on the containing side first
            if (d < 0.0)
                Recall(node.Left, node.Right);
            else
                Recall(node.Right, node.Left);

            void Recall(Node n0, Node n1)
            {
                BoxSearchImpl(n0, point, range, callback, j);
                
                if (Math.Abs(d) < range[i])
                    BoxSearchImpl(n1, point, range, callback, j);
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
        /// <param name="rangeSqr"></param>
        /// <param name="callback"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool RangeSearchL2Impl(Node node, double[] point, double rangeSqr, Func<T, bool> callback, int i)
        {
            if (node == null) return true;

            // enqueue value if node is within range
            if (Vector.SquareDistanceL2(point, node.Point) < rangeSqr)
                if(!callback(node.Value)) return false;

            // signed distance to cut plane
            double d = point[i] - node.Point[i];
            int j = NextIndex(i);

            // recall on the containing side first
            if (d < 0.0)
                Recall(node.Left, node.Right);
            else
                Recall(node.Right, node.Left);

            void Recall(Node n0, Node n1)
            {
                RangeSearchL2Impl(n0, point, rangeSqr, callback, j);

                if (d * d < rangeSqr)
                    RangeSearchL2Impl(n1, point, rangeSqr, callback, j);
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

            // add node if within range
            if (Vector.DistanceL1(point, node.Point) < range)
                if (!callback(node.Value)) return false;

            // signed distance to cut plane
            double d = point[i] - node.Point[i];
            int j = NextIndex(i);

            // recall on the containing side first
            if (d < 0.0)
                Recall(node.Left, node.Right);
            else
                Recall(node.Right, node.Left);

            void Recall(Node n0, Node n1)
            {
                RangeSearchL1Impl(n0, point, range, callback, j);

                if (Math.Abs(d) < range)
                    RangeSearchL1Impl(n1, point, range, callback, j);
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
            var result = new Pair<T, double>() { Item2 = double.PositiveInfinity };
            NearestL2(_root, point, 0, result);
            distance = result.Item2;
            return result.Item1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="result"></param>
        private void NearestL2(Node node, double[] point, int i, Pair<T, double> result)
        {
            if (node == null) return;

            // update nearest if necessary
            double d = Vector.SquareDistanceL2(point, node.Point);
            if (d < result.Item2)
            {
                result.Item1 = node.Value;
                result.Item2 = d;
            }

            // signed distance to cut plane
            d = point[i] - node.Point[i];
            int j = NextIndex(i);

            // recall on the containing side first
            if (d < 0.0)
                Recall(node.Left, node.Right);
            else
                Recall(node.Right, node.Left);

            void Recall(Node n0, Node n1)
            {
                NearestL2(n0, point, j, result);

                if (d * d < result.Item2)
                    NearestL2(n1, point, j, result);
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
            var result = new Pair<T, double>() { Item2 = double.PositiveInfinity };
            NearestL1(_root, point, 0, result);
            distance = result.Item2;
            return result.Item1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="result"></param>
        private void NearestL1(Node node, double[] point, int i, Pair<T, double> result)
        {
            if (node == null) return;

            // update nearest if necessary
            double d = Vector.DistanceL1(point, node.Point);
            if (d < result.Item2)
            {
                result.Item1 = node.Value;
                result.Item2 = d;
            }

            // signed distance to cut plane
            d = point[i] - node.Point[i];
            int j = NextIndex(i);

            // recall on the containing side first
            if (d < 0.0)
                Recall(node.Left, node.Right);
            else
                Recall(node.Right, node.Left);

            void Recall(Node n0, Node n1)
            {
                NearestL1(n0, point, j, result);
                
                if (Math.Abs(d) < result.Item2)
                    NearestL1(n1, point, j, result);
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
        /// Returns the nearest k values and their squared Euclidean distances.
        /// Note distances will be negative as the search uses a max priority queue.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="k"></param>
        /// <param name="result"></param>
        public void KNearestL2(double[] point, int k, PriorityQueue<double, T> result)
        {
            KNearestL2(_root, point, k, 0, result);
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

            // use negative square distance for max priority queue
            double d = -Vector.SquareDistanceL2(point, node.Point); 

            // insert node if closer than nth element
            if (result.Count < k)
                result.Insert(d, node.Value);
            else if (d > result.Min.Key)
                result.ReplaceMin(d, node.Value);

            // signed distance to cut plane
            d = point[i] - node.Point[i];
            int j = NextIndex(i);

            // recall on the containing side first
            if (d < 0.0)
                Recall(node.Left, node.Right);
            else
                Recall(node.Right, node.Left);
            
            void Recall(Node n0, Node n1)
            {
                KNearestL2(n0, point, k, j, result);
                
                if (result.Count < k || d * d < -result.Min.Key)
                    KNearestL2(n1, point, k, j, result);
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
        /// Returns the nearest k values and their Manhattan distances.
        /// Note distances will be negative as the search uses a max priority queue.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="k"></param>
        /// <param name="result"></param>
        public void KNearestL1(double[] point, int k, PriorityQueue<double, T> result)
        {
            KNearestL1(_root, point, k, 0, result);
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

            // use negative distance for max priority queue
            double d = -Vector.DistanceL1(point, node.Point);

            // insert node if closer than nth element
            if (result.Count < k)
                result.Insert(d, node.Value);
            else if (d > result.Min.Key)
                result.ReplaceMin(d, node.Value);

            // signed distance to cut plane
            d = point[i] - node.Point[i];
            int j = NextIndex(i);

            // recall on the containing side first
            if (d < 0.0)
                Recall(node.Left, node.Right);
            else
                Recall(node.Right, node.Left);
            
            void Recall(Node n0, Node n1)
            {
                KNearestL1(n0, point, k, j, result);
                
                if (result.Count < k || Math.Abs(d) < -result.Min.Key)
                    KNearestL1(n1, point, k, j, result);
            }
        }
    }
}
