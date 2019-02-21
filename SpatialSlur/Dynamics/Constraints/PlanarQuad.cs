/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SpatialSlur.Collections;

using static SpatialSlur.Geometry;
using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PlanarQuad : Impl.PositionConstraint
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
                Weight = 1.0
            };

            /// <summary>Number of particles used by this element</summary>
            public const int Count = 4;

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

                // TODO Consider particle masses?

                for (int i = from; i < to; i++)
                {
                    ref var e = ref elements[i];

                    int j = i << 2;
                    ref var p0 = ref positions[particles[j].PositionIndex].Current;
                    ref var p1 = ref positions[particles[j + 1].PositionIndex].Current;
                    ref var p2 = ref positions[particles[j + 2].PositionIndex].Current;
                    ref var p3 = ref positions[particles[j + 3].PositionIndex].Current;

                    var d = LineLineShortestVector(p0, p2, p1, p3) * 0.5;

                    deltas[j] = deltas[j + 2] = new Vector4d(d, 1.0) * e.Weight;
                    deltas[j + 1] = deltas[j + 3] = new Vector4d(-d, 1.0) * e.Weight;
                }
            }
        }
    }
}
