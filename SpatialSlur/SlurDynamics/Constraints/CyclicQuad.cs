using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

using static SpatialSlur.SlurCore.GeometryUtil;

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
    public class CyclicQuad : Constraint, IConstraint
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
        public CyclicQuad(int i0, int i1, int i2, int i3, double weight = 1.0)
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
            Vec3d p0 = bodies[_h0].Position;
            Vec3d p1 = bodies[_h1].Position;
            Vec3d p2 = bodies[_h2].Position;
            Vec3d p3 = bodies[_h3].Position;

            // get average center of each circle
            Vec3d cen = ( 
                GetCurvatureCenter(p0, p1, p2) + 
                GetCurvatureCenter(p1, p2, p3) +
                GetCurvatureCenter(p2, p3, p0) +
                GetCurvatureCenter(p3, p0, p1)) * 0.25;

            // get average distance to center
            var d0 = p0.DistanceTo(cen);
            var d1 = p1.DistanceTo(cen);
            var d2 = p2.DistanceTo(cen);
            var d3 = p3.DistanceTo(cen);
            var rad = (d0 + d1 + d2 + d3) * 0.25;

            // calculate deltas
            _h0.Delta = (cen - p0) * (1.0 - rad / d0);
            _h1.Delta = (cen - p1) * (1.0 - rad / d1);
            _h2.Delta = (cen - p2) * (1.0 - rad / d2);
            _h3.Delta = (cen - p3) * (1.0 - rad / d3);
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
            bodies[_h3].ApplyMove(_h3.Delta, Weight);
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
                yield return _h2;
                yield return _h3;
            }
        }

        #endregion
    }
}
