
/*
 * Notes 
 * 
 * TODO consider alternatives to property delegates for return values
 * Should return IEnumerable where possible for Linq-like API
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

using SpatialSlur.Collections;
using SpatialSlur.Meshes.Impl;

using static SpatialSlur.Meshes.Delegates;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeStructureExtensions
    {
        #region HeStructure<V, E>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="transform"></param>
        /// <param name="parallel"></param>
        public static void Transform<V, E>(this HeStructure<V, E> graph, Transform3d transform, bool parallel = false)
            where V : HeStructure<V, E>.Vertex, IPosition3d, INormal3d
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;

            var rotate = transform.Rotation;
            var invScale = 1.0 / transform.Scale;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    v.Position = transform.Apply(v.Position);
                    v.Normal = rotate.Apply(v.Normal * invScale);
                }
            }
        }


        /// <summary>
        /// Returns the first halfedge from each connected component in the graph.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<E> GetConnectedComponents<V, E>(this HeStructure<V, E> graph)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var hedges = graph.Halfedges;

            var stack = new Stack<E>();
            int currTag = hedges.NextTag;

            // dfs
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused || he.Tag == currTag) continue;

                // flag all connected halfedges as visited
                he.Tag = currTag;
                stack.Push(he);

                while (stack.Count > 0)
                {
                    var he0 = stack.Pop();

                    TryPush(he0.Previous);
                    TryPush(he0.Next);

                    he0 = he0.Twin;

                    TryPush(he0.Previous);
                    TryPush(he0.Next);
                }

                yield return he;
            }
            
            void TryPush(E hedge)
            {
                if (hedge.Tag != currTag)
                {
                    hedge.Tag = hedge.Twin.Tag = currTag;
                    stack.Push(hedge);
                }
            }
        }


        /// <summary>
        /// Returns edges in breadth-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesBreadthFirst<V, E>(this HeStructure<V, E> graph, IEnumerable<E> sources, IEnumerable<E> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var hedges = graph.Halfedges;

            var queue = new Queue<E>();
            int currTag = hedges.NextTag;

            // set sources
            foreach (var he in sources)
            {
                if (he.IsUnused) continue;
                hedges.OwnsCheck(he);

                he.Tag = he.Twin.Tag = currTag;
                queue.Enqueue(he);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var he in exclude)
                    he.Tag = he.Twin.Tag = currTag;
            }

            // search
            while (queue.Count > 0)
            {
                var he0 = queue.Dequeue();
                yield return he0;
                
                TryEnqueue(he0.Previous);
                TryEnqueue(he0.Next);

                he0 = he0.Twin;

                TryEnqueue(he0.Previous);
                TryEnqueue(he0.Next);
            }

            void TryEnqueue(E hedge)
            {
                if (hedge.Tag != currTag)
                {
                    hedge.Tag = hedge.Twin.Tag = currTag;
                    queue.Enqueue(hedge);
                }
            }
        }


        /// <summary>
        /// Returns edges in depth-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesDepthFirst<V, E>(this HeStructure<V, E> graph, IEnumerable<E> sources, IEnumerable<E> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var hedges = graph.Halfedges;

            var stack = new Stack<E>();
            int currTag = hedges.NextTag;

            // set sources
            foreach (var he in sources)
            {
                if (he.IsUnused) continue;
                hedges.OwnsCheck(he);

                he.Tag = he.Twin.Tag = currTag;
                stack.Push(he);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var he in exclude)
                    he.Tag = he.Twin.Tag = currTag;
            }

            // search
            while (stack.Count > 0)
            {
                var he0 = stack.Pop();
                yield return he0;

                TryPush(he0.Previous);
                TryPush(he0.Next);

                he0 = he0.Twin;

                TryPush(he0.Previous);
                TryPush(he0.Next);
            }

            void TryPush(E hedge)
            {
                if (hedge.Tag != currTag)
                {
                    hedge.Tag = hedge.Twin.Tag = currTag;
                    stack.Push(hedge);
                }
            }
        }


        /// <summary>
        /// Returns edges in best-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="getKey"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesBestFirst<V, E, K>(this HeStructure<V, E> graph, IEnumerable<E> sources, Func<E, K> getKey, IEnumerable<E> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            var hedges = graph.Halfedges;

            var pq = new PriorityQueue<K, E>();
            int currTag = hedges.NextTag;

            // set sources
            foreach (var he in sources)
            {
                if (he.IsUnused) continue;
                hedges.OwnsCheck(he);

                he.Tag = he.Twin.Tag = currTag;
                pq.Insert(getKey(he), he);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var he in exclude)
                    he.Tag = he.Twin.Tag = currTag;
            }

            // search
            while (pq.Count > 0)
            {
                var he0 = pq.RemoveMin().Value;
                yield return he0;
                
                TryInsert(he0.Previous);
                TryInsert(he0.Next);

                he0 = he0.Twin;

                TryInsert(he0.Previous);
                TryInsert(he0.Next);
            }

            void TryInsert(E hedge)
            {
                if (hedge.Tag != currTag)
                {
                    hedge.Tag = hedge.Twin.Tag = currTag;
                    pq.Insert(getKey(hedge), hedge);
                }
            }
        }

        
        /// <summary>
        /// Returns vertices in breadth-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<V> GetVerticesBreadthFirst<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, IEnumerable<V> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            return GetVerticesBreadthFirst2(graph, sources, exclude).Select(he => he.Start);
        }


        /// <summary>
        /// Returns vertices in breadth-first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetVerticesBreadthFirst2<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, IEnumerable<V> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;

            var queue = new Queue<E>();
            int currTag = verts.NextTag;

            // set sources
            foreach (var v in sources)
            {
                if (v.IsUnused) continue;
                verts.OwnsCheck(v);

                v.Tag = currTag;
                queue.Enqueue(v.First);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var v in exclude)
                    v.Tag = currTag;
            }

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


        /// <summary>
        /// Returns vertices in depth-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<V> GetVerticesDepthFirst<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, IEnumerable<V> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            return GetVerticesDepthFirst2(graph, sources, exclude).Select(he => he.Start);
        }


        /// <summary>
        /// Returns vertices in depth-first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetVerticesDepthFirst2<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, IEnumerable<V> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;

            var stack = new Stack<E>();
            int currTag = verts.NextTag;

            // set sources
            foreach (var v in sources)
            {
                if (v.IsUnused) continue;
                verts.OwnsCheck(v);

                v.Tag = currTag;
                stack.Push(v.First);
            }

            // exclude
            if(exclude != null)
            {
                foreach (var v in exclude)
                    v.Tag = currTag;
            }

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


        /// <summary>
        /// Returns vertices in best-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="getKey"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<V> GetVerticesBestFirst<V, E, K>(this HeStructure<V, E> graph, IEnumerable<V> sources, Func<V, K> getKey, IEnumerable<V> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            return GetVerticesBestFirst2(graph, sources, he => getKey(he.Start), exclude).Select(he => he.Start);
        }


        /// <summary>
        /// Returns vertices in best-first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="getKey"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetVerticesBestFirst2<V, E, K>(this HeStructure<V, E> graph, IEnumerable<V> sources, Func<E, K> getKey, IEnumerable<V> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            var verts = graph.Vertices;

            var pq = new PriorityQueue<K, E>();
            int currTag = verts.NextTag;

            // set sources
            foreach (var v in sources)
            {
                if (v.IsUnused) continue;
                verts.OwnsCheck(v);

                v.Tag = currTag;
                var he = v.First;
                pq.Insert(getKey(he), he);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var v in exclude)
                    v.Tag = currTag;
            }

            // search
            while (pq.Count > 0)
            {
                var he = pq.RemoveMin().Value;
                yield return he;

                foreach (var he0 in he.CirculateStart)
                {
                    var he1 = he0.Twin;
                    var v1 = he1.Start;

                    if (v1.Tag != currTag)
                    {
                        v1.Tag = currTag;
                        pq.Insert(getKey(he1), he1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static double GetEdgeLengthSum<V, E>(this HeStructure<V, E> graph)
            where V : HeStructure<V, E>.Vertex, IPosition3d
            where E : HeStructure<V, E>.Halfedge
        {
            return GetEdgeLengthSum(graph, Position3d<V>.Get);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetEdgeLengthSum<V, E>(this HeStructure<V, E> graph, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var hedges = graph.Halfedges;
            var sum = 0.0;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (!he.IsUnused)
                    sum += he.GetLength(getPosition);
            }

            return sum;
        }


        /// <summary>
        /// Returns the connected component index of each edge in the mesh.
        /// Also returns the number of connected components.
        /// </summary>
        public static int GetEdgeComponentIndices<V, E>(this HeStructure<V, E> graph, Action<E, int> setIndex)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
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
                if (he.IsUnused)
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
                    
                    TryPush(he0.Previous);
                    TryPush(he0.Next);

                    he0 = he0.Twin;

                    TryPush(he0.Previous);
                    TryPush(he0.Next);
                }

                currComp++;
            }
            
            void TryPush(E hedge)
            {
                if (hedge.Tag != currTag)
                {
                    hedge.Tag = hedge.Twin.Tag = currTag;
                    stack.Push(hedge);
                }
            }

            return currComp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void GetEdgeDepths<V,E>(this HeStructure<V, E> graph, IEnumerable<E> sources, ArrayView<int> result, IEnumerable<E> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var prop = Halfedge<E>.CreateEdgeProperty(result);
            GetEdgeDepths(graph, sources, prop, exclude);
        }


        /// <summary>
        /// Calculates the minimum topological depth of each edge from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetEdgeDepths<V, E>(this HeStructure<V, E> graph, IEnumerable<E> sources, Property<E, int> depth, IEnumerable<E> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var edges = graph.Edges;
            var queue = new Queue<E>();

            // set depths to max
            foreach (var he in edges)
                depth.Set(he, int.MaxValue);

            // enqueue sources and set to 0
            foreach (var he in sources)
            {
                edges.OwnsCheck(he);
                if (he.IsUnused) continue;

                depth.Set(he, 0);
                queue.Enqueue(he);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var he in exclude)
                    depth.Set(he, 0);
            }

            // bfs
            while (queue.Count > 0)
            {
                var he0 = queue.Dequeue();
                int d0 = depth.Get(he0) + 1;

                TryEnqueue(he0.Previous);
                TryEnqueue(he0.Next);

                he0 = he0.Twin;

                TryEnqueue(he0.Previous);
                TryEnqueue(he0.Next);

                void TryEnqueue(E hedge)
                {
                    if (d0 < depth.Get(hedge))
                    {
                        depth.Set(hedge, d0);
                        queue.Enqueue(hedge);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the morse smale classification for each vertex (0 = normal, 1 = minima, 2 = maxima, 3 = saddle).
        /// Assumes halfedges are radially sorted around the given vertices (note that this will always be the case with HeMesh types).
        /// </summary>
        public static void GetVertexMorseSmaleLabels<V, E>(this HeStructure<V, E> graph, Func<V, double> getValue, Action<V, int> setLabel, bool parallel = false)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v0 = verts[i];
                    double t0 = getValue(v0);

                    // check first neighbour
                    var he0 = v0.First.Twin;
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
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void GetVertexDepths<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, ArrayView<int> result, IEnumerable<V> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var prop = Node<V>.CreateProperty(result);
            GetVertexDepths(graph, sources, prop, exclude);
        }


        /// <summary>
        /// Calculates the minimum topological depth of each vertex from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetVertexDepths<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, Property<V, int> depth, IEnumerable<V> exclude = null)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;
            var queue = new Queue<V>();

            // set depths to max
            foreach (var v in verts)
                depth.Set(v, int.MaxValue);

            // enqueue sources and set to zero
            foreach (var v in sources)
            {
                v.UnusedCheck();
                verts.OwnsCheck(v);

                depth.Set(v, 0);
                queue.Enqueue(v);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var v in exclude)
                    depth.Set(v, 0);
            }

            // bfs
            while (queue.Count > 0)
            {
                var v0 = queue.Dequeue();
                int t0 = depth.Get(v0) + 1;

                foreach (var v1 in v0.ConnectedVertices)
                {
                    if (t0 < depth.Get(v1))
                    {
                        depth.Set(v1, t0);
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="getLength"></param>
        /// <param name="result"></param>
        /// <param name="bestFirst"></param>
        public static void GetVertexDistances<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, Func<E, double> getLength, ArrayView<double> result, bool bestFirst = false)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var prop = Node<V>.CreateProperty(result);
            GetVertexDistances(graph, sources, getLength, prop, bestFirst);
        }


        /// <summary>
        /// Calculates the minimum topological distance to each vertex from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetVertexDistances<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, Func<E, double> getLength, Property<V, double> distance, bool bestFirst = false)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            if (bestFirst)
                GetVertexDistancesBestFirst(graph, sources, getLength, distance);
            else
                GetVertexDistancesBreadthFirst(graph, sources, getLength, distance);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetVertexDistancesBreadthFirst<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, Func<E, double> getLength, Property<V, double> distance)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;
            var queue = new Queue<V>();

            // set to max distance
            foreach (var v in verts)
                distance.Set(v, double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (var v in sources)
            {
                v.UnusedCheck();
                verts.OwnsCheck(v);

                distance.Set(v, 0.0);
                queue.Enqueue(v);
            }
            
            // bfs from sources
            while (queue.Count > 0)
            {
                var v0 = queue.Dequeue();
                double d0 = distance.Get(v0);

                foreach (var he in v0.IncomingHalfedges)
                {
                    var v1 = he.Start;
                    double d1 = d0 + getLength(he);

                    if (d1 < distance.Get(v1))
                    {
                        distance.Set(v1, d1);
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetVertexDistancesBestFirst<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, Func<E, double> getLength, Property<V, double> distance)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;
            var pq = new PriorityQueue<double, V>();

            // set all to max distance
            foreach (var v in verts)
                distance.Set(v, double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (var v in sources)
            {
                v.UnusedCheck();
                verts.OwnsCheck(v);

                distance.Set(v, 0.0);
                pq.Insert(0.0, v);
            }

            // best first search from sources
            while (pq.Count > 0)
            {
                (var d0, var v0) = pq.RemoveMin();
                if (d0 > distance.Get(v0)) continue; // skip if lower value was already assigned

                foreach (var he in v0.IncomingHalfedges)
                {
                    var v1 = he.Start;
                    double d1 = d0 + getLength(he);

                    if (d1 < distance.Get(v1))
                    {
                        distance.Set(v1, d1);
                        pq.Insert(d1, v1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="normal"></param>
        /// <param name="start"></param>
        public static void UnifyVertexNormals<V, E>(this HeStructure<V, E> graph, Property<V, Vector3d> normal, V start)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            foreach (var he in graph.GetVerticesBreadthFirst2(start.Yield()))
            {
                var v = he.Start;
                var n = normal.Get(v);

                if (Vector3d.Dot(n, normal.Get(he.End)) < 0.0)
                    normal.Set(v, -n);
            }
        }

        #endregion


        #region HeStructure<V, E, F>

        /// <summary>
        /// Returns faces in breadth-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<F> GetFacesBreadthFirst<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, IEnumerable<F> exclude = null)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetFacesBreadthFirst2(mesh, sources, exclude).Select(he => he.Face);
        }


        /// <summary>
        /// Returns faces in breadth-first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetFacesBreadthFirst2<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, IEnumerable<F> exclude = null)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;

            var queue = new Queue<E>();
            int currTag = faces.NextTag;

            // set sources
            foreach (var f in sources)
            {
                if (f.IsUnused) continue;
                faces.OwnsCheck(f);

                f.Tag = currTag;
                queue.Enqueue(f.First);
            }

            // exclude
            if(exclude != null)
            {
                foreach (var f in exclude)
                    f.Tag = currTag;
            }
         
            // search
            while (queue.Count > 0)
            {
                var he = queue.Dequeue();
                yield return he;

                foreach (var he0 in he.Circulate)
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


        /// <summary>
        /// Returns faces in depth-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<F> GetFacesDepthFirst<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, IEnumerable<F> exclude = null)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetFacesDepthFirst2(mesh, sources, exclude).Select(he => he.Face);
        }


        /// <summary>
        /// Returns faces in depth-first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetFacesDepthFirst2<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, IEnumerable<F> exclude = null)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;

            var stack = new Stack<E>();
            int currTag = faces.NextTag;

            // set sources
            foreach (var f in sources)
            {
                if (f.IsUnused) continue;
                faces.OwnsCheck(f);

                f.Tag = currTag;
                stack.Push(f.First);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var f in exclude)
                    f.Tag = currTag;
            }

            // search
            while (stack.Count > 0)
            {
                var he = stack.Pop();
                yield return he;

                foreach (var he0 in he.Circulate)
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
        

        /// <summary>
        /// Returns faces in best-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="getKey"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<F> GetFacesBestFirst<V, E, F, K>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<F, K> getKey, IEnumerable<F> exclude = null)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
            where K : IComparable<K>
        {
            return GetFacesBestFirst2(mesh, sources, he => getKey(he.Face), exclude).Select(he => he.Face);
        }


        /// <summary>
        /// Returns faces in best-first order via the traversed halfedge.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="getKey"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetFacesBestFirst2<V, E, F, K>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<E, K> getKey, IEnumerable<F> exclude = null)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
            where K : IComparable<K>
        {
            var faces = mesh.Faces;

            var pq = new PriorityQueue<K, E>();
            int currTag = faces.NextTag;

            // set sources
            foreach (var f in sources)
            {
                if (f.IsUnused) continue;
                faces.OwnsCheck(f);

                f.Tag = currTag;
                var he = f.First;
                pq.Insert(getKey(he), he);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var f in exclude)
                    f.Tag = currTag;
            }

            // search
            while (pq.Count > 0)
            {
                var he = pq.RemoveMin().Value;
                yield return he;

                foreach (var he0 in he.Circulate)
                {
                    var he1 = he0.Twin;
                    var f1 = he1.Face;

                    if (f1 != null && f1.Tag != currTag)
                    {
                        f1.Tag = currTag;
                        pq.Insert(getKey(he1), he1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns all halfedges whose angle to its previous exceeds the given tolerance.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getPosition"></param>
        /// <param name="angleTolerance"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetCornerHalfedges<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vector3d> getPosition, double angleTolerance = D.ZeroTolerance)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var ct = Math.Cos(angleTolerance);

            foreach (var he in mesh.Halfedges)
            {
                if (he.IsUnused) continue;

                var p = getPosition(he.Start);

                var d0 = p - getPosition(he.Previous.Start);
                var d1 = getPosition(he.Next.Start) - p;

                var m = d0.SquareLength * d1.SquareLength;

                if (m > 0.0)
                {
                    if (Vector3d.Dot(d0, d1) / Math.Sqrt(m) < ct)
                        yield return he;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void GetEdgeDepths<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<E> sources, ArrayView<int> result, IEnumerable<E> exclude = null)
           where V : HeStructure<V, E, F>.Vertex
           where E : HeStructure<V, E, F>.Halfedge
           where F : HeStructure<V, E, F>.Face
        {
            var prop = Halfedge<E>.CreateEdgeProperty(result);
            GetEdgeDepths(mesh, sources, prop, exclude);
        }


        /// <summary>
        /// Calculates the minimum topological depth of each edge from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetEdgeDepths<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<E> sources, Property<E, int> depth, IEnumerable<E> exclude = null)
        where V : HeStructure<V, E, F>.Vertex
        where E : HeStructure<V, E, F>.Halfedge
        where F : HeStructure<V, E, F>.Face
        {
            var edges = mesh.Edges;
            var queue = new Queue<E>();

            // set depths to max
            foreach (var he in edges)
                depth.Set(he, int.MaxValue);

            // enqueue sources and set to 0
            foreach (var he in sources)
            {
                edges.OwnsCheck(he);
                if (he.IsUnused) continue;

                depth.Set(he, 0);
                queue.Enqueue(he.IsHole ? he.Twin : he);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var he in exclude)
                    depth.Set(he, 0);
            }

            // bfs
            while (queue.Count > 0)
            {
                var he0 = queue.Dequeue();
                int d0 = depth.Get(he0) + 1;

                TryEnqueue(he0.Previous);
                TryEnqueue(he0.Next);

                he0 = he0.Twin;
                if (he0.IsHole) continue; // no traversal along boundaries

                TryEnqueue(he0.Previous);
                TryEnqueue(he0.Next);

                void TryEnqueue(E hedge)
                {
                    if (d0 < depth.Get(hedge))
                    {
                        depth.Set(hedge, d0);
                        queue.Enqueue(hedge);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the barycentric dual area around each vertex.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasBarycentric<V, E, F>(this HeStructure<V, E, F> mesh, Action<V, double> addArea)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            GetVertexAreasBarycentric(mesh, Position3d<V>.Get, addArea);
        }


        /// <summary>
        /// Calculates the barycentric dual area around each vertex.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasBarycentric<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vector3d> getPosition, Action<V, double> addArea)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            const double inv6 = 1.0 / 6.0; // (1.0 / 3.0) * 0.5

            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            // distribute face areas to vertices
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                GetTriVerts(f, out V v0, out V v1, out V v2);
                var p = getPosition(v0);
                var a = Vector3d.Cross(p - getPosition(v2), getPosition(v1) - p).Length * inv6;

                addArea(v0, a);
                addArea(v1, a);
                addArea(v2, a);
            }
            
            void GetTriVerts(F face, out V v0, out V v1, out V v2)
            {
                var he = face.First;
                v0 = he.Start;

                he = he.Next;
                v1 = he.Start;

                he = he.Next;
                v2 = he.Start;
            }
        }


        /// <summary>
        /// Calculates the circumcentric dual area around each vertex.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasCircumcentric<V, E, F>(this HeStructure<V, E, F> mesh, Action<V, double> addArea)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            GetVertexAreasCircumcentric(mesh, Position3d<V>.Get, addArea);
        }


        /// <summary>
        /// Calculates the circumcentric dual area around each vertex.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasCircumcentric<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vector3d> getPosition, Action<V, double> addArea)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            // impl ref
            // http://www.cs.cmu.edu/~kmcrane/Projects/Other/TriangleAreasCheatSheet.pdf
        
            const double inv8 = 1.0 / 8.0;

            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            // distribute face areas to vertices
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;
                
                GetTriVerts(f, out V v0, out V v1, out V v2);

                var p0 = getPosition(v0);
                var p1 = getPosition(v1);
                var p2 = getPosition(v2);

                var d0 = p1 - p0;
                var d1 = p2 - p1;
                var d2 = p0 - p2;
                
                var t = 1.0 / Vector3d.Cross(d0, d1).Length;
                var cot0 = -Vector3d.Dot(d2, d0) * t;
                var cot1 = -Vector3d.Dot(d0, d1) * t;
                var cot2 = -Vector3d.Dot(d1, d2) * t;

                var m0 = d0.SquareLength;
                var m1 = d1.SquareLength;
                var m2 = d2.SquareLength;
                
                addArea(v0, (m2 * cot1 + m0 * cot2) * inv8);
                addArea(v1, (m0 * cot2 + m1 * cot0) * inv8);
                addArea(v2, (m1 * cot0 + m2 * cot1) * inv8);
            }
            
            void GetTriVerts(F face, out V v0, out V v1, out V v2)
            {
                var he = face.First;
                v0 = he.Start;

                he = he.Next;
                v1 = he.Start;

                he = he.Next;
                v2 = he.Start;
            }
        }


        /// <summary>
        /// Calculates the mixed dual area around each vertex.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasMixed<V, E, F>(this HeStructure<V, E, F> mesh, Action<V, double> addArea)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            GetVertexAreasMixed(mesh, Position3d<V>.Get, addArea);
        }


        /// <summary>
        /// Calculates the mixed dual area around each vertex.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasMixed<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vector3d> getPosition, Action<V, double> addArea)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            // impl ref
            // http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)

            // TODO implement
            throw new NotImplementedException();
        }

        
        /// <summary>
        /// Returns the total surface area of the mesh.
        /// Note that the area calculation assumes planar faces.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static double GetFaceAreaSum<V, E, F>(this HeStructure<V, E, F> mesh) 
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetFaceAreaSum(mesh, Position3d<V>.Get);
        }

        
        /// <summary>
        /// Returns the total surface area of the mesh.
        /// Note that the area calculation assumes planar faces.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetFaceAreaSum<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vector3d> getPosition) 
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;
            var sum = 0.0;

            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                
                if (!f.IsUnused)
                    sum += f.GetArea(getPosition);
            }

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void GetFaceDepths<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, ArrayView<int> result, IEnumerable<F> exclude = null)
           where V : HeStructure<V, E, F>.Vertex
           where E : HeStructure<V, E, F>.Halfedge
           where F : HeStructure<V, E, F>.Face
        {
            var prop = Node<F>.CreateProperty(result);
            GetFaceDepths(mesh, sources, prop, exclude);
        }


        /// <summary>
        /// Calculates the minimum topological depth of all faces connected to a set of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetFaceDepths<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Property<F, int> depth, IEnumerable<F> exclude = null)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;
            var queue = new Queue<F>();

            // set depths to max
            foreach (var f in faces)
                depth.Set(f, int.MaxValue);

            // enqueue sources and set to zero
            foreach (var f in sources)
            {
                f.UnusedCheck();
                faces.OwnsCheck(f);

                depth.Set(f, 0);
                queue.Enqueue(f);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var f in exclude)
                    depth.Set(f, 0);
            }

            // bfs
            while (queue.Count > 0)
            {
                var f0 = queue.Dequeue();
                int t0 = depth.Get(f0) + 1;

                foreach (var f1 in f0.AdjacentFaces)
                {
                    if (t0 < depth.Get(f1))
                    {
                        depth.Set(f1, t0);
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="getLength"></param>
        /// <param name="result"></param>
        /// <param name="bestFirst"></param>
        public static void GetFaceDistances<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<E, double> getLength, ArrayView<double> result, bool bestFirst = false)
           where V : HeStructure<V, E, F>.Vertex
           where E : HeStructure<V, E, F>.Halfedge
           where F : HeStructure<V, E, F>.Face
        {
            var prop = Node<F>.CreateProperty(result);
            GetFaceDistances(mesh, sources, getLength, prop, bestFirst);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="sources"></param>
        /// <param name="getLength"></param>
        /// <param name="distance"></param>
        /// <param name="bestFirst"></param>
        public static void GetFaceDistances<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<E, double> getLength, Property<F, double> distance, bool bestFirst = false)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            if (bestFirst)
                GetFaceDistancesBestFirst(mesh, sources, getLength, distance);
            else
                GetFaceDistancesBreadthFirst(mesh, sources, getLength, distance);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetFaceDistancesBreadthFirst<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<E, double> getLength, Property<F, double> distance)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;
            var queue = new Queue<F>();

            // set distances to max
            foreach (var f in faces)
                distance.Set(f, double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (var f in sources)
            {
                f.UnusedCheck();
                faces.OwnsCheck(f);

                distance.Set(f, 0.0);
                queue.Enqueue(f);
            }

            // bfs from sources
            while (queue.Count > 0)
            {
                var f0 = queue.Dequeue();
                double t0 = distance.Get(f0);

                foreach (var he in f0.Halfedges)
                {
                    var f1 = he.Twin.Face;
                    if (f1 == null) continue;

                    double t1 = t0 + getLength(he);

                    if (t1 < distance.Get(f1))
                    {
                        distance.Set(f1, t1);
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetFaceDistancesBestFirst<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<E, double> getLength, Property<F, double> distance)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;
            var pq = new PriorityQueue<double, F>();

            // set distances to max
            foreach (var f in faces)
                distance.Set(f, double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (var f in sources)
            {
                f.UnusedCheck();
                faces.OwnsCheck(f);

                distance.Set(f, 0.0);
                pq.Insert(0.0, f);
            }

            // best first search from sources
            while (pq.Count > 0)
            {
                (var d0, var f0) = pq.RemoveMin();
                if (d0 > distance.Get(f0)) continue; // skip if lower value was already assigned

                foreach (var he in f0.Halfedges)
                {
                    var f1 = he.Twin.Face;
                    if (f1 == null) continue;

                    double d1 = d0 + getLength(he);

                    if (d1 < distance.Get(f1))
                    {
                        distance.Set(f1, d1);
                        pq.Insert(d1, f1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="normal"></param>
        /// <param name="start"></param>
        public static void UnifyVertexNormals<V, E, F>(this HeStructure<V, E, F> mesh, Property<V, Vector3d> normal, V start)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            foreach (var he in mesh.GetVerticesBreadthFirst2(start.Yield()))
            {
                var v = he.Start;
                var n = normal.Get(v);

                if (Vector3d.Dot(n, normal.Get(he.End)) < 0.0)
                    normal.Set(v, -n);
            }
        }

        #endregion
    }
}
