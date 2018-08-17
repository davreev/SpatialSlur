
/*
 * Notes 
 */

using System.Linq;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using SpatialSlur;
using SpatialSlur.Meshes;
using SpatialSlur.Rhino;

namespace SpatialSlur.Grasshopper.Types
{
    /// <summary>
    /// 
    /// </summary>
    public class GH_HeGraph3d : GH_GeometricGoo<HeGraph3d>
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        public static implicit operator HeGraph3d(GH_HeGraph3d graph)
        {
            return graph.Value;
        }

        #endregion


        private BoundingBox _bbox = BoundingBox.Unset;


        /// <summary>
        /// 
        /// </summary>
        public GH_HeGraph3d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public GH_HeGraph3d(HeGraph3d value)
        {
            Value = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GH_HeGraph3d(GH_HeGraph3d other)
        {
            Value = other.Value;
        }


        /// <inheritdoc />
        public override bool IsValid
        {
            get { return true; }
        }


        /// <inheritdoc />
        public override string TypeName
        {
            get { return "HeGraph"; }
        }


        /// <inheritdoc />
        public override string TypeDescription
        {
            get { return "HeGraph"; }
        }


        /// <inheritdoc />
        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value != null && !_bbox.IsValid)
                    _bbox = new Interval3d(Value.Vertices.Select(v => v.Position)).ToBoundingBox();

                return _bbox;
            }
        }


        /// <inheritdoc />
        public override IGH_Goo Duplicate()
        {
            return DuplicateGeometry();
        }


        /// <inheritdoc />
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return new GH_HeGraph3d(new HeGraph3d(Value));
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return Value.ToString();
        }


        /// <inheritdoc />
        public override object ScriptVariable()
        {
            return Value;
        }


        /// <inheritdoc />
        public override bool CastTo<T>(ref T target)
        {
            if (typeof(T).IsAssignableFrom(typeof(HeGraph3d)))
            {
                object obj = Value;
                target = (T)obj;
                return true;
            }

            if (typeof(T).IsAssignableFrom(typeof(GH_ObjectWrapper)))
            {
                object obj = new GH_ObjectWrapper(Value);
                target = (T)obj;
                return true;
            }

            return false;
        }


        /// <inheritdoc />
        public override bool CastFrom(object source)
        {
            if (source is HeGraph3d g)
            {
                Value = g;
                return true;
            }

            if (source is HeMesh3d hem)
            {
                Value = HeGraph3d.Factory.CreateFromVertexTopology(hem);
                return true;
            }

            if (source is Mesh m)
            {
                base.Value = m.ToHeGraph();
                return true;
            }

            if (source is GH_HeMesh3d ghem)
            {
                Value = HeGraph3d.Factory.CreateFromVertexTopology(ghem.Value);
                return true;
            }

            if (source is GH_Mesh gm)
            {
                Value = gm.Value.ToHeGraph();
                return true;
            }
            
            return false;
        }


        /// <inheritdoc />
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            var b = Boundingbox;
            b.Transform(xform);
            return b;
        }


        /// <inheritdoc />
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            Value.Transform(xform);
            return this;
        }


        /// <inheritdoc />
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            Value.SpaceMorph(xmorph);
            return this;
        }
    }
}