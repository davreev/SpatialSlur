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
            int currTag = hedges.NextTag;

            for (int i = 0; i < hedges.Count; i += 2)
            {
                Halfedge he = hedges[i];
                if (he.IsUnused || he.Tag == currTag) continue; // skip if unused or already visited

                // set label and flag as visited
                result[he.Index >> 1] = 0;
                he.Tag = he.Twin.Tag = currTag;

                // add to stack
                stack.Push(he);
                UpdateEdgeLabels(stack, currTag, result);
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
            int currTag = hedges.NextTag;
       
            // set label and flag as visited
            result[start.Index >> 1] = 0;
            start.Tag = start.Twin.Tag = currTag;

            // add to stack
            stack.Push(start);
            UpdateEdgeLabels(stack, currTag, result);
        }


        /// <summary>
        /// Assumes the result array contains default values.
        /// </summary>
        private static void UpdateEdgeLabels(Stack<Halfedge> stack, int currTag, IList<int> result)
        {
            while (stack.Count > 0)
            {
                Halfedge he0 = stack.Pop();
                Halfedge he1 = he0.Twin.Next;
                int id = result[he0.Index >> 1] + 1;

                do
                {
                    // set result and add to stack if not yet visited
                    if (he1.Tag != currTag && he1.Face != null)
                    {
                        result[he1.Index >> 1] = id & 1; // set result

                        Halfedge he2 = he1.Twin;
                        he1.Tag = he2.Tag = currTag;
                        stack.Push(he2);
                    }

                    he1 = he1.Twin.Next;
                    id++;
                } while (he1 != he0);
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
        /// Dihedral angle is in range [0-Tau] where 0 is max convex and Tau is max concave.
        /// Assumes the given face normals are unitized.
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
