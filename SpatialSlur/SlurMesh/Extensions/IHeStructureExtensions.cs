using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

/*
 * Notes 
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class IHeStructureExtensions
    {
        #region IHeStructure<V, E>

        #region Topological Search

        /// <summary>
        /// Returns the number of connected components in the graph.
        /// </summary>
        public static int CountConnectedComponents<V, E>(this IHeStructure<V, E> graph)
          where V : HeElement, IHeVertex<V, E>
          where E : HeElement, IHalfedge<V, E>
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the first halfedge from each connected component in the graph.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<E> GetConnectedComponents<V, E>(this IHeStructure<V, E> graph)
        where V : HeElement, IHeVertex<V, E>
        where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            var stack = new Stack<E>();
            int currTag = hedges.NextTag;

            // dfs
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsRemoved || he.Tag == currTag) continue;

                // flag all connected halfedges as visited
                he.Tag = currTag;
                stack.Push(he);

                while (stack.Count > 0)
                {
                    var he0 = stack.Pop();

                    // add unvisited neighbours to the stack
                    foreach (var he1 in he0.ConnectedPairs)
                    {
                        if (he1.Tag == currTag) continue;
                        he1.Tag = he1.Twin.Tag = currTag; // tag halfedge pair as visited
                        stack.Push(he1);
                    }
                }

                yield return he;
            }
        }


        /// <summary>
        /// Returns edges in breadth first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesBreadthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<E> sources)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            return graph.GetEdgesBreadthFirst(sources, Enumerable.Empty<E>());
        }


        /// <summary>
        /// Returns edges in breadth first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesBreadthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<E> sources, IEnumerable<E> exclude)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            var queue = new Queue<E>();
            int currTag = hedges.NextTag;

            // set sources
            foreach (var he in sources)
            {
                if (he.IsRemoved) continue;
                hedges.ContainsCheck(he);

                he.Tag = he.Twin.Tag = currTag;
                queue.Enqueue(he);
            }

            // exclude
            foreach (var he in exclude)
                he.Tag = he.Twin.Tag = currTag;

            // search
            while (queue.Count > 0)
            {
                var he0 = queue.Dequeue();
                yield return he0;

                foreach (var he1 in he0.ConnectedPairs)
                {
                    if (he1.Tag != currTag)
                    {
                        he1.Tag = he1.Twin.Tag = currTag;
                        queue.Enqueue(he1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns edges in depth first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesDepthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<E> sources)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            return GetEdgesDepthFirst(graph, sources, Enumerable.Empty<E>());
        }


        /// <summary>
        /// Returns edges in depth first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesDepthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<E> sources, IEnumerable<E> exclude)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            var stack = new Stack<E>();
            int currTag = hedges.NextTag;

            // set sources
            foreach (var he in sources)
            {
                if (he.IsRemoved) continue;
                hedges.ContainsCheck(he);

                he.Tag = he.Twin.Tag = currTag;
                stack.Push(he);
            }

            // exclude
            foreach (var he in exclude)
                he.Tag = he.Twin.Tag = currTag;

            // search
            while (stack.Count > 0)
            {
                var he0 = stack.Pop();
                yield return he0;

                foreach (var he1 in he0.ConnectedPairs)
                {
                    if (he1.Tag != currTag)
                    {
                        he1.Tag = he1.Twin.Tag = currTag;
                        stack.Push(he1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns edges in order of increasing priority.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="getPriority"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesPriorityFirst<V, E, T>(this IHeStructure<V, E> graph, IEnumerable<E> sources, Func<E, T> getPriority)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
            where T : IComparable<T>
        {
            return graph.GetEdgesPriorityFirst(sources, getPriority, Enumerable.Empty<E>());
        }


        /// <summary>
        /// Returns edges in order of increasing priority.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="getPriority"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesPriorityFirst<V, E, T>(this IHeStructure<V, E> graph, IEnumerable<E> sources, Func<E, T> getPriority, IEnumerable<E> exclude)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
            where T : IComparable<T>
        {
            var hedges = graph.Halfedges;

            var pq = new PriorityQueue<E>((he0, he1) => getPriority(he0).CompareTo(getPriority(he1)));
            int currTag = hedges.NextTag;

            // set sources
            foreach (var he in sources)
            {
                if (he.IsRemoved) continue;
                hedges.ContainsCheck(he);

                he.Tag = he.Twin.Tag = currTag;
                pq.Insert(he);
            }

            // exclude
            foreach (var he in exclude)
                he.Tag = he.Twin.Tag = currTag;

            // search
            while (pq.Count > 0)
            {
                var he0 = pq.RemoveMin();
                yield return he0;

                foreach (var he1 in he0.ConnectedPairs)
                {
                    if (he1.Tag != currTag)
                    {
                        he1.Tag = he1.Twin.Tag = currTag;
                        pq.Insert(he1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns vertices in breadth first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetVerticesBreadthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            return graph.GetVerticesBreadthFirst(sources, Enumerable.Empty<V>());
        }


        /// <summary>
        /// Returns vertices in breadth first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetVerticesBreadthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources, IEnumerable<V> exclude)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            var queue = new Queue<E>();
            int currTag = verts.NextTag;

            // set sources
            foreach (var v in sources)
            {
                if (v.IsRemoved) continue;
                verts.ContainsCheck(v);

                v.Tag = currTag;
                queue.Enqueue(v.FirstOut);
            }

            // exclude
            foreach (var v in exclude)
                v.Tag = currTag;

            // search
            while (queue.Count > 0)
            {
                var he = queue.Dequeue();
                yield return he;

                foreach (var he0 in he.CirculateStart)
                {
                    var he1 = he0.Twin;
                    var v1 = he1.Start;

                    if (v1.Tag != currTag)
                    {
                        v1.Tag = currTag;
                        queue.Enqueue(he1);
                    }
                }
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<V> GetVerticesBreadthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            return graph.GetVerticesBreadthFirst(sources, Enumerable.Empty<V>());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<V> GetVerticesBreadthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources, IEnumerable<V> exclude)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            var queue = new Queue<V>();
            int currTag = verts.NextTag;

            // set sources
            foreach (var v in sources)
            {
                if (v.IsRemoved) continue;
                verts.OwnsCheck(v);

                v.Tag = currTag;
                queue.Enqueue(v);
            }

            // exclude
            foreach (var v in exclude)
                v.Tag = currTag;

            // search
            while (queue.Count > 0)
            {
                var v0 = queue.Dequeue();
                yield return v0;

                foreach (var v1 in v0.ConnectedVertices)
                {
                    if (v1.Tag != currTag)
                    {
                        v1.Tag = currTag;
                        queue.Enqueue(v1);
                    }
                }
            }
        }
        */


        /// <summary>
        /// Returns vertices in depth first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetVerticesDepthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            return graph.GetVerticesDepthFirst(sources, Enumerable.Empty<V>());
        }


        /// <summary>
        /// Returns vertices in depth first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetVerticesDepthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources, IEnumerable<V> exclude)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            var stack = new Stack<E>();
            int currTag = verts.NextTag;

            // set sources
            foreach (var v in sources)
            {
                if (v.IsRemoved) continue;
                verts.ContainsCheck(v);

                v.Tag = currTag;
                stack.Push(v.FirstOut);
            }

            // exclude
            foreach (var v in exclude)
                v.Tag = currTag;

            // search
            while (stack.Count > 0)
            {
                var he = stack.Pop();
                yield return he;

                foreach (var he0 in he.CirculateStart)
                {
                    var he1 = he0.Twin;
                    var v1 = he1.Start;

                    if (v1.Tag != currTag)
                    {
                        v1.Tag = currTag;
                        stack.Push(he1);
                    }
                }
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<V> GetVerticesDepthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            return graph.GetVerticesDepthFirst(sources, Enumerable.Empty<V>());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<V> GetVerticesDepthFirst<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources, IEnumerable<V> exclude)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            var stack = new Stack<V>();
            int currTag = verts.NextTag;

            // set sources
            foreach (var v in sources)
            {
                if (v.IsRemoved) continue;
                verts.OwnsCheck(v);

                v.Tag = currTag;
                stack.Push(v);
            }

            // exclude
            foreach (var v in exclude)
                v.Tag = currTag;

            // search
            while (stack.Count > 0)
            {
                var v0 = stack.Pop();
                yield return v0;

                foreach (var v1 in v0.ConnectedVertices)
                {
                    if (v1.Tag != currTag)
                    {
                        v1.Tag = currTag;
                        stack.Push(v1);
                    }
                }
            }
        }
        */


        /// <summary>
        /// Returns vertices in order of increasing priority via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="getPriority"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetVerticesPriorityFirst<V, E, T>(this IHeStructure<V, E> graph, IEnumerable<V> sources, Func<V, T> getPriority)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
            where T : IComparable<T>
        {
            return graph.GetVerticesPriorityFirst(sources, getPriority, Enumerable.Empty<V>());
        }


        /// <summary>
        /// Returns vertices in order of increasing priority via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="getPriority"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetVerticesPriorityFirst<V, E, T>(this IHeStructure<V, E> graph, IEnumerable<V> sources, Func<V, T> getPriority, IEnumerable<V> exclude)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
            where T : IComparable<T>
        {
            var verts = graph.Vertices;

            var pq = new PriorityQueue<E>((he0, he1) => getPriority(he0.Start).CompareTo(getPriority(he1.Start)));
            int currTag = verts.NextTag;

            // set sources
            foreach (var v in sources)
            {
                if (v.IsRemoved) continue;
                verts.ContainsCheck(v);

                v.Tag = currTag;
                pq.Insert(v.FirstOut);
            }

            // exclude
            foreach (var v in exclude)
                v.Tag = currTag;

            // search
            while (pq.Count > 0)
            {
                var he = pq.RemoveMin();
                yield return he;

                foreach (var he0 in he.CirculateStart)
                {
                    var he1 = he0.Twin;
                    var v1 = he1.Start;

                    if (v1.Tag != currTag)
                    {
                        v1.Tag = currTag;
                        pq.Insert(he1);
                    }
                }
            }
        }

        #endregion


        #region Element Attributes


        #region Halfedge Attributes

        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeAngles<V, E>(this IHeStructure<V, E> graph, Func<V, Vec3d> getPosition, Action<E, double> setAngle, bool parallel = false)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i];
                    if (he.IsRemoved) continue;
                    setAngle(he, he.GetAngle(getPosition));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), body);
            else
                body(Tuple.Create(0, hedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeAngles<V, E>(this IHeStructure<V, E> graph, Func<V, Vec3d> getPosition, Func<E, double> getLength, Action<E, double> setAngle, bool parallel = false)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he0 = hedges[i];
                    if (he0.IsRemoved) continue;

                    var he1 = he0.PrevAtStart;
                    double d = getLength(he0) * getLength(he1);

                    if (d > 0.0)
                    {
                        var v0 = he0.GetDelta(getPosition);
                        var v1 = he1.GetDelta(getPosition);
                        setAngle(he0, Math.Acos(SlurMath.Clamp(v0 * v1 / d, -1.0, 1.0))); // clamp dot product to remove noise
                    }
                    else
                    {
                        setAngle(he0, Double.NaN);
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), body);
            else
                body(Tuple.Create(0, hedges.Count));
        }

        #endregion


        #region Edge Attributes

        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeDeltas<V, E>(this IHeStructure<V, E> graph, Func<V, double> getValue, Action<E, double> setDelta, bool parallel = false)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i << 1];
                    if (he.IsRemoved) continue;
                    setDelta(he, he.GetDelta(getValue));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, hedges.Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeDeltas<V, E>(this IHeStructure<V, E> graph, Func<V, Vec2d> getValue, Action<E, Vec2d> setDelta, bool parallel = false)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i << 1];
                    if (he.IsRemoved) continue;
                    setDelta(he, he.GetDelta(getValue));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, hedges.Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeDeltas<V, E>(this IHeStructure<V, E> graph, Func<V, Vec3d> getValue, Action<E, Vec3d> setDelta, bool parallel = false)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i << 1];
                    if (he.IsRemoved) continue;
                    setDelta(he, he.GetDelta(getValue));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, hedges.Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetEdgeLengthSum<V, E>(this IHeStructure<V, E> graph, Func<V, Vec3d> getPosition)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;
            var sum = 0.0;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsRemoved) continue;

                sum += he.GetLength(getPosition);
            }

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeLengths<V, E>(this IHeStructure<V, E> graph, Func<V, Vec3d> getPosition, Action<E, double> setLength, bool parallel = false)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i << 1];
                    if (he.IsRemoved) continue;
                    setLength(he, he.GetLength(getPosition));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, hedges.Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeTangents<V, E>(this IHeStructure<V, E> graph, Func<V, Vec3d> getPosition, Action<E, Vec3d> setTangent, bool parallel = false)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i << 1];
                    if (he.IsRemoved) continue;

                    var d = he.GetDelta(getPosition);
                    d.Unitize();

                    setTangent(he, d);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, hedges.Count >> 1));
        }


        /// <summary>
        /// Returns the connected component index of each edge in the mesh.
        /// Also returns the number of connected components.
        /// </summary>
        public static int GetEdgeComponentIndices<V, E>(this IHeStructure<V, E> graph, Action<E, int> setIndex)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            var stack = new Stack<E>();
            int currTag = hedges.NextTag;
            int currComp = 0;

            // edge DFS search
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];

                // unused edges don't belong to any component
                if (he.IsRemoved)
                {
                    setIndex(he, -1);
                    continue;
                }

                // already visited
                if (he.Tag == currTag) continue;

                // add first halfedge to the stack and flag as visited
                stack.Push(he);
                he.Tag = currTag;

                while (stack.Count > 0)
                {
                    var he0 = stack.Pop();
                    setIndex(he0, currComp);

                    // add unvisited neighbours to the stack
                    foreach (var he1 in he0.ConnectedPairs)
                    {
                        if (he1.Tag == currTag) continue;
                        he1.Tag = he1.Twin.Tag = currTag; // tag halfedge pair as visited
                        stack.Push(he1);
                    }
                }

                currComp++;
            }

            return currComp;
        }


        /// <summary>
        /// Calculates the minimum topological depth of each edge from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetEdgeDepths<V, E>(this IHeStructure<V, E> graph, IEnumerable<E> sources, Func<E, int> getDepth, Action<E, int> setDepth)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;
            var queue = new Queue<E>();

            // set depths to max
            foreach (var he in hedges)
                setDepth(he, int.MaxValue);

            // enqueue sources and set to 0
            foreach (var he in sources)
            {
                hedges.ContainsCheck(he);
                if (he.IsRemoved) continue;

                setDepth(he, 0);
                queue.Enqueue(he);
            }

            // bfs
            while (queue.Count > 0)
            {
                var he0 = queue.Dequeue();
                int t0 = getDepth(he0) + 1;

                foreach (var he1 in he0.ConnectedPairs)
                {
                    if (t0 < getDepth(he1))
                    {
                        setDepth(he1, t0);
                        queue.Enqueue(he1);
                    }
                }
            }
        }

        #endregion


        #region Vertex Attributes

        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexDegrees<V, E>(this IHeStructure<V, E> graph, Action<V, int> setDegree, bool parallel = false)
           where V : HeElement, IHeVertex<V, E>
           where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;
                    setDegree(v, v.Degree);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this IHeStructure<V, E> graph, Func<V, double> getValue, Action<V, double> setLaplace, bool parallel = false)
           where V : HeElement, IHeVertex<V, E>
           where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = verts[i];
                    if (v0.IsRemoved) continue;

                    var t = getValue(v0);
                    var sum = 0.0;
                    int n = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += getValue(v1) - t;
                        n++;
                    }

                    setLaplace(v0, sum / n);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this IHeStructure<V, E> graph, Func<V, Vec2d> getValue, Action<V, Vec2d> setLaplace, bool parallel = false)
           where V : HeElement, IHeVertex<V, E>
           where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = verts[i];
                    if (v0.IsRemoved) continue;

                    var t = getValue(v0);
                    var sum = new Vec2d();
                    int n = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += getValue(v1) - t;
                        n++;
                    }

                    setLaplace(v0, sum / n);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this IHeStructure<V, E> graph, Func<V, Vec3d> getValue, Action<V, Vec3d> setLaplace, bool parallel = false)
           where V : HeElement, IHeVertex<V, E>
           where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = verts[i];
                    if (v0.IsRemoved) continue;

                    var t0 = getValue(v0);
                    var sum = new Vec3d();
                    int n = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += getValue(v1) - t0;
                        n++;
                    }

                    setLaplace(v0, sum / n);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this IHeStructure<V, E> graph, Func<V, double> getValue, Func<E, double> getWeight, Action<V, double> setLaplace, bool parallel = false)
           where V : HeElement, IHeVertex<V, E>
           where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    var t = getValue(v);
                    var sum = 0.0;

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (getValue(he.End) - t) * getWeight(he);

                    setLaplace(v, sum);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this IHeStructure<V, E> graph, Func<V, Vec2d> getValue, Func<E, double> getWeight, Action<V, Vec2d> setLaplace, bool parallel = false)
           where V : HeElement, IHeVertex<V, E>
           where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    var t = getValue(v);
                    var sum = new Vec2d();

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (getValue(he.End) - t) * getWeight(he);

                    setLaplace(v, sum);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this IHeStructure<V, E> graph, Func<V, Vec3d> getValue, Func<E, double> getWeight, Action<V, Vec3d> setLaplace, bool parallel = false)
           where V : HeElement, IHeVertex<V, E>
           where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    var t = getValue(v);
                    var sum = new Vec3d();

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (getValue(he.End) - t) * getWeight(he);

                    setLaplace(v, sum);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Normalizes halfedge weights such that the weights of outgoing halfedges around each vertex sum to 1.
        /// Note that this breaks weight symmetry between halfedge pairs.
        /// </summary>
        public static void NormalizeHalfedgeWeightsAtVertices<V, E>(this IHeStructure<V, E> graph, Func<E, double> getWeight, Action<E, double> setWeight, bool parallel = false)
           where V : HeElement, IHeVertex<V, E>
           where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;
                    v.OutgoingHalfedges.Normalize(getWeight, setWeight);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates the morse smale classification for each vertex (0 = normal, 1 = minima, 2 = maxima, 3 = saddle).
        /// Assumes halfedges are radially sorted around the given vertices.
        /// </summary>
        public static void GetVertexMorseSmaleLabels<V, E>(this IHeStructure<V, E> graph, Func<V, double> getValue, Action<V, int> setLabel, bool parallel = false)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = verts[i];
                    double t0 = getValue(v0);

                    // check first neighbour
                    var he0 = v0.FirstOut.Twin;
                    double t1 = getValue(he0.Start);

                    bool last = (t1 < t0); // was the last neighbour lower?
                    int count = 0; // number of discontinuities
                    he0 = he0.NextAtStart.Twin;

                    // circulate remaining neighbours
                    var he1 = he0;
                    do
                    {
                        t1 = getValue(he1.Start);

                        if (t1 < t0)
                        {
                            if (!last) count++;
                            last = true;
                        }
                        else
                        {
                            if (last) count++;
                            last = false;
                        }

                        he1 = he1.NextAtStart.Twin;
                    } while (he1 != he0);

                    // classify current vertex based on number of discontinuities
                    switch (count)
                    {
                        case 0:
                            setLabel(v0, (last) ? 2 : 1);
                            break;
                        case 2:
                            setLabel(v0, 0);
                            break;
                        default:
                            setLabel(v0, 3);
                            break;
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates the minimum topological depth of each vertex from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetVertexDepths<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources, Func<V, int> getDepth, Action<V, int> setDepth)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;
            var queue = new Queue<V>();

            // set depths to max
            foreach (var v in verts)
                setDepth(v, int.MaxValue);

            // enqueue sources and set to zero
            foreach (var v in sources)
            {
                verts.ContainsCheck(v);
                v.RemovedCheck();

                setDepth(v, 0);
                queue.Enqueue(v);
            }

            // bfs
            while (queue.Count > 0)
            {
                var v0 = queue.Dequeue();
                int t0 = getDepth(v0) + 1;

                foreach (var v1 in v0.ConnectedVertices)
                {
                    if (t0 < getDepth(v1))
                    {
                        setDepth(v1, t0);
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the minimum topological distance to each vertex from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetVertexDistances<V, E>(this IHeStructure<V, E> graph, IEnumerable<V> sources, Func<E, double> getLength, Func<V, double> getDistance, Action<V, double> setDistance)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;
            var queue = new Queue<V>();

            // set to max distance
            foreach (var v in verts)
                setDistance(v, double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (var v in sources)
            {
                verts.ContainsCheck(v);
                v.RemovedCheck();

                setDistance(v, 0.0);
                queue.Enqueue(v);
            }

            // TODO compare to priority queue implementation
            while (queue.Count > 0)
            {
                var v0 = queue.Dequeue();
                double t0 = getDistance(v0);

                foreach (var he in v0.IncomingHalfedges)
                {
                    var v1 = he.Start;
                    double t1 = t0 + getLength(he);

                    if (t1 < getDistance(v1))
                    {
                        setDistance(v1, t1);
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the entries of the incidence matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetVertexIncidenceMatrix<V, E>(this IHeStructure<V, E> graph, double[] result)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var hedges = graph.Halfedges;

            int nv = graph.Vertices.Count;
            int ne = hedges.Count >> 1;
            Array.Clear(result, 0, nv * ne);

            for (int i = 0; i < ne; i++)
            {
                var he = hedges[i >> 1];
                if (he.IsRemoved) continue;

                int j = i * nv;
                result[j + he.Start.Index] = result[j + he.End.Index] = 1.0;
            }
        }


        /// <summary>
        /// Returns the entries of the adjacency matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetVertexAdjacencyMatrix<V, E>(this IHeStructure<V, E> graph, double[] result)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v0 = verts[i];
                if (v0.IsRemoved) continue;

                foreach (var v1 in v0.ConnectedVertices)
                    result[i * nv + v1.Index] = 1.0;
            }
        }


        /// <summary>
        /// Calculates the Laplacian matrix in column-major order.
        /// </summary>
        public static void GetVertexLaplacianMatrix<V, E>(this IHeStructure<V, E> graph, double[] result)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsRemoved) continue;

                double wsum = 0.0;

                foreach (var he in v.OutgoingHalfedges)
                {
                    result[i * nv + he.End.Index] = 1.0;
                    wsum++;
                }

                result[i + i * nv] = -wsum;
            }
        }


        /// <summary>
        /// Calculates the Laplacian matrix in column-major order.
        /// </summary>
        public static void GetVertexLaplacianMatrix<V, E>(this IHeStructure<V, E> graph, Func<E, double> getWeight, double[] result)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
        {
            var verts = graph.Vertices;

            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsRemoved) continue;

                double wsum = 0.0;

                foreach (var he in v.OutgoingHalfedges)
                {
                    double w = getWeight(he);
                    result[i * nv + he.End.Index] = w;
                    wsum += w;
                }

                result[i * nv + i] = -wsum;
            }
        }

        #endregion

        #endregion

        #endregion


        #region IHeStructure<V, E, F>

        #region Topological Search

        /// <summary>
        /// Returns faces in breadth first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetFacesBreadthFirst<V, E, F>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            return mesh.GetFacesBreadthFirst(sources, Enumerable.Empty<F>());
        }


        /// <summary>
        /// Returns faces in breadth first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetFacesBreadthFirst<V, E, F>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources, IEnumerable<F> exclude)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            var queue = new Queue<E>();
            int currTag = faces.NextTag;

            // set sources
            foreach (var f in sources)
            {
                if (f.IsRemoved) continue;
                faces.ContainsCheck(f);

                f.Tag = currTag;
                queue.Enqueue(f.First);
            }

            // exclude
            foreach (var f in exclude)
                f.Tag = currTag;

            // search
            while (queue.Count > 0)
            {
                var he = queue.Dequeue();
                yield return he;

                foreach (var he0 in he.CirculateFace)
                {
                    var he1 = he0.Twin;
                    var f1 = he1.Face;

                    if (f1 != null && f1.Tag != currTag)
                    {
                        f1.Tag = currTag;
                        queue.Enqueue(he1);
                    }
                }
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        internal static IEnumerable<F> GetFacesBreadthFirst<V, E, F>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            var queue = new Queue<F>();
            int currTag = faces.NextTag;

            foreach (var f in sources)
            {
                if (f.IsRemoved) continue;
                faces.OwnsCheck(f);

                f.Tag = currTag;
                queue.Enqueue(f);
            }

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
        */


        /// <summary>
        /// Returns faces in depth first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetFacesDepthFirst<V, E, F>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            return mesh.GetFacesDepthFirst(sources, Enumerable.Empty<F>());
        }


        /// <summary>
        /// Returns faces in depth first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetFacesDepthFirst<V, E, F>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources, IEnumerable<F> exclude)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            var stack = new Stack<E>();
            int currTag = faces.NextTag;

            // set sources
            foreach (var f in sources)
            {
                if (f.IsRemoved) continue;
                faces.ContainsCheck(f);

                f.Tag = currTag;
                stack.Push(f.First);
            }

            // exclude
            foreach (var f in exclude)
                f.Tag = currTag;

            // search
            while (stack.Count > 0)
            {
                var he = stack.Pop();
                yield return he;

                foreach (var he0 in he.CirculateFace)
                {
                    var he1 = he0.Twin;
                    var f1 = he1.Face;

                    if (f1 != null && f1.Tag != currTag)
                    {
                        f1.Tag = currTag;
                        stack.Push(he1);
                    }
                }
            }
        }


        /*
       /// <summary>
       /// 
       /// </summary>
       /// <typeparam name="E"></typeparam>
       /// <typeparam name="V"></typeparam>
       /// <typeparam name="F"></typeparam>
       /// <param name="mesh"></param>
       /// <param name="sources"></param>
       /// <returns></returns>
       internal static IEnumerable<F> GetFacesDepthFirst<V, E, F>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources)
       where V : HeElement, IHeVertex<V, E, F>
       where E : HeElement, IHalfedge<V, E, F>
       where F : HeElement, IHeFace<V, E, F>
       {
           var faces = mesh.Faces;

           var stack = new Stack<F>();
           int currTag = faces.NextTag;

           foreach (var f in sources)
           {
               if (f.IsRemoved) continue;
               faces.OwnsCheck(f);

               f.Tag = currTag;
               stack.Push(f);
           }

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
       */


        /// <summary>
        /// Returns faces in order of increasing priority via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="getPriority"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetFacesPriorityFirst<V, E, F, T>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<F, T> getPriority)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
            where T : IComparable<T>
        {
            return mesh.GetFacesPriorityFirst(sources, getPriority, Enumerable.Empty<F>());
        }


        /// <summary>
        /// Returns faces in order of increasing priority via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="getPriority"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetFacesPriorityFirst<V, E, F, T>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<F, T> getPriority, IEnumerable<F> exclude)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
            where T : IComparable<T>
        {
            var faces = mesh.Faces;

            var pq = new PriorityQueue<E>((he0, he1) => getPriority(he0.Face).CompareTo(getPriority(he1.Face)));
            int currTag = faces.NextTag;

            // set sources
            foreach (var f in sources)
            {
                if (f.IsRemoved) continue;
                faces.ContainsCheck(f);

                f.Tag = currTag;
                pq.Insert(f.First);
            }

            // exclude
            foreach (var f in exclude)
                f.Tag = currTag;

            // search
            while (pq.Count > 0)
            {
                var he = pq.RemoveMin();
                yield return he;

                foreach (var he0 in he.CirculateFace)
                {
                    var he1 = he0.Twin;
                    var f1 = he1.Face;

                    if (f1 != null && f1.Tag != currTag)
                    {
                        f1.Tag = currTag;
                        pq.Insert(he1);
                    }
                }
            }
        }

        #endregion


        #region Element Attributes

        #region Halfedge Attributes

        /// <summary>
        /// Calculates the area associated with each halfedge.
        /// This is calculated as W in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static void GetHalfedgeAreas<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<F, Vec3d> getCenter, Action<E, double> setAreas, bool parallel = false)
          where V : HeElement, IHeVertex<V, E, F>
          where E : HeElement, IHalfedge<V, E, F>
          where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i];
                    if (he.IsRemoved || he.Face == null) continue;
                    setAreas(he, he.GetArea(getPosition, getCenter));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), body);
            else
                body(Tuple.Create(0, hedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeNormals<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<E, Vec3d> setNormal, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i];
                    if (he.IsRemoved) continue;
                    setNormal(he, he.GetNormal(getPosition));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), body);
            else
                body(Tuple.Create(0, hedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeUnitNormals<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<E, Vec3d> setNormal, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i];
                    if (he.IsRemoved) continue;

                    Vec3d v = he.GetNormal(getPosition);
                    v.Unitize();

                    setNormal(he, v);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), body);
            else
                body(Tuple.Create(0, hedges.Count));
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>
        public static void GetDihedralAngles<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<F, Vec3d> getNormal, Action<E, double> setAngle, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i << 1];
                    if (he.IsRemoved || he.IsBoundary) continue;
                    setAngle(he, he.GetDihedralAngle(getPosition, getNormal));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, hedges.Count >> 1));
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite each halfedge.
        /// Assumes triangular faces.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static void GetHalfedgeCotangents<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<E, double> setCotangent, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = hedges[i];
                    if (he.IsRemoved || he.Face == null) continue;
                    setCotangent(he, he.GetCotangent(getPosition));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), body);
            else
                body(Tuple.Create(0, hedges.Count));
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each halfedge.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetHalfedgeCotanWeights<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<E, double> setWeight, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he0 = hedges[j];
                    if (he0.IsRemoved) continue;

                    var he1 = hedges[j + 1];
                    double w = 0.0;

                    if (he0.Face != null) w += he0.GetCotangent(getPosition);
                    if (he1.Face != null) w += he1.GetCotangent(getPosition);
                    w *= 0.5;

                    setWeight(he0, w / getArea(he0.Start));
                    setWeight(he1, w / getArea(he1.Start));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, hedges.Count >> 1));
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each halfedge along with the barycentric dual area of each vertex.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetHalfedgeCotanWeights<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<V, double> setArea, Func<E, double> getWeight, Action<E, double> setWeight)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            // clear areas
            foreach (var v in mesh.Vertices)
                setArea(v, 0.0);

            // accumulate cotangent weights and vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsRemoved) continue;

                var he1 = hedges[i + 1];
                var v0 = he0.Start;
                var v1 = he1.Start;

                double w = 0.0;
                if (he0.Face != null) w += GetWeight(he0);
                if (he1.Face != null) w += GetWeight(he1);
                setWeight(he0, w * 0.5);

                double GetWeight(E hedge)
                {
                    const double t = 0.5 / 3.0;
                    var v = hedge.PrevInFace.Start;

                    var p = getPosition(v);
                    var d0 = getPosition(v0) - p;
                    var d1 = getPosition(v1) - p;

                    double a = Vec3d.Cross(d0, d1).Length;
                    setArea(v, getArea(v) + a * t);

                    return d0 * d1 / a;
                }
            }

            // normalize weights by vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsRemoved) continue;

                var he1 = hedges[i + 1];
                double w = getWeight(he0);

                setWeight(he0, w / getArea(he0.Start));
                setWeight(he1, w / getArea(he1.Start));
            }
        }

        #endregion


        #region Edge Attributes

        /// <summary>
        /// Calculates the minimum topological depth of each edge from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetEdgeDepths<V, E, F>(this IHeStructure<V, E, F> mesh, IEnumerable<E> sources, Func<E, int> getDepth, Action<E, int> setDepth)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;
            var queue = new Queue<E>();

            // set to max depth
            foreach (var he in hedges)
                setDepth(he, int.MaxValue);

            // enqueue sources and set to 0
            foreach (var he in sources)
            {
                hedges.ContainsCheck(he);
                he.RemovedCheck();

                setDepth(he, 0);
                queue.Enqueue(he);
            }

            // bfs
            while (queue.Count > 0)
            {
                var he0 = queue.Dequeue();
                int t0 = getDepth(he0) + 1;

                foreach (var he1 in he0.ConnectedPairs)
                {
                    if (t0 < getDepth(he1))
                    {
                        setDepth(he1, t0);
                        if (he1.Face != null) queue.Enqueue(he1); // only enqueue the halfedge if it has a face
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the cotangent weight for each edge (symmetric).
        /// Based on Pinkall and Polthier's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetEdgeCotanWeights<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<E, double> setWeight, bool parallel = false)
        where V : HeElement, IHeVertex<V, E, F>
        where E : HeElement, IHalfedge<V, E, F>
        where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he0 = hedges[j];
                    if (he0.IsRemoved) continue;

                    var he1 = hedges[j + 1];
                    double w = 0.0;

                    if (he0.Face != null) w += he0.GetCotangent(getPosition);
                    if (he1.Face != null) w += he1.GetCotangent(getPosition);

                    setWeight(he0, w * 0.5);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, hedges.Count >> 1));
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each edge (symmetric).
        /// Based on Levy and Vallet's derivation of the symmetric Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetEdgeCotanWeights<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<E, double> setWeight, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he0 = hedges[j];
                    if (he0.IsRemoved) continue;

                    var he1 = hedges[j + 1];
                    double w = 0.0;

                    if (he0.Face != null) w += he0.GetCotangent(getPosition);
                    if (he1.Face != null) w += he1.GetCotangent(getPosition);
                    w *= 0.5 / Math.Sqrt(getArea(he0.Start) * getArea(he1.Start));

                    setWeight(he0, w);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, hedges.Count >> 1));
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each edge (symmetric) along with the barycentric dual area of each vertex.
        /// Based on Levy and Vallet's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetEdgeCotanWeights<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<V, double> setArea, Func<E, double> getWeight, Action<E, double> setWeight)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            // clear areas
            foreach (var v in mesh.Vertices)
                setArea(v, 0.0);

            // accumulate cotangent weights and vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsRemoved) continue;

                var he1 = hedges[i + 1];
                var v0 = he0.Start;
                var v1 = he1.Start;

                double w = 0.0;
                if (he0.Face != null) w += GetWeight(he0);
                if (he1.Face != null) w += GetWeight(he1);
                setWeight(he0, w * 0.5);

                double GetWeight(E hedge)
                {
                    const double t = 0.5 / 3.0;
                    var v = hedge.PrevInFace.Start;

                    var p = getPosition(v);
                    var d0 = getPosition(v0) - p;
                    var d1 = getPosition(v1) - p;

                    double a = Vec3d.Cross(d0, d1).Length;
                    setArea(v, getArea(v) + a * t);

                    return d0 * d1 / a;
                }
            }

            // symmetrically normalize weights by vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsRemoved) continue;

                double w = getWeight(he0) / Math.Sqrt(getArea(he0.Start) * getArea(hedges[i + 1].Start));
                setWeight(he0, w);
            }
        }

        #endregion


        #region Vertex Attributes

        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexAreas<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<F, Vec3d> getCenter, Action<V, double> setArea, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;
                    double sum = 0.0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        if (he.Face != null)
                            sum += he.GetArea(getPosition, getCenter);
                    }

                    setArea(v, sum);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexAreas<V, E, F>(this IHeStructure<V, E, F> mesh, Func<E, double> getArea, Action<V, double> setArea, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;
                    setArea(v, v.OutgoingHalfedges.Sum(getArea));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates the barycentric dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasBarycentric<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<V, double> setArea)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            const double t = 1.0 / 6.0; // (1.0 / 3.0) * 0.5

            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            // clear areas
            foreach (var v in verts)
                setArea(v, 0.0);

            // distribute face areas to vertices
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsRemoved) continue;

                double a = f.First.GetNormal(getPosition).Length * t;

                foreach (var v in f.Vertices)
                    setArea(v, getArea(v) + a);
            }
        }


        /// <summary>
        /// Calculates the circumcentric dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasCircumcentric<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<V, double> setArea)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Calculates the mixed dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasMixed<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<V, double> setArea)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Calculates the circle packing radii for each vertex.
        /// Assumes the mesh is a circle packing mesh http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        public static void GetVertexCirclePackingRadii<V, E, F>(this IHeStructure<V, E, F> mesh, Func<E, double> getLength, Action<V, double> setRadius, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue; // skip unused vertices

                    double sum = 0.0;
                    int n = 0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        if (he.Face == null) continue; // skip boundary edges
                        sum += (getLength(he) + getLength(he.PrevInFace) - getLength(he.NextInFace)) * 0.5;
                        n++;
                    }

                    setRadius(v, sum / n);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculated as the signed magnitude of the vertex laplacian with respect to the vertex normal.
        /// http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 47)
        /// </summary>
        public static void GetVertexMeanCurvature<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getLaplace, Action<V, double> setCurvature, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    if (v.IsBoundary)
                        setCurvature(v, 0.0);
                    else
                        setCurvature(v, getLaplace(v).Length * -0.5);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculated as the signed magnitude of the vertex laplacian with respect to the vertex normal.
        /// http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 47)
        /// </summary>
        public static void GetVertexMeanCurvatureSigned<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getLaplace, Func<V, Vec3d> getNormal, Action<V, double> setCurvature, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    if (v.IsBoundary)
                    {
                        setCurvature(v, 0.0);
                    }
                    else
                    {
                        var lap = getLaplace(v);
                        setCurvature(v, Math.Sign(getNormal(v) * lap) * lap.Length * -0.5);
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculated as the angle defect around each vertex.
        /// </summary>
        public static void GetVertexGaussianCurvature<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<V, double> setCurvature, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    if (v.IsBoundary)
                    {
                        setCurvature(v, 0.0);
                    }
                    else
                    {
                        double sum = 0.0;
                        foreach (var he in v.OutgoingHalfedges)
                            sum += he.GetAngle(getPosition);

                        setCurvature(v, (SlurMath.TwoPI - sum) / getArea(v));
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculated as the angle defect around each vertex.
        /// </summary>
        public static void GetVertexGaussianCurvature<V, E, F>(this IHeStructure<V, E, F> mesh, Func<E, double> getAngle, Func<V, double> getArea, Action<V, double> setCurvature, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    if (v.IsBoundary)
                    {
                        setCurvature(v, 0.0);
                    }
                    else
                    {
                        double sum = 0.0;
                        foreach (var he in v.OutgoingHalfedges)
                            sum += getAngle(he);

                        setCurvature(v, (SlurMath.TwoPI - sum) / getArea(v));
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates vertex normals as the area-weighted sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        public static void GetVertexNormals<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<V, Vec3d> setNormal, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;
                    setNormal(v, v.GetNormal(getPosition));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }


        /// <summary>
        /// Calculates vertex normals as the sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        public static void GetVertexNormals<V, E, F>(this IHeStructure<V, E, F> mesh, Func<E, Vec3d> getNormal, Action<V, Vec3d> setNormal, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;
                    setNormal(v, v.GetNormal(getNormal));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), body);
            else
                body(Tuple.Create(0, verts.Count));
        }

        #endregion


        #region Face Attributes

        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceDegrees<V, E, F>(this IHeStructure<V, E, F> mesh, Action<F, int> setDegree, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    setDegree(f, f.Degree);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Normalizes halfedge weights such that the weights of halfedges within each face sum to 1.
        /// Note that this breaks weight symmetry between halfedge pairs.
        /// </summary>
        public static void NormalizeHalfedgeWeightsInFaces<V, E, F>(this IHeStructure<V, E, F> mesh, Func<E, double> getWeight, Action<E, double> setWeight, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    f.Halfedges.Normalize(getWeight, setWeight);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates the barycenter of each face.
        /// </summary>
        public static void GetFaceBarycenters<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    setCenter(f, f.GetBarycenter(getPosition));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates the circumcenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceCircumcenters<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    setCenter(f, f.GetCircumcenter(getPosition));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates the incenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceIncenters<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    setCenter(f, f.GetIncenter(getPosition));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Returns the incenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceIncenters<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<E, double> getLength, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    setCenter(f, f.GetIncenter(getPosition, getLength));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates face normals as the area-weighted sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        public static void GetFaceNormals<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setNormal, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    setNormal(f, f.GetNormal(getPosition));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates face normals as the sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        public static void GetFaceNormals<V, E, F>(this IHeStructure<V, E, F> mesh, Func<E, Vec3d> getNormal, Action<F, Vec3d> setNormal, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    setNormal(f, f.GetNormal(getNormal));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates face normals as the normal of the first halfedge in each face.
        /// Face normals are unitized by default.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceNormalsTri<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setNormal, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;

                    Vec3d n = f.First.GetNormal(getPosition);
                    n.Unitize();

                    setNormal(f, n);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceAreas<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<F, Vec3d> getCenter, Action<F, double> setArea, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    var he0 = f.First;

                    if (he0 == he0.NextInFace.NextInFace.NextInFace)
                    {
                        // simplified tri case
                        Vec3d n = he0.GetNormal(getPosition);
                        setArea(f, n.Length * 0.5);
                    }
                    else
                    {
                        // general ngon case
                        Vec3d cen = getCenter(f);
                        double sum = 0.0;

                        foreach (var he in f.Halfedges)
                            sum += he.GetArea(getPosition, cen);

                        setArea(f, sum);
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates the area of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceAreasTri<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, double> setArea, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;
                    setArea(f, f.First.GetNormal(getPosition).Length * 0.5);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Returns the planar deviation for each face.
        /// </summary>
        public static void GetFacePlanarity<V, E, F>(this IHeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, double> setPlanarity, bool parallel = false)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsRemoved) continue;

                    var he0 = f.First;
                    var he1 = he0;

                    // tri case
                    if (he1 == he0)
                    {
                        setPlanarity(f, 0.0);
                        continue;
                    }

                    var p0 = getPosition(he1.Start); he1 = he1.NextInFace;
                    var p1 = getPosition(he1.Start); he1 = he1.NextInFace;
                    var p2 = getPosition(he1.Start); he1 = he1.NextInFace;

                    if (he1.NextInFace == he0)
                    {
                        // quad case
                        var p3 = getPosition(he1.Start);
                        setPlanarity(f, GeometryUtil.LineLineShortestVector(p0, p2, p1, p3).Length);
                    }
                    else
                    {
                        // ngon case
                        var he2 = he1;
                        double sum = 0.0;
                        int count = 0;

                        do
                        {
                            var p3 = getPosition(he2.Start);
                            sum += GeometryUtil.LineLineShortestVector(p0, p2, p1, p3).Length;
                            count++;

                            // advance to next set of verts
                            p0 = p1; p1 = p2; p2 = p3;
                            he2 = he2.NextInFace;
                        } while (he2 != he1);

                        setPlanarity(f, sum / count);
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), body);
            else
                body(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates the minimum topological depth of all faces connected to a set of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetFaceDepths<V, E, F>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<F, int> getDepth, Action<F, int> setDepth)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;
            var queue = new Queue<F>();

            // set depths to max
            foreach (var f in faces)
                setDepth(f, int.MaxValue);

            // enqueue sources and set to zero
            foreach (var f in sources)
            {
                faces.ContainsCheck(f);
                f.RemovedCheck();

                setDepth(f, 0);
                queue.Enqueue(f);
            }

            // bfs
            while (queue.Count > 0)
            {
                var f0 = queue.Dequeue();
                int t0 = getDepth(f0) + 1;

                foreach (var f1 in f0.AdjacentFaces)
                {
                    if (t0 < getDepth(f1))
                    {
                        setDepth(f1, t0);
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the minimum topological distance to each face from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetFaceDistances<V, E, F>(this IHeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<E, double> getLength, Func<F, double> getDistance, Action<F, double> setDistance)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;
            var queue = new Queue<F>();

            // set distances to max
            foreach (var f in faces)
                setDistance(f, double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (var f in sources)
            {
                faces.ContainsCheck(f);
                f.RemovedCheck();

                setDistance(f, 0.0);
                queue.Enqueue(f);
            }

            // TODO compare to priority queue implementation
            while (queue.Count > 0)
            {
                var f0 = queue.Dequeue();
                double t0 = getDistance(f0);

                foreach (var he in f0.Halfedges)
                {
                    var f1 = he.Twin.Face;
                    if (f1 == null) continue;

                    double t1 = t0 + getLength(he);

                    if (t1 < getDistance(f1))
                    {
                        setDistance(f1, t1);
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the entries of the incidence matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetFaceIncidenceMatrix<V, E, F>(this IHeStructure<V, E, F> mesh, double[] result)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            int nf = mesh.Faces.Count;
            int ne = hedges.Count >> 1;
            Array.Clear(result, 0, nf * ne);

            for (int i = 0; i < ne; i++)
            {
                var he = hedges[i >> 1];
                if (he.IsRemoved) continue;

                int j = i * nf;
                result[j + he.Start.Index] = result[j + he.End.Index] = 1.0;
            }
        }


        /// <summary>
        /// Returns the entries of the adjacency matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetFaceAdjacencyMatrix<V, E, F>(this IHeStructure<V, E, F> mesh, double[] result)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            int nf = faces.Count;
            Array.Clear(result, 0, nf * nf);

            for (int i = 0; i < nf; i++)
            {
                var f0 = faces[i];
                if (f0.IsRemoved) continue;

                foreach (var f1 in f0.AdjacentFaces)
                    result[i * nf + f1.Index] = 1.0;
            }
        }


        /// <summary>
        /// Calculates the Laplacian matrix in column-major order.
        /// </summary>
        public static void GetFaceLaplacianMatrix<V, E, F>(this IHeStructure<V, E, F> mesh, double[] result)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            int nf = faces.Count;
            Array.Clear(result, 0, nf * nf);

            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];
                if (f.IsRemoved) continue;

                double wsum = 0.0;

                foreach (var he in f.Halfedges)
                {
                    result[i * nf + he.End.Index] = 1.0;
                    wsum++;
                }

                result[i + i * nf] = -wsum;
            }
        }


        /// <summary>
        /// Calculates the Laplacian matrix in column-major order.
        /// </summary>
        public static void GetFaceLaplacianMatrix<V, E, F>(this IHeStructure<V, E, F> mesh, Func<E, double> getWeight, double[] result)
            where V : HeElement, IHeVertex<V, E, F>
            where E : HeElement, IHalfedge<V, E, F>
            where F : HeElement, IHeFace<V, E, F>
        {
            var faces = mesh.Faces;

            int nf = faces.Count;
            Array.Clear(result, 0, nf * nf);

            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];
                if (f.IsRemoved) continue;

                double wsum = 0.0;

                foreach (var he in f.Halfedges)
                {
                    double w = getWeight(he);
                    result[i * nf + he.End.Index] = w;
                    wsum += w;
                }

                result[i * nf + i] = -wsum;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
