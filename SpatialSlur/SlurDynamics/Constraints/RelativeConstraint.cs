using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */
 
namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = BodyHandle;

    /// <summary>
    /// Constrains relative orientation between a pair of bodies.
    /// </summary>
    public class RelativeConstraint : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();

        // targets of first body in frame of second
        private Quaterniond _r01 = Quaterniond.Identity;
        private Vec3d _p01 = Vec3d.Zero;

        // targets of second body in frame of first
        private Quaterniond _r10 = Quaterniond.Identity;
        private Vec3d _p10 = Vec3d.Zero;


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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="weight"></param>
        public RelativeConstraint(int i0, int i1, double weight = 1.0)
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
        public RelativeConstraint(int i0, int i1, Vec3d p0, Quaterniond r0, Vec3d p1, Quaterniond r1, double weight = 1.0)
            :this(i0, i1)
        {
            Set(p0, r0, p1, r1);
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
            var r0Inv = r0.Inverse;
            var r1Inv = r1.Inverse;

            // set targets of the first body in the frame of the second
            _r01 = r1Inv.Apply(r0);
            _p01 = r1Inv.Apply(p0 - p1);

            // set targets of the second body in the frame of the first
            _r10 = r0Inv.Apply(r1);
            _p10 = r0Inv.Apply(p1 - p0);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            var b0 = bodies[_h0];
            var b1 = bodies[_h1];
            
            var p0 = b0.Position;
            var r0 = b0.Rotation;

            var p1 = b1.Position;
            var r1 = b1.Rotation;

            // apply position deltas
            var dp = p1 - p0;
            _h0.Delta = (r1.Apply(_p01) + dp) * 0.5;
            _h1.Delta = (r0.Apply(_p10) - dp) * 0.5;
            
            // apply rotation deltas
            var dr0 = Quaterniond.CreateFromTo(r0, r1.Apply(_r01));
            dr0.ToAxisAngle(out Vec3d a0, out double t0);
            _h0.AngleDelta = a0 * (t0 * 0.5);

            var dr1 = Quaterniond.CreateFromTo(r1, r0.Apply(_r10));
            dr1.ToAxisAngle(out Vec3d a1, out double t1);
            _h1.AngleDelta = a1 * (t1 * 0.5);
        }
        

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
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

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        bool IConstraint.AppliesRotation
        {
            get { return true; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
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
