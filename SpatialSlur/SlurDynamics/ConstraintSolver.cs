using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

using SpatialSlur.SlurDynamics.Constraints;

using static SpatialSlur.SlurDynamics.ConstraintSolver;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Projection based constraint solver based on
    /// https://www.cs.utah.edu/~ladislav/bouaziz14projective/bouaziz14projective.pdf
    /// http://lgg.epfl.ch/publications/2015/ShapeOp/ShapeOp_DMSC15.pdf
    /// </summary>
    /// <typeparam name="P"></typeparam>
    public class ConstraintSolver<P>
        where P : IParticle
    {
        private Settings _settings = new Settings();
        private double _maxDelta = double.MaxValue;
        private double _maxAngleDelta = double.MaxValue;
        private int _stepCount = 0;


        /// <summary>
        /// 
        /// </summary>
        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value ?? throw new ArgumentNullException(); }
        }


        /// <summary>
        /// 
        /// </summary>
        public int StepCount
        {
            get { return _stepCount; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsConverged
        {
            get { return _maxDelta < _settings.ToleranceSqr && _maxAngleDelta < _settings.AngularToleranceSqr; }
        }


        /// <summary>
        /// 
        /// </summary>
        public ConstraintSolver()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public ConstraintSolver(Settings settings)
        {
            Settings = settings;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        public void Step(IReadOnlyList<P> particles, IReadOnlyList<IConstraint<P>> constraints)
        {
            LocalStep(particles, constraints);
            GlobalStep(particles, constraints);
            UpdateParticles(particles);
            _stepCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        public void StepParallel(IReadOnlyList<P> particles, IReadOnlyList<IConstraint<P>> constraints)
        {
            LocalStepParallel(particles, constraints);
            GlobalStep(particles, constraints);
            UpdateParticlesParallel(particles);
            _stepCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        private void LocalStep(IReadOnlyList<P> particles, IReadOnlyList<IConstraint<P>> constraints)
        {
            for (int i = 0; i < constraints.Count; i++)
                constraints[i].Calculate(particles);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        private void LocalStepParallel(IReadOnlyList<P> particles, IReadOnlyList<IConstraint<P>> constraints)
        {
            Parallel.ForEach(Partitioner.Create(0, constraints.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    constraints[i].Calculate(particles);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        private void GlobalStep(IReadOnlyList<P> particles, IReadOnlyList<IConstraint<P>> constraints)
        {
            for (int i = 0; i < constraints.Count; i++)
                constraints[i].Apply(particles);
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateParticles(IReadOnlyList<P> particles)
        {
            _maxDelta = 0.0;
            _maxAngleDelta = 0.0;

            var timeStep = _settings.TimeStep;
            var damp = _settings.Damping;
            var dampAng = _settings.AngularDamping;

            for (int i = 0; i < particles.Count; i++)
            {
                var p = particles[i];
                _maxDelta = Math.Max(_maxDelta, p.UpdatePosition(timeStep, damp));
                _maxAngleDelta = Math.Max(_maxAngleDelta, p.UpdateRotation(timeStep, dampAng));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateParticlesParallel(IReadOnlyList<P> particles)
        {
            _maxDelta = 0.0;
            _maxAngleDelta = 0.0;

            var locker = new object();
            var timeStep = _settings.TimeStep;
            var damp = _settings.Damping;
            var dampAng = _settings.AngularDamping;

            Parallel.ForEach(Partitioner.Create(0, particles.Count), range =>
            {
                double dpMax = 0.0;
                double daMax = 0.0;

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var p = particles[i];
                    dpMax = Math.Max(dpMax, p.UpdatePosition(timeStep, damp));
                    daMax = Math.Max(daMax, p.UpdateRotation(timeStep, dampAng));
                }

                // update max delta
                if (dpMax > _maxDelta)
                {
                    lock (locker)
                        _maxDelta = dpMax;
                }

                // update max angle delta
                if(daMax > _maxAngleDelta)
                {
                    lock (locker)
                        _maxAngleDelta = daMax;
                }
            });
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public static class ConstraintSolver
    {
        /// <summary>
        /// 
        /// </summary>
        public class Settings
        {
            private double _timeStep = 1.0;
            private double _damping = 0.9;
            private double _angleDamping = 0.9;
            private double _tolerance = 1.0e-4;
            private double _angleTolerance = 1.0e-4;


            /// <summary>
            /// 
            /// </summary>
            public Settings()
            {
            }


            /// <summary>
            /// 
            /// </summary>
            public double TimeStep
            {
                get { return _timeStep; }
                set
                {
                    if (value < 0.0)
                        throw new ArgumentOutOfRangeException("Time step cannot be negative.");

                    _timeStep = value;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            public double Damping
            {
                get { return _damping; }
                set
                {
                    if (value < 0.0 || value > 1.0)
                        throw new ArgumentOutOfRangeException("Damping must be between 0.0 and 1.0.");

                    _damping = value;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            public double AngularDamping
            {
                get { return _angleDamping; }
                set
                {
                    if (value < 0.0 || value > 1.0)
                        throw new ArgumentOutOfRangeException("The value must be between 0.0 and 1.0.");

                    _angleDamping = value;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            public double Tolerance
            {
                get { return _tolerance; }
                set
                {
                    if (value < 0.0)
                        throw new ArgumentOutOfRangeException("The value cannot be negative.");

                    _tolerance = value;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            public double AngularTolerance
            {
                get { return _angleTolerance; }
                set
                {
                    if (value < 0.0)
                        throw new ArgumentOutOfRangeException("The value cannot be negative.");

                    _angleTolerance = value;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            internal double ToleranceSqr
            {
                get { return _tolerance * _tolerance; }
            }


            /// <summary>
            /// 
            /// </summary>
            internal double AngularToleranceSqr
            {
                get { return _angleTolerance * _angleTolerance; }
            }
        }
    }
}
