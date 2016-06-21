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
    /// Static extension methods for calculating various mesh element attributes.
    /// </summary>
    public static class HeAttributes
    {
        #region Vertex Attributes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetVertexDegrees(this HeVertexList verts, bool parallel = false)
        {
            int[] result = new int[verts.Count];
            verts.UpdateVertexDegrees(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateVertexDegrees(this HeVertexList verts, IList<int> result, bool parallel = false)
        {
            verts.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateVertexDegrees(result, range.Item1, range.Item2));
            else
                verts.UpdateVertexDegrees(result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateVertexDegrees(this HeVertexList verts, IList<int> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;
                result[i] = v.Degree;
            }
        }


        /// <summary>
        /// Returns the topological depth of all vertices connected to a set of sources.
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static int[] GetVertexDepths(this HeVertexList verts, IEnumerable<HeVertex> sources)
        {
            int[] result = new int[verts.Count];
            verts.UpdateVertexDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public static void UpdateVertexDepths(this HeVertexList verts, IEnumerable<HeVertex> sources, IList<int> result)
        {
            verts.SizeCheck(result);

            var queue = new Queue<HeVertex>();
            result.Set(Int32.MaxValue);

            // enqueue sources and set to zero
            foreach (HeVertex v in sources)
            {
                verts.OwnsCheck(v);
                if (v.IsUnused) continue;

                queue.Enqueue(v);
                result[v.Index] = 0;
            }

            // breadth first search from sources
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
        /// Returns the topological distance of all vertices connected to a set of sources.
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <returns></returns>
        public static double[] GetVertexDepths(this HeVertexList verts, IEnumerable<HeVertex> sources, IList<double> edgeLengths)
        {
            double[] result = new double[verts.Count];
            verts.UpdateVertexDepths(sources, edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        public static void UpdateVertexDepths(this HeVertexList verts, IEnumerable<HeVertex> sources, IList<double> edgeLengths, IList<double> result)
        {
            verts.SizeCheck(result);
            verts.Mesh.Halfedges.SizeCheck(edgeLengths);

            var queue = new Queue<HeVertex>();
            result.Set(Double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (HeVertex v in sources)
            {
                verts.OwnsCheck(v);
                if (v.IsUnused) continue;

                queue.Enqueue(v);
                result[v.Index] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                HeVertex v0 = queue.Dequeue();
                double t0 = result[v0.Index];

                foreach (Halfedge he in v0.IncomingHalfedges)
                {
                    HeVertex v1 = he.Start;
                    int i1 = v1.Index;
                    double t1 = t0 + edgeLengths[he.Index >> 1];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the morse smale classification for each vertex (0 = normal, 1 = minima, 2 = maxima, 3 = saddle).
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetMorseSmaleClassification(this HeVertexList verts, IList<double> values, bool parallel = false)
        {
            int[] result = new int[verts.Count];
            verts.UpdateMorseSmaleClassification(values, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateMorseSmaleClassification(this HeVertexList verts, IList<double> values, IList<int> result, bool parallel = false)
        {
            verts.SizeCheck(values);
            verts.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateMorseSmaleClassification(values, result, range.Item1, range.Item2));
            else
                verts.UpdateMorseSmaleClassification(values, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateMorseSmaleClassification(this HeVertexList verts, IList<double> values, IList<int> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v0 = verts[i];
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
        /// Returns the area associated with each vertex.
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halfedgeAreas"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetVertexAreas(this HeVertexList verts, IList<double> halfedgeAreas, bool parallel = false)
        {
            double[] result = new double[verts.Count];
            verts.UpdateVertexAreas(halfedgeAreas, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halfedgeAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateVertexAreas(this HeVertexList verts, IList<double> halfedgeAreas, IList<double> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.Mesh.Halfedges.SizeCheck(halfedgeAreas);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateVertexAreas(halfedgeAreas, result, range.Item1, range.Item2));
            else
                verts.UpdateVertexAreas(halfedgeAreas, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateVertexAreas(this HeVertexList verts, IList<double> halfedgeAreas, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
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
        /// <param name="verts"></param>
        /// <param name="faceCenters"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetVertexAreas(this HeVertexList verts, IList<Vec3d> faceCenters, bool parallel = false)
        {
            double[] result = new double[verts.Count];
            verts.UpdateVertexAreas(faceCenters, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateVertexAreas(this HeVertexList verts, IList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.Mesh.Faces.SizeCheck(faceCenters);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    UpdateVertexAreas(verts, faceCenters, result, range.Item1, range.Item2));
            else
                UpdateVertexAreas(verts, faceCenters, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateVertexAreas(this HeVertexList verts, IList<Vec3d> faceCenters, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double sum = 0.0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    if (he.Face == null) continue;

                    Vec3d v0 = he.Span * 0.5;
                    Vec3d v1 = faceCenters[he.Face.Index] - he.Start.Position;
                    Vec3d v2 = he.Previous.Twin.Span * 0.5;

                    sum += (Vec3d.Cross(v0, v1).Length + Vec3d.Cross(v1, v2).Length) * 0.5;
                }

                result[i] = sum;
            }
        }


        /// <summary>
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        public static double[] GetVertexAreasTri(this HeVertexList verts)
        {
            double[] result = new double[verts.Count];
            verts.UpdateVertexAreasTri(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="result"></param>
        public static void UpdateVertexAreasTri(this HeVertexList verts, IList<double> result)
        {
            verts.SizeCheck(result);

            HeFaceList faces = verts.Mesh.Faces;
            double t = 1.0 / 6.0; // (1.0 / 3.0) * 0.5

            for (int i = 0; i < faces.Count; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                double a = Vec3d.Cross(f.First.Span, f.First.Next.Span).Length * t; // * 0.5
                foreach (HeVertex v in f.Vertices)
                    result[v.Index] += a;
            }
        }


        /// <summary>
        /// Returns a radii for each vertex.
        /// If the mesh is a circle packing mesh, these will be the radii of tangent spheres centered on each vertex.
        /// http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetVertexRadii(this HeVertexList verts, IList<double> edgeLengths, bool parallel = false)
        {
            double[] result = new double[verts.Count];
            verts.UpdateVertexRadii(edgeLengths, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateVertexRadii(this HeVertexList verts, IList<double> edgeLengths, IList<double> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.Mesh.Halfedges.HalfSizeCheck(edgeLengths);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateVertexRadii(edgeLengths, result, range.Item1, range.Item2));
            else
                verts.UpdateVertexRadii(edgeLengths, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateVertexRadii(this HeVertexList verts, IList<double> edgeLengths, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
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
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        public static double[] GetMeanCurvature(this HeVertexList verts)
        {
            double[] result = new double[verts.Count];
            verts.UpdateMeanCurvature(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="result"></param>
        public static void UpdateMeanCurvature(this HeVertexList verts, IList<double> result)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetGaussianCurvature(this HeVertexList verts, bool parallel = false)
        {
            double[] result = new double[verts.Count];
            verts.UpdateGaussianCurvature(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateGaussianCurvature(this HeVertexList verts, IList<double> result, bool parallel = false)
        {
            verts.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                verts.UpdateGaussianCurvature(result, range.Item1, range.Item2));
            else
                verts.UpdateGaussianCurvature(result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateGaussianCurvature(this HeVertexList verts, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue;

                double sum = 0.0;
                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += Math.PI - he.GetAngle();

                result[i] = Math.Abs(sum - SlurMath.Tau);
            }
        }


        /// <summary>
        /// Returns the gaussian curvature at each vertex.
        /// This is calculated as the angle defect.
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetGaussianCurvature(this HeVertexList verts, IList<double> halfedgeAngles, bool parallel = false)
        {
            double[] result = new double[verts.Count];
            verts.UpdateGaussianCurvature(halfedgeAngles, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halfedgeAngles"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateGaussianCurvature(this HeVertexList verts, IList<double> halfedgeAngles, IList<double> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.Mesh.Halfedges.SizeCheck(halfedgeAngles);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateGaussianCurvature(halfedgeAngles, result, range.Item1, range.Item2));
            else
                verts.UpdateGaussianCurvature(halfedgeAngles, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateGaussianCurvature(this HeVertexList verts, IList<double> halfedgeAngles, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue;

                double sum = 0.0;
                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += Math.PI - halfedgeAngles[he.Index];

                result[i] = Math.Abs(sum - SlurMath.Tau);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetVertexPositions(this HeVertexList verts, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[verts.Count];
            verts.UpdateVertexPositions(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateVertexPositions(this HeVertexList verts, IList<Vec3d> result, bool parallel = false)
        {
            verts.SizeCheck(result);

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = verts[i].Position;
                });
            }
            else
            {
                for (int i = 0; i < verts.Count; i++)
                    result[i] = verts[i].Position;
            }
        }


        /// <summary>
        /// Calculates the vertex normal as the area-weighted sum of halfedge normals around each vertex.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetVertexNormals(this HeVertexList verts, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[verts.Count];
            verts.UpdateVertexNormals(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateVertexNormals(this HeVertexList verts, IList<Vec3d> result, bool parallel = false)
        {
            verts.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateVertexNormals(result, range.Item1, range.Item2));
            else
                verts.UpdateVertexNormals(result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateVertexNormals(this HeVertexList verts, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;
                result[i] = v.GetNormal();
            }
        }


        /// <summary>
        /// Calculates the vertex normals as the unitized sum of halfedge normals around each vertex.
        /// Half-edge normals can be scaled in advance for custom weighting.
        /// Vertex normals are unitized by default.
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halfedgeNormals"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetVertexNormals(this HeVertexList verts, IList<Vec3d> halfedgeNormals, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[verts.Count];
            verts.UpdateVertexNormals(halfedgeNormals, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halfedgeNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateVertexNormals(this HeVertexList verts, IList<Vec3d> halfedgeNormals, IList<Vec3d> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.Mesh.Halfedges.SizeCheck(halfedgeNormals);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateVertexNormals(halfedgeNormals, result, range.Item1, range.Item2));
            else
                verts.UpdateVertexNormals(halfedgeNormals, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateVertexNormals(this HeVertexList verts, IList<Vec3d> halfedgeNormals, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
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
        /// Calculates the Laplacian using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetVertexLaplacians(this HeVertexList verts, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[verts.Count];
            verts.UpdateVertexLaplacians(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateVertexLaplacians(this HeVertexList verts, IList<Vec3d> result, bool parallel = false)
        {
            verts.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateVertexLaplacians(result, range.Item1, range.Item2));
            else
                verts.UpdateVertexLaplacians(result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateVertexLaplacians(this HeVertexList verts, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
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
        /// Calculates the Laplacian using given halfedge weights.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetVertexLaplacians(this HeVertexList verts, IList<double> halfedgeWeights, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[verts.Count];
            verts.UpdateVertexLaplacians(halfedgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateVertexLaplacians(this HeVertexList verts, IList<double> halfedgeWeights, IList<Vec3d> result, bool parallel)
        {
            verts.SizeCheck(result);
            verts.Mesh.Halfedges.SizeCheck(halfedgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateVertexLaplacians(halfedgeWeights, result, range.Item1, range.Item2));
            else
                verts.UpdateVertexLaplacians(halfedgeWeights, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateVertexLaplacians(this HeVertexList verts, IList<double> halfedgeWeights, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                Vec3d sum = new Vec3d();

                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += he.Span * halfedgeWeights[he.Index];

                result[i] = sum;
            }
        }


        /*
         * TODO
         * Can attribute laplacians be made generic?
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetAttributeLaplacians(this HeVertexList verts, IList<double> values, bool parallel = false)
        {
            var result = new double[verts.Count];
            verts.UpdateAttributeLaplacians(values, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetAttributeLaplacians(this HeVertexList verts, IList<Vec3d> values, bool parallel = false)
        {
            var result = new Vec3d[verts.Count];
            verts.UpdateAttributeLaplacians(values, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateAttributeLaplacians(this HeVertexList verts, IList<double> values, IList<double> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.SizeCheck(values);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateAttributeLaplacians(values, result, range.Item1, range.Item2));
            else
                verts.UpdateAttributeLaplacians(values, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateAttributeLaplacians(this HeVertexList verts, IList<Vec3d> values, IList<Vec3d> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.SizeCheck(values);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateAttributeLaplacians(values, result, range.Item1, range.Item2));
            else
                verts.UpdateAttributeLaplacians(values, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateAttributeLaplacians(this HeVertexList verts, IList<double> values, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double t = values[i];
                double sum = 0.0;
                int n = 0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    sum += (values[he.End.Index] - t);
                    n++;
                }

                result[i] = sum / n;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateAttributeLaplacians(this HeVertexList verts, IList<Vec3d> values, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                Vec3d t = values[i];
                Vec3d sum = new Vec3d();
                int n = 0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    sum += (values[he.End.Index] - t);
                    n++;
                }

                result[i] = sum / n;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetAttributeLaplacians(this HeVertexList verts, IList<double> values, IList<double> halfedgeWeights, bool parallel = false)
        {
            var result = new double[verts.Count];
            verts.UpdateAttributeLaplacians(values, halfedgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetAttributeLaplacians(this HeVertexList verts, IList<Vec3d> values, IList<double> halfedgeWeights, bool parallel = false)
        {
            var result = new Vec3d[verts.Count];
            verts.UpdateAttributeLaplacians(values, halfedgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateAttributeLaplacians(this HeVertexList verts, IList<double> values, IList<double> halfedgeWeights, IList<double> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.SizeCheck(values);
            verts.Mesh.Halfedges.SizeCheck(halfedgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateAttributeLaplacians(values, halfedgeWeights, result, range.Item1, range.Item2));
            else
                verts.UpdateAttributeLaplacians(values, halfedgeWeights, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="values"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateAttributeLaplacians(this HeVertexList verts, IList<Vec3d> values, IList<double> halfedgeWeights, IList<Vec3d> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.SizeCheck(values);
            verts.Mesh.Halfedges.SizeCheck(halfedgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.UpdateAttributeLaplacians(values, halfedgeWeights, result, range.Item1, range.Item2));
            else
                verts.UpdateAttributeLaplacians(values, halfedgeWeights, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateAttributeLaplacians(this HeVertexList verts, IList<double> values, IList<double> halfedgeWeights, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double t = values[i];
                double sum = 0.0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += (values[he.End.Index] - t) * halfedgeWeights[he.Index];

                result[i] = sum;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateAttributeLaplacians(this HeVertexList verts, IList<Vec3d> values, IList<double> halfedgeWeights, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                Vec3d t = values[i];
                Vec3d sum = new Vec3d();

                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += (values[he.End.Index] - t) * halfedgeWeights[he.Index];

                result[i] = sum;
            }
        }

        #endregion


        #region Halfedge Attributes

        /// <summary>
        /// Returns the length of each halfedge in the mesh.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetHalfedgeLengths(this HalfedgeList hedges, bool parallel = false)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateHalfedgeLengths(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateHalfedgeLengths(this HalfedgeList hedges, IList<double> result, bool parallel = false)
        {
            hedges.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.UpdateHalfedgeLengths(result, range.Item1 << 1, range.Item2 << 1));
            else
                hedges.UpdateHalfedgeLengths(result, 0, hedges.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeLengths(this HalfedgeList hedges, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i += 2)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;

                double d = he.Span.Length;
                result[i] = d;
                result[i + 1] = d;
            }
        }


        /// <summary>
        /// Returns the angle between each halfedge and its previous.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetHalfedgeAngles(this HalfedgeList hedges, bool parallel = false)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateHalfedgeAngles(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateHalfedgeAngles(this HalfedgeList hedges, IList<double> result, bool parallel = false)
        {
            hedges.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), range =>
                    hedges.UpdateHalfedgeAngles(result, range.Item1, range.Item2));
            else
                hedges.UpdateHalfedgeAngles(result, 0, hedges.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeAngles(this HalfedgeList hedges, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;
                result[i] = he.GetAngle();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetHalfedgeAngles(this HalfedgeList hedges, IList<double> edgeLengths, bool parallel = false)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateHalfedgeAngles(edgeLengths, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateHalfedgeAngles(this HalfedgeList hedges, IList<double> edgeLengths, IList<double> result, bool parallel = false)
        {
            hedges.SizeCheck(result);
            hedges.HalfSizeCheck(edgeLengths);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), range =>
                    hedges.UpdateHalfedgeAngles(edgeLengths, result, range.Item1, range.Item2));
            else
                hedges.UpdateHalfedgeAngles(edgeLengths, result, 0, hedges.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeAngles(this HalfedgeList hedges, IList<double> edgeLengths, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he0 = hedges[i];
                if (he0.IsUnused) continue;

                Halfedge he1 = he0.Previous;
                double d = edgeLengths[i >> 1] * edgeLengths[he1.Index >> 1];

                if (d > 0.0)
                    result[i] = Math.Acos(he0.Span * he1.Span / d);
                else
                    result[i] = Double.NaN;
            }
        }


        /// <summary>
        /// Returns the area associated with each halfedge.
        /// This is calculated as W in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="faceCenters"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetHalfedgeAreas(this HalfedgeList hedges, IList<Vec3d> faceCenters, bool parallel = false)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateHalfedgeAreas(faceCenters, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateHalfedgeAreas(this HalfedgeList hedges, IList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            hedges.SizeCheck(result);
            hedges.Mesh.Faces.SizeCheck(faceCenters);

            if(parallel)
            Parallel.ForEach(Partitioner.Create(0, hedges.Count), range =>
                hedges.UpdateHalfedgeAreas(faceCenters,result,range.Item1, range.Item2));
            else
                hedges.UpdateHalfedgeAreas(faceCenters,result,0,hedges.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeAreas(this HalfedgeList hedges, IList<Vec3d> faceCenters, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused || he.Face == null) continue;

                /*
                Vec3d v0 = he.Span * 0.5;
                Vec3d v1 = faceCenters[he.Face.Index] - he.Start.Position;
                Vec3d v2 = he.Previous.Span * -0.5;
                result[i] = (Vec3d.Cross(v0, v1).Length + Vec3d.Cross(v1, v2).Length) * 0.5;
                */

                // area of projected planar quad
                Vec3d v0 = (he.Span + he.Previous.Span) * 0.5;
                Vec3d v1 = faceCenters[he.Face.Index] - he.Start.Position;
                result[i] = Vec3d.Cross(v0, v1).Length * 0.5;
            }
        }


        /// <summary>
        /// Returns the cotangent of each halfedge.
        /// Intended for use on triangle meshes.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetHalfedgeCotangents(this HalfedgeList hedges, bool parallel = false)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateHalfedgeCotangents(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateHalfedgeCotangents(this HalfedgeList hedges, IList<double> result, bool parallel = false)
        {
            hedges.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), range =>
                    hedges.UpdateHalfedgeCotangents(result, range.Item1, range.Item2));
            else
                hedges.UpdateHalfedgeCotangents(result, 0, hedges.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeCotangents(this HalfedgeList hedges, IList<double> result, int i0,  int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d v0 = he.Previous.Span;
                Vec3d v1 = he.Next.Twin.Span;
                result[i] = v0 * v1 / Vec3d.Cross(v0, v1).Length;
            }
        }


        /// <summary>
        /// Returns a symmetric cotangent weight for each halfedge.
        /// Intended for use on triangle meshes.
        /// http://reuter.mit.edu/papers/reuter-smi09.pdf
        /// http://libigl.github.io/libigl/tutorial/tutorial.html#normals
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetCotanWeights(this HalfedgeList hedges, bool parallel = false)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateCotanWeights(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateCotanWeights(this HalfedgeList hedges, IList<double> result, bool parallel = false)
        {
            hedges.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.UpdateCotanWeights(result, range.Item1, range.Item2));
            else
                hedges.UpdateCotanWeights(result, 0, hedges.Count >> 1);
        }


       /// <summary>
       /// 
       /// </summary>
        private static void UpdateCotanWeights(this HalfedgeList hedges, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he = hedges[j];
                if (he.IsUnused) continue;
                double w = 0.0;

                if (he.Face != null)
                {
                    Vec3d v0 = he.Previous.Span;
                    Vec3d v1 = he.Next.Twin.Span;
                    w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                }

                he = he.Twin;
                if (he.Face != null)
                {
                    Vec3d v0 = he.Previous.Span;
                    Vec3d v1 = he.Next.Twin.Span;
                    w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                }

                result[j] = result[j + 1] = w * 0.5;
            }
        }


        /// <summary>
        /// Alternative implementation that returns the vertex areas for subsequent normalization.
        /// Note that this version isn't suitable for parallel calculation so it may be faster to calculate vertex areas separately for large meshes.
        /// Based on symmetric derivation of the Laplace-Beltrami operator detailed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="vertexAreas"></param>
        /// <returns></returns>
        public static double[] GetCotanWeights(this HalfedgeList hedges, out double[] vertexAreas)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateCotanWeights(result, out vertexAreas);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        /// <param name="vertexAreas"></param>
        public static void UpdateCotanWeights(this HalfedgeList hedges, IList<double> result, out double[] vertexAreas)
        {
            hedges.SizeCheck(result);
            vertexAreas = new double[hedges.Mesh.Vertices.Count];
            double t = 1.0 / 6.0;

            // calculate cotangent weights and vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;
                double w = 0.0;

                if (he.Face != null)
                {
                    Vec3d v0 = he.Previous.Span;
                    Vec3d v1 = he.Next.Twin.Span;
                    double a = Vec3d.Cross(v0, v1).Length;
                    w += v0 * v1 / a;

                    vertexAreas[he.Start.Index] += a * t; // 1/3rd the triangular area (or 1/6th the parallelgram area)
                }

                he = he.Twin;
                if (he.Face != null)
                {
                    Vec3d v0 = he.Previous.Span;
                    Vec3d v1 = he.Next.Twin.Span;
                    double a = Vec3d.Cross(v0, v1).Length;
                    w += v0 * v1 / a;

                    vertexAreas[he.Start.Index] += a * t; // 1/3rd the triangular area (or 1/6th the parallelgram area)
                }

                result[i] = result [i + 1] = w * 0.5;
            }
        }


        /// <summary>
        /// Normalizes halfedge weights such that the weights of outgoing edges around each vertex sum to 1.
        /// Note that this breaks weight symmetry between halfedge pairs.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        public static void NormalizeHalfedgeWeights(this HalfedgeList hedges, IList<double> halfedgeWeights, bool parallel = false)
        {
            hedges.SizeCheck(halfedgeWeights);
            var verts = hedges.Mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    hedges.NormalizeHalfedgeWeights(halfedgeWeights, range.Item1, range.Item2));
            else
                 hedges.NormalizeHalfedgeWeights(halfedgeWeights, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void NormalizeHalfedgeWeights(this HalfedgeList hedges, IList<double> halfedgeWeights, int i0, int i1)
        {
            var verts = hedges.Mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double sum = 0.0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                    sum += halfedgeWeights[he.Index];

                if (sum > 0.0)
                {
                    sum = 1.0 / sum;
                    foreach (Halfedge he in v.OutgoingHalfedges)
                        halfedgeWeights[he.Index] *= sum;
                }
            }
        }


        /// <summary>
        /// Applies symmetric normalization of halfedge weights based on vertex areas.
        /// Intended for use on triangle meshes.
        /// Based on symmetric derivation of the Laplace-Beltrami operator detailed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="parallel"></param>
        public static void NormalizeHalfedgeWeights(this HalfedgeList hedges, IList<double> halfedgeWeights, IList<double> vertexAreas, bool parallel = false)
        {
            hedges.SizeCheck(halfedgeWeights);
            hedges.Mesh.Vertices.SizeCheck(vertexAreas);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.NormalizeHalfedgeWeights(halfedgeWeights, vertexAreas, range.Item1, range.Item2));
            else
                hedges.NormalizeHalfedgeWeights(halfedgeWeights, vertexAreas, 0, hedges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void NormalizeHalfedgeWeights(this HalfedgeList hedges, IList<double> halfedgeWeights, IList<double> vertexAreas, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he = hedges[j];
                if (he.IsUnused) continue;

                double w = halfedgeWeights[j] / Math.Sqrt(vertexAreas[he.Start.Index] * vertexAreas[he.End.Index]); // symmetric area weighting
                halfedgeWeights[j] = halfedgeWeights[j + 1] = w;
            }
        }


        /// <summary>
        /// Returns the span vector for each halfedge in the mesh.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="unitize"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetHalfedgeVectors(this HalfedgeList hedges, bool unitize, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[hedges.Count];
            hedges.UpdateHalfedgeVectors(unitize, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="unitize"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateHalfedgeVectors(this HalfedgeList hedges, bool unitize, IList<Vec3d> result, bool parallel = false)
        {
            hedges.SizeCheck(result);

            if(unitize)
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                        hedges.UpdateHalfedgeUnitVectors(result, range.Item1, range.Item2));
                else
                    hedges.UpdateHalfedgeUnitVectors(result, 0, hedges.Count >> 1);
            }
            else
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                        hedges.UpdateHalfedgeVectors(result, range.Item1, range.Item2));
                else
                    hedges.UpdateHalfedgeVectors(result, 0, hedges.Count >> 1);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeVectors(this HalfedgeList hedges, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he = hedges[j];
                if (he.IsUnused) continue;

                Vec3d v = he.Span;
                result[j] = v;
                result[j + 1] = -v;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeUnitVectors(this HalfedgeList hedges, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he = hedges[j];
                if (he.IsUnused) continue;

                Vec3d v = he.Span;
                v.Unitize();
                result[j] = v;
                result[j + 1] = -v;
            }
        }


        /// <summary>
        /// Returns the normal for each halfedge in the mesh.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="unitize"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetHalfedgeNormals(this HalfedgeList hedges, bool unitize, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[hedges.Count];
            hedges.UpdateHalfedgeNormals(unitize, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="unitize"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateHalfedgeNormals(this HalfedgeList hedges, bool unitize, IList<Vec3d> result, bool parallel = false)
        {
            hedges.SizeCheck(result);

            if (unitize)
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, hedges.Count), range =>
                        hedges.UpdateHalfedgeUnitNormals(result, range.Item1, range.Item2));
                else
                    hedges.UpdateHalfedgeUnitNormals(result, 0, hedges.Count);
            }
            else
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, hedges.Count), range =>
                        hedges.UpdateHalfedgeNormals(result, range.Item1, range.Item2));
                else
                    hedges.UpdateHalfedgeNormals(result, 0, hedges.Count);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeNormals(this HalfedgeList hedges, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;
                result[i] = he.GetNormal();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeUnitNormals(this HalfedgeList hedges, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d v = he.GetNormal();
                v.Unitize();
                result[i] = v;
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="unitize"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetHalfedgeBisectors(this HalfedgeList hedges, bool unitize, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[hedges.Count];
            hedges.UpdateHalfedgeBisectors(unitize, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="unitize"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateHalfedgeBisectors(this HalfedgeList hedges, bool unitize, IList<Vec3d> result, bool parallel = false)
        {
            hedges.SizeCheck(result);

            if (unitize)
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, hedges.Count), range =>
                        hedges.UpdateHalfedgeUnitBisectors(result, range.Item1, range.Item2));
                else
                    hedges.UpdateHalfedgeUnitBisectors(result, 0, hedges.Count);
            }
            else
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, hedges.Count), range =>
                        hedges.UpdateHalfedgeBisectors(result, range.Item1, range.Item2));
                else
                    hedges.UpdateHalfedgeBisectors(result, 0, hedges.Count);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeBisectors(this HalfedgeList hedges, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d v0 = he.Span;
                Vec3d v1 = he.Previous.Span;

                v0 = (v0 / v0.Length - v1 / v1.Length) * 0.5;
                result[i] = v0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeUnitBisectors(this HalfedgeList hedges, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;

                Vec3d v0 = he.Span;
                Vec3d v1 = he.Previous.Span;

                v0 = (v0 / v0.Length - v1 / v1.Length) * 0.5;
                v0.Unitize();
                result[i] = v0;
            }
        }
        */

        #endregion


        #region Edge Attributes


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <returns></returns>
        public static int[] GetEdgeLabels(this HalfedgeList hedges)
        {
            int[] result = new int[hedges.Count >> 1];
            hedges.UpdateEdgeLabels(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        public static void UpdateEdgeLabels(this HalfedgeList hedges, IList<int> result)
        {
            Stack<Halfedge> stack = new Stack<Halfedge>();
      
            for (int i = 0; i < hedges.Count; i+=2)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused || result[i >> 1] != 0) continue; // skip if unused or already visited

                stack.Push(he);
                UpdateEdgeLabels(stack, result);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int[] GetEdgeLabels(this HalfedgeList hedges, Halfedge start)
        {
            int[] result = new int[hedges.Count >> 1];
            hedges.UpdateEdgeLabels(start, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="start"></param>
        /// <param name="result"></param>
        public static void UpdateEdgeLabels(this HalfedgeList hedges, Halfedge start, IList<int> result)
        {
            start.UsedCheck();
            hedges.OwnsCheck(start);
            hedges.HalfSizeCheck(result);

            Stack<Halfedge> stack = new Stack<Halfedge>();
            stack.Push(start);
            UpdateEdgeLabels(stack, result);
        }


        /// <summary>
        /// Assumes the result array contains default values.
        /// </summary>
        private static void UpdateEdgeLabels(Stack<Halfedge> stack, IList<int> result)
        {
            // TODO finish implementation
            throw new NotImplementedException();
        
            while (stack.Count > 0)
            {
                Halfedge he = stack.Pop();
                int ei = he.Index >> 1;
                if (he.Face == null || result[ei] != 0) continue; // skip if already flagged

                result[ei] = 1; // flag edge

                /*
                // break if on boundary
                if (he.IsBoundary == null) continue;
                */

                // add next halfedges to stack 
                // give preference to one direction over to minimize discontinuities
                stack.Push(he.Twin.Next.Next); // down
                stack.Push(he.Next.Next.Twin); // up
                stack.Push(he.Previous.Twin.Previous); // left
                stack.Push(he.Next.Twin.Next); // right
            }
        }


        /// <summary>
        /// Returns the length of each edge in the mesh.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetEdgeLengths(this HalfedgeList hedges, bool parallel = false)
        {
            double[] result = new double[hedges.Count >> 1];
            hedges.UpdateEdgeLengths(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateEdgeLengths(this HalfedgeList hedges, IList<double> result, bool parallel = false)
        {
            hedges.HalfSizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.UpdateEdgeLengths(result, range.Item1, range.Item2));
            else
                hedges.UpdateEdgeLengths(result, 0, hedges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateEdgeLengths(this HalfedgeList hedges, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i << 1];
                if (he.IsUnused) continue;
                result[i] = he.Span.Length;
            }
        }


        /// <summary>
        /// Returns the dihedral angle for each pair of halfedges in the mesh.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="faceNormals"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetDihedralAngles(this HalfedgeList hedges, IList<Vec3d> faceNormals, bool parallel = false)
        {
            double[] result = new double[hedges.Count >> 1];
            hedges.UpdateDihedralAngles(faceNormals, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="faceNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateDihedralAngles(this HalfedgeList hedges, IList<Vec3d> faceNormals, IList<double> result, bool parallel = false)
        {
            hedges.HalfSizeCheck(result);
            hedges.Mesh.Faces.SizeCheck(faceNormals);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.UpdateDihedralAngles(faceNormals, result, range.Item1, range.Item2));
            else
                hedges.UpdateDihedralAngles(faceNormals, result, 0, hedges.Count >> 1);

        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateDihedralAngles(this HalfedgeList hedges, IList<Vec3d> faceNormals, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i << 1];
                if (he.IsUnused || he.IsBoundary) continue;

                double angle = Vec3d.Angle(faceNormals[he.Face.Index], faceNormals[he.Twin.Face.Index]);
                result[i] = angle;
            }
        }

        #endregion


        #region Face Attributes

        /// <summary>
        /// Returns the boundary status of each face.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static bool[] GetFaceBoundaryStatus(this HeFaceList faces, bool parallel = false)
        {
            bool[] result = new bool[faces.Count];
            faces.UpdateFaceBoundaryStatus(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceBoundaryStatus(this HeFaceList faces, IList<bool> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFaceBoundaryStatus(result, range.Item1, range.Item2));
            else
                faces.UpdateFaceBoundaryStatus(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceBoundaryStatus(this HeFaceList faces, IList<bool> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;
                result[i] = f.IsBoundary;
            }
        }


        /// <summary>
        /// Returns the number of edges in each face.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFaceDegrees(this HeFaceList faces, bool parallel = false)
        {
            int[] result = new int[faces.Count];
            faces.UpdateFaceDegrees(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceDegrees(this HeFaceList faces, IList<int> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFaceDergees(result, range.Item1, range.Item2));
            else
                faces.UpdateFaceDergees(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceDergees(this HeFaceList faces, IList<int> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;
                result[i] = f.Degree;
            }
        }


        /// <summary>
        /// Returns the topological depth of all faces connected to a set of sources.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static int[] GetFaceDepths(this HeFaceList faces, IEnumerable<HeFace> sources)
        {
            int[] result = new int[faces.Count];
            faces.UpdateFaceDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public static void UpdateFaceDepths(this HeFaceList faces, IEnumerable<HeFace> sources, IList<int> result)
        {
            faces.SizeCheck(result);

            var queue = new Queue<HeFace>();
            result.Set(Int32.MaxValue);

            // enqueue sources and set to zero
            foreach (HeFace f in sources)
            {
                faces.OwnsCheck(f);
                if (f.IsUnused) continue;

                queue.Enqueue(f);
                result[f.Index] = 0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                HeFace f0 = queue.Dequeue();
                int t0 = result[f0.Index] + 1;

                foreach (HeFace f1 in f0.AdjacentFaces)
                {
                    int i1 = f1.Index;
                    if (t0 < result[i1])
                    {
                        result[i1] = t0;
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the topological distance of all faces connected to a set of sources.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <returns></returns>
        public static double[] GetFaceDepths(this HeFaceList faces, IEnumerable<HeFace> sources, IList<double> edgeLengths)
        {
            double[] result = new double[faces.Count];
            faces.UpdateFaceDepths(sources, edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        public static void UpdateFaceDepths(this HeFaceList faces, IEnumerable<HeFace> sources, IList<double> edgeLengths, IList<double> result)
        {
            faces.SizeCheck(result);
            faces.Mesh.Halfedges.HalfSizeCheck(edgeLengths);

            var queue = new Queue<HeFace>();
            result.Set(Double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (HeFace f in sources)
            {
                faces.OwnsCheck(f);
                if (f.IsUnused) continue;

                queue.Enqueue(f);
                result[f.Index] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                HeFace f0 = queue.Dequeue();
                double t0 = result[f0.Index];

                foreach (Halfedge he in f0.Halfedges)
                {
                    HeFace f1 = he.Twin.Face;
                    if (f1 == null) continue;

                    int i1 = f1.Index;
                    double t1 = t0 + edgeLengths[he.Index >> 1];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetFaceCenters(this HeFaceList faces, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[faces.Count];
            faces.UpdateFaceCenters(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceCenters(this HeFaceList faces, IList<Vec3d> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    UpdateFaceCenters(faces, result, range.Item1, range.Item2));
            else
                UpdateFaceCenters(faces, result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceCenters(this HeFaceList faces, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;
                result[i] = f.GetCenter();
            }
        }


        /// <summary>
        /// Calculates face normals as the area-weighted sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetFaceNormals(this HeFaceList faces, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[faces.Count];
            faces.UpdateFaceNormals(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceNormals(this HeFaceList faces, IList<Vec3d> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    UpdateFaceNormals(faces, result, range.Item1, range.Item2));
            else
                UpdateFaceNormals(faces, result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceNormals(this HeFaceList faces, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;
                result[i] = f.GetNormal();
            }
        }


        /// <summary>
        /// Calculates face normals as the sum of halfedge normals in each face.
        /// Half-edge normals can be scaled in advance for custom weighting.
        /// Face normals are unitized by default.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="halfedgeNormals"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetFaceNormals(this HeFaceList faces, IList<Vec3d> halfedgeNormals, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[faces.Count];
            faces.UpdateFaceNormals(halfedgeNormals, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="halfedgeNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceNormals(this HeFaceList faces, IList<Vec3d> halfedgeNormals, IList<Vec3d> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFaceNormals(halfedgeNormals, result, range.Item1, range.Item2));
            else
                faces.UpdateFaceNormals(halfedgeNormals, result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceNormals(this HeFaceList faces, IList<Vec3d> halfedgeNormals, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                if (f.IsTri)
                {
                    // simplified tri case
                    Vec3d v = halfedgeNormals[f.First.Index];
                    v.Unitize();
                    result[i] = v;
                }
                else
                {
                    // general ngon case
                    Vec3d sum = new Vec3d();

                    foreach (Halfedge he in f.Halfedges)
                        sum += halfedgeNormals[he.Index];

                    sum.Unitize();
                    result[i] = sum;
                }
            }
        }


        /// <summary>
        /// Calculates face normals as the normal of the first halfedge in each face.
        /// Face normals are unitized by default.
        /// This method assumes all faces are triangular.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetFaceNormalsTri(this HeFaceList faces, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[faces.Count];
            faces.UpdateFaceNormalsTri(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceNormalsTri(this HeFaceList faces, IList<Vec3d> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFaceNormalsTri(result, range.Item1, range.Item2));
            else
                faces.UpdateFaceNormalsTri(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceNormalsTri(this HeFaceList faces, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                Vec3d v = Vec3d.Cross(f.First.Span, f.First.Next.Span);
                v.Unitize();
                result[i] = v;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetFaceAreas(this HeFaceList faces, bool parallel = false)
        {
            double[] result = new double[faces.Count];
            faces.UpdateFaceAreas(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceAreas(this HeFaceList faces, IList<double> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFaceAreas(result, range.Item1, range.Item2));
            else
                faces.UpdateFaceAreas(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceAreas(this HeFaceList faces, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                if (f.IsTri)
                {
                    // simplified tri case
                    Vec3d norm = Vec3d.Cross(f.First.Span, f.First.Next.Span);
                    result[i] = norm.Length * 0.5;
                }
                else
                {
                    // general ngon case
                    Vec3d cen = f.GetCenter();
                    double sum = 0.0;

                    foreach (Halfedge he in f.Halfedges)
                        sum += Vec3d.Cross(he.Start.Position - cen, he.Span).Length * 0.5;

                    result[i] = sum;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="faceCenters"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetFaceAreas(this HeFaceList faces, IList<Vec3d> faceCenters, bool parallel = false)
        {
            double[] result = new double[faces.Count];
            faces.UpdateFaceAreas(faceCenters, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceAreas(this HeFaceList faces, IList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFaceAreas(faceCenters, result, range.Item1, range.Item2));
            else
                faces.UpdateFaceAreas(faceCenters, result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceAreas(this HeFaceList faces, IList<Vec3d> faceCenters, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                if (f.IsTri)
                {
                    // simplified tri case
                    Vec3d norm = Vec3d.Cross(f.First.Span, f.First.Next.Span);
                    result[i] = norm.Length * 0.5;
                }
                else
                {
                    // general ngon case
                    Vec3d cen = faceCenters[i];
                    double sum = 0.0;

                    foreach (Halfedge he in f.Halfedges)
                        sum += Vec3d.Cross(he.Start.Position - cen, he.Span).Length * 0.5;

                    result[i] = sum;
                }
            }
        }


        /// <summary>
        /// Returns the area of each face.
        /// This method assumes all faces are triangular.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetFaceAreasTri(this HeFaceList faces, bool parallel = false)
        {
            double[] result = new double[faces.Count];
            faces.UpdateFaceAreasTri(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceAreasTri(this HeFaceList faces, IList<double> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFaceAreasTri(result, range.Item1, range.Item2));
            else
                faces.UpdateFaceAreasTri(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceAreasTri(this HeFaceList faces, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;
                result[i] = Vec3d.Cross(f.First.Span, f.First.Next.Span).Length * 0.5;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetFacePlanarity(this HeFaceList faces, bool parallel = false)
        {
            double[] result = new double[faces.Count];
            faces.UpdateFacePlanarity(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFacePlanarity(this HeFaceList faces, IList<double> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFacePlanarity(result, range.Item1, range.Item2));
            else
                faces.UpdateFacePlanarity(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFacePlanarity(this HeFaceList faces, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                Halfedge he0 = f.First;
                Halfedge he1 = he0.Next;
                Halfedge he2 = he1.Next;
                Halfedge he3 = he2.Next;
                if (he3 == he0) continue; // ensure face has at least 4 edges

                if (he3.Next == he0)
                {
                    // simplified quad case
                    Vec3d span = GeometryUtil.LineLineShortestVector(he0.Start.Position, he2.Start.Position, he1.Start.Position, he3.Start.Position);
                    result[i] = span.Length;
                }
                else
                {
                    // general ngon case
                    double sum = 0.0;
                    do
                    {
                        Vec3d span = GeometryUtil.LineLineShortestVector(he0.Start.Position, he2.Start.Position, he1.Start.Position, he3.Start.Position);
                        sum += span.Length;

                        // advance to next set of 4 edges
                        he0 = he0.Next;
                        he1 = he1.Next;
                        he2 = he2.Next;
                        he3 = he3.Next;
                    } while (he0 != f.First);

                    result[i] = sum;
                }
            }
        }

        #endregion
    }
}
