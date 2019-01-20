/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SpatialSlur.Collections;
using SpatialSlur.Fields;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force to each particle
    /// </summary>
    [Serializable]
    public class ForceField : Impl.PositionForce
    {
        private IField3d<Vector3d> _field;
        private double _strength;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="strength"></param>
        public ForceField(IField3d<Vector3d> field, double strength = 1.0)
        {
            Field = field;
            _strength = strength;
        }


        /// <summary>
        /// 
        /// </summary>
        public IField3d<Vector3d> Field
        {
            get => _field;
            set => _field = value ?? throw new ArgumentNullException();
        }


        /// <summary>
        /// 
        /// </summary>
        public double Strength
        {
            get => _strength;
            set => _strength = value;
        }


        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            var particles = Particles;

            if (Parallel)
                ForEach(Partitioner.Create(0, particles.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, particles.Count);

            void Calculate(int from, int to)
            {
                var deltas = Deltas;
                var field = Field;
                var strength = _strength;

                for (int i = from; i < to; i++)
                    deltas[i] = field.ValueAt(positions[particles[i].PositionIndex].Current) * strength;
            }
        }
    }
}
