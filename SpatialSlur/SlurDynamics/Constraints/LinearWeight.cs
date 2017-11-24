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
    /// Applies a force proportional to the distance between 2 particles.
    /// </summary>
    [Serializable]
    public class LinearWeight : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();

        /// <summary></summary>
        public Vec3d Force;

        private double _massPerLength;


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
        public double MassPerLength
        {
            get { return _massPerLength; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Mass must be greater than zero.");

                _massPerLength = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="force"></param>
        /// <param name="massPerLength"></param>
        /// <param name="weight"></param>
        public LinearWeight(int i0, int i1, Vec3d force, double massPerLength = 1.0, double weight = 1.0)
        {
            _h0.Index = i0;
            _h1.Index = i1;

            Force = force;
            MassPerLength = massPerLength;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            _h0.Delta = _h1.Delta = Force * (particles[_h0].Position.DistanceTo(particles[_h1].Position) * _massPerLength * 0.5);
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
        bool IConstraint.AppliesRotation
        {
            get { return false; }
        }


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
