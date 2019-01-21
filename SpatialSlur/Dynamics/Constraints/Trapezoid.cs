
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
    public class Trapezoid : Constraint, IConstraint
    {
        private Vector3d _d0, _d1;
        private int _i0, _i1, _i2, _i3;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="index3"></param>
        /// <param name="weight"></param>
        public Trapezoid(int index0, int index1, int index2, int index3, double weight = 1.0)
        {
            _i0 = index0;
            _i1 = index1;
            _i2 = index2;
            _i3 = index3;
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


        /// <summary>
        /// 
        /// </summary>
        public int Index3
        {
            get { return _i3; }
            set { _i3 = value; }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            Vector3d d0 = bodies[_i1].Position.Current - bodies[_i0].Position.Current;
            Vector3d d1 = bodies[_i3].Position.Current - bodies[_i2].Position.Current;
            double d01 = Vector3d.Dot(d0, d1);

            _d0 = (d0 - d1 * (d01 / d1.SquareLength)) * 0.25; // rejection of d0 onto d1
            _d1 = (d1 - d0 * (d01 / d0.SquareLength)) * 0.25; // rejection of d1 onto d0
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_i0].Position.AddProjection(_d0, Weight);
            bodies[_i1].Position.AddProjection(-_d0, Weight);
            bodies[_i2].Position.AddProjection(_d1, Weight);
            bodies[_i3].Position.AddProjection(-_d1, Weight);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = _d0.Length * 2.0 + _d1.Length * 2.0;
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
                yield return _i2;
                yield return _i3;
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

                itr.MoveNext();
                _i3 = itr.Current;
            }
        }

        #endregion
    }
}
