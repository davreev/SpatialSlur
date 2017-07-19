using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurDynamics;


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
        const string FileIn = "chair_smooth.obj";
        const string FileOut = "chair_planar.obj";

        /// <summary>
        /// 
        /// </summary>
        public static void Run()
        {
            var mesh = HeMesh3d.Factory.CreateFromOBJ(Paths.Resources + FileIn);
            var verts = mesh.Vertices;
            var faces = mesh.Halfedges;

            // create particles
            var particles = mesh.Vertices.Select(v => new Particle(v.Position)).ToArray();

            // create constraints
            var constraints = mesh.Faces.Where(f => !f.IsTriangle)
                .Select(f => new PlanarNgon(f.Vertices
                .Select(v => v.Index))).ToArray();

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

            // update mesh vertices
            verts.Action(v => v.Position = particles[v.Index].Position);
            // verts.Action(v => v.Position = particles[v].Position); // mesh elements (vertices, halfedges, faces) are converted to their indices implicitly so this also works

            // compute vertex normals & write to file
            mesh.GetVertexNormals(v => v.Position, (v, n) => v.Normal = n, true);
            mesh.WriteToOBJ(Paths.Resources + FileOut);
            Console.ReadLine();
        }
    }
}
