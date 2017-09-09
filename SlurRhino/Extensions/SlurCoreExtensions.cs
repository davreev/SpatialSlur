using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using Rhino.Geometry;

/*
 * Notes 
 */ 

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// Extension methods for classes in the SpatialSlur.SlurCore namespace
    /// </summary>
    public static class SlurCoreExtensions
    {
        #region Vec2d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point2d ToPoint2d(this Vec2d vector)
        {
            return new Point2d(vector.X, vector.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point2f ToPoint2f(this Vec2d vector)
        {
            return new Point2f((float)vector.X, (float)vector.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2d ToVector2d(this Vec2d vector)
        {
            return new Vector2d(vector.X, vector.Y);
        }

        #endregion


        #region Vec3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point3d ToPoint3d(this Vec3d vector)
        {
            return new Point3d(vector.X, vector.Y, vector.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point3f ToPoint3f(this Vec3d vector)
        {
            return new Point3f((float)vector.X, (float)vector.Y, (float)vector.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3d ToVector3d(this Vec3d vector)
        {
            return new Vector3d(vector.X, vector.Y, vector.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3f ToVector3f(this Vec3d vector)
        {
            return new Vector3f((float)vector.X, (float)vector.Y, (float)vector.Z);
        }

        #endregion


        #region Interval

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Interval ToInterval(this Interval1d interval)
        {
            return new Interval(interval.A, interval.B);
        }

        #endregion


        #region Interval2d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Interval2d interval)
        {
            Interval1d x = interval.X;
            Interval1d y = interval.Y;
            return new BoundingBox(x.A, y.A, 0.0, x.B, y.B, 0.0);
        }

        #endregion


        #region Interval3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Interval3d interval)
        {
            Interval1d x = interval.X;
            Interval1d y = interval.Y;
            Interval1d z = interval.Z;
            return new BoundingBox(x.A, y.A, z.A, x.B, y.B, z.B);
        }

        #endregion


        #region Orient3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        /// <returns></returns>
        public static Plane ToPlane(this Orient3d orient)
        {
            return new Plane(
                orient.Translation.ToPoint3d(),
                orient.Rotation.X.ToVector3d(),
                orient.Rotation.Y.ToVector3d()
                );
        }

        #endregion


        #region Mat4d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Transform ToTransform(Matrix4d matrix)
        {
            var m = new Transform();

            m.M00 = matrix.M00;
            m.M01 = matrix.M01;
            m.M02 = matrix.M02;
            m.M03 = matrix.M03;

            m.M10 = matrix.M10;
            m.M11 = matrix.M11;
            m.M12 = matrix.M12;
            m.M13 = matrix.M13;

            m.M20 = matrix.M20;
            m.M21 = matrix.M21;
            m.M22 = matrix.M22;
            m.M23 = matrix.M23;

            m.M30 = matrix.M30;
            m.M31 = matrix.M31;
            m.M32 = matrix.M32;
            m.M33 = matrix.M33;

            return m;
        }

        #endregion
    }
}
