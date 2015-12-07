using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurData;

namespace SpatialSlur.SlurCore
{
    public struct Vec3d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Zero
        {
            get { return new Vec3d(0.0, 0.0, 0.0); }
        }


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
        /// <param name="v1"></param>
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
        /// returns the dot product of two vectors
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="t"></param>
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
        /// <returns></returns>
        public static Vec3d Cross(Vec3d v0, Vec3d v1)
        {
            double x = v0.y * v1.z - v0.z * v1.y;
            double y = v0.z * v1.x - v0.x * v1.z;
            double z = v0.x * v1.y - v0.y * v1.x;
            return new Vec3d(x, y, z);
        }


        /// <summary>
        /// returns the angle between two vectors
        /// returns NaN if either vector is zero length
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Angle(Vec3d v0, Vec3d v1)
        {
            if (v0.Unitize() && v1.Unitize()) 
                return Math.Acos(v0 * v1);

            return Double.NaN;
        }


        /// <summary>
        /// reflects v0 about v1
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static Vec3d Reflect(Vec3d v0, Vec3d v1)
        {
            v1.Unitize();
            return v1 * (v0 * v1 * 2.0) - v0;
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
        /// the given hash should be scaled appropriately for the point set
        /// </summary>
        /// <param name="points"></param>
        /// <param name="hash"></param>
        /// <param name="epsilon"></param>
        /// <param name="map"></param>
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
        /// <param name="hash"></param>
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
        /// For each point, returns the index of the first encountered coincident point within the list. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static int[] GetCoincident(IList<Vec3d> points, double epsilon)
        {
            SpatialHash3d<int> hash;
            return GetCoincident(points, epsilon, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static int[] GetCoincident(IList<Vec3d> points, double epsilon, out SpatialHash3d<int> hash)
        {
            int n = points.Count;
            int[] result = new int[n];
            hash = new SpatialHash3d<int>(n * 4, epsilon * 2.0);

            // insert points into spatial hash
            for (int i = 0; i < n; i++)
                hash.Insert(points[i], i);

            // search for collisions
            List<int> foundIds = new List<int>();
            Vec3d offset = new Vec3d(epsilon, epsilon, epsilon);

            for (int i = 0; i < n; i++)
            {
                Vec3d p = points[i];
                hash.Search(new Domain3d(p - offset, p + offset), foundIds);

                int coinId = -1;
                foreach(int j in foundIds)
                {
                    if (j == i) continue; // ignore self coincidence

                    if (p.Equals(points[j],epsilon))
                    {
                        coinId = j;
                        break;
                    }
                }
                foundIds.Clear();

                result[i] = coinId;
            }

            return result;
        }


        /// <summary>
        /// For each point in A, returns the index of the first encountered coincident point in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static int[] GetCoincident(IList<Vec3d> pointsA, IList<Vec3d> pointsB, double epsilon)
        {
            SpatialHash3d<int> hash;
            return GetCoincident(pointsA, pointsB, epsilon, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static int[] GetCoincident(IList<Vec3d> pointsA, IList<Vec3d> pointsB, double epsilon, out SpatialHash3d<int> hash)
        {
            int nA = pointsA.Count;
            int nB = pointsB.Count;
            int[] result = new int[nA];
            hash = new SpatialHash3d<int>(nB * 4, epsilon * 2.0);

            // insert points
            for (int i = 0; i < nB; i++)
                hash.Insert(pointsB[i], i);

            // search for collisions
            List<int> foundIds = new List<int>();
            Vec3d offset = new Vec3d(epsilon, epsilon, epsilon);

            for (int i = 0; i < nA; i++)
            {
                Vec3d pA = pointsA[i];
                hash.Search(new Domain3d(pA - offset, pA + offset), foundIds);

                int coinId = -1;
                foreach(int j in foundIds)
                {
                    if (pA.Equals(pointsB[j], epsilon))
                    {
                        coinId = j;
                        break;
                    }
                }
                foundIds.Clear();

                result[i] = coinId;
            }

            return result;
        }


        [Obsolete("Use method in GeometryUtil instead")]
        /// <summary>
        /// returns the entries of the covariance matrix in column-major order
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[] GetCovariance(IList<Vec3d> vectors)
        {
            Vec3d mean;
            return GetCovariance(vectors, out mean);
        }


        [Obsolete("Use method in GeometryUtil instead")]
        /// <summary>
        /// returns the entries of the covariance matrix in column-major order
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[] GetCovariance(IList<Vec3d> vectors, out Vec3d mean)
        {
            // calculate mean
            mean = new Vec3d();
            foreach (Vec3d v in vectors) mean += v;
            mean /= vectors.Count;

            // calculate covariance matrix
            double[] result = new double[9];
   
            for (int i = 0; i < vectors.Count; i++)
            {
                Vec3d d = vectors[i] - mean;
                result[0] += d.x * d.x;
                result[1] += d.x * d.y;
                result[2] += d.x * d.z;
                result[4] += d.y * d.y;
                result[5] += d.y * d.z;
                result[8] += d.z * d.z;
            }

            // set symmetric values
            result[3] = result[1];
            result[6] = result[2];
            result[7] = result[5];
            return result;
        }

        #endregion


        public double x, y, z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
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
        /// 
        /// </summary>
        public bool IsZero
        {
            get { return x == 0.0 && y == 0.0 && z == 0.0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit
        {
            get { return SquareLength == 1.0; }
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
        public void Set(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        /// <summary>
        /// convert from euclidean to spherical coordiantes 
        /// (x,y,z) -> (radius, azimuth, polar)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vec3d ToSpherical()
        {
            double r = this.Length;
            return new Vec3d(r, Math.Atan(y / x), Math.Acos(z / r));
        }


        /// <summary>
        /// convert from spherical to euclidean coordiantes
        /// (radius, azimuth, polar) -> (x,y,z)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
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
        /// returns false if the vector is zero length
        /// </summary>
        public bool Unitize()
        {
            double d = SquareLength;
            if (d == 0.0) return false;

            d = 1.0 / Math.Sqrt(d);
            x *= d;
            y *= d;
            z *= d;
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vec3d Abs()
        {
            return new Vec3d(Math.Abs(x), Math.Abs(y), Math.Abs(z));
        }


        /// <summary>
        /// returns the largest component in the vector
        /// </summary>
        /// <returns></returns>
        public double Max()
        {
            return Math.Max(x, Math.Max(y, z));
        }


        /// <summary>
        /// returns the smallest component in the vector
        /// </summary>
        /// <returns></returns>
        public double Min()
        {
            return Math.Min(x, Math.Min(y, z));
        }

    }
}
