
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
    public static partial class Orient3dExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        /// <returns></returns>
        public static Plane ToPlane(this Orient3d orient)
        {
            return new Plane(
                orient.Translation,
                orient.Rotation.X,
                (Vector3d)orient.Rotation.Y
                );
        }
    }
}

#endif