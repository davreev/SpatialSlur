
/*
 * Notes
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;

using SpatialSlur;
using SpatialSlur.Fields;
using SpatialSlur.Rhino;

namespace SpatialSlur.Grasshopper.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateGridField : GH_Component
    {
        private FieldType _type = FieldType.Scalar;
        private SampleMode _sampleMode = SampleMode.Linear;
        private WrapMode _wrapMode = WrapMode.Clamp;


        private Param_Number _scalarParam = new Param_Number()
        {
            Name = "values",
            NickName = "values",
            Description = "Optional initial scalar values",
            Access = GH_ParamAccess.list,
            Optional = true
        };


        private Param_Vector _vectorParam = new Param_Vector()
        {
            Name = "values",
            NickName = "values",
            Description = "Optional initial vector values",
            Access = GH_ParamAccess.list,
            Optional = true
        };


        /// <summary>
        /// 
        /// </summary>
        public FieldType FieldType
        {
            get { return _type; }
            set
            {
                _type = value;
                Message = _type.ToString();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public CreateGridField()
          : base("Create Grid Field", "GridField",
              "Creates a field on a uniform grid.",
              "SpatialSlur", "Field")
        {
            FieldType = FieldType.Scalar;
        }


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("boundingBox", "bounds", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("countX", "countX", "Resolution in the x direction", GH_ParamAccess.item);
            pManager.AddIntegerParameter("countY", "countY", "Resolution in the y direction", GH_ParamAccess.item);
            pManager.AddIntegerParameter("countZ", "countZ", "Resolution in the z direction", GH_ParamAccess.item);
            pManager.AddParameter(_scalarParam);
          
            pManager[3].Optional = true;
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("result", "result", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Box box = null;
            if (!DA.GetData(0, ref box)) return;

            GH_Integer nx = null;
            if (!DA.GetData(1, ref nx)) return;

            GH_Integer ny = null;
            if (!DA.GetData(2, ref ny)) return;

            GH_Integer nz = null;
            DA.GetData(3, ref nz);

            switch (_type)
            {
                case FieldType.Scalar:
                    {
                        if (nz == null)
                            InitScalar(DA, nx.Value, ny.Value, box.Value.BoundingBox.ToInterval2d());
                        else
                            InitScalar(DA, nx.Value, ny.Value, nz.Value, box.Value.BoundingBox.ToInterval3d());
                        break;
                    }
                case FieldType.Vector:
                    {
                        if (nz == null)
                            InitVector(DA, nx.Value, ny.Value, box.Value.BoundingBox.ToInterval2d());
                        else
                            InitVector(DA, nx.Value, ny.Value, nz.Value, box.Value.BoundingBox.ToInterval3d());
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("The specified field type is not supported.");
                    }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        private void InitScalar(IGH_DataAccess DA, int nx, int ny, Interval2d bounds)
        {
            var f = GridField2d.Double.Create(nx, ny, bounds);
            f.WrapMode = _wrapMode;
            f.SampleMode = _sampleMode;

            List<GH_Number> vals = new List<GH_Number>();

            if (DA.GetDataList(4, vals))
                f.Set(LongestList(vals.Select(x => x.Value), f.CountXY));

            DA.SetData(0, new GH_ObjectWrapper(f));
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitScalar(IGH_DataAccess DA, int nx, int ny, int nz, Interval3d bounds)
        {
            var f = GridField3d.Double.Create(nx, ny, nz, bounds);
            f.WrapMode = _wrapMode;
            f.SampleMode = _sampleMode;

            List<GH_Number> vals = new List<GH_Number>();

            if (DA.GetDataList(4, vals))
                f.Set(LongestList(vals.Select(x => x.Value), f.CountXYZ));

            DA.SetData(0, new GH_ObjectWrapper(f));
        }

        
        /// <summary>
        /// 
        /// </summary>
        private void InitVector(IGH_DataAccess DA, int nx, int ny, Interval2d bounds)
        {
            var f = GridField2d.Vector2d.Create(nx, ny, bounds);
            f.WrapMode = _wrapMode;
            f.SampleMode = _sampleMode;

            List<GH_Vector> vals = new List<GH_Vector>();

            if (DA.GetDataList(4,vals))
                f.Set(LongestList(vals.Select(As2d), f.CountXY));

            DA.SetData(0, new GH_ObjectWrapper(f));
            
            Vector2d As2d(GH_Vector vector)
            {
                var v = vector.Value;
                return new Vector2d(v.X, v.Y);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitVector(IGH_DataAccess DA, int nx, int ny, int nz, Interval3d bounds)
        {
            var f = GridField3d.Vector3d.Create(nx, ny, nz, bounds);
            f.WrapMode = _wrapMode;
            f.SampleMode = _sampleMode;

            List<GH_Vector> vals = new List<GH_Vector>();

            if (DA.GetDataList(4, vals))
                f.Set(LongestList(vals.Select(x => (Vector3d)x.Value), f.CountXYZ));

            DA.SetData(0, new GH_ObjectWrapper(f));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<T> LongestList<T>(IEnumerable<T> source, int count)
        {
            var itr = source.GetEnumerator();
            var curr = default(T);

            for(int i = 0; i < count; i++)
            {
                if(itr.MoveNext())
                    curr = itr.Current;

                yield return curr;
            }
        }


        /// <inheritdoc />
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            var sub = Menu_AppendItem(menu, "Type");
            Menu_AppendItem(sub.DropDown, "Scalar", ScalarClicked, true, _type == FieldType.Scalar);
            Menu_AppendItem(sub.DropDown, "Vector", VectorClicked, true, _type == FieldType.Vector);

            sub = Menu_AppendItem(menu, "Wrap Mode");
            Menu_AppendItem(sub.DropDown, "Clamp", ClampClicked, true, _wrapMode == WrapMode.Clamp);
            Menu_AppendItem(sub.DropDown, "Repeat", RepeatClicked, true, _wrapMode == WrapMode.Repeat);
            Menu_AppendItem(sub.DropDown, "Mirror", MirrorClicked, true, _wrapMode == WrapMode.Mirror);

            sub = Menu_AppendItem(menu, "Sample Mode");
            Menu_AppendItem(sub.DropDown, "Nearest", NearestClicked, true, _sampleMode == SampleMode.Nearest);
            Menu_AppendItem(sub.DropDown, "Linear", LinearClicked, true, _sampleMode == SampleMode.Linear);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScalarClicked(object sender, EventArgs e)
        {
            FieldType = FieldType.Scalar;
            Params.Input[4] = _scalarParam;
            Params.OnParametersChanged();
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VectorClicked(object sender, EventArgs e)
        {
            FieldType = FieldType.Vector;
            Params.Input[4] = _vectorParam;
            Params.OnParametersChanged();
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NearestClicked(object sender, EventArgs e)
        {
            _sampleMode = SampleMode.Nearest;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinearClicked(object sender, EventArgs e)
        {
            _sampleMode = SampleMode.Linear;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClampClicked(object sender, EventArgs e)
        {
            _wrapMode = WrapMode.Clamp;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RepeatClicked(object sender, EventArgs e)
        {
            _wrapMode = WrapMode.Repeat;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MirrorClicked(object sender, EventArgs e)
        {
            _wrapMode = WrapMode.Mirror;
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
            get { return new Guid("{C2B98274-51EA-4021-B973-CBCF9CAA7E4B}"); }
        }
    }
}
