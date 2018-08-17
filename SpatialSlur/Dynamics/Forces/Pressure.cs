
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force along the normal of the triangle between 3 particles with a magnitude proportional to the area.
    /// </summary>
    [Serializable]
    public class Pressure : Force, IConstraint
    {
        private Vector3d _delta;
        private int _i0, _i1, _i2;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="strength"></param>
        public Pressure(int index0, int index1, int index2, double strength = 1.0)
        {
            _i0 = index0;
            _i1 = index1;
            _i2 = index2;
            Strength = strength;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index0
        {
            get { return _i0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index1
        {
            get { return _i1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index2
        {
            get { return _i2; }
        }

        
        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            Vector3d p0 = bodies[_i0].Position.Current;
            Vector3d p1 = bodies[_i1].Position.Current;
            Vector3d p2 = bodies[_i2].Position.Current;

            const double inv6 = 1.0 / 6.0;
            _delta = Vector3d.Cross(p1 - p0, p2 - p1) * (Strength * inv6); // force is proportional to 1/3 area of tri
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
