using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurRhino;

using SpatialSlur.SlurGH.Grasshopper.Params;
using SpatialSlur.SlurGH.Grasshopper.Types;


/*
 * Notes
 */

namespace SlurSketchGH.Grasshopper.Components
{
    public class VertexPositions : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexPositions()
          : base("Vertex Positions", "VertPos",
              "Returns the position of each vertex in a given mesh",
              "SpatialSlur", "Mesh")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMeshParam(), "heMesh", "heMesh", "Mesh to extract from", GH_ParamAccess.item);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("result", "result", "Vertex positions", GH_ParamAccess.list);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            HeMesh3d mesh = null;
            if (!DA.GetData(0, ref mesh)) return;

            DA.SetDataList(0, mesh.VertexPositions.Select(p => new GH_Point(p.ToPoint3d())));
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
            get { return new Guid("{F39B5887-09E7-4D40-B0C6-CA69073936B1}"); }
        }
    }
}

