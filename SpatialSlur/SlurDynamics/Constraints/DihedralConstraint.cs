using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes 
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// http://www.tsg.ne.jp/TT/cg/ElasticOrigami_Tachi_IASS2013.pdf
    /// </summary>
    [Serializable]
    public class DihedralConstraint : ParticleConstraint<H>
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private H _h2 = new H();
        private H _h3 = new H();

        private double _targetAngle;


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
        /// 
        /// </summary>
        public override sealed IEnumerable<H> Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
                yield return _h2;
                yield return _h3;
            }
        }


        /// <summary>
        /// Note this value is wrapped between 0 and 2PI.
        /// </summary>
        public double TargetAngle
        {
            get { return _targetAngle; }
            set { _targetAngle = SlurMath.Mod(value, SlurMath.TwoPI); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="targetAngle"></param>
        /// <param name="weight"></param>
        public DihedralConstraint(int start, int end, int left, int right, double targetAngle, double weight = 1.0)
        {
            SetHandles(start, end, left, right);
            TargetAngle = targetAngle;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            // TODO revise implementation
            throw new NotImplementedException();

            // TODO
            // cache local Rotation3ds for the target angle
            // avoids need to explicitly measure angle deviation

            Vec3d p0 = particles[_h0].Position;
            Vec3d p1 = particles[_h1].Position;
            Vec3d p2 = particles[_h2].Position;
            Vec3d p3 = particles[_h3].Position;

            Vec3d v01 = p1 - p0;
            Vec3d v02 = p2 - p0;
            Vec3d v03 = p3 - p0;

            // get heights
            double h0 = Vec3d.Reject(v02, v01).Length;
            double h1 = Vec3d.Reject(v03, v01).Length;
            double h = 0.5 / (h0 + h1); // inv mean height

            // get projection directions (face normals)
            Vec3d n0 = Vec3d.Cross(v01, v02);
            Vec3d n1 = Vec3d.Cross(v03, v01);

            // cache lengths
            double m0 = 1.0 / n0.Length;
            double m1 = 1.0 / n1.Length;

            // angle error
            double angle = Math.Acos(SlurMath.Clamp(Vec3d.Dot(n0, n1) * m0 * m1, -1.0, 1.0));
            if (Vec3d.Dot(n1, v02) < 0.0) angle *= -1.0; // negate if convex
            angle += Math.PI;

            // projection magnitude & relevant cotangents
            double m = (angle - _targetAngle) * h * 0.5;
            double c0 = Vec3d.Dot(v02, v01) * m0;
            double c1 = Vec3d.Dot(p1 - p2, v01) * m0;
            double c2 = Vec3d.Dot(v03, v01) * m1;
            double c3 = Vec3d.Dot(p1 - p3, v01) * m1;

            // calculate deltas
            _h0.Delta = n0 * (m * c1) + n1 * (m * c3);
            _h1.Delta = n0 * (m * c0) + n1 * (m * c2);
            _h2.Delta = n0 * -(m * (c0 + c1));
            _h3.Delta = n1 * -(m * (c2 + c3));

            _h0.Weight = _h1.Weight = _h2.Weight = _h3.Weight = Weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void SetHandles(int start, int end, int left, int right)
        {
            _h0.Index = start;
            _h1.Index = end;
            _h2.Index = left;
            _h3.Index = right;
        }
    }
}
