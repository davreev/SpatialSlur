
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class OnTarget<T> : Constraint, IConstraint
        where T : class
    {
        private T _target;
        private Vector3d _delta;
        private int _index;
        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="target"></param>
        /// <param name="weight"></param>
        public OnTarget(int index, T target, double weight = 1.0)
        {
            _index = index;
            Target = target;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public T Target
        {
            get { return _target; }
            set { _target = value ?? throw new ArgumentNullException(); }
        }


        /// <summary>
        /// Returns the point on the target object that is closest to the given point.
        /// </summary>
        /// <returns></returns>
        protected abstract Vector3d GetClosestPoint(Vector3d point);


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            var p = bodies[_index].Position.Current;
            _delta = GetClosestPoint(p) - p;
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_index].Position.AddDelta(_delta, Weight);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = _delta.Length;
            angular = 0.0;
        }


        #region Explicit Interface Implementations

        bool IConstraint.AffectsPosition
        {
            get { return true; }
        }


        bool IConstraint.AffectsRotation
        {
            get { return false; }
        }


        IEnumerable<int> IConstraint.Indices
        {
            get { yield return _index; }
            set { _index = value.First(); }
        }

        #endregion
    }
}
