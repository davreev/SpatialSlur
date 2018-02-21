using System;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ParticleHandle : IHandle
    {
        #region Static

        /// <summary>
        /// Implicitly converts a handle to its index for convenience.
        /// </summary>
        /// <param name="handle"></param>
        public static implicit operator int(ParticleHandle handle)
        {
            return handle._index;
        }

        #endregion


        private Vec3d _delta;
        private int _index = -1;


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public ParticleHandle(int index)
        {
            Index = index;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Delta { get => _delta; set => _delta = value; }


        /// <summary>
        /// 
        /// </summary>
        public int Index { get => _index; set => _index = value; }


        #region Explicit interface implementations

        /// <summary></summary>
        Vec3d IHandle.AngleDelta
        {
            get { return Vec3d.Zero; }
        }

        #endregion
    }
}
