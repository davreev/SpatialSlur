using System;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurGH.Types;
using SpatialSlur.SlurGH.Params;

/*
 * Notes
 */

namespace SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class HoleBoundaries : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public HoleBoundaries()
          : base("Hole Boundaries", "HoleBnds",
              "Returns the boundary of each hole in a given halfedge mesh as a closed polyline",
              "SpatialSlur", "Mesh")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "heMesh", "heMesh", "Halfedge structure to extract from", GH_ParamAccess.item);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("result", "result", "Face polylines", GH_ParamAccess.list);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_HeMesh3d mesh = null;
            if (!DA.GetData(0, ref mesh)) return;

            var result = mesh.Value.GetHoles().Select(he0 =>
            {
                var poly = new Polyline(he0.Circulate.Select(he => (Point3d)he.Start.Position));
                poly.Add(poly[0]);

                return new GH_Curve(poly.ToNurbsCurve());
            });

            DA.SetDataList(0, result);
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
            get { return new Guid("{0B107055-DE31-467B-9762-C0F44430A2FF}"); }
        }
    }
}

