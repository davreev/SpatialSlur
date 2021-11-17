
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Cospherical : Impl.PositionConstraint
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public struct Element
        {
            /// <summary>
            /// 
            /// </summary>
            public static Element Default = new Element()
            {
                First = -1,
                Count = 0,
                Weight = 1.0
            };

            /// <summary>Index of the first particle used by this element</summary>
            public int First;

            /// <summary>Number of particles used by this element</summary>
            public int Count;

            /// <summary>Relative influence of this element</summary>
            public double Weight;
        }

        #endregion

        private DynamicArray<Element> _elements = new DynamicArray<Element>();


        /// <summary>
        /// 
        /// </summary>
        public DynamicArray<Element> Elements
        {
            get => _elements;
        }


        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            base.Calculate(positions, rotations);
            var elements = _elements;

            if (Parallel)
                ForEach(Partitioner.Create(0, elements.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, elements.Count);

            void Calculate(int from, int to)
            {
                var particles = Particles;
                var deltas = Deltas;

                for (int i = from; i < to; i++)
                {
                    var e = elements[i];

                    if (e.Count < 5 || Geometry.FitSphere(particles.AsView(e.First, e.Count), positions, out var origin, out var radius))
                    {
                        // Zero out deltas if not enough particles
                        for (int j = 0; j < e.Count; j++)
                            deltas[e.First + j] = Vector4d.Zero;
                    }
                    else
                    {
                        for (int j = 0; j < e.Count; j++)
                        {
                            var d = origin - positions[particles[e.First + j].PositionIndex].Current;
                            d *= 1.0 - radius / d.Length;
                            deltas[e.First + j] = new Vector4d(d, 1.0) * e.Weight;
                        }
                    }
                }
            }
        }
    }
}

