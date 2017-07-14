using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurDynamics;

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
    [Serializable]
    public class ConstraintSolver
    {
        private ConstraintSolverSettings _settings = new ConstraintSolverSettings();
        private double _maxDelta = double.MaxValue;
        private double _maxAngleDelta = double.MaxValue;
        private int _stepCount = 0;


        /// <summary>
        /// 
        /// </summary>
        public ConstraintSolverSettings Settings
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
        public ConstraintSolver(ConstraintSolverSettings settings)
        {
            Settings = settings;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        public void Step(IReadOnlyList<IBody> particles, IReadOnlyList<IConstraint> constraints)
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
        public void StepParallel(IReadOnlyList<IBody> particles, IReadOnlyList<IConstraint> constraints)
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
        private void LocalStep(IReadOnlyList<IBody> particles, IReadOnlyList<IConstraint> constraints)
        {
            for (int i = 0; i < constraints.Count; i++)
                constraints[i].Calculate(particles);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        private void LocalStepParallel(IReadOnlyList<IBody> particles, IReadOnlyList<IConstraint> constraints)
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
        private void GlobalStep(IReadOnlyList<IBody> particles, IReadOnlyList<IConstraint> constraints)
        {
            for (int i = 0; i < constraints.Count; i++)
                constraints[i].Apply(particles);
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateParticles(IReadOnlyList<IBody> particles)
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
        private void UpdateParticlesParallel(IReadOnlyList<IBody> particles)
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
}
