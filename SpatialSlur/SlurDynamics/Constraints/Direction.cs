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
    public class Direction : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private Vec3d _target;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="target"></param>
        /// <param name="weight"></param>
        public Direction(int start, int end, Vec3d target, double weight = 1.0)
        {
            _h0.Index = start;
            _h1.Index = end;

            _target = target;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public H Start
        {
            get { return _h0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H End
        {
            get { return _h1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Target
        {
            get { return _target; }
            set { _target = value; }
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
            var d = Vec3d.Reject(bodies[_h1].Position - bodies[_h0].Position, _target) * 0.5;

            _h0.Delta = d;
            _h1.Delta = -d;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_h0].ApplyMove(_h0.Delta, Weight);
            bodies[_h1].ApplyMove(_h1.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IHandle> IConstraint.Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
            }
        }

        #endregion
    }
}
