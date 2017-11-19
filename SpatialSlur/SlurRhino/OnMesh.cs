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
            return mesh.ClosestPoint(point);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="parallel"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnMesh(Mesh mesh, bool parallel, double weight = 1.0, int capacity = DefaultCapacity)
            :base(mesh, ClosestPoint, parallel, weight, capacity)
        {
        }
    }
}

#endif