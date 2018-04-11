using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes 
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DihedralAngle : Constraint, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private H _h2 = new H();
        private H _h3 = new H();
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
            _h0.Index = start;
            _h1.Index = end;
            _h2.Index = left;
            _h3.Index = right;

            Target = target;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public H Start
        {
            get { return _h0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H End
        {
            get { return _h1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H Left
        {
            get { return _h2; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H Right
        {
            get { return _h3; }
        }


        /// <summary>
        /// Note this value is wrapped between 0 and 2PI.
        /// </summary>
        public double Target
        {
            get { return _target; }
            set { _target = SlurMath.Mod(value, SlurMath.TwoPI); }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            // TOOD
            // test implementation

            Vec3d p0 = bodies[_h0].Position;
            Vec3d p1 = bodies[_h1].Position;
            Vec3d p2 = bodies[_h2].Position;
            Vec3d p3 = bodies[_h3].Position;

            var rotation = AxisAngle3d.Identity;
            rotation.Axis = p1 - p0;

            var angle = GeometryUtil.GetDihedralAngle(rotation.Axis, Vec3d.Cross(rotation.Axis, p2 - p1), Vec3d.Cross(rotation.Axis, p0 - p3));
            rotation.Angle = (_target - angle) * 0.5;

            /*
            // calculate deltas
            _h3.Delta = rotation.Apply(p3) - p3;
            rotation.Invert();
            _h2.Delta = rotation.Apply(p2) - p2;
            */

            // calc deltas
            var d3 = rotation.Apply(p3) - p3;
            rotation.Invert();
            var d2 = rotation.Apply(p2) - p2;

            _h0.Delta = _h1.Delta = (d2 + d3) * 0.25; // distribute projection bw pair
            _h2.Delta = d2;
            _h3.Delta = d3;
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_h0].ApplyMove(_h0.Delta, Weight);
            bodies[_h1].ApplyMove(_h1.Delta, Weight);
            bodies[_h2].ApplyMove(_h2.Delta, Weight);
            bodies[_h3].ApplyMove(_h3.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
                yield return _h2;
                yield return _h3;
            }
        }

        #endregion
    }
}
