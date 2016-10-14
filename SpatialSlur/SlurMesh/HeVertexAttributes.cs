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
    /// Methods for calculating various vertex attributes.
    /// </summary>
    public partial class HeVertexList
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public int[] GetVertexDegrees(bool parallel = false)
        {
            var result = new int[Count];
            GetVertexDegrees(result, parallel);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetVertexDegrees(IList<int> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetVertexDegrees(result, range.Item1, range.Item2));
            else
                GetVertexDegrees(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetVertexDegrees(IList<int> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var v = this[i];
                if (v.IsUnused) continue;
                result[i] = v.Degree;
            }
        }


        /// <summary>
        /// Calculates the topological depth of all vertices connected to a set of sources.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public int[] GetVertexDepths(IEnumerable<HeVertex> sources)
        {
            var result = new int[Count];
            GetVertexDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetVertexDepths(IEnumerable<HeVertex> sources, IList<int> result)
        {
            SizeCheck(result);

            var queue = new Queue<HeVertex>();
            result.Set(Int32.MaxValue);

            // enqueue sources and set to zero
            foreach (HeVertex v in sources)
            {
                OwnsCheck(v);
                if (v.IsUnused) continue;

                queue.Enqueue(v);
                result[v.Index] = 0;
            }

            GetVertexDepths(queue, result);
        }


        /// <summary>
        /// Calculates the topological depth of all vertices connected to a set of sources.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public int[] GetVertexDepths(IEnumerable<int> sources)
        {
            var result = new int[Count];
            GetVertexDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetVertexDepths(IEnumerable<int> sources, IList<int> result)
        {
            SizeCheck(result);

            var queue = new Queue<HeVertex>();
            result.Set(Int32.MaxValue);

            // enqueue sources and set to zero
            foreach (int vi in sources)
            {
                var v = this[vi];
                if (v.IsUnused) continue;

                queue.Enqueue(v);
                result[vi] = 0;
            }

            GetVertexDepths(queue, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetVertexDepths(Queue<HeVertex> queue, IList<int> result)
        {
            while (queue.Count > 0)
            {
                HeVertex v0 = queue.Dequeue();
                int t0 = result[v0.Index] + 1;

                foreach (HeVertex v1 in v0.ConnectedVertices)
                {
                    int i1 = v1.Index;
                    if (t0 < result[i1])
                    {
                        result[i1] = t0;
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the topological distance of all vertices connected to a set of sources.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public double[] GetVertexDistances(IEnumerable<HeVertex> sources, IList<double> edgeWeights)
        {
            var result = new double[Count];
            GetVertexDistances(sources, edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void GetVertexDistances(IEnumerable<HeVertex> sources, IList<double> edgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Mesh.Halfedges.HalfSizeCheck(edgeWeights);

            var queue = new Queue<HeVertex>();
            result.Set(Double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (HeVertex v in sources)
            {
                OwnsCheck(v);
                if (v.IsUnused) continue;

                queue.Enqueue(v);
                result[v.Index] = 0.0;
            }

            GetVertexDistances(queue, edgeWeights, result);
        }


        /// <summary>
        /// Calculates the topological distance of all vertices connected to a set of sources.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public double[] GetVertexDistances(IEnumerable<int> sources, IList<double> edgeWeights)
        {
            var result = new double[Count];
            GetVertexDistances(sources, edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void GetVertexDistances(IEnumerable<int> sources, IList<double> edgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Mesh.Halfedges.HalfSizeCheck(edgeWeights);

            var queue = new Queue<HeVertex>();
            result.Set(Double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (int vi in sources)
            {
                var v = this[vi];
                if (v.IsUnused) continue;

                queue.Enqueue(v);
                result[vi] = 0.0;
            }

            GetVertexDistances(queue, edgeWeights, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        private static void GetVertexDistances(Queue<HeVertex> queue, IList<double> edgeWeights, IList<double> result)
        {
            // TODO switch to priority queue implementation
            while (queue.Count > 0)
            {
                HeVertex v0 = queue.Dequeue();
                double t0 = result[v0.Index];

                foreach (Halfedge he in v0.IncomingHalfedges)
                {
                    HeVertex v1 = he.Start;
                    int i1 = v1.Index;
                    double t1 = t0 + edgeWeights[he.Index >> 1];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the morse smale classification for each vertex (0 = normal, 1 = minima, 2 = maxima, 3 = saddle).
        /// </summary>
        /// <param name="values"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public int[] GetMorseSmaleClassification(IList<double> values, bool parallel = false)
        {
            var result = new int[Count];
            GetMorseSmaleClassification(values, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetMorseSmaleClassification(IList<double> values, IList<int> result, bool parallel = false)
        {
            SizeCheck(values);
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetMorseSmaleClassification(values, result, range.Item1, range.Item2));
            else
                GetMorseSmaleClassification(values, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetMorseSmaleClassification(IList<double> values, IList<int> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v0 = this[i];
                double t0 = values[i];

                // check first neighbour
                Halfedge first = v0.First.Twin;
                double t1 = values[first.Start.Index];

                bool last = (t1 < t0); // was the last neighbour lower?
                int count = 0; // number of discontinuities
                first = first.Next.Twin;

                // circulate remaining neighbours
                Halfedge he = first;
                do
                {
                    t1 = values[he.Start.Index];

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

                    he = he.Next.Twin;
                } while (he != first);

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
        }


        /// <summary>
        /// Calculates the area associated with each vertex as the sum of halfedge areas.
        /// </summary>
        /// <param name="halfedgeAreas"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetVertexAreas(IList<double> halfedgeAreas, bool parallel = false)
        {
            var result = new double[Count];
            GetVertexAreas(halfedgeAreas, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetVertexAreas(IList<double> halfedgeAreas, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Halfedges.SizeCheck(halfedgeAreas);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetVertexAreas(halfedgeAreas, result, range.Item1, range.Item2));
            else
                GetVertexAreas(halfedgeAreas, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetVertexAreas(IList<double> halfedgeAreas, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;

                double sum = 0.0;
                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    if (he.Face == null) continue;
                    sum += halfedgeAreas[he.Index];
                }

                result[i] = sum;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetVertexAreas(IList<Vec3d> faceCenters,  bool parallel = false)
        {
            var result = new double[Count];
            GetVertexAreas(faceCenters, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetVertexAreas(IList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Faces.SizeCheck(faceCenters);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetVertexAreas(faceCenters, result, range.Item1, range.Item2));
            else
                GetVertexAreas(faceCenters, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetVertexAreas(IList<Vec3d> faceCenters, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;

                double sum = 0.0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    if (he.Face == null) continue;

                    /*
                    Vec3d v0 = he.Span * 0.5;
                    Vec3d v1 = faceCenters[he.Face.Index] - he.Start.Position;
                    Vec3d v2 = he.Previous.Twin.Span * 0.5;
                    sum += (Vec3d.Cross(v0, v1).Length + Vec3d.Cross(v1, v2).Length) * 0.5;
                    */

                    // add area of projected planar quad
                    Vec3d v0 = (he.Span + he.Previous.Span) * 0.5;
                    Vec3d v1 = faceCenters[he.Face.Index] - he.Start.Position;
                    sum += Vec3d.Cross(v0, v1).Length * 0.5;
                }

                result[i] = sum;
            }
        }


        /// <summary>
        ///  Assumes triangular faces.
        /// </summary>
        /// <returns></returns>
        public double[] GetVertexAreasTri()
        {
            double[] result = new double[Count];
            GetVertexAreasTriImpl(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetVertexAreasTri(IList<double> result)
        {
            SizeCheck(result);
            result.Set(0.0);
            GetVertexAreasTriImpl(result);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="result"></param>
        private void GetVertexAreasTriImpl(IList<double> result)
        {
            HeFaceList faces = Mesh.Faces;
            const double t = 1.0 / 6.0; // (1.0 / 3.0) * 0.5

            // distribute face areas to vertices
            for (int i = 0; i < faces.Count; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                double a = f.First.GetNormal().Length * t;
                foreach (HeVertex v in f.Vertices)
                    result[v.Index] += a;
            }
        }


        /// <summary>
        /// Calculates the circle packing radii for each vertex.
        /// Assumes the mesh is a circle packing mesh http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        /// <param name="edgeLengths"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetVertexRadii(IList<double> edgeLengths, bool parallel = false)
        {
            var result = new double[Count];
            GetVertexRadii(edgeLengths, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetVertexRadii(IList<double> edgeLengths, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Halfedges.HalfSizeCheck(edgeLengths);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetVertexRadii(edgeLengths, result, range.Item1, range.Item2));
            else
                GetVertexRadii(edgeLengths, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetVertexRadii(IList<double> edgeLengths, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue; // skip unused vertices

                double sum = 0.0;
                int n = 0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    if (he.Face == null) continue; // skip boundary edges
                    sum += (edgeLengths[he.Index >> 1] + edgeLengths[he.Previous.Index >> 1] - edgeLengths[he.Next.Index >> 1]) * 0.5;
                    n++;
                }

                result[v.Index] = sum / n;
            }
        }


        /// <summary>
        /// Calculates mean curvature as half the length of the vertex position laplacian.
        /// </summary>
        /// <param name="vertexLaplacian"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetMeanCurvature(IList<Vec3d> vertexLaplacian, bool parallel = false)
        {
            var result = new double[Count];
            GetMeanCurvature(vertexLaplacian, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexLaplacian"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetMeanCurvature(IList<Vec3d> vertexLaplacian, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            SizeCheck(vertexLaplacian);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetMeanCurvature(vertexLaplacian, result, range.Item1, range.Item2));
            else
                GetMeanCurvature(vertexLaplacian, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetMeanCurvature(IList<Vec3d> vertexLaplacian, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused || v.IsBoundary) continue;
                result[i] = vertexLaplacian[i].Length * 0.5;
            }
        }


        /// <summary>
        /// Calculates the gaussian curvature at each vertex as the angle defect.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetGaussianCurvature( bool parallel = false)
        {
            var result = new double[Count];
            GetGaussianCurvature(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGaussianCurvature(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetGaussianCurvature(result, range.Item1, range.Item2));
            else
                GetGaussianCurvature(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetGaussianCurvature(IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused || v.IsBoundary) continue;

                double sum = 0.0;
                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += Math.PI - he.GetAngle();

                result[i] = Math.Abs(SlurMath.TwoPI - sum);
            }
        }


        /// <summary>
        /// Calculates the Gaussian curvature at each vertex as the angle defect.
        /// </summary>
        /// <param name="halfedgeAngles"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetGaussianCurvature2(IList<double> halfedgeAngles, bool parallel = false)
        {
            var result = new double[Count];
            GetGaussianCurvature2(halfedgeAngles, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeAngles"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGaussianCurvature2(IList<double> halfedgeAngles, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Halfedges.SizeCheck(halfedgeAngles);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetGaussianCurvature2(halfedgeAngles, result, range.Item1, range.Item2));
            else
                GetGaussianCurvature2(halfedgeAngles, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetGaussianCurvature2(IList<double> halfedgeAngles, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused || v.IsBoundary) continue;

                double sum = 0.0;
                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += Math.PI - halfedgeAngles[he.Index];

                result[i] = Math.Abs(SlurMath.TwoPI - sum);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetVertexPositions(bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetVertexPositions(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetVertexPositions(IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = this[i].Position;
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    result[i] = this[i].Position;
            }
        }


        /// <summary>
        /// Calculates vertex normals as the area-weighted sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetVertexNormals(bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetVertexNormals(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetVertexNormals(IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetVertexNormals(result, range.Item1, range.Item2));
            else
                GetVertexNormals(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetVertexNormals(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;
                result[i] = v.GetNormal();
            }
        }


        /// <summary>
        /// Calculates vertex normals as the sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        /// <param name="halfedgeNormals"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetVertexNormals2(IList<Vec3d> halfedgeNormals, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetVertexNormals2(halfedgeNormals, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetVertexNormals2(IList<Vec3d> halfedgeNormals, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Halfedges.SizeCheck(halfedgeNormals);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetVertexNormals2(halfedgeNormals, result, range.Item1, range.Item2));
            else
                GetVertexNormals2(halfedgeNormals, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetVertexNormals2(IList<Vec3d> halfedgeNormals, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;

                Vec3d sum = new Vec3d();

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    if (he.Face == null) continue;
                    sum += halfedgeNormals[he.Index];
                }

                sum.Unitize();
                result[i] = sum;
            }
        }


        /// <summary>
        /// Calculates the Laplacian of vertex positions using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetPositionLaplacian(bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetPositionLaplacian(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetPositionLaplacian(IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetPositionLaplacian(result, range.Item1, range.Item2));
            else
                GetPositionLaplacian(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetPositionLaplacian(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;

                Vec3d sum = new Vec3d();
                int n = 0;

                foreach (Halfedge he in v.IncomingHalfedges)
                {
                    sum += he.Start.Position;
                    n++;
                }

                result[i] = sum / n - v.Position;
            }
        }


        /// <summary>
        /// Calculates the Laplacian of vertex positions using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetPositionLaplacian(IList<double> halfedgeWeights, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetPositionLaplacian(halfedgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetPositionLaplacian(IList<double> halfedgeWeights, IList<Vec3d> result, bool parallel)
        {
            SizeCheck(result);
            Mesh.Halfedges.SizeCheck(halfedgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetPositionLaplacian(halfedgeWeights, result, range.Item1, range.Item2));
            else
                GetPositionLaplacian(halfedgeWeights, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetPositionLaplacian(IList<double> halfedgeWeights, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;

                Vec3d sum = new Vec3d();

                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += he.Span * halfedgeWeights[he.Index];

                result[i] = sum;
            }
        }


        /// <summary>
        /// Calculates the Laplacian of vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetLaplacian(IList<double> vertexValues, bool parallel = false)
        {
            var result = new double[Count];
            GetLaplacian(vertexValues, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IList<double> vertexValues, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            SizeCheck(vertexValues);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacian(vertexValues, result, range.Item1, range.Item2));
            else
                GetLaplacian(vertexValues, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacian(IList<double> vertexValues, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;

                double t = vertexValues[i];
                double sum = 0.0;
                int n = 0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    sum += (vertexValues[he.End.Index] - t);
                    n++;
                }

                result[i] = sum / n;
            }
        }


        /// <summary>
        /// Calculates the Laplacian of vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetLaplacian2(IList<double> vertexValues, IList<double> halfedgeWeights, bool parallel = false)
        {
            var result = new double[Count];
            GetLaplacian2(vertexValues, halfedgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian2(IList<double> vertexValues, IList<double> halfedgeWeights, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            SizeCheck(vertexValues);
            Mesh.Halfedges.SizeCheck(halfedgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacian2(vertexValues, halfedgeWeights, result, range.Item1, range.Item2));
            else
                GetLaplacian2(vertexValues, halfedgeWeights, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacian2(IList<double> vertexValues, IList<double> halfedgeWeights, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;

                double t = vertexValues[i];
                double sum = 0.0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += (vertexValues[he.End.Index] - t) * halfedgeWeights[he.Index];

                result[i] = sum;
            }
        }


        /// <summary>
        /// Calculates the Laplacian of vertex attributes using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetLaplacian(IList<Vec3d> vertexValues, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetLaplacian(vertexValues, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IList<Vec3d> vertexValues, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);
            SizeCheck(vertexValues);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacian(vertexValues, result, range.Item1, range.Item2));
            else
                GetLaplacian(vertexValues, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacian(IList<Vec3d> vertexValues, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;

                Vec3d t = vertexValues[i];
                Vec3d sum = new Vec3d();
                int n = 0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    sum += (vertexValues[he.End.Index] - t);
                    n++;
                }

                result[i] = sum / n;
            }
        }


        /// <summary>
        /// Calculates the Laplacian of vertex attributes using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetLaplacian2(IList<Vec3d> vertexValues, IList<double> halfedgeWeights, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetLaplacian2(vertexValues, halfedgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian2(IList<Vec3d> vertexValues, IList<double> halfedgeWeights, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);
            SizeCheck(vertexValues);
            Mesh.Halfedges.SizeCheck(halfedgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacian2(vertexValues, halfedgeWeights, result, range.Item1, range.Item2));
            else
                GetLaplacian2(vertexValues, halfedgeWeights, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacian2(IList<Vec3d> vertexValues, IList<double> halfedgeWeights, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = this[i];
                if (v.IsUnused) continue;

                Vec3d t = vertexValues[i];
                Vec3d sum = new Vec3d();

                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += (vertexValues[he.End.Index] - t) * halfedgeWeights[he.Index];

                result[i] = sum;
            }
        }
    }
}
