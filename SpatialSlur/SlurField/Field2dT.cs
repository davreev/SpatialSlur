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
    [Serializable]
    public abstract class Field2d<T> : Field2d
    {
        private readonly T[] _values;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="boundaryType"></param>
        protected Field2d(Domain2d domain, int countX, int countY, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : base(domain, countX, countY, boundaryType)
        {
            _values = new T[Count + 1];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Field2d(Field2d other)
            : base(other)
        {
            _values = new T[Count + 1];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Field2d(Field2d<T> other)
            : base(other)
        {
            int n = Count + 1;
            _values = new T[n];
            Array.Copy(other._values, _values, n);
        }


        /// <summary>
        /// 
        /// </summary>
        public T[] Values
        {
            get { return _values; }
        }


        /// <summary>
        /// Gets or sets the value used outside the domain of the field.
        /// This only applies when BoundaryType is set to Constant.
        /// </summary>
        public T BoundaryValue
        {
            get { return _values[Count]; }
            set { _values[Count] = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T Evaluate(FieldPoint2d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Set(Field2d<T> other)
        {
            Array.Copy(other._values, _values, Count);
        }


        /// <summary>
        /// Sets a subset of this field to a given value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="parallel"></param>
        public void SetBlock(T value, Vec2i from, Vec2i to, bool parallel = false)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets all values along the boundary of the field to a given constant
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundary(T value)
        {
            // top/bottom
            int offset = Count - CountX;
            for (int i = 0; i < CountX; i++)
            {
                _values[i] = value;
                _values[i + offset] = value;
            }

            // left/right
            offset = CountX - 1;
            for (int i = CountX; i < Count - CountX; i += CountX)
            {
                _values[i] = value;
                _values[i + offset] = value;
            }
        }


        /// <summary>
        /// Sets this field to some function of itself.
        /// </summary>
        /// <param name="func"></param>
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
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <param name="other"></param>
        public void Function<U>(Func<U, T> func, Field2d<U> other)
        {
            var u = other.Values;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(u[i]);
            });
        }


        /// <summary>
        /// Sets this field to some function of itself and another field.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Function<U>(Func<T, U, T> func, Field2d<U> other)
        {
            var u = other.Values;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(_values[i], u[i]);
            });
        }


        /// <summary>
        ///  Sets this field to some function of two other fields.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="func"></param>
        /// <param name="otherU"></param>
        /// <param name="otherV"></param>
        public void Function<U, V>(Func<U, V, T> func, Field2d<U> otherU, Field2d<V> otherV)
        {
            var u = otherU.Values;
            var v = otherV.Values;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(u[i], v[i]);
            });
        }


        /// <summary>
        ///  Sets this field to some function of itself and two other fields.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="func"></param>
        /// <param name="otherU"></param>
        /// <param name="otherV"></param>
        public void Function<U, V>(Func<T, U, V, T> func, Field2d<U> otherU, Field2d<V> otherV)
        {
            var u = otherU.Values;
            var v = otherV.Values;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _values[i] = func(_values[i], u[i], v[i]);
            });
        }


        /// <summary>
        /// Sets a subset of this field to some function of itself.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Function(Func<T, T> func, Vec2i from, Vec2i to)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets this field to some function of its coordinates.
        /// </summary>
        /// <param name="func"></param>
        public void SpatialFunction(Func<Vec2d, T> func)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(CoordinateAt(i,j));
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its coordinates.
        /// </summary>
        /// <param name="func"></param>
        public void SpatialFunction(Func<double, double, T> func)
        {
            double x0 = Domain.x.t0;
            double y0 = Domain.y.t0;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i * ScaleX + x0, j * ScaleY + y0);
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its coordinates and another field.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <param name="other"></param>
        public void SpatialFunction<U>(Func<Vec2d, U, T> func, Field2d<U> other)
        {
            var u = other.Values;
     
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(CoordinateAt(i, j), u[index]);
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its coordinates and another field.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <param name="other"></param>
        public void SpatialFunction<U>(Func<double, double, U, T> func, Field2d<U> other)
        {
            double x0 = Domain.x.t0;
            double y0 = Domain.y.t0;
            var u = other.Values;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i * ScaleX + x0, j * ScaleY + y0, u[index]);
                }
            });
        }


        /// <summary>
        /// Sets a subset of this field to some function of its field points.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<Vec2d, T> func, Vec2i from, Vec2i to, bool parallel = false)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets a subset of this field to some function of its coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<double, double, double, T> func, Vec2i from, Vec2i to, bool parallel = false)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        public void NormSpatialFunction(Func<Vec2d, T> func)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(new Vec2d(i * ti, j * tj));
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        public void NormSpatialFunction(Func<double, double, T> func)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i * ti, j * tj);
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates and another field.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <param name="other"></param>
        public void NormSpatialFunction<U>(Func<Vec2d, U, T> func, Field2d<U> other)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            var u = other.Values;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(new Vec2d(i * ti, j * tj), u[index]);
                }
            });
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates and another field.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <param name="other"></param>
        public void NormSpatialFunction<U>(Func<double, double, U, T> func, Field2d<U> other)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            var u = other.Values;

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i * ti, j * tj, u[index]);
                }
            });
        }


        /// <summary>
        /// Sets a subset of this field to some function of its normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="parallel"></param>
        public void NormSpatialFunction(Func<Vec2d, T> func, Vec2i from, Vec2i to, bool parallel = false)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets a subset of this field to some function of its normalized coordinates and another field.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="parallel"></param>
        public void NormSpatialFunction(Func<double, double, T> func, Vec2i from, Vec2i to, bool parallel = false)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Samples another field of a different resolution.
        /// </summary>
        /// <param name="other"></param>
        public void Sample(Field2d<T> other)
        {
            if (ResolutionEquals(other))
            {
                Set(other);
                return;
            }

            Domain2d d = other.Domain;
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    int i, j;
                    ExpandIndex(range.Item1, out i, out j);
                    FieldPoint2d fp = new FieldPoint2d();

                    for (int index = range.Item1; index < range.Item2; index++, i++)
                    {
                        if (i == CountX) { j++; i = 0; }

                        Vec2d p = d.Evaluate(new Vec2d(i * ti, j * tj));
                        other.FieldPointAt(p, fp);
                        _values[index] = other.Evaluate(fp);
                    }
                });
        }

    }
     
}
