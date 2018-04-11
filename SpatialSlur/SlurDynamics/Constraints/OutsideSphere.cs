using System;
using System.Collections.Generic;

using SpatialSlur.SlurCore;

/*
 * Notes 
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    public class OutsideSphere: Constraint, IConstraint
    {
        private H _handle = new H();
        private Vec3d _origin;
        private double _radius;
        private bool _apply;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="strength"></param>
        public OutsideSphere(int index, Vec3d origin, double radius, double weight = 1.0)
        {
            _handle.Index = index;
            _origin = origin;
            Radius = radius;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle
        {
            get { return _handle; }
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
        public double Radius
        {
            get { return _radius; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value must be positive.");

                _radius = value;
            }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            var d = _origin - bodies[_handle].Position;
            var m = d.SquareLength;

            if (m >= _radius * _radius)
            {
                _apply = false;
                return;
            }

            _handle.Delta = d * (1.0 - _radius / Math.Sqrt(m));
            _apply = true;
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            if (_apply)
                bodies[_handle].ApplyMove(_handle.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { yield return _handle; }
        }

        #endregion
    }
}
