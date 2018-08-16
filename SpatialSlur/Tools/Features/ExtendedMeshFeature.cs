
/*
 * Notes
 */

#if USING_RHINO

using System;
using Rhino.Geometry;
using SpatialSlur;

namespace SpatialSlur.Tools
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
        public Vector3d ClosestPoint(Vector3d point)
        {
            var mp = _mesh.ClosestMeshPoint(point, 0.0);

            Vector3d cp = _mesh.PointAt(mp);
            Vector3d cn = _mesh.NormalAt(mp);
            return point + Vector3d.Project(cp - point, cn);
        }
    }
}

#endif
