using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Base class for constraints.
    /// </summary>
    [Serializable]
    public abstract class BodyConstraint<H> : IConstraint
        where H : BodyHandle
    {
        private double _weight;


        /// <summary>
        /// All handles used by this constraint.
        /// </summary>
        public abstract IEnumerable<H> Handles { get; }


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
        

        /// <inheritdoc/>
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
            foreach (var h in Handles)
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
