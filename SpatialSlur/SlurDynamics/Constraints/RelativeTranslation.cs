using System.Collections.Generic;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = BodyHandle;

    /// <summary>
    /// Constrains relative orientation between a pair of bodies.
    /// </summary>
    public class RelativeTranslation : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();

        private Vec3d _d01; // offset of the first body in the frame of the second
        private Vec3d _d10; // offset of the second body in the frame of the first


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="weight"></param>
        public RelativeTranslation(int i0, int i1, double weight = 1.0)
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
        public RelativeTranslation(int i0, int i1, Vec3d p0, Quaterniond r0, Vec3d p1, Quaterniond r1, double weight = 1.0)
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
            get { return ConstraintType.Rotation; }
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
            _d10 = r0.Inverse.Apply(p1 = p0);
            _d01 = r1.Inverse.Apply(p0 - p1);
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            var b0 = bodies[_h0];
            var b1 = bodies[_h1];

            var d = b1.Position - b0.Position;
            _h0.Delta = (b1.Rotation.Apply(_d01) + d) * 0.5;
            _h1.Delta = (b0.Rotation.Apply(_d10) - d) * 0.5;
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_h0].ApplyMove(_h0.Delta, Weight);
            bodies[_h1].ApplyMove(_h1.Delta, Weight);
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
