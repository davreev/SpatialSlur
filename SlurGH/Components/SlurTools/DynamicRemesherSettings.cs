using System;

using Grasshopper.Kernel;
using Rhino.Geometry;

using SpatialSlur.SlurTools;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
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


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("lengthRange", "lengthRange", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("lengthTolerance", "lengthTol", "", GH_ParamAccess.item, 0.1);

            pManager.AddNumberParameter("featureWeight", "featureWeight", "", GH_ParamAccess.item, 100.0);
            pManager.AddNumberParameter("featureTolerance", "featureTol", "", GH_ParamAccess.item, 0.01);
            
            pManager.AddNumberParameter("damping", "damping", "", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("timeStep", "timeStep", "", GH_ParamAccess.item, 1.0);

            pManager.AddIntegerParameter("refineFrequency", "refineFreq", "", GH_ParamAccess.item, 5);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("settings", "settings", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Interval lengthRng = new Interval();
            if (!DA.GetData(0, ref lengthRng)) return;

            double lengthTol = 0.0;
            if (!DA.GetData(1, ref lengthTol)) return;

            double featureWt = 0.0;
            if (!DA.GetData(2, ref featureWt)) return;

            double featureTol = 0.0;
            if (!DA.GetData(3, ref featureTol)) return;

            double damping = 0.0;
            if (!DA.GetData(4, ref damping)) return;

            double timeStep = 0.0;
            if (!DA.GetData(5, ref timeStep)) return;

            int refineFreq = 0;
            if (!DA.GetData(6, ref refineFreq)) return;

            var settings = new DynamicRemesher.Settings()
            {
                LengthRange = lengthRng,
                LengthTolerance = lengthTol,
                FeatureWeight = featureWt,
                Damping = damping,
                TimeStep = timeStep,
                RefineFrequency = refineFreq
            };

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

