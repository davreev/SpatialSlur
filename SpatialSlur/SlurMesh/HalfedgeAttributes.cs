using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 * TODO convert extension methods to partial class
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Methods for calculating various halfedge attributes.
    /// </summary>
    public partial class HalfedgeList
    {
        /// <summary>
        /// Returns the length of each halfedge in the mesh.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetHalfedgeLengths(bool parallel = false)
        {
            var result = new double[Count];
            GetHalfedgeLengths(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeLengths(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
                    GetHalfedgeLengths(result, range.Item1 << 1, range.Item2 << 1));
            else
                GetHalfedgeLengths(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHalfedgeLengths(IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i += 2)
            {
                Halfedge he = this[i];
                if (he.IsUnused) continue;

                double d = he.Span.Length;
                result[i] = d;
                result[i + 1] = d;
            }
        }


        /// <summary>
        /// Returns the angle between each halfedge and its previous.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetHalfedgeAngles(bool parallel = false)
        {
            var result = new double[Count];
            GetHalfedgeAngles(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeAngles(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetHalfedgeAngles(result, range.Item1, range.Item2));
            else
                GetHalfedgeAngles(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHalfedgeAngles(IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = this[i];
                if (he.IsUnused) continue;
                result[i] = he.GetAngle();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeLengths"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetHalfedgeAngles2(IList<double> edgeLengths, bool parallel = false)
        {
            var result = new double[Count];
            GetHalfedgeAngles2(edgeLengths, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeAngles2(IList<double> edgeLengths, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            HalfSizeCheck(edgeLengths);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetHalfedgeAngles2(edgeLengths, result, range.Item1, range.Item2));
            else
                GetHalfedgeAngles2(edgeLengths, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHalfedgeAngles2(IList<double> edgeLengths, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he0 = this[i];
                if (he0.IsUnused) continue;

                Halfedge he1 = he0.Previous;
                double d = edgeLengths[i >> 1] * edgeLengths[he1.Index >> 1];

                if (d > 0.0)
                    result[i] = Math.Acos(SlurMath.Clamp(he0.Span * he1.Span / d, -1.0, 1.0)); // clamp dot product to remove noise
                else
                    result[i] = Double.NaN;
            }
        }


        /// <summary>
        /// Returns the area associated with each halfedge.
        /// This is calculated as W in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetHalfedgeAreas(IList<Vec3d> faceCenters, bool parallel = false)
        {
            var result = new double[Count];
            GetHalfedgeAreas(faceCenters, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeAreas(IList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Faces.SizeCheck(faceCenters);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetHalfedgeAreas(faceCenters, result, range.Item1, range.Item2));
            else
                GetHalfedgeAreas(faceCenters, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHalfedgeAreas(IList<Vec3d> faceCenters, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = this[i];
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
        /// Calculates the cotangent of each halfedge.
        /// Assumes triangular faces.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetHalfedgeCotangents(bool parallel = false)
        {
            var result = new double[Count];
            GetHalfedgeCotangents(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeCotangents(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetHalfedgeCotangents(result, range.Item1, range.Item2));
            else
                GetHalfedgeCotangents(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHalfedgeCotangents(IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = this[i];
                if (he.IsUnused || he.Face == null) continue;

                Vec3d v0 = he.Previous.Span;
                Vec3d v1 = he.Next.Twin.Span;
                result[i] = v0 * v1 / Vec3d.Cross(v0, v1).Length;
            }
        }


        /// <summary>
        /// Returns the symmetric cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Pinkall and Polthier's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetCotanWeights(bool parallel = false)
        {
            var result = new double[Count];
            GetCotanWeights(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCotanWeights(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
                    GetCotanWeights(result, range.Item1, range.Item2));
            else
                GetCotanWeights(result, 0, Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCotanWeights(IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he0 = this[j];
                if (he0.IsUnused) continue;

                Halfedge he1 = this[j + 1];
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


        /// <summary>
        /// Returns the area-dependant cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="vertexAreas"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetCotanWeights2(IList<double> vertexAreas, bool parallel = false)
        {
            var result = new double[Count];
            GetCotanWeights2(vertexAreas, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCotanWeights2(IList<double> vertexAreas, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Vertices.SizeCheck(vertexAreas);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
                    GetCotanWeights2(vertexAreas, result, range.Item1, range.Item2));
            else
                GetCotanWeights2(vertexAreas, result, 0, Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCotanWeights2(IList<double> vertexAreas, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he0 = this[j];
                if (he0.IsUnused) continue;

                Halfedge he1 = this[j + 1];
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
        /// Returns the area-dependant cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Meyer and Desbrun's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="vertexAreas"></param>
        /// <returns></returns>
        public double[] GetCotanWeights2(out double[] vertexAreas)
        {
            var result = new double[Count];
            vertexAreas = new double[Mesh.Vertices.Count];
            GetCotanWeights2Impl(result, vertexAreas);
            return result;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vertexAreasOut"></param>
        public void GetCotanWeights2(IList<double> result, IList<double> vertexAreasOut)
        {
            SizeCheck(result);
            result.Set(0.0);
            vertexAreasOut.Set(0.0);
            GetCotanWeights2Impl(result, vertexAreasOut);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vertexAreasOut"></param>
        private void GetCotanWeights2Impl(IList<double> result, IList<double> vertexAreasOut)
        {
            const double t = 1.0 / 6.0;

            // accumulate cotangent weights and vertex areas
            for (int i = 0; i < Count; i++)
            {
                Halfedge he = this[i];
                if (he.IsUnused || he.Face == null) continue;

                Vec3d v0 = he.Previous.Span;
                Vec3d v1 = he.Next.Twin.Span;
                double a = Vec3d.Cross(v0, v1).Length;

                result[i - (i & 1)] += v0 * v1 / a; // increment at index of first edge in pair
                vertexAreasOut[he.Start.Index] += a * t; // 1/3rd the triangular area (or 1/6th the parallelogram area)
            }

            // normalize weights by vertex areas
            for (int i = 0; i < Count; i += 2)
            {
                Halfedge he = this[i];
                if (he.IsUnused) continue;

                double w = result[i] * 0.5;
                result[i] = w / vertexAreasOut[he.Start.Index];
                result[i + 1] = w / vertexAreasOut[he.End.Index];
            }
        }


        /// <summary>
        /// Returns the symmetric, area-dependant, cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Levy and Vallet's derivation of the symmetric Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="vertexAreas"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetCotanWeights3(IList<double> vertexAreas, bool parallel = false)
        {
            var result = new double[Count];
            GetCotanWeights3(vertexAreas, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexAreas"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCotanWeights3(IList<double> vertexAreas, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            Mesh.Vertices.SizeCheck(vertexAreas);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
                    GetCotanWeights3(vertexAreas, result, range.Item1, range.Item2));
            else
                GetCotanWeights3(vertexAreas, result, 0, Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetCotanWeights3(IList<double> vertexAreas, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he0 = this[j];
                if (he0.IsUnused) continue;

                Halfedge he1 = this[j + 1];
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
        /// Returns the symmetric, area-dependant, cotangent weight for each halfedge.
        /// Assumes triangular faces.
        /// Based on Levy and Vallet's derivation of the Laplace-Beltrami operator discussed in http://reuter.mit.edu/papers/reuter-smi09.pdf.
        /// </summary>
        /// <param name="vertexAreas"></param>
        /// <returns></returns>
        public double[] GetCotanWeights3(out double[] vertexAreas)
        {
            var result = new double[Count];
            vertexAreas = new double[Mesh.Vertices.Count];
            GetCotanWeights3Impl(result, vertexAreas);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vertexAreasOut"></param>
        public void GetCotanWeights3(IList<double> result, IList<double> vertexAreasOut)
        {
            SizeCheck(result);
            result.Set(0.0);
            vertexAreasOut.Set(0.0);
            GetCotanWeights3Impl(result, vertexAreasOut);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vertexAreasOut"></param>
        private void GetCotanWeights3Impl(IList<double> result, IList<double> vertexAreasOut)
        {
            const double t = 1.0 / 6.0;

            // accumulate cotangent weights and vertex areas
            for (int i = 0; i < Count; i++)
            {
                Halfedge he = this[i];
                if (he.IsUnused || he.Face == null) continue;

                Vec3d v0 = he.Previous.Span;
                Vec3d v1 = he.Next.Twin.Span;
                double a = Vec3d.Cross(v0, v1).Length;

                result[i - (i & 1)] += v0 * v1 / a; // increment at index of first edge in pair
                vertexAreasOut[he.Start.Index] += a * t; // 1/3rd the triangular area (or 1/6th the parallelogram area)
            }

            // symmetrically normalize weights by vertex areas
            for (int i = 0; i < Count; i += 2)
            {
                Halfedge he = this[i];
                if (he.IsUnused) continue;

                double w = result[i] * 0.5;
                result[i] = result[i + 1] = w / Math.Sqrt(vertexAreasOut[he.Start.Index] * vertexAreasOut[he.End.Index]);
            }
        }


        /// <summary>
        /// Normalizes halfedge weights such that the weights of outgoing edges around each vertex sum to 1.
        /// Note that this breaks weight symmetry between halfedge pairs.
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <param name="parallel"></param>
        public void NormalizeHalfedgeWeights(IList<double> halfedgeWeights, bool parallel = false)
        {
            SizeCheck(halfedgeWeights);
            var verts = Mesh.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Mesh.Vertices.Count), range =>
                    NormalizeHalfedgeWeights(halfedgeWeights, range.Item1, range.Item2));
            else
                NormalizeHalfedgeWeights(halfedgeWeights, 0, Mesh.Vertices.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void NormalizeHalfedgeWeights(IList<double> halfedgeWeights, int i0, int i1)
        {
            var verts = Mesh.Vertices;

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
        /// Returns the span vector for each halfedge in the mesh.
        /// </summary>
        /// <param name="unitize"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetHalfedgeVectors(bool unitize, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetHalfedgeVectors(unitize, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitize"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeVectors(bool unitize, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (unitize)
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
                        GetHalfedgeUnitVectors(result, range.Item1, range.Item2));
                else
                    GetHalfedgeUnitVectors(result, 0, Count >> 1);
            }
            else
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
                        GetHalfedgeVectors(result, range.Item1, range.Item2));
                else
                    GetHalfedgeVectors(result, 0, Count >> 1);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHalfedgeVectors(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he = this[j];
                if (he.IsUnused) continue;

                Vec3d v = he.Span;
                result[j] = v;
                result[j + 1] = -v;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHalfedgeUnitVectors(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                int j = i << 1;

                Halfedge he = this[j];
                if (he.IsUnused) continue;

                Vec3d v = he.Span;
                v.Unitize();
                result[j] = v;
                result[j + 1] = -v;
            }
        }


        /// <summary>
        /// Returns the normal vector for each halfedge in the mesh.
        /// </summary>
        /// <param name="unitize"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetHalfedgeNormals(bool unitize, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetHalfedgeNormals(unitize, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitize"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetHalfedgeNormals( bool unitize, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (unitize)
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, Count), range =>
                        GetHalfedgeUnitNormals(result, range.Item1, range.Item2));
                else
                    GetHalfedgeUnitNormals(result, 0, Count);
            }
            else
            {
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, Count), range =>
                        GetHalfedgeNormals(result, range.Item1, range.Item2));
                else
                    GetHalfedgeNormals(result, 0, Count);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHalfedgeNormals(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = this[i];
                if (he.IsUnused) continue;
                result[i] = he.GetNormal();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetHalfedgeUnitNormals(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = this[i];
                if (he.IsUnused) continue;

                Vec3d v = he.GetNormal();
                v.Unitize();
                result[i] = v;
            }
        }
    }
}
