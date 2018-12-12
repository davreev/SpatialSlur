
/*
 * Notes
 * 
 * TODO 
 * Make IInfluences hold ParticleHandles directly
 */ 

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInfluence
    {
        /// <summary>
        /// Sets any parameters that require the initial state of particles. 
        /// </summary>
        /// <param name="particles"></param>
        void Initialize(ParticleBuffer particles);


        /// <summary>
        /// Calculates all deltas applied by this influence.
        /// </summary>
        /// <param name="particles"></param>
        void Calculate(ParticleBuffer particles);
    }
}
