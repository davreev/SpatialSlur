using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

using static SpatialSlur.SlurData.DataUtil;
using static System.Threading.Tasks.Parallel;

/*
 * Notes
 * 
 * TODO check grid resizing
 */

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = SphereCollide.Handle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SphereCollide : MultiConstraint<H>, IConstraint
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Handle : ParticleHandle
        {
            /// <summary></summary>
            internal bool Skip = false;


            /// <summary>
            /// 
            /// </summary>
            public Handle(int index)
                : base(index)
            {
            }
        }

        #endregion


        /// <summary>If true, collisions are calculated in parallel</summary>
        public bool Parallel;

        private HashGrid3d<H> _grid;
        private double _radius = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value can not be negative");

                _radius = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="parallel"></param>
        /// <param name="weight"></param>
        public SphereCollide(double radius, bool parallel, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Radius = radius;
            Parallel = parallel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="radius"></param>
        /// <param name="parallel"></param>
        /// <param name="weight"></param>
        public SphereCollide(IEnumerable<int> indices, double radius, bool parallel, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Radius = radius;
            Parallel = parallel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            UpdateGrid(particles);

            if (Parallel)
                CalculateImplParallel(particles);
            else
                CalculateImpl(particles);

            _grid.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        private void CalculateImplParallel(IReadOnlyList<IBody> particles)
        {
            var diam = _radius * 2.0;
            var diamSqr = diam * diam;

            // insert all particles
            foreach (var h in Handles)
                _grid.Insert(particles[h].Position, h);

            // search for collisions from each particle
            ForEach(Partitioner.Create(0, Handles.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var h0 = Handles[i];
                    var p0 = particles[h0].Position;

                    var deltaSum = new Vec3d();
                    int count = 0;

                    foreach(var h1 in _grid.Search(new Interval3d(p0, diam)))
                    {
                        var d = particles[h1].Position - p0;
                        var m = d.SquareLength;

                        if (m < diamSqr && m > 0.0)
                        {
                            deltaSum += d * (1.0 - diam / Math.Sqrt(m));
                            count++;
                        }
                    }

                    // no projections applied
                    if (count == 0)
                    {
                        h0.Skip = true;
                        continue;
                    }

                    h0.Delta = deltaSum * 0.5;
                    h0.Skip = false;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        private void CalculateImpl(IReadOnlyList<IBody> particles)
        {
            var diam = _radius * 2.0;
            var diamSqr = diam * diam;

            // clear handles
            foreach(var h in Handles)
            {
                h.Delta = Vec3d.Zero;
                h.Skip = true;
            }

            // calculate collisions
            foreach(var h0 in Handles)
            {
                var p0 = particles[h0].Position;

                // search from h0
                foreach (var h1 in _grid.Search(new Interval3d(p0, diam)))
                {
                    var d = particles[h1].Position - p0;
                    var m = d.SquareLength;

                    if (m < diamSqr && m > 0.0)
                    {
                        d *= (1.0 - diam / Math.Sqrt(m)) * 0.5;
                        h0.Delta += d;
                        h1.Delta -= d;
                        h0.Skip = h1.Skip = false;
                    }
                }

                // insert h0
                _grid.Insert(p0, h0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        private void UpdateGrid(IReadOnlyList<IBody> particles)
        {
            if (_grid == null)
                _grid = new HashGrid3d<H>(Radius * RadiusToHashScale, Handles.Count);
            else
                _grid.Scale = Radius * RadiusToHashScale;
        }

        
        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            foreach (var h in Handles)
                if (!h.Skip) bodies[h].ApplyMove(h.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        bool IConstraint.AppliesRotation
        {
            get { return false; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { return Handles; }
        }

        #endregion
    }
}
