using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

using static SpatialSlur.SlurData.DataUtil;
using static System.Threading.Tasks.Parallel;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = SphereCollide.CustomHandle;

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
        public class CustomHandle : ParticleHandle
        {
            private bool _apply;


            /// <summary>
            /// 
            /// </summary>
            public CustomHandle(int index)
                : base(index)
            {
            }


            /// <summary>
            /// 
            /// </summary>
            internal bool Apply { get => _apply; set => _apply = value; }
        }

        #endregion

        
        private HashGrid3d<H> _grid;
        private double _radius = 1.0;
        private bool _parallel;


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
            _parallel = parallel;
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
            _parallel = parallel;
        }


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
        /// If true, collisions are calculated in parallel
        /// </summary>
        public bool Parallel
        {
            get { return _parallel; }
            set { _parallel = value; }
        }
        

        /// <summary>
        /// 
        /// </summary>
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            UpdateGrid(bodies);

            if (_parallel)
                CalculateImplParallel(bodies);
            else
                CalculateImpl(bodies);

            _grid.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        private void CalculateImplParallel(IReadOnlyList<IBody> bodies)
        {
            var diam = _radius * 2.0;
            var diamSqr = diam * diam;

            // insert all particles
            foreach (var h in Handles)
                _grid.Insert(bodies[h].Position, h);

            // search for collisions from each particle
            ForEach(Partitioner.Create(0, Handles.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var h0 = Handles[i];
                    var p0 = bodies[h0].Position;

                    var deltaSum = new Vec3d();
                    int count = 0;

                    foreach(var h1 in _grid.Search(new Interval3d(p0, diam)))
                    {
                        var d = bodies[h1].Position - p0;
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
                        h0.Apply = false;
                        continue;
                    }

                    h0.Delta = deltaSum * 0.5;
                    h0.Apply = true;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        private void CalculateImpl(IReadOnlyList<IBody> bodies)
        {
            var diam = _radius * 2.0;
            var diamSqr = diam * diam;

            // clear handles
            foreach(var h in Handles)
            {
                h.Delta = Vec3d.Zero;
                h.Apply = false;
            }

            // calculate collisions
            foreach(var h0 in Handles)
            {
                var p0 = bodies[h0].Position;

                // search from h0
                foreach (var h1 in _grid.Search(new Interval3d(p0, diam)))
                {
                    var d = bodies[h1].Position - p0;
                    var m = d.SquareLength;

                    if (m < diamSqr && m > 0.0)
                    {
                        d *= (1.0 - diam / Math.Sqrt(m)) * 0.5;
                        h0.Delta += d;
                        h1.Delta -= d;
                        h0.Apply = h1.Apply = true;
                    }
                }

                // insert h0
                _grid.Insert(p0, h0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        private void UpdateGrid(IReadOnlyList<IBody> bodies)
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
                if (h.Apply) bodies[h].ApplyMove(h.Delta, Weight);
        }


        #region Explicit interface implementations
        
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
