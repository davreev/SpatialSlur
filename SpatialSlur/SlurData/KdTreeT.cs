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
 * TODO Compare to array-based implementation
 * 
 * References
 * https://www.cs.umd.edu/class/spring2008/cmsc420/L19.kd-trees.pdf
 * http://www.cs.umd.edu/~meesh/420/Notes/MountNotes/lecture18-kd2.pdf
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Implementation of a generic k-dimensional binary search tree.
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
        private static Node Min(Node n0, Node n1, int dim)
        {
            return (n1.Point[dim] < n0.Point[dim]) ? n1 : n0;
        }


        /// <summary>
        /// Inserts point value pairs in a way that produces a more balanced tree.
        /// </summary>
        public static KdTree<T> CreateBalanced(IList<VecKd> points, IList<T> values)
        {
            if (points.Count != values.Count)
                throw new ArgumentException("Must provide an equal number of points and values.");

            KdTree<T> result = new KdTree<T>(points[0].K);
            result._root = result.InsertBalanced(points, values, 0, points.Count - 1, 0);
            result._n = points.Count;

            return result;
        }

        #endregion


        private Node _root;
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
            return GetMinDepth(_root, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private int GetMinDepth(Node node, int i)
        {
            if (node == null) return i;
            return Math.Min(GetMinDepth(node.Left, i + 1), GetMinDepth(node.Right, i + 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private void DimCheck(VecKd point)
        {
            if (_k != point.K)
                throw new System.ArgumentException("The given point must have the same number of dimensions as this tree.");
        }


        /// <summary>
        /// Returns true if the tree contains the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(VecKd point)
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
        public bool Contains(VecKd point, out T value)
        {
            DimCheck(point);
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
        private Node Find(Node node, VecKd point, int i)
        {
            if (node == null) 
                return null;

            if (point.Equals(node.Point, _epsilon)) 
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
        public void Insert(VecKd point, T value)
        {
            DimCheck(point);
            _root = Insert(_root, point, value, 0);
            _n++;
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <param name="i"></param>
        private Node Insert(Node node, VecKd point, T value, int i)
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
        /// <param name="points"></param>
        /// <param name="values"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private Node InsertBalanced(IList<VecKd> points, IList<T> values, int from, int to, int i)
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
            CompareKd comparer = new CompareKd(i, _epsilon);
            VecKd pt = points.QuickSelect(mid, from, to, comparer, values);

            // make sure there's no duplicate elements to the left of the median
            int j = from;
            while(j < mid)
            {
                if (comparer.Compare(points[j], pt) == 0)
                    points.Swap(j, --mid);
                else
                    j++;
            }

            // check dimension and create node 
            DimCheck(points[mid]);
            Node node = new Node(points[mid], values[mid]);

            // recurse on left and right children
            node.Left = InsertBalanced(points, values, from, mid - 1, i + 1);
            node.Right = InsertBalanced(points, values, mid + 1, to, i + 1);
            return node;
        }


        /// <summary> 
        /// Removes the first node in the tree which is equal to the given point.
        /// </summary>
        /// <param name="point"></param>
        public bool Remove(VecKd point)
        {
            DimCheck(point);

            int n = _n;
            _root = Remove(_root, point, 0);
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
        private Node Remove(Node node, VecKd point, int i)
        {
            if (node == null)
                return null;

            // wrap dimension
            if (i == _k) i = 0;

            if (point.Equals(node.Point, _epsilon))
            {
                // found the node to delete
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
                    node = null; // node is a leaf, can safely delete
                    _n--; // decrement node count
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
                // recurse on left subtree tree only
                if (node.Left == null)
                    return node;
                else
                    return FindMin(node.Left, dim, i + 1);
            }
            else
            {
                // recurse on both subtrees if they exist
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
        /// Returns all values within the given Euclidean distance.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        public List<T> EuclideanSearch(VecKd point, double range)
        {
            DimCheck(point);

            List<T> result = new List<T>();
            EuclideanSearch(_root, point, range * range, result, 0);
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
        private void EuclideanSearch(Node node, VecKd point, double range, List<T> result, int i)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // enqueue value if node is within range
            if (point.SquareDistanceTo(node.Point) < range) 
                result.Add(node.Value);

            double d = point[i] - node.Point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                EuclideanSearch(node.Left, point, range, result, i + 1); // recurse in left subtree
                if (d * d < range) EuclideanSearch(node.Right, point, range, result, i + 1); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                EuclideanSearch(node.Right, point, range, result, i + 1); // recurse in right subtree
                if (d * d < range) EuclideanSearch(node.Left, point, range, result, i + 1); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns all values within the given box.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        public List<T> BoxSearch(VecKd point, VecKd range)
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
        private void BoxSearch(Node node, VecKd point, VecKd range, List<T> result, int i)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if within range
            if (point.Equals(node.Point, range))
                result.Add(node.Value);

            // if left side of tree
            double d = point[i] - node.Point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                BoxSearch(node.Left, point, range, result, i + 1); // recurse in left subtree
                if (Math.Abs(d) < range[i]) BoxSearch(node.Right, point, range, result, i + 1); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                BoxSearch(node.Right, point, range, result, i + 1); // recurse in right subtree
                if (Math.Abs(d) < range[i]) BoxSearch(node.Left, point, range, result, i + 1); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns all values within the given Manhattan distance.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="range"></param>
        public List<T> ManhattanSearch(VecKd point, double range)
        {
            DimCheck(point);

            List<T> result = new List<T>();
            ManhattanSearch(_root, point, range, result, 0);
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
        private void ManhattanSearch(Node node, VecKd point, double range, List<T> result, int i)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if within range
            if (point.ManhattanDistanceTo(node.Point) < range)
                result.Add(node.Value);

            double d = point[i] - node.Point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                ManhattanSearch(node.Left, point, range, result, i + 1); // recurse in left subtree
                if (Math.Abs(d) < range) ManhattanSearch(node.Right, point, range, result, i + 1); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                ManhattanSearch(node.Right, point, range, result, i + 1); // recurse in right subtree
                if (Math.Abs(d) < range) ManhattanSearch(node.Left, point, range, result, i + 1); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Euclidean distance metric.
        /// If the tree is empty the default value of T is returned.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T EuclideanNearest(VecKd point)
        {
            DimCheck(point);

            NearestHelper result = new NearestHelper();
            EuclideanNearest(_root, point, 0, result);

            return result.Value;
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Euclidean distance metric.
        /// If the tree is empty the default value of T is returned.
        /// Also returns the distance to the nearest value as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public T EuclideanNearest(VecKd point, out double distance)
        {
            DimCheck(point);

            NearestHelper result = new NearestHelper();
            EuclideanNearest(_root, point, 0, result);

            distance = Math.Sqrt(result.Distance);
            return result.Value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="result"></param>
        /// <param name="i"></param>
        private void EuclideanNearest(Node node, VecKd point, int i, NearestHelper result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // update nearest if necessary
            double d = point.SquareDistanceTo(node.Point);
            if (d < result.Distance)
            {
                result.Value = node.Value;
                result.Distance = d;
            }
            
            d = point[i] - node.Point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                EuclideanNearest(node.Left, point, i + 1, result); // recurse in left subtree first
                if (d * d < result.Distance) 
                    EuclideanNearest(node.Right, point, i + 1, result); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                EuclideanNearest(node.Right, point, i + 1, result); // recurse in right subtree first
                if (d * d < result.Distance) 
                    EuclideanNearest(node.Left, point, i + 1, result); // recurse in left subtree only if necessary
            }
        }

         
        /// <summary>
        /// Returns the nearest n values in the tree using a Euclidean distance metric.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public T[] EuclideanNearestN(VecKd point, int n)
        {
            DimCheck(point);

            if (n < 1 || n > _n)
                throw new ArgumentException("n must be greater than 0 and less than or equal the number of points in the tree");

            SortedList<double, T> result = new SortedList<double, T>(new DuplicateComparer<double>());
            EuclideanNearestN(_root, point, n, 0, result);
            
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
        public T[] EuclideanNearestN(VecKd point, int n, out double[] distances)
        {
            DimCheck(point);

            if (n < 1 || n > _n)
                throw new ArgumentException("n must be greater than 0 and less than or equal the number of points in the tree");

            SortedList<double, T> result = new SortedList<double, T>(new DuplicateComparer<double>());
            EuclideanNearestN(_root, point, n, 0, result);

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
        private void EuclideanNearestN(Node node, VecKd point, int n, int i, SortedList<double, T> result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if closer than nth element
            double d = point.SquareDistanceTo(node.Point);
            if (result.Count < n || d < result.Keys[n-1])
                result.Add(d, node.Value);

            d = point[i] - node.Point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                EuclideanNearestN(node.Left, point, n, i + 1, result); // recurse in left subtree first
                if (result.Count < n || d * d < result.Keys[n - 1]) 
                    EuclideanNearestN(node.Right, point, n, i + 1, result); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                EuclideanNearestN(node.Right, point, n, i + 1, result); // recurse in right subtree first
                if (result.Count < n || d * d < result.Keys[n - 1]) 
                    EuclideanNearestN(node.Left, point, n, i + 1, result); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Manhattan distance metric.
        /// If the tree is empty the default value of T is returned.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ManhattanNearest(VecKd point)
        {
            DimCheck(point);

            NearestHelper result = new NearestHelper();
            ManhattanNearest(_root, point, 0, result);

            return result.Value;
        }


        /// <summary>
        /// Returns the nearest value in the tree using a Manhattan distance metric.
        /// If the tree is empty the default value of T is returned.
        /// Also returns the distance to the nearest value as an out parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public T ManhattanNearest(VecKd point, out double distance)
        {
            DimCheck(point);

            NearestHelper result = new NearestHelper();
            ManhattanNearest(_root, point, 0, result);

            distance = result.Distance;
            return result.Value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="point"></param>
        /// <param name="result"></param>
        /// <param name="i"></param>
        private void ManhattanNearest(Node node, VecKd point, int i, NearestHelper result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // update nearest if necessary
            double d = point.ManhattanDistanceTo(node.Point);
            if (d < result.Distance)
            {
                result.Value = node.Value;
                result.Distance = d;
            }

            d = point[i] - node.Point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                ManhattanNearest(node.Left, point, i + 1, result); // recurse in left subtree first
                if (Math.Abs(d) < result.Distance) 
                    ManhattanNearest(node.Right, point, i + 1, result); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                ManhattanNearest(node.Right, point,  i + 1, result); // recurse in right subtree first
                if (Math.Abs(d) < result.Distance) 
                    ManhattanNearest(node.Left, point, i + 1, result); // recurse in left subtree only if necessary
            }
        }


        /// <summary>
        /// Returns the nearest n values in the tree using a Manhattan distance metric.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public T[] ManhattanNearestN(VecKd point, int n)
        {
            DimCheck(point);

            if (n < 1 || n > _n)
                throw new ArgumentException("n must be greater than 0 and less than or equal the number of points in the tree");

            SortedList<double, T> result = new SortedList<double, T>(new DuplicateComparer<double>());
            ManhattanNearestN(_root, point, n, 0, result);

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
        public T[] ManhattanNearestN(VecKd point, int n, out double[] distances)
        {
            DimCheck(point);

            if (n < 1 || n > _n)
                throw new ArgumentException("n must be greater than 0 and less than or equal the number of points in the tree");

            SortedList<double, T> result = new SortedList<double, T>(new DuplicateComparer<double>());
            ManhattanNearestN(_root, point, n, 0, result);

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
        private void ManhattanNearestN(Node node, VecKd point, int n, int i, SortedList<double, T> result)
        {
            if (node == null) return;

            // wrap dimension
            if (i == _k) i = 0;

            // add node if closer than nth element
            double d = point.ManhattanDistanceTo(node.Point);
            if (result.Count < n || d < result.Keys[n - 1])
                result.Add(d, node.Value);

            d = point[i] - node.Point[i]; // signed distance to cut plane
            if (d < 0.0)
            {
                // point is in left subtree
                ManhattanNearestN(node.Left, point, n, i + 1, result); // recurse in left subtree first
                if (result.Count < n || Math.Abs(d) < result.Keys[n - 1]) 
                    ManhattanNearestN(node.Right, point, n, i + 1, result); // recurse in right subtree only if necessary
            }
            else
            {
                // point is in right subtree
                ManhattanNearestN(node.Right, point, n, i + 1, result); // recurse in right subtree first
                if (result.Count < n || Math.Abs(d) < result.Keys[n - 1]) 
                    ManhattanNearestN(node.Left, point, n, i + 1, result); // recurse in left subtree only if necessary
            }
        }

       
        /// <summary>
        /// 
        /// </summary>
        private class Node
        {
            private Node _left, _right;
            private readonly VecKd _point;
            private readonly T _value;


            /// <summary>
            /// 
            /// </summary>
            public Node Left
            {
                get { return _left; }
                set { _left = value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public Node Right
            {
                get { return _right; }
                set { _right = value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public VecKd Point
            {
                get { return _point; }
            }


            /// <summary>
            /// 
            /// </summary>
            public T Value
            {
                get { return _value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public bool IsLeaf
            {
                get { return _left == null && _right == null; }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="point"></param>
            /// <param name="value"></param>
            public Node(VecKd point, T value)
            {
                _point = point;
                _value = value;
            }
        }


        /// <summary>
        /// Used to cache relevant information during a nearest search.
        /// </summary>
        private class NearestHelper
        {
            private T _value;
            private double _distance = Double.PositiveInfinity;

 
            /// <summary>
            /// 
            /// </summary>
            public T Value
            {
                get { return _value; }
                set { _value = value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public double Distance
            {
                get { return _distance; }
                set { _distance = value; }
            }
        }


        /// <summary>
        /// Comparer that treats equality as greater than.
        /// Used to avoid errors related to duplicate keys.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        private class DuplicateComparer<U> : IComparer<U> 
            where U : IComparable
        {
            public int Compare(U x, U y)
            {
                int result = x.CompareTo(y);
                return (result == 0) ? 1 : result;
            }
        }

    }
}
