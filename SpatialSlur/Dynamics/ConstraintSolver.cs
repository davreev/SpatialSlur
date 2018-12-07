
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

namespace SpatialSlur.Dynamics.WIP
{
    /// <summary>
    /// Position-based constraint solver for shape optimization and exploration.
    /// </summary>
    [Serializable]
    public class ConstraintSolver
    {
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
        public void Step(ReadOnlyArrayView<Particle> particles, ConstraintGroup constraints, bool parallel = false)
        {
            Step(particles, constraints.Yield(), parallel);
        }


        /// <summary>
        /// Updates the given particles while attempting to preserve the given constraints.
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraintGroups"></param>
        /// <param name="parallel"></param>
        public void Step(ReadOnlyArrayView<Particle> particles, IEnumerable<ConstraintGroup> constraintGroups, bool parallel = false)
        {
            UpdateNoForce(particles, parallel);

            // Apply constraints
            foreach (var grp in constraintGroups)
                grp.Apply(particles);

            Correct(particles, parallel);
            _stepCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraints"></param>
        /// <param name="forces"></param>
        /// <param name="parallel"></param>
        public void Step(ReadOnlyArrayView<Particle> particles, ConstraintGroup constraints, ForceGroup forces, bool parallel = false)
        {
            Step(particles, constraints.Yield(), forces.Yield(), parallel);
        }


        /// <summary>
        /// Updates the given particles while attempting to preserve the given constraints.
        /// Also considers the application of external forces.
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="constraintGroups"></param>
        /// <param name="forceGroups"></param>
        /// <param name="parallel"></param>
        public void Step(ReadOnlyArrayView<Particle> particles, IEnumerable<ConstraintGroup> constraintGroups, IEnumerable<ForceGroup> forceGroups, bool parallel = false)
        {
            // Apply external forces
            foreach (var grp in forceGroups)
                grp.Apply(particles);

            Update(particles, parallel);

            // Apply constraints
            foreach (var grp in constraintGroups)
                grp.Apply(particles);

            Correct(particles, parallel);
            _stepCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateNoForce(ReadOnlyArrayView<Particle> particles, bool parallel)
        {
            var dt = Settings.TimeStep;
            var linScale = 1.0 - Settings.LinearDamping;
            var angScale = 1.0 - Settings.AngularDamping;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, particles.Count), range => Update(range.Item1, range.Item2));
            else
                Update(0, particles.Count);

            // Update via explicit integration to get predicted future state
            void Update(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    (var p, var r) = particles[i];

                    // position update
                    if (p != null)
                    {
                        var v = p.Velocity * linScale;
                        p.Current += v * dt;
                        p.Velocity = v;
                        p.ForceSum = Vector3d.Zero;
                    }

                    // rotation update
                    if (r != null)
                    {
                        var v = r.Velocity * angScale;
                        r.Current = new Quaterniond(v * dt) * r.Current;
                        r.Velocity = v;
                        r.TorqueSum = Vector3d.Zero;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void Update(ReadOnlyArrayView<Particle> particles, bool parallel)
        {
            var dt = Settings.TimeStep;
            var linScale = 1.0 - Settings.LinearDamping;
            var angScale = 1.0 - Settings.AngularDamping;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, particles.Count), range => Update(range.Item1, range.Item2));
            else
                Update(0, particles.Count);

            // Update via explicit integration to get predicted future state
            void Update(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    (var p, var r) = particles[i];

                    // position update
                    if (p != null)
                    {
                        var v = p.Velocity * linScale + p.ForceSum * (p.MassInv * dt);
                        p.Current += v * dt;
                        p.Velocity = v;
                        p.ForceSum = Vector3d.Zero;
                    }

                    // rotation update
                    if (r != null)
                    {
                        var m = r.Current.ToMatrix();
                        var v = r.Velocity * angScale + m.Apply(r.InertiaInv * m.ApplyTranspose(r.TorqueSum)) * dt;
                        r.Current = new Quaterniond(v * dt) * r.Current;
                        r.Velocity = v;
                        r.TorqueSum = Vector3d.Zero;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void Correct(ReadOnlyArrayView<Particle> particles, bool parallel)
        {
            var dtInv = 1.0 / Settings.TimeStep;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, particles.Count), range => Correct(range.Item1, range.Item2));
            else
                Correct(0, particles.Count);

            // Correct predicted state via regularized Jacobi scheme
            void Correct(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    (var p, var r) = particles[i];

                    // position correction
                    if (p != null)
                    {
                        var w = p.WeightSum;

                        if (w > 0.0)
                        {
                            var d = p.DeltaSum / w;
                            p.Current += d;
                            p.Velocity += d * dtInv;
                        }

                        p.DeltaSum = Vector3d.Zero;
                        p.WeightSum = 0.0;
                    }

                    // rotation correction
                    if (r != null)
                    {
                        var w = r.WeightSum;

                        if (w > 0.0)
                        {
                            var d = r.DeltaSum / w;
                            r.Current = new Quaterniond(d) * r.Current;
                            r.Velocity += d * dtInv;
                        }

                        r.DeltaSum = Vector3d.Zero;
                        r.WeightSum = 0.0;
                    }
                }
            }
        }


        /// <summary>
        /// Returns true if the given particles have converged within tolerance.
        /// </summary>
        /// <returns></returns>
        public bool HaveConverged(IEnumerable<Particle> particles)
        {
            var dtInv = 1.0 / _settings.TimeStep;
            var linTolSqr = Square(_settings.LinearTolerance * dtInv);
            var angTolSqr = Square(_settings.AngularTolerance * dtInv);

            double Square(double x) { return x * x; }

            foreach ((var p, var r) in particles)
            {
                if (p != null && p.Velocity.SquareLength > linTolSqr)
                    return false;

                if (r != null && r.Velocity.SquareLength > angTolSqr)
                    return false;
            }

            return true;
        }
    }
}
