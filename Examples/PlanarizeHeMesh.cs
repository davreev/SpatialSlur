
/*
 * Notes
 */

using System;
using System.Linq;
using SpatialSlur;
using SpatialSlur.Meshes;
using SpatialSlur.Dynamics;
using SpatialSlur.Dynamics.Constraints;

namespace SpatialSlur.Examples
{
    /// <summary>
    /// 
    /// </summary>
    static class PlanarizeHeMesh
    {
        const string _fileIn = "chair_smooth_0.obj";
        const string _fileOut = "chair_planar_0.obj";


        /// <summary>
        /// 
        /// </summary>
        public static void Start()
        {
            // import halfedge mesh
            var mesh = HeMesh3d.Factory.CreateFromOBJ(Paths.Resources + _fileIn);

            // create particles
            var bodies = mesh.Vertices.Select(v => new Body(v.Position)).ToArray();

            // create constraints
            var constraints = mesh.Faces.Where(f => !f.IsDegree3)
                .Select(f => new Coplanar(f.Vertices.Select(v => v.Index)))
                .ToArray();

            // create solver
            var solver = new ConstraintSolver();
            solver.Settings.LinearDamping = 0.1;

            // wait for keypress to start the solver
            Console.WriteLine("Press return to start the solver.");
            Console.ReadLine();

            // step the solver until converged
            while (!solver.IsConverged)
            {
                solver.Step(bodies, constraints, true);
                Console.WriteLine($"    step {solver.StepCount}");
            }
            Console.WriteLine("\nSolver converged! Press return to exit.");

            // update mesh vertices
            mesh.Vertices.Action(v => v.Position = bodies[v].Position.Current); // mesh elements (vertices, halfedges, faces) are implicitly converted to their index

            // compute vertex normals & write to file
            mesh.Vertices.Action(v => v.Normal = v.GetNormal(), true);
            Interop.Meshes.WriteToObj(mesh, Paths.Resources + _fileOut);
            Console.ReadLine();
        }
    }
}
