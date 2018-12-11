
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
        /// <param name="particles"></param>
        void Initialize(ReadOnlyArrayView<Particle> particles);


        /// <summary>
        /// Calculates all deltas applied by this influence.
        /// </summary>
        /// <param name="particles"></param>
        void Calculate(ReadOnlyArrayView<Particle> particles);


        /// <summary>
        /// Applies calculated deltas to the affected particles.
        /// </summary>
        /// <param name="particles"></param>
        void Apply(ReadOnlyArrayView<Particle> particles);
    }
}
