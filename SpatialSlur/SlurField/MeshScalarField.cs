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
   /// TODO research scalar field gradient calculation on polygon meshes
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
        /// <param name="mesh"></param>
        public MeshScalarField(MeshField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshScalarField(MeshScalarField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MeshField Duplicate()
        {
            return new MeshScalarField(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MeshField DuplicateDeep()
        {
            return new MeshScalarField(this, true);
        }


        /// <summary>
        /// returns the interpolated value at a given point in the field
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double Evaluate(MeshPoint point)
        {
            MeshFace face = DisplayMesh.Faces[point.FaceIndex];
            double result = 0;
            int count = (face.IsQuad) ? 4 : 3;

            for (int i = 0; i < count; i++)
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
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        public void Remap(Domain to)
        {
            Remap(new Domain(Values), to);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        public void Remap(Domain from, Domain to)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Domain.Remap(Values[i], from, to);
            });
        }


        /// <summary>
        /// uses a normalized umbrella weighting scheme (Tutte scheme)
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
        /// uses a user-defined weighting scheme
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public MeshScalarField GetLaplacian(IList<double> edgeWeights)
        {
            MeshScalarField result = new MeshScalarField((MeshField)this);
            UpdateLaplacian(edgeWeights, result.Values);
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
        /// <param name="rate"></param>
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

                    foreach (HeEdge e in verts[i].IncomingEdges)
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
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateLaplacian(IList<double> edgeWeights, MeshScalarField result)
        {
            UpdateLaplacian(edgeWeights, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rate"></param>
        public void UpdateLaplacian(IList<double> edgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Mesh.Edges.SizeCheck(edgeWeights);

            HeVertexList verts = Mesh.Vertices;
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double value = Values[i];
                    double sum = 0.0;

                    foreach (HeEdge e in verts[i].OutgoingEdges)
                        sum += (Values[e.End.Index] - value) * edgeWeights[e.Index];

                    result[i] = sum;
                }
            });
        }


        /// <summary>
        /// calculates gradient based on the average of directional derivatives around each vertex
        /// uses a normalized umbrella weighting scheme (Tutte scheme)
        /// http://math.kennesaw.edu/~plaval/math2203/gradient.pdf
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <returns></returns>
        public MeshVectorField GetGradient()
        {
            MeshVectorField result = new MeshVectorField(this);
            UpdateGradient(result);
            return result;
        }


        /// <summary>
        /// calculates gradient based on the weighted average of directional derivatives around each vertex
        /// uses a user defined weighting scheme
        /// http://math.kennesaw.edu/~plaval/math2203/gradient.pdf
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public MeshVectorField GetGradient(IList<double> edgeWeights)
        {
            MeshVectorField result = new MeshVectorField(this);
            UpdateGradient(edgeWeights, result);
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

                    foreach (HeEdge e in v.OutgoingEdges)
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
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateGradient(IList<double> edgeWeights, MeshVectorField result)
        {
            UpdateGradient(edgeWeights, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateGradient(IList<double> edgeWeights, IList<Vec3d> result)
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

                    foreach (HeEdge e in v.OutgoingEdges)
                    {
                        Vec3d d = e.Span;
                        double m = 1.0 / d.Length;
                        double w = edgeWeights[e.Index];
            
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
    }
}
