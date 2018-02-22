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
    public class InsideBounds : Constraint, IConstraint
    {
        private H _handle = new H();
        private Interval3d _bounds;
        private bool _apply;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="weight"></param>
        public InsideBounds(int index, Interval3d bounds, double weight = 1.0)
        {
            _handle.Index = index;
            _bounds = bounds;
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
        public Interval3d Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
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
            var p = bodies[_handle].Position;

            if (_bounds.Contains(p))
            {
                _apply = false;
                return;
            }

            _handle.Delta = _bounds.Clamp(p) - p;
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
