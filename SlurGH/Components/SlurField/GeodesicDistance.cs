using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurField;

/*
 * Notes
 */

namespace SlurSketchGH.Grasshopper.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class GeodesicDistance : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FastMarching2dTest class.
        /// </summary>
        public GeodesicDistance()
          : base("Geodesic Distance", "GeoDist",
                "Computes the geodesic distance field for a set of sources.",
                "SpatialSlur", "Field")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("travelCost", "cost", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("sources", "sources", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("distanceMetric", "metric", "0 = Manhattan (L1), 1 = Euclidean (L2)", GH_ParamAccess.item, 1);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("result", "result", "", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper costGoo = null;
            if (!DA.GetData(0, ref costGoo)) return;

            List<GH_Integer> sources = new List<GH_Integer>();
            if (!DA.GetDataList(1, sources)) return;

            int metric = -1;
            if (!DA.GetData(2, ref metric)) return;

            var cost = (GridScalarField2d)costGoo.Value;
            var dist = new GridScalarField2d(cost);

            switch(metric)
            {
                case 0:
                    SimulationUtil.GeodesicDistanceL1(cost, sources.Select(x => x.Value), dist.Values);
                    break;
                case 1:
                    SimulationUtil.GeodesicDistanceL2(cost, sources.Select(x => x.Value), dist.Values);
                    break;
                default:
                    throw new NotSupportedException("The specified distance metric is not supported.");
            }
            
            DA.SetData(0, new GH_ObjectWrapper(dist));
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{057278e1-ad59-424b-abf1-58508a7d9f6c}"); }
        }
    }
}