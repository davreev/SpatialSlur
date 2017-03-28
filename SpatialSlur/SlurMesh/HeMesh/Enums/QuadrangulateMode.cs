
/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public enum QuadrangulateMode
    {
        /// <summary>Adds quads radially around the first vertex in the face</summary>
        Fan,
        /// <summary>Adds quads in an alternating manner from the first vertex in the face</summary>
        Strip,
        /// <summary>Adds quads radially around a new central vertex</summary>
        Poke
    }
}
