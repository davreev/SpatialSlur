
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
    public class OnExtendedMesh : OnTarget<Mesh>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mesh"></param>
        /// <param name="weight"></param>
        public OnExtendedMesh(int index, Mesh mesh, double weight = 1.0)
            : base(index, mesh, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override Vector3d GetClosestPoint(Vector3d point)
        {
            var mesh = Target;
            var mp = mesh.ClosestMeshPoint(point, 0.0);

            Vector3d cp = mesh.PointAt(mp);
            Vector3d cn = mesh.NormalAt(mp);
            return point + Vector3d.Project(cp - point, cn);
        }
    }
}

#endif