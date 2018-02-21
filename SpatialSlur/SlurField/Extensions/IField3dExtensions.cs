using System.Collections.Generic;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class IField3dExtensions
    {
        #region IField2d<Vec3d>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="stepSize"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IEnumerable<Vec3d> IntegrateFrom(this IField3d<Vec3d> field, Vec3d point, double stepSize, IntegrationMode mode = IntegrationMode.Euler)
        {
            return SimulationUtil.IntegrateFrom(field, point, stepSize, mode);
        }

        #endregion
    }
}
