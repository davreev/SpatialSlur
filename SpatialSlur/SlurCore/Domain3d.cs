using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurCore
{
    public struct Domain3d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Domain3d Unit
        {
            get { return new Domain3d(Domain.Unit, Domain.Unit, Domain.Unit); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vec3d Remap(Vec3d point, Domain3d from, Domain3d to)
        {
            point.x = Domain.Remap(point.x, from.x, to.x);
            point.y = Domain.Remap(point.y, from.y, to.y);
            point.y = Domain.Remap(point.z, from.z, to.z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Domain3d Intersect(Domain3d d0, Domain3d d1)
        {
            d0.x = Domain.Intersect(d0.x, d1.x);
            d0.y = Domain.Intersect(d0.y, d1.y);
            d0.z = Domain.Intersect(d0.z, d1.z);
            return d0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Domain3d Union(Domain3d d0, Domain3d d1)
        {
            d0.x = Domain.Union(d0.x, d1.x);
            d0.y = Domain.Union(d0.y, d1.y);
            d0.z = Domain.Union(d0.z, d1.z);
            return d0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator Domain3d(Domain2d d)
        {
            return new Domain3d(d.x, d.y, new Domain());
        }

        #endregion


        public Domain x, y, z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Domain3d(Domain x, Domain y, Domain z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Domain3d(Vec3d from, Vec3d to)
        {
            x = new Domain(from.x, to.x);
            y = new Domain(from.y, to.y);
            z = new Domain(from.z, to.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        public Domain3d(double x0, double x1, double y0, double y1, double z0, double z1)
        {
            x = new Domain(x0, x1);
            y = new Domain(y0, y1);
            z = new Domain(z0, z1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public Domain3d(IEnumerable<Vec3d> points)
            : this()
        {
            foreach (Vec3d pt in points) Include(pt);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsIncreasing
        {
            get { return x.IsIncreasing && y.IsIncreasing && z.IsIncreasing; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsValid
        {
            get { return x.IsValid && y.IsValid && z.IsValid; }
        }

        [Obsolete("Use From property instead")]
        /// <summary>
        /// 
        /// </summary>
        public Vec3d P0
        {
            get { return new Vec3d(x.t0, y.t0, z.t0); }
            set
            {
                x.t0 = value.x;
                y.t0 = value.y;
                z.t0 = value.z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d From
        {
            get { return new Vec3d(x.t0, y.t0, z.t0); }
            set
            {
                x.t0 = value.x;
                y.t0 = value.y;
                z.t0 = value.z;
            }
        }


        [Obsolete("Use To property instead")]
        /// <summary>
        /// 
        /// </summary>
        public Vec3d P1
        {
            get { return new Vec3d(x.t1, y.t1, z.t1); }
            set
            {
                x.t1 = value.x;
                y.t1 = value.y;
                z.t1 = value.z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d To
        {
            get { return new Vec3d(x.t1, y.t1, z.t1); }
            set
            {
                x.t1 = value.x;
                y.t1 = value.y;
                z.t1 = value.z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Span
        {
            get { return new Vec3d(x.Span, y.Span, z.Span); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Mid
        {
            get { return new Vec3d(x.Mid, y.Mid, z.Mid); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Min
        {
            get { return new Vec3d(x.Min, y.Min, z.Min); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Max
        {
            get { return new Vec3d(x.Max, y.Max, z.Max); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0} to {1}, {2} to {3}, {4} to {5})", x.t0, x.t1, y.t0, y.t1, z.t0, z.t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(Domain3d other, double epsilon)
        {
            return x.Equals(other.x, epsilon) && y.Equals(other.y, epsilon) && z.Equals(other.z, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        public Vec3d Evaluate(Vec3d point)
        {
            point.x = x.Evaluate(point.x);
            point.y = y.Evaluate(point.y);
            point.z = z.Evaluate(point.z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public Vec3d Normalize(Vec3d point)
        {
            point.x = x.Normalize(point.x);
            point.y = y.Normalize(point.y);
            point.z = z.Normalize(point.z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d Clamp(Vec3d point)
        {
            point.x = x.Clamp(point.x);
            point.y = y.Clamp(point.y);
            point.z = z.Clamp(point.z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d Wrap(Vec3d point)
        {
            point.x = x.Wrap(point.x);
            point.y = y.Wrap(point.y);
            point.z = z.Wrap(point.z);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vec3d point)
        {
            return x.Contains(point.x) && y.Contains(point.y) && z.Contains(point.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsIncl(Vec3d point)
        {
            return x.ContainsIncl(point.x) && y.ContainsIncl(point.y) && z.ContainsIncl(point.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Translate(Vec3d delta)
        {
            x.Translate(delta.x);
            y.Translate(delta.y);
            z.Translate(delta.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Expand(Vec3d delta)
        {
            x.Expand(delta.x);
            y.Expand(delta.y);
            z.Expand(delta.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void Include(Vec3d point)
        {
            x.Include(point.x);
            y.Include(point.y);
            z.Include(point.z);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Reverse()
        {
            x.Reverse();
            y.Reverse();
            z.Reverse();
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeIncreasing()
        {
            x.MakeIncreasing();
            y.MakeIncreasing();
            z.MakeIncreasing();
        }
    }
}
