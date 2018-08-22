
/*
 * Notes
 */

#if USING_UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public partial struct Vector2d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vector2d(Vector2 vector)
        {
            return new Vector2d(vector.x, vector.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static explicit operator Vector2(Vector2d vector)
        {
            return new Vector2((float)vector.X, (float)vector.Y);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public partial struct Vector3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vector3d(Vector3 vector)
        {
            return new Vector3f(vector.x, vector.y, vector.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static explicit operator Vector3(Vector3d vector)
        {
            return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    public partial struct Vector3f
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vector3f(Vector3 vector)
        {
            return new Vector3f(vector.x, vector.y, vector.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vector3(Vector3f vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public partial struct Quaterniond
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Quaterniond(Quaternion rotation)
        {
            return new Quaterniond(rotation.x, rotation.y, rotation.z, rotation.w);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static explicit operator Quaternion(Quaterniond rotation)
        {
            return new Quaternion((float)rotation.X, (float)rotation.Y, (float)rotation.Z, (float)rotation.W);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public partial struct Matrix4d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public static implicit operator Matrix4d(Matrix4x4 matrix)
        {
            return new Matrix4d(
                matrix.m00, matrix.m01, matrix.m02, matrix.m03,
                matrix.m10, matrix.m11, matrix.m12, matrix.m13,
                matrix.m20, matrix.m21, matrix.m22, matrix.m23,
                matrix.m30, matrix.m31, matrix.m32, matrix.m33
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public static explicit operator Matrix4x4(Matrix4d matrix)
        {
            var m = Matrix4x4.identity;

            m.m00 = (float)matrix.M00;
            m.m01 = (float)matrix.M01;
            m.m02 = (float)matrix.M02;
            m.m03 = (float)matrix.M03;

            m.m10 = (float)matrix.M10;
            m.m11 = (float)matrix.M11;
            m.m12 = (float)matrix.M12;
            m.m13 = (float)matrix.M13;

            m.m20 = (float)matrix.M20;
            m.m21 = (float)matrix.M21;
            m.m22 = (float)matrix.M22;
            m.m23 = (float)matrix.M23;

            m.m30 = (float)matrix.M30;
            m.m31 = (float)matrix.M31;
            m.m32 = (float)matrix.M32;
            m.m33 = (float)matrix.M33;

            return m;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public partial struct Interval3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        public static implicit operator Interval3d(Bounds bounds)
        {
            return new Interval3d(bounds.min, bounds.max);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public static explicit operator Bounds(Interval3d interval)
        {
            return new Bounds((Vector3)interval.Mid, (Vector3)interval.Delta);
        }
    }
}

#endif