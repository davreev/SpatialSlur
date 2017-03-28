using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using SpatialSlur.SlurCore;

/*
 * Notes
 * TODO add set methods for different boundary conditions (VonNeumann, Dirichlet, etc.)
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class Field2d<T> : Field2d, IField<T>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="path"></param>
        /// <param name="mapper"></param>
        public static void SaveAsImage(Field2d<T> field, string path, Func<T, Color> mapper)
        {
            using (Bitmap bmp = new Bitmap(field.CountX, field.CountY, PixelFormat.Format32bppArgb))
            {
                FieldIO.WriteToImage(field, bmp, mapper);
                bmp.Save(path);
            }
        }

        #endregion


        private readonly T[] _values;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapMode"></param>
        protected Field2d(Domain2d domain, int countX, int countY, FieldWrapMode wrapMode)
            : base(domain, countX, countY, wrapMode)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        protected Field2d(Domain2d domain, int countX, int countY, FieldWrapMode wrapModeX = FieldWrapMode.Clamp, FieldWrapMode wrapModeY = FieldWrapMode.Clamp)
            : base(domain, countX, countY, wrapModeX, wrapModeY)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Field2d(Field2d other)
            : base(other)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Field2d(Field2d<T> other)
            : base(other)
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
        /// <param name="i"></param>
        /// <param name="j"></param>
        public T ValueAtUnchecked(int i, int j)
        {
            return _values[IndexAtUnchecked(i, j)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public T ValueAtUnchecked(Vec2i indices)
        {
            return _values[IndexAtUnchecked(indices.x, indices.y)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public T ValueAt(int i, int j)
        {
            return _values[IndexAt(i, j)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public T ValueAt(Vec2i indices)
        {
            return _values[IndexAt(indices.x, indices.y)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T ValueAt(FieldPoint2d point);
 

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        public void SetDirichletBoundary(T value, int direction)
        {
            // TODO
            throw new NotImplementedException();
        }
        */


        /// <summary>
        /// Sets all values along the boundary of the field to a given constant
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundary(T value)
        {
            SetBoundaryX(value);
            SetBoundaryY(value);
        }


        /// <summary>
        /// Sets all values along the X boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryX(T value)
        {
            int offset = Count - CountX;

            for (int i = 0; i < CountX; i++)
                _values[i] = _values[i + offset] = value;
        }


        /// <summary>
        /// Sets all values along the Y boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryY(T value)
        {
            int offset = CountX - 1;

            for (int i = 0; i < Count; i += CountX)
                _values[i] = _values[i + offset] = value;
        }


        /// <summary>
        /// Sets all values along the lower X boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryX0(T value)
        {
            for (int i = 0; i < CountX; i++)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the upper X boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryX1(T value)
        {
            for (int i = Count - CountX; i < Count; i++)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the lower Y boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryY0(T value)
        {
            for (int i = 0; i < Count; i += CountX)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the upper Y boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryY1(T value)
        {
            for (int i = CountX - 1; i < Count; i += CountX)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all corner values to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetCorners(T value)
        {
            _values[0] = _values[CountX - 1] = _values[Count - 1] = _values[Count - CountX] = value;
        }


        /// <summary>
        /// Sets a block of values to a given constant.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void SetBlock(T value, Vec2i from, Vec2i to)
        {
            SetBlock(value, from.x, from.y, to.x, to.y);
        }


        /// <summary>
        /// Sets a block of values to a given constant.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="i0"></param>
        /// <param name="j0"></param>
        /// <param name="i1"></param>
        /// <param name="j1"></param>
        public void SetBlock(T value, int i0, int j0, int i1, int j1)
        {
            // TODO
            throw new NotImplementedException();
        }


        /*
        /// <summary>
        /// Sets a subset of this field to some function of itself.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="parallel"></param>
        public void Function(Func<T, T> func, Vec2i from, Vec2i to, bool parallel = false)
        {
            // TODO
            throw new NotImplementedException();
        }
        */


        /// <summary>
        /// Sets this field to some function of its coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<Vec2d, T> func, bool parallel = false)
        {
            Action<Tuple<int, int>> func2 = range =>
            {
                int i, j;
                IndicesAt(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(CoordinateAt(i, j));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<double, double, T> func, bool parallel = false)
        {
            double x0 = Domain.x.t0;
            double y0 = Domain.y.t0;

            Action<Tuple<int, int>> func2 = range =>
            {
                int i, j;
                IndicesAt(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i * ScaleX + x0, j * ScaleY + y0);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionNorm(Func<Vec2d, T> func, bool parallel = false)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);

            Action<Tuple<int, int>> func2 = range =>
            {
                int i, j;
                IndicesAt(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(new Vec2d(i * ti, j * tj));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionNorm(Func<double, double, T> func, bool parallel = false)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);

            Action<Tuple<int, int>> func2 = range =>
            {
                int i, j;
                IndicesAt(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i * ti, j * tj);
                }
            };


            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its indices.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void IndexedFunction(Func<Vec2i, T> func, bool parallel = false)
        {
            Action<Tuple<int, int>> func2 = range =>
            {
                int i, j;
                IndicesAt(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(new Vec2i(i, j));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its indices.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void IndexedFunction(Func<int, int, T> func, bool parallel = false)
        {
            Action<Tuple<int, int>> func2 = range =>
            {
                int i, j;
                IndicesAt(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i, j);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Samples another field nearest using the nearest value.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void ResampleNearest(Field2d<T> other, bool parallel = false)
        {
            if (ResolutionEquals(other))
            {
                _values.Set(other._values);
                return;
            }

            var otherVals = other._values;

            Action<Tuple<int, int>> func = range =>
            {
                int i, j;
                IndicesAt(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    _values[index] = otherVals[other.IndexAt(CoordinateAt(i, j))];
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Samples another field using bilinear interpolation of the 4 nearest values.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void ResampleLinear(Field2d<T> other, bool parallel = false)
        {
            if (ResolutionEquals(other))
            {
                _values.Set(other._values);
                return;
            }

            Action<Tuple<int, int>> func = range =>
            {
                FieldPoint2d fp = new FieldPoint2d();
                int i, j;
                IndicesAt(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }

                    other.FieldPointAt(CoordinateAt(i, j), fp);
                    _values[index] = other.ValueAt(fp);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }
    }
     
}
