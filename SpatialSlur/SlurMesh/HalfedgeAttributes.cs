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
    /// Extension methods for calculating various halfedge attributes.
    /// </summary>
    public static class HalfedgeAttributes
    {
        /// <summary>
        /// Returns the length of each halfedge in the mesh.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetHalfedgeLengths(this HalfedgeList2 hedges, bool parallel = false)
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
        public static void UpdateHalfedgeLengths(this HalfedgeList2 hedges, IList<double> result, bool parallel = false)
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
        private static void UpdateHalfedgeLengths(this HalfedgeList2 hedges, IList<double> result, int i0, int i1)
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
        public static double[] GetHalfedgeAngles(this HalfedgeList2 hedges, bool parallel = false)
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
        public static void UpdateHalfedgeAngles(this HalfedgeList2 hedges, IList<double> result, bool parallel = false)
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
        private static void UpdateHalfedgeAngles(this HalfedgeList2 hedges, IList<double> result, int i0, int i1)
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
        public static double[] GetHalfedgeAngles(this HalfedgeList2 hedges, IList<double> edgeLengths, bool parallel = false)
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
        public static void UpdateHalfedgeAngles(this HalfedgeList2 hedges, IList<double> edgeLengths, IList<double> result, bool parallel = false)
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
        private static void UpdateHalfedgeAngles(this HalfedgeList2 hedges, IList<double> edgeLengths, IList<double> result, int i0, int i1)
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
        public static double[] GetHalfedgeAreas(this HalfedgeList2 hedges, IList<Vec3d> faceCenters, bool parallel = false)
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
        public static void UpdateHalfedgeAreas(this HalfedgeList2 hedges, IList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            hedges.SizeCheck(result);
            hedges.Mesh.Faces.SizeCheck(faceCenters);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count), range =>
                    hedges.UpdateHalfedgeAreas(faceCenters, result, range.Item1, range.Item2));
            else
                hedges.UpdateHalfedgeAreas(faceCenters, result, 0, hedges.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateHalfedgeAreas(this HalfedgeList2 hedges, IList<Vec3d> faceCenters, IList<double> result, int i0, int i1)
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
        /// Assumes all faces are triangles.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetHalfedgeCotangents(this HalfedgeList2 hedges, bool parallel = false)
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
        public static void UpdateHalfedgeCotangents(this HalfedgeList2 hedges, IList<double> result, bool parallel = false)
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
        private static void UpdateHalfedgeCotangents(this HalfedgeList2 hedges, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused || he.Face == null) continue;

                Vec3d v0 = he.Previous.Span;
                Vec3d v1 = he.Next.Twin.Span;
                result[i] = v0 * v1 / Vec3d.Cross(v0, v1).Length;
            }
        }


        /// <summary>
        /// Calculates the cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="vertexAreas"></param>
        /// <returns></returns>
        public static double[] GetCotanWeights(this HalfedgeList2 hedges, out double[] vertexAreas)
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
        public static void UpdateCotanWeights(this HalfedgeList2 hedges, IList<double> result, out double[] vertexAreas)
        {
            hedges.SizeCheck(result);
            hedges.UpdateCotanWeightsImpl(result, out vertexAreas);

            // normalize weights by vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;

                double w = result[i] * 0.5;
                result[i] = w / vertexAreas[he.Start.Index];
                result[i + 1] = w / vertexAreas[he.End.Index];
            }
        }


        /// <summary>
        /// Calculates the symmetric cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Levy and Vallet's derivation of the symmetric Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="vertexAreas"></param>
        /// <returns></returns>
        public static double[] GetCotanWeightsSymmetric(this HalfedgeList2 hedges, out double[] vertexAreas)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateCotanWeightsSymmetric(result, out vertexAreas);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        /// <param name="vertexAreas"></param>
        public static void UpdateCotanWeightsSymmetric(this HalfedgeList2 hedges, IList<double> result, out double[] vertexAreas)
        {
            hedges.SizeCheck(result);
            hedges.UpdateCotanWeightsImpl(result, out vertexAreas);

            // normalize weights by vertex areas
            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused) continue;

                double w = result[i] * 0.5;
                result[i] = result[i + 1] = w / Math.Sqrt(vertexAreas[he.Start.Index] * vertexAreas[he.End.Index]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateCotanWeightsImpl(this HalfedgeList2 hedges, IList<double> result, out double[] vertexAreas)
        {
            vertexAreas = new double[hedges.Mesh.Vertices.Count];
            double t = 1.0 / 6.0;

            // calculate cotangent weights and vertex areas
            for (int i = 0; i < hedges.Count; i++)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused || he.Face == null) continue;

                Vec3d v0 = he.Previous.Span;
                Vec3d v1 = he.Next.Twin.Span;
                double a = Vec3d.Cross(v0, v1).Length;

                result[i - (i & 1)] += v0 * v1 / a; // increment at index of first edge in pair
                vertexAreas[he.Start.Index] += a * t; // 1/3rd the triangular area (or 1/6th the parallelogram area)
            }
        }


        /*
        /// <summary>
        /// Calculates the symmetric cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Pinkall and Polthier's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
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

                Halfedge he0 = hedges[j];
                if (he0.IsUnused) continue;

                Halfedge he1 = hedges[j + 1];
                double w = 0.0;

                if (he0.Face != null)
                {
                    Vec3d v0 = he0.Previous.Span;
                    Vec3d v1 = he0.Next.Twin.Span;
                    w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                }

                if (he1.Face != null)
                {
                    Vec3d v0 = he1.Previous.Span;
                    Vec3d v1 = he1.Next.Twin.Span;
                    w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                }

                result[j] = result[j + 1] = w * 0.5;
            }
        }
        */


        /// <summary>
        /// Calculates the cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetCotanWeights(this HalfedgeList2 hedges, IList<double> vertexAreas, bool parallel = false)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateCotanWeights(vertexAreas, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateCotanWeights(this HalfedgeList2 hedges, IList<double> vertexAreas, IList<double> result, bool parallel = false)
        {
            hedges.SizeCheck(result);
            hedges.Mesh.Vertices.SizeCheck(vertexAreas);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.UpdateCotanWeights(vertexAreas, result, range.Item1, range.Item2));
            else
                hedges.UpdateCotanWeights(vertexAreas, result, 0, hedges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateCotanWeights(this HalfedgeList2 hedges, IList<double> vertexAreas, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he0 = hedges[j];
                if (he0.IsUnused) continue;

                Halfedge he1 = hedges[j + 1];
                double w = 0.0;

                if (he0.Face != null)
                {
                    Vec3d v0 = he0.Previous.Span;
                    Vec3d v1 = he0.Next.Twin.Span;
                    w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                }

                if (he1.Face != null)
                {
                    Vec3d v0 = he1.Previous.Span;
                    Vec3d v1 = he1.Next.Twin.Span;
                    w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                }

                w *= 0.5;
                result[j] = w / vertexAreas[he0.Start.Index];
                result[j + 1] = w / vertexAreas[he1.Start.Index];
            }
        }


        /// <summary>
        /// Calculates the symmetric cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Levy and Vallet's derivation of the symmetric Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetCotanWeightsSymmetric(this HalfedgeList2 hedges, IList<double> vertexAreas, bool parallel = false)
        {
            double[] result = new double[hedges.Count];
            hedges.UpdateCotanWeightsSymmetric(vertexAreas, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateCotanWeightsSymmetric(this HalfedgeList2 hedges, IList<double> vertexAreas, IList<double> result, bool parallel = false)
        {
            hedges.SizeCheck(result);
            hedges.Mesh.Vertices.SizeCheck(vertexAreas);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.UpdateCotanWeightsSymmetric(vertexAreas, result, range.Item1, range.Item2));
            else
                hedges.UpdateCotanWeightsSymmetric(vertexAreas, result, 0, hedges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateCotanWeightsSymmetric(this HalfedgeList2 hedges, IList<double> vertexAreas, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he0 = hedges[j];
                if (he0.IsUnused) continue;

                Halfedge he1 = hedges[j + 1];
                double w = 0.0;

                if (he0.Face != null)
                {
                    Vec3d v0 = he0.Previous.Span;
                    Vec3d v1 = he0.Next.Twin.Span;
                    w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                }

                if (he1.Face != null)
                {
                    Vec3d v0 = he1.Previous.Span;
                    Vec3d v1 = he1.Next.Twin.Span;
                    w += v0 * v1 / Vec3d.Cross(v0, v1).Length;
                }

                result[j] = result[j + 1] = w * 0.5 / Math.Sqrt(vertexAreas[he0.Start.Index] * vertexAreas[he1.Start.Index]);
            }
        }


        /// <summary>
        /// Normalizes halfedge weights such that the weights of outgoing edges around each vertex sum to 1.
        /// Note that this breaks weight symmetry between halfedge pairs.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        public static void NormalizeHalfedgeWeights(this HalfedgeList2 hedges, IList<double> halfedgeWeights, bool parallel = false)
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
        private static void NormalizeHalfedgeWeights(this HalfedgeList2 hedges, IList<double> halfedgeWeights, int i0, int i1)
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


        /*
        /// <summary>
        /// Applies normalization of halfedge weights based on given vertex areas.
        /// Note that this breaks symmetry between halfedge pairs.
        /// Intended for use on triangle meshes.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="parallel"></param>
        public static void NormalizeHalfedgeWeights(this HalfedgeList hedges, IList<double> halfedgeWeights, IList<double> vertexAreas, bool parallel = false)
        {
            hedges.SizeCheck(halfedgeWeights);

            var verts = hedges.Mesh.Vertices;
            hedges.Mesh.Vertices.SizeCheck(vertexAreas);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                    verts.NormalizeHalfedgeWeights(halfedgeWeights, vertexAreas, range.Item1, range.Item2));
            else
                verts.NormalizeHalfedgeWeights(halfedgeWeights, vertexAreas, 0, verts.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void NormalizeHalfedgeWeights(this HeVertexList verts, IList<double> halfedgeWeights, IList<double> vertexAreas, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double a = 1.0 / vertexAreas[i];

                foreach (Halfedge he in v.OutgoingHalfedges)
                    halfedgeWeights[he.Index] *= a;
            }
        }


        /// <summary>
        /// Applies symmetric normalization of halfedge weights based on vertex areas.
        /// Intended for use on triangle meshes.
        /// Based on symmetric derivation of the Laplace-Beltrami operator detailed in http://www.cs.jhu.edu/~misha/ReadingSeminar/Papers/Vallet08.pdf.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="halfedgeWeights"></param>
        /// <param name="vertexAreas"></param>
        /// <param name="parallel"></param>
        public static void NormalizeHalfedgeWeightsSymmetric(this HalfedgeList hedges, IList<double> halfedgeWeights, IList<double> vertexAreas, bool parallel = false)
        {
            hedges.SizeCheck(halfedgeWeights);
            hedges.Mesh.Vertices.SizeCheck(vertexAreas);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.NormalizeHalfedgeWeightsSymmetric(halfedgeWeights, vertexAreas, range.Item1, range.Item2));
            else
                hedges.NormalizeHalfedgeWeightsSymmetric(halfedgeWeights, vertexAreas, 0, hedges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void NormalizeHalfedgeWeightsSymmetric(this HalfedgeList hedges, IList<double> halfedgeWeights, IList<double> vertexAreas, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;
                Halfedge he = hedges[j];
                if (he.IsUnused) continue;

                double w = halfedgeWeights[j] / Math.Sqrt(vertexAreas[he.Start.Index] * vertexAreas[he.End.Index]);
                halfedgeWeights[j] = halfedgeWeights[j + 1] = w;
            }
        }
        */


        /// <summary>
        /// Returns the span vector for each halfedge in the mesh.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="unitize"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetHalfedgeVectors(this HalfedgeList2 hedges, bool unitize, bool parallel = false)
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
        public static void UpdateHalfedgeVectors(this HalfedgeList2 hedges, bool unitize, IList<Vec3d> result, bool parallel = false)
        {
            hedges.SizeCheck(result);

            if (unitize)
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
        private static void UpdateHalfedgeVectors(this HalfedgeList2 hedges, IList<Vec3d> result, int i0, int i1)
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
        private static void UpdateHalfedgeUnitVectors(this HalfedgeList2 hedges, IList<Vec3d> result, int i0, int i1)
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
        public static Vec3d[] GetHalfedgeNormals(this HalfedgeList2 hedges, bool unitize, bool parallel = false)
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
        public static void UpdateHalfedgeNormals(this HalfedgeList2 hedges, bool unitize, IList<Vec3d> result, bool parallel = false)
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
        private static void UpdateHalfedgeNormals(this HalfedgeList2 hedges, IList<Vec3d> result, int i0, int i1)
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
        private static void UpdateHalfedgeUnitNormals(this HalfedgeList2 hedges, IList<Vec3d> result, int i0, int i1)
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
    }
}
