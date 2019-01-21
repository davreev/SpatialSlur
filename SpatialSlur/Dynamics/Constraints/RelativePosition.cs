
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// Constrains relative orientation between a pair of bodies.
    /// </summary>
    [Serializable]
    public class RelativePosition : Constraint, IConstraint
    {
        private Vector3d _dp0, _dp1;
        private int _i0, _i1;

        private Vector3d _tp0; // target position of body0 in the frame of body1
        private Vector3d _tp1; // target position of body1 in the frame of body0


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="weight"></param>
        public RelativePosition(int index0, int index1, double weight = 1.0)
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public void Set(Vector3d p0, Quaterniond r0, Vector3d p1, Quaterniond r1)
        {
            var dp = p0 - p1;
            _tp0 = r1.Inverse.Apply(dp);
            _tp1 = r0.Inverse.Apply(-dp);
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            var b0 = bodies[_i0];
            var b1 = bodies[_i1];
            
            var dp = b1.Position.Current - b0.Position.Current;
            _dp0 = (b1.Rotation.Current.Apply(_tp0) + dp) * 0.5;
            _dp1 = (b0.Rotation.Current.Apply(_tp1) - dp) * 0.5;
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_i0].Position.AddProjection(_dp0, Weight);
            bodies[_i1].Position.AddProjection(_dp1, Weight);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = _dp0.Length + _dp1.Length;
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
