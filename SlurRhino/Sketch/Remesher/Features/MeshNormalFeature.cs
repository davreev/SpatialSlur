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
    [Serializable]
    public class MeshNormalFeature : IFeature
    {
        private Mesh _mesh;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshNormalFeature(Mesh mesh)
        {
            _mesh = mesh;
            _mesh.Normals.ComputeNormals();
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ClosestPoint(Vec3d point)
        {
            var mp = _mesh.ClosestMeshPoint(point.ToPoint3d(), 0.0);
            var cp = _mesh.PointAt(mp).ToVec3d();
            var cn = _mesh.NormalAt(mp).ToVec3d();
            return point + Vec3d.Project(cp - point, cn);
        }
    }
}
