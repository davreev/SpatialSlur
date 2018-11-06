
/*
 * Notes
 */

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public class TriMesh3f : TriMesh<Vector3f, Vector2f>
    {
        /// <summary>
        /// 
        /// </summary>
        public TriMesh3f()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCount"></param>
        /// <param name="faceCount"></param>
        public TriMesh3f(int vertexCount, int faceCount)
            : base(vertexCount, faceCount)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public TriMesh3f(TriMesh3f other)
            : base(other)
        {
        }
    }
}
