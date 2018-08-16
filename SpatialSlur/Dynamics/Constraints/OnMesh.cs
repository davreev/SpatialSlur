
/*
 * Notes
 */

#if USING_RHINO

using System;
using Rhino.Geometry;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnMesh : OnTarget<Mesh>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mesh"></param>
        /// <param name="weight"></param>
        public OnMesh(int index, Mesh mesh, double weight = 1.0)
            :base(index, mesh, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override Vector3d GetClosestPoint(Vector3d point)
        {
            return Target.ClosestPoint(point);
        }
    }
}

#endif