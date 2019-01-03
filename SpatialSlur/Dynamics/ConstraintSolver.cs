
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

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// Position-based constraint solver for shape optimization and exploration.
    /// </summary>
    [Serializable]
    public class ConstraintSolver
    {
        private (Vector3d Delta, double Weight)[] _linearCorrectSums = Array.Empty<(Vector3d, double)>();
        private (Vector3d Delta, double Weight)[] _angularCorrectSums = Array.Empty<(Vector3d, double)>();
        private Vector3d[] _forceSums = Array.Empty<Vector3d>();
        private Vector3d[] _torqueSums = Array.Empty<Vector3d>();

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
            var positions = particles.Positions;
            var rotations = particles.Rotations;

            EnsureCapacity(positions);
            EnsureCapacity(rotations);

            Predict(positions, _settings.TimeStep, _settings.LinearDamping, parallel);
            Predict(rotations, _settings.TimeStep, _settings.AngularDamping, parallel);

            // Apply constraints
            foreach (var grp in constraints)
                grp.Apply(positions, rotations, _linearCorrectSums, _angularCorrectSums);

            Correct(positions, _linearCorrectSums, _settings.TimeStep, parallel);
            Correct(rotations, _angularCorrectSums, _settings.TimeStep, parallel);

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
            var positions = particles.Positions;
            var rotations = particles.Rotations;

            EnsureCapacity(positions);
            EnsureCapacity(rotations);

            // Apply external forces
            foreach (var grp in forces)
                grp.Apply(positions, rotations, _forceSums, _torqueSums);

            Predict(positions, _forceSums, _settings.TimeStep, _settings.LinearDamping, parallel);
            Predict(rotations, _torqueSums, _settings.TimeStep, _settings.AngularDamping, parallel);

            // Apply constraints
            foreach (var grp in constraints)
                grp.Apply(positions, rotations, _linearCorrectSums, _angularCorrectSums);

            Correct(positions, _linearCorrectSums, _settings.TimeStep, parallel);
            Correct(rotations, _angularCorrectSums, _settings.TimeStep, parallel);

            _stepCount++;
        }


        /// <summary>
        /// Resizes buffers if necessary
        /// </summary>
        private void EnsureCapacity(ArrayView<ParticlePosition> positions)
        {
            if (positions.Count > _linearCorrectSums.Length)
            {
                int newSize = positions.Source.Length;
                _linearCorrectSums = new(Vector3d, double)[newSize];
                _forceSums = new Vector3d[newSize];
            }
        }


        /// <summary>
        /// Resizes buffers if necessary
        /// </summary>
        private void EnsureCapacity(ArrayView<ParticleRotation> rotations)
        {
            if (rotations.Count > _angularCorrectSums.Length)
            {
                int newSize = rotations.Source.Length;
                _angularCorrectSums = new(Vector3d, double)[newSize];
                _torqueSums = new Vector3d[newSize];
            }
        }


        /// <summary>
        /// Updates via explicit integration to get an approximate prediction of the future state
        /// </summary>
        private static void Predict(
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
        /// Updates via explicit integration to get an approximate prediction of the future state
        /// </summary>
        private static void Predict(
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
        /// Updates via explicit integration to get an approximate prediction of the future state
        /// </summary>
        private static void Predict(
            ArrayView<ParticlePosition> positions,
            ArrayView<Vector3d> forceSums,
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
                    p.Velocity = p.Velocity * k + forceSums[i] * (p.InverseMass * timeStep);
                    p.Current += p.Velocity * timeStep;
                    forceSums[i] = Vector3d.Zero;
                }
            }
        }


        /// <summary>
        /// Updates via explicit integration to get an approximate prediction of the future state
        /// </summary>
        private static void Predict(
            ArrayView<ParticleRotation> rotations,
            ArrayView<Vector3d> torqueSums,
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
                    r.Velocity = r.Velocity * k + m.Apply(r.InverseInertia * m.ApplyTranspose(torqueSums[i])) * timeStep;
                    r.Current = new Quaterniond(r.Velocity * timeStep) * r.Current;
                    torqueSums[i] = Vector3d.Zero;
                }
            }
        }


        /// <summary>
        /// Corrects the predicted future state via a regularized Jacobi scheme
        /// </summary>
        private static void Correct(
            ArrayView<ParticlePosition> positions,
            ArrayView<(Vector3d, double)> correctSums,
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
                    (var d, var w) = correctSums[i];

                    if (w > 0.0)
                    {
                        d /= w;
                        ref var p = ref positions[i];
                        p.Current += d;
                        p.Velocity += d * dtInv;
                    }

                    correctSums[i] = (Vector3d.Zero, 0.0);
                }
            }
        }


        /// <summary>
        /// Corrects the predicted future state via a regularized Jacobi scheme
        /// </summary>
        private static void Correct(
            ArrayView<ParticleRotation> rotations, 
            ArrayView<(Vector3d, double)> correctSums, 
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
                    (var d, var w) = correctSums[i];

                    if (w > 0.0)
                    {
                        d /= w;
                        ref var r = ref rotations[i];
                        r.Current = new Quaterniond(d) * r.Current;
                        r.Velocity += d * dtInv;
                    }

                    correctSums[i] = (Vector3d.Zero, 0.0);
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
