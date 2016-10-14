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
    public partial class HalfedgeList
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] GetEdgeLabels()
        {
            int[] result = new int[Count >> 1];
            GetEdgeLabels(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetEdgeLabels(IList<int> result)
        {
            Stack<Halfedge> stack = new Stack<Halfedge>();

            for (int i = 0; i < Count; i += 2)
            {
                Halfedge he = this[i];
                if (he.IsUnused || result[i >> 1] != 0) continue; // skip if unused or already visited

                result[he.Index >> 1] = 1;
                stack.Push((he.Face == null) ? he.Twin : he);
                GetEdgeLabels(stack, result);
            }
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public int[] GetEdgeLabels(Halfedge start)
        {
            int[] result = new int[Count >> 1];
            GetEdgeLabels(start, result);
            return result;
        }
    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="result"></param>
        public void GetEdgeLabels(Halfedge start, IList<int> result)
        {
            start.UsedCheck();
            OwnsCheck(start);
            HalfSizeCheck(result);

            Stack<Halfedge> stack = new Stack<Halfedge>();

            result[start.Index >> 1] = 1;
            stack.Push((start.Face == null) ? start.Twin : start);
            GetEdgeLabels(stack, result);
        }


        /// <summary>
        /// Assumes the result array contains default values.
        /// </summary>
        private static void GetEdgeLabels(Stack<Halfedge> stack, IList<int> result)
        {
            while (stack.Count > 0)
            {
                Halfedge he0 = stack.Pop();
                int label = -result[he0.Index >> 1];

                // circulate face
                Halfedge he1 = he0.Next;
                while (he1 != he0)
                {
                    int index = he1.Index >> 1;

                    // bypass if already visited
                    if (result[index] == 0)
                    {
                        // set label
                        result[index] = label;

                        // add twin to stack if not on boundary
                        Halfedge he2 = he1.Twin;
                        if (he2.Face != null) stack.Push(he2);
                    }

                    he1 = he1.Next;
                    label = -label;
                }
            }
        }


        /// <summary>
        /// Returns the length of each edge in the mesh.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetEdgeLengths(bool parallel = false)
        {
            double[] result = new double[Count >> 1];
            GetEdgeLengths(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetEdgeLengths(IList<double> result, bool parallel = false)
        {
            HalfSizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
                    GetEdgeLengths(result, range.Item1, range.Item2));
            else
                GetEdgeLengths(result, 0, Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetEdgeLengths(IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = this[i << 1];
                if (he.IsUnused) continue;
                result[i] = he.Span.Length;
            }
        }


        /// <summary>
        /// Returns the dihedral angle for each pair of halfedges in the mesh.
        /// Dihedral angle is in range [0-Tau] where 0 is max convex and Tau is max concave.
        /// Assumes the given face normals are unitized.
        /// </summary>
        /// <param name="faceNormals"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetDihedralAngles(IList<Vec3d> faceNormals, bool parallel = false)
        {
            double[] result = new double[Count >> 1];
            GetDihedralAngles(faceNormals, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceNormals"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetDihedralAngles(IList<Vec3d> faceNormals, IList<double> result, bool parallel = false)
        {
            HalfSizeCheck(result);
            Mesh.Faces.SizeCheck(faceNormals);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count >> 1), range =>
                    GetDihedralAngles(faceNormals, result, range.Item1, range.Item2));
            else
                GetDihedralAngles(faceNormals, result, 0, Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetDihedralAngles(IList<Vec3d> faceNormals, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge he = this[i << 1];
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
