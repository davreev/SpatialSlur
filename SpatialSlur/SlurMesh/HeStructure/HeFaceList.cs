using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="EE"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="VV"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="FF"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class HeFaceList<S, EE, VV, FF, E, V, F> : HeElementList<S, F>
        where S : HeStructure<S, EE, VV, FF, E, V, F>
        where EE : HalfedgeList<S, EE, VV, FF, E, V, F>
        where VV : HeVertexList<S, EE, VV, FF, E, V, F>
        where FF : HeFaceList<S, EE, VV, FF, E, V, F>
        where E : Halfedge<E, V, F>
        where V : HeVertex<E, V, F>
        where F : HeFace<E, V, F>
    {
        /// <summary>
        /// 
        /// </summary>
        internal F Add()
        {
            var f = CreateElement();
            Add(f);
            return f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public IEnumerable<F> GetBreadthFirstOrder(F start)
        {
            OwnsCheck(start);

            if (start.IsUnused)
                yield break;

            var queue = new Queue<F>();
            int currTag = NextTag;

            queue.Enqueue(start);
            start.Tag = currTag;

            while (queue.Count > 0)
            {
                var f0 = queue.Dequeue();
                yield return f0;

                foreach (var f1 in f0.AdjacentFaces)
                {
                    if (f1.Tag != currTag)
                    {
                        f1.Tag = currTag;
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public IEnumerable<F> GetDepthFirstOrder(F start)
        {
            OwnsCheck(start);

            if (start.IsUnused)
                yield break;

            var stack = new Stack<F>();
            int currTag = NextTag;

            stack.Push(start);
            start.Tag = currTag;

            while (stack.Count > 0)
            {
                var f0 = stack.Pop();
                yield return f0;

                foreach (var f1 in f0.AdjacentFaces)
                {
                    if (f1.Tag != currTag)
                    {
                        f1.Tag = currTag;
                        stack.Push(f1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public IEnumerable<F> BreadthFirstSearch(F start, int maxDepth)
        {
            // TODO
            throw new NotImplementedException();

            // use double queue approach
            // see KdTree.MinDepth
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public IEnumerable<F> DepthFirstSearch(F start, int maxDepth)
        {
            // TODO
            throw new NotImplementedException();

            /*
            OwnsCheck(start);

            if (start.IsUnused)
                yield break;

            var stack = new Stack<F>();
            int currTag = NextTag;

            stack.Push(start);
            start.Tag = currTag;

            while (stack.Count > 0)
            {
                var f0 = stack.Pop();
                yield return f0;

                foreach (var f1 in f0.AdjacentFaces)
                {
                    if (f1.Tag != currTag)
                    {
                        f1.Tag = currTag;
                        stack.Push(f1);
                    }
                }
            }
            */
        }


        /// <summary>
        /// Orients each face such that the first halfedge returns the minimum value for the given function.
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="parallel"></param>
        public void OrientFacesToMin(Comparison<E> comparer, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = this[i];
                    if (f.IsUnused) continue;
                    f.First = f.Halfedges.SelectMin(comparer);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Orients each face such that the first halfedge returns the minimum value for the given function.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="parallel"></param>
        public void OrientFacesToMin(Func<E,double> selector, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = this[i];
                    if (f.IsUnused) continue;
                    f.First = f.Halfedges.SelectMin(selector);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        public void OrientFacesToBoundary(bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = this[i];
                    if (f.IsUnused) continue;
                    f.SetFirstToBoundary();
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Counts the number of faces adjacent to both given faces.
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public int CountCommonNeighbours(F f0, F f1)
        {
            OwnsCheck(f0);
            OwnsCheck(f1);

            if (f0.IsUnused || f1.IsUnused)
                return 0;

            return CountCommonNeighboursImpl(f0, f1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        internal int CountCommonNeighboursImpl(F f0, F f1)
        {
            int currTag = NextTag;

            // flag neighbours of f1
            foreach (var f in f1.AdjacentFaces)
                f.Tag = currTag;

            // count flagged neighbours of f0
            int count = 0;
            foreach (var f in f0.AdjacentFaces)
                if (f.Tag == currTag) count++;

            return count;
        }


        /// <summary>
        /// Returns all faces adjacent to both given faces.
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public IEnumerable<F> GetCommonNeighbours(F f0, F f1)
        {
            OwnsCheck(f0);
            OwnsCheck(f1);

            if (f0.IsUnused || f1.IsUnused)
                return Enumerable.Empty<F>();

            return GetCommonNeighboursImpl(f0, f1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        internal IEnumerable<F> GetCommonNeighboursImpl(F f0, F f1)
        {
            int currTag = NextTag;

            // flag neighbours of f1
            foreach (var f in f1.AdjacentFaces)
                f.Tag = currTag;

            // count flagged neighbours of f0
            foreach (var f in f0.AdjacentFaces)
                if (f.Tag == currTag) yield return f;
        }


        /// <summary>
        /// Counts the number of vertices shared by the two given faces.
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public int CountCommonVertices(F f0, F f1)
        {
            OwnsCheck(f0);
            OwnsCheck(f1);

            if (f0.IsUnused || f1.IsUnused)
                return 0;

            return CountCommonVerticesImpl(f0, f1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        internal int CountCommonVerticesImpl(F f0, F f1)
        {
            int currTag = Owner.Vertices.NextTag;

            // flag neighbours of f1
            foreach (var v in f1.Vertices)
                v.Tag = currTag;

            // count flagged neighbours of f0
            int count = 0;
            foreach (var v in f0.Vertices)
                if (v.Tag == currTag) count++;

            return count;
        }


        /// <summary>
        /// Returns all vertices shared by both given faces.
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public IEnumerable<V> GetCommonVertices(F f0, F f1)
        {
            OwnsCheck(f0);
            OwnsCheck(f1);

            if (f0.IsUnused || f1.IsUnused)
                return Enumerable.Empty<V>();

            return GetCommonVerticesImpl(f0, f1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        internal IEnumerable<V> GetCommonVerticesImpl(F f0, F f1)
        {
            int currTag = Owner.Vertices.NextTag;

            // flag neighbours of f1
            foreach (var v in f1.Vertices)
                v.Tag = currTag;

            // count flagged neighbours of f0
            foreach (var v in f0.Vertices)
                if (v.Tag == currTag) yield return v;
        }


        #region Attributes

        /// <summary>
        /// Calculates the topological depth of all faces connected to a set of sources.
        /// Assumes a travel cost of 1 for all edges.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetFaceDepths(IEnumerable<F> sources, IList<int> result)
        {
            var queue = new Queue<F>();
            result.SetRange(Int32.MaxValue, 0, Count);

            // enqueue sources and set to zero
            foreach (var f in sources)
            {
                OwnsCheck(f);
                if (f.IsUnused) continue;

                result[f.Index] = 0;
                queue.Enqueue(f);
            }

            // bfs
            while (queue.Count > 0)
            {
                var f0 = queue.Dequeue();
                int t0 = result[f0.Index] + 1;

                foreach (var f1 in f0.AdjacentFaces)
                {
                    int i1 = f1.Index;
                    if (t0 < result[i1])
                    {
                        result[i1] = t0;
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the topological distance of all faces connected to a set of sources.
        /// Travel cost is defined per edge.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeCosts"></param>
        /// <param name="result"></param>
        public void GetFaceDistances(IEnumerable<F> sources, IReadOnlyList<double> edgeCosts, IList<double> result)
        {
            var queue = new Queue<F>();
            result.SetRange(Double.PositiveInfinity, 0, Count);

            // enqueue sources and set to zero
            foreach (var f in sources)
            {
                OwnsCheck(f);
                if (f.IsUnused) continue;

                result[f.Index] = 0.0;
                queue.Enqueue(f);
            }

            // TODO compare to priority queue implementation
            while (queue.Count > 0)
            {
                var f0 = queue.Dequeue();
                double t0 = result[f0.Index];

                foreach (var he in f0.Halfedges)
                {
                    var f1 = he.Twin.Face;
                    if (f1 == null) continue;

                    int i1 = f1.Index;
                    double t1 = t0 + edgeCosts[he.Index >> 1];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the topological distance of all faces connected to a set of sources.
        /// Travel cost is defined per half-edge allowing for directed representations.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="halfedgeCosts"></param>
        /// <param name="result"></param>
        public void GetFaceDistancesDirected(IEnumerable<F> sources, IReadOnlyList<double> halfedgeCosts, IList<double> result)
        {
            var queue = new Queue<F>();
            result.SetRange(Double.PositiveInfinity, 0, Count);

            // enqueue sources and set to zero
            foreach (var f in sources)
            {
                OwnsCheck(f);
                if (f.IsUnused) continue;

                result[f.Index] = 0.0;
                queue.Enqueue(f);
            }

            // TODO compare to priority queue implementation
            while (queue.Count > 0)
            {
                var f0 = queue.Dequeue();
                double t0 = result[f0.Index];

                foreach (var he in f0.Halfedges)
                {
                    var f1 = he.Twin.Face;
                    if (f1 == null) continue;

                    int i1 = f1.Index;
                    double t1 = t0 + halfedgeCosts[he.Index];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the entries of the incidence matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public void GetIncidenceMatrix(double[] result)
        {
            var hedges = Owner.Halfedges;

            int n = Count;
            int ne = hedges.Count >> 1;
            Array.Clear(result, 0, n * ne);

            for (int i = 0; i < ne; i++)
            {
                var he = hedges[i >> 1];
                if (he.IsUnused) continue;

                int j = i * n;
                result[j + he.Start.Index] = result[j + he.End.Index] = 1.0;
            }
        }


        /// <summary>
        /// Returns the entries of the adjacency matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public void GetAdjacencyMatrix(double[] result)
        {
            int n = Count;
            Array.Clear(result, 0, n * n);

            for (int i = 0; i < n; i++)
            {
                var f0 = this[i];
                if (f0.IsUnused) continue;

                foreach (var f1 in f0.AdjacentFaces)
                    result[i * n + f1.Index] = 1.0;
            }
        }


        /// <summary>
        /// Calculates the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="result"></param>
        public void GetLaplacianMatrix(double[] result)
        {
            int n = Count;
            Array.Clear(result, 0, n * n);

            for (int i = 0; i < n; i++)
            {
                var f = this[i];
                if (f.IsUnused) continue;

                double wsum = 0.0;

                foreach (var he in f.Halfedges)
                {
                    result[i * n + he.End.Index] = 1.0;
                    wsum++;
                }

                result[i + i * n] = -wsum;
            }
        }


        /// <summary>
        /// Calculates the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        public void GetLaplacianMatrix(IReadOnlyList<double> halfedgeWeights, double[] result)
        {
            int n = Count;
            Array.Clear(result, 0, n * n);

            for (int i = 0; i < n; i++)
            {
                var f = this[i];
                if (f.IsUnused) continue;

                double wsum = 0.0;

                foreach (var he in f.Halfedges)
                {
                    double w = halfedgeWeights[he.Index];
                    result[i * n + he.End.Index] = w;
                    wsum += w;
                }

                result[i * n + i] = -wsum;
            }
        }

        #endregion
    }
}
