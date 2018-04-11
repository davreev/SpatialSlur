using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurRhino;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class EdgeLines : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public EdgeLines()
          : base("Edge Lines", "EdgeLns",
              "Returns a line for each edge in a given halfedge structure",
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
            pManager.AddLineParameter("result", "result", "Edge lines", GH_ParamAccess.list);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper goo = null;
            if (!DA.GetData(0, ref goo)) return;

            switch(goo.Value)
            {
                case HeGraph3d g:
                    DA.SetDataList(0, GetEdgeLines(g, v => v.Position));
                    break;
                case HeMesh3d m:
                    DA.SetDataList(0, GetEdgeLines(m, v => v.Position));
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        IEnumerable<GH_Line> GetEdgeLines<V, E>(HeStructure<V, E> graph, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            return graph.Edges.Select(he => (he.IsUnused) ? new GH_Line(Line.Unset) : new GH_Line(he.ToLine(getPosition)));
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
            get { return new Guid("{D3EBB8A8-08D5-47FB-AD7A-275C9A82C4BD}"); }
        }
    }
}

