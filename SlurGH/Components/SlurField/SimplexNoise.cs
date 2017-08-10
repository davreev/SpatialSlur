using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;

/*
 * Notes
 * 
 * TODO add noise type selection (Perlin, Simplex)
 */ 

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class SimplexNoise : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public SimplexNoise()
          : base("Simplex Noise", "SNoise",
              "Returns the noise value at a given point in space.",
              "SpatialSlur", "Field")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("point", "point", "Sample point", GH_ParamAccess.item);
            pManager.AddVectorParameter("scale", "scale", "Optional component wise scale values", GH_ParamAccess.item, new Vector3d(1, 1, 1));
            pManager[1].Optional = true;
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("result", "result", "Noise value at the given point", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d point = new Point3d();
            Vector3d scale = new Vector3d(1.0, 1.0, 1.0);

            if (!DA.GetData(0, ref point)) return;
            DA.GetData(1, ref scale);

            double t = SlurField.SimplexNoise.ValueAt(point.X * scale.X, point.Y * scale.Y, point.Z * scale.Z);

            DA.SetData(0, t);
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
            get { return new Guid("{0e599310-c465-4c2b-92c7-e12ecf6b143e}"); }
        }
    }
}
