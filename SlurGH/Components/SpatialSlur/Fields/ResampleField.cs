
/*
 * Notes
 */
 
 using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SpatialSlur;
using SpatialSlur.Fields;

namespace SpatialSlur.Grasshopper.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class ResampleField : GH_Component
    {
        private bool _parallel = true;


        /// <summary>
        /// 
        /// </summary>
        public ResampleField()
          : base("Resample Field", "Resample",
              "Resamples a field with the coordinates of another field.",
              "SpatialSlur", "Field")
        {
        }


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("source", "source", "Field to resample from", GH_ParamAccess.item);
            pManager.AddGenericParameter("target", "target", "Field to resample with", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("result", "result", "Field containing sampled values", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper sourceGoo = null;
            if (!DA.GetData(0, ref sourceGoo)) return;

            GH_ObjectWrapper sampleGoo = null;
            if (!DA.GetData(1, ref sampleGoo)) return;

            var source = sourceGoo.Value;
            var sampler = sampleGoo.Value;
            
            if (sampler is ISampledField3d<double>)
            {
                if (source is IField3d<double>)
                {
                    var f0 = (IField3d<double>)source;
                    var f1 = ((ISampledField3d<double>)sampler).Duplicate(false);

                    f1.Sample(f0, _parallel);
                    DA.SetData(0, new GH_ObjectWrapper(f1));
                    return;
                }
            }

            if (sampler is ISampledField3d<Vector3d>)
            {
                if (source is IField3d<Vector3d>)
                {
                    var f0 = (IField3d<Vector3d>)source;
                    var f1 = ((ISampledField3d<Vector3d>)sampler).Duplicate(false);

                    f1.Sample(f0, _parallel);
                    DA.SetData(0, new GH_ObjectWrapper(f1));
                    return;
                }
            }

            throw new ArgumentException("The given fields are incompatible for sampling.");
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
            get { return new Guid("{1D25C8A1-4BAF-44B4-A31E-8C83374FEA07}"); }
        }
    }
}
