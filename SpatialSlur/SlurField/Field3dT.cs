using System;
using System.Collections.Concurrent;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 * 
 * TODO add setters for different boundary conditions (VonNeumann, Dirichlet, etc.)
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class Field3d<T> : Field3d, IField<T>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="path"></param>
        /// <param name="mapper"></param>
        public static void SaveAsImageStack(Field3d<T> field, string path, Func<T, Color> mapper)
        {
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            Parallel.For(0, field.CountZ, z =>
            {
                using (Bitmap bmp = new Bitmap(field.CountX, field.CountY, PixelFormat.Format32bppArgb))
                {
                    FieldIO.WriteToImage(field, z, bmp, mapper);
                    bmp.Save(String.Format(@"{0}\{1}_{2}{3}", dir, name, z, ext));
                }
            });
        }

        #endregion


        private readonly T[] _values;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="wrapMode"></param>
        protected Field3d(Domain3d domain, int nx, int ny, int nz, FieldWrapMode wrapMode)
            : base(domain, nx, ny, nz, wrapMode)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        protected Field3d(Domain3d domain, int nx, int ny, int nz, 
            FieldWrapMode wrapModeX = FieldWrapMode.Clamp, 
            FieldWrapMode wrapModeY = FieldWrapMode.Clamp, 
            FieldWrapMode wrapModeZ = FieldWrapMode.Clamp)
            : base(domain, nx, ny, nz, wrapModeX, wrapModeY, wrapModeZ)
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
        /// <param name="k"></param>
        /// <returns></returns>
        public T ValueAtUnchecked(int i, int j, int k)
        {
            return _values[IndexAtUnchecked(i, j, k)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public T ValueAtUnchecked(Vec3i indices)
        {
            return _values[IndexAtUnchecked(indices.x, indices.y, indices.z)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public T ValueAt(int i, int j, int k)
        {
            return _values[IndexAt(i, j, k)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public T ValueAt(Vec3i indices)
        {
            return _values[IndexAt(indices.x, indices.y, indices.z)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T ValueAt(FieldPoint3d point);


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitMask"></param>
        public void SetDirichletBoundary(T value, int direction)
        {
            // TODO
            throw new NotImplementedException();

            if ((bitMask & 1) != 0) ; // -x
            if ((bitMask & 2) != 0) ; // +x

            if ((bitMask & 4) != 0) ; // -y
            if ((bitMask & 8) != 0) ; // +y

            if ((bitMask & 16) != 0) ; // -z
            if ((bitMask & 32) != 0) ; // +z
        }
        */


        /// <summary>
        /// Sets all values along the boundary of the field to a given constant
        /// </summary>
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


        /*
        /// <summary>
        /// Sets a subset of this field to some function of itself.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="parallel"></param>
        public void Function(Func<T, T> func, Vec3i from, Vec3i to, bool parallel = false)
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
        public void SpatialFunction(Func<Vec3d, T> func, bool parallel = false)
        {
            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }
                    _values[index] = func(CoordinateAt(i, j, k));
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
        public void SpatialFunction(Func<double, double, double, T> func, bool parallel = false)
        {
            double x0 = Domain.x.t0;
            double y0 = Domain.y.t0;
            double z0 = Domain.z.t0;

            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }
                    _values[index] = func(i * ScaleX + x0, j * ScaleY + y0, k * ScaleZ + z0);
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
        public void SpatialFunctionNorm(Func<Vec3d, T> func, bool parallel = false)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            double tk = 1.0 / (CountZ - 1);

            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(new Vec3d(i * ti, j * tj, k * tk));
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
        public void SpatialFunctionNorm(Func<double, double, double, T> func, bool parallel = false)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            double tk = 1.0 / (CountZ - 1);

            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = func(i * ti, j * tj, k * tk);
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
        public void IndexedFunction(Func<Vec3i, T> func, bool parallel = false)
        {
            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }
                    _values[index] = func(new Vec3i(i, j, k));
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
        public void IndexedFunction(Func<int, int, int, T> func, bool parallel = false)
        {
            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }
                    _values[index] = func(i, j, k);
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
        public void ResampleNearest(Field3d<T> other, bool parallel = false)
        {
            if (ResolutionEquals(other))
            {
                _values.Set(other._values);
                return;
            }

            var otherVals = other.Values;

            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    _values[index] = otherVals[other.IndexAt(CoordinateAt(i, j, k))];
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Samples another field using bilinear interpolation of the 8 nearest values.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void ResampleLinear(Field3d<T> other, bool parallel = false)
        {
            if (ResolutionEquals(other))
            {
                _values.Set(other._values);
                return;
            }

            Action<Tuple<int, int>> func = range =>
            {
                FieldPoint3d fp = new FieldPoint3d();
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    other.FieldPointAt(CoordinateAt(i, j, k), fp);
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
