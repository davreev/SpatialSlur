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

using SpatialSlur.SlurGH.Types;
using SpatialSlur.SlurGH.Params;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class DisplayFacePlanarity : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public DisplayFacePlanarity()
          : base("Display Face Planarity", "FacePln",
              "Displays the planar deviation of each face in a given mesh.",
              "SpatialSlur", "Display")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "heMesh", "heMesh", "Mesh to color", GH_ParamAccess.item);
            pManager.AddColourParameter("colors", "colors", "", GH_ParamAccess.list);
            pManager.AddIntervalParameter("interval", "interval", "", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Colored mesh", GH_ParamAccess.item);
            pManager.AddIntervalParameter("range", "range", "Range of displayed values", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            HeMesh3d mesh = null;
            List<Color> colors = new List<Color>();
            Interval interval = new Interval();

            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetDataList(1, colors)) return;
            DA.GetData(2, ref interval);

            var dispMesh = SolveInstanceImpl(mesh, colors, interval.ToInterval1d(), out Interval1d range);

            DA.SetData(0, new GH_Mesh(dispMesh));
            DA.SetData(1, new GH_Interval(range.ToInterval()));
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
            get { return new Guid("{607BA85B-E57C-42B4-8EF1-C1C216C505C6}"); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Mesh SolveInstanceImpl(HeMesh3d mesh, IReadOnlyList<Color> colors, Interval1d interval, out Interval1d range)
        {
            var verts = mesh.Vertices;
            var faces = mesh.Faces;

            var planarDev = new double[faces.Count];
            mesh.GetFacePlanarity(v => v.Position, (f, t) => planarDev[f] = t);

            // get planarity range
            range = new Interval1d(planarDev);
            if (!interval.IsValid) interval = range;

            // create new mesh
            return mesh.ToPolySoup(f => colors.Lerp(interval.Normalize(planarDev[f])));
        }
    }
}