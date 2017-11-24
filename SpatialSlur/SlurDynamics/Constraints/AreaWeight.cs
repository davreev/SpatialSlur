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
    /// Applies a force proportional to the area of the triangle defined by 3 particles.
    /// </summary>
    [Serializable]
    public class AreaWeight : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private H _h2 = new H();

        /// <summary></summary>
        public Vec3d Force;

        private double _massPerArea = 1.0;


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
        public H Handle2
        {
            get { return _h2; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double MassPerArea
        {
            get { return _massPerArea; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Mass must be greater than zero.");

                _massPerArea = value;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="force"></param>
        /// <param name="massPerArea"></param>
        /// <param name="weight"></param>
        public AreaWeight(int i0, int i1, int i2, Vec3d force, double massPerArea = 1.0, double weight = 1.0)
        {
            _h0.Index = i0;
            _h1.Index = i1;
            _h2.Index = i2;

            Force = force;
            MassPerArea = massPerArea;
            Weight = weight;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            Vec3d p0 = particles[_h0].Position;
            Vec3d p1 = particles[_h1].Position;
            Vec3d p2 = particles[_h2].Position;

            const double inv6 = 1.0 / 6.0;
            _h0.Delta = _h1.Delta = _h2.Delta = Force * (Vec3d.Cross(p1 - p0, p2 - p1).Length * _massPerArea * inv6);
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
            bodies[_h2].ApplyMove(_h2.Delta, Weight);
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
                yield return _h2;
            }
        }

        #endregion
    }
}
