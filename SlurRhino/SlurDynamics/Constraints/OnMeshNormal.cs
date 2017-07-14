using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurDynamics;

using Rhino.Geometry;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    public class OnMeshNormal : OnTarget<Mesh>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Vec3d ClosestPoint(Mesh mesh, Vec3d point)
        {
            var mp = mesh.ClosestMeshPoint(point.ToPoint3d(), 0.0);
            var cp = mesh.PointAt(mp).ToVec3d();
            var cn = mesh.NormalAt(mp).ToVec3d();
            return point + Vec3d.Project(cp - point, cn);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnMeshNormal(Mesh mesh, int capacity, double weight = 1.0)
            :base(mesh, ClosestPoint, capacity, weight)
        {
        }
    }
}
