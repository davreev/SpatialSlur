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
    public class BodyHandle : ParticleHandle, IHandle
    {
        private Vec3d _angleDelta;
        

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


        /// <summary>
        /// 
        /// </summary>
        public Vec3d AngleDelta
        {
            get => _angleDelta;
            set => _angleDelta = value;
        }
    }
}
