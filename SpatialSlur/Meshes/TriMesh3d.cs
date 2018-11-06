
/*
 * Notes
 */

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public class TriMesh3d : TriMesh<Vector3d, Vector2d>
    {
        /// <summary>
        /// 
        /// </summary>
        public TriMesh3d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCount"></param>
        /// <param name="faceCount"></param>
        public TriMesh3d(int vertexCount, int faceCount)
            : base(vertexCount, faceCount)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public TriMesh3d(TriMesh3d other)
            : base(other)
        {
        }
    }
}
