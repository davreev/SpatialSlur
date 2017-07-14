using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes 
 * 
 * Can also be used to find tangent incircles for adjacent triangles.
 */
 
namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    public class TangentialQuad : PositionConstraint<H>
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
        public TangentialQuad(int vertex0, int vertex1, int vertex2, int vertex3, double weight = 1.0)
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
            // equalize sum of opposite edges
            Vec3d p0 = particles[_h0].Position;
            Vec3d p1 = particles[_h1].Position;
            Vec3d p2 = particles[_h2].Position;
            Vec3d p3 = particles[_h3].Position;

            Vec3d v0 = p1 - p0;
            Vec3d v1 = p2 - p1;
            Vec3d v2 = p3 - p2;
            Vec3d v3 = p0 - p3;

            double m0 = v0.Length;
            double m1 = v1.Length;
            double m2 = v2.Length;
            double m3 = v3.Length;

            double sum0 = m0 + m2;
            double sum1 = m1 + m3;

            // compute projection magnitude as deviation from mean
            double mean = (sum0 + sum1) * 0.5;
            sum0 = (sum0 - mean) * 0.125; // 0.25 / 2 (2 deltas applied per index)
            sum1 = (sum1 - mean) * 0.125;

            // scale deltas
            v0 *= sum0 / m0;
            v1 *= sum1 / m1;
            v2 *= sum0 / m2;
            v3 *= sum1 / m3;

            // cache deltas
            _h0.Delta = v0 - v3;
            _h1.Delta = v1 - v0;
            _h2.Delta = v2 - v1;
            _h3.Delta = v3 - v2;
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
