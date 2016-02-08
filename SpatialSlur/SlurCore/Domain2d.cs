using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public struct Domain2d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Domain2d Unit
        {
            get { return new Domain2d(Domain.Unit, Domain.Unit); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vec2d Remap(Vec2d point, Domain2d from, Domain2d to)
        {
            point.x = Domain.Remap(point.x, from.x, to.x);
            point.y = Domain.Remap(point.y, from.y, to.y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Domain2d Intersect(Domain2d d0, Domain2d d1)
        {
            d0.x = Domain.Intersect(d0.x, d1.x);
            d0.y = Domain.Intersect(d0.y, d1.y);
            return d0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static Domain2d Union(Domain2d d0, Domain2d d1)
        {
            d0.x = Domain.Union(d0.x, d1.x);
            d0.y = Domain.Union(d0.y, d1.y);
            return d0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator Domain2d(Domain3d d)
        {
            return new Domain2d(d.x, d.y);
        }

        #endregion


        public Domain x, y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Domain2d(Domain x, Domain y)
        {
            this.x = x;
            this.y = y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Domain2d(Vec2d from, Vec2d to)
        {
            x = new Domain(from.x, to.x);
            y = new Domain(from.y, to.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        public Domain2d(double x0, double x1, double y0, double y1)
        {
            x = new Domain(x0, x1);
            y = new Domain(y0, y1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public Domain2d(IEnumerable<Vec2d> points)
            : this()
        {
            foreach (Vec2d pt in points) Include(pt);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsIncreasing
        {
            get { return x.IsIncreasing && y.IsIncreasing; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsValid
        {
            get { return x.IsValid && y.IsValid; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d From
        {
            get { return new Vec2d(x.t0, y.t0); }
            set
            {
                x.t0 = value.x;
                y.t0 = value.y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d To
        {
            get { return new Vec2d(x.t1, y.t1); }
            set
            {
                x.t1 = value.x;
                y.t1 = value.y;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Span
        {
            get { return new Vec2d(x.Span, y.Span); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Mid
        {
            get { return new Vec2d(x.Mid, y.Mid); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Min
        {
            get { return new Vec2d(x.Min, y.Min); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Max
        {
            get { return new Vec2d(x.Max, y.Max); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0} to {1}, {2} to {3})", x.t0, x.t1, y.t0, y.t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Equals(Domain2d other, double epsilon)
        {
            return x.Equals(other.x, epsilon) && y.Equals(other.y, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d Evaluate(Vec2d point)
        {
            point.x = x.Evaluate(point.x);
            point.y = y.Evaluate(point.y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d Normalize(Vec2d point)
        {
            point.x = x.Normalize(point.x);
            point.y = y.Normalize(point.y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d Clamp(Vec2d point)
        {
            point.x = x.Clamp(point.x);
            point.y = y.Clamp(point.y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d Wrap(Vec2d point)
        {
            point.x = x.Wrap(point.x);
            point.y = y.Wrap(point.y);
            return point;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vec2d point)
        {
            return x.Contains(point.x) && y.Contains(point.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsIncl(Vec2d point)
        {
            return x.ContainsIncl(point.x) && y.ContainsIncl(point.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vec2d delta)
        {
            x.Translate(delta.x);
            y.Translate(delta.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Expand(Vec2d delta)
        {
            x.Expand(delta.x);
            y.Expand(delta.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void Include(Vec2d point)
        {
            x.Include(point.x);
            y.Include(point.y);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Reverse()
        {
            x.Reverse();
            y.Reverse();
        }


        /// <summary>
        /// 
        /// </summary>
        public void MakeIncreasing()
        {
            x.MakeIncreasing();
            y.MakeIncreasing();
        }
    }
}
