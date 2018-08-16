
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MinimizeArea : Constraint, IConstraint
    {
        private Vector3d _d0, _d1, _d2;
        private int _i0, _i1, _i2;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="weight"></param>
        public MinimizeArea(int index0, int index1, int index2, double weight = 1.0)
        {
            _i0 = index0;
            _i1 = index1;
            _i2 = index2;
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


        /// <summary>
        /// 
        /// </summary>
        public int Index2
        {
            get { return _i2; }
            set { _i2 = value; }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            // impl ref 
            // https://www.cs.cmu.edu/~kmcrane/Projects/DDG/paper.pdf (p.64)

            // TODO fix stability issues
            // project to triangle orthocenter?

            Geometry.GetAreaGradients(
                bodies[_i0].Position.Current,
                bodies[_i1].Position.Current,
                bodies[_i2].Position.Current, 
                out _d0, out _d1, out _d2);

            _d0.Negate();
            _d1.Negate();
            _d2.Negate();
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_i0].Position.AddDelta(_d0, Weight);
            bodies[_i1].Position.AddDelta(_d1, Weight);
            bodies[_i2].Position.AddDelta(_d2, Weight);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = _d0.Length + _d1.Length + _d2.Length;
            angular = 0.0;
        }


        #region Explicit interface implementations

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
                yield return _i2;
            }
            set
            {
                var itr = value.GetEnumerator();

                itr.MoveNext();
                _i0 = itr.Current;

                itr.MoveNext();
                _i1 = itr.Current;

                itr.MoveNext();
                _i2 = itr.Current;
            }
        }

        #endregion
    }
}
