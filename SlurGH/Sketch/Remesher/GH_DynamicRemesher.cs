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

using SpatialSlur.SlurRhino.Remesher;

using SpatialSlur.SlurGH.Types;
using SpatialSlur.SlurGH.Params;

/*
 * Notes
 * 
 * TODO check GH duplication
 */

namespace SpatialSlur.SlurGH.Remesher
{
    /// <summary>
    /// 
    /// </summary>
    public class GH_DynamicRemesher : GH_Component
    {
        private DynamicRemesher _remesher;
        private StringBuilder _print = new StringBuilder();


        private Action<HeMesh3d.Vertex, HeMeshSim.Vertex> _setV = (v0, v1) =>
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        };


        /// <summary>
        /// 
        /// </summary>
        public GH_DynamicRemesher()
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
            pManager.AddGenericParameter("features", "features", "Additional features to constrain to", GH_ParamAccess.list);
            pManager.AddGenericParameter("lengthField", "lengthField", "Scalar field defining target edge lengths", GH_ParamAccess.item);
            pManager.AddGenericParameter("settings", "settings", "Remesher settings", GH_ParamAccess.item);
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
            // pManager.AddGenericParameter("debug", "debug", "", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int state = 0;
            if (!DA.GetData(5, ref state)) return;

            // reset
            if (state == 0)
            {
                _remesher = null;
                return;
            }

            // initialize
            if (_remesher == null)
            {
                HeMesh3d source = null;
                Mesh target = null;
               
                if (!DA.GetData(0, ref source)) return;
                if (!DA.GetData(1, ref target)) return;

                var feats = new List<GH_ObjectWrapper>(); 
                DA.GetDataList(2, feats);
                
                _remesher = DynamicRemesher.Create(source.Duplicate(), new MeshFeature(target), feats.Select(f => (IFeature)f.Value));
            }

            // update dynamic parameters
            GH_ObjectWrapper field = null;
            if (DA.GetData(3, ref field))
                _remesher.EdgeLengthField = (IField3d<double>)field.Value;
            else
                _remesher.EdgeLengthField = null;

            GH_ObjectWrapper settings = null;
            if (!DA.GetData(4, ref settings)) return;
            _remesher.Settings = (DynamicRemesherSettings)settings.Value;

            // step
            _remesher.Step();
            _print.AppendLine($"{_remesher.StepCount} steps");

            // output
            DA.SetData(0, new GH_String(_print.ToString()));
            DA.SetData(1, HeMesh3d.Factory.CreateCopy(_remesher.Mesh, _setV, delegate { }, delegate { }));
            // DA.SetData(2, _remesher.Mesh);

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

