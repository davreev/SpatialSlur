using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
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
    public abstract class HalfedgeList<S, EE, VV, E, V> : HeElementList<S, E>
        where S: HeStructure<S, EE, VV, E, V>
        where EE : HalfedgeList<S, EE, VV, E, V>
        where VV : HeVertexList<S, EE, VV, E, V>
        where E : Halfedge<E, V>
        where V : HeVertex<E, V>
    {
        /// <summary>
        /// Creates a new pair of halfedges and adds them to the list.
        /// </summary>
        /// <returns></returns>
        internal E AddPair()
        {
            var he0 = CreateElement();
            var he1 = CreateElement();

            Halfedge<E, V>.MakeTwins(he0, he1);
            Add(he0);
            Add(he1);

            return he0;
        }


        #region Attributes

        /// <summary>
        /// Returns the number of components and the connected component index of each edge in the mesh.
        /// </summary>
        /// <returns></returns>
        public int GetEdgeComponentMap(IList<int> result)
        {
            var stack = new Stack<E>();
            int currTag = NextTag;
            int currComp = 0;

            // edge DFS search
            for (int i = 0; i < Count; i += 2)
            {
                var he = this[i];

                // unused edges don't belong to any component
                if (he.IsUnused)
                {
                    result[i >> 1] = -1;
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
                    result[he0.Index >> 1] = currComp;

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
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeDeltas(IReadOnlyList<double> vertexValues, IList<double> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he = this[j];
                    if (he.IsUnused) continue;

                    var v = he.GetDelta(vertexValues);
                    result[j] = v;
                    result[j + 1] = -v;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeDeltas(IReadOnlyList<Vec2d> vertexValues, IList<Vec2d> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he = this[j];
                    if (he.IsUnused) continue;

                    var v = he.GetDelta(vertexValues);
                    result[j] = v;
                    result[j + 1] = -v;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeDeltas(IReadOnlyList<Vec3d> vertexValues, IList<Vec3d> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he = this[j];
                    if (he.IsUnused) continue;

                    var v = he.GetDelta(vertexValues);
                    result[j] = v;
                    result[j + 1] = -v;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetEdgeDeltas(IReadOnlyList<double> vertexValues, IList<double> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = this[i << 1];
                    if (he.IsUnused) continue;

                    result[i] = he.GetDelta(vertexValues);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetEdgeDeltas(IReadOnlyList<Vec2d> vertexValues, IList<Vec2d> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = this[i << 1];
                    if (he.IsUnused) continue;

                    result[i] = he.GetDelta(vertexValues);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetEdgeDeltas(IReadOnlyList<Vec3d> vertexValues, IList<Vec3d> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = this[i << 1];
                    if (he.IsUnused) continue;

                    result[i] = he.GetDelta(vertexValues);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }

        #endregion


        #region Geometric Attributes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeLengths(IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he = this[j];
                    if (he.IsUnused) continue;
                    result[j] = result[j + 1] = he.GetLength(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetEdgeLengths(IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = this[i << 1];
                    if (he.IsUnused) continue;
                    result[i] = he.GetLength(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeTangents(IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he = this[j];
                    if (he.IsUnused) continue;

                    var v = he.GetDelta(vertexPositions);
                    v.Unitize();
                    result[j] = v;
                    result[j + 1] = -v;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetEdgeTangents(IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = this[i << 1];
                    if (he.IsUnused) continue;

                    var v = he.GetDelta(vertexPositions);
                    v.Unitize();
                    result[i] = v;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
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
    public abstract class HalfedgeList<S, EE, VV, FF, E, V, F> : HalfedgeList<S, EE, VV, E, V>
        where S : HeStructure<S, EE, VV, FF, E, V, F>
        where EE : HalfedgeList<S, EE, VV, FF, E, V, F>
        where VV : HeVertexList<S, EE, VV, FF, E, V, F>
        where FF : HeFaceList<S, EE, VV, FF, E, V, F>
        where E : Halfedge<E, V, F>
        where V : HeVertex<E, V, F>
        where F : HeFace<E, V, F>
    {
        /// <summary>
        /// Creates a new pair of halfedges between the given vertices and add them to the list.
        /// Returns the halfedge starting from v0.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        internal E AddPair(V v0, V v1)
        {
            var he = AddPair();
            he.Start = v0;
            he.Twin.Start = v1;
            return he;
        }

        #region Geometric Attributes

        /// <summary>
        /// Calculates the symmetric cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Pinkall and Polthier's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCotangentWeights1(IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he0 = this[j];
                    if (he0.IsUnused) continue;

                    var he1 = this[j + 1];
                    double w = 0.0;

                    if (he0.Face != null) w += he0.GetCotangent(vertexPositions);
                    if (he1.Face != null) w += he1.GetCotangent(vertexPositions);

                    result[j] = result[j + 1] = w * 0.5;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// Returns the area-dependant cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCotangentWeights2(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> vertexAreas, IList<double> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he0 = this[j];
                    if (he0.IsUnused) continue;

                    var he1 = this[j + 1];
                    double w = 0.0;

                    if (he0.Face != null) w += he0.GetCotangent(vertexPositions);
                    if (he1.Face != null) w += he1.GetCotangent(vertexPositions);
                    w *= 0.5;

                    result[j] = w / vertexAreas[he0.Start.Index];
                    result[j + 1] = w / vertexAreas[he1.Start.Index];
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// Returns the area-dependant cotangent weight for each halfedge.
        /// Also rerturns barycentric dual area of each vertex which is calculated in the process.
        /// Assumes triangular faces.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="weightsOut"></param>
        /// <param name="vertexAreasOut"></param>
        public void GetCotangentWeights2(IReadOnlyList<Vec3d> vertexPositions, IList<double> weightsOut, IList<double> vertexAreasOut)
        {
            const double t = 1.0 / 6.0; // 1.0 / 3.0 * 0.5
            weightsOut.SetRange(0.0, 0, Count);
            vertexAreasOut.SetRange(0.0, 0, Owner.Vertices.Count);

            // accumulate cotangent weights and vertex areas
            for (int i = 0; i < Count; i++)
            {
                var he = this[i];
                if (he.IsUnused || he.Face == null) continue;
                int vi = he.Start.Index;

                Vec3d p = vertexPositions[he.Previous.Start.Index];
                Vec3d v0 = vertexPositions[vi] - p;
                Vec3d v1 = vertexPositions[he.End.Index] - p;

                double a = Vec3d.Cross(v0, v1).Length;
                weightsOut[i - (i & 1)] += v0 * v1 / a; // increment at index of first edge in pair
                vertexAreasOut[vi] += a * t; // 1/3rd the triangular area
            }

            // normalize weights by vertex areas
            for (int i = 0; i < Count; i += 2)
            {
                var he = this[i];
                if (he.IsUnused) continue;

                double w = weightsOut[i] * 0.5;
                weightsOut[i] = w / vertexAreasOut[he.Start.Index];
                weightsOut[i + 1] = w / vertexAreasOut[he.End.Index];
            }
        }


        /// <summary>
        ///  Returns the symmetric, area-dependant, cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Levy and Vallet's derivation of the symmetric Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCotangentWeights3(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> vertexAreas, IList<double> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i << 1;
                    var he0 = this[j];
                    if (he0.IsUnused) continue;

                    var he1 = this[j + 1];
                    double w = 0.0;

                    if (he0.Face != null) w += he0.GetCotangent(vertexPositions);
                    if (he1.Face != null) w += he1.GetCotangent(vertexPositions);

                    result[j] = result[j + 1] = w * 0.5 / Math.Sqrt(vertexAreas[he0.Start.Index] * vertexAreas[he1.Start.Index]);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }


        /// <summary>
        /// Returns the symmetric, area-dependant, cotangent weight for each halfedge.
        /// Also rerturns vertex areas which are calculated in the process.
        /// Assumes triangular faces.
        /// Based on Levy and Vallet's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="weightsOut"></param>
        /// <param name="vertexAreasOut"></param>
        public void GetCotangentWeights3(IReadOnlyList<Vec3d> vertexPositions, IList<double> weightsOut, IList<double> vertexAreasOut)
        {
            const double t = 1.0 / 6.0; // 1.0 / 3.0 * 0.5
            weightsOut.SetRange(0.0, 0, Count);
            vertexAreasOut.SetRange(0.0, 0, Owner.Vertices.Count);

            // accumulate cotangent weights and vertex areas
            for (int i = 0; i < Count; i++)
            {
                var he = this[i];
                if (he.IsUnused || he.Face == null) continue;
                var vi = he.Start.Index;

                Vec3d p = vertexPositions[he.Previous.Start.Index];
                Vec3d v0 = vertexPositions[vi] - p;
                Vec3d v1 = vertexPositions[he.End.Index] - p;

                double a = Vec3d.Cross(v0, v1).Length;
                weightsOut[i - (i & 1)] += v0 * v1 / a; // increment at index of first edge in pair
                vertexAreasOut[vi] += a * t; // 1/3rd the triangular area
            }

            // symmetrically normalize weights by vertex areas
            for (int i = 0; i < Count; i += 2)
            {
                var he = this[i];
                if (he.IsUnused) continue;

                double w = weightsOut[i] * 0.5;
                weightsOut[i] = weightsOut[i + 1] = w / Math.Sqrt(vertexAreasOut[he.Start.Index] * vertexAreasOut[he.End.Index]);
            }
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="faceNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetDihedralAngles(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceNormals, IList<double> result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = this[i << 1];
                    if (he.IsUnused || he.IsBoundary) continue;
                    result[i] = he.GetDihedralAngle(vertexPositions, faceNormals);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), func);
            else
                func(Tuple.Create(0, Count >> 1));
        }

        #endregion
    }
}
