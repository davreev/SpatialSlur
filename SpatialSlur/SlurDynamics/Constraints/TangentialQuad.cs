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
    [Serializable]
    public class TangentialQuad : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private H _h2 = new H();
        private H _h3 = new H();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="i3"></param>
        /// <param name="weight"></param>
        public TangentialQuad(int i0, int i1, int i2, int i3, double weight = 1.0)
        {
            _h0.Index = i0;
            _h1.Index = i1;
            _h2.Index = i2;
            _h3.Index = i3;

            Weight = weight;
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
        public H Handle2
        {
            get { return _h2; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle3
        {
            get { return _h3; }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            // equalize sum of opposite edges
            Vec3d p0 = bodies[_h0].Position;
            Vec3d p1 = bodies[_h1].Position;
            Vec3d p2 = bodies[_h2].Position;
            Vec3d p3 = bodies[_h3].Position;

            Vec3d v0 = p1 - p0;
            Vec3d v1 = p2 - p1;
            Vec3d v2 = p3 - p2;
            Vec3d v3 = p0 - p3;

            double m0 = v0.Length;
            double m1 = v1.Length;
            double m2 = v2.Length;
            double m3 = v3.Length;

            double sum0 = m0 + m2;
            double sum1 = m1 + m3;

            // compute projection magnitude as deviation from mean
            double mean = (sum0 + sum1) * 0.5;
            sum0 = (sum0 - mean) * 0.125; // 0.25 / 2 (2 deltas applied per index)
            sum1 = (sum1 - mean) * 0.125;

            // scale deltas
            v0 *= sum0 / m0;
            v1 *= sum1 / m1;
            v2 *= sum0 / m2;
            v3 *= sum1 / m3;

            // cache deltas
            _h0.Delta = v0 - v3;
            _h1.Delta = v1 - v0;
            _h2.Delta = v2 - v1;
            _h3.Delta = v3 - v2;
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_h0].ApplyMove(_h0.Delta, Weight);
            bodies[_h1].ApplyMove(_h1.Delta, Weight);
            bodies[_h2].ApplyMove(_h2.Delta, Weight);
            bodies[_h3].ApplyMove(_h3.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
                yield return _h2;
                yield return _h3;
            }
        }

        #endregion
    }
}
