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

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class VertexPositions : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexPositions()
          : base("Vertex Positions", "VertPos",
              "Returns the position of each vertex in a halfedge graph.",
              "SpatialSlur", "Mesh")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("heGraph", "heGraph", "", GH_ParamAccess.item);
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
            IGH_Goo goo = null;
            if (!DA.GetData(0, ref goo)) return;

            if (goo is GH_Goo<HeGraph3d>)
            {
                var value = ((GH_Goo<HeGraph3d>)goo).Value;
                DA.SetDataList(0, value.Vertices.Select(v => new GH_Point(v.Position.ToPoint3d())));
            }
            else if (goo is GH_Goo<HeMesh3d>)
            {
                var value = ((GH_Goo<HeMesh3d>)goo).Value;
                DA.SetDataList(0, value.Vertices.Select(v => new GH_Point(v.Position.ToPoint3d())));
            }
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

