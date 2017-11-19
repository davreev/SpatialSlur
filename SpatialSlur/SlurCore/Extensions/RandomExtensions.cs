using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Interval1d NextInterval(this Random random)
        {
            return new Interval1d(random.NextDouble(), random.NextDouble());
        }


        /// <summary>
        /// returns a random 2d vector with a 0.0 to 1.0 range in each dimension
        /// </summary>
        /// <param name="random"></param>
        public static Vec2d NextVec2d(this Random random)
        {
            return new Vec2d(random.NextDouble(), random.NextDouble());
        }


        /// <summary>
        /// Returns a random vector which has components within the given interval.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Vec2d NextVec2d(this Random random, double t0, double t1)
        {
            return new Vec2d(
                SlurMath.Lerp(t0, t1, random.NextDouble()),
                SlurMath.Lerp(t0, t1, random.NextDouble()));
        }


        /// <summary>
        /// Returns a random 2d vector which has components within the given interval
        /// </summary>
        /// <param name="random"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Vec2d NextVec2d(this Random random, Interval1d interval)
        {
            return new Vec2d(
                interval.Evaluate(random.NextDouble()),
                interval.Evaluate(random.NextDouble()));
        }


        /// <summary>
        /// Returns a random 2d vector which has components within the given interval
        /// </summary>
        /// <param name="random"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Vec2d NextVec2d(this Random random, Interval2d interval)
        {
            return new Vec2d(
                interval.X.Evaluate(random.NextDouble()),
                interval.Y.Evaluate(random.NextDouble()));
        }


        /// <summary>
        /// returns a random vector with a 0.0 to 1.0 range in each dimension.
        /// </summary>
        /// <param name="random"></param>
        public static Vec3d NextVec3d(this Random random)
        {
            return new Vec3d(random.NextDouble(), random.NextDouble(), random.NextDouble());
        }


        /// <summary>
        /// Returns a random vector which has components within the given interval.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Vec2d NextVec3d(this Random random, double t0, double t1)
        {
            return new Vec3d(
                SlurMath.Lerp(t0, t1, random.NextDouble()),
                SlurMath.Lerp(t0, t1, random.NextDouble()),
                SlurMath.Lerp(t0, t1, random.NextDouble()));
        }


        /// <summary>
        /// Returns a random vector which has components within the given interval.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Vec3d NextVec3d(this Random random, Interval1d interval)
        {
            return new Vec3d(
                interval.Evaluate(random.NextDouble()),
                interval.Evaluate(random.NextDouble()),
                interval.Evaluate(random.NextDouble()));
        }


        /// <summary>
        /// Returns a random vector which has components within the given interval.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Vec3d NextVec3d(this Random random, Interval3d interval)
        {
            return new Vec3d(
                interval.X.Evaluate(random.NextDouble()),
                interval.Y.Evaluate(random.NextDouble()),
                interval.Z.Evaluate(random.NextDouble()));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T NextItem<T>(this Random random, IReadOnlyList<T> items)
        {
            return items[random.Next(items.Count)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<T> NextItem<T>(this Random random, IReadOnlyList<T> items, int count)
        {
            while (count > 0)
                yield return random.NextItem(items);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T NextItem<T>(this Random random, T[] items)
        {
            return items[random.Next(items.Length)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<T> NextItem<T>(this Random random, T[] items, int count)
        {
            while (count > 0)
                yield return random.NextItem(items);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<int> Next(this Random random, int count)
        {
            while (count > 0)
                yield return random.Next();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<double> NextDouble(this Random random, int count)
        {
            while (count > 0)
                yield return random.NextDouble();
        }
    }
}
