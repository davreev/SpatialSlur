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
    public class Distance : Impl.PositionConstraint
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
                RestDistance = 0.0,
                Weight = 1.0
            };

            /// <summary>The number of particles used by this element</summary>
            public const int Count = 2;

            /// <summary></summary>
            public double RestDistance;

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="rotations"></param>
        public override void Initialize(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            var elements = _elements;

            if (Parallel)
                ForEach(Partitioner.Create(0, elements.Count), range => Initialize(range.Item1, range.Item2));
            else
                Initialize(0, elements.Count);

            void Initialize(int from, int to)
            {
                var particles = Particles;
                var deltas = Deltas;

                for (int i = from; i < to; i++)
                {
                    int j = i << 1;
                    ref var p0 = ref positions[particles[j].PositionIndex];
                    ref var p1 = ref positions[particles[j + 1].PositionIndex];
                    elements[i].RestDistance = p0.Current.DistanceTo(p1.Current);
                }
            }
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
                    d *= 1.0 - e.RestDistance / d.Length;

                    var w0 = p0.InverseMass;
                    var w1 = p1.InverseMass;
                    var invSum = e.Weight / (w0 + w1);

                    deltas[j] = new Vector4d(d * (w0 * invSum), e.Weight);
                    deltas[j + 1] = new Vector4d(d * -(w1 * invSum), e.Weight);
                }
            }
        }
    }
}
