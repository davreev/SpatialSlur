using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurRhino;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class IntegrateField : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public IntegrateField()
          : base("Integrate Field", "Integrate",
              "Integrates a vector field from a given point",
              "SpatialSlur", "Field")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("vectorField", "vecField", "Field to integrate", GH_ParamAccess.item);
            pManager.AddPointParameter("points", "points", "Start point", GH_ParamAccess.list);
            pManager.AddNumberParameter("stepSize", "stepSize", "", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("stepCount", "stepCount", "", GH_ParamAccess.item, 100);
            pManager.AddIntegerParameter("integrationMode", "mode", "0 = Euler, 1 = RK2, 2 = RK4", GH_ParamAccess.item, 2);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("result", "result", "Integrated field line", GH_ParamAccess.list);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper goo = null;
            if (!DA.GetData(0, ref goo)) return;

            var points = new List<Point3d>();
            if (!DA.GetDataList(1, points)) return;

            double stepSize = 0.0;
            DA.GetData(2, ref stepSize);

            int stepCount = 0;
            DA.GetData(3, ref stepCount);

            int mode = 0;
            DA.GetData(4, ref mode);

            var value = goo.Value;

            if (value is IField3d<Vec3d>)
            {
                var field = (IField3d<Vec3d>)value;
                var polys = SolveInstanceImpl(field, points, stepSize, stepCount, (IntegrationMode)mode);
                DA.SetDataList(0, polys);
            }
            else if (value is IField2d<Vec2d>)
            {
                var field = (IField2d<Vec2d>)value;
                var polys = SolveInstanceImpl(field, points, stepSize, stepCount, (IntegrationMode)mode);
                DA.SetDataList(0, polys);
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="points"></param>
        /// <param name="stepSize"></param>
        /// <param name="stepCount"></param>
        /// <param name="mode"></param>
        private PolylineCurve[] SolveInstanceImpl(IField3d<Vec3d> field, List<Point3d> points, double stepSize, int stepCount, IntegrationMode mode)
        {
            var result = new PolylineCurve[points.Count];

            Parallel.For(0, points.Count, i =>
            {
                var pts = field.IntegrateFrom(points[i], stepSize, mode).Take(stepCount).Select(p => (Point3d)p);
                result[i] = new PolylineCurve(pts);
            });

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="points"></param>
        /// <param name="stepSize"></param>
        /// <param name="stepCount"></param>
        /// <param name="mode"></param>
        private PolylineCurve[] SolveInstanceImpl(IField2d<Vec2d> field, List<Point3d> points, double stepSize, int stepCount, IntegrationMode mode)
        {
            var result = new PolylineCurve[points.Count];

            Parallel.For(0, points.Count, i =>
            {
                Vec3d p0 = points[i];
                var z = p0.Z;

                var pts = field.IntegrateFrom(p0, stepSize, mode).Take(stepCount).Select(p => new Point3d(p.X, p.Y, z));
                result[i] = new PolylineCurve(pts);
            });

            return result;
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
            get { return new Guid("{C1AF4518-6B68-4E6A-87C3-0F94C30B3AD6}"); }
        }
    }
}
