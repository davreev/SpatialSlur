using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Base class for constraints on a dynamic collection of particles.
    /// </summary>
    [Serializable]
    public abstract class MultiBodyConstraint<H> : IConstraint
        where H : BodyHandle
    {
        #region Static

        protected const int DefaultCapacity = 4;

        #endregion


        private List<H> _handles;
        private double _weight;


        /// <summary>
        /// 
        /// </summary>
        public List<H> Handles
        {
            get { return _handles; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public double Weight
        {
            get { return _weight; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("Weight cannot be negative.");

                _weight = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="capacity"></param>
        public MultiBodyConstraint(double weight = 1.0, int capacity = DefaultCapacity)
        {
            _handles = new List<H>(capacity);
            Weight = weight;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public abstract void Calculate(IReadOnlyList<IBody> bodies);


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            foreach (var h in _handles)
            {
                var p = bodies[h];
                p.ApplyMove(h.Delta, h.Weight);
                p.ApplyRotate(h.AngleDelta, h.AngleWeight);
            }
        }


        #region Explicit interface implementations

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        bool IConstraint.AppliesRotation
        {
            get { return true; }
        }


        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { return Handles; }
        }

        #endregion
    }
}
