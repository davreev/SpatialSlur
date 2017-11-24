using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class ParallelPairs : Constraint, IConstraint
    {
        private H _hA0 = new H();
        private H _hA1 = new H();
        private H _hB0 = new H();
        private H _hB1 = new H();


        /// <summary>
        /// 
        /// </summary>
        public H StartA
        {
            get { return _hA0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H EndA
        {
            get { return _hA1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H StartB
        {
            get { return _hB0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H EndB
        {
            get { return _hB1; }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        /// <param name="weight"></param>
        public ParallelPairs(int startA, int endA, int startB, int endB, double weight = 1.0)
        {
            _hA0.Index = startA;
            _hA1.Index = endA;
            _hB0.Index = startB;
            _hB1.Index = endB;

            Weight = weight;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            Vec3d d0 = particles[_hA1].Position - particles[_hA0].Position;
            Vec3d d1 = particles[_hB1].Position - particles[_hB0].Position;
            double d01 = Vec3d.Dot(d0, d1);

            var r0 = (d0 - d01 / d1.SquareLength * d1) * 0.25; // rejection of d0 onto d1
            var r1 = (d1 - d01 / d0.SquareLength * d0) * 0.25; // rejection of d1 onto d0

            _hA1.Delta = r1;
            _hB1.Delta = r0;

            _hA0.Delta = -r1;
            _hB0.Delta = -r0;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_hA0].ApplyMove(_hA0.Delta, Weight);
            bodies[_hA1].ApplyMove(_hA1.Delta, Weight);
            bodies[_hB0].ApplyMove(_hB0.Delta, Weight);
            bodies[_hB1].ApplyMove(_hB1.Delta, Weight);
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
                yield return _hA0;
                yield return _hA1;
                yield return _hB0;
                yield return _hB1;
            }
        }

        #endregion
    }
}
