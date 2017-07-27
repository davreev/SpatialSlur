using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Utility class for related constants and static methods.
    /// </summary>
    public static class FieldUtil
    {
        /// <summary>
        /// 
        /// </summary>
        public static double ValueAt(double[] values, int[] indices, double[] weights)
        {
            double sum = 0.0;

            for (int i = 0; i < indices.Length; i++)
                sum += values[indices[i]] * weights[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d ValueAt(Vec2d[] values, int[] indices, double[] weights)
        {
            Vec2d sum = new Vec2d();

            for (int i = 0; i < indices.Length; i++)
                sum += values[indices[i]] * weights[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d ValueAt(Vec3d[] values, int[] indices, double[] weights)
        {
            Vec3d sum = new Vec3d();

            for (int i = 0; i < indices.Length; i++)
                sum += values[indices[i]] * weights[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetAt(double[] values, int[] indices, double[] weights, double value)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                int j = indices[i];
                values[j] += (value - values[j]) * weights[i];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetAt(Vec2d[] values, int[] indices, double[] weights, Vec2d value)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                int j = indices[i];
                values[j] += (value - values[j]) * weights[i];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetAt(Vec3d[] values, int[] indices, double[] weights, Vec3d value)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                int j = indices[i];
                values[j] += (value - values[j]) * weights[i];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void IncrementAt(double[] values, int[] indices, double[] weights, double amount)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += amount * weights[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void IncrementAt(Vec2d[] values, int[] indices, double[] weights, Vec2d amount)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += amount * weights[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void IncrementAt(Vec3d[] values, int[] indices, double[] weights, Vec3d amount)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += amount * weights[i];
        }
    }
}
