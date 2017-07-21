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
    public abstract class Constraint<H> : IConstraint
        where H : BodyHandle
    {
        private double _weight;


        /// <summary>
        /// Allows iteration over all handles used by this constraint.
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
        public bool AppliesRotation
        {
            get { return true; }
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


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        public void SetHandles(IEnumerable<int> indices)
        {
            var itr = indices.GetEnumerator();

            foreach (var h in Handles)
            {
                itr.MoveNext();
                h.Index = itr.Current;
            }
        }
    }
}
