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
    public class RelativeOrientation : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();

        // targets of first body in frame of second
        private Quaterniond _r10 = Quaterniond.Identity; // rotation of the second in the frame of the first
        private Vec3d _d10;
        private Vec3d _d01;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="weight"></param>
        public RelativeOrientation(int i0, int i1, double weight = 1.0)
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
        public RelativeOrientation(int i0, int i1, Vec3d p0, Quaterniond r0, Vec3d p1, Quaterniond r1, double weight = 1.0)
            : this(i0, i1)
        {
            Set(p0, r0, p1, r1);
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
            get { return ConstraintType.PositionRotation; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public void Set(Vec3d p0, Quaterniond r0, Vec3d p1, Quaterniond r1)
        {
            r0.Invert();
            _r10 = r0.Apply(r1);
            _d10 = r0.Apply(p1 - p0);

            r1.Invert();
            _d01 = r1.Apply(p0 - p1);
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            var b0 = bodies[_h0];
            var b1 = bodies[_h1];
            
            var r0 = b0.Rotation;
            var r1 = b1.Rotation;
            
            var dp = b1.Position - b0.Position;
            _h0.Delta = (r1.Apply(_d01) + dp) * 0.5;
            _h1.Delta = (r0.Apply(_d10) - dp) * 0.5;

            var dr = Quaterniond.CreateFromTo(r1, r0.Apply(_r10)).ToAxisAngle() * 0.5;
            _h0.AngleDelta = -dr;
            _h1.AngleDelta = dr;
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            var b0 = bodies[_h0];
            b0.ApplyMove(_h0.Delta, Weight);
            b0.ApplyRotate(_h0.AngleDelta, Weight);

            var b1 = bodies[_h1];
            b1.ApplyMove(_h1.Delta, Weight);
            b1.ApplyRotate(_h1.AngleDelta, Weight);
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
