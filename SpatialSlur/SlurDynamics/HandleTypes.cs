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
    public class ParticleHandle
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
        public int Index = -1;
        /// <summary></summary>
        public Vec3d Delta;
        /// <summary></summary>
        public double Weight;


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
    }


    /// <summary>
    /// 
    /// </summary>
    public class BodyHandle : ParticleHandle
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
    }
}
