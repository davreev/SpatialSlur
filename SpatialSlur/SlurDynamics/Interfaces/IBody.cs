using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Interface for a dynamic body with position and orientation.
    /// </summary>
    public interface IBody : IUpdatable
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
        Rotation3d Rotation { get; set; }


        /// <summary>
        /// Returns true if the implementation supports rotation.
        /// </summary>
        bool HasRotation { get; }


        /// <summary>
        /// 
        /// </summary>
        Vec3d AngularVelocity { get; set; }


        /// <summary>
        /// 
        /// </summary>
        double Mass { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        void ApplyMove(Vec3d delta, double weight);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        void ApplyRotate(Vec3d delta, double weight);
    }
}