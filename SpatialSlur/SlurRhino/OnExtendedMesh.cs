#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using Rhino.Geometry;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnExtendedMesh : OnTarget<Mesh>
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
            var mp = mesh.ClosestMeshPoint(point, 0.0);

            Vec3d cp = mesh.PointAt(mp);
            Vec3d cn = mesh.NormalAt(mp);
            return point + Vec3d.Project(cp - point, cn);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="parallel"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnExtendedMesh(Mesh mesh, bool parallel, double weight = 1.0, int capacity = DefaultCapacity)
            :base(mesh, ClosestPoint, parallel, weight, capacity)
        {
        }
    }
}

#endif