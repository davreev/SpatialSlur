
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurRhino;
using SpatialSlur.SlurGH.Params;

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class DisplayFaceColors : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public DisplayFaceColors()
          : base("Display Face Colors", "FaceCol",
              "Applies a solid color to each face in a halfedge mesh.",
              "SpatialSlur", "Display")
        {
        }


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "heMesh", "heMesh", "Mesh to color", GH_ParamAccess.item);
            pManager.AddColourParameter("colors", "colors", "", GH_ParamAccess.list);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Colored mesh", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            HeMesh3d mesh = null;
            List<Color> colors = new List<Color>();

            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetDataList(1, colors)) return;

            int last = colors.Count - 1;
            var result = mesh.ToPolySoup(f => colors[Math.Min(f, last)]);

            DA.SetData(0, new GH_Mesh(result));
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
            get { return new Guid("{65C6B961-F1D1-4C41-87C9-C43FB3778923}"); }
        }
    }
}