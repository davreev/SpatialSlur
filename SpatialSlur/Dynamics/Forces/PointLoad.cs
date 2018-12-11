
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
    /// 
    /// </summary>
    [Serializable]
    public class PointLoad : IForce
    {
        Vector3d _d0;
        private int _i0;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="acceleration"></param>
        /// <param name="mass"></param>
        /// <param name="strength"></param>
        public PointLoad(int index, Vector3d force)
        {
            _i0 = index;
            _d0 = force;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get { return _i0; }
            set { _i0 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Force
        {
            get { return _d0; }
            set { _d0 = value; }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Particle> particles)
        {
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Particle> particles)
        {
            particles[_i0].Position.ForceSum += _d0;
        }


        #region Explicit Interface Implementations

        void IInfluence.Initialize(ReadOnlyArrayView<Particle> particles) { }

        #endregion
    }
}
