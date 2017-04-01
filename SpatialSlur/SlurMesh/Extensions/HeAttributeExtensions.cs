using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Collection of attribute getters that don't require access to the parent mesh and don't make assumptions about the order of elements.
    /// </summary>
    public static class HeAttributeExtensions
    {
        #region IReadOnlyList<Halfedge>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeLengths<E, V>(this IReadOnlyList<Halfedge<E, V>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused) continue;
                    result[i] = he.GetLength(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeDeltas<E, V>(this IReadOnlyList<Halfedge<E, V>> halfedges, IReadOnlyList<double> vertexValues, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused) continue;
                    result[i] = he.GetDelta(vertexValues);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeDeltas<E, V>(this IReadOnlyList<Halfedge<E, V>> halfedges, IReadOnlyList<Vec2d> vertexValues, IList<Vec2d> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused) continue;
                    result[i] = he.GetDelta(vertexValues);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeDeltas<E, V>(this IReadOnlyList<Halfedge<E, V>> halfedges, IReadOnlyList<Vec3d> vertexValues, IList<Vec3d> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused) continue;
                    result[i] = he.GetDelta(vertexValues);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeTangents<E, V>(this IReadOnlyList<Halfedge<E, V>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused) continue;

                    var v = he.GetDelta(vertexPositions);
                    v.Unitize();
                    result[i] = v;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// Calculates the angle between each halfedge and its previous.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeAngles<E, V>(this IReadOnlyList<Halfedge<E, V>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused) continue;
                    result[i] = he.GetAngle(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeAngles<E, V>(this IReadOnlyList<Halfedge<E, V>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> edgeLengths, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he0 = halfedges[i];
                    if (he0.IsUnused) continue;

                    var he1 = he0.Previous;
                    double d = edgeLengths[he0.Index >> 1] * edgeLengths[he1.Index >> 1];

                    if (d > 0.0)
                    {
                        var p = vertexPositions[he0.Start.Index];
                        var v0 = p - vertexPositions[he1.Start.Index];
                        var v1 = vertexPositions[he0.End.Index] - p;
                        result[i] = Math.Acos(SlurMath.Clamp(v0 * v1 / d, -1.0, 1.0)); // clamp dot product to remove noise
                    }
                    else
                    {
                        result[i] = Double.NaN;
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// Calculates the area associated with each halfedge.
        /// This is calculated as W in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeAreas<E,V,F>(this IReadOnlyList<Halfedge<E, V, F>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    var f = he.Face;
                    if (he.IsUnused || f == null) continue;
                    result[i] = he.GetArea(vertexPositions, faceCenters[f.Index]);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite each halfedge.
        /// Assumes triangular faces.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeCotangents<E, V, F>(this IReadOnlyList<Halfedge<E, V, F>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused || he.Face == null) continue;
                    result[i] = he.GetCotangent(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeNormals<E, V, F>(this IReadOnlyList<Halfedge<E, V, F>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused) continue;
                    result[i] = he.GetNormal(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetHalfedgeUnitNormals<E,V,F>(this IReadOnlyList<Halfedge<E, V, F>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i];
                    if (he.IsUnused) continue;

                    Vec3d v = he.GetNormal(vertexPositions);
                    v.Unitize();
                    result[i] = v;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count), func);
            else
                func(Tuple.Create(0, halfedges.Count));
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="halfedges"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetDihedralAngles<E, V, F>(this IReadOnlyList<Halfedge<E, V, F>> halfedges, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceNormals, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = halfedges[i << 1];
                    if (he.IsUnused || he.IsBoundary) continue;
                    result[i] = he.GetDihedralAngle(vertexPositions, faceNormals);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, halfedges.Count >> 1), func);
            else
                func(Tuple.Create(0, halfedges.Count >> 1));
        }

        #endregion


        #region IReadOnlyList<HeVertex>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetVertexDegrees<E,V>(this IReadOnlyList<HeVertex<E, V>> vertices, IList<int> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;
                    result[i] = v.Degree;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian<E,V>(this IReadOnlyList<HeVertex<E, V>> vertices, IReadOnlyList<double> vertexValues, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = vertices[i];
                    if (v0.IsUnused) continue;

                    double t = vertexValues[v0.Index];
                    double sum = 0.0;
                    int n = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += (vertexValues[v1.Index] - t);
                        n++;
                    }

                    result[i] = sum / n;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian<E, V>(this IReadOnlyList<HeVertex<E, V>> vertices, IReadOnlyList<Vec2d> vertexValues, IList<Vec2d> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = vertices[i];
                    if (v0.IsUnused) continue;

                    var t = vertexValues[v0.Index];
                    var sum = new Vec2d();
                    int n = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += (vertexValues[v1.Index] - t);
                        n++;
                    }

                    result[i] = sum / n;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian<E, V>(this IReadOnlyList<HeVertex<E, V>> vertices, IReadOnlyList<Vec3d> vertexValues, IList<Vec3d> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = vertices[i];
                    if (v0.IsUnused) continue;

                    var t = vertexValues[v0.Index];
                    var sum = new Vec3d();
                    int n = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += (vertexValues[v1.Index] - t);
                        n++;
                    }

                    result[i] = sum / n;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexValues"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian<E,V>(this IReadOnlyList<HeVertex<E, V>> vertices, IReadOnlyList<double> vertexValues, IReadOnlyList<double> halfedgeWeights, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;

                    double t = vertexValues[v.Index];
                    double sum = 0.0;

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (vertexValues[he.End.Index] - t) * halfedgeWeights[he.Index];

                    result[i] = sum;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexValues"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian<E, V>(this IReadOnlyList<HeVertex<E, V>> vertices, IReadOnlyList<Vec2d> vertexValues, IReadOnlyList<double> halfedgeWeights, IList<Vec2d> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;

                    var t = vertexValues[v.Index];
                    var sum = new Vec2d();

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (vertexValues[he.End.Index] - t) * halfedgeWeights[he.Index];

                    result[i] = sum;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates the Laplacian of the given vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexValues"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian<E, V>(this IReadOnlyList<HeVertex<E, V>> vertices, IReadOnlyList<Vec3d> vertexValues, IReadOnlyList<double> halfedgeWeights, IList<Vec3d> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;

                    var t = vertexValues[v.Index];
                    var sum = new Vec3d();

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (vertexValues[he.End.Index] - t) * halfedgeWeights[he.Index];

                    result[i] = sum;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Normalizes halfedge weights such that the weights of outgoing halfedges around each vertex sum to 1.
        /// Note that this breaks weight symmetry between halfedge pairs.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        public static void NormalizeHalfedgeWeights<E, V>(this IReadOnlyList<HeVertex<E, V>> vertices, IList<double> halfedgeWeights, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;
                    v.OutgoingHalfedges.Normalize(halfedgeWeights);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates the morse smale classification for each vertex (0 = normal, 1 = minima, 2 = maxima, 3 = saddle).
        /// Assumes halfedges are radially sorted around the given vertices.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetMorseSmaleClassification<E, V>(this IReadOnlyList<HeVertex<E, V>> vertices, IReadOnlyList<double> vertexValues, IList<int> result, bool parallel = false)
            where E : Halfedge<E, V>
            where V : HeVertex<E, V>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = vertices[i];
                    double t0 = vertexValues[v0.Index];

                    // check first neighbour
                    var he0 = v0.First.Twin;
                    double t1 = vertexValues[he0.Start.Index];

                    bool last = (t1 < t0); // was the last neighbour lower?
                    int count = 0; // number of discontinuities
                    he0 = he0.Next.Twin;

                    // circulate remaining neighbours
                    var he1 = he0;
                    do
                    {
                        t1 = vertexValues[he1.Start.Index];

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

                        he1 = he1.Next.Twin;
                    } while (he1 != he0);

                    // classify current vertex based on number of discontinuities
                    switch (count)
                    {
                        case 0:
                            result[i] = (last) ? 2 : 1;
                            break;
                        case 2:
                            result[i] = 0;
                            break;
                        default:
                            result[i] = 3;
                            break;
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculated as the sum of halfedge areas around each vertex.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetVertexAreas<E, V, F>(this IReadOnlyList<HeVertex<E, V, F>> vertices, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;
                    double sum = 0.0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        var f = he.Face;
                        if (f == null) continue;
                        sum += he.GetArea(vertexPositions, faceCenters[f.Index]);
                    }

                    result[i] = sum;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates the area associated with each vertex as the sum of halfedge areas.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="halfedgeAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetVertexAreas<E, V, F>(this IReadOnlyList<HeVertex<E, V, F>> vertices, IReadOnlyList<double> halfedgeAreas, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;
                    result[i] = v.OutgoingHalfedges.Sum(halfedgeAreas);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculated as 1/3 the sum of face areas around each vertex.
        /// Assumes triangular faces.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="faceAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetVertexAreasBarycentric<E, V, F>(this IReadOnlyList<HeVertex<E, V, F>> vertices, IReadOnlyList<double> faceAreas, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            const double inv3 = 1.0 / 3.0;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;
                    result[i] = v.SurroundingFaces.Sum(faceAreas) * inv3;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates the circle packing radii for each vertex.
        /// Assumes the mesh is a circle packing mesh http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetCirclePackingRadii<E, V, F>(this IReadOnlyList<HeVertex<E, V, F>> vertices, IReadOnlyList<double> edgeLengths, IList<double> result, bool parallel = false)
            where E : Halfedge<E, V, F>
            where V : HeVertex<E, V, F>
            where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue; // skip unused vertices

                    double sum = 0.0;
                    int n = 0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        if (he.Face == null) continue; // skip boundary edges
                        sum += (edgeLengths[he.Index >> 1] + edgeLengths[he.Previous.Index >> 1] - edgeLengths[he.Next.Index >> 1]) * 0.5;
                        n++;
                    }

                    result[v.Index] = sum / n;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculated as the signed magnitude of the vertex laplacian with respect to the vertex normal.
        /// http://www.hao-li.com/cs599-ss2015/slides/Lecture04.1.pdf (p 47)
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexNormals"></param>
        /// <param name="vertexLaplacian"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetMeanCurvature<E, V, F>(this IReadOnlyList<HeVertex<E, V, F>> vertices, IReadOnlyList<Vec3d> vertexNormals, IReadOnlyList<Vec3d> vertexLaplacian, IList<double> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;

                    if (v.IsBoundary)
                    {
                        result[i] = 0.0;
                    }
                    else
                    {
                        int vi = v.Index;
                        Vec3d vlap = vertexLaplacian[vi];
                        result[i] = -Math.Sign(vlap * vertexNormals[vi]) * vlap.Length * 0.5;
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculated as the angle defect around each vertex.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetGaussianCurvature<E, V, F>(this IReadOnlyList<HeVertex<E, V, F>> vertices, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> vertexAreas, IList<double> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;

                    if (v.IsBoundary)
                    {
                        result[i] = 0.0;
                    }
                    else
                    {
                        double sum = 0.0;
                        foreach (var he in v.OutgoingHalfedges)
                            sum += Math.PI - he.GetAngle(vertexPositions);

                        result[i] = (SlurMath.TwoPI - sum) / vertexAreas[v.Index];
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculated as the angle defect around each vertex.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetGaussianCurvature<E, V, F>(this IReadOnlyList<HeVertex<E, V, F>> vertices, IReadOnlyList<double> halfedgeAngles, IReadOnlyList<double> vertexAreas, IList<double> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;

                    if (v.IsBoundary)
                    {
                        result[i] = 0.0;
                    }
                    else
                    {
                        double sum = 0.0;
                        foreach (var he in v.OutgoingHalfedges)
                            sum += Math.PI - halfedgeAngles[he.Index];

                        result[i] = (SlurMath.TwoPI - sum) / vertexAreas[v.Index];
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates vertex normals as the area-weighted sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetVertexNormals<E,V,F>(this IReadOnlyList<HeVertex<E, V, F>> vertices, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;
                    result[i] = v.GetNormal(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }


        /// <summary>
        /// Calculates vertex normals as the sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="halfedgeNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetVertexNormals2<E, V, F>(this IReadOnlyList<HeVertex<E, V, F>> vertices, IReadOnlyList<Vec3d> halfedgeNormals, IList<Vec3d> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vertices[i];
                    if (v.IsUnused) continue;
                    result[i] = v.GetNormal2(halfedgeNormals);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, vertices.Count), func);
            else
                func(Tuple.Create(0, vertices.Count));
        }

        #endregion


        #region IReadOnlyList<HeFace>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceDegrees<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IList<int> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.Degree;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Normalizes halfedge weights such that the weights of halfedges within each face sum to 1.
        /// Note that this breaks weight symmetry between halfedge pairs.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        public static void NormalizeHalfedgeWeights<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IList<double> halfedgeWeights, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    f.Halfedges.Normalize(halfedgeWeights);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceBarycenters<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.GetBarycenter(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates the circumcenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceCircumcenters<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.GetCircumcenter(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Returns the incenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceIncenters<E,V,F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.GetIncenter(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Returns the incenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceIncenters<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> edgeLengths, IList<Vec3d> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.GetIncenter(vertexPositions, edgeLengths);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates face normals as the area-weighted sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceNormals<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.GetNormal(vertexPositions);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates face normals as the sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="halfedgeNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceNormals2<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> halfedgeNormals, IList<Vec3d> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.GetNormal2(halfedgeNormals);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates face normals as the normal of the first halfedge in each face.
        /// Face normals are unitized by default.
        /// Assumes triangular faces.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceNormalsTri<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;

                    Vec3d v = f.First.GetNormal(vertexPositions);
                    v.Unitize();
                    result[i] = v;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceAreas<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    var he0 = f.First;

                    if (he0 == he0.Next.Next.Next)
                    {
                        // simplified tri case
                        Vec3d norm = he0.GetNormal(vertexPositions);
                        result[i] = norm.Length * 0.5;
                    }
                    else
                    {
                        // general ngon case
                        Vec3d cen = f.GetBarycenter(vertexPositions);
                        double sum = 0.0;

                        foreach (var he in f.Halfedges)
                            sum += he.GetArea(vertexPositions, cen);
                        
                        result[i] = sum;
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceAreas<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    var he0 = f.First;

                    if (he0 == he0.Next.Next.Next)
                    {
                        // simplified tri case
                        Vec3d norm = he0.GetNormal(vertexPositions);
                        result[i] = norm.Length * 0.5;
                    }
                    else
                    {
                        // general ngon case
                        Vec3d cen = faceCenters[f.Index];
                        double sum = 0.0;

                        foreach (var he in f.Halfedges)
                        {
                            var p0 = vertexPositions[he.Start.Index];
                            var p1 = vertexPositions[he.Start.Index];
                            sum += Vec3d.Cross(p0 - cen, p1 - cen).Length * 0.5;
                        }

                        result[i] = sum;
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Calculates the area of each face.
        /// Assumes triangular faces.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFaceAreasTri<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.First.GetNormal(vertexPositions).Length * 0.5;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }


        /// <summary>
        /// Returns the planar deviation for each face.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="faces"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetFacePlanarity<E, V, F>(this IReadOnlyList<HeFace<E, V, F>> faces, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
             where E : Halfedge<E, V, F>
             where V : HeVertex<E, V, F>
             where F : HeFace<E, V, F>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;

                    var he0 = f.First;
                    var he1 = he0;

                    var p0 = vertexPositions[he1.Start.Index]; he1 = he1.Next;
                    var p1 = vertexPositions[he1.Start.Index]; he1 = he1.Next;
                    var p2 = vertexPositions[he1.Start.Index]; he1 = he1.Next;

                    if (he1 == he0)
                    {
                        // tri case
                        result[i] = 0.0;
                    }
                    else if (he1.Next == he0)
                    {
                        // quad case
                        var p3 = vertexPositions[he1.Start.Index];
                        Vec3d span = GeometryUtil.LineLineShortestVector(p0, p2, p1, p3);
                        result[i] = span.Length;
                    }
                    else
                    {
                        // ngon case
                        var he2 = he1;
                        double sum = 0.0;
                        int count = 0;

                        do
                        {
                            var p3 = vertexPositions[he2.Start.Index];
                            Vec3d span = GeometryUtil.LineLineShortestVector(p0, p2, p1, p3);
                            sum += span.Length;
                            count++;

                            // advance to next set of verts
                            p0 = p1; p1 = p2; p2 = p3;
                            he2 = he2.Next;
                        } while (he2 != he1);

                        result[i] = sum / count;
                    }
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), func);
            else
                func(Tuple.Create(0, faces.Count));
        }

        #endregion


        #region IEnumerable<HeElement>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <returns></returns>
        public static double Sum(this IEnumerable<HeElement> elements, IReadOnlyList<double> elementValues)
        {
            double result = 0.0;

            foreach (var e in elements)
                result += elementValues[e.Index];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <returns></returns>
        public static Vec2d Sum(this IEnumerable<HeElement> elements, IReadOnlyList<Vec2d> elementValues)
        {
            var result = new Vec2d();

            foreach (var e in elements)
                result += elementValues[e.Index];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <returns></returns>
        public static Vec3d Sum(this IEnumerable<HeElement> elements, IReadOnlyList<Vec3d> elementValues)
        {
            var result = new Vec3d();

            foreach (var e in elements)
                result += elementValues[e.Index];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <param name="elementWeights"></param>
        /// <returns></returns>
        public static double Sum(this IEnumerable<HeElement> elements, IReadOnlyList<double> elementValues, IReadOnlyList<double> elementWeights)
        {
            double result = 0.0;

            foreach (var e in elements)
            {
                int ei = e.Index;
                result += elementValues[ei] * elementWeights[ei];
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <param name="elementWeights"></param>
        /// <returns></returns>
        public static Vec2d Sum(this IEnumerable<HeElement> elements, IReadOnlyList<Vec2d> elementValues, IReadOnlyList<double> elementWeights)
        {
            var result = new Vec2d();

            foreach (var e in elements)
            {
                int ei = e.Index;
                result += elementValues[ei] * elementWeights[ei];
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <param name="elementWeights"></param>
        /// <returns></returns>
        public static Vec3d Sum(this IEnumerable<HeElement> elements, IReadOnlyList<Vec3d> elementValues, IReadOnlyList<double> elementWeights)
        {
            var result = new Vec3d();

            foreach (var e in elements)
            {
                int ei = e.Index;
                result += elementValues[ei] * elementWeights[ei];
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <returns></returns>
        public static double Mean(this IEnumerable<HeElement> elements, IReadOnlyList<double> elementValues)
        {
            double result = 0.0;
            int n = 0;

            foreach (var e in elements)
            {
                result += elementValues[e.Index];
                n++;
            }

            return result / n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <returns></returns>
        public static Vec2d Mean(this IEnumerable<HeElement> elements, IReadOnlyList<Vec2d> elementValues)
        {
            var result = new Vec2d();
            int n = 0;

            foreach (var e in elements)
            {
                result += elementValues[e.Index];
                n++;
            }

            return result / n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <returns></returns>
        public static Vec3d Mean(this IEnumerable<HeElement> elements, IReadOnlyList<Vec3d> elementValues)
        {
            var result = new Vec3d();
            int n = 0;

            foreach (var e in elements)
            {
                result += elementValues[e.Index];
                n++;
            }

            return result / n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <param name="elementWeights"></param>
        /// <returns></returns>
        public static double Mean(this IEnumerable<HeElement> elements, IReadOnlyList<double> elementValues, IReadOnlyList<double> elementWeights)
        {
            double result = 0.0;
            double wsum = 0.0;

            foreach (var e in elements)
            {
                int ei = e.Index;
                double w = elementWeights[ei];
                result += elementValues[ei] * w;
                wsum += w;
            }

            return result / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <param name="elementWeights"></param>
        /// <returns></returns>
        public static Vec2d Mean(this IEnumerable<HeElement> elements, IReadOnlyList<Vec2d> elementValues, IReadOnlyList<double> elementWeights)
        {
            var result = new Vec2d();
            double wsum = 0.0;

            foreach (var e in elements)
            {
                int ei = e.Index;
                double w = elementWeights[ei];
                result += elementValues[ei] * w;
                wsum += w;
            }

            return result / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <param name="elementWeights"></param>
        /// <returns></returns>
        public static Vec3d Mean(this IEnumerable<HeElement> elements, IReadOnlyList<Vec3d> elementValues, IReadOnlyList<double> elementWeights)
        {
            var result = new Vec3d();
            double wsum = 0.0;

            foreach (var e in elements)
            {
                int ei = e.Index;
                double w = elementWeights[ei];
                result += elementValues[ei] * w;
                wsum += w;
            }

            return result / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementValues"></param>
        /// <returns></returns>
        public static void Normalize(this IEnumerable<HeElement> elements, IList<double> elementValues)
        {
            double sum = 0.0;
            foreach (var e in elements)
                sum += elementValues[e.Index];

            if (sum > 0.0)
            {
                double t = 1 / sum;
                foreach (var e in elements)
                    elementValues[e.Index] *= t;
            }
        }

        #endregion
    }
}
