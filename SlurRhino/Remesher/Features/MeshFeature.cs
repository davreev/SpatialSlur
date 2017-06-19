using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurRhino;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino.Remesher
{
    /// <summary>
    /// 
    /// </summary>
    public class MeshFeature : IFeature
    {
        private Mesh _mesh;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public MeshFeature(Mesh mesh)
        {
            _mesh = mesh;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ClosestPoint(Vec3d point)
        {
            return _mesh.ClosestPoint(point.ToPoint3d()).ToVec3d();
        }
    }
}
