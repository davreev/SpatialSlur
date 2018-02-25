using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

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
            where V : HeVertex<V, E>, IPosition3d, INormal3d
            where E : Halfedge<V, E>
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
        /// Returns the number of connected components in the graph.
        /// </summary>
        public static int CountConnectedComponents<V, E>(this HeStructure<V, E> graph)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the first halfedge from each connected component in the graph.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<E> GetConnectedComponents<V, E>(this HeStructure<V, E> graph)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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

                    // add unvisited neighbours to the stack
                    foreach (var he1 in he0.ConnectedEdges)
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
        /// Returns edges in breadth-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesBreadthFirst<V, E>(this HeStructure<V, E> graph, IEnumerable<E> sources, IEnumerable<E> exclude = null)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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

                foreach (var he1 in he0.ConnectedEdges)
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
        /// Returns edges in depth-first order.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgesDepthFirst<V, E>(this HeStructure<V, E> graph, IEnumerable<E> sources, IEnumerable<E> exclude = null)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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

                foreach (var he1 in he0.ConnectedEdges)
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
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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

                foreach (var he1 in he0.ConnectedEdges)
                {
                    if (he1.Tag != currTag)
                    {
                        he1.Tag = he1.Twin.Tag = currTag;
                        pq.Insert(getKey(he1), he1);
                    }
                }
            }
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
        public static IEnumerable<V> GetVerticesBreadthFirst<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, IEnumerable<V> exclude = null)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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


        #region Edge Attributes
        
        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeLengths<V, E>(this HeStructure<V, E> graph, Action<E, double> setLength, bool parallel = false)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            GetEdgeLengths(graph, IPosition3d<V>.Get, setLength, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeLengths<V, E>(this HeStructure<V, E> graph, Func<V, Vec3d> getPosition, Action<E, double> setLength, bool parallel = false)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var edges = graph.Edges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, edges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he = edges[i];
                    if (he.IsUnused) continue;
                    setLength(he, he.GetLength(getPosition));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeTangents<V, E>(this HeStructure<V, E> graph, Action<E, Vec3d> setTangent, bool parallel = false)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            GetEdgeTangents(graph, IPosition3d<V>.Get, setTangent, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeTangents<V, E>(this HeStructure<V, E> graph, Func<V, Vec3d> getPosition, Action<E, Vec3d> setTangent, bool parallel = false)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var edges = graph.Edges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, edges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he = edges[i];
                    if (he.IsUnused) continue;

                    var d = getPosition(he.End) - getPosition(he.Start);
                    setTangent(he, d.Unit);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetEdgeLengthSum<V, E>(this HeStructure<V, E> graph)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            return GetEdgeLengthSum(graph, IPosition3d<V>.Get);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetEdgeLengthSum<V, E>(this HeStructure<V, E> graph, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var hedges = graph.Halfedges;
            var sum = 0.0;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused) continue;

                sum += he.GetLength(getPosition);
            }

            return sum;
        }


        /// <summary>
        /// Returns the connected component index of each edge in the mesh.
        /// Also returns the number of connected components.
        /// </summary>
        public static int GetEdgeComponentIndices<V, E>(this HeStructure<V, E> graph, Action<E, int> setIndex)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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

                    // add unvisited neighbours to the stack
                    foreach (var he1 in he0.ConnectedEdges)
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
        public static void GetEdgeDepths<V, E>(this HeStructure<V, E> graph, IEnumerable<E> sources, Property<E, int> depth, IEnumerable<E> exclude = null)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
                int t0 = depth.Get(he0) + 1;

                foreach (var he1 in he0.ConnectedEdges)
                {
                    if (t0 < depth.Get(he1))
                    {
                        depth.Set(he1, t0);
                        queue.Enqueue(he1);
                    }
                }
            }
        }

        #endregion


        #region Vertex Attributes

        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this HeStructure<V, E> graph, Func<V, double> getValue, Action<V, double> setLaplace, bool parallel = false)
           where V : HeVertex<V, E>
           where E : Halfedge<V, E>
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
                    if (v0.IsUnused) continue;

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
            }
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this HeStructure<V, E> graph, Func<V, Vec2d> getValue, Action<V, Vec2d> setLaplace, bool parallel = false)
           where V : HeVertex<V, E>
           where E : Halfedge<V, E>
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
                    if (v0.IsUnused) continue;

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
            }
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this HeStructure<V, E> graph, Func<V, Vec3d> getValue, Action<V, Vec3d> setLaplace, bool parallel = false)
           where V : HeVertex<V, E>
           where E : Halfedge<V, E>
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
                    if (v0.IsUnused) continue;

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
            }
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this HeStructure<V, E> graph, Func<V, double> getValue, Func<E, double> getWeight, Action<V, double> setLaplace, bool parallel = false)
           where V : HeVertex<V, E>
           where E : Halfedge<V, E>
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
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    var t = getValue(v);
                    var sum = 0.0;

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (getValue(he.End) - t) * getWeight(he);

                    setLaplace(v, sum);
                }
            }
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this HeStructure<V, E> graph, Func<V, Vec2d> getValue, Func<E, double> getWeight, Action<V, Vec2d> setLaplace, bool parallel = false)
           where V : HeVertex<V, E>
           where E : Halfedge<V, E>
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
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    var t = getValue(v);
                    var sum = new Vec2d();

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (getValue(he.End) - t) * getWeight(he);

                    setLaplace(v, sum);
                }
            }
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this HeStructure<V, E> graph, Func<V, Vec3d> getValue, Func<E, double> getWeight, Action<V, Vec3d> setLaplace, bool parallel = false)
           where V : HeVertex<V, E>
           where E : Halfedge<V, E>
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
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    var t = getValue(v);
                    var sum = new Vec3d();

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (getValue(he.End) - t) * getWeight(he);

                    setLaplace(v, sum);
                }
            }
        }


        /// <summary>
        /// Normalizes halfedge values such that the weights of outgoing halfedges around each vertex sum to 1.
        /// Note that this breaks weight symmetry between halfedge pairs.
        /// </summary>
        public static void NormalizeAtVertices<V, E>(this HeStructure<V, E> graph, Property<E, double> value, bool parallel = false)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
                    var v = verts[i];
                    if (v.IsUnused) continue;
                    v.OutgoingHalfedges.Normalize(value);
                }
            }
        }


        /// <summary>
        /// Calculates the morse smale classification for each vertex (0 = normal, 1 = minima, 2 = maxima, 3 = saddle).
        /// Assumes halfedges are radially sorted around the given vertices.
        /// </summary>
        public static void GetVertexMorseSmaleLabels<V, E>(this HeStructure<V, E> graph, Func<V, double> getValue, Action<V, int> setLabel, bool parallel = false)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
        /// Calculates the minimum topological depth of each vertex from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetVertexDepths<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, Property<V, int> depth, IEnumerable<V> exclude = null)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
        /// Calculates the minimum topological distance to each vertex from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetVertexDistances<V, E>(this HeStructure<V, E> graph, IEnumerable<V> sources, Func<E, double> getLength, Property<V, double> distance, bool bestFirst = false)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
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
        /// Returns the entries of the incidence matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetVertexIncidenceMatrix<V, E>(this HeStructure<V, E> graph, double[] result)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var hedges = graph.Halfedges;

            int nv = graph.Vertices.Count;
            int ne = hedges.Count >> 1;
            Array.Clear(result, 0, nv * ne);

            for (int i = 0; i < ne; i++)
            {
                var he = hedges[i >> 1];
                if (he.IsUnused) continue;

                int j = i * nv;
                result[j + he.Start.Index] = result[j + he.End.Index] = 1.0;
            }
        }


        /// <summary>
        /// Returns the entries of the adjacency matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetVertexAdjacencyMatrix<V, E>(this HeStructure<V, E> graph, double[] result)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var verts = graph.Vertices;

            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v0 = verts[i];
                if (v0.IsUnused) continue;

                foreach (var v1 in v0.ConnectedVertices)
                    result[i * nv + v1.Index] = 1.0;
            }
        }


        /// <summary>
        /// Calculates the Laplacian matrix in column-major order.
        /// </summary>
        public static void GetVertexLaplacianMatrix<V, E>(this HeStructure<V, E> graph, double[] result)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var verts = graph.Vertices;

            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

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
        public static void GetVertexLaplacianMatrix<V, E>(this HeStructure<V, E> graph, Func<E, double> getWeight, double[] result)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var verts = graph.Vertices;

            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

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


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="getValue"></param>
        /// <param name="getWeight"></param>
        /// <param name="setLaplace"></param>
        /// <param name="parallel"></param>
        public static void UnifyVertexNormals<V, E>(this HeStructure<V, E> graph, Property<V, Vec3d> normal, V start)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            foreach (var he in graph.GetVerticesBreadthFirst2(start.Yield()))
            {
                var v = he.Start;
                var n = normal.Get(v);

                if (Vec3d.Dot(n, normal.Get(he.End)) < 0.0)
                    normal.Set(v, -n);
            }
        }

        #endregion

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
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
        public static IEnumerable<F> GetFacesBestFirst<V, E, F, K>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<F, K> getKey, IEnumerable<F> exclude = null)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
        /// <param name="graph"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetCornerHalfedges<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, double angleTolerance = SlurMath.ZeroTolerance)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
                    if (Vec3d.Dot(d0, d1) / Math.Sqrt(m) < ct)
                        yield return he;
                }
            }
        }


        #region Halfedge Attributes

        /// <summary>
        /// Calculates the area associated with each halfedge.
        /// This is calculated as V in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static void GetHalfedgeAreas<V, E, F>(this HeStructure<V, E, F> mesh, Func<F, Vec3d> getCenter, Action<E, double> setAreas, bool parallel = false)
          where V : HeVertex<V, E, F>, IPosition3d
          where E : Halfedge<V, E, F>
          where F : HeFace<V, E, F>
        {
            GetHalfedgeAreas(mesh, IPosition3d<V>.Get, getCenter, setAreas, parallel);
        }


        /// <summary>
        /// Calculates the area associated with each halfedge.
        /// This is calculated as V in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static void GetHalfedgeAreas<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<F, Vec3d> getCenter, Action<E, double> setAreas, bool parallel = false)
          where V : HeVertex<V, E, F>
          where E : Halfedge<V, E, F>
          where F : HeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, hedges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he = hedges[i];
                    var f = he.Face;
                    if (he.IsUnused || f == null) continue;
                    setAreas(he, he.GetArea(getPosition, getCenter(f)));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeNormals<V, E, F>(this HeStructure<V, E, F> mesh, Action<E, Vec3d> setNormal, bool unitize = false, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetHalfedgeNormals(mesh, IPosition3d<V>.Get, setNormal, unitize, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeNormals<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<E, Vec3d> setNormal, bool unitize = false, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, hedges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he = hedges[i];
                    if (he.IsUnused) continue;

                    var n = he.GetNormal(getPosition);
                    setNormal(he, unitize ? n.Unit : n);
                }
            }
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>
        public static void GetDihedralAngles<V, E, F>(this HeStructure<V, E, F> mesh, Func<F, Vec3d> getNormal, Action<E, double> setAngle, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetDihedralAngles(mesh, IPosition3d<V>.Get, getNormal, setAngle, parallel);
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>
        public static void GetDihedralAngles<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<F, Vec3d> getNormal, Action<E, double> setAngle, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var edges = mesh.Edges;
            
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, edges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he = edges[i];
                    if (he.IsUnused || he.IsBoundary) continue;
                    setAngle(he, he.GetDihedralAngle(getPosition, getNormal));
                }
            }
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite each halfedge.
        /// Assumes triangular faces.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static void GetHalfedgeCotangents<V, E, F>(this HeStructure<V, E, F> mesh, Action<E, double> setCotangent, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetHalfedgeCotangents(mesh, IPosition3d<V>.Get, setCotangent, parallel);
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite each halfedge.
        /// Assumes triangular faces.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static void GetHalfedgeCotangents<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<E, double> setCotangent, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, hedges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he = hedges[i];
                    if (he.IsUnused || he.Face == null) continue;
                    setCotangent(he, he.GetCotangent(getPosition));
                }
            }
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each halfedge.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetHalfedgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, double> getArea, Action<E, double> setWeight, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetHalfedgeCotanWeights(mesh, IPosition3d<V>.Get, getArea, setWeight, parallel);
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each halfedge.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetHalfedgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<E, double> setWeight, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var edges = mesh.Edges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, edges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he0 = edges[i];
                    if (he0.IsUnused) continue;

                    var he1 = he0.Twin;
                    double w = 0.0;

                    if (he0.Face != null) w += he0.GetCotangent(getPosition);
                    if (he1.Face != null) w += he1.GetCotangent(getPosition);
                    w *= 0.5;

                    setWeight(he0, w / getArea(he0.Start));
                    setWeight(he1, w / getArea(he1.Start));
                }
            }
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each halfedge along with the barycentric dual area of each vertex.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetHalfedgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Property<V, double> area, Property<E, double> weight)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetHalfedgeCotanWeights(mesh, IPosition3d<V>.Get, area, weight);
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each halfedge along with the barycentric dual area of each vertex.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetHalfedgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Property<V, double> area, Property<E, double> weight)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            // clear areas
            foreach (var v in mesh.Vertices)
                area.Set(v, 0.0);

            // accumulate cotangent weights and vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsUnused) continue;

                var he1 = hedges[i + 1];
                var v0 = he0.Start;
                var v1 = he1.Start;

                double w = 0.0;
                if (he0.Face != null) w += GetWeight(he0);
                if (he1.Face != null) w += GetWeight(he1);
                weight.Set(he0, w * 0.5);

                double GetWeight(E hedge)
                {
                    const double t = 0.5 / 3.0;
                    var v = hedge.Previous.Start;

                    var p = getPosition(v);
                    var d0 = getPosition(v0) - p;
                    var d1 = getPosition(v1) - p;

                    double a = Vec3d.Cross(d0, d1).Length;
                    area.Set(v, area.Get(v) + a * t);

                    return Vec3d.Dot(d0, d1) / a;
                }
            }

            // normalize weights by vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsUnused) continue;

                var he1 = hedges[i + 1];
                double w = weight.Get(he0);

                weight.Set(he0, w / area.Get(he0.Start));
                weight.Set(he1, w / area.Get(he1.Start));
            }
        }

        #endregion


        #region Edge Attributes


        /// <summary>
        /// Calculates the cotangent weight for each edge (symmetric).
        /// Based on Pinkall and Polthier's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetEdgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Action<E, double> setWeight, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetEdgeCotanWeights(mesh, IPosition3d<V>.Get, setWeight, parallel);
        }


        /// <summary>
        /// Calculates the cotangent weight for each edge (symmetric).
        /// Based on Pinkall and Polthier's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetEdgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<E, double> setWeight, bool parallel = false)
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            var edges = mesh.Edges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, edges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he0 = edges[i];
                    if (he0.IsUnused) continue;

                    var he1 = he0.Twin;
                    double w = 0.0;

                    if (he0.Face != null) w += he0.GetCotangent(getPosition);
                    if (he1.Face != null) w += he1.GetCotangent(getPosition);

                    setWeight(he0, w * 0.5);
                }
            }
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each edge (symmetric).
        /// Based on Levy and Vallet's derivation of the symmetric Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetEdgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, double> getArea, Action<E, double> setWeight, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetEdgeCotanWeights(mesh, IPosition3d<V>.Get, getArea, setWeight, parallel);
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each edge (symmetric).
        /// Based on Levy and Vallet's derivation of the symmetric Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetEdgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<E, double> setWeight, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var edges = mesh.Edges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, edges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he0 = edges[i];
                    if (he0.IsUnused) continue;

                    var he1 = he0.Twin;
                    double w = 0.0;

                    if (he0.Face != null) w += he0.GetCotangent(getPosition);
                    if (he1.Face != null) w += he1.GetCotangent(getPosition);
                    w *= 0.5 / Math.Sqrt(getArea(he0.Start) * getArea(he1.Start));

                    setWeight(he0, w);
                }
            }
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each edge (symmetric) along with the barycentric dual area of each vertex.
        /// Based on Levy and Vallet's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetEdgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Property<V, double> area, Property<E, double> weight)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetEdgeCotanWeights(mesh, IPosition3d<V>.Get, area, weight);
        }


        /// <summary>
        /// Calculates the area-dependant cotangent weight for each edge (symmetric) along with the barycentric dual area of each vertex.
        /// Based on Levy and Vallet's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetEdgeCotanWeights<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Property<V, double> area, Property<E, double> weight)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            // clear areas
            foreach (var v in mesh.Vertices)
                area.Set(v, 0.0);

            // accumulate cotangent weights and vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsUnused) continue;

                var he1 = hedges[i + 1];
                var v0 = he0.Start;
                var v1 = he1.Start;

                double w = 0.0;
                if (he0.Face != null) w += GetWeight(he0);
                if (he1.Face != null) w += GetWeight(he1);
                weight.Set(he0, w * 0.5);

                double GetWeight(E hedge)
                {
                    const double t = 0.5 / 3.0;
                    var v = hedge.Previous.Start;

                    var p = getPosition(v);
                    var d0 = getPosition(v0) - p;
                    var d1 = getPosition(v1) - p;

                    double a = Vec3d.Cross(d0, d1).Length;
                    area.Set(v, area.Get(v) + a * t);

                    return Vec3d.Dot(d0, d1) / a;
                }
            }

            // symmetrically normalize weights by vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he0 = hedges[i];
                if (he0.IsUnused) continue;

                double w = weight.Get(he0) / Math.Sqrt(area.Get(he0.Start) * area.Get(hedges[i + 1].Start));
                weight.Set(he0, w);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        public static void GetEdgeNormals<V, E, F>(this HeStructure<V, E, F> mesh, Action<E, Vec3d> setNormal, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetEdgeNormals(mesh, IPosition3d<V>.Get, setNormal, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        public static void GetEdgeNormals<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<E, Vec3d> setNormal, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var edges = mesh.Edges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, edges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he = edges[i];
                    if (he.IsUnused) continue;
                    setNormal(he, he.GetEdgeNormal(getPosition));
                }
            }
        }


        /// <summary>
        /// Calculates the minimum topological depth of each edge from a collection of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetEdgeDepths<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<E> sources, Property<E, int> depth, IEnumerable<E> exclude = null)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
                int t0 = depth.Get(he0) + 1;

                foreach (var he1 in ConnectedEdges(he0))
                {
                    if (t0 < depth.Get(he1))
                    {
                        depth.Set(he1, t0);
                        queue.Enqueue(he1);
                    }
                }
            }
            
            IEnumerable<E> ConnectedEdges(E hedge)
            {
                yield return hedge.Previous;
                yield return hedge.Next;

                hedge = hedge.Twin;
                if (hedge.IsHole) yield break; // don't allow traversal along hole boundaries

                yield return hedge.Previous;
                yield return hedge.Next;
            }
        }

        #endregion


        #region Vertex Attributes

        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexAreas<V, E, F>(this HeStructure<V, E, F> mesh, Func<F, Vec3d> getCenter, Action<V, double> setArea, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetVertexAreas(mesh, IPosition3d<V>.Get, getCenter, setArea, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexAreas<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<F, Vec3d> getCenter, Action<V, double> setArea, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    double sum = 0.0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        var f = he.Face;

                        if (f != null)
                            sum += he.GetArea(getPosition, getCenter(f));
                    }

                    setArea(v, sum);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexAreas<V, E, F>(this HeStructure<V, E, F> mesh, Func<E, double> getArea, Action<V, double> setArea, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;
                    setArea(v, v.OutgoingHalfedges.Sum(getArea));
                }
            }
        }


        /// <summary>
        /// Calculates the barycentric dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasBarycentric<V, E, F>(this HeStructure<V, E, F> mesh, Property<V, double> area)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetVertexAreasBarycentric(mesh, IPosition3d<V>.Get, area);
        }


        /// <summary>
        /// Calculates the barycentric dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasBarycentric<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Property<V, double> area)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            const double t = 1.0 / 6.0; // (1.0 / 3.0) * 0.5

            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            // clear areas
            foreach (var v in verts)
                area.Set(v, 0.0);

            // distribute face areas to vertices
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                double a = f.First.GetNormal(getPosition).Length * t;

                foreach (var v in f.Vertices)
                    area.Set(v, area.Get(v) + a);
            }
        }


        /// <summary>
        /// Calculates the circumcentric dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasCircumcentric<V, E, F>(this HeStructure<V, E, F> mesh, Property<V, double> area)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetVertexAreasCircumcentric(mesh, IPosition3d<V>.Get, area);
        }


        /// <summary>
        /// Calculates the circumcentric dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasCircumcentric<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Property<V, double> area)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Calculates the mixed dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasMixed<V, E, F>(this HeStructure<V, E, F> mesh, Property<V, double> area)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetVertexAreasMixed(mesh, IPosition3d<V>.Get, area);
        }


        /// <summary>
        /// Calculates the mixed dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Note that corresponding get/set delegates should read/write to the same location.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetVertexAreasMixed<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Property<V, double> area)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Calculates the circle packing radii for each vertex.
        /// Assumes the mesh is a circle packing mesh http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        public static void GetVertexCirclePackingRadii<V, E, F>(this HeStructure<V, E, F> mesh, Func<E, double> getLength, Action<V, double> setRadius, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue; // skip unused vertices

                    double sum = 0.0;
                    int n = 0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        if (he.Face == null) continue; // skip boundary edges
                        sum += (getLength(he) + getLength(he.Previous) - getLength(he.Next)) * 0.5;
                        n++;
                    }

                    setRadius(v, sum / n);
                }
            }
        }


        /// <summary>
        /// Calculated as the signed magnitude of the vertex laplacian.
        /// http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 47)
        /// </summary>
        public static void GetVertexMeanCurvature<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getLaplace, Action<V, double> setCurvature, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    if (v.IsBoundary)
                        setCurvature(v, 0.0);
                    else
                        setCurvature(v, getLaplace(v).Length * -0.5);
                }
            }
        }


        /// <summary>
        /// Calculated as the signed magnitude of the vertex laplacian with respect to the vertex normal.
        /// http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 47)
        /// </summary>
        public static void GetVertexMeanCurvatureSigned<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getLaplace, Func<V, Vec3d> getNormal, Action<V, double> setCurvature, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    if (v.IsBoundary)
                    {
                        setCurvature(v, 0.0);
                    }
                    else
                    {
                        var lap = getLaplace(v);
                        setCurvature(v, Math.Sign(Vec3d.Dot(getNormal(v), lap)) * lap.Length * -0.5);
                    }
                }
            }
        }


        /// <summary>
        /// Calculated as the angle defect around each vertex divided by the vertex area.
        /// </summary>
        public static void GetVertexGaussianCurvature<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, double> getArea, Action<V, double> setCurvature, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetVertexGaussianCurvature(mesh, IPosition3d<V>.Get, getArea, setCurvature, parallel);
        }


        /// <summary>
        /// Calculated as the angle defect around each vertex divided by the vertex area.
        /// </summary>
        public static void GetVertexGaussianCurvature<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Func<V, double> getArea, Action<V, double> setCurvature, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

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
            }
        }


        /// <summary>
        /// Calculated as the angle defect around each vertex.
        /// </summary>
        public static void GetVertexGaussianCurvature<V, E, F>(this HeStructure<V, E, F> mesh, Func<E, double> getAngle, Func<V, double> getArea, Action<V, double> setCurvature, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

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
            }
        }


        /// <summary>
        /// Calculates vertex normals as the area-weighted sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        public static void UpdateVertexNormals<V, E, F>(this HeStructure<V, E, F> mesh, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d, INormal3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetVertexNormals(mesh, IPosition3d<V>.Get, INormal3d<V>.Set, parallel);
        }


        /// <summary>
        /// Calculates vertex normals as the area-weighted sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        public static void GetVertexNormals<V, E, F>(this HeStructure<V, E, F> mesh, Action<V, Vec3d> setNormal, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetVertexNormals(mesh, IPosition3d<V>.Get, setNormal, parallel);
        }


        /// <summary>
        /// Calculates vertex normals as the area-weighted sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        public static void GetVertexNormals<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<V, Vec3d> setNormal, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var verts = mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;
                    setNormal(v, v.GetNormal(getPosition));
                }
            }
        }

        #endregion


        #region Face Attributes

        /// <summary>
        /// Normalizes halfedge values such that the weights of halfedges within each face sum to 1.
        /// Note that this breaks weight symmetry between halfedge pairs.
        /// </summary>
        public static void NormalizeInFaces<V, E, F>(this HeStructure<V, E, F> mesh, Property<E, double> values, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    f.Halfedges.Normalize(values);
                }
            }
        }


        /// <summary>
        /// Calculates the barycenter of each face.
        /// </summary>
        public static void GetFaceBarycenters<V, E, F>(this HeStructure<V, E, F> mesh, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetFaceBarycenters(mesh, IPosition3d<V>.Get, setCenter, parallel);
        }


        /// <summary>
        /// Calculates the barycenter of each face.
        /// </summary>
        public static void GetFaceBarycenters<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setCenter(f, f.GetBarycenter(getPosition));
                }
            }
        }


        /// <summary>
        /// Calculates the circumcenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceCircumcenters<V, E, F>(this HeStructure<V, E, F> mesh, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetFaceCircumcenters(mesh, IPosition3d<V>.Get, setCenter, parallel);
        }


        /// <summary>
        /// Calculates the circumcenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceCircumcenters<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setCenter(f, f.GetCircumcenter(getPosition));
                }
            }
        }


        /// <summary>
        /// Calculates the incenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceIncenters<V, E, F>(this HeStructure<V, E, F> mesh, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetFaceIncenters(mesh, IPosition3d<V>.Get, setCenter, parallel);
        }


        /// <summary>
        /// Calculates the incenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceIncenters<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setCenter, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setCenter(f, f.GetIncenter(getPosition));

                }
            }
        }


        /// <summary>
        /// Calculates face normals as the area-weighted sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        public static void GetFaceNormals<V, E, F>(this HeStructure<V, E, F> mesh, Action<F, Vec3d> setNormal, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetFaceNormals(mesh, IPosition3d<V>.Get, setNormal, parallel);
        }


        /// <summary>
        /// Calculates face normals as the area-weighted sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        public static void GetFaceNormals<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setNormal, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setNormal(f, f.GetNormal(getPosition));
                }
            }
        }


        /// <summary>
        /// Calculates face normals as the sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        public static void GetFaceNormals<V, E, F>(this HeStructure<V, E, F> mesh, Func<E, Vec3d> getNormal, Action<F, Vec3d> setNormal, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setNormal(f, f.GetNormal(getNormal));
                }
            }
        }


        /// <summary>
        /// Calculates face normals as the normal of the first halfedge in each face.
        /// Face normals are unitized by default.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceNormalsTri<V, E, F>(this HeStructure<V, E, F> mesh, Action<F, Vec3d> setNormal, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetFaceNormalsTri(mesh, IPosition3d<V>.Get, setNormal, parallel);
        }


        /// <summary>
        /// Calculates face normals as the normal of the first halfedge in each face.
        /// Face normals are unitized by default.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceNormalsTri<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, Vec3d> setNormal, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setNormal(f, f.First.GetNormal(getPosition).Unit);
                }
            }
        }


        /// <summary>
        /// Calculates the area for each face in the mesh.
        /// Note that this assumes planar faces.
        /// </summary>
        public static void GetFaceAreas<V, E, F>(this HeStructure<V, E, F> mesh, Action<F, double> setArea, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetFaceAreas(mesh, IPosition3d<V>.Get, setArea, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceAreas<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, double> setArea, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    var he0 = f.First;

                    if (he0 == he0.Next.Next.Next)
                    {
                        Vec3d n = he0.GetNormal(getPosition);
                        setArea(f, n.Length * 0.5);
                    }
                    else
                    {
                        Vec3d n = f.Halfedges.Sum(he => he.GetNormal(getPosition));
                        setArea(f, n.Length * 0.5);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the area of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceAreasTri<V, E, F>(this HeStructure<V, E, F> mesh, Action<F, double> setArea, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetFaceAreasTri(mesh, IPosition3d<V>.Get, setArea, parallel);
        }


        /// <summary>
        /// Calculates the area of each face.
        /// Assumes triangular faces.
        /// </summary>
        public static void GetFaceAreasTri<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, double> setArea, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setArea(f, f.First.GetNormal(getPosition).Length * 0.5);
                }
            }
        }


        /// <summary>
        /// Returns the planar deviation for each face.
        /// </summary>
        public static void GetFacePlanarity<V, E, F>(this HeStructure<V, E, F> mesh, Action<F, double> setPlanarity, bool parallel = false)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            GetFacePlanarity(mesh, IPosition3d<V>.Get, setPlanarity, parallel);
        }


        /// <summary>
        /// Returns the planar deviation for each face.
        /// </summary>
        public static void GetFacePlanarity<V, E, F>(this HeStructure<V, E, F> mesh, Func<V, Vec3d> getPosition, Action<F, double> setPlanarity, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setPlanarity(f, f.GetPlanarity(getPosition));
                }
            }
        }
        

        /// <summary>
        /// Calculates the minimum topological depth of all faces connected to a set of sources.
        /// Note that corresponding get/set delegates must read/write to the same location.
        /// </summary>
        public static void GetFaceDepths<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Property<F, int> depth, IEnumerable<F> exclude = null)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
            if(exclude != null)
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
        /// <param name="distance"></param>
        /// <param name="bestFirst"></param>
        private static void GetFaceDistances<V, E, F>(this HeStructure<V, E, F> mesh, IEnumerable<F> sources, Func<E, double> getLength, Property<F, double> distance, bool bestFirst = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
        /// Returns the entries of the incidence matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetFaceIncidenceMatrix<V, E, F>(this HeStructure<V, E, F> mesh, double[] result)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var hedges = mesh.Halfedges;

            int nf = mesh.Faces.Count;
            int ne = hedges.Count >> 1;
            Array.Clear(result, 0, nf * ne);

            for (int i = 0; i < ne; i++)
            {
                var he = hedges[i >> 1];
                if (he.IsUnused) continue;

                int j = i * nf;
                result[j + he.Start.Index] = result[j + he.End.Index] = 1.0;
            }
        }


        /// <summary>
        /// Returns the entries of the adjacency matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetFaceAdjacencyMatrix<V, E, F>(this HeStructure<V, E, F> mesh, double[] result)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            int nf = faces.Count;
            Array.Clear(result, 0, nf * nf);

            for (int i = 0; i < nf; i++)
            {
                var f0 = faces[i];
                if (f0.IsUnused) continue;

                foreach (var f1 in f0.AdjacentFaces)
                    result[i * nf + f1.Index] = 1.0;
            }
        }


        /// <summary>
        /// Calculates the Laplacian matrix in column-major order.
        /// </summary>
        public static void GetFaceLaplacianMatrix<V, E, F>(this HeStructure<V, E, F> mesh, double[] result)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            int nf = faces.Count;
            Array.Clear(result, 0, nf * nf);

            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

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
        public static void GetFaceLaplacianMatrix<V, E, F>(this HeStructure<V, E, F> mesh, Func<E, double> getWeight, double[] result)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            int nf = faces.Count;
            Array.Clear(result, 0, nf * nf);

            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

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


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="getValue"></param>
        /// <param name="getWeight"></param>
        /// <param name="setLaplace"></param>
        /// <param name="parallel"></param>
        public static void UnifyVertexNormals<V, E, F>(this HeStructure<V, E, F> mesh, Property<V, Vec3d> normal, V start)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            foreach (var he in mesh.GetVerticesBreadthFirst2(start.Yield()))
            {
                var v = he.Start;
                var n = normal.Get(v);

                if (Vec3d.Dot(n, normal.Get(he.End)) < 0.0)
                    normal.Set(v, -n);
            }
        }

        #endregion

        #endregion
    }
}
