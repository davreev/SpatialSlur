
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;
using static SpatialSlur.Geometry;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CyclicQuad : Constraint, IConstraint
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
        public CyclicQuad(int index0, int index1, int index2, int index3, double weight = 1.0)
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

            // get average center of each circle
            Vector3d cen = ( 
                GetCurvatureCenter(p0, p1, p2) + 
                GetCurvatureCenter(p1, p2, p3) +
                GetCurvatureCenter(p2, p3, p0) +
                GetCurvatureCenter(p3, p0, p1)) * 0.25;

            // get average distance to center
            var d0 = p0.DistanceTo(cen);
            var d1 = p1.DistanceTo(cen);
            var d2 = p2.DistanceTo(cen);
            var d3 = p3.DistanceTo(cen);
            var rad = (d0 + d1 + d2 + d3) * 0.25;

            // calculate deltas
            _d0 = (cen - p0) * (1.0 - rad / d0);
            _d1 = (cen - p1) * (1.0 - rad / d1);
            _d2 = (cen - p2) * (1.0 - rad / d2);
            _d3 = (cen - p3) * (1.0 - rad / d3);
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_i0].Position.AddProjection(_d0, Weight);
            bodies[_i1].Position.AddProjection(_d1, Weight);
            bodies[_i2].Position.AddProjection(_d2, Weight);
            bodies[_i3].Position.AddProjection(_d3, Weight);
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
