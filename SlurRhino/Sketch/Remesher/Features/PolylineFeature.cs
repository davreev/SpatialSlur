using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurRhino;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino.Remesher
{
    /// <summary>
    /// 
    /// </summary>
    public class PolylineFeature : IFeature
    {
        private Mesh _mesh;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="vertexPositions"></param>
        public PolylineFeature(IEnumerable<Point3d> points, bool close = false)
        {
            _mesh = new Mesh();

            var verts = _mesh.Vertices;
            var faces = _mesh.Faces;

            foreach (var p in points)
            {
                verts.Add(p);
                verts.Add(p);
            }

            int nv = verts.Count;

            for (int i = 0; i < nv - 2; i++)
                faces.AddFace(i, i + 1, i + 3, i + 2);

            if (close)
                faces.AddFace(nv - 2, nv - 1, 1, 0); // last face
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
