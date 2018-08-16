
/*
 * Notes
 */
 
using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur;
using SpatialSlur.Fields;
using SpatialSlur.Grasshopper.Types;

namespace SpatialSlur.Grasshopper.Components.Field
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


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("field", "field", "Field to evaluate", GH_ParamAccess.item);
            pManager.AddPointParameter("points", "points", "Points to evaluate at", GH_ParamAccess.list);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("values", "values", "Values at the given points", GH_ParamAccess.list);
        }


        /// <inheritdoc />
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
                case IField3d<Vector2d> f:
                    {
                        var vals = points.Convert(p => f.ValueAt(p.Value), true);
                        DA.SetDataList(0, vals.Select(x => new GH_Vector(x.As3d)));
                        break;
                    }
                case IField3d<Vector3d> f:
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
            get { return new Guid("9906e722-0ac2-45ad-a86a-8bb290bff637"); }
        }
    }
}