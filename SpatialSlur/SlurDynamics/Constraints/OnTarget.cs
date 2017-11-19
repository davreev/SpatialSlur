using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using SpatialSlur.SlurCore;

using static System.Threading.Tasks.Parallel;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class OnTarget<T> : MultiParticleConstraint<H>
    {
        /// <summary>If set to true, collisions are calculated in parallel</summary>
        public bool Parallel;

        private T _target;
        private Func<T, Vec3d, Vec3d> _closestPoint;
        

        /// <summary>
        /// 
        /// </summary>
        public T Target
        {
            get { return _target; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _target = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="closestPoint"></param>
        /// <param name="parallel"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnTarget(T target, Func<T, Vec3d, Vec3d> closestPoint, bool parallel, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Target = target;
            Parallel = parallel;
            _closestPoint = closestPoint;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="target"></param>
        /// <param name="closestPoint"></param>
        /// <param name="parallel"></param>
        /// <param name="weight"></param>
        public OnTarget(IEnumerable<int> indices, T target, Func<T, Vec3d, Vec3d> closestPoint, bool parallel, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Target = target;
            Parallel = parallel;
            _closestPoint = closestPoint;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            if (Parallel)
                ForEach(Partitioner.Create(0, Handles.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Handles.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var h = Handles[i];
                    var p = particles[h].Position;
                    h.Delta = _closestPoint(_target, p) - p;
                    h.Weight = Weight;
                }
            }
        }
    }
}
