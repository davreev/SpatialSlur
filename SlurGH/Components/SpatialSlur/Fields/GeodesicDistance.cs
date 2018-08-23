
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SpatialSlur.Fields;

using G = SpatialSlur.Fields.GeodesicDistance;

namespace SpatialSlur.Grasshopper.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class GeodesicDistance : GH_Component
    {
        private DistanceMetric _metric = DistanceMetric.Euclidean;


        /// <summary>
        /// 
        /// </summary>
        public DistanceMetric Metric
        {
            get { return _metric; }
            set
            {
                _metric = value;
                Message = _metric.ToString();
            }
        }


        /// <summary>
        /// Initializes a new instance of the FastMarching2dTest class.
        /// </summary>
        public GeodesicDistance()
          : base("Geodesic Distance", "GeoDist",
                "Computes the geodesic distance field for a set of sources.",
                "SpatialSlur", "Field")
        {
            Metric = DistanceMetric.Euclidean;
        }


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("travelCost", "cost", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("sources", "sources", "", GH_ParamAccess.list);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("result", "result", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper costGoo = null;
            if (!DA.GetData(0, ref costGoo)) return;

            List<GH_Integer> sources = new List<GH_Integer>();
            if (!DA.GetDataList(1, sources)) return;

            var cost = (GridField2d<double>)costGoo.Value;
            var result = GridField2d.Double.Create(cost);

            switch(_metric)
            {
                case DistanceMetric.Manhattan:
                    {
                        G.CalculateL1(cost, sources.Select(x => x.Value), result);
                        break;
                    }
                case DistanceMetric.Euclidean:
                    {
                        G.CalculateL2(cost, sources.Select(x => x.Value), result);
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("The specified distance metric is not supported.");
                    }
            }
            
            DA.SetData(0, new GH_ObjectWrapper(result));
        }


        /// <inheritdoc />
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            var sub = Menu_AppendItem(menu, "Distance Metric");
            Menu_AppendItem(sub.DropDown, "Manhattan", MahattanClicked, true, _metric == DistanceMetric.Manhattan);
            Menu_AppendItem(sub.DropDown, "Euclidean", EuclideanClicked, true, _metric == DistanceMetric.Euclidean);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MahattanClicked(object sender, EventArgs e)
        {
            Metric = DistanceMetric.Manhattan;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EuclideanClicked(object sender, EventArgs e)
        {
            Metric = DistanceMetric.Euclidean;
            ExpireSolution(true);
        }


        /// <inheritdoc />
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }


        /// <inheritdoc />
        public override Guid ComponentGuid
        {
            get { return new Guid("{057278e1-ad59-424b-abf1-58508a7d9f6c}"); }
        }
    }
}