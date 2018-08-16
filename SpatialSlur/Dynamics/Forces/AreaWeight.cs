
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Forces
{

    /// <summary>
    /// Applies a force proportional to the area of the triangle defined by 3 particles.
    /// </summary>
    [Serializable]
    public class AreaWeight : Force, IConstraint
    {
        private Vector3d _delta;
        private int _i0, _i1, _i2;

        private Vector3d _acceleration;
        private double _massPerArea = 1.0;


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
        public Vector3d Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double MassPerArea
        {
            get { return _massPerArea; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Mass must be greater than zero.");

                _massPerArea = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="acceleration"></param>
        /// <param name="massPerArea"></param>
        /// <param name="strength"></param>
        public AreaWeight(int index0, int index1, int index2, Vector3d acceleration, double massPerArea = 1.0, double strength = 1.0)
        {
            _i0 = index0;
            _i1 = index1;
            _i2 = index2;
            _acceleration = acceleration;
            MassPerArea = massPerArea;
            Strength = strength;
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            Vector3d p0 = bodies[_i0].Position.Current;
            Vector3d p1 = bodies[_i1].Position.Current;
            Vector3d p2 = bodies[_i2].Position.Current;

            const double inv6 = 1.0 / 6.0;
            _delta = _acceleration * (Vector3d.Cross(p1 - p0, p2 - p1).Length * _massPerArea * Strength * inv6);
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_i0].Position.AddForce(_delta);
            bodies[_i1].Position.AddForce(_delta);
            bodies[_i2].Position.AddForce(_delta);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = _delta.Length * 3.0;
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
