using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Interface for a 3dof particle
    /// </summary>
    public interface IParticle : IUpdatable
    {
        /// <summary>
        /// 
        /// </summary>
        Vec3d Position { get; set; }


        /// <summary>
        /// 
        /// </summary>
        Vec3d Velocity { get; set; }


        /// <summary>
        /// 
        /// </summary>
        double Mass { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        void ApplyForce(Vec3d delta, double weight);
    }
}