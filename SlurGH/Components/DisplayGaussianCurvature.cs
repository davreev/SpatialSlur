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

/*
 * Notes
 */

namespace SlurSketchGH.Grasshopper.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class DisplayGaussianCurvature : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public DisplayGaussianCurvature()
          : base("Display Gaussian Curvature", "GaussCrv",
              "Displays the Gaussian curvature of a given mesh.",
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
            get { return new Guid("{C20C4FC2-D47F-4230-AD2D-6536A4923954}"); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="colors"></param>
        /// <param name="interval"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        Mesh SolveInstanceImpl(HeMesh3d mesh, IReadOnlyList<Color> colors, Domain interval, out Domain range)
        {
            var topo = mesh.Topology.Duplicate(); // create copy for topological changes
            var hedges = topo.Halfedges;
            var verts = topo.Vertices;
            var faces = topo.Faces;

            var vertPos = mesh.VertexPositions;

            // triangulate
            faces.OrientFacesToMin(GetDiagonalLength);
            faces.Triangulate();

            double GetDiagonalLength(HeMeshHalfedge hedge)
            {
                int vi0 = hedge.Start.Index;
                int vi1 = hedge.NextInFace.End.Index;
                return vertPos[vi0].SquareDistanceTo(vertPos[vi1]);
            }

            // vertex areas
            var areas = new double[verts.Count];
            verts.GetVertexAreasBarycentric(vertPos, areas);

            // gaussian curvature
            var gaussCrv = new double[verts.Count];
            verts.GetGaussianCurvature(vertPos, areas, gaussCrv, true);
            range = new Domain(gaussCrv);

            // paint mesh
            Mesh dispMesh = topo.ToMesh(vertPos);
            if (!interval.IsValid) interval = range;
            dispMesh.PaintByVertexValue(gaussCrv, x => colors.Lerp(interval.Normalize(x)), true);
            dispMesh.Compact();

            return dispMesh;
        }

    }
}
