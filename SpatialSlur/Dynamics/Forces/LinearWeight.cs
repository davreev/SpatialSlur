
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force proportional to the distance between 2 particles.
    /// </summary>
    [Serializable]
    public class LinearWeight : Force, IConstraint
    {
        private Vector3d _delta;
        private int _i0, _i1, _i2;

        private Vector3d _acceleration;
        private double _massPerLength;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="acceleration"></param>
        /// <param name="massPerLength"></param>
        /// <param name="strength"></param>
        public LinearWeight(int index0, int index1, Vector3d acceleration, double massPerLength = 1.0, double strength = 1.0)
        {
            _i0 = index0;
            _i1 = index1;
            _acceleration = acceleration;
            MassPerLength = massPerLength;
            Strength = strength;
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
        public Vector3d Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double MassPerLength
        {
            get { return _massPerLength; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Mass must be greater than zero.");

                _massPerLength = value;
            }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            var d = bodies[_i1].Position.Current - bodies[_i0].Position.Current;
            _delta = _acceleration * (d.Length * _massPerLength * Strength * 0.5);
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_i0].Position.AddForce(_delta);
            bodies[_i1].Position.AddForce(_delta);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = _delta.Length * 2.0;
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
