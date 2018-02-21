#if USING_RHINO

using System;

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
            return _mesh.ClosestPoint(point);
        }
    }
}

#endif