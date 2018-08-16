
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
using SpatialSlur.Fields;

using Vec3d = Rhino.Geometry.Vector3d;

namespace SpatialSlur.Grasshopper.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateNoiseField : GH_Component
    {
        private NoiseType _type = NoiseType.Perlin;


        /// <summary>
        /// 
        /// </summary>
        public NoiseType NoiseType
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
        public CreateNoiseField()
          : base("Create Noise Field", "NoiseField",
              "Creates a procedural noise field",
              "SpatialSlur", "Field")
        {
            NoiseType = NoiseType.Perlin;
        }


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddVectorParameter("scale", "scale", "", GH_ParamAccess.item, new Vec3d(1.0, 1.0, 1.0));
            pManager.AddVectorParameter("offset", "offset", "", GH_ParamAccess.item, new Vec3d(1.0, 1.0, 1.0));

            pManager[0].Optional = true;
            pManager[1].Optional = true;
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("result", "result", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Vector scale = null;
            if (!DA.GetData(0, ref scale)) return;

            GH_Vector offset = null;
            if (!DA.GetData(1, ref offset)) return;

            switch (_type)
            {
                case NoiseType.Perlin:
                    {
                        var f = new PerlinNoise3d(scale.Value, offset.Value);
                        DA.SetData(0, new GH_ObjectWrapper(f));
                        break;
                    }
                case NoiseType.Simplex:
                    {
                        var f = new SimplexNoise3d(scale.Value, offset.Value);
                        DA.SetData(0, new GH_ObjectWrapper(f));
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("The specified noise type is not supported.");
                    }
            }
        }


        /// <inheritdoc />
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            var sub = Menu_AppendItem(menu, "Noise Type");
            Menu_AppendItem(sub.DropDown, "Perlin", PerlinClicked, true, _type == NoiseType.Perlin);
            Menu_AppendItem(sub.DropDown, "Simplex", SimplexClicked, true, _type == NoiseType.Simplex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PerlinClicked(object sender, EventArgs e)
        {
            NoiseType = NoiseType.Perlin;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimplexClicked(object sender, EventArgs e)
        {
            NoiseType = NoiseType.Simplex;
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
            get { return new Guid("{EB41D9EB-B556-4642-8C97-11F92EAD1082}"); }
        }
    }
}
