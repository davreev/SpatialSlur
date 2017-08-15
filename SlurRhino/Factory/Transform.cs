using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{
    using T = Rhino.Geometry.Transform;

    /// <summary>
    /// 
    /// </summary>
    public static partial class RhinoFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Transform
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="plane"></param>
            public static T CreateFromPlane(Plane plane)
            {
                return Create(plane.Origin.ToVec3d(), plane.XAxis.ToVec3d(), plane.YAxis.ToVec3d(), plane.ZAxis.ToVec3d());
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="plane"></param>
            public static T CreateInverseFromPlane(Plane plane)
            {
                return CreateInverse(plane.Origin.ToVec3d(), plane.XAxis.ToVec3d(), plane.YAxis.ToVec3d(), plane.ZAxis.ToVec3d());
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="p0"></param>
            /// <param name="p1"></param>
            /// <param name="p2"></param>
            /// <returns></returns>
            public static T CreateFrom3Points(Vec3d p0, Vec3d p1, Vec3d p2)
            {
                return Create(p0, p1 - p0, p2 - p0);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="p0"></param>
            /// <param name="p1"></param>
            /// <param name="p2"></param>
            /// <returns></returns>
            public static T CreateInverseFrom3Points(Vec3d p0, Vec3d p1, Vec3d p2)
            {
                return CreateInverse(p0, p1 - p0, p2 - p0);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static T Create(Vec3d origin, Vec3d x, Vec3d y)
            {
                if (GeometryUtil.OrthoNormalize(ref x, ref y, out Vec3d z))
                    return Create(origin, x, y, z);

                return T.Unset;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static T CreateInverse(Vec3d origin, Vec3d x, Vec3d y)
            {
                if (GeometryUtil.OrthoNormalize(ref x, ref y, out Vec3d z))
                    return CreateInverse(origin, x, y, z);

                return T.Unset;
            }


            /// <summary>
            /// Assumes the given axes are orthonormal.
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            private static T Create(Vec3d origin, Vec3d x, Vec3d y, Vec3d z)
            {
                T m = new T();

                m[0, 0] = x.X;
                m[0, 1] = x.Y;
                m[0, 2] = x.Z;
                m[0, 3] = -(origin * x);

                m[1, 0] = y.X;
                m[1, 1] = y.Y;
                m[1, 2] = y.Z;
                m[1, 3] = -(origin * y);

                m[2, 0] = z.X;
                m[2, 1] = z.Y;
                m[2, 2] = z.Z;
                m[2, 3] = -(origin * z);

                m[3, 3] = 1.0;

                return m;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="origin"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            private static T CreateInverse(Vec3d origin, Vec3d x, Vec3d y, Vec3d z)
            {
                T m = new T();

                m[0, 0] = x.X;
                m[0, 1] = y.X;
                m[0, 2] = z.X;
                m[0, 3] = origin.X;

                m[1, 0] = x.Y;
                m[1, 1] = y.Y;
                m[1, 2] = z.Y;
                m[1, 3] = origin.Y;

                m[2, 0] = x.Z;
                m[2, 1] = y.Z;
                m[2, 2] = z.Z;
                m[2, 3] = origin.Z;

                m[3, 3] = 1.0;

                return m;
            }
        }
    }
}