
/*
 * Notes
 */
 
using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurRhino;

using SpatialSlur.SlurGH.Types;

namespace SpatialSlur.SlurGH.Components.Field
{
    /// <summary>
    /// 
    /// </summary>
    public class EvaluateField : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public EvaluateField()
          : base("Evaluate Field", "EvalField",
              "Evaluates a field at a given point",
              "SpatialSlur", "Field")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("field", "field", "Field to evaluate", GH_ParamAccess.item);
            pManager.AddPointParameter("points", "points", "Points to evaluate at", GH_ParamAccess.list);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("values", "values", "Values at the given points", GH_ParamAccess.list);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper fieldGoo = null;
            if (!DA.GetData(0, ref fieldGoo)) return;

            List<GH_Point> points = new List<GH_Point>();
            if (!DA.GetDataList(1, points)) return;

            switch (fieldGoo.Value)
            {
                case IField3d<double> f:
                    {
                        var vals = points.Convert(p => f.ValueAt(p.Value), true);
                        DA.SetDataList(0, vals.Select(x => new GH_Number(x)));
                        break;
                    }
                case IField3d<Vec2d> f:
                    {
                        var vals = points.Convert(p => f.ValueAt(p.Value), true);
                        DA.SetDataList(0, vals.Select(x => new GH_Vector((Vec3d)x)));
                        break;
                    }
                case IField3d<Vec3d> f:
                    {
                        var vals = points.Convert(p => f.ValueAt(p.Value), true);
                        DA.SetDataList(0, vals.Select(x => new GH_Vector(x)));
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("The given field object can not be evaluated.");
                    }
            }
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9906e722-0ac2-45ad-a86a-8bb290bff637"); }
        }
    }
}