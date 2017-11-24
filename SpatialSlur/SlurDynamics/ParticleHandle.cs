using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return handle.Index;
        }

        #endregion


        /// <summary></summary>
        public Vec3d Delta;

        /// <summary></summary>
        public int Index = -1;


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


        #region Explicit interface implementations

        /// <summary></summary>
        Vec3d IHandle.Delta
        {
            get { return Delta; }
        }


        /// <summary></summary>
        Vec3d IHandle.AngleDelta
        {
            get { return Vec3d.Zero; }
        }


        /// <summary></summary>
        int IHandle.Index
        {
            get { return Index; }
            set { Index = value; }
        }

        #endregion
    }
}
