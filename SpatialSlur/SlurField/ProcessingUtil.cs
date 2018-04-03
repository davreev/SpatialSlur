using System;
using System.Collections.Generic;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProcessingUtil
    {
        /// <summary>
        /// Calculates L1 (Manhattan) geodesic distance via Dijksta's algorithm as detailed in 
        /// http://www.numerical-tours.com/matlab/fastmarching_0_implementing/
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="field"></param>
        public static void GeodesicDistanceL1(GridField2d<double> field, IEnumerable<int> sources, IEnumerable<int> exclude = null)
        {
            // TODO handle additonal wrap modes

            var dists = field.Values;
            int nx = field.CountX;
            int ny = field.CountY;

            (double dx, double dy) = Vec2d.Abs(field.Scale);

            var queue = new Queue<int>();
            dists.Set(double.PositiveInfinity);

            // enqueue sources
            foreach (int i in sources)
            {
                dists[i] = 0.0;
                queue.Enqueue(i);
            }

            // exclude
            if (exclude != null)
            {
                foreach (int i in exclude)
                    dists[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                int i0 = queue.Dequeue();
                var d0 = dists[i0];

                (int x0, int y0) = field.IndicesAt(i0);

                // -x
                if (x0 > 0)
                    TryUpdate(d0 + dx, i0 - 1);

                // +x
                if (x0 < nx - 1)
                    TryUpdate(d0 + dx, i0 + 1);

                // -y
                if (y0 > 0)
                    TryUpdate(d0 + dy, i0 - nx);

                // +y
                if (y0 < ny - 1)
                    TryUpdate(d0 + dy, i0 + nx);

                // add to queue if less than current min
                void TryUpdate(double distance, int index)
                {
                    if (distance < dists[index])
                    {
                        dists[index] = distance;
                        queue.Enqueue(index);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates L1 (Manhattan) geodesic distance via Dijksta's algorithm as detailed in 
        /// http://www.numerical-tours.com/matlab/fastmarching_0_implementing/
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public static void GeodesicDistanceL1(GridField2d<double> field, double[] cost, IEnumerable<int> sources, IEnumerable<int> exclude = null)
        {
            // TODO handle additonal wrap modes

            var dists = field.Values;
            int nx = field.CountX;
            int ny = field.CountY;

            (double dx, double dy) = Vec2d.Abs(field.Scale);

            var queue = new PriorityQueue<double, int>();
            dists.Set(double.PositiveInfinity);

            // enqueue sources
            foreach (int i in sources)
            {
                dists[i] = 0.0;
                queue.Insert(0.0, i);
            }

            // exclude
            if (exclude != null)
            {
                foreach (int i in exclude)
                    dists[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                (var d0, int i0) = queue.RemoveMin();
                if (dists[i0] < d0) continue; // skip if lower value has been assigned

                (int x0, int y0) = field.IndicesAt(i0);

                // -x
                if (x0 > 0)
                    TryUpdate(d0 + dx * cost[i0 - 1], i0 - 1);

                // +x
                if (x0 < nx - 1)
                    TryUpdate(d0 + dx * cost[i0 + 1], i0 + 1);

                // -y
                if (y0 > 0)
                    TryUpdate(d0 + dy * cost[i0 - nx], i0 - nx);

                // +y
                if (y0 < ny - 1)
                    TryUpdate(d0 + dy * cost[i0 + nx], i0 + nx);

                // add to queue if less than current min
                void TryUpdate(double distance, int index)
                {
                    if (distance < dists[index])
                    {
                        dists[index] = distance;
                        queue.Insert(distance, index);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="exclude"></param>
        public static void GeodesicDistanceL2(GridField2d<double> field, double[] cost, IEnumerable<int> sources, IEnumerable<int> exclude = null)
        {
            // TODO handle additonal wrap modes

            var dists = field.Values;
            var nx = field.CountX;
            var ny = field.CountY;

            (var dx, var dy) = Vec2d.Abs(field.Scale);
            var eikonal = new Eikonal2d(dx, dy);

            var queue = new PriorityQueue<double, int>();
            dists.Set(double.PositiveInfinity);

            // enqueue sources
            foreach (int i in sources)
            {
                dists[i] = 0.0;
                queue.Insert(0.0, i);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var i in exclude)
                    dists[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                (double d0, int i0) = queue.RemoveMin();
                if (dists[i0] < d0) continue; // skip if lower value has been assigned

                (int x0, int y0) = field.IndicesAt(i0);

                if (x0 > 0) X(i0 - 1);
                if (x0 < nx - 1) X(i0 + 1);

                if (y0 > 0) Y(i0 - nx);
                if (y0 < ny - 1) Y(i0 + nx);

                // process x neighbor
                void X(int index)
                {
                    var d1 = dists[index];
                    if (d1 < d0) return; // no backtracking

                    double d2;
                    double minY = GetMinY(index); // will return infinity if neither neighbor has been visited
                    
                    if (minY > double.MaxValue || !eikonal.Evaluate(d0, minY, cost[index], out d2))
                        d2 = d0 + dx * cost[index];
                  
                    // add to queue if less than current min
                    if (d2 < d1)
                    {
                        dists[index] = d2;
                        queue.Insert(d2, index);
                    }
                }

                // process y neighbor
                void Y(int index)
                {
                    var d1 = dists[index];
                    if (d1 < d0) return; // no backtracking

                    double d2;
                    double minX = GetMinX(index); // will return infinity if neither neighbor has been visited

                    if (minX > double.MaxValue || !eikonal.Evaluate(minX, d0, cost[index], out d2))
                        d2 = d0 + dy * cost[index];

                    // add to queue if less than current min
                    if (d2 < d1)
                    {
                        dists[index] = d2;
                        queue.Insert(d2, index);
                    }
                }

                // returns the minimum adjacent value in the x
                double GetMinX(int index)
                {
                    if (x0 == 0) return dists[index + 1];
                    else if (x0 == nx - 1) return dists[index - 1];
                    return Math.Min(dists[index - 1], dists[index + 1]);
                }

                // returns the minimum adjacent value in the y
                double GetMinY(int index)
                {
                    if (y0 == 0) return dists[index + nx];
                    else if (y0 == ny - 1) return dists[index - nx];
                    return Math.Min(dists[index - nx], dists[index + nx]);
                }
            }
        }


        /// <summary>
        /// Grid Eikonal equation solver based on
        /// http://www.numerical-tours.com/matlab/fastmarching_0_implementing/
        /// </summary>
        public struct Eikonal2d
        {
            private double _tx, _ty;
            private double _a, _a2Inv;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="dx"></param>
            /// <param name="dy"></param>
            public Eikonal2d(double dx, double dy)
            {
                _tx = 1.0 / (dx * dx);
                _ty = 1.0 / (dy * dy);
                _a = _tx + _ty;
                _a2Inv = 0.5 / _a;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="w"></param>
            /// <returns></returns>
            public bool Evaluate(double x, double y, out double result)
            {
                double b = -2.0 * (x * _tx + y * _ty);
                double c = x * x * _tx + y * y * _ty - 1.0;

                double disc = b * b - 4.0 * _a * c;

                if (disc < 0.0)
                {
                    result = 0.0;
                    return false;
                }
           
                result = (Math.Sqrt(disc) - b) * _a2Inv;
                return true;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="w"></param>
            /// <returns></returns>
            public bool Evaluate(double x, double y, double w, out double result)
            {
                double b = -2.0 * (x * _tx + y * _ty);
                double c = x * x * _tx + y * y * _ty - w * w;

                double disc = b * b - 4.0 * _a * c; // discriminant

                if (disc < 0.0)
                {
                    result = 0.0;
                    return false;
                }

                result = (Math.Sqrt(disc) - b) * _a2Inv;
                return true;

                // TODO compare with alt quadratic solve 
                // impl ref
                // https://www2.units.it/ipl/students_area/imm2/files/Numerical_Recipes.pdf (5.6)
            }
        }
    }
}
