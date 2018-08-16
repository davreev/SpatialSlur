
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

using SpatialSlur.Collections;
using SpatialSlur.Dynamics.Constraints;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// Projection based constraint solver for geometry optimization and form-finding based on method described in http://lgg.epfl.ch/publications/2012/shapeup/paper.pdf.
    /// </summary>
    [Serializable]
    public class ConstraintSolver
    {
        private ConstraintSolverSettings _settings = new ConstraintSolverSettings();
        private double _maxLinearSpeedSqr = double.MaxValue;
        private double _maxAngularSpeedSqr = double.MaxValue;
        private int _stepCount = 0;


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
        /// <returns></returns>
        public bool IsConverged
        {
            get
            {
                var dtInv = 1.0 / _settings.TimeStep;
                var linTol = _settings.LinearTolerance * dtInv;
                var angTol = _settings.AngularTolerance * dtInv;

                return 
                    _maxLinearSpeedSqr < linTol * linTol && 
                    _maxAngularSpeedSqr < angTol * angTol;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="constraints"></param>
        /// <param name="parallel"></param>
        public void Step(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<IConstraint> constraints, bool parallel = false)
        {
            if (parallel)
            {
                ApplyConstraintsParallel(bodies, constraints);
                UpdateBodiesParallel(bodies);
            }
            else
            {
                ApplyConstraints(bodies, constraints);
                UpdateBodies(bodies);
            }

            _stepCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="constraints"></param>
        private void ApplyConstraints(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<IConstraint> constraints)
        {
            for (int i = 0; i < constraints.Count; i++)
                constraints[i].Calculate(bodies);

            for (int i = 0; i < constraints.Count; i++)
                constraints[i].Apply(bodies);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="constraints"></param>
        private void ApplyConstraintsParallel(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<IConstraint> constraints)
        {
            Parallel.ForEach(Partitioner.Create(0, constraints.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    constraints[i].Calculate(bodies);
            });

            for (int i = 0; i < constraints.Count; i++)
                constraints[i].Apply(bodies);
        }
        

        /// <summary>
        /// 
        /// </summary>
        private void UpdateBodies(ReadOnlyArrayView<Body> bodies)
        {
            _maxLinearSpeedSqr = _maxAngularSpeedSqr = 0.0;

            var timeStep = _settings.TimeStep;
            var linDamp = _settings.LinearDamping;
            var angDamp = _settings.AngularDamping;

            for (int i = 0; i < bodies.Count; i++)
            {
                (var bp, var br) = bodies[i];

                if (bp != null)
                {
                    bp.Update(timeStep, linDamp);
                    _maxLinearSpeedSqr = Math.Max(bp.Velocity.SquareLength, _maxLinearSpeedSqr);
                }

                if (br != null)
                {
                    br.Update(timeStep, angDamp);
                    _maxAngularSpeedSqr = Math.Max(br.Velocity.SquareLength, _maxAngularSpeedSqr);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateBodiesParallel(ReadOnlyArrayView<Body> bodies)
        {
            _maxLinearSpeedSqr = _maxAngularSpeedSqr = 0.0;

            Parallel.ForEach(Partitioner.Create(0, bodies.Count), range =>
            {
                var timeStep = _settings.TimeStep;
                var linDamp = _settings.LinearDamping;
                var angDamp = _settings.AngularDamping;
                var linMax = 0.0;
                var angMax = 0.0;

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    (var bp, var br) = bodies[i];

                    if (bp != null)
                    {
                        bp.Update(timeStep, linDamp);
                        linMax = Math.Max(bp.Velocity.SquareLength, linMax);
                    }

                    if (br != null)
                    {
                        br.Update(timeStep, angDamp);
                        angMax = Math.Max(br.Velocity.SquareLength, angMax);
                    }
                }
                
                if (linMax > _maxLinearSpeedSqr)
                    Interlocked.Exchange(ref _maxLinearSpeedSqr, linMax);

                if (angMax > _maxAngularSpeedSqr)
                    Interlocked.Exchange(ref _maxAngularSpeedSqr, angMax);
            });
        }
    }
}
