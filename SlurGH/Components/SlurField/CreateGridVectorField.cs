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
    public class CreateGridVectorField : GH_Component
    {
        private SampleMode _sample = SampleMode.Linear;
        private WrapMode _wrapX = WrapMode.Clamp;
        private WrapMode _wrapY = WrapMode.Clamp;
        private WrapMode _wrapZ = WrapMode.Clamp;

        // TODO
        // add wrap and sample modes as menu items

        /// <summary>
        /// 
        /// </summary>
        public CreateGridVectorField()
          : base("Create GridVectorField", "GridVecF",
              "Creates a vector field on a uniform grid.",
              "SpatialSlur", "Field")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("boundingBox", "bounds", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("countX", "countX", "Resolution in the x direction", GH_ParamAccess.item);
            pManager.AddIntegerParameter("countY", "countY", "Resolution in the y direction", GH_ParamAccess.item);
            pManager.AddIntegerParameter("countZ", "countZ", "Resolution in the z direction", GH_ParamAccess.item);
            pManager.AddVectorParameter("values", "values", "Optional initial values", GH_ParamAccess.list);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("result", "result", "", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
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

            List<GH_Vector> vals = new List<GH_Vector>();
            DA.GetDataList(4, vals);

            if (nz == null)
            {
                var d = box.Value.BoundingBox.ToInterval2d();
                var f = new GridVectorField2d(d, nx.Value, ny.Value, _wrapX, _wrapY, _sample);

                // set values
                if (vals != null)
                {
                    if (vals.Count == 1)
                    {
                        var v = vals[0].Value;
                        f.Set(new Vec2d(v.X, v.Y));
                    }
                    else
                    {
                        f.Set(vals.Select(x =>
                        {
                            var v = x.Value;
                            return new Vec2d(v.X, v.Y);
                        }));
                    }
                }

                DA.SetData(0, new GH_ObjectWrapper(f));
            }
            else
            {
                var d = box.Value.BoundingBox.ToInterval3d();
                var f = new GridVectorField3d(d, nx.Value, ny.Value, nz.Value, _wrapX, _wrapY, _wrapZ, _sample);

                // set values
                if (vals != null)
                {
                    if (vals.Count == 1)
                        f.Set(vals[0].Value);
                    else
                        f.Set(vals.Select(x => (Vec3d)x.Value));
                }

                DA.SetData(0, new GH_ObjectWrapper(f));
            }
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
            get { return new Guid("{FE606239-5B89-4E63-85BD-00CD286A10E2}"); }
        }
    }
}
