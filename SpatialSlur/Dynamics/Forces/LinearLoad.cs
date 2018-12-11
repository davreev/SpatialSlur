
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force proportional to the distance between 2 particles.
    /// </summary>
    [Serializable]
    public class LinearLoad : IForce
    {
        private Vector3d _d0;
        private int _i0, _i1;

        private Vector3d _force;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="force"></param>
        /// <param name="massPerLength"></param>
        /// <param name="strength"></param>
        public LinearLoad(int index0, int index1, Vector3d force)
        {
            _i0 = index0;
            _i1 = index1;
            _force = force;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index0
        {
            get { return _i0; }
            set { _i0 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index1
        {
            get { return _i1; }
            set { _i1 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Force
        {
            get { return _force; }
            set { _force = value; }
        }


        /// <inheritdoc />
        /// 
        public void Calculate(ReadOnlyArrayView<Particle> bodies)
        {
            var d = bodies[_i1].Position.Current - bodies[_i0].Position.Current;
            _d0 = _force * (d.Length * 0.5);
        }


        /// <inheritdoc />
        /// 
        public void Apply(ReadOnlyArrayView<Particle> bodies)
        {
            bodies[_i0].Position.ForceSum += _d0;
            bodies[_i1].Position.ForceSum += _d0;
        }


        #region Explicit interface implementations
        
        void IInfluence.Initialize(ReadOnlyArrayView<Particle> particles) { }

        #endregion
    }
}
