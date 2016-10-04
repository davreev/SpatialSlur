using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurData;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct Vec2d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        public static Vec2d UnitX
        {
            get { return new Vec2d(1.0, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d UnitY
        {
            get { return new Vec2d(0.0, 1.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d operator +(Vec2d v0, Vec2d v1)
        {
            v0.x += v1.x;
            v0.y += v1.y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d operator -(Vec2d v0, Vec2d v1)
        {
            v0.x -= v1.x;
            v0.y -= v1.y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec2d operator -(Vec2d v)
        {
            v.x = -v.x;
            v.y = -v.y;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d operator *(Vec2d v, double t)
        {
            v.x *= t;
            v.y *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d operator *(double t, Vec2d v)
        {
            v.x *= t;
            v.y *= t;
            return v;
        }


        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double operator *(Vec2d v0, Vec2d v1)
        {
            return v0.x * v1.x + v0.y * v1.y;
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double operator ^(Vec2d v0, Vec2d v1)
        {
            return -v0.y * v1.x + v0.x * v1.y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d operator /(Vec2d v, double t)
        {
            t = 1.0 / t;
            v.x *= t;
            v.y *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec2d v0, Vec2d v1)
        {
            return v0.x * v1.x + v0.y * v1.y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double AbsDot(Vec2d v0, Vec2d v1)
        {
            return Math.Abs(v0.x * v1.x) + Math.Abs(v0.y * v1.y);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Cross(Vec2d v0, Vec2d v1)
        {
            return -v0.y * v1.x + v0.x * v1.y;
        }


        /// <summary>
        /// Returns the angle between two vectors.
        /// If either vector is zero length, Double.NaN is returned.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Angle(Vec2d v0, Vec2d v1)
        {
            double d = v0.Length * v1.Length;

            if (d > 0.0)
                return Math.Acos(SlurMath.Clamp(v0 * v1 / d, -1.0, 1.0)); // clamp dot product to remove noise

            return Double.NaN;
        }


        /// <summary>
        /// Returns the projection of v0 onto v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Project(Vec2d v0, Vec2d v1)
        {
            return (v0 * v1) / v1.SquareLength * v1;
        }


        /// <summary>
        /// Returns the perpendicular component of v0 with respect to v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Perp(Vec2d v0, Vec2d v1)
        {
            return v0 - Project(v0, v1);
        }


        /// <summary>
        /// Returns v0 relected about v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Reflect(Vec2d v0, Vec2d v1)
        {
            return Project(v0, v1) * 2.0 - v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d Lerp(Vec2d v0, Vec2d v1, double t)
        {
            v0.x += (v1.x - v0.x) * t;
            v0.y += (v1.y - v0.y) * t;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec2d(Vec3d v)
        {
            return new Vec2d(v.x, v.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static List<Vec2d> RemoveDuplicates(IList<Vec2d> points, double epsilon)
        {
            int[] indexMap;
            SpatialHash2d<int> hash;
            return RemoveDuplicates(points, epsilon, out indexMap, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public static List<Vec2d> RemoveDuplicates(IList<Vec2d> points, double epsilon, out int[] indexMap)
        {
            SpatialHash2d<int> hash;
            return RemoveDuplicates(points, epsilon, out indexMap, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="indexMap"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static List<Vec2d> RemoveDuplicates(IList<Vec2d> points, double epsilon, out int[] indexMap, out SpatialHash2d<int> hash)
        {
            List<Vec2d> result = new List<Vec2d>();
            indexMap = new int[points.Count];

            hash = new SpatialHash2d<int>(points.Count * 4, epsilon * 2.0); // TODO test optimal factors for size and scale
            List<int> foundIds = new List<int>();
            Vec2d offset = new Vec2d(epsilon, epsilon);

            // add points to result if no duplicates are found in the hash
            for (int i = 0; i < points.Count; i++)
            {
                Vec2d p = points[i];
                hash.Search(new Domain2d(p - offset, p + offset), foundIds);
        
                // check found ids
                bool isDup = false;
                foreach(int j in foundIds)
                {
                    if (p.Equals(result[j], epsilon))
                    {
                        indexMap[i] = j;
                        isDup = true;
                        break;
                    }
                }
                foundIds.Clear();

                // if no duplicate, add to result and hash
                if (!isDup)
                {
                    int id = result.Count;
                    indexMap[i] = id;
                    hash.Insert(p, id);
                    result.Add(p);
                }
            }

            return result;
        }




        /// <summary>
        /// For each point, returns the index of the first coincident point within the list. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(IList<Vec2d> points, double epsilon, bool parallel = false)
        {
            SpatialHash2d<int> hash;
            return GetFirstCoincident(points, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(IList<Vec2d> points, double epsilon, out SpatialHash2d<int> hash, bool parallel = false)
        {
            int n = points.Count;
            int[] result = new int[n];
            hash = new SpatialHash2d<int>(n * 4, epsilon * 2.0);

            // insert points into spatial hash
            for (int i = 0; i < n; i++)
                hash.Insert(points[i], i);

            // search for collisions (threadsafe)
            if (parallel)
            {
                var temp = hash; // can't use out parameter in lambda statement below
                Parallel.ForEach(Partitioner.Create(0, n), range =>
                    GetFirstCoincident(points, epsilon, temp, result, range.Item1, range.Item2));
            }
            else
                GetFirstCoincident(points, epsilon, hash, result, 0, n);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetFirstCoincident(IList<Vec2d> points, double epsilon, SpatialHash2d<int> hash, IList<int> result, int i0, int i1)
        {
            List<int> foundIds = new List<int>();
            Vec2d offset = new Vec2d(epsilon, epsilon);

            for (int i = i0; i < i1; i++)
            {
                Vec2d p = points[i];
                hash.Search(new Domain2d(p - offset, p + offset), foundIds);

                int coinId = -1;
                foreach (int j in foundIds)
                {
                    if (j == i) continue; // ignore self coincidence

                    if (p.Equals(points[j], epsilon))
                    {
                        coinId = j;
                        break;
                    }
                }

                result[i] = coinId;
                foundIds.Clear();
            }
        }


        /// <summary>
        /// For each point in A, returns the index of the first coincident point in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(IList<Vec2d> pointsA, IList<Vec2d> pointsB, double epsilon, bool parallel = false)
        {
            SpatialHash2d<int> hash;
            return GetFirstCoincident(pointsA, pointsB, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(IList<Vec2d> pointsA, IList<Vec2d> pointsB, double epsilon, out SpatialHash2d<int> hash, bool parallel = false)
        {
            int nA = pointsA.Count;
            int nB = pointsB.Count;
            int[] result = new int[nA];
            hash = new SpatialHash2d<int>(nB * 4, epsilon * 2.0);

            // insert points
            for (int i = 0; i < nB; i++)
                hash.Insert(pointsB[i], i);

            // search for collisions (threadsafe)
            if (parallel)
            {
                var temp = hash; // can't use out parameter in lambda statement below
                Parallel.ForEach(Partitioner.Create(0, nA), range =>
                    GetFirstCoincident(pointsA, pointsB, epsilon, temp, result, range.Item1, range.Item2));
            }
            else
                GetFirstCoincident(pointsA, pointsB, epsilon, hash, result, 0, nA);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetFirstCoincident(IList<Vec2d> pointsA, IList<Vec2d> pointsB, double epsilon, SpatialHash2d<int> hash, IList<int> result, int i0, int i1)
        {
            List<int> foundIds = new List<int>();
            Vec2d offset = new Vec2d(epsilon, epsilon);

            for (int i = i0; i < i1; i++)
            {
                Vec2d p = pointsA[i];
                hash.Search(new Domain2d(p - offset, p + offset), foundIds);

                int coinId = -1;
                foreach (int id in foundIds)
                {
                    if (p.Equals(pointsB[id], epsilon))
                    {
                        coinId = id;
                        break;
                    }
                }

                result[i] = coinId;
                foundIds.Clear();
            }
        }


        /// <summary>
        /// For each point in A, returns the index of all coincident points in B.
        /// Note that the resulting list of indices for each point in A may contain duplicate entries.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static List<int>[] GetAllCoincident(IList<Vec2d> pointsA, IList<Vec2d> pointsB, double epsilon, bool parallel = false)
        {
            SpatialHash2d<int> hash;
            return GetAllCoincident(pointsA, pointsB, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static List<int>[] GetAllCoincident(IList<Vec2d> pointsA, IList<Vec2d> pointsB, double epsilon, out SpatialHash2d<int> hash, bool parallel = false)
        {
            int nA = pointsA.Count;
            int nB = pointsB.Count;
            List<int>[] result = new List<int>[nA];
            hash = new SpatialHash2d<int>(nB * 4, epsilon * 2.0);

            // insert points
            for (int i = 0; i < nB; i++)
                hash.Insert(pointsB[i], i);

            // search for collisions (threadsafe)
            if (parallel)
            {
                var temp = hash; // can't use out parameter in lambda statement below
                Parallel.ForEach(Partitioner.Create(0, nA), range =>
                    GetAllCoincident(pointsA, pointsB, epsilon, temp, result, range.Item1, range.Item2));
            }
            else
                GetAllCoincident(pointsA, pointsB, epsilon, hash, result, 0, nA);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetAllCoincident(IList<Vec2d> pointsA, IList<Vec2d> pointsB, double epsilon, SpatialHash2d<int> hash, IList<int>[] result, int i0, int i1)
        {
            Vec2d offset = new Vec2d(epsilon, epsilon);

            for (int i = i0; i < i1; i++)
            {
                Vec2d p = pointsA[i];
                List<int> coinIds = new List<int>();

                hash.Search(new Domain2d(p - offset, p + offset), ids =>
                    {
                        foreach (int id in ids)
                        {
                            if (p.Equals(pointsB[id], epsilon))
                                coinIds.Add(id);
                        }
                    });

                result[i] = coinIds;
            }
        }

        #endregion


        public double x, y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vec2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Length
        {
            get { return Math.Sqrt(SquareLength); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double SquareLength
        {
            get { return x * x + y * y; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ManhattanLength
        {
            get { return Math.Abs(x) + Math.Abs(y); }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double Max
        {
            get { return Math.Max(x, y); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double Min
        {
            get { return Math.Min(x, y); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vec2d Abs
        {
            get { return new Vec2d(Math.Abs(x), Math.Abs(y)); }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(double epsilon)
        {
            return (Math.Abs(x) < epsilon) && (Math.Abs(y) < epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit(double epsilon)
        {
            return Math.Abs(SquareLength - 1.0) < epsilon;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0},{1})", x, y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(double x, double y)
        {
            this.x = x;
            this.y = y;
        }


        /// <summary>
        /// Converts from euclidean to polar coordiantes
        /// (x,y) = (radius, theta)
        /// </summary>
        /// <returns></returns>
        public Vec2d ToPolar()
        {
            return new Vec2d(Length, Math.Atan(y / x));
        }


        /// <summary>
        /// Converts from polar to euclidean coordiantes
        /// (x,y) = (radius, theta)
        /// </summary>
        /// <returns></returns>
        public Vec2d ToEuclidean()
        {
            return new Vec2d(Math.Cos(y) * x, Math.Sin(y) * x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(Vec2d other, double epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon) && (Math.Abs(other.y - y) < epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(Vec2d other, Vec2d epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon.x) && (Math.Abs(other.y - y) < epsilon.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vec2d other)
        {
            other.x -= x;
            other.y -= y;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec2d other)
        {
            other.x -= x;
            other.y -= y;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(Vec2d other)
        {
            other.x -= x;
            other.y -= y;
            return other.ManhattanLength;
        }


        /// <summary>
        /// Unitizes the vector in place.
        /// Returns false if the vector is zero length.
        /// </summary>
        public bool Unitize()
        {
            double d = SquareLength;

            if (d > 0.0)
            {
                d = 1.0 / Math.Sqrt(d);
                x *= d;
                y *= d;
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vec2d LerpTo(Vec2d other, double factor)
        {
            return new Vec2d(
                x + (other.x - x) * factor,
                y + (other.y - y) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new double[] { x, y };
        }
    }
}
