
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
    public class TangentialQuad : Constraint, IConstraint
    {
        private Vector3d _d0, _d1, _d2, _d3;
        private int _i0, _i1, _i2, _i3;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="index3"></param>
        /// <param name="weight"></param>
        public TangentialQuad(int index0, int index1, int index2, int index3, double weight = 1.0)
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
            Vector3d p0 = bodies[_i0].Position.Current;
            Vector3d p1 = bodies[_i1].Position.Current;
            Vector3d p2 = bodies[_i2].Position.Current;
            Vector3d p3 = bodies[_i3].Position.Current;

            Vector3d v0 = p1 - p0;
            Vector3d v1 = p2 - p1;
            Vector3d v2 = p3 - p2;
            Vector3d v3 = p0 - p3;

            double m0 = v0.Length;
            double m1 = v1.Length;
            double m2 = v2.Length;
            double m3 = v3.Length;

            // equalize sum of opposite edges
            double sum0 = m0 + m2;
            double sum1 = m1 + m3;

            // projection magnitude as standard deviation
            double mean = (sum0 + sum1) * 0.5;
            sum0 = (sum0 - mean) * 0.125; // 0.25 / 2 (2 deltas applied per index)
            sum1 = (sum1 - mean) * 0.125;
            v0 *= sum0 / m0;
            v1 *= sum1 / m1;
            v2 *= sum0 / m2;
            v3 *= sum1 / m3;

            // cache deltas
            _d0 = v0 - v3;
            _d1 = v1 - v0;
            _d2 = v2 - v1;
            _d3 = v3 - v2;
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_i0].Position.AddDelta(_d0, Weight);
            bodies[_i1].Position.AddDelta(_d1, Weight);
            bodies[_i2].Position.AddDelta(_d2, Weight);
            bodies[_i3].Position.AddDelta(_d3, Weight);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = _d0.Length + _d1.Length + _d2.Length + _d3.Length;
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
