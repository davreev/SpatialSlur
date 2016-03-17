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
        public void Normalize()
        {
            Normalize(new Domain(Values));
        }


        /// <summary>
        /// 
        /// </summary>
        public void Normalize(Domain domain)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = domain.Normalize(Values[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        public void Remap(Domain to)
        {
            Remap(new Domain(Values), to);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Remap(Domain from, Domain to)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Domain.Remap(Values[i], from, to);
            });
        }


        /// <summary>
        /// Calculates the Laplacian using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <returns></returns>
        public MeshScalarField GetLaplacian()
        {
            MeshScalarField result = new MeshScalarField((MeshField)this);
            UpdateLaplacian(result.Values);
            return result;
        }


        /// <summary>
        /// Calculates the Laplacian using a custom weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="halfEdgeWeights"></param>
        /// <returns></returns>
        public MeshScalarField GetLaplacian(IList<double> halfEdgeWeights)
        {
            MeshScalarField result = new MeshScalarField((MeshField)this);
            UpdateLaplacian(halfEdgeWeights, result.Values);
            return result;
        }
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateLaplacian(MeshScalarField result)
        {
            UpdateLaplacian(result.Values);
        }

   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(IList<double> result)
        {
            SizeCheck(result);

            HeVertexList verts = Mesh.Vertices;
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double sum = 0.0;
                    int n = 0;

                    foreach (HalfEdge e in verts[i].IncomingHalfEdges)
                    {
                        sum += Values[e.Start.Index];
                        n++;
                    }

                    result[i] = sum / n - Values[i];
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfEdgeWeights"></param>
        /// <param name="result"></param>
        public void UpdateLaplacian(IList<double> halfEdgeWeights, MeshScalarField result)
        {
            UpdateLaplacian(halfEdgeWeights, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfEdgeWeights"></param>
        /// <param name="result"></param>
        public void UpdateLaplacian(IList<double> halfEdgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Mesh.HalfEdges.SizeCheck(halfEdgeWeights);

            HeVertexList verts = Mesh.Vertices;
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double value = Values[i];
                    double sum = 0.0;

                    foreach (HalfEdge e in verts[i].OutgoingHalfEdges)
                        sum += (Values[e.End.Index] - value) * halfEdgeWeights[e.Index];

                    result[i] = sum;
                }
            });
        }


        /// <summary>
        /// Naive calculation of gradient based on the average of directional derivatives around each vertex.
        /// TODO further research into vertex-based mesh vector fields
        /// http://graphics.pixar.com/library/VectorFieldCourse/paper.pdf
        /// </summary>
        /// <returns></returns>
        public MeshVectorField GetGradient()
        {
            MeshVectorField result = new MeshVectorField(this);
            UpdateGradient(result);
            return result;
        }


        /// <summary>
        /// Naive calculation of gradient based on the weighted sum of directional derivatives around each vertex.
        /// TODO further research into vertex-based mesh vector fields
        /// http://graphics.pixar.com/library/VectorFieldCourse/paper.pdf
        /// </summary>
        /// <param name="halfEdgeWeights"></param>
        /// <returns></returns>
        public MeshVectorField GetGradient(IList<double> halfEdgeWeights)
        {
            MeshVectorField result = new MeshVectorField(this);
            UpdateGradient(halfEdgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateGradient(MeshVectorField result)
        {
            UpdateGradient(result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateGradient(IList<Vec3d> result)
        {
            SizeCheck(result);

            HeVertexList verts = Mesh.Vertices;
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    double value = Values[i];
                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (HalfEdge e in v.OutgoingHalfEdges)
                    {
                        Vec3d d = e.Span;
                        double m = 1.0 / d.Length;

                        d *= (Values[e.End.Index] - value) * m * m; // unitized directional derivative
                        sum += d;
                        n++;
                    }

                    result[i] = sum / n;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfEdgeWeights"></param>
        /// <param name="result"></param>
        public void UpdateGradient(IList<double> halfEdgeWeights, MeshVectorField result)
        {
            UpdateGradient(halfEdgeWeights, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfEdgeWeights"></param>
        /// <param name="result"></param>
        public void UpdateGradient(IList<double> halfEdgeWeights, IList<Vec3d> result)
        {
            SizeCheck(result);

            HeVertexList verts = Mesh.Vertices;
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    double value = Values[i];
                    Vec3d sum = new Vec3d();

                    foreach (HalfEdge e in v.OutgoingHalfEdges)
                    {
                        Vec3d d = e.Span;
                        double m = 1.0 / d.Length;
                        double w = halfEdgeWeights[e.Index];
            
                        d *= (Values[e.End.Index] - value) * m * m * w; // unitized directional derivative
                        sum += d;
                    }

                    result[i] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] GetFaceMeans()
        {
            double[] result = new double[Mesh.Faces.Count];
            UpdateFaceMeans(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateFaceMeans(IList<double> result)
        {
            HeFaceList faces = Mesh.Faces;
            faces.SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
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
                });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceValues"></param>
        public void SetVertexMeans(IList<double> faceValues)
        {
            HeVertexList verts = Mesh.Vertices;
       
            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    double sum = 0.0;
                    int n = 0;
                    foreach(HeFace f in v.SurroundingFaces)
                    {
                        sum += faceValues[f.Index];
                        n++;
                    }

                    Values[i] = sum / n;
                }
            });
        }
    }
}
