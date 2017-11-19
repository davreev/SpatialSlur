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

namespace SpatialSlur.SlurTools.Features
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ExtendedMeshFeature : IFeature
    {
        private Mesh _mesh;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public ExtendedMeshFeature(Mesh mesh)
        {
            _mesh = mesh;
            _mesh.Normals.ComputeNormals();
        }


        /// <summary>
        /// 
        /// </summary>
        public int Rank
        {
            get { return 2; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ClosestPoint(Vec3d point)
        {
            var mp = _mesh.ClosestMeshPoint(point, 0.0);

            Vec3d cp = _mesh.PointAt(mp);
            Vec3d cn = _mesh.NormalAt(mp);
            return point + Vec3d.Project(cp - point, cn);
        }
    }
}

#endif
