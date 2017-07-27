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


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool AppliesRotation
        {
            get { return true; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public MultiBodyConstraint(double weight = 1.0)
        {
            _handles = new List<H>();
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public MultiBodyConstraint(int capacity, double weight = 1.0)
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


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        public void SetHandles(IEnumerable<int> indices)
        {
            var itr = indices.GetEnumerator();

            foreach (var h in _handles)
            {
                itr.MoveNext();
                h.Index = itr.Current;
            }
        }
    }
}
