
/*
 * Notes
 */

using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PositionPair : Constraint, IConstraint
    {
        private Vector3d _d0, _d1;
        private int _i0, _i1;
        private bool _apply = true;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public PositionPair(double weight = 1.0)
        {
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="weight"></param>
        public PositionPair(int index0, int index1, double weight = 1.0)
        {
            _i0 = index0;
            _i1 = index1;
            Weight = weight;
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


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            _apply = Calculate(bodies, _i0, _i1, out _d0, out _d1);
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract bool Calculate(ReadOnlyArrayView<Body> bodies, int index0, int index1, out Vector3d delta0, out Vector3d delta1);


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            if (!_apply) return;
            bodies[_i0].Position.AddDelta(_d0, Weight);
            bodies[_i1].Position.AddDelta(_d1, Weight);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = _d0.Length + _d1.Length;
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
            get
            {
                yield return _i0;
                yield return _i1;
            }
            set
            {
                var itr = value.GetEnumerator();

                itr.MoveNext();
                _i0 = itr.Current;

                itr.MoveNext();
                _i1 = itr.Current;
            }
        }

        #endregion
    }
}
