
/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public enum TriangulationMode
    {
        /// <summary>Adds tris radially around the first vertex in the face</summary>
        Fan,
        /// <summary>Adds tris in an alternating manner from the first vertex in the face</summary>
        Strip,
        /// <summary>Adds tris radially around a new central vertex</summary>
        Poke
    }
}
