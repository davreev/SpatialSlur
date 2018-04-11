
/*
 * Notes
 */

#if USING_RHINO

using System;
using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurTools
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ExtendedMeshFeature : ISurfaceFeature
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


        /// <inheritdoc />
        public int Rank
        {
            get { return 2; }
        }


        /// <inheritdoc />
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
