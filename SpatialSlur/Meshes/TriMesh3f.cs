
/*
 * Notes
 */

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public class TriMesh3f : TriMesh<Vector3f>
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
        /// <param name="positionCapacity"></param>
        /// <param name="normalCapacity"></param>
        /// <param name="faceCapacity"></param>
        public TriMesh3f(int positionCapacity, int normalCapacity, int faceCapacity)
            : base(positionCapacity, normalCapacity, faceCapacity)
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
