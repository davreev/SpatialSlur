/*
 * Notes
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
        /// <param name="positions"></param>
        /// <param name="rotations"></param>
        void Initialize(
            ArrayView<ParticlePosition> positions, 
            ArrayView<ParticleRotation> rotations);


        /// <summary>
        /// Calculates all deltas applied by this influence.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="rotations"></param>
        void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations);
    }
}
