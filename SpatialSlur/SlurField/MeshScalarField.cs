using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MeshScalarField : MeshField<double>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshScalarField(HeMesh mesh)
            : base(mesh)
        {
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        public MeshScalarField(MeshScalarField other)
            : base(other)
        {
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
        public MeshScalarField GetLaplacian(bool parallel = false)
        {
            MeshScalarField result = new MeshScalarField(Mesh);
            Mesh.Vertices.GetLaplacian(Values, result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public void GetLaplacian(MeshScalarField result, bool parallel = false)
        {
            Mesh.Vertices.GetLaplacian(Values, result.Values, parallel);
        }


        /// <summary>
        /// Calculates the Laplacian using a custom weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshScalarField GetLaplacian(IReadOnlyList<double> halfedgeWeights, bool parallel = false)
        {
            MeshScalarField result = new MeshScalarField(Mesh);
            Mesh.Vertices.GetLaplacian(Values, halfedgeWeights, result.Values, parallel);
            return result;
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IReadOnlyList<double> halfedgeWeights, MeshScalarField result, bool parallel = false)
        {
            Mesh.Vertices.GetLaplacian(Values, halfedgeWeights, result.Values, parallel);
        }


        /// <summary>
        /// Calculates the gradient as the average of directional derivatives around each vertex.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshVectorField GetGradient(IReadOnlyList<Vec3d> vertexPositions, bool parallel = false)
        {
            MeshVectorField result = new MeshVectorField(Mesh);
            GetGradient(vertexPositions, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vertexPositions"></param>
        /// <param name="parallel"></param>
        public void GetGradient(IReadOnlyList<Vec3d> vertexPositions, MeshVectorField result, bool parallel = false)
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
            var verts = Mesh.Vertices;
            var vals = Values;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = verts[i];
                    if (v0.IsUnused) continue;

                    var p0 = vertexPositions[i];
                    double t0 = vals[i];
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        int i1 = v1.Index;

                        Vec3d d = vertexPositions[i1] - p0;
                        double m = 1.0 / d.SquareLength;

                        d *= (vals[i1] - t0) * m; // unitized directional derivative
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
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshVectorField GetGradient(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> halfedgeWeights, bool parallel = false)
        {
            MeshVectorField result = new MeshVectorField(Mesh);
            GetGradient(vertexPositions, halfedgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> halfedgeWeights, MeshVectorField result, bool parallel = false)
        {
            GetGradient(vertexPositions, halfedgeWeights, result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> halfedgeWeights, Vec3d[] result, bool parallel = false)
        {
            // TODO refactor according to http://libigl.github.io/libigl/tutorial/tutorial.html 
            var verts = Mesh.Vertices;
            var vals = Values;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    var p0 = vertexPositions[i];
                    double t0 = vals[i];
                    Vec3d sum = new Vec3d();

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        var i1 = he.End.Index;

                        Vec3d d = vertexPositions[i1] - p0;
                        double m = 1.0 / d.SquareLength;
                        double w = halfedgeWeights[he.Index];

                        d *= (vals[i1] - t0) * m * w; // unitized directional derivative
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
            var faces = Mesh.Faces;
            var vals = Values;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    result[i] = f.Vertices.Mean(vals);
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
            var verts = Mesh.Vertices;
            var vals = Values;

            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;
                    vals[i] = v.SurroundingFaces.Mean(vals);
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
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("MeshScalarField ({0})", Mesh.ToString());
        }
    }
}
