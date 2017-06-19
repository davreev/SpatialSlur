using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Interface for a 6dof rigid body
    /// </summary>
    public interface IRigidBody : IParticle
    {
        /// <summary>
        /// 
        /// </summary>
        Rotation3d Rotation { get; set; }


        /// <summary>
        /// 
        /// </summary>
        Vec3d AngularVelocity { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        void ApplyTorque(Vec3d delta, double weight);
    }
}
