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
    public class AbovePlane : Constraint, IConstraint
    {
        private H _handle = new H();
        private Vec3d _origin;
        private Vec3d _normal;
        private bool _apply;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public AbovePlane(int index, Vec3d origin, Vec3d normal, double weight = 1.0)
        {
            _handle.Index = index;
            _origin = origin;
            _normal = normal;
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
        public Vec3d Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            double d = Vec3d.Dot(_origin - bodies[_handle].Position, _normal);

            if (d <= 0.0)
            {
                _apply = false;
                return;
            }

            _handle.Delta = (d / _normal.SquareLength * _normal);
            _apply = true;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            if (_apply)
                bodies[_handle].ApplyMove(_handle.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc/>
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
