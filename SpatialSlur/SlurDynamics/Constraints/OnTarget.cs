using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

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
    /// <typeparam name="P"></typeparam>
    [Serializable]
    public class OnTarget<T> : DynamicPositionConstraint<H>
    {
        private T _target;
        Func<T, Vec3d, Vec3d> _closestPoint;
        

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
        /// <param name="weight"></param>
        public OnTarget(T target, Func<T, Vec3d, Vec3d> closestPoint, double weight = 1.0)
            : base(weight)
        {
            Target = target;
            _closestPoint = closestPoint;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="closestPoint"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnTarget(T target, Func<T, Vec3d, Vec3d> closestPoint, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Target = target;
            _closestPoint = closestPoint;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="target"></param>
        /// <param name="closestPoint"></param>
        /// <param name="weight"></param>
        public OnTarget(IEnumerable<int> indices, T target, Func<T, Vec3d, Vec3d> closestPoint, double weight = 1.0)
            : base(indices.Select(i => new H(i)), weight)
        {
            Target = target;
            _closestPoint = closestPoint;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="target"></param>
        /// <param name="closestPoint"></param>
        /// <param name="weight"></param>
        public OnTarget(IEnumerable<H> handles, T target, Func<T, Vec3d, Vec3d> closestPoint, double weight = 1.0)
            : base(handles, weight)
        {
            Target = target;
            _closestPoint = closestPoint;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach(var h in Handles)
            {
                var p = particles[h].Position;
                h.Delta = _closestPoint(_target, p) - p;
            }
        }
    }
}
