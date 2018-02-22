using System;
using System.Collections.Generic;
using System.Drawing;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class GridField3dFactory<T>
        where T : struct
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <returns></returns>
        public abstract GridField3d<T> Create(int countX, int countY, int countZ);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        public GridField3d<T> Create(int countX, int countY, int countZ, Vec3d origin, Vec3d scale)
        {
            var result = Create(countX, countY, countZ);
            result.Origin = origin;
            result.Scale = scale;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        public GridField3d<T> Create(int countX, int countY, int countZ, Interval3d bounds)
        {
            var result = Create(countX, countY, countZ);
            result.Bounds = bounds;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <returns></returns>
        public abstract GridField3d<T> Create(Grid3d grid);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="other"></param>
        /// <param name="converter"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public GridField3d<T> Create<U>(GridField3d<U> other)
            where U : struct
        {
            var result = Create((Grid3d)other);
            result.SampleMode = other.SampleMode;
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
        public GridField3d<T> CreateCopy(GridField3d<T> other)
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
        public GridField3d<T> CreateCopy<U>(GridField3d<U> other, Func<U, T> converter, bool parallel = false)
            where U : struct
        {
            var result = Create(other);
            other.Convert(converter, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="images"></param>
        /// <param name="mapper"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public GridField3d<T> CreateFromImageStack(IList<Bitmap> images, Func<Color, T> mapper)
        {
            var bmp0 = images[0];
            var result = Create(bmp0.Width, bmp0.Height, images.Count);
            FieldIO.ReadFromImageStack(images, result, mapper);
            return result;
        }
    }
}