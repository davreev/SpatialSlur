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
        ///
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
        public void Function<U>(Func<U, T> func, Field3d<U> other)
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
        /// Sets this field to some function of two other fields.
        /// </summary>
        /// <param name="result"></param>
        public void Function<U, V>(Func<U, V, T> func, Field3d<U> otherU, Field3d<V> otherV)
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
        public void Function<U, V>(Func<T, U, V, T> func, Field3d<U> otherU, Field3d<V> otherV)
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
        /// TODO
        /// Sets this field to some function of itself.
        /// </summary>
        /// <param name="result"></param>
        public void Function(Func<T, T> func, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets this field to some function of its field points.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SpatialFunction(Func<Vec3d, T> func)
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

                    _values[index] = func(new Vec3d(i * ScaleX + x0, j * ScaleY + y0, k * ScaleZ + z0));
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its field points.
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
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(i * ScaleX + x0, j * ScaleY + y0, k * ScaleZ + z0);
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its field points and another field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SpatialFunction<U>(Func<Vec3d, U, T> func, Field3d<U> other)
        {
            SizeCheck(other);
            double x0 = Domain.x.t0;
            double y0 = Domain.y.t0;
            double z0 = Domain.z.t0;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(new Vec3d(i * ScaleX + x0, j * ScaleY + y0, k * ScaleZ + z0), other._values[index]);
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its field points and another field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SpatialFunction<U>(Func<double, double, double, U, T> func, Field3d<U> other)
        {
            SizeCheck(other);
            double x0 = Domain.x.t0;
            double y0 = Domain.y.t0;
            double z0 = Domain.z.t0;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(i * ScaleX + x0, j * ScaleY + y0, k * ScaleZ + z0, other._values[index]);
                }
            });
        }


        /// <summary>
        /// TODO
        /// Sets this field to some function of its field points.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SpatialFunction(Func<Vec3d, T> func, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// TODO
        /// Sets this field to some function of its field points.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SpatialFunction(Func<double, double, double, T> func, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets this field to some function of its normalized field points.
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
                ExpandIndex(range.Item1, out i, out j, out k);

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
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(i * ti, j * tj, k * tk);
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its normalized field points and another field.
        /// </summary>
        /// <param name="func"></param>
        public void NormSpatialFunction<U>(Func<Vec3d, U, T> func, Field3d<U> other)
        {
            SizeCheck(other);
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            double tk = 1.0 / (CountZ - 1);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(new Vec3d(i * ti, j * tj, k * tk), other._values[index]);
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its normalized field points and another field.
        /// </summary>
        /// <param name="func"></param>
        public void NormSpatialFunction<U>(Func<double, double, double, U, T> func, Field3d<U> other)
        {
            SizeCheck(other);
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            double tk = 1.0 / (CountZ - 1);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(i * ti, j * tj, k * tk, other._values[index]);
                }
            });
        }


        /// <summary>
        /// TODO
        /// Sets this field to some function of its normalized field points.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void NormSpatialFunction(Func<Vec3d, T> func, Vec3i from, Vec3i to)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// TODO
        /// Sets this field to some function of its normalized field points.
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
                    other.FieldPointAt(p, fp);
                    _values[index] = other.Evaluate(fp);
                }
            });
        }
    }
}
