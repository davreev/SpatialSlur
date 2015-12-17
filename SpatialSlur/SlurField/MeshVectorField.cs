using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

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
        /// <param name="mesh"></param>
        public MeshVectorField(MeshField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshVectorField(MeshVectorField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MeshField Duplicate()
        {
            return new MeshVectorField(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MeshField DuplicateDeep()
        {
            return new MeshVectorField(this, true);
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
        /// 
        /// </summary>
        /// <returns></returns>
        public MeshScalarField GetMagnitudes()
        {
            MeshScalarField result = new MeshScalarField(this);
            UpdateMagnitudes(result.Values);
            return result;
        }


        /// <summary>
        /// gets the magnitudes of all vectors in the field
        /// </summary>
        /// <param name="result"></param>
        public void UpdateMagnitudes(MeshScalarField result)
        {
            UpdateMagnitudes(result.Values);
        }


        /// <summary>
        /// gets the magnitudes of all vectors in the field
        /// </summary>
        /// <param name="result"></param>
        public void UpdateMagnitudes(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Values[i].Length;
            });
        }


        /// <summary>
        /// unitizes all vectors in the field
        /// </summary>
        public void Unitize()
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] /= Values[i].Length;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Cross(MeshVectorField other)
        {
            SizeCheck(other);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Cross(Values[i], other.Values[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Cross(MeshVectorField other, MeshVectorField result)
        {
            Cross(other, result.Values);
        }


        /// <summary>up
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        public void Cross(MeshVectorField other, IList<Vec3d> result)
        {
            SizeCheck(other);
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Cross(Values[i], other.Values[i]);
            });
        }

    }
}
