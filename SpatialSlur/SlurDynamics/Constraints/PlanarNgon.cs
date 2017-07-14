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
    /// 
    /// </summary>
    [Serializable]
    public class PlanarNgon : DynamicPositionConstraint<H>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public PlanarNgon(IEnumerable<int> indices, double weight = 1.0)
            : base(indices.Select(i => new H(i)), weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="weight"></param>
        public PlanarNgon(IEnumerable<H> handles, double weight = 1.0)
            : base(handles, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            int n = Handles.Count;

            if(n < 4)
            {
                foreach (var h in Handles) h.Delta = new Vec3d();
            }
            else if(n == 4)
            {
                var h0 = Handles[0];
                var h1 = Handles[1];
                var h2 = Handles[2];
                var h3 = Handles[3];

                var d = GeometryUtil.LineLineShortestVector(
                    particles[h0].Position,
                    particles[h2].Position,
                    particles[h1].Position,
                    particles[h3].Position) * 0.5;

                h0.Delta = h2.Delta = d;
                h1.Delta = h3.Delta = -d;
            }
            else
            {
                foreach (var h in Handles) h.Delta = new Vec3d();

                for (int i = 0; i < n; i++)
                {
                    var h0 = Handles[i];
                    var h1 = Handles[(i + 1) % n];
                    var h2 = Handles[(i + 2) % n];
                    var h3 = Handles[(i + 3) % n];


                    var d = GeometryUtil.LineLineShortestVector(
                        particles[h0].Position,
                        particles[h2].Position,
                        particles[h1].Position,
                        particles[h3].Position) * 0.125; // 0.5 / 4 (4 deltas applied per index)

                    h0.Delta += d;
                    h2.Delta += d;

                    h1.Delta -= d;
                    h3.Delta -= d;
                }
            }
        }
    }
}
