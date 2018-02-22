using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnCircle : Constraint, IConstraint
    {
        private H _handle = new H();
        private Vec3d _origin;
        private Vec3d _normal;
        private double _radius;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnCircle(int index, Vec3d origin, Vec3d normal, double radius, double weight = 1.0)
        {
            _handle.Index = index;
            _origin = origin;
            _normal = normal;
            Radius = radius;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle
        {
            get { return _handle; }
            set { _handle = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _radius = value;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            Vec3d p = bodies[_handle].Position;

            Vec3d d = Vec3d.Reject(p - _origin, _normal);
            d *= _radius / d.Length;

            _handle.Delta = ((_origin + d) - p);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_handle].ApplyMove(_handle.Delta, Weight);
        }


        #region Explicit interface implementations
        
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { yield return _handle; }
        }

        #endregion
    }
}
