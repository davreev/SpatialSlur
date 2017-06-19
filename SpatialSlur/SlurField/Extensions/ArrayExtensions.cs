using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * 
 */ 

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArrayExtensions
    {
        #region double[]


        /// <summary>
        /// 
        /// </summary>
        internal static double ValueAt(this double[] values, int[] indices, double[] weights)
        {
            double sum = 0.0;

            for (int i = 0; i < indices.Length; i++)
                sum += values[indices[i]] * weights[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        internal static void SetAt(this double[] values, int[] indices, double[] weights, double value)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += (value - values[indices[i]]) * weights[i];
        }


        /// <summary>
        /// 
        /// </summary>
        internal static void IncrementAt(this double[] values, int[] indices, double[] weights, double amount)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += amount * weights[i];
        }


        #endregion


        #region Vec2d[]


        /// <summary>
        /// 
        /// </summary>
        internal static Vec2d ValueAt(this Vec2d[] values, int[] indices, double[] weights)
        {
            Vec2d sum = new Vec2d();

            for (int i = 0; i < indices.Length; i++)
                sum += values[indices[i]] * weights[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        internal static void SetAt(this Vec2d[] values, int[] indices, double[] weights, Vec2d value)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += (value - values[indices[i]]) * weights[i];
        }


        /// <summary>
        /// 
        /// </summary>
        internal static void IncrementAt(this Vec2d[] values, int[] indices, double[] weights, Vec2d amount)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += amount * weights[i];
        }


        #endregion


        #region Vec3d[]


        /// <summary>
        /// 
        /// </summary>
        internal static Vec3d ValueAt(this Vec3d[] values, int[] indices, double[] weights)
        {
            Vec3d sum = new Vec3d();

            for (int i = 0; i < indices.Length; i++)
                sum += values[indices[i]] * weights[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        internal static void SetAt(this Vec3d[] values, int[] indices, double[] weights, Vec3d value)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += (value - values[indices[i]]) * weights[i];
        }


        /// <summary>
        /// 
        /// </summary>
        internal static void IncrementAt(this Vec3d[] values, int[] indices, double[] weights, Vec3d amount)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += amount * weights[i];
        }


        #endregion
    }
}
