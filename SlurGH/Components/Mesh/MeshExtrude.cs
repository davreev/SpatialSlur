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
    public class MeshExtrude : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public MeshExtrude()
          : base("Mesh Extrude", "Extrude",
              "Extrudes a mesh from a polyline",
              "Mesh", "Util")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("poly", "polyline", "", GH_ParamAccess.item);
            pManager.AddVectorParameter("dir", "direction", "", GH_ParamAccess.item, Vector3d.Zero);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("result", "result", "Extruded mesh", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = null;
            Vector3d dir = new Vector3d();

            if (!DA.GetData(0, ref crv)) return;
            if (!DA.GetData(1, ref dir)) return;

            Polyline poly;
            if (!crv.TryGetPolyline(out poly))
                throw new ArgumentException();

            var mesh = MeshUtil.Extrude(poly, dir);

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
            get { return new Guid("{7A0D856D-7C43-42D1-AEEC-BACDDA1881A4}"); }
        }
    }
}
