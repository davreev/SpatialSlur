using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;


/*
 * Notes
 * TODO conduct further research into vertex-based mesh vector fields
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
        /// <param name="duplicateMesh"></param>
        public MeshVectorField(MeshField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="duplicateMesh"></param>
        public MeshVectorField(MeshVectorField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Vec3d Evaluate(MeshPoint point)
        {
            MeshFace face = DisplayMesh.Faces[point.FaceIndex];
            Vec3d result = new Vec3d();
            int count = (face.IsQuad) ? 4 : 3;

            for (int i = 0; i < count; i++)
                result += Values[face[i]] * point.T[i];

            return result;
        }


        /// <summary>
        /// Gets the magnitudes of all vectors in the field.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshScalarField GetMagnitudes(bool parallel = false)
        {
            MeshScalarField result = new MeshScalarField(this);
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

    }
}
