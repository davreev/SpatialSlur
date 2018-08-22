
/*
 * Notes
 */

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public class TriMesh2d : TriMesh<Vector2d>
    {
        /// <summary>
        /// 
        /// </summary>
        public TriMesh2d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="positionCapacity"></param>
        /// <param name="normalCapacity"></param>
        /// <param name="faceCapacity"></param>
        public TriMesh2d(int positionCapacity, int normalCapacity, int faceCapacity)
            : base(positionCapacity, normalCapacity, faceCapacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public TriMesh2d(TriMesh2d other)
            : base(other)
        {
        }
    }
}
