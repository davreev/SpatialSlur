
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public readonly struct Body
    {
        /// <summary></summary>
        public readonly BodyPosition Position;
        
        /// <summary></summary>
        public readonly BodyRotation Rotation;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public Body(Vector3d position)
        {
            Position = new BodyPosition(position);
            Rotation = null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Body(Vector3d position, Quaterniond rotation)
        {
            Position = new BodyPosition(position);
            Rotation = new BodyRotation(rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Body(BodyPosition position, BodyRotation rotation)
        {
            Position = position;
            Rotation = rotation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private Body(Body other)
        {
            Position = other.Position?.Duplicate();
            Rotation = other.Rotation?.Duplicate();
        }


        /// <summary>
        /// Returns a deep copy of this body.
        /// </summary>
        /// <returns></returns>
        public Body Duplicate()
        {
            return new Body(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void Deconstruct(out BodyPosition position, out BodyRotation rotation)
        {
            position = Position;
            rotation = Rotation;
        }
    }
}
