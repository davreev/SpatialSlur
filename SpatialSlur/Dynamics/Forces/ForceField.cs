
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
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
        private bool _parallel;


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
        /// <param name="handles"></param>
        /// <param name="field"></param>
        /// <param name="strength"></param>
        public ForceField(IEnumerable<ParticleHandle> handles, IField3d<Vector3d> field, double strength = 1.0)
        {
            SetHandles(handles);
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


        /// <summary>
        /// 
        /// </summary>
        public bool Parallel
        {
            get => _parallel;
            set => _parallel = value;
        }


        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            var handles = Handles;

            if (_parallel)
                ForEach(Partitioner.Create(0, handles.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, handles.Count);

            void Calculate(int from, int to)
            {
                var deltas = Deltas;
                var field = Field;
                var strength = _strength;

                for (int i = from; i < to; i++)
                    deltas[i] = field.ValueAt(positions[handles[i].PositionIndex].Current) * strength;
            }
        }
    }
}
