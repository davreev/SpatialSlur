using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class DotNetExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="other"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Color LerpTo(this Color c, Color other, double t)
        {
            int a = (int)(c.A + (other.A - c.A) * t);
            int r = (int)(c.R + (other.R - c.R) * t);
            int g = (int)(c.G + (other.G - c.G) * t);
            int b = (int)(c.B + (other.B - c.B) * t);
            return Color.FromArgb(a, r, g, b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Domain NextDomain(this Random random)
        {
            return new Domain(random.NextDouble(), random.NextDouble());
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
        /// Returns a random 2d vector which has components within the given domain
        /// </summary>
        /// <param name="random"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Vec2d NextVec2d(this Random random, Domain domain)
        {
            return new Vec2d(domain.Evaluate(random.NextDouble()),domain.Evaluate(random.NextDouble()));
        }


        /// <summary>
        /// Returns a random 2d vector which has components within the given domain
        /// </summary>
        /// <param name="random"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Vec2d NextVec2d(this Random random, Domain2d domain)
        {
            return new Vec2d(domain.x.Evaluate(random.NextDouble()), domain.y.Evaluate(random.NextDouble()));
        }


        /// <summary>
        /// Returns a random 2d vector with a -1.0 to 1.0 range in each dimension
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Vec2d NextVec2d2(this Random random)
        {
            return new Vec2d(random.NextDouble() * 2.0 - 1.0, random.NextDouble() * 2.0 - 1.0);
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
        /// Returns a random vector which has components within the given domain.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Vec2d NextVec3d(this Random random, Domain domain)
        {
            return new Vec3d(domain.Evaluate(random.NextDouble()), domain.Evaluate(random.NextDouble()), domain.Evaluate(random.NextDouble()));
        }


        /// <summary>
        /// Returns a random vector which has components within the given domain.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Vec2d NextVec3d(this Random random, Domain3d domain)
        {
            return new Vec3d(domain.x.Evaluate(random.NextDouble()), domain.y.Evaluate(random.NextDouble()), domain.z.Evaluate(random.NextDouble()));
        }


        /// <summary>
        /// Returns a random vector with a -1.0 to 1.0 range in each dimension.
        /// </summary>
        /// <param name="random"></param>
        public static Vec3d NextVec3d2(this Random random)
        {
            return new Vec3d(random.NextDouble() * 2.0 - 1.0, random.NextDouble() * 2.0 - 1.0, random.NextDouble() * 2.0 - 1.0);
        }


        /// <summary>
        /// Removes elements from the list for which the given delegate returns false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="include"></param>
        public static void Compact<T>(this List<T> list, Func<T, bool> include)
        {
            int marker = 0;

            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                if (include(t))
                    list[marker++] = t;
            }

            list.RemoveRange(marker, list.Count - marker); // trim list to include only used elements
        }


        /// <summary>
        /// Removes elements from the list for which correspond with false values in the mask.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="include"></param>
        public static void Compact<T>(this List<T> list, IList<bool> include)
        {
            int marker = 0;

            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                if (include[i])
                    list[marker++] = t;
            }

            list.RemoveRange(marker, list.Count - marker); // trim list to include only used elements
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Fill<T>(this List<T> list)
        {
            while (list.Count < list.Capacity)
                list.Add(default(T));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        public static void Fill<T>(this List<T> list, T value)
        {
            while (list.Count < list.Capacity)
                list.Add(value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T SelectMin<T, U>(this IEnumerable<T> collection, Func<T, U> selector)
            where U : IComparable<U>
        {
            T tMin = collection.ElementAt(0);
            U uMin = selector(tMin);

            foreach (T t in collection.Skip(1))
            {
                U u = selector(t);

                if (u.CompareTo(uMin) < 0)
                {
                    tMin = t;
                    uMin = u;
                }
            }

            return tMin;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T SelectMax<T, U>(this IEnumerable<T> collection, Func<T, U> selector)
            where U : IComparable<U>
        {
            T tMax = collection.ElementAt(0);
            U uMax = selector(tMax);

            foreach (T t in collection.Skip(1))
            {
                U u = selector(t);

                if (u.CompareTo(uMax) > 0)
                {
                    tMax = t;
                    uMax = u;
                }
            }

            return tMax;
        }
    }
}
