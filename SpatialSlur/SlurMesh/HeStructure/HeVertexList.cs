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
    [Serializable]
    public abstract class HeVertexList<S, EE, VV, E, V> : HeElementList<S, V>
        where S : HeStructure<S, EE, VV, E, V>
        where EE : HalfedgeList<S, EE, VV, E, V>
        where VV : HeVertexList<S, EE, VV, E, V>
        where E : Halfedge<E, V>
        where V : HeVertex<E, V>
    {
        /// <summary>
        /// 
        /// </summary>
        public V Add()
        {
            var v = CreateElement();
            Add(v);
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantity"></param>
        public void Add(int quantity)
        {
            for (int i = 0; i < quantity; i++)
                Add(CreateElement());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public IEnumerable<V> GetBreadthFirstOrder(V start)
        {
            OwnsCheck(start);

            if (start.IsUnused)
                yield break;

            var queue = new Queue<V>();
            int currTag = NextTag;

            queue.Enqueue(start);
            start.Tag = currTag;

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public IEnumerable<V> GetDepthFirstOrder(V start)
        {
            OwnsCheck(start);

            if (start.IsUnused)
                yield break;

            var stack = new Stack<V>();
            int currTag = NextTag;

            stack.Push(start);
            start.Tag = currTag;

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


        /// <summary>
        /// Returns the number of common neigbours shared between v0 and v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public int CountCommonNeighbours(V v0, V v1)
        {
            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0.IsUnused || v1.IsUnused)
                return 0;

            return CountCommonNeighboursImpl(v0, v1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal int CountCommonNeighboursImpl(V v0, V v1)
        {
            int currTag = NextTag;

            // flag neighbours of v1
            foreach (var v in v1.ConnectedVertices)
                v.Tag = currTag;

            // count flagged neighbours of v0
            int count = 0;
            foreach (var v in v0.ConnectedVertices)
                if (v.Tag == currTag) count++;

            return count;
        }


        /// <summary>
        /// Returns all common neigbours shared between v0 and v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public IEnumerable<V> GetCommonNeighbours(V v0, V v1)
        {
            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0.IsUnused || v1.IsUnused)
                return Enumerable.Empty<V>();

            return GetCommonNeighboursImpl(v0, v1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal IEnumerable<V> GetCommonNeighboursImpl(V v0, V v1)
        {
            int currTag = NextTag;

            // flag neighbours of v1
            foreach (var v in v1.ConnectedVertices)
                v.Tag = currTag;

            // collect flagged neighbours of v0
            foreach (var v in v0.ConnectedVertices)
                if (v.Tag == currTag) yield return v;
        }

        #region Attributes

        /// <summary>
        /// Calculates the minimum topological depth of each vertex from a set of source vertices.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetVertexDepths(IEnumerable<V> sources, IList<int> result)
        {
            var queue = new Queue<V>();
            result.SetRange(Int32.MaxValue, 0, Count);

            // enqueue sources and set to zero
            foreach (var v in sources)
            {
                OwnsCheck(v);
                if (v.IsUnused) continue;

                result[v.Index] = 0;
                queue.Enqueue(v);
            }

            // bfs
            while (queue.Count > 0)
            {
                var v0 = queue.Dequeue();
                int t0 = result[v0.Index] + 1;

                foreach (var v1 in v0.ConnectedVertices)
                {
                    int i1 = v1.Index;
                    if (t0 < result[i1])
                    {
                        result[i1] = t0;
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the minimum topological distance to each vertex from a set of source vertices.
        /// Travel cost is defined per edge.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeCosts"></param>
        /// <param name="result"></param>
        public void GetVertexDistances(IEnumerable<V> sources, IReadOnlyList<double> edgeCosts, IList<double> result)
        {
            var queue = new Queue<V>();
            result.SetRange(Double.PositiveInfinity, 0, Count);

            // enqueue sources and set to zero
            foreach (var v in sources)
            {
                OwnsCheck(v);
                if (v.IsUnused) continue;

                result[v.Index] = 0.0;
                queue.Enqueue(v);
            }

            // TODO compare to priority queue implementation
            while (queue.Count > 0)
            {
                var v0 = queue.Dequeue();
                double t0 = result[v0.Index];

                foreach (var he in v0.IncomingHalfedges)
                {
                    var v1 = he.Start;
                    int i1 = v1.Index;
                    double t1 = t0 + edgeCosts[he.Index >> 1];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the minimum topological distance to each vertex from a set of source vertices.
        /// Travel cost is defined per half-edge allowing for directed representations.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="halfedgeCosts"></param>
        /// <param name="result"></param>
        public void GetVertexDistancesDirected(IEnumerable<V> sources, IReadOnlyList<double> halfedgeCosts, IList<double> result)
        {
            var queue = new Queue<V>();
            result.SetRange(Double.PositiveInfinity, 0, Count);

            // enqueue sources and set to zero
            foreach (var v in sources)
            {
                OwnsCheck(v);
                if (v.IsUnused) continue;

                result[v.Index] = 0.0;
                queue.Enqueue(v);
            }

            // TODO compare to priority queue implementation
            while (queue.Count > 0)
            {
                var v0 = queue.Dequeue();
                double t0 = result[v0.Index];

                foreach (var he in v0.IncomingHalfedges)
                {
                    var v1 = he.Start;
                    int i1 = v1.Index;
                    double t1 = t0 + halfedgeCosts[he.Index];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(v1);
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
                var v0 = this[i];
                if (v0.IsUnused) continue;

                foreach (var v1 in v0.ConnectedVertices)
                    result[i * n + v1.Index] = 1.0;
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
                var v = this[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (var he in v.OutgoingHalfedges)
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
                var v = this[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (var he in v.OutgoingHalfedges)
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
    public abstract class HeVertexList<S, EE, VV, FF, E, V, F> : HeVertexList<S, EE, VV, E, V>
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
        /// <returns></returns>
        public int CountBoundaryVertices()
        {
            int n = 0;

            for (int i = 0; i < Count; i++)
            {
                var v = this[i];
                if (!v.IsUnused && v.IsBoundary) n++;
            }

            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<V> GetBoundaryVertices()
        {
            for (int i = 0; i < Count; i++)
            {
                var v = this[i];
                if (!v.IsUnused && v.IsBoundary) yield return v;
            }
        }


        /// <summary>
        /// Appends all boundary vertices to the given list.
        /// </summary>
        /// <returns></returns>
        public void GetBoundaryVertices(List<V> result)
        {
            for (int i = 0; i < Count; i++)
            {
                var v = this[i];
                if (!v.IsUnused && v.IsBoundary) result.Add(v);
            }
        }


        /// <summary>
        /// Returns the number of common adjacent faces shared between v0 and v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public int CountCommonFaces(V v0, V v1)
        {
            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0.IsUnused || v1.IsUnused)
                return 0;

            return CountCommonFacesImpl(v0, v1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal int CountCommonFacesImpl(V v0, V v1)
        {
            int currTag = Owner.Faces.NextTag;

            // flag neighbours of v1
            foreach (var f in v1.SurroundingFaces)
                f.Tag = currTag;

            // count flagged neighbours of v0
            int count = 0;
            foreach (var f in v0.SurroundingFaces)
                if (f.Tag == currTag) count++;

            return count;
        }


        /// <summary>
        /// Returns all common adjacent faces shared between v0 and v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public IEnumerable<F> GetCommonFaces(V v0, V v1)
        {
            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0.IsUnused || v1.IsUnused)
                return Enumerable.Empty<F>();

            return GetCommonFacesImpl(v0, v1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal IEnumerable<F> GetCommonFacesImpl(V v0, V v1)
        {
            int currTag = Owner.Faces.NextTag;

            // flag neighbours of v1
            foreach (var f in v1.SurroundingFaces)
                f.Tag = currTag;

            // collect flagged neighbours of v0
            foreach (var f in v0.SurroundingFaces)
                if (f.Tag == currTag) yield return f;
        }


        #region Geometric Attributes

        /// <summary>
        /// Calculated as the barycentric dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        public void GetVertexAreasBarycentric(IReadOnlyList<Vec3d> vertexPositions, IList<double> result)
        {
            const double t = 1.0 / 6.0; // (1.0 / 3.0) * 0.5
            var faces = Owner.Faces;
            result.SetRange(0.0, 0, Count);

            // distribute face areas to vertices
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                double a = f.First.GetNormal(vertexPositions).Length * t;
                foreach (var v in f.Vertices) result[v.Index] += a;
            }
        }


        /// <summary>
        /// Calculated as the circumcentric dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        public void GetVertexAreasCircumcentric(IReadOnlyList<Vec3d> vertexPositions, IList<double> result)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Calculated as the mixed dual area around each vertex as per http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 7)
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        public void GetVertexAreasMixed(IReadOnlyList<Vec3d> vertexPositions, IList<double> result)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion
    }
}
