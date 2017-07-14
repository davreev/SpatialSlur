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
    public class PositionHandle
    {
        #region Static

        /// <summary>
        /// Implicitly converts a handle to its index for convenience.
        /// </summary>
        /// <param name="handle"></param>
        public static implicit operator int(PositionHandle handle)
        {
            return handle.Index;
        }

        #endregion


        /// <summary></summary>
        public int Index = -1;

        /// <summary></summary>
        public Vec3d Delta;
    }


    /// <summary>
    /// 
    /// </summary>
    public class ParticleHandle : PositionHandle
    {
        /// <summary></summary>
        public Vec3d AngleDelta;
    }
}
