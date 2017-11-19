using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using UnityEngine;

using static SpatialSlur.SlurCore.CoreUtil;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurUnity
{
    /// <summary>
    /// 
    /// </summary>
    public static class EngineExtensions
    {
        #region Matrix4x4

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            return new Vector3(
                new Vector3(matrix.m00, matrix.m10, matrix.m20).magnitude,
                new Vector3(matrix.m01, matrix.m11, matrix.m21).magnitude,
                new Vector3(matrix.m02, matrix.m12, matrix.m22).magnitude
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(
                new Vector3(matrix.m02, matrix.m12, matrix.m22), 
                new Vector3(matrix.m01, matrix.m11, matrix.m21)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3 ExtractTranslation(this Matrix4x4 matrix)
        {
            return new Vector3(matrix.m03, matrix.m13, matrix.m23);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public static Matrix4x4 SwapYZ(this Matrix4x4 matrix)
        {
            Swap(ref matrix.m01, ref matrix.m02);
            Swap(ref matrix.m10, ref matrix.m20);
            Swap(ref matrix.m11, ref matrix.m22);

            Swap(ref matrix.m12, ref matrix.m21);
            Swap(ref matrix.m13, ref matrix.m23);
            Swap(ref matrix.m31, ref matrix.m32);

            return matrix;
        }

        #endregion
    }
}
