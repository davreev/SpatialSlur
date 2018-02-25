using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
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
        private SimState _state = SimState.Reset;

        private Action<HeMesh3d.Vertex, M.Vertex> _setV = (v0, v1) =>
        {
            v0.Position = v1.Position;
        };


        /// <summary>
        /// 
        /// </summary>
        public SimState State
        {
            get { return _state; }
            set { _state = value; }
        }


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
            pManager.AddGenericParameter("target", "target", "Surface feature to constrain to", GH_ParamAccess.item);

            pManager.AddGenericParameter("features", "features", "Features to maintain during remeshing", GH_ParamAccess.list);
            pManager.AddGenericParameter("lengthField", "lengthField", "Scalar field defining target edge lengths", GH_ParamAccess.item);

            pManager.AddGenericParameter("settings", "settings", "Remesher settings", GH_ParamAccess.item);
            pManager.AddIntegerParameter("subSteps", "subSteps", "The number of steps taken per solve.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("simState", "simState", "Simulation state (0 = Reset, 1 = Play, 2 = Pause)", GH_ParamAccess.item);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
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

            GH_ObjectWrapper settingsGoo = null;
            if (!DA.GetData(4, ref settingsGoo)) return;

            if (_solver == null)
                InitializeSolver(DA, (DynamicRemesher.Settings)settingsGoo.Value);
            else
                _solver.Settings = (DynamicRemesher.Settings)settingsGoo.Value;

            // update length field
            GH_ObjectWrapper fieldGoo = null;
            if (DA.GetData(3, ref fieldGoo))
                _solver.LengthField = (IField3d<double>)fieldGoo.Value;
            else
                _solver.LengthField = null;

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
        /// 
        /// </summary>
        /// <param name="DA"></param>
        private void InitializeSolver(IGH_DataAccess DA, DynamicRemesher.Settings settings)
        {
            GH_HeMesh3d source = null;
            if (!DA.GetData(0, ref source)) return;

            IGH_Goo targetGoo = null;
            if (!DA.GetData(1, ref targetGoo)) return;

            var featsGoo = new List<GH_ObjectWrapper>();
            DA.GetDataList(2, featsGoo);

            var target = CreateSurfaceFeature(targetGoo);
            var features = featsGoo.Select(obj => (IFeature)obj.Value).Concat(CreateBoundaryFeatures(source));
            _solver = DynamicRemesher.Solver.Create(source.Value, target, features, settings);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ISurfaceFeature CreateSurfaceFeature(IGH_Goo goo)
        {
            switch(goo)
            {
                case GH_ObjectWrapper obj:
                    return (ISurfaceFeature) obj.Value;
                case GH_Mesh mesh:
                    return new MeshFeature(mesh.Value);
                default:
                    return null;
            }
        }


        /// <summary>
        /// Create and assign boundary features
        /// </summary>
        private IEnumerable<IFeature> CreateBoundaryFeatures(HeMesh3d mesh)
        {
            foreach (var he0 in mesh.GetHoles())
            {
                var poly = new Polyline(he0.Circulate.Select(he1 => (Point3d)he1.Start.Position));
                poly.Add(poly[0]);

                yield return new MeshFeature(RhinoFactory.Mesh.CreateExtrusion(poly, new Vector3d()));
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
            get { return new Guid("{5A07A030-B0C7-47F8-8E42-60D63610BEB7}"); }
        }
    }
}

