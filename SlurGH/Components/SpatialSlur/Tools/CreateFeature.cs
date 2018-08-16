
/*
 * Notes
 */
 
 using System;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SpatialSlur.Tools;

namespace SpatialSlur.Grasshopper.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateFeature : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public CreateFeature()
          : base("Creates Features", "Feature",
              "Creates a geometric feature used for dynamic remeshing",
              "SpatialSlur", "Mesh")
        {
        }


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("geometry", "geom", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("result", "result", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper goo = null;
            if (!DA.GetData(0, ref goo)) return;
            
            IFeature feat = null;

            switch (goo.Value)
            {
                case Mesh m:
                    feat = new MeshFeature(m);
                    break;
                case Curve c:
                    feat = new CurveFeature(c);
                    break;
                case Point3d p:
                    feat = new PointFeature(p);
                    break;
                default:
                    throw new ArgumentException();
            }
            
            DA.SetData(0, new GH_ObjectWrapper(feat));
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
            get { return new Guid("{0FEF2BE4-6432-4352-ADE2-F160108EDA12}"); }
        }
    }
}

