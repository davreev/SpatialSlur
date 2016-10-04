using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 * 
 * TODO 
 * Further research into vertex-based mesh vector fields
 * 
 * References
 * http://graphics.pixar.com/library/VectorFieldCourse/paper.pdf
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public class MeshVectorField:MeshField<Vec3d>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshVectorField(HeMesh mesh)
            : base(mesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public MeshVectorField(MeshVectorField other)
            : base(other)
        {
        }


        /*
        /// <summary>
        /// Returns the interpolated value at a given point in the field.
        /// Assumes triangular faces in the queried mesh.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Vec3d Evaluate(MeshPoint point)
        {
            MeshFace face = point.Mesh.Faces[point.FaceIndex];
            Vec3d result = new Vec3d();

            for (int i = 0; i < 3; i++)
                result += Values[face[i]] * point.T[i];

            return result;
        }
        */
    

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
        public override Vec3d Evaluate(int i0, int i1, int i2, double w0, double w1, double w2)
        {
            return Values[i0] * w0 + Values[i1] * w1 + Values[i2] * w2;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public override Vec3d Evaluate(IList<int> indices, IList<double> weights)
        {
            Vec3d result = new Vec3d();

            for (int i = 0; i < indices.Count; i++)
                result += Values[indices[i]] * weights[i];

            return result;
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshScalarField GetMagnitudes(bool parallel = false)
        {
            MeshScalarField result = new MeshScalarField(Mesh);
            UpdateMagnitudes(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateMagnitudes(MeshScalarField result, bool parallel = false)
        {
            UpdateMagnitudes(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateMagnitudes(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Values[i].Length;
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    result[i] = Values[i].Length;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        public void Unitize(bool parallel = false)
        {
            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        Values[i] /= Values[i].Length;
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    Values[i] /= Values[i].Length;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void Cross(MeshVectorField other, bool parallel = false)
        {
            SizeCheck(other);

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        Values[i] = Vec3d.Cross(Values[i], other.Values[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    Values[i] = Vec3d.Cross(Values[i], other.Values[i]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void Cross(MeshVectorField other, MeshVectorField result, bool parallel = false)
        {
            Cross(other.Values, result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void Cross(IList<Vec3d> vectors, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(vectors);
            SizeCheck(result);

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Vec3d.Cross(Values[i], vectors[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    result[i] = Vec3d.Cross(Values[i], vectors[i]);
            }
        }


        /// <summary>
        /// Calculates the Laplacian using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshVectorField GetLaplacian(bool parallel = false)
        {
            var result = new MeshVectorField(Mesh);
            Mesh.Vertices.UpdateAttributeLaplacians(Values, result.Values, parallel);
            return result;
        }


        /// <summary>
        /// Calculates the Laplacian using a custom weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshVectorField GetLaplacian(IList<double> halfedgeWeights, bool parallel = false)
        {
            var result = new MeshVectorField(Mesh);
            Mesh.Vertices.UpdateAttributeLaplacians(Values, halfedgeWeights, result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public void UpdateLaplacian(MeshVectorField result, bool parallel = false)
        {
            Mesh.Vertices.UpdateAttributeLaplacians(Values, result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateLaplacian(IList<double> halfedgeWeights, MeshVectorField result, bool parallel = false)
        {
            Mesh.Vertices.UpdateAttributeLaplacians(Values, halfedgeWeights, result.Values, parallel);
        }


    }
}
