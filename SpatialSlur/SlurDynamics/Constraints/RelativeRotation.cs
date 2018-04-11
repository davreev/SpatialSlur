using System.Collections.Generic;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = BodyHandle;

    /// <summary>
    /// 
    /// </summary>
    public class RelativeRotation : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();
        
        private Quaterniond _r10 = Quaterniond.Identity; // rotation of second in frame of first


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="weight"></param>
        public RelativeRotation(int i0, int i1, double weight = 1.0)
        {
            _h0.Index = i0;
            _h1.Index = i1;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="p0"></param>
        /// <param name="r0"></param>
        /// <param name="p1"></param>
        /// <param name="r1"></param>
        /// <param name="weight"></param>
        public RelativeRotation(int i0, int i1, Quaterniond r0, Quaterniond r1, double weight = 1.0)
            : this(i0, i1)
        {
            Set(r0, r1);
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle0
        {
            get { return _h0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle1
        {
            get { return _h1; }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Rotation; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public void Set(Quaterniond r0, Quaterniond r1)
        {
            _r10 = r0.Inverse.Apply(r1); // set target of the second body in the frame of the first
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            var r0 = bodies[_h0].Rotation;
            var r1 = bodies[_h1].Rotation;

            // apply rotation deltas
            var dr = Quaterniond.CreateFromTo(r1, r0.Apply(_r10)).ToAxisAngle() * 0.5;
            _h0.AngleDelta = -dr;
            _h1.AngleDelta = dr;
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_h0].ApplyRotate(_h0.AngleDelta, Weight);
            bodies[_h1].ApplyRotate(_h1.AngleDelta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
            }
        }

        #endregion
    }
}
