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
        /// <param name="other"></param>
        /// <param name="duplicateMesh"></param>
        protected MeshField(MeshField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="duplicateMesh"></param>
        protected MeshField(MeshField<T> other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
            _values = new T[Count];
            Array.Copy(other._values, _values, Count);
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
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Set(MeshField<T> other)
        {
            Array.Copy(other._values, _values, Count);
        }


        /// <summary>
        /// Sets this field to some function of itself.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void Function(Func<T, T> func, bool parallel = false)
        {
            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        _values[i] = func(_values[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    _values[i] = func(_values[i]);
            }
        }


        /// <summary>
        /// Sets this field to some function of another field.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void Function<U>(Func<U, T> func, MeshField<U> other, bool parallel = false)
        {
            SizeCheck(other);
            var u = other.Values;

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        _values[i] = func(u[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    _values[i] = func(u[i]);
            }
        }


        /// <summary>
        /// Sets this field to some function of itself and another field.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void Function<U>(Func<T, U, T> func, MeshField<U> other, bool parallel = false)
        {
            SizeCheck(other);
            var u = other.Values;

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        _values[i] = func(_values[i], u[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    _values[i] = func(_values[i], u[i]);
            }
        }


        /// <summary>
        /// Sets this field to some function of two other fields.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="func"></param>
        /// <param name="otherU"></param>
        /// <param name="otherV"></param>
        /// <param name="parallel"></param>
        public void Function<U, V>(Func<U, V, T> func, MeshField<U> otherU, MeshField<V> otherV, bool parallel = false)
        {
            SizeCheck(otherU);
            SizeCheck(otherV);
            var u = otherU.Values;
            var v = otherV.Values;

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        _values[i] = func(u[i], v[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    _values[i] = func(u[i], v[i]);
            }
        }


        /// <summary>
        /// Sets this field to some function of itself and two other fields.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="func"></param>
        /// <param name="otherU"></param>
        /// <param name="otherV"></param>
        /// <param name="parallel"></param>
        public void Function<U, V>(Func<T, U, V, T> func, MeshField<U> otherU, MeshField<V> otherV, bool parallel = false)
        {
            SizeCheck(otherU);
            SizeCheck(otherV);
            var u = otherU.Values;
            var v = otherV.Values;

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        _values[i] = func(_values[i], u[i], v[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    _values[i] = func(_values[i], u[i], v[i]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="parallel"></param>
        public void PaintDisplayMesh(Func<T, Color> mapper, bool parallel = false)
        {
            var vc = DisplayMesh.VertexColors;

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        vc[i] = mapper(Values[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    vc[i] = mapper(Values[i]);
            }
        }
    }
}
