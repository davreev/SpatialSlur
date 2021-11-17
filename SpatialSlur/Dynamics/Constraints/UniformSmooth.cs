/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;
using static SpatialSlur.Collections.DynamicArray;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UniformSmooth : Impl.PositionConstraint
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

            if(Parallel)
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
                    var e = elements[i];
                    
                    if(e.Count < 3)
                    {
                        // Zero out deltas if not enough particles
                        for(int j = 0; j < e.Count; j++)
                            deltas[j + e.First] = Vector4d.Zero;
                    }
                    else
                    {
                        // Calculate centroid of the 1 ring
                        var sum = Vector3d.Zero;

                        for (int j = 1; j < e.Count; j++)
                            sum += positions[particles[e.First + j].PositionIndex].Current;

                        double t = 1.0 / (e.Count - 1);
                        var d = (sum * t - positions[particles[e.First].PositionIndex].Current) * (0.5 * e.Weight);

                        // Apply projection to central particle
                        deltas[e.First] = new Vector4d(d, e.Weight);

                        // Distribute reverse projection among 1 ring
                        {
                            d *= -t;
                            for (int j = 1; j < e.Count; j++)
                                deltas[e.First + j] = new Vector4d(d, e.Weight);
                        }
                    }
                }
            } 
        }
    }
}
