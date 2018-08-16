
/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Linq;
using System.Collections.Generic;
using Rhino.Geometry;
using SpatialSlur;

namespace SpatialSlur.Tools
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
        public Vector3d ClosestPoint(Vector3d point)
        {
            return _mesh.ClosestPoint(point);
        }
    }
}

#endif