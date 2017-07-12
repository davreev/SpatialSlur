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

using SlurSketchGH.Grasshopper.Types;
using SlurSketchGH.Grasshopper.Params;

/*
 * Notes
 */

namespace SlurSketchGH.Grasshopper.Components
{
    public class DisplayPlanarity : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public DisplayPlanarity()
          : base("Display Planarity", "Planarity",
              "Displays the planar deviation of each face in a given mesh.",
              "SpatialSlur", "Display")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMeshParam(), "heMesh", "heMesh", "Mesh to paint", GH_ParamAccess.item);
            pManager.AddColourParameter("colors", "colors", "", GH_ParamAccess.list);
            pManager.AddIntervalParameter("interval", "interval", "", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Painted mesh", GH_ParamAccess.item);
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

            var dispMesh = SolveInstanceImpl(mesh, colors, interval.ToDomain(), out Domain range);

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
        Mesh SolveInstanceImpl(HeMesh3d mesh, IReadOnlyList<Color> colors, Domain interval, out Domain range)
        {
            var topo = mesh.Topology.Duplicate(); // create copy for topological changes
            var faces = topo.Faces;

            var vertPos = mesh.VertexPositions;
         
            // get planarity
            var planarDev = new List<double>(faces.Count);
            planarDev.Fill(0.0);
            faces.GetFacePlanarity(vertPos, planarDev, true);

            // get planarity range
            range = new Domain(planarDev);

            // set planarity of additional faces created during quadrangulation
            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                int n = (f.Degree - 4 + 1) >> 1; // number of new faces created
                for (int j = 0; j < n; j++)
                    planarDev.Add(planarDev[i]);
            }

            // quadrangulate
            faces.Quadrangulate();

            // paint mesh
            var dispMesh = topo.ToPolySoup(vertPos);
            if (!interval.IsValid) interval = range;
            dispMesh.PaintByFaceValue(planarDev, x => colors.Lerp(interval.Normalize(x)));
            dispMesh.Compact();

            return dispMesh;
        }
    }
}