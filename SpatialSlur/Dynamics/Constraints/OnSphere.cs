
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
    public class OnSphere : Constraint, IConstraint
    {
        private Vector3d _delta;
        private int _index;

        private Vector3d _origin;
        private double _radius;

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        public OnSphere(int index, Vector3d origin, double radius, double weight = 1.0)
        {
            _index = index;
            _origin = origin;
            Radius = radius;
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
        public Vector3d Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be less than zero.");

                _radius = value;
            }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            var d = _origin - bodies[_index].Position.Current;
            _delta = d * (1.0 - _radius / d.Length);
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
            get { yield return _index; }
            set { _index = value.First(); }
        }

        #endregion
    }
}
