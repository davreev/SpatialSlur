
/*
 * Notes
 */
 
 using System.Linq;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur;
using SpatialSlur.Meshes;
using SpatialSlur.Rhino;

namespace SpatialSlur.Grasshopper.Types
{
    /// <summary>
    /// 
    /// </summary>
    public class GH_HeMesh3d : GH_GeometricGoo<HeMesh3d>
    {
        #region Static members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public static implicit operator HeMesh3d(GH_HeMesh3d mesh)
        {
            return mesh.Value;
        }

        #endregion


        private BoundingBox _bbox = BoundingBox.Unset;


        /// <summary>
        /// 
        /// </summary>
        public GH_HeMesh3d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public GH_HeMesh3d(HeMesh3d value)
        {
            Value = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GH_HeMesh3d(GH_HeMesh3d other)
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
            get { return "HeMesh"; }
        }


        /// <inheritdoc />
        public override string TypeDescription
        {
            get { return "HeMesh"; }
        }


        /// <inheritdoc />
        public override BoundingBox Boundingbox
        {
            get
            {
                if(Value != null && !_bbox.IsValid)
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
            return new GH_HeMesh3d(new HeMesh3d(Value));
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
            if (typeof(T).IsAssignableFrom(typeof(HeMesh3d)))
            {
                object obj = Value;
                target = (T)obj;
                return true;
            }

            if (typeof(T).IsAssignableFrom(typeof(Mesh)))
            {
                object obj = new GH_Mesh(base.Value.ToMesh());
                target = (T)obj;
                return true;
            }

            if (typeof(T).IsAssignableFrom(typeof(GH_Mesh)))
            {
                object obj = new GH_Mesh(Value.ToMesh());
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
            if (source is HeMesh3d hem)
            {
                Value = hem;
                return true;
            }

            if (source is Mesh m)
            {
                base.Value = m.ToHeMesh();
                return true;
            }

            if (source is GH_Mesh gm)
            {
                Value = gm.Value.ToHeMesh();
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
