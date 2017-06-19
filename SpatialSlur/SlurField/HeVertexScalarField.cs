using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 * 
 * TODO finish implementation
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public class HeVertexScalarField : HeVertexField<double>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        public HeVertexScalarField(HeMeshVertexList vertices)
            : base(vertices)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeVertexScalarField Duplicate()
        {
            var copy = new HeVertexScalarField(Vertices);
            copy.Set(this);
            return copy;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        public override double ValueAt(int i0, int i1, int i2, double w0, double w1, double w2)
        {
            return Values[i0] * w0 + Values[i1] * w1 + Values[i2] * w2;
        }


        /// <summary>
        /// Assumes weights sum to 1.0
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <param name="value"></param>
        public void SetAt(int i0, int i1, int i2, double w0, double w1, double w2, double value)
        {
            Values[i0] += (value - Values[i0]) * w0;
            Values[i1] += (value - Values[i1]) * w1;
            Values[i2] += (value - Values[i2]) * w2;
        }


        /// <summary>
        /// Assumes weights sum to 1.0
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <param name="amount"></param>
        public void IncrementAt(int i0, int i1, int i2, double w0, double w1, double w2, double amount)
        {
            Values[i0] += amount * w0;
            Values[i1] += amount * w1;
            Values[i2] += amount * w2;
        }


        /// <summary>
        /// Calculates the Laplacian using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public HeVertexScalarField GetLaplacian(bool parallel = false)
        {
            HeVertexScalarField result = new HeVertexScalarField(Vertices);
            Vertices.GetLaplacian(Values, result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public void GetLaplacian(HeVertexScalarField result, bool parallel = false)
        {
            Vertices.GetLaplacian(Values, result.Values, parallel);
        }


        /// <summary>
        /// Calculates the Laplacian using a custom weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="hedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public HeVertexScalarField GetLaplacian(IReadOnlyList<double> hedgeWeights, bool parallel = false)
        {
            HeVertexScalarField result = new HeVertexScalarField(Vertices);
            Vertices.GetLaplacian(Values, hedgeWeights, result.Values, parallel);
            return result;
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IReadOnlyList<double> hedgeWeights, HeVertexScalarField result, bool parallel = false)
        {
            Vertices.GetLaplacian(Values, hedgeWeights, result.Values, parallel);
        }


        /// <summary>
        /// Calculates the gradient as the average of directional derivatives around each vertex.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public HeVertexVectorField GetGradient(IReadOnlyList<Vec3d> vertexPositions, bool parallel = false)
        {
            HeVertexVectorField result = new HeVertexVectorField(Vertices);
            GetGradient(vertexPositions, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="parallel"></param>
        public void GetGradient(IReadOnlyList<Vec3d> vertexPositions, HeVertexVectorField result, bool parallel = false)
        {
            GetGradient(vertexPositions, result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        private void GetGradient(IReadOnlyList<Vec3d> vertexPositions, Vec3d[] result,  bool parallel = false)
        {
            // TODO refactor according to http://libigl.github.io/libigl/tutorial/tutorial.html
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = Vertices[i];
                    if (v0.IsUnused) continue;

                    var p0 = vertexPositions[i];
                    double t0 = Values[i];
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        int i1 = v1.Index;

                        Vec3d d = vertexPositions[i1] - p0;
                        double m = 1.0 / d.SquareLength;

                        d *= (Values[i1] - t0) * m; // unitized directional derivative
                        sum += d;
                        n++;
                    }

                    result[i] = sum / n;
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
        /// <param name="vertexPositions"></param>
        /// <param name="hedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public HeVertexVectorField GetGradient(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> hedgeWeights, bool parallel = false)
        {
            HeVertexVectorField result = new HeVertexVectorField(Vertices);
            GetGradient(vertexPositions, hedgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="hedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> hedgeWeights, HeVertexVectorField result, bool parallel = false)
        {
            GetGradient(vertexPositions, hedgeWeights, result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="hedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> hedgeWeights, Vec3d[] result, bool parallel = false)
        {
            // TODO refactor according to http://libigl.github.io/libigl/tutorial/tutorial.html 
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = Vertices[i];
                    if (v.IsUnused) continue;

                    var p0 = vertexPositions[i];
                    double t0 = Values[i];
                    Vec3d sum = new Vec3d();

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        var i1 = he.End.Index;

                        Vec3d d = vertexPositions[i1] - p0;
                        double m = 1.0 / d.SquareLength;
                        double w = hedgeWeights[he.Index];

                        d *= (Values[i1] - t0) * m * w; // unitized directional derivative
                        sum += d;
                    }

                    result[i] = sum;
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
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceMeans(IList<double> result, bool parallel = false)
        {
            var faces = Vertices.Owner.Faces;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.Vertices.Mean(Values);
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
        /// <param name="faceValues"></param>
        /// <param name="parallel"></param>
        public void SetVertexMeans(IReadOnlyList<double> faceValues, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = Vertices[i];
                    if (v.IsUnused) continue;
                    Values[i] = v.SurroundingFaces.Mean(Values);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }
    }
}
