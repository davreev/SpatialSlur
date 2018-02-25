using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SpatialSlur.SlurRhino;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class MeshLoftPair : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public MeshLoftPair()
          : base("Mesh Loft Pair", "LoftPair",
              "Creates a mesh between a pair of polylines",
              "Mesh", "Util")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("polylineA", "polyA", "", GH_ParamAccess.item);
            pManager.AddCurveParameter("polylineB", "polyB", "", GH_ParamAccess.item);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("result", "result", "Lofted mesh", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crvA = null;
            Curve crvB = null;

            if (!DA.GetData(0, ref crvA)) return;
            if (!DA.GetData(1, ref crvB)) return;

            if (!crvA.TryGetPolyline(out Polyline polyA))
                throw new ArgumentException();

            if (!crvB.TryGetPolyline(out Polyline polyB))
                throw new ArgumentException();

            var mesh = RhinoFactory.Mesh.CreateLoft(polyA, polyB);

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
            get { return new Guid("{C8AA832B-3A0C-4CF8-A63A-65414619BDF8}"); }
        }
    }
}

