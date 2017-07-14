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
    public class CyclicQuad : PositionConstraint<H>
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private H _h2 = new H();
        private H _h3 = new H();


        /// <summary>
        /// 
        /// </summary>
        public H Vertex0
        {
            get { return _h0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H Vertex1
        {
            get { return _h1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H Vertex2
        {
            get { return _h2; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H Vertex3
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
        /// 
        /// </summary>
        /// <param name="vertex0"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="vertex3"></param>
        /// <param name="weight"></param>
        public CyclicQuad(int vertex0, int vertex1, int vertex2, int vertex3, double weight = 1.0)
        {
            SetHandles(vertex0, vertex1, vertex2, vertex3);
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            Vec3d p0 = particles[_h0].Position;
            Vec3d p1 = particles[_h1].Position;
            Vec3d p2 = particles[_h2].Position;
            Vec3d p3 = particles[_h3].Position;

            // get average center of each circle
            Vec3d cen =
                (
                GeometryUtil.GetCurvatureCenter(p0, p1, p2) +
                GeometryUtil.GetCurvatureCenter(p1, p2, p3) +
                GeometryUtil.GetCurvatureCenter(p2, p3, p0) +
                GeometryUtil.GetCurvatureCenter(p3, p0, p1)
                ) * 0.25;

            // get average distance to center
            double d0 = p0.DistanceTo(cen);
            double d1 = p1.DistanceTo(cen);
            double d2 = p2.DistanceTo(cen);
            double d3 = p3.DistanceTo(cen);
            double rad = (d0 + d1 + d2 + d3) * 0.25;

            // calculate deltas
            _h0.Delta = (cen - p0) * (1.0 - rad / d0);
            _h1.Delta = (cen - p1) * (1.0 - rad / d1);
            _h2.Delta = (cen - p2) * (1.0 - rad / d2);
            _h3.Delta = (cen - p3) * (1.0 - rad / d3);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex0"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="vertex3"></param>
        public void SetHandles(int vertex0, int vertex1, int vertex2, int vertex3)
        {
            _h0.Index = vertex0;
            _h1.Index = vertex1;
            _h2.Index = vertex2;
            _h3.Index = vertex3;
        }
    }
}
