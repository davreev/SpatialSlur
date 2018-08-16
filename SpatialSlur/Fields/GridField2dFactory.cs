
/*
 * Notes
 */

using System;
using System.Drawing;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class GridField2dFactory<T>
        where T : struct
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <returns></returns>
        public abstract GridField2d<T> Create(int countX, int countY);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        public GridField2d<T> Create(int countX, int countY, Vector2d origin, Vector2d scale)
        {
            var result = Create(countX, countY);
            result.Origin = origin;
            result.Scale = scale;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public GridField2d<T> Create(int countX, int countY, Interval2d bounds)
        {
            var result = Create(countX, countY);
            result.Bounds = bounds;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public abstract GridField2d<T> Create(Grid2d grid);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public GridField2d<T> Create<U>(GridField2d<U> other)
            where U : struct
        {
            var result = Create((Grid2d)other);
            result.SampleMode = other.SampleMode;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public GridField2d<T> CreateCopy(GridField2d<T> other)
        {
            var result = Create(other);
            result.Set(other);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="other"></param>
        /// <param name="converter"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridField2d<T> CreateCopy<U>(GridField2d<U> other, Func<U, T> converter, bool parallel = false)
            where U : struct
        {
            var result = Create(other);
            other.Convert(converter, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public GridField2d<T> CreateFromImage(Bitmap image, Func<Color, T> mapper)
        {
            var result = Create(image.Width, image.Height);
            Interop.Fields.ReadFromImage(image, result, mapper);
            return result;
        }
    }
}
