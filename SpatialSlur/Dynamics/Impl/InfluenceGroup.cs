
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class InfluenceGroup
    {
        private bool _parallel;


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
        /// <typeparam name="T"></typeparam>
        /// <param name="influences"></param>
        /// <param name="particles"></param>
        protected void Apply<T>(List<T> influences, ReadOnlyArrayView<Particle> particles)
            where T : IInfluence
        {
            if (_parallel)
                ApplyParallel(influences, particles);
            else
                ApplySerial(influences, particles);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="influences"></param>
        /// <param name="particles"></param>
        private static void ApplySerial<T>(List<T> influences, ReadOnlyArrayView<Particle> particles)
            where T: IInfluence
        {
            foreach (var infl in influences)
            {
                infl.Calculate(particles);
                infl.Apply(particles);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="influences"></param>
        /// <param name="particles"></param>
        private static void ApplyParallel<T>(List<T> influences, ReadOnlyArrayView<Particle> particles)
            where T : IInfluence
        {
            ForEach(Partitioner.Create(0, influences.Count), range => Body(range.Item1, range.Item2));

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                    influences[i].Calculate(particles);
            }
            
            // Must apply serially to avoid race conditions
            foreach (var infl in influences)
                infl.Apply(particles);
        }
    }
}
