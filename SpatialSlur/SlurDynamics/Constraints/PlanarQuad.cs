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
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    public class PlanarQuad<P> : Constraint<P, H>
        where P : IParticle
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
        public override IEnumerable<H> Handles
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
        /// <param name="restAngle"></param>
        /// <param name="weight"></param>
        public PlanarQuad(int vertex0, int vertex1, int vertex2, int vertex3, double weight = 1.0)
        {
            SetHandles(vertex0, vertex1, vertex2, vertex3);
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override void Calculate(IReadOnlyList<P> particles)
        {
            var d = GeometryUtil.LineLineShortestVector(
                particles[_h0].Position,
                particles[_h2].Position,
                particles[_h1].Position,
                particles[_h3].Position) * 0.5;

            _h0.Delta = _h2.Delta = d;
            _h1.Delta = _h3.Delta = -d;
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
