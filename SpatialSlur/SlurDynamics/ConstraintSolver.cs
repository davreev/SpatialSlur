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
    /// Projection based constraint solver for geometry optimization and form-finding. 
    /// This is an implementation of principles described in http://lgg.epfl.ch/publications/2012/shapeup/paper.pdf and follows many of the implementation details given in http://lgg.epfl.ch/publications/2015/ShapeOp/ShapeOp_DMSC15.pdf.
    /// </summary>
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
        /// <param name="bodies"></param>
        /// <param name="constraints"></param>
        public void Step(IReadOnlyList<IBody> bodies, IReadOnlyList<IConstraint> constraints)
        {
            LocalStep(bodies, constraints);
            GlobalStep(bodies, constraints);
            UpdateBodies(bodies);
            _stepCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="constraints"></param>
        public void StepParallel(IReadOnlyList<IBody> bodies, IReadOnlyList<IConstraint> constraints)
        {
            LocalStepParallel(bodies, constraints);
            GlobalStep(bodies, constraints);
            UpdateBodiesParallel(bodies);
            _stepCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="constraints"></param>
        private void LocalStep(IReadOnlyList<IBody> bodies, IReadOnlyList<IConstraint> constraints)
        {
            for (int i = 0; i < constraints.Count; i++)
                constraints[i].Calculate(bodies);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="constraints"></param>
        private void LocalStepParallel(IReadOnlyList<IBody> bodies, IReadOnlyList<IConstraint> constraints)
        {
            Parallel.ForEach(Partitioner.Create(0, constraints.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    constraints[i].Calculate(bodies);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="constraints"></param>
        private void GlobalStep(IReadOnlyList<IBody> bodies, IReadOnlyList<IConstraint> constraints)
        {
            for (int i = 0; i < constraints.Count; i++)
                constraints[i].Apply(bodies);
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateBodies(IReadOnlyList<IBody> bodies)
        {
            _maxDelta = 0.0;
            _maxAngleDelta = 0.0;

            var timeStep = _settings.TimeStep;
            var damp = _settings.Damping;
            var dampAng = _settings.AngularDamping;

            for (int i = 0; i < bodies.Count; i++)
            {
                var p = bodies[i];
                _maxDelta = Math.Max(_maxDelta, p.UpdatePosition(timeStep, damp));
                _maxAngleDelta = Math.Max(_maxAngleDelta, p.UpdateRotation(timeStep, dampAng));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateBodiesParallel(IReadOnlyList<IBody> bodies)
        {
            _maxDelta = 0.0;
            _maxAngleDelta = 0.0;

            var locker = new object();
            var timeStep = _settings.TimeStep;
            var damp = _settings.Damping;
            var dampAng = _settings.AngularDamping;

            Parallel.ForEach(Partitioner.Create(0, bodies.Count), range =>
            {
                double dpMax = 0.0;
                double daMax = 0.0;

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var p = bodies[i];
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
