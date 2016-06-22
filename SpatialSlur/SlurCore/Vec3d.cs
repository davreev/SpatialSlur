using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurData;

/*
 * Notes
 * 
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct Vec3d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        public static Vec3d UnitX
        {
            get { return new Vec3d(1.0, 0.0, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d UnitY
        {
            get { return new Vec3d(0.0, 1.0, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d UnitZ
        {
            get { return new Vec3d(0.0, 0.0, 1.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d operator +(Vec3d v0, Vec3d v1)
        {
            v0.x += v1.x;
            v0.y += v1.y;
            v0.z += v1.z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d operator -(Vec3d v0, Vec3d v1)
        {
            v0.x -= v1.x;
            v0.y -= v1.y;
            v0.z -= v1.z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec3d operator -(Vec3d v)
        {
            v.x = -v.x;
            v.y = -v.y;
            v.z = -v.z;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d operator *(Vec3d v, double t)
        {
            v.x *= t;
            v.y *= t;
            v.z *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d operator *(double t, Vec3d v)
        {
            v.x *= t;
            v.y *= t;
            v.z *= t;
            return v;
        }


        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double operator *(Vec3d v0, Vec3d v1)
        {
            return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d operator /(Vec3d v, double t)
        {
            t = 1.0 / t;
            v.x *= t;
            v.y *= t;
            v.z *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d Lerp(Vec3d v0, Vec3d v1, double t)
        {
            v0.x += (v1.x - v0.x) * t;
            v0.y += (v1.y - v0.y) * t;
            v0.z += (v1.z - v0.z) * t;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d Slerp(Vec3d v0, Vec3d v1, double t)
        {
            return Slerp(v0, v1, Vec3d.Angle(v0, v1), t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="theta"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d Slerp(Vec3d v0, Vec3d v1, double theta, double t)
        {
            double st = 1.0 / Math.Sin(theta);
            return v0 * (Math.Sin((1.0 - t) * theta) * st) + v1 * (Math.Sin(t * theta) * st);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec3d v0, Vec3d v1)
        {
            return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Cross(Vec3d v0, Vec3d v1)
        {
            return new Vec3d(
                v0.y * v1.z - v0.z * v1.y,
                v0.z * v1.x - v0.x * v1.z,
                v0.x * v1.y - v0.y * v1.x);
        }


        /// <summary>
        /// Returns the angle between two vectors.
        /// If either vector is zero length, Double.NaN is returned.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Angle(Vec3d v0, Vec3d v1)
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
        public static Vec3d Project(Vec3d v0, Vec3d v1)
        {
            return (v0 * v1) / v1.SquareLength * v1;
        }


        /// <summary>
        /// Returns the perpendicular component of v0 with respect to v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Perp(Vec3d v0, Vec3d v1)
        {
            return v0 - Project(v0, v1);
        }


        /// <summary>
        /// Returns v0 relected about v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Reflect(Vec3d v0, Vec3d v1)
        {
            return Project(v0, v1) * 2.0 - v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec3d(Vec2d v)
        {
            return new Vec3d(v.x, v.y, 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static List<Vec3d> RemoveDuplicates(IList<Vec3d> points, double epsilon)
        {
            int[] indexMap;
            SpatialHash3d<int> hash;
            return RemoveDuplicates(points, epsilon, out indexMap, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public static List<Vec3d> RemoveDuplicates(IList<Vec3d> points, double epsilon, out int[] indexMap)
        {
            SpatialHash3d<int> hash;
            return RemoveDuplicates(points, epsilon, out indexMap, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="hash"></param>
        /// <param name="epsilon"></param>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public static List<Vec3d> RemoveDuplicates(IList<Vec3d> points, double epsilon, out int[] indexMap, out SpatialHash3d<int> hash)
        {
            List<Vec3d> result = new List<Vec3d>();
            indexMap = new int[points.Count];

            hash = new SpatialHash3d<int>(points.Count * 4, epsilon * 2.0); // TODO test optimal factors for size and scale
            List<int> foundIds = new List<int>();
            Vec3d offset = new Vec3d(epsilon, epsilon, epsilon);
         
            // add points to result if no duplicates are found in the hash
            for (int i = 0; i < points.Count; i++)
            {
                Vec3d p = points[i];
                hash.Search(new Domain3d(p - offset, p + offset), foundIds);

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
        public static int[] GetCoincident(IList<Vec3d> points, double epsilon, bool parallel = false)
        {
            SpatialHash3d<int> hash;
            return GetCoincident(points, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetCoincident(IList<Vec3d> points, double epsilon, out SpatialHash3d<int> hash, bool parallel = false)
        {
            int n = points.Count;
            int[] result = new int[n];
            hash = new SpatialHash3d<int>(n * 4, epsilon * 2.0);

            // insert points into spatial hash
            for (int i = 0; i < n; i++)
                hash.Insert(points[i], i);

            // search for collisions (threadsafe)
            if (parallel)
            {
                var temp = hash; // can't use out parameter in lambda statement below
                Parallel.ForEach(Partitioner.Create(0, n), range =>
                    GetCoincident(points, epsilon, temp, result, range.Item1, range.Item2));
            }
            else
                GetCoincident(points, epsilon, hash, result, 0, n);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetCoincident(IList<Vec3d> points, double epsilon, SpatialHash3d<int> hash, IList<int> result, int i0, int i1)
        {
            List<int> foundIds = new List<int>();
            Vec3d offset = new Vec3d(epsilon, epsilon, epsilon);

            for (int i = i0; i < i1; i++)
            {
                Vec3d p = points[i];
                hash.Search(new Domain3d(p - offset, p + offset), foundIds);

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
        /// For each point in A, returns the index of the first encountered coincident point in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetCoincident(IList<Vec3d> pointsA, IList<Vec3d> pointsB, double epsilon, bool parallel = false)
        {
            SpatialHash3d<int> hash;
            return GetCoincident(pointsA, pointsB, epsilon, out hash, parallel);
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
        public static int[] GetCoincident(IList<Vec3d> pointsA, IList<Vec3d> pointsB, double epsilon, out SpatialHash3d<int> hash, bool parallel = false)
        {
            int nA = pointsA.Count;
            int nB = pointsB.Count;
            int[] result = new int[nA];
            hash = new SpatialHash3d<int>(nB * 4, epsilon * 2.0);

            // insert points
            for (int i = 0; i < nB; i++)
                hash.Insert(pointsB[i], i);

            // search for collisions (threadsafe)
            if (parallel)
            {
                var temp = hash; // can't use out parameter in lambda statement below
                Parallel.ForEach(Partitioner.Create(0, nA), range =>
                    GetCoincident(pointsA, pointsB, epsilon, temp, result, range.Item1, range.Item2));
            }
            else
                GetCoincident(pointsA, pointsB, epsilon, hash, result, 0, nA);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetCoincident(IList<Vec3d> pointsA, IList<Vec3d> pointsB, double epsilon, SpatialHash3d<int> hash, IList<int> result, int i0, int i1)
        {
            List<int> foundIds = new List<int>();
            Vec3d offset = new Vec3d(epsilon, epsilon, epsilon);

            for (int i = i0; i < i1; i++)
            {
                Vec3d p = pointsA[i];
                hash.Search(new Domain3d(p - offset, p + offset), foundIds);

                int coinId = -1;
                foreach (int j in foundIds)
                {
                    if (p.Equals(pointsB[j], epsilon))
                    {
                        coinId = j;
                        break;
                    }
                }

                result[i] = coinId;
                foundIds.Clear();
            }
        }

        #endregion

        public double x, y, z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vec3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
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
            get { return x * x + y * y + z * z; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ManhattanLength
        {
            get { return Math.Abs(x) + Math.Abs(y) + Math.Abs(z); }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double Max
        {
            get { return Math.Max(x, Math.Max(y, z)); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double Min
        {
            get { return Math.Min(x, Math.Min(y, z)); }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vec3d Abs
        {
            get{return new Vec3d(Math.Abs(x), Math.Abs(y), Math.Abs(z));}
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(double epsilon)
        {
            return (Math.Abs(x) < epsilon) && (Math.Abs(y) < epsilon) && (Math.Abs(z) < epsilon);
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
            return String.Format("({0},{1},{2})", x, y, z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Set(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        /// <summary>
        /// Converts from euclidean to spherical coordiantes.
        /// (x,y,z) = (radius, azimuth, polar)
        /// </summary>
        /// <returns></returns>
        public Vec3d ToSpherical()
        {
            double r = this.Length;
            return new Vec3d(r, Math.Atan(y / x), Math.Acos(z / r));
        }


        /// <summary>
        /// Converts from spherical to euclidean coordiantes.
        /// (x,y,z) = (radius, azimuth, polar)
        /// </summary>
        /// <returns></returns>
        public Vec3d ToEuclidean()
        {
            double rxy = Math.Sin(z) * x * x;
            return new Vec3d(Math.Cos(y) * rxy, Math.Sin(y) * rxy, Math.Cos(z) * x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(Vec3d other, double epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon) && (Math.Abs(other.y - y) < epsilon) && (Math.Abs(other.z - z) < epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(Vec3d other, Vec3d epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon.x) && (Math.Abs(other.y - y) < epsilon.y) && (Math.Abs(other.z - z) < epsilon.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vec3d other)
        {
            other -= this;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec3d other)
        {
            other -= this;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(Vec3d other)
        {
            other -= this;
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
                z *= d;
                return true;
            }

            return false;
        }
    }
}
