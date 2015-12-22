using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurMesh;
using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MeshField<T> : MeshField
    {
        private readonly T[] _values;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        protected MeshField(HeMesh mesh)
            : base(mesh)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        protected MeshField(MeshField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        protected MeshField(MeshField<T> other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
            _values = new T[Count];
            _values.Set(other._values);
        }


        /// <summary>
        /// 
        /// </summary>
        public T[] Values
        {
            get { return _values; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T Evaluate(MeshPoint point);


        /// <summary>
        /// Sets this field to some function of itself.
        /// </summary>
        /// <param name="result"></param>
        public void Function(Func<T, T> func)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(_values[i]);
            });
        }


        /// <summary>
        /// Sets this field to some function of another field.
        /// </summary>
        /// <param name="result"></param>
        public void Function<U>(Func<U, T> func, MeshField<U> other)
        {
            SizeCheck(other);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(other._values[i]);
            });
        }


        /// <summary>
        /// Sets this field to some function of itself and another field.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Function<U>(Func<T, U, T> func, MeshField<U> other)
        {
            SizeCheck(other);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(_values[i], other._values[i]);
            });
        }


        /// <summary>
        /// Sets this field to some function of two other fields.
        /// </summary>
        /// <param name="result"></param>
        public void Function<U, V>(Func<U, V, T> func, MeshField<U> otherU, MeshField<V> otherV)
        {
            SizeCheck(otherU);
            SizeCheck(otherV);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(otherU._values[i], otherV._values[i]);
            });
        }


        /// <summary>
        /// Sets this field to some function of itself and two other fields.
        /// </summary>
        /// <param name="result"></param>
        public void Function<U, V>(Func<T, U, V, T> func, MeshField<U> otherU, MeshField<V> otherV)
        {
            SizeCheck(otherU);
            SizeCheck(otherV);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(_values[i], otherU._values[i], otherV._values[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        public void PaintDisplayMesh(Func<T, Color> mapper)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    DisplayMesh.VertexColors[i] = mapper(Values[i]);
            });
        }
    }
}
