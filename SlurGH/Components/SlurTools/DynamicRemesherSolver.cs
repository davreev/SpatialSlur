using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;

using SpatialSlur.SlurRhino;
using SpatialSlur.SlurTools;
using SpatialSlur.SlurTools.Features;

using SpatialSlur.SlurGH.Types;
using SpatialSlur.SlurGH.Params;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
{
    using M = DynamicRemesher.HeMesh;

    /// <summary>
    /// 
    /// </summary>
    public class DynamicRemesherSolver : GH_Component
    {
        private DynamicRemesher.Solver _solver;
        private StringBuilder _print = new StringBuilder();


        private Action<HeMesh3d.Vertex, M.Vertex> _setV = (v0, v1) =>
        {
            v0.Position = v1.Position;
        };


        /// <summary>
        /// 
        /// </summary>
        public DynamicRemesherSolver()
          : base("Dynamic Remesher", "Remesher",
              "Dynamically remeshes a given mesh.",
              "SpatialSlur", "Mesh")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "source", "source", "Mesh to start from", GH_ParamAccess.item);
            pManager.AddMeshParameter("target", "target", "Mesh to constrain to", GH_ParamAccess.item);
            pManager.AddGenericParameter("features", "features", "Features to maintain during remeshing", GH_ParamAccess.list);
            pManager.AddGenericParameter("lengthField", "lengthField", "Scalar field defining target edge lengths", GH_ParamAccess.item);

            pManager.AddGenericParameter("settings", "settings", "Remesher settings", GH_ParamAccess.item);
            pManager.AddIntegerParameter("subSteps", "subSteps", "The number of steps taken per solve.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("simState", "simState", "Simulation state (0 = Reset, 1 = Play, 2 = Pause)", GH_ParamAccess.item);

            pManager[2].Optional = true;
            pManager[4].Optional = true;
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("out", "out", "", GH_ParamAccess.item);
            pManager.AddParameter(new HeMesh3dParam(), "result", "result", "Remeshed result", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int state = 0;
            if (!DA.GetData(6, ref state)) return;

            // reset
            if (state == 0)
            {
                _solver = null;
                return;
            }

            // initialize
            if (_solver == null)
            {
                GH_HeMesh3d source = null;
                if (!DA.GetData(0, ref source)) return;

                GH_Mesh target = null;
                if (!DA.GetData(1, ref target)) return;

                var feats = new List<GH_ObjectWrapper>();
                DA.GetDataList(2, feats);

                _solver = DynamicRemesher.Solver.Create(
                    source.Value.Duplicate(), new MeshFeature(target.Value),
                    feats.Select(f => (IFeature)f.Value)
                    );
            }

            // update dynamic parameters
            GH_ObjectWrapper goo = null;

            // update fields
            if (DA.GetData(3, ref goo))
                _solver.LengthField = (IField3d<double>)goo.Value;
            else
                _solver.LengthField = null;

            // update settings
            if (!DA.GetData(4, ref goo)) return;
            _solver.Settings = (DynamicRemesher.Settings)goo.Value;

            int subSteps = 0;
            DA.GetData(5, ref subSteps);

            // step
            for (int i = 0; i < subSteps; i++)
                _solver.Step();

            _print.AppendLine($"{_solver.StepCount} steps");

            // output
            DA.SetData(0, new GH_String(_print.ToString()));
            DA.SetData(1, new GH_HeMesh3d(HeMesh3d.Factory.CreateCopy(_solver.Mesh, _setV, delegate { }, delegate { })));

            // recall
            if (state == 1)
                ExpireSolution(true);

            _print.Clear();
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
            get { return new Guid("{5A07A030-B0C7-47F8-8E42-60D63610BEB7}"); }
        }
    }
}

