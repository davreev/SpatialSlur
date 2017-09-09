using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurCore;

using SpatialSlur.SlurRhino;
using SpatialSlur.SlurRhino.Remesher;

using SpatialSlur.SlurGH.Types;
using SpatialSlur.SlurGH.Params;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Remesher
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicRemesherSettings : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public DynamicRemesherSettings()
          : base("Dynamic Remesher Settings", "Settings",
              "Creates settings used for dynamic remeshing",
              "SpatialSlur", "Mesh")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("lengthRange", "lengthRange", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("lengthTolerance", "lengthTolerance", "", GH_ParamAccess.item, 0.1);
            pManager.AddNumberParameter("featureWeight", "featureWeight", "", GH_ParamAccess.item, 100.0);
            pManager.AddNumberParameter("smoothWeight", "smoothWeight", "", GH_ParamAccess.item, 1.0);

            pManager.AddNumberParameter("timeStep", "timeStep", "", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("damping", "damping", "", GH_ParamAccess.item, 0.5);

            pManager.AddIntegerParameter("subSteps", "subSteps", "", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("refineFrequency", "refineFreq", "", GH_ParamAccess.item, 5);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("settings", "settings", "", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Interval lengthRng = new Interval();
            if (!DA.GetData(0, ref lengthRng)) return;

            double lengthTol = 0.0;
            if (!DA.GetData(1, ref lengthTol)) return;

            double feature = 0.0;
            if (!DA.GetData(2, ref feature)) return;

            double smooth = 0.0;
            if (!DA.GetData(3, ref smooth)) return;

            double timeStep = 0.0;
            if (!DA.GetData(4, ref timeStep)) return;

            double damping = 0.0;
            if (!DA.GetData(5, ref damping)) return;

            int subSteps = 0;
            if (!DA.GetData(6, ref subSteps)) return;

            int refineFreq = 0;
            if (!DA.GetData(7, ref refineFreq)) return;

            var settings = new SlurRhino.Remesher.DynamicRemesherSettings();
            settings.LengthRange = lengthRng.ToInterval1d();
            settings.LengthTolerance = lengthTol;
            settings.FeatureWeight = feature;
            settings.SmoothWeight = smooth;
            settings.TimeStep = timeStep;
            settings.Damping = damping;
            settings.SubSteps = subSteps;
            settings.RefineFrequency = refineFreq;

            DA.SetData(0, settings);
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
            get { return new Guid("{E50BE204-CB0D-4A0F-AF68-C439C57FBE22}"); }
        }
    }
}

