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
    /// Extension methods for calculating various vertex attributes.
    /// </summary>
    public static class HeVertexAttributes
    {
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

                foreach (Halfedge2 he in v0.IncomingHalfedges)
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
                Halfedge2 first = v0.First.Twin;
                double t1 = values[first.Start.Index];

                bool last = (t1 < t0); // was the last neighbour lower?
                int count = 0; // number of discontinuities
                first = first.Next.Twin;

                // circulate remaining neighbours
                Halfedge2 he = first;
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
                foreach (Halfedge2 he in v.OutgoingHalfedges)
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

                foreach (Halfedge2 he in v.OutgoingHalfedges)
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

                double a = f.First.GetNormal().Length * t; // * 0.5
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

                foreach (Halfedge2 he in v.OutgoingHalfedges)
                {
                    if (he.Face == null) continue; // skip boundary edges
                    sum += (edgeLengths[he.Index >> 1] + edgeLengths[he.Previous.Index >> 1] - edgeLengths[he.Next.Index >> 1]) * 0.5;
                    n++;
                }

                result[v.Index] = sum / n;
            }
        }


        /// <summary>
        /// Calculates mean curvature as half the length of the laplacian of vertex positions.
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        public static double[] GetMeanCurvature(this HeVertexList verts, IList<Vec3d> vertexLaplacians, bool parallel = false)
        {
            double[] result = new double[verts.Count];
            verts.UpdateMeanCurvature(vertexLaplacians, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="result"></param>
        public static void UpdateMeanCurvature(this HeVertexList verts, IList<Vec3d> vertexLaplacians, IList<double> result, bool parallel = false)
        {
            verts.SizeCheck(result);
            verts.SizeCheck(vertexLaplacians);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                verts.UpdateMeanCurvature(vertexLaplacians, result, range.Item1, range.Item2));
            else
                verts.UpdateMeanCurvature(vertexLaplacians, result, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateMeanCurvature(this HeVertexList verts, IList<Vec3d> vertexLaplacians, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused || v.IsBoundary) continue;
                result[i] = vertexLaplacians[i].Length * 0.5;
            }
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
                foreach (Halfedge2 he in v.OutgoingHalfedges)
                    sum += Math.PI - he.GetAngle();

                result[i] = Math.Abs(SlurMath.Tau - sum);
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
                foreach (Halfedge2 he in v.OutgoingHalfedges)
                    sum += Math.PI - halfedgeAngles[he.Index];

                result[i] = Math.Abs(SlurMath.Tau - sum);
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

                foreach (Halfedge2 he in v.OutgoingHalfedges)
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

                foreach (Halfedge2 he in v.IncomingHalfedges)
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

                foreach (Halfedge2 he in v.OutgoingHalfedges)
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

                foreach (Halfedge2 he in v.OutgoingHalfedges)
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

                foreach (Halfedge2 he in v.OutgoingHalfedges)
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

                foreach (Halfedge2 he in v.OutgoingHalfedges)
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

                foreach (Halfedge2 he in v.OutgoingHalfedges)
                    sum += (values[he.End.Index] - t) * halfedgeWeights[he.Index];

                result[i] = sum;
            }
        }
    }
}
