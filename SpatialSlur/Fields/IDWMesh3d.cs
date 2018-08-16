
/*
 * Notes
 */

#if USING_RHINO

using System;
using Rhino.Geometry;
using SpatialSlur;
using SpatialSlur.Fields;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDWMesh3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="value"></param>
        /// <param name="influence"></param>
        /// <returns></returns>
        public static IDWMesh3d<T> Create<T>(Mesh mesh, T value, double influence = 1.0)
        {
            return new IDWMesh3d<T>()
            {
                Mesh = mesh,
                Value = value,
                Influence = influence
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IDWMesh3d<T> Create<T>(IDWMesh3d<T> other)
        {
            return new IDWMesh3d<T>()
            {
                Mesh = other.Mesh,
                Value = other.Value,
                Influence = other.Influence
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="other"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static IDWMesh3d<T> Create<T, U>(IDWMesh3d<U> other, Func<U, T> converter)
        {
            return new IDWMesh3d<T>()
            {
                Mesh = other.Mesh,
                Value = converter(other.Value),
                Influence = other.Influence
            };
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IDWMesh3d<T> : IDWObject3d<T>
    {
        private Mesh _mesh;


        /// <summary>
        /// 
        /// </summary>
        public Mesh Mesh
        {
            get => _mesh;
            set => _mesh = value;
        }


        /// <inheritdoc />
        public override double DistanceTo(Vector3d point)
        {
            return point.DistanceTo(_mesh.ClosestPoint(point));
        }


        /// <inheritdoc />
        public override IDWObject3d<T> Duplicate()
        {
            return IDWMesh3d.Create(this);
        }


        /// <inheritdoc />
        public override IDWObject3d<U> Convert<U>(Func<T, U> converter)
        {
            return IDWMesh3d.Create(this, converter);
        }
    }
}

#endif