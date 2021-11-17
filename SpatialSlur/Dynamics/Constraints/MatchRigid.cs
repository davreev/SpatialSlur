/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;
using static SpatialSlur.Collections.DynamicArray;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public class MatchRigid : Impl.PositionConstraint
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
        private Vector3d[] _restPositions = Array.Empty<Vector3d>();


        /// <inheritdoc />
        public override void Initialize(
            ArrayView<ParticlePosition> positions, 
            ArrayView<ParticleRotation> rotations)
        {
            if (_restPositions.Length < Particles.Count)
                Array.Resize(ref _restPositions, Particles.Capacity);

            var elements = _elements;

            if (Parallel)
                ForEach(Partitioner.Create(0, elements.Count), range => Initialize(range.Item1, range.Item2));
            else
                Initialize(0, elements.Count);

            void Initialize(int from, int to)
            {
                var particles = Particles;
                var restPositions = _restPositions;

                // Cache centered rest positions
                for (int i = from; i < to; i++)
                {
                    ref var e = ref elements[i];

                    var center = Geometry.GetMassCenter(particles.Segment(e.First, e.Count), positions);

                    for (int j = 0; j < e.Count; j++)
                        restPositions[e.First + j] = positions[particles[e.First + j].PositionIndex].Current - center;
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
                var restPositions = _restPositions;
                var deltas = Deltas;
                
                for (int i = from; i < to; i++)
                {
                    var e = elements[i];

                    var center = Geometry.GetMassCenter(particles.Segment(e.First, e.Count), positions);

                    Matrix3d A = new Matrix3d();
                    {
                        for (int j = 0; j < e.Count; j++)
                        {
                            ref var p = ref positions[particles[e.First + j].PositionIndex];

                            var u = p.Current - center;
                            var v = restPositions[e.First + j];
                            var w = 1.0 / p.InverseMass;

                            A.M00 += u.X * v.X * w;
                            A.M01 += u.X * v.Y * w;
                            A.M02 += u.X * v.Z * w;

                            A.M10 += u.Y * v.X * w;
                            A.M11 += u.Y * v.Y * w;
                            A.M12 += u.Y * v.Z * w;

                            A.M20 += u.Z * v.X * w;
                            A.M21 += u.Z * v.Y * w;
                            A.M22 += u.Z * v.Z * w;
                        }
                    }

                    // Polar decomposition to extract the rotational component
                    int rank =  Matrix3d.Decompose.Polar(ref A, out Matrix3d R, out Matrix3d S);

                    if (rank < 2)
                    {
                        // Zero out deltas if rank is insufficient
                        for (int j = 0; j < e.Count; j++)
                            deltas[j + e.First] = Vector4d.Zero;
                    }
                    else
                    {
                        // Calculate projection deltas as difference bw current positions and the best fit rotation of the rest positions
                        for (int j = 0; j < e.Count; j++)
                        {
                            var d = center + R.Apply(restPositions[e.First + j]) - positions[particles[e.First + j].PositionIndex].Current;
                            deltas[e.First + j] = new Vector4d(d, 1.0) * e.Weight;
                        }
                    }
                }
            }
        }
    }
}
