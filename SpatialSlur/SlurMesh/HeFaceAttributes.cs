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
    /// Extension methods for calculating various face attributes.
    /// </summary>
    public static class HeFaceAttributes
    {
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
        public static double[] GetFaceDistances(this HeFaceList faces, IEnumerable<HeFace> sources, IList<double> edgeLengths)
        {
            double[] result = new double[faces.Count];
            faces.UpdateFaceDistances(sources, edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        public static void UpdateFaceDistances(this HeFaceList faces, IEnumerable<HeFace> sources, IList<double> edgeLengths, IList<double> result)
        {
            // TODO switch to pq implementation

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
        public static Vec3d[] GetFaceBarycenters(this HeFaceList faces, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[faces.Count];
            faces.UpdateFaceBarycenters(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceBarycenters(this HeFaceList faces, IList<Vec3d> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    UpdateFaceBarycenters(faces, result, range.Item1, range.Item2));
            else
                UpdateFaceBarycenters(faces, result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceBarycenters(this HeFaceList faces, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;
                result[i] = f.GetBarycenter();
            }
        }


        /// <summary>
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetFaceCircumcenters(this HeFaceList faces, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[faces.Count];
            faces.UpdateFaceCircumcenters(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceCircumcenters(this HeFaceList faces, IList<Vec3d> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    UpdateFaceCircumcenters(faces, result, range.Item1, range.Item2));
            else
                UpdateFaceCircumcenters(faces, result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceCircumcenters(this HeFaceList faces, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;
                result[i] = f.GetCircumcenter();
            }
        }






        /// <summary>
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetFaceIncenters(this HeFaceList faces, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[faces.Count];
            faces.UpdateFaceIncenters(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceIncenters(this HeFaceList faces, IList<Vec3d> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    UpdateFaceIncenters(faces, result, range.Item1, range.Item2));
            else
                UpdateFaceIncenters(faces, result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceIncenters(this HeFaceList faces, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;
                result[i] = f.GetIncenter();
            }
        }




        /// <summary>
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Vec3d[] GetFaceIncenters(this HeFaceList faces, IList<double> edgeLengths, bool parallel = false)
        {
            Vec3d[] result = new Vec3d[faces.Count];
            faces.UpdateFaceIncenters(edgeLengths, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceIncenters(this HeFaceList faces, IList<double> edgeLengths, IList<Vec3d> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    UpdateFaceIncenters(faces, edgeLengths, result, range.Item1, range.Item2));
            else
                UpdateFaceIncenters(faces, edgeLengths, result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceIncenters(this HeFaceList faces, IList<double> edgeLengths, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
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

                Vec3d v = f.First.GetNormal();
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
    }
}
