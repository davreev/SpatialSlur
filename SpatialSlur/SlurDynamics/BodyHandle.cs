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
    public class BodyHandle : ParticleHandle, IHandle
    {
        /// <summary></summary>
        public Vec3d AngleDelta;
        /// <summary></summary>
        public double AngleWeight;


        /// <summary>
        /// 
        /// </summary>
        public BodyHandle()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public BodyHandle(int index)
            :base(index)
        {
        }


        #region Explicit interface implementations

        /// <summary></summary>
        Vec3d IHandle.AngleDelta
        {
            get { return AngleDelta; }
        }


        /// <summary>
        /// 
        /// </summary>
        double IHandle.AngleWeight
        {
            get { return AngleWeight; }
        }

        #endregion
    }
}
