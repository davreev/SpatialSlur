using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

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
        protected MeshField(MeshField<T> other)
            : base(other.Mesh)
        {
            _values = new T[Count];
            Set(other);
        }


        /// <summary>
        /// 
        /// </summary>
        public T[] Values
        {
            get { return _values; }
        }


        /// <summary>
        /// Assumes weights sum to 1.0.
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        public abstract T Evaluate(int i0, int i1, int i2, double w0, double w1, double w2);


        /// <summary>
        /// Assumes weights sum to 1.0.
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public abstract T Evaluate(IList<int> indices, IList<double> weights);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Set(MeshField<T> other)
        {
            other._values.CopyTo(_values, 0);
        }


        /// <summary>
        /// Sets this field to some function of itself.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void Function(Func<T, T> func, bool parallel = false)
        {
            if (parallel)
                VecMath.FunctionParallel(func, Values, Count, Values);
            else
                VecMath.Function(func, Values, Count, Values);
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
            if (parallel)
                VecMath.FunctionParallel(func, other.Values, Count, Values);
            else
                VecMath.Function(func, other.Values, Count, Values);
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
            if (parallel)
                VecMath.FunctionParallel(func, Values, other.Values, Count, Values);
            else
                VecMath.Function(func, Values, other.Values, Count, Values);
        }


        /// <summary>
        /// Sets this field to some function of two other fields.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="func"></param>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="parallel"></param>
        public void Function<U, V>(Func<U, V, T> func, MeshField<U> f0, MeshField<V> f1, bool parallel = false)
        {
            if (parallel)
                VecMath.FunctionParallel(func, f0.Values, f1.Values, Count, Values);
            else
                VecMath.Function(func, f0.Values, f1.Values, Count, Values);
        }


        /// <summary>
        /// Sets this field to some function of itself and two other fields.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="func"></param>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="parallel"></param>
        public void Function<U, V>(Func<T, U, V, T> func, MeshField<U> f0, MeshField<V> f1, bool parallel = false)
        {
            if (parallel)
                VecMath.FunctionParallel(func, Values, f0.Values, f1.Values, Count, Values);
            else
                VecMath.Function(func, Values, f0.Values, f1.Values, Count, Values);
        }


        /// <summary>
        /// Sets this field to some function of its vertex positions.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<Vec3d, T> func, bool parallel = false)
        {
            var verts = Mesh.Vertices;

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        _values[i] = func(verts[i].Position);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    _values[i] = func(verts[i].Position);
            }
        }


        /// <summary>
        /// Sets this field to some function of its vertex positions and another field.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction<U>(Func<Vec3d, U, T> func, MeshField<U> other, bool parallel = false)
        {
            SizeCheck(other);
            var verts = Mesh.Vertices;
            var u = other.Values;

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        _values[i] = func(verts[i].Position, u[i]);
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    _values[i] = func(verts[i].Position, u[i]);
            }
        }
    }
}
