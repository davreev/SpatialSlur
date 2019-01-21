/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;
using static SpatialSlur.Collections.Buffer;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EqualizeLengths : Impl.PositionConstraint
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

        private SlurList<Element> _elements = new SlurList<Element>();


        /// <summary>
        /// 
        /// </summary>
        public SlurList<Element> Elements
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

                    if (e.Count < 4)
                    {
                        // Zero out deltas if not enough particles
                        for (int j = 0; j < e.Count; j++)
                            deltas[j + e.First] = Vector4d.Zero;
                    }
                    else
                    {
                        var restDist = 0.0;
                        var invWeightSum = 0.0;

                        // Calculate rest distance as weighted mean of segment lengths
                        for (int j = 0; j < e.Count; j += 2)
                        {
                            ref var p0 = ref positions[particles[e.First + j].PositionIndex];
                            ref var p1 = ref positions[particles[e.First + j + 1].PositionIndex];

                            var w = p0.InverseMass + p1.InverseMass;
                            restDist += p0.Current.DistanceTo(p1.Current) / w;
                            invWeightSum += w;
                        }

                        restDist *= invWeightSum;

                        // Calculate projection deltas
                        for (int j = 0; j < e.Count; j += 2)
                        {
                            ref var p0 = ref positions[particles[e.First + j].PositionIndex];
                            ref var p1 = ref positions[particles[e.First + j + 1].PositionIndex];

                            var d = p1.Current - p0.Current;
                            d *= 1.0 - restDist / d.Length;

                            var w0 = p0.InverseMass;
                            var w1 = p1.InverseMass;
                            var t = e.Weight / (w0 + w1);

                            deltas[e.First + j] = new Vector4d(d * (w0 * t), e.Weight);
                            deltas[e.First + j + 1] = new Vector4d(d * -(w1 * t), e.Weight);
                        }
                    }
                }
            }
        }
    }
}
