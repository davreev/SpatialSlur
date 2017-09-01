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


        #region Domain

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Interval ToInterval(this Domain1d domain)
        {
            return new Interval(domain.T0, domain.T1);
        }

        #endregion


        #region Domain2d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Domain2d domain)
        {
            Domain1d x = domain.X;
            Domain1d y = domain.Y;
            return new BoundingBox(x.T0, y.T0, 0.0, x.T1, y.T1, 0.0);
        }

        #endregion


        #region Domain3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Domain3d domain)
        {
            Domain1d x = domain.X;
            Domain1d y = domain.Y;
            Domain1d z = domain.Z;
            return new BoundingBox(x.T0, y.T0, z.T0, x.T1, y.T1, z.T1);
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
        public static Transform ToTransform(Mat4d matrix)
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
