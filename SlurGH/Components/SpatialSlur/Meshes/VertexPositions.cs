
/*
 * Notes
 */

using System;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SpatialSlur.Meshes;

namespace SpatialSlur.Grasshopper.Components
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


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("heGraph", "heGraph", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("result", "result", "Vertex positions", GH_ParamAccess.list);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper goo = null;
            if (!DA.GetData(0, ref goo)) return;

            var obj = goo.Value;

            if (obj is HeGraph3d)
            {
                var graph = (HeGraph3d)obj;
                DA.SetDataList(0, graph.Vertices.Select(v => new GH_Point(v.Position)));
            }
            else if (obj is HeMesh3d)
            {
                var mesh = (HeMesh3d)obj;
                DA.SetDataList(0, mesh.Vertices.Select(v => new GH_Point(v.Position)));
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

