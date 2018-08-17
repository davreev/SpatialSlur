
/*
 * Notes
 */

using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    public class RelativeRotation : Constraint, IConstraint
    {
        private Vector3d _dr0;
        private int _i0, _i1;
        
        private Quaterniond _tr0 = Quaterniond.Identity; // target rotation of body0 in frame of body1


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="weight"></param>
        public RelativeRotation(int index0, int index1, double weight = 1.0)
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
        public void SetTargets(Quaterniond r0, Quaterniond r1)
        {
            _tr0 = r1.Inverse.Apply(r0);
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            var r0 = bodies[_i0].Rotation.Current;
            var r1 = bodies[_i1].Rotation.Current;

            // apply rotation deltas
            _dr0 = Quaterniond.CreateFromTo(r0, r1.Apply(_tr0)).ToAxisAngle() * 0.5;
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_i0].Rotation.AddDelta(_dr0, Weight);
            bodies[_i1].Rotation.AddDelta(-_dr0, Weight);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = 0.0;
            angular = _dr0.Length * 2.0;
        }


        #region Explicit Interface Implementations

        bool IConstraint.AffectsPosition
        {
            get { return false; }
        }


        bool IConstraint.AffectsRotation
        {
            get { return true; }
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
