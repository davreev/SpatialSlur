using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurDynamics;
using SpatialSlur.SlurDynamics.Constraints;

/*
 * Notes
 */

namespace SpatialSlur.Examples
{
    /// <summary>
    /// 
    /// </summary>
    static class PlanarizeHeMesh
    {
        const string _fileIn = "chair_smooth.obj";
        const string _fileOut = "chair_planar.obj";


        /// <summary>
        /// 
        /// </summary>
        public static void Start()
        {
            // import halfedge mesh
            var mesh = HeMesh3d.Factory.CreateFromOBJ(Paths.Resources + _fileIn);

            // create particles
            var particles = mesh.Vertices.Select(v => new Particle(v.Position)).ToArray();

            // create constraints
            var constraints = mesh.Faces.Where(f => !f.IsDegree3)
                .Select(f => new PlanarNgon(f.Vertices.Select(v => v.Index)))
                .ToArray();

            // create solver
            var solver = new ConstraintSolver();

            // wait for keypress to start the solver
            Console.WriteLine("Press return to start the solver.");
            Console.ReadLine();

            // step the solver until converged
            while (!solver.IsConverged)
            {
                solver.Step(particles, constraints, true);
                Console.WriteLine($"    step {solver.StepCount}");
            }
            Console.WriteLine("\nSolver converged! Press return to exit.");

            // update mesh vertices
            mesh.Vertices.Action(v => v.Position = particles[v.Index].Position);
            // mesh.Vertices.Action(v => v.Position = particles[v].Position); // mesh elements (vertices, halfedges, faces) are converted to their indices implicitly so this also works

            // compute vertex normals & write to file
            mesh.GetVertexNormals(v => v.Position, (v, n) => v.Normal = n, true);
            mesh.WriteToOBJ(Paths.Resources + _fileOut);
            Console.ReadLine();
        }
    }
}
