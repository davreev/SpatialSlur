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
    /// Methods for calculating various face attributes.
    /// </summary>
    public partial class HeFaceList
    {
        /// <summary>
        /// Returns the number of edges in each face.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public int[] GetFaceEdgeCounts(bool parallel = false)
        {
            var result = new int[Count];
            GetFaceEdgeCounts(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceEdgeCounts(IList<int> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceEdgeCounts(result, range.Item1, range.Item2));
            else
                GetFaceEdgeCounts(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceEdgeCounts(IList<int> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var f = this[i];
                if (f.IsUnused) continue;
                result[i] = f.CountEdges();
            }
        }

        /// <summary>
        /// Returns the number of boundary edges in each face.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public int[] GetFaceBoundaryCounts(bool parallel = false)
        {
            var result = new int[Count];
            GetFaceBoundaryCounts(result, parallel);
            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceBoundaryCounts(IList<int> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceBoundaryCounts(result, range.Item1, range.Item2));
            else
                GetFaceBoundaryCounts(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceBoundaryCounts(IList<int> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var f = this[i];
                if (f.IsUnused) continue;
                result[i] = f.CountBoundaryEdges();
            }
        }


        /// <summary>
        /// Calculates the topological depth of all faces connected to a set of sources.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public int[] GetFaceDepths(IEnumerable<HeFace> sources)
        {
            var result = new int[Count];
            GetFaceDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetFaceDepths(IEnumerable<HeFace> sources, IList<int> result)
        {
            SizeCheck(result);

            var queue = new Queue<HeFace>();
            result.Set(Int32.MaxValue);

            // enqueue sources and set to zero
            foreach (HeFace f in sources)
            {
                OwnsCheck(f);
                if (f.IsUnused) continue;

                queue.Enqueue(f);
                result[f.Index] = 0;
            }

            GetFaceDepths(queue, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public int[] GetFaceDepths(IEnumerable<int> sources)
        {
            var result = new int[Count];
            GetFaceDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetFaceDepths(IEnumerable<int> sources, IList<int> result)
        {
            SizeCheck(result);

            var queue = new Queue<HeFace>();
            result.Set(Int32.MaxValue);

            // enqueue sources and set to zero
            foreach (int fi in sources)
            {
                var f = this[fi];
                if (f.IsUnused) continue;

                queue.Enqueue(f);
                result[fi] = 0;
            }

            GetFaceDepths(queue, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetFaceDepths(Queue<HeFace> queue, IList<int> result)
        {
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
        /// Calculates the topological distance of all faces connected to a set of sources.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public double[] GetFaceDistances(IEnumerable<HeFace> sources, IList<double> edgeWeights)
        {
            var result = new double[Count];
            GetFaceDistances(sources, edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void GetFaceDistances(IEnumerable<HeFace> sources, IList<double> edgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Mesh.Halfedges.HalfSizeCheck(edgeWeights);

            var queue = new Queue<HeFace>();
            result.Set(Double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (HeFace f in sources)
            {
                OwnsCheck(f);
                if (f.IsUnused) continue;

                queue.Enqueue(f);
                result[f.Index] = 0.0;
            }

            GetFaceDistances(queue, edgeWeights, result);
        }


        /// <summary>
        /// Calculates the topological distance of all faces connected to a set of sources.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public double[] GetFaceDistances(IEnumerable<int> sources, IList<double> edgeWeights)
        {
            var result = new double[Count];
            GetFaceDistances(sources, edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void GetFaceDistances(IEnumerable<int> sources, IList<double> edgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Mesh.Halfedges.HalfSizeCheck(edgeWeights);

            var queue = new Queue<HeFace>();
            result.Set(double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (int fi in sources)
            {
                var f = this[fi];
                if (f.IsUnused) continue;

                queue.Enqueue(f);
                result[fi] = 0.0;
            }

            GetFaceDistances(queue, edgeWeights, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        private static void GetFaceDistances(Queue<HeFace> queue, IList<double> edgeWeights, IList<double> result)
        {
            // TODO switch to priority queue implementation
            while (queue.Count > 0)
            {
                HeFace f0 = queue.Dequeue();
                double t0 = result[f0.Index];

                foreach (Halfedge he in f0.Halfedges)
                {
                    HeFace f1 = he.Twin.Face;
                    if (f1 == null) continue;

                    int i1 = f1.Index;
                    double t1 = t0 + edgeWeights[he.Index >> 1];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(f1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the average vertex position within each face.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetFaceBarycenters(bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetFaceBarycenters(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceBarycenters(IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceBarycenters(result, range.Item1, range.Item2));
            else
                GetFaceBarycenters(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceBarycenters(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
                if (f.IsUnused) continue;
                result[i] = f.GetBarycenter();
            }
        }


        /// <summary>
        /// Returns the circumcenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetFaceCircumcenters(bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetFaceCircumcenters(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceCircumcenters(IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceCircumcenters(result, range.Item1, range.Item2));
            else
                GetFaceCircumcenters(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceCircumcenters(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
                if (f.IsUnused) continue;
                result[i] = f.GetCircumcenter();
            }
        }


        /// <summary>
        /// Returns the incenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetFaceIncenters(bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetFaceIncenters(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceIncenters(IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceIncenters(result, range.Item1, range.Item2));
            else
                GetFaceIncenters(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceIncenters(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
                if (f.IsUnused) continue;
                result[i] = f.GetIncenter();
            }
        }


        /// <summary>
        /// Returns the incenter of each face.
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="edgeLengths"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetFaceIncenters(IList<double> edgeLengths, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetFaceIncenters(edgeLengths, result, parallel);
            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceIncenters(IList<double> edgeLengths, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceIncenters(edgeLengths, result, range.Item1, range.Item2));
            else
                GetFaceIncenters(edgeLengths, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceIncenters(IList<double> edgeLengths, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
                if (f.IsUnused) continue;

                var he0 = f.First;
                var he1 = he0.Next;
                var he2 = he1.Next;

                Vec3d p0 = he0.Start.Position;
                Vec3d p1 = he1.Start.Position;
                Vec3d p2 = he2.Start.Position;

                double d01 = edgeLengths[he0.Index >> 1];
                double d12 = edgeLengths[he1.Index >> 1]; 
                double d20 = edgeLengths[he2.Index >> 1]; 
                double pInv = 1.0 / (d01 + d12 + d20); // inverse perimeter

                result[i] = p0 * (d12 * pInv) + p1 * (d20 * pInv) + p2 * (d01 * pInv);
            }
        }


        /// <summary>
        /// Calculates face normals as the area-weighted sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetFaceNormals(bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetFaceNormals(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceNormals(IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceNormals(result, range.Item1, range.Item2));
            else
                GetFaceNormals(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceNormals(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
                if (f.IsUnused) continue;
                result[i] = f.GetNormal();
            }
        }


        /// <summary>
        /// Calculates face normals as the sum of halfedge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        /// <param name="halfedgeNormals"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetFaceNormals2(IList<Vec3d> halfedgeNormals, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetFaceNormals2(halfedgeNormals, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceNormals2 (IList<Vec3d> halfedgeNormals, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceNormals2(halfedgeNormals, result, range.Item1, range.Item2));
            else
                GetFaceNormals2(halfedgeNormals, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceNormals2(IList<Vec3d> halfedgeNormals, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
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
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetFaceNormalsTri(bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetFaceNormalsTri( result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceNormalsTri(IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceNormalsTri(result, range.Item1, range.Item2));
            else
                GetFaceNormalsTri(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceNormalsTri(IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
                if (f.IsUnused) continue;

                Vec3d v = f.First.GetNormal();
                v.Unitize();
                result[i] = v;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetFaceAreas(bool parallel = false)
        {
            var result = new double[Count];
            GetFaceAreas(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceAreas(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceAreas(result, range.Item1, range.Item2));
            else
                GetFaceAreas(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceAreas(IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
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
                    Vec3d cen = f.GetBarycenter();
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
        /// <param name="faceCenters"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetFaceAreas(IList<Vec3d> faceCenters, bool parallel = false)
        {
            var result = new double[Count];
            GetFaceAreas(faceCenters, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceAreas( IList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceAreas(faceCenters, result, range.Item1, range.Item2));
            else
                GetFaceAreas(faceCenters, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceAreas(IList<Vec3d> faceCenters, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
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
        /// Calculates the area of each face.
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetFaceAreasTri(bool parallel = false)
        {
            var result = new double[Count];
            GetFaceAreasTri(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFaceAreasTri(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFaceAreasTri(result, range.Item1, range.Item2));
            else
                GetFaceAreasTri(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFaceAreasTri(IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
                if (f.IsUnused) continue;
                result[i] = Vec3d.Cross(f.First.Span, f.First.Next.Span).Length * 0.5;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetFacePlanarity(bool parallel = false)
        {
            var result = new double[Count];
            GetFacePlanarity(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetFacePlanarity(IList<double> result, bool parallel = false)
        {
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetFacePlanarity(result, range.Item1, range.Item2));
            else
                GetFacePlanarity(result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetFacePlanarity(IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = this[i];
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
    }
}
