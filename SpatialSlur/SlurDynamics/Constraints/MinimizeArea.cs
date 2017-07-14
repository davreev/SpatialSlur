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
    public class MinimizeArea : PositionConstraint<H>
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private H _h2 = new H();


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
        public override sealed IEnumerable<H> Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
                yield return _h2;
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex0"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="weight"></param>
        public MinimizeArea(int vertex0, int vertex1, int vertex2, double weight = 1.0)
        {
            SetHandles(vertex0, vertex1, vertex2);
            Weight = weight;
        }


        /// <summary>
        /// https://www.cs.cmu.edu/~kmcrane/Projects/DDG/paper.pdf p64
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            GeometryUtil.GetTriAreaGrads(
                particles[_h0].Position,
                particles[_h1].Position,
                particles[_h2].Position, 
                out Vec3d g0, out Vec3d g1, out Vec3d g2);

            _h0.Delta = -g0;
            _h1.Delta = -g1;
            _h2.Delta = -g2;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex0"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        public void SetHandles(int vertex0, int vertex1, int vertex2)
        {
            _h0.Index = vertex0;
            _h1.Index = vertex1;
            _h2.Index = vertex2;
        }
    }
}
