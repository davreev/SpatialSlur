using System.Collections.Generic;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 *
 * TODO finish implementation
 * 
 * References
 * http://libigl.github.io/libigl/tutorial/tutorial.html
 * http://graphics.pixar.com/library/VectorFieldCourse/paper.pdf
 * http://graphics.pixar.com/library/VectorFieldCourse/paper.pdf
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public class HeVertexVectorField:HeVertexField<Vec3d>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        public HeVertexVectorField(HeMeshVertexList vertices)
            : base(vertices)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeVertexVectorField Duplicate()
        {
            var copy = new HeVertexVectorField(Vertices);
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
        public override Vec3d ValueAt(int i0, int i1, int i2, double w0, double w1, double w2)
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
        public void IncrementAt(int i0, int i1, int i2, double w0, double w1, double w2, Vec3d amount)
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
        public HeVertexVectorField GetLaplacian(bool parallel = false)
        {
            var result = new HeVertexVectorField(Vertices);
            Vertices.GetLaplacian(Values, result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public void GetLaplacian(HeVertexVectorField result, bool parallel = false)
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
        public HeVertexVectorField GetLaplacian(IReadOnlyList<double> hedgeWeights, bool parallel = false)
        {
            var result = new HeVertexVectorField(Vertices);
            Vertices.GetLaplacian(Values, hedgeWeights, result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IReadOnlyList<double> hedgeWeights, HeVertexVectorField result, bool parallel = false)
        {
            Vertices.GetLaplacian(Values, hedgeWeights, result.Values, parallel);
        }
    }
}
