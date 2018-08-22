
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DihedralAngle : Constraint, IConstraint
    {
        #region Static Members

        /// <summary>
        /// Assumes both angles are between 0 and 2PI
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <returns></returns>
        private static double GetMinAngleDifference(double a0, double a1)
        {
            var d0 = (a0 < a1) ? a0 - a1 + D.TwoPi : a0 - a1;
            return d0 > Math.PI ? d0 - D.TwoPi : d0;
        }

        #endregion


        private Vector3d _d0, _d1, _d2, _d3;
        private int _i0, _i1, _i2, _i3;
        private double _target;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="target"></param>
        /// <param name="weight"></param>
        public DihedralAngle(int start, int end, int left, int right, double target, double weight = 1.0)
        {
            _i0 = start;
            _i1 = end;
            _i2 = left;
            _i3 = right;

            Target = target;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Start
        {
            get { return _i0; }
            set { _i0 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int End
        {
            get { return _i1; }
            set { _i1 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Left
        {
            get { return _i2; }
            set { _i2 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Right
        {
            get { return _i3; }
            set { _i3 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Target
        {
            get { return _target; }
            set { _target = SlurMath.Repeat(value, D.TwoPi); }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            Vector3d p0 = bodies[_i0].Position.Current;
            Vector3d p1 = bodies[_i1].Position.Current;
            Vector3d p2 = bodies[_i2].Position.Current;
            Vector3d p3 = bodies[_i3].Position.Current;

            var rotation = AxisAngle3d.Identity;
            rotation.Axis = p1 - p0;

            var d2 = p2 - p0;
            var d3 = p3 - p0;

            var angle = Geometry.GetDihedralAngle(rotation.Axis, Vector3d.Cross(rotation.Axis, d2), Vector3d.Cross(rotation.Axis, -d3)) + Math.PI;
            rotation.Angle = GetMinAngleDifference(_target, angle) * 0.5;

            // calculate deltas as diff bw current and rotated
            _d2 = (rotation.Inverse.Apply(d2) - d2) * 0.5;
            _d3 = (rotation.Apply(d3) - d3) * 0.5;

            // distribute reverse projection among hinge bodies
            _d0 = _d1 = (_d2 + _d3) * -0.5; 
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
