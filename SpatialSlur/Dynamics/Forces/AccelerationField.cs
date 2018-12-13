
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
    /// Applies a force to each particle proportional to its mass
    /// </summary>
    [Serializable]
    public class AccelerationField : Impl.PositionForce
    {
        private IField3d<Vector3d> _field;
        private double _strength;
        private bool _parallel;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="strength"></param>
        public AccelerationField(IField3d<Vector3d> field, double strength = 1.0)
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
        public AccelerationField(IEnumerable<ParticleHandle> handles, IField3d<Vector3d> field, double strength = 1.0)
            : base(handles)
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


        /// <summary>
        /// 
        /// </summary>
        public bool Parallel
        {
            get => _parallel;
            set => _parallel = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override void Calculate(ParticleBuffer particles)
        {
            var positions = particles.Positions;
            var handles = Handles;
            var deltas = Deltas;

            var field = Field;
            var strength = _strength;

            if (_parallel)
                ForEach(Partitioner.Create(0, handles.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, handles.Count);

            void Calculate(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    ref var p = ref positions[handles[i].PositionIndex];
                    deltas[i] = field.ValueAt(p.Current) * (strength / p.MassInv);
                }
            }
        }
    }
}
