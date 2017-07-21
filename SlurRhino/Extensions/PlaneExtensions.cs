using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class PlaneExtensions
    {
        /// <summary>
        /// Returns the transform matrix given by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform ToTransform(this Plane plane)
        {
            Point3d o = plane.Origin;
            Vector3d x = plane.XAxis;
            Vector3d y = plane.YAxis;
            Vector3d z = plane.ZAxis;

            Transform m = new Transform();

            m[0, 0] = x.X;
            m[0, 1] = y.X;
            m[0, 2] = z.X;
            m[0, 3] = o.X;

            m[1, 0] = x.Y;
            m[1, 1] = y.Y;
            m[1, 2] = z.Y;
            m[1, 3] = o.Y;

            m[2, 0] = x.Z;
            m[2, 1] = y.Z;
            m[2, 2] = z.Z;
            m[2, 3] = o.Z;

            m[3, 3] = 1.0;

            return m;
        }


        /// <summary>
        /// Returns the inverse transformation matrix given by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform ToInverseTransform(this Plane plane)
        {
            Vector3d d = new Vector3d(plane.Origin);
            Vector3d x = plane.XAxis;
            Vector3d y = plane.YAxis;
            Vector3d z = plane.ZAxis;

            Transform m = new Transform();

            m[0, 0] = x.X;
            m[0, 1] = x.Y;
            m[0, 2] = x.Z;
            m[0, 3] = -(d * x);

            m[1, 0] = y.X;
            m[1, 1] = y.Y;
            m[1, 2] = y.Z;
            m[1, 3] = -(d * y);

            m[2, 0] = z.X;
            m[2, 1] = z.Y;
            m[2, 2] = z.Z;
            m[2, 3] = -(d * z);

            m[3, 3] = 1.0;

            return m;
        }


        /// <summary>
        /// Returns the transform matrix given by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        [Obsolete("")]
        public static Transform ToWorld(this Plane plane)
        {
            return ToTransform(plane);
        }


        /// <summary>
        /// Returns the inverse transformation matrix given by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        [Obsolete("")]
        public static Transform ToLocal(this Plane plane)
        {
            return ToInverseTransform(plane);
        }
    }
}
