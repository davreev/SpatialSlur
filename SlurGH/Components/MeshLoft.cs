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
    public class MeshLoft : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public MeshLoft()
          : base("Mesh Loft", "Loft",
              "Creates a mesh through a list of polylines",
              "Mesh", "Util")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("polys", "polylines", "", GH_ParamAccess.list);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
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
            List<Curve> curves = new List<Curve>();

            if (!DA.GetDataList(0, curves)) return;

            var polys = curves.ConvertAll(crv =>
            {
                if (!crv.TryGetPolyline(out Polyline poly))
                    throw new ArgumentException();

                return poly;
            });

            var mesh = MeshUtil.Loft(polys);

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
            get { return new Guid("{BCFCF1AE-046B-4164-AF5A-12E5EE9F71CD}"); }
        }
    }
}

