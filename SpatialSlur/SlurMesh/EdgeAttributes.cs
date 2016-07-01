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
    /// Extension methods for calculating various edge attributes.
    /// </summary>
    public static class EdgeAttributes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <returns></returns>
        public static int[] GetEdgeLabels(this HalfedgeList2 hedges)
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
        public static void UpdateEdgeLabels(this HalfedgeList2 hedges, IList<int> result)
        {
            Stack<Halfedge> stack = new Stack<Halfedge>();

            for (int i = 0; i < hedges.Count; i += 2)
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
        public static int[] GetEdgeLabels(this HalfedgeList2 hedges, Halfedge start)
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
        public static void UpdateEdgeLabels(this HalfedgeList2 hedges, Halfedge start, IList<int> result)
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
        public static double[] GetEdgeLengths(this HalfedgeList2 hedges, bool parallel = false)
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
        public static void UpdateEdgeLengths(this HalfedgeList2 hedges, IList<double> result, bool parallel = false)
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
        private static void UpdateEdgeLengths(this HalfedgeList2 hedges, IList<double> result, int i0, int i1)
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
        /// Dihedral angle is in range [0-Tau] where 0 is max convex and Tau is max concave.
        /// Assumes the given face normals are unitized.
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="faceNormals"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetDihedralAngles(this HalfedgeList2 hedges, IList<Vec3d> faceNormals, bool parallel = false)
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
        public static void UpdateDihedralAngles(this HalfedgeList2 hedges, IList<Vec3d> faceNormals, IList<double> result, bool parallel = false)
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
        private static void UpdateDihedralAngles(this HalfedgeList2 hedges, IList<Vec3d> faceNormals, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i << 1];
                if (he.IsUnused || he.IsBoundary) continue;

                Vec3d n0 = faceNormals[he.Face.Index];
                Vec3d n1 = faceNormals[he.Twin.Face.Index];

                double angle = Math.Acos(SlurMath.Clamp(n0 * n1, -1.0, 1.0)); // clamp dot product to remove noise
                if (n1 * he.Next.Span < 0.0) angle *= -1.0; // negate if convex

                result[i] = angle + Math.PI; 
            }
        }
       

        /*
        /// <summary>
        /// 
        /// </summary>
        private static void UpdateDihedralAngles(this HalfedgeList hedges, IList<Vec3d> faceNormals, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = hedges[i << 1];
                if (he.IsUnused || he.IsBoundary) continue;

                double d = faceNormals[he.Face.Index] * faceNormals[he.Twin.Face.Index];
                result[i] = Math.Acos(SlurMath.Clamp(d, -1.0, 1.0)); // clamp dot product to remove noise
            }
        }
        */
    }
}
