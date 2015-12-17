using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Field3d<T> : Field3d
    {
        private readonly T[] _values;
        private T _boundaryValue;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="boundaryType"></param>
        protected Field3d(Domain3d domain, int nx, int ny, int nz, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : base(domain, nx, ny, nz, boundaryType)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Field3d(Field3d other)
            : base(other)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Field3d(Field3d<T> other)
            : base(other)
        {
            _values = new T[Count];
            _values.Set(other._values);
            _boundaryValue = other._boundaryValue;
        }


        /// <summary>
        /// 
        /// </summary>
        public T[] Values
        {
            get { return _values; }
        }


        /// <summary>
        /// Gets/sets the constant value 
        /// only applies when BoundaryType is set to Constant
        /// </summary>
        public T BoundaryValue
        {
            get { return _boundaryValue; }
            set { _boundaryValue = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T Evaluate(FieldPoint3d point);


        /// <summary>
        /// TODO clean up with SetBlock()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void SetBoundary(T value)
        {
            int i0, i1;

            // top/bottom
            for (int i = 0; i < CountY; i++)
            {
                i0 = i * CountX;
                i1 = i0 + Count - CountXY;

                for (int j = 0; j < CountX; j++, i0++, i1++)
                {
                    _values[i0] = value;
                    _values[i1] = value;
                }
            }

            // front/back
            for (int i = 0; i < CountZ; i++)
            {
                i0 = i * CountXY;
                i1 = i0 + CountXY - CountX;

                for (int j = 0; j < CountX; j++, i0++, i1++)
                {
                    _values[i0] = value;
                    _values[i1] = value;
                }
            }

            // left/right
            for (int i = 0; i < CountZ; i++)
            {
                i0 = i * CountXY;
                i1 = i0 + CountX - 1;

                for (int j = 0; j < CountY; j++, i0 += CountX, i1 += CountX)
                {
                    _values[i0] = value;
                    _values[i1] = value;
                }
            }
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void SetBlock(T value, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// sets this field to some function of itself
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
        /// sets the result to some function of this field
        /// </summary>
        /// <param name="result"></param>
        public void Function<U>(Func<T, U> func, Field3d<U> result)
        {
            Function(func, result._values);
        }


        /// <summary>
        /// sets the result to some function of this field
        /// </summary>
        /// <param name="result"></param>
        public void Function<U>(Func<T, U> func, IList<U> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = func(_values[i]);
            });
        }


        /// <summary>
        /// sets this field to some function of itself and another field
        /// </summary>
        /// <param name="func"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Function<U>(Func<T, U, T> func, Field3d<U> other)
        {
            SizeCheck(other);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(_values[i], other._values[i]);
            });
        }


        /// <summary>
        /// sets the result to some function of this field and another field
        /// </summary>
        /// <param name="func"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Function<U, V>(Func<T, U, V> func, Field3d<U> other, Field3d<V> result)
        {
            Function(func, other, result._values);
        }


        /// <summary>
        /// sets the result to some function of this field and another field
        /// </summary>
        /// <param name="func"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Function<U, V>(Func<T, U, V> func, Field3d<U> other, IList<V> result)
        {
            SizeCheck(other);
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = func(_values[i], other._values[i]);
            });
        }


        /// <summary>
        /// TODO
        /// sets this field to some function of itself
        /// </summary>
        /// <param name="result"></param>
        public void Function(Func<T, T> func, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// sets this field to some function of its field points
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SpatialFunction(Func<Vec3d, T> func)
        {
            Vec3d p0 = Domain.From;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                this.ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(p0 + new Vec3d(i * ScaleX, j * ScaleY, k * ScaleZ));
                }
            });
        }


        /// <summary>
        /// sets this field to some function of its field points
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SpatialFunction(Func<double, double, double, T> func)
        {
            double x0 = Domain.x.t0;
            double y0 = Domain.y.t0;
            double z0 = Domain.z.t0;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                this.ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(x0 + i * ScaleX, y0 + j * ScaleY, z0 + k * ScaleZ);
                }
            });
        }


        /// <summary>
        /// TODO
        /// sets this field to some function of its field points
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SpatialFunction(Func<Vec3d, T> func, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// TODO
        /// sets this field to some function of its field points
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SpatialFunction(Func<double, double, double, T> func, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// sets this field to some function of its normalized field points
        /// </summary>
        /// <param name="func"></param>
        public void NormSpatialFunction(Func<Vec3d, T> func)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            double tk = 1.0 / (CountZ - 1);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                this.ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(new Vec3d(i * ti, j * tj, k * tk));
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its normalized field points.
        /// </summary>
        /// <param name="func"></param>
        public void NormSpatialFunction(Func<double, double, double, T> func)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            double tk = 1.0 / (CountZ - 1);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                this.ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(i * ti, j * tj, k * tk);
                }
            });
        }


        /// <summary>
        /// TODO
        /// sets this field to some function of its normalized field points
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void NormSpatialFunction(Func<Vec3d, T> func, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// TODO
        /// sets this field to some function of its normalized field points
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void NormSpatialFunction(Func<double, double, double, T> func, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Samples another field of a different resolution.
        /// </summary>
        /// <param name="other"></param>
        public void Sample(Field3d<T> other)
        {
            if (ResolutionEquals(other))
            {
                _values.Set(other._values);
                return;
            }

            Domain3d d = other.Domain;
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            double tk = 1.0 / (CountZ - 1);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);
                FieldPoint3d fp = new FieldPoint3d();

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    Vec3d p = d.Evaluate(new Vec3d(i * ti, j * tj, k * tk));
                    fp = other.FieldPointAt(p);
                    _values[i] = other.Evaluate(fp);
                }
            });
        }
    }
}
