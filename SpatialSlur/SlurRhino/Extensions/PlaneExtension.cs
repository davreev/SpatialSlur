
/*
 * Notes
 */

#if USING_RHINO


using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class PlaneExtension
    {
        /// <summary>
        /// Returns the transformation matrix defined by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform ToTransform(this Plane plane)
        {
            return RhinoFactory.Transform.CreateFromPlane(plane);
        }


        /// <summary>
        /// Returns the inverse of the transformation matrix defined by this plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Transform ToTransformInverse(this Plane plane)
        {
            return RhinoFactory.Transform.CreateInverseFromPlane(plane);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Orient3d ToOrient3d(this Plane plane)
        {
            return new Orient3d(plane.Origin, plane.XAxis, plane.YAxis);
        }
    }
}

#endif
