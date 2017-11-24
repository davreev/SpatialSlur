using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurDynamics;
using SpatialSlur.SlurDynamics.Constraints;

/*
 * Notes
 */

namespace SpatialSlur.Examples
{
    /// <summary>
    /// Planarizes a randomly generated quadrilateral.
    /// </summary>
    static class PlanarizeQuad
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Start()
        {
            var random = new Random(0);
            var box = new Interval3d(new Vec3d(0.0), new Vec3d(10.0)); // create a interval between the (0,0,0) and (10,10,10)

            // create particles
            var particles = new Particle[] {
                new Particle(random.NextVec3d(box)),
                new Particle(random.NextVec3d(box)),
                new Particle(random.NextVec3d(box)),
                new Particle(random.NextVec3d(box))
            };

            // create constraints
            var constraints = new IConstraint[] {
                new PlanarQuad(0, 1, 2, 3)
            };

            // create solver
            var solver = new ConstraintSolver();
     
            // wait for keypress to start the solver
            Console.WriteLine("Press return to start the solver.");
            Console.ReadLine();

            // step the solver until converged
            while (!solver.IsConverged)
            {
                solver.Step(particles, constraints);
                Console.WriteLine($"    step {solver.StepCount}");
            }

            Console.WriteLine("\nSolver converged! Press return to exit.");
            Console.ReadLine();
        }
    }
}
