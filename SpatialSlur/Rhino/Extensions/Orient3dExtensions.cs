
/*
 * Notes 
 */

#if USING_RHINO

using Rhino.Geometry;
using SpatialSlur;

using Vec3d = Rhino.Geometry.Vector3d;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Orient3dExtensions
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
                (Vec3d)orient.Rotation.Y
                );
        }
    }
}

#endif