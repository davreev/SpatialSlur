
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
    public class MeshFeature : ISurfaceFeature
    {
        private Mesh _mesh;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshFeature(Mesh mesh)
        {
            _mesh = mesh;
        }


        /// <inheritdoc />
        public int Rank
        {
            get { return 2; }
        }


        /// <inheritdoc />
        public Vec3d ClosestPoint(Vec3d point)
        {
            return _mesh.ClosestPoint(point);
        }
    }
}

#endif