using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurRhino;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class AlignVertices : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public AlignVertices()
          : base("Align Vertices", "AlignVerts",
              "Aligns nearby vertices within a mesh",
              "Mesh", "Util")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Mesh with vertices to align", GH_ParamAccess.item);
            pManager.AddNumberParameter("radius", "radius", "Vertex search radius", GH_ParamAccess.item, 0.01);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("result", "result", "Mesh with aligned vertices", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            double tol = 0.0;

            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetData(1, ref tol)) return;

            var verts = mesh.Vertices;
            var points = verts.Select(p => p.ToVec3d()).ToArray();

            Message = (points.Consolidate(tol)) ? "Converged" : "Not converged";
   
            for (int i = 0; i < verts.Count; i++)
                verts[i] = points[i].ToPoint3f();

            DA.SetData(0, new GH_Mesh(mesh));
        }


        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
        }


        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{FC8A6CF2-C16A-4A23-8012-28B9F36B1844}"); }
        }
    }
}
