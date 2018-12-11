
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
    /// 
    /// </summary>
    [Serializable]
    public class ForceField : PositionForceBase
    {
        private IField3d<Vector3d> _field;
        private double _strength;
        private bool _parallel;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
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
        /// <param name="index"></param>
        /// <param name="field"></param>
        /// <param name="strength"></param>
        public ForceField(IEnumerable<int> indices, IField3d<Vector3d> field, double strength = 1.0)
            : base(indices)
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
        /// <param name="indices"></param>
        /// <param name="deltas"></param>
        protected override void Calculate(ReadOnlyArrayView<Particle> particles, ReadOnlyArrayView<int> indices, ArrayView<Vector3d> deltas)
        {
            if (_parallel)
                ForEach(Partitioner.Create(0, indices.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, indices.Count);

            void Calculate(int from, int to)
            {
                var field = _field;
                var strength = _strength;

                for (int i = from; i < to; i++)
                    deltas[i] = field.ValueAt(particles[indices[i]].Position.Current) * strength;
            }
        }
    }
}
