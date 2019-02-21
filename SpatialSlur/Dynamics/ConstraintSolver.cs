/*
 * Notes
 * 
 * Impl refs
 * https://www.youtube.com/watch?v=fH3VW9SaQ_c&index=6&list=PL_a9tY9IhJuPuw5nu-WU7mG8T8MiX4JnY
 * https://drive.google.com/file/d/1BX8as-MEemWBgDrViegejsLLJKIiVRU-/view?usp=sharing
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

using SpatialSlur;
using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;
using static SpatialSlur.Collections.Buffer;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// Position-based constraint solver for shape optimization and exploration.
    /// </summary>
    [Serializable]
    public class ConstraintSolver
    {
        private Vector4d[] _linearCorrectSums = Array.Empty<Vector4d>(); // x,y,z = delta, w = weight
        private Vector4d[] _angularCorrectSums = Array.Empty<Vector4d>();

        private Vector3d[] _forceSums = Array.Empty<Vector3d>(); // x,y,z = delta, w = unused
        private Vector3d[] _torqueSums = Array.Empty<Vector3d>();

        private ConstraintSolverSettings _settings = new ConstraintSolverSettings();
        private bool _parallel;


        /// <summary>
        /// 
        /// </summary>
        public ConstraintSolver()
        {
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
        public bool Parallel
        {
            get => _parallel;
            set => _parallel = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Predict(ParticleBuffer particles)
        {
            Predict(particles.Positions, _settings.TimeStep, _settings.LinearDamping, _parallel);
            Predict(particles.Rotations, _settings.TimeStep, _settings.AngularDamping, _parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="forces"></param>
        public void Predict(ParticleBuffer particles, ForceGroup forces)
        {
            var positions = particles.Positions;
            var rotations = particles.Rotations;

            // Apply external forces
            ExpandToFit(ref _forceSums, positions.Count);
            ExpandToFit(ref _torqueSums, rotations.Count);
            forces.Apply(positions, rotations, _forceSums, _torqueSums);

            Predict(positions, _forceSums, _settings.TimeStep, _settings.LinearDamping, _parallel);
            Predict(rotations, _torqueSums, _settings.TimeStep, _settings.AngularDamping, _parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        public void Correct(ParticleBuffer particles, ConstraintGroup constraints)
        {
            var positions = particles.Positions;
            var rotations = particles.Rotations;

            // Apply constraints
            ExpandToFit(ref _linearCorrectSums, positions.Count);
            ExpandToFit(ref _angularCorrectSums, rotations.Count);
            constraints.Apply(positions, rotations, _linearCorrectSums, _angularCorrectSums);

            Correct(positions, _linearCorrectSums, _settings.TimeStep, _parallel);
            Correct(rotations, _angularCorrectSums, _settings.TimeStep, _parallel);
        }


        /// <summary>
        /// Predicts future positions via simple explicit integration
        /// </summary>
        private static void Predict(
            ArrayView<ParticlePosition> positions,
            double timeStep,
            double damping,
            bool parallel)
        {
            if (parallel)
                ForEach(Partitioner.Create(0, positions.Count), range => Update(range.Item1, range.Item2));
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
        /// Predicts future rotations via simple explicit integration
        /// </summary>
        private static void Predict(
            ArrayView<ParticleRotation> rotations,
            double timeStep,
            double damping,
            bool parallel)
        {
            if (parallel)
                ForEach(Partitioner.Create(0, rotations.Count), range => Update(range.Item1, range.Item2));
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
        /// Predicts future positions via simple explicit integration
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
                ForEach(Partitioner.Create(0, positions.Count), range => Update(range.Item1, range.Item2));
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
        /// Predicts future rotations via simple explicit integration
        /// </summary>
        private static void Predict(
            ArrayView<ParticleRotation> rotations,
            ArrayView<Vector3d> torqueSums,
            double timeStep,
            double damping,
            bool parallel)
        {
            if (parallel)
                ForEach(Partitioner.Create(0, rotations.Count), range => Update(range.Item1, range.Item2));
            else
                Update(0, rotations.Count);

            void Update(int from, int to)
            {
                var k = 1.0 - damping;

                for (int i = from; i < to; i++)
                {
                    ref var r = ref rotations[i];

                    var m = r.Current.ToMatrix();
                    r.Velocity = r.Velocity * k + m.Apply(r.InverseMass * m.ApplyTranspose(torqueSums[i])) * timeStep;
                    r.Current = new Quaterniond(r.Velocity * timeStep) * r.Current;

                    torqueSums[i] = Vector3d.Zero;
                }
            }
        }


        /// <summary>
        /// Corrects the predicted positions via a regularized Jacobi scheme
        /// </summary>
        private static void Correct(
            ArrayView<ParticlePosition> positions,
            ArrayView<Vector4d> correctSums,
            double timeStep,
            bool parallel)
        {
            if (parallel)
                ForEach(Partitioner.Create(0, positions.Count), range => Correct(range.Item1, range.Item2));
            else
                Correct(0, positions.Count);
            
            void Correct(int from, int to)
            {
                var dtInv = 1.0 / timeStep;

                for (int i = from; i < to; i++)
                {
                    ref var c = ref correctSums[i];

                    if (c.W > 0.0)
                    {
                        var d = c.XYZ / c.W;

                        ref var p = ref positions[i];
                        p.Current += d;
                        p.Velocity += d * dtInv;
                    }

                    correctSums[i] = Vector4d.Zero;
                }
            }
        }


        /// <summary>
        /// Corrects the predicted rotations via a regularized Jacobi scheme
        /// </summary>
        private static void Correct(
            ArrayView<ParticleRotation> rotations, 
            ArrayView<Vector4d> correctSums, 
            double timeStep, 
            bool parallel)
        {
            if (parallel)
                ForEach(Partitioner.Create(0, rotations.Count), range => Correct(range.Item1, range.Item2));
            else
                Correct(0, rotations.Count);
            
            void Correct(int from, int to)
            {
                var dtInv = 1.0 / timeStep;

                for (int i = from; i < to; i++)
                {
                    ref var c = ref correctSums[i];

                    if (c.W > 0.0)
                    {
                        var d = c.XYZ / c.W;

                        ref var r = ref rotations[i];
                        r.Current = new Quaterniond(d) * r.Current;
                        r.Velocity += d * dtInv;
                    }

                    correctSums[i] = Vector4d.Zero;
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
