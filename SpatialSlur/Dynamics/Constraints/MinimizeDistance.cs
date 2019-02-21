/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class MinimizeDistance : Impl.PositionConstraint
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

            /// <summary>The number of particles used by this element</summary>
            public const int Count = 2;

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
                    ref var e = ref elements[i];

                    int j = i << 1;
                    ref var p0 = ref positions[particles[j].PositionIndex];
                    ref var p1 = ref positions[particles[j + 1].PositionIndex];

                    var d = p1.Current - p0.Current;

                    var w0 = p0.InverseMass;
                    var w1 = p1.InverseMass;
                    var t = e.Weight / (w0 + w1);

                    deltas[j] = new Vector4d(d * (w0 * t), e.Weight);
                    deltas[j + 1] = new Vector4d(d * -(w1 * t), e.Weight);
                }
            }
        }
    }
}
