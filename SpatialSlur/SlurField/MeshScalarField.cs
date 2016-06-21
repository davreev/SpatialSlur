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
    public class MeshScalarField:MeshField<double>
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
        /// <param name="duplicateMesh"></param>
        public MeshScalarField(MeshField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <param name="duplicateMesh"></param>
        public MeshScalarField(MeshScalarField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
        }


        /// <summary>
        /// Returns the interpolated value at a given point in the field
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double Evaluate(MeshPoint point)
        {
            MeshFace face = DisplayMesh.Faces[point.FaceIndex];
            double result = 0.0;

            int n = (face.IsQuad) ? 4 : 3;
            for (int i = 0; i < n; i++)
                result += Values[face[i]] * point.T[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Normalize(bool parallel = false)
        {
            Normalize(new Domain(Values), parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Normalize(Domain domain, bool parallel = false)
        {
            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        Values[i] = domain.Normalize(Values[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    Values[i] = domain.Normalize(Values[i]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="parallel"></param>
        public void Remap(Domain to, bool parallel = false)
        {
            Remap(new Domain(Values), to, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="parallel"></param>
        public void Remap(Domain from, Domain to, bool parallel = false)
        {
            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        Values[i] = Domain.Remap(Values[i], from, to);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    Values[i] = Domain.Remap(Values[i], from, to);
            }
        }


        /// <summary>
        /// Calculates the Laplacian using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshScalarField GetLaplacian(bool parallel = false)
        {
            MeshScalarField result = new MeshScalarField((MeshField)this);
            UpdateLaplacian(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// Calculates the Laplacian using a custom weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshScalarField GetLaplacian(IList<double> halfedgeWeights, bool parallel = false)
        {
            MeshScalarField result = new MeshScalarField((MeshField)this);
            UpdateLaplacian(halfedgeWeights, result.Values, parallel);
            return result;
        }
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public void UpdateLaplacian(MeshScalarField result, bool parallel = false)
        {
            UpdateLaplacian(result.Values, parallel);
        }

   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateLaplacian(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => 
                    UpdateLaplacian(result, range.Item1, range.Item2));
            else
                UpdateLaplacian(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateLaplacian(IList<double> result, int i0, int i1)
        {
            HeVertexList verts = Mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                double sum = 0.0;
                int n = 0;

                foreach (Halfedge he in verts[i].IncomingHalfedges)
                {
                    sum += Values[he.Start.Index];
                    n++;
                }

                result[i] = sum / n - Values[i];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateLaplacian(IList<double> halfedgeWeights, MeshScalarField result, bool parallel = false)
        {
            UpdateLaplacian(halfedgeWeights, result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateLaplacian(IList<double> halfedgeWeights, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Halfedges.SizeCheck(halfedgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => 
                    UpdateLaplacian(halfedgeWeights, result, range.Item1, range.Item2));
            else
                UpdateLaplacian(halfedgeWeights, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateLaplacian(IList<double> halfedgeWeights, IList<double> result, int i0, int i1)
        {
            HeVertexList verts = Mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                double value = Values[i];
                double sum = 0.0;

                foreach (Halfedge he in verts[i].OutgoingHalfedges)
                    sum += (Values[he.End.Index] - value) * halfedgeWeights[he.Index];

                result[i] = sum;
            }
        }


        /// <summary>
        /// Calculates the gradient as the average of directional derivatives around each vertex.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshVectorField GetGradient(bool parallel = false)
        {
            // TODO conduct further research into vertex-based mesh vector fields
            // http://graphics.pixar.com/library/VectorFieldCourse/paper.pdf
            
            MeshVectorField result = new MeshVectorField(this);
            UpdateGradient(result, parallel);
            return result;
        }


        /// <summary>
        /// Calculates the gradient as the weighted sum of directional derivatives around each vertex.
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshVectorField GetGradient(IList<double> halfedgeWeights, bool parallel = false)
        {
            MeshVectorField result = new MeshVectorField(this);
            UpdateGradient(halfedgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateGradient(MeshVectorField result, bool parallel = false)
        {
            UpdateGradient(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateGradient(IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => 
                    UpdateGradient(result, range.Item1, range.Item2));
            else
                UpdateGradient(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateGradient(IList<Vec3d> result, int i0, int i1)
        {
            HeVertexList verts = Mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double value = Values[i];
                Vec3d sum = new Vec3d();
                int n = 0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    Vec3d d = he.Span;
                    double m = 1.0 / d.Length;

                    d *= (Values[he.End.Index] - value) * m * m; // unitized directional derivative
                    sum += d;
                    n++;
                }

                result[i] = sum / n;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateGradient(IList<double> halfedgeWeights, MeshVectorField result, bool parallel = false)
        {
            UpdateGradient(halfedgeWeights, result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateGradient(IList<double> halfedgeWeights, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Halfedges.SizeCheck(halfedgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => 
                    UpdateGradient(halfedgeWeights, result, range.Item1, range.Item2));
            else
                UpdateGradient(halfedgeWeights, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateGradient(IList<double> halfedgeWeights, IList<Vec3d> result, int i0, int i1)
        {
            HeVertexList verts = Mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double value = Values[i];
                Vec3d sum = new Vec3d();

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    Vec3d d = he.Span;
                    double m = 1.0 / d.Length;
                    double w = halfedgeWeights[he.Index];

                    d *= (Values[he.End.Index] - value) * m * m * w; // unitized directional derivative
                    sum += d;
                }

                result[i] = sum;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetFaceMeans(bool parallel = false)
        {
            double[] result = new double[Mesh.Faces.Count];
            UpdateFaceMeans(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void UpdateFaceMeans(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Mesh.Faces.Count), range => 
                    UpdateFaceMeans(result, range.Item1, range.Item2));
            else
                UpdateFaceMeans(result, 0, Mesh.Faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateFaceMeans(IList<double> result, int i0, int i1)
        {
            HeFaceList faces = Mesh.Faces;

            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                double sum = 0.0;
                int n = 0;
                foreach (HeVertex v in f.Vertices)
                {
                    sum += Values[v.Index];
                    n++;
                }

                result[i] = sum / n;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceValues"></param>
        /// <param name="parallel"></param>
        public void SetVertexMeans(IList<double> faceValues, bool parallel = false)
        {
            Mesh.Faces.SizeCheck(faceValues);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => 
                    SetVertexMeans(faceValues, range.Item1, range.Item2));
            else
                SetVertexMeans(faceValues, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetVertexMeans(IList<double> faceValues, int i0, int i1)
        {
            HeVertexList verts = Mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double sum = 0.0;
                int n = 0;
                foreach (HeFace f in v.SurroundingFaces)
                {
                    sum += faceValues[f.Index];
                    n++;
                }

                Values[i] = sum / n;
            }
        }
    }
}
