
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SpatialSlur.Meshes;
using SpatialSlur.Rhino;
using SpatialSlur.Grasshopper.Types;
using SpatialSlur.Grasshopper.Params;

namespace SpatialSlur.Grasshopper.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class HeGraphFromLines : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public HeGraphFromLines()
          : base("HeGraph From Lines", "FromLns",
              "Creates a halfedge graph from a list of line segments",
              "SpatialSlur", "Mesh")
        {
        }


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("lines", "lines", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("tolerance", "tol", "", GH_ParamAccess.item, 1.0e-4);

            // TODO make menu items
            pManager.AddBooleanParameter("allowMulti", "multi", "", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("allowLoops", "loops", "", GH_ParamAccess.item, false);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new HeGraph3dParam(), "result", "result", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var lines = new List<GH_Line>();
            if (!DA.GetDataList(0, lines)) return;

            var tol = 0.0;
            if (!DA.GetData(1, ref tol)) return;

            var multi = false;
            if (!DA.GetData(2, ref multi)) return;

            var loops = false;
            if (!DA.GetData(3, ref loops)) return;

            var graph = HeGraph3d.Factory.CreateFromLineSegments(lines.Select(ln => ln.Value), tol, multi, loops);

            DA.SetData(0, new GH_HeGraph3d(graph));
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
            get { return new Guid("{AD761F5E-656B-4FA3-903C-E176ED422868}"); }
        }
    }
}
