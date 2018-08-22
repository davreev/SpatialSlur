
/*
 * Notes
 */

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public class TriMesh3d : TriMesh<Vector3d>
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
        /// <param name="positionCapacity"></param>
        /// <param name="normalCapacity"></param>
        /// <param name="faceCapacity"></param>
        public TriMesh3d(int positionCapacity, int normalCapacity, int faceCapacity)
            : base(positionCapacity, normalCapacity, faceCapacity)
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
