
/*
 * Notes
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur;
using SpatialSlur.Fields;
using System.Windows.Forms;

using Vec3d = Rhino.Geometry.Vector3d;

namespace SpatialSlur.Grasshopper.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class IntegrateField : GH_Component
    {
        private IntegrationMode _mode = IntegrationMode.Euler;


        /// <summary>
        /// 
        /// </summary>
        public IntegrationMode IntegrationMode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                Message = _mode.ToString();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IntegrateField()
          : base("Integrate Field", "Integrate",
              "Integrates a vector field from a given point",
              "SpatialSlur", "Field")
        {
            IntegrationMode = IntegrationMode.Euler;
        }


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("vectorField", "vecField", "Vector field to integrate", GH_ParamAccess.item);
            pManager.AddPointParameter("points", "points", "Start point", GH_ParamAccess.list);
            pManager.AddNumberParameter("stepSize", "stepSize", "", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("stepCount", "stepCount", "", GH_ParamAccess.item, 100);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("result", "result", "Integrated field line", GH_ParamAccess.list);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper fieldGoo = null;
            if (!DA.GetData(0, ref fieldGoo)) return;

            var points = new List<Point3d>();
            if (!DA.GetDataList(1, points)) return;

            double stepSize = 0.0;
            DA.GetData(2, ref stepSize);

            int stepCount = 0;
            DA.GetData(3, ref stepCount);

            PolylineCurve[] paths = null;

            switch(fieldGoo.Value)
            {
                case IField3d<Vector3d> f:
                    {
                        paths = SolveInstanceImpl(f, points, stepSize, stepCount);
                        break;
                    }
                case IField2d<Vector2d> f:
                    {
                        paths = SolveInstanceImpl(f, points, stepSize, stepCount);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("The given vector field is not recognized.");
                    }
            }

            DA.SetDataList(0, paths);
        }
        

        /// <summary>
        /// 
        /// </summary>
        private PolylineCurve[] SolveInstanceImpl(IField3d<Vector3d> field, List<Point3d> points, double stepSize, int stepCount)
        {
            var result = new PolylineCurve[points.Count];

            Parallel.For(0, points.Count, i =>
            {
                var pts = Streamline.IntegrateFrom(field, points[i], stepSize, _mode).Take(stepCount).Select(p => (Point3d)p);
                result[i] = new PolylineCurve(pts);
            });

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private PolylineCurve[] SolveInstanceImpl(IField2d<Vector2d> field, List<Point3d> points, double stepSize, int stepCount)
        {
            var result = new PolylineCurve[points.Count];

            Parallel.For(0, points.Count, i =>
            {
                Vector3d p0 = points[i];
                double z = p0.Z;

                var pts = Streamline.IntegrateFrom(field, p0.XY, stepSize, _mode).Take(stepCount).Select(p => new Point3d(p.X, p.Y, z));
                result[i] = new PolylineCurve(pts);
            });

            return result;
        }


        /// <inheritdoc />
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            var sub = Menu_AppendItem(menu, "Integration mode");
            Menu_AppendItem(sub.DropDown, "Euler", EulerClicked, true, _mode == IntegrationMode.Euler);
            Menu_AppendItem(sub.DropDown, "RK2", RK2Clicked, true, _mode == IntegrationMode.RK2);
            Menu_AppendItem(sub.DropDown, "RK4", RK4Clicked, true, _mode == IntegrationMode.RK4);
        }


        /// <summary>
        /// 
        /// </summary>
        private void EulerClicked(object sender, EventArgs e)
        {
            IntegrationMode = IntegrationMode.Euler;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        private void RK2Clicked(object sender, EventArgs e)
        {
            IntegrationMode = IntegrationMode.RK2;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        private void RK4Clicked(object sender, EventArgs e)
        {
            IntegrationMode = IntegrationMode.RK4;
            ExpireSolution(true);
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
