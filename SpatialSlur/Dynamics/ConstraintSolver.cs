
/*
 * Notes
 * 
 * Impl refs
 * https://www.youtube.com/watch?v=fH3VW9SaQ_c&index=6&list=PL_a9tY9IhJuPuw5nu-WU7mG8T8MiX4JnY
 * https://drive.google.com/file/d/1BX8as-MEemWBgDrViegejsLLJKIiVRU-/view?usp=sharing
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

using SpatialSlur;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.WIP
{
    /// <summary>
    /// Position-based constraint solver for shape optimization and exploration.
    /// </summary>
    [Serializable]
    public class ConstraintSolver
    {
        private (Vector3d Delta, double Weight)[] _linearSum;
        private (Vector3d Delta, double Weight)[] _angularSum;
        private Vector3d[] _forceSum;
        private Vector3d[] _torqueSum;

        private ConstraintSolverSettings _settings = new ConstraintSolverSettings();
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
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        /// <param name="parallel"></param>
        public void Step(
            ParticleBuffer particles,
            ConstraintGroup constraints,
            bool parallel = false)
        {
            Step(particles, constraints.Yield(), parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        /// <param name="parallel"></param>
        public void Step(
            ParticleBuffer particles,
            IEnumerable<ConstraintGroup> constraints,
            bool parallel = false)
        {
            // TODO
            // Ensure delta/force buffers are large enough
            
            Update(particles.Positions, _settings.TimeStep, _settings.LinearDamping, parallel);
            Update(particles.Rotations, _settings.TimeStep, _settings.AngularDamping, parallel);

            // Apply constraints
            foreach (var grp in constraints)
                grp.Apply(particles, _linearSum, _angularSum);

            Correct(particles.Positions, _linearSum, _settings.TimeStep, parallel);
            Correct(particles.Rotations, _angularSum, _settings.TimeStep, parallel);

            _stepCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        /// <param name="forces"></param>
        /// <param name="parallel"></param>
        public void Step(
            ParticleBuffer particles, 
            ConstraintGroup constraints, 
            ForceGroup forces, 
            bool parallel = false)
        {
            Step(particles, constraints.Yield(), forces.Yield(), parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        /// <param name="forces"></param>
        /// <param name="parallel"></param>
        public void Step(
            ParticleBuffer particles, 
            IEnumerable<ConstraintGroup> constraints, 
            IEnumerable<ForceGroup> forces, 
            bool parallel = false)
        {
            // TODO
            // Ensure delta/force buffers are large enough

            // Apply external forces
            foreach (var grp in forces)
                grp.Apply(particles, _forceSum, _torqueSum);

            Update(particles.Positions, _forceSum, _settings.TimeStep, _settings.LinearDamping, parallel);
            Update(particles.Rotations, _torqueSum, _settings.TimeStep, _settings.AngularDamping, parallel);

            // Apply constraints
            foreach (var grp in constraints)
                grp.Apply(particles, _linearSum, _angularSum);

            Correct(particles.Positions, _linearSum, _settings.TimeStep, parallel);
            Correct(particles.Rotations, _angularSum, _settings.TimeStep, parallel);

            _stepCount++;
        }


        /// <summary>
        /// Updates via explicit integration for an approximation of the future state
        /// </summary>
        private static void Update(
            ArrayView<ParticlePosition> positions,
            double timeStep,
            double damping,
            bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, positions.Count), range => Update(range.Item1, range.Item2));
            else
                Update(0, positions.Count);

            void Update(int from, int to)
            {
                var k = 1.0 - damping;

                for (int i = from; i < to; i++)
                {
                    ref var p = ref positions[i];
                    p.Velocity *= k;
                    p.Current += p.Velocity * timeStep;
                }
            }
        }


        /// <summary>
        /// Updates via explicit integration for an approximation of the future state
        /// </summary>
        private static void Update(
            ArrayView<ParticleRotation> rotations,
            double timeStep,
            double damping,
            bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, rotations.Count), range => Update(range.Item1, range.Item2));
            else
                Update(0, rotations.Count);

            void Update(int from, int to)
            {
                var k = 1.0 - damping;

                for (int i = from; i < to; i++)
                {
                    ref var r = ref rotations[i];
                    r.Velocity *= k;
                    r.Current = new Quaterniond(r.Velocity * timeStep) * r.Current;
                }
            }
        }


        /// <summary>
        /// Updates via explicit integration for an approximation of the future state
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="forceSum"></param>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        /// <param name="parallel"></param>
        private static void Update(
            ArrayView<ParticlePosition> positions,
            Vector3d[] forceSum,
            double timeStep,
            double damping,
            bool parallel
            )
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, positions.Count), range => Update(range.Item1, range.Item2));
            else
                Update(0, positions.Count);

            void Update(int from, int to)
            {
                var k = 1.0 - damping;

                for (int i = from; i < to; i++)
                {
                    ref var p = ref positions[i];
                    p.Velocity = p.Velocity * k + forceSum[i] * (p.MassInv * timeStep);
                    p.Current += p.Velocity * timeStep;
                    forceSum[i] = Vector3d.Zero;
                }
            }
        }


        /// <summary>
        /// Updates via explicit integration for an approximation of the future state
        /// </summary>
        private static void Update(
            ArrayView<ParticleRotation> rotations,
            Vector3d[] torqueSum,
            double timeStep,
            double damping,
            bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, rotations.Count), range => Update(range.Item1, range.Item2));
            else
                Update(0, rotations.Count);

            void Update(int from, int to)
            {
                var k = 1.0 - damping;

                for (int i = from; i < to; i++)
                {
                    ref var r = ref rotations[i];
                    var m = r.Current.ToMatrix();
                    r.Velocity = r.Velocity * k + m.Apply(r.InertiaInv * m.ApplyTranspose(torqueSum[i])) * timeStep;
                    r.Current = new Quaterniond(r.Velocity * timeStep) * r.Current;
                    torqueSum[i] = Vector3d.Zero;
                }
            }
        }


        /// <summary>
        /// Corrects the predicted future state via a regularized Jacobi scheme
        /// </summary>
        private static void Correct(
            ArrayView<ParticlePosition> positions,
            ArrayView<(Vector3d, double)> deltaSum,
            double timeStep,
            bool parallel)
        {
            var dtInv = 1.0 / timeStep;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, positions.Count), range => Correct(range.Item1, range.Item2));
            else
                Correct(0, positions.Count);
            
            void Correct(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    (var d, var w) = deltaSum[i];

                    if (w > 0.0)
                    {
                        d /= w;
                        ref var p = ref positions[i];
                        p.Current += d;
                        p.Velocity += d * dtInv;
                    }

                    deltaSum[i] = (Vector3d.Zero, 0.0);
                }
            }
        }


        /// <summary>
        /// Corrects the predicted future state via a regularized Jacobi scheme
        /// </summary>
        private static void Correct(
            ArrayView<ParticleRotation> rotations, 
            ArrayView<(Vector3d, double)> deltaSum, 
            double timeStep, 
            bool parallel)
        {
            var dtInv = 1.0 / timeStep;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, rotations.Count), range => Correct(range.Item1, range.Item2));
            else
                Correct(0, rotations.Count);
            
            void Correct(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    (var d, var w) = deltaSum[i];

                    if (w > 0.0)
                    {
                        d /= w;
                        ref var r = ref rotations[i];
                        r.Current = new Quaterniond(d) * r.Current;
                        r.Velocity += d * dtInv;
                    }

                    deltaSum[i] = (Vector3d.Zero, 0.0);
                }
            }
        }


        /// <summary>
        /// Returns true if the velocities of all given particles are within the current tolerance threshold
        /// </summary>
        public bool HasConverged(ParticleBuffer particles)
        {
            var dtInv = 1.0 / _settings.TimeStep;

            return
                HasConverged(particles.Positions, _settings.LinearTolerance * dtInv) &&
                HasConverged(particles.Rotations, _settings.AngularTolerance * dtInv);
        }


        /// <summary>
        /// 
        /// </summary>
        private static bool HasConverged(
            ArrayView<ParticlePosition> positions, 
            double tolerance)
        {
            var tolSqr = tolerance * tolerance;

            for(int i = 0; i < positions.Count; i++)
            {
                if (positions[i].Velocity.SquareLength > tolSqr)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        private static bool HasConverged(
            ArrayView<ParticleRotation> rotations, 
            double tolerance)
        {
            var tolSqr = tolerance * tolerance;

            for (int i = 0; i < rotations.Count; i++)
            {
                if (rotations[i].Velocity.SquareLength > tolSqr)
                    return false;
            }

            return true;
        }
    }
}
