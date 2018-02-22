using System;
using System.Collections.Generic;

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
    public class Distance : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private double _target;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="target"></param>
        /// <param name="weight"></param>
        public Distance(int i0, int i1, double target, double weight = 1.0)
        {
            _h0.Index = i0;
            _h1.Index = i1;
            Weight = weight;
            Target = target;
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle0
        {
            get { return _h0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle1
        {
            get { return _h1; }
        }
        

        /// <summary>
        /// 
        /// </summary>
        public double Target
        {
            get { return _target; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _target = value;
            }
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
            var d = bodies[_h1].Position - bodies[_h0].Position;
            d *= (1.0 - _target / d.Length) * 0.5;
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
