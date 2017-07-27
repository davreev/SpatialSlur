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
    [Serializable]
    public class OnMesh : OnTarget<Mesh>
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
            return mesh.ClosestPoint(point.ToPoint3d()).ToVec3d();
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="parallel"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnMesh(Mesh mesh, bool parallel, int capacity, double weight = 1.0)
            :base(mesh, ClosestPoint, parallel, capacity, weight)
        {
        }
    }
}
