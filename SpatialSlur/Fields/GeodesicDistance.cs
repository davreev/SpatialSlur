
/*
 * Notes
 */

using System;
using System.Collections.Generic;

using SpatialSlur.Collections;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class GeodesicDistance
    {
        #region Nested Types

        /// <summary>
        /// Eikonal equation solver for uniform grids in 3 dimensions
        /// </summary>
        private struct Eikonal3d
        {
            // impl ref
            // http://www.numerical-tours.com/matlab/fastmarching_0_implementing/

            private double _tx, _ty, _tz;
            private double _a, _a2Inv;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="scaleX"></param>
            /// <param name="scaleY"></param>
            /// <param name="scaleZ"></param>
            public Eikonal3d(double scaleX, double scaleY, double scaleZ)
            {
                _tx = 1.0 / (scaleX * scaleX);
                _ty = 1.0 / (scaleY * scaleY);
                _tz = 1.0 / (scaleZ * scaleZ);
                _a = _tx + _ty + _tz;
                _a2Inv = 0.5 / _a;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <param name="result"></param>
            /// <returns></returns>
            public bool Solve(double x, double y, double z, out double result)
            {
                double b = -2.0 * (x * _tx + y * _ty + z * _tz);
                double c = x * x * _tx + y * y * _ty + z * z * _tz - 1.0;
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
            /// <param name="z"></param>
            /// <param name="w"></param>
            /// <param name="result"></param>
            /// <returns></returns>
            public bool Solve(double x, double y, double z, double w, out double result)
            {
                double b = -2.0 * (x * _tx + y * _ty + z * _tz);
                double c = x * x * _tx + y * y * _ty + z * z * _tz - w * w;
                double disc = b * b - 4.0 * _a * c;

                if (disc < 0.0)
                {
                    result = 0.0;
                    return false;
                }

                result = (Math.Sqrt(disc) - b) * _a2Inv;
                return true;
            }
        }


        /// <summary>
        /// Eikonal equation solver for uniform grids in 2 dimensions
        /// </summary>
        private struct Eikonal2d
        {
            // impl ref
            // http://www.numerical-tours.com/matlab/fastmarching_0_implementing/

            private double _tx, _ty;
            private double _a, _a2Inv;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="scaleX"></param>
            /// <param name="scaleY"></param>
            public Eikonal2d(double scaleX, double scaleY)
            {
                _tx = 1.0 / (scaleX * scaleX);
                _ty = 1.0 / (scaleY * scaleY);
                _a = _tx + _ty;
                _a2Inv = 0.5 / _a;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="result"></param>
            /// <returns></returns>
            public bool Solve(double x, double y, out double result)
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
            /// <param name="result"></param>
            /// <returns></returns>
            public bool Solve(double x, double y, double w, out double result)
            {
                double b = -2.0 * (x * _tx + y * _ty);
                double c = x * x * _tx + y * y * _ty - w * w;
                double disc = b * b - 4.0 * _a * c;

                if (disc < 0.0)
                {
                    result = 0.0;
                    return false;
                }

                result = (Math.Sqrt(disc) - b) * _a2Inv;
                return true;
            }
        }

        #endregion


        /// <summary>
        /// Calculates the L1 (Manhattan) geodesic distance from the given sources.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void CalculateL1(Grid2d grid, IEnumerable<int> sources, double[] result, IEnumerable<int> exclude = null)
        {
            // impl ref
            // http://www.numerical-tours.com/matlab/fastmarching_0_implementing/

            (var nx, var ny) = grid.Count;
            (var dx, var dy) = Vector2d.Abs(grid.Scale);

            var queue = new Queue<int>();
            result.SetRange(double.PositiveInfinity, grid.CountXY);

            // enqueue sources
            foreach (int i in sources)
            {
                result[i] = 0.0;
                queue.Enqueue(i);
            }

            // exclude
            if (exclude != null)
            {
                foreach (int i in exclude)
                    result[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                int i0 = queue.Dequeue();
                var d0 = result[i0];

                (int x0, int y0) = grid.ToGridSpace(i0);

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
                    if (distance < result[index])
                    {
                        result[index] = distance;
                        queue.Enqueue(index);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the L1 (Manhattan) geodesic distance from the given sources.
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void CalculateL1(GridField2d<double> cost, IEnumerable<int> sources, double[] result, IEnumerable<int> exclude = null)
        {
            // impl ref
            // http://www.numerical-tours.com/matlab/fastmarching_0_implementing/

            var costVals = cost.Values;
            (var nx, var ny) = cost.Count;
            (double dx, double dy) = Vector2d.Abs(cost.Scale);

            var queue = new PriorityQueue<double, int>();
            result.SetRange(double.PositiveInfinity, cost.CountXY);

            // enqueue sources
            foreach (int i in sources)
            {
                result[i] = 0.0;
                queue.Insert(0.0, i);
            }

            // exclude
            if (exclude != null)
            {
                foreach (int i in exclude)
                    result[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                (var d0, int i0) = queue.RemoveMin();
                if (result[i0] < d0) continue; // skip if lower value has been assigned

                (int x0, int y0) = cost.ToGridSpace(i0);

                // -x
                if (x0 > 0)
                    TryUpdate(d0 + dx * costVals[i0 - 1], i0 - 1);

                // +x
                if (x0 < nx - 1)
                    TryUpdate(d0 + dx * costVals[i0 + 1], i0 + 1);

                // -y
                if (y0 > 0)
                    TryUpdate(d0 + dy * costVals[i0 - nx], i0 - nx);

                // +y
                if (y0 < ny - 1)
                    TryUpdate(d0 + dy * costVals[i0 + nx], i0 + nx);

                // add to queue if less than current min
                void TryUpdate(double distance, int index)
                {
                    if (distance < result[index])
                    {
                        result[index] = distance;
                        queue.Insert(distance, index);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the L2 (Euclidiean) geodesic distance from the given sources.
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void CalculateL2(GridField2d<double> cost, IEnumerable<int> sources, double[] result, IEnumerable<int> exclude = null)
        {
            // impl ref
            // http://www.numerical-tours.com/matlab/fastmarching_0_implementing/

            var costVals = cost.Values;
            (var nx, var ny) = cost.Count;
            (var dx, var dy) = Vector2d.Abs(cost.Scale);
            var eikonal = new Eikonal2d(dx, dy);

            var queue = new PriorityQueue<double, int>();
            result.SetRange(double.PositiveInfinity, cost.CountXY);

            // enqueue sources
            foreach (int i in sources)
            {
                result[i] = 0.0;
                queue.Insert(0.0, i);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var i in exclude)
                    result[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                (double d0, int i0) = queue.RemoveMin();
                if (result[i0] < d0) continue; // skip if lower value has been assigned

                (int x0, int y0) = cost.ToGridSpace(i0);

                if (x0 > 0)
                    TryUpdateX(i0 - 1);

                if (x0 < nx - 1)
                    TryUpdateX(i0 + 1);

                if (y0 > 0)
                    TryUpdateY(i0 - nx);

                if (y0 < ny - 1)
                    TryUpdateY(i0 + nx);

                // process x neighbor
                void TryUpdateX(int index)
                {
                    var d1 = result[index];
                    if (d1 < d0) return; // no backtracking

                    double d2;
                    double minY = GetMinY(index); // will return infinity if neither neighbor has been visited

                    if (minY > double.MaxValue || !eikonal.Solve(d0, minY, costVals[index], out d2))
                        d2 = d0 + dx * costVals[index];

                    // add to queue if less than current min
                    if (d2 < d1)
                    {
                        result[index] = d2;
                        queue.Insert(d2, index);
                    }
                }

                // process y neighbor
                void TryUpdateY(int index)
                {
                    var d1 = result[index];
                    if (d1 < d0) return; // no backtracking

                    double d2;
                    double minX = GetMinX(index); // will return infinity if neither neighbor has been visited

                    if (minX > double.MaxValue || !eikonal.Solve(minX, d0, costVals[index], out d2))
                        d2 = d0 + dy * costVals[index];

                    // add to queue if less than current min
                    if (d2 < d1)
                    {
                        result[index] = d2;
                        queue.Insert(d2, index);
                    }
                }

                // returns the minimum adjacent value in the x
                double GetMinX(int index)
                {
                    if (x0 == 0)
                        return result[index + 1];
                    else if (x0 == nx - 1)
                        return result[index - 1];

                    return Math.Min(result[index - 1], result[index + 1]);
                }

                // returns the minimum adjacent value in the y
                double GetMinY(int index)
                {
                    if (y0 == 0)
                        return result[index + nx];
                    else if (y0 == ny - 1)
                        return result[index - nx];

                    return Math.Min(result[index - nx], result[index + nx]);
                }
            }
        }


        /// <summary>
        /// Calculates the L1 (Manhattan) geodesic distance from the given sources.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void CalculateL1(Grid3d grid, IEnumerable<int> sources, double[] result, IEnumerable<int> exclude = null)
        {
            // impl ref
            // http://www.numerical-tours.com/matlab/fastmarching_0_implementing/

            (var nx, var ny, var nz) = grid.Count;
            var nxy = grid.CountXY;

            (var dx, var dy, var dz) = Vector3d.Abs(grid.Scale);

            var queue = new Queue<int>();
            result.SetRange(double.PositiveInfinity, grid.CountXYZ);

            // enqueue sources
            foreach (int i in sources)
            {
                result[i] = 0.0;
                queue.Enqueue(i);
            }

            // exclude
            if (exclude != null)
            {
                foreach (int i in exclude)
                    result[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                int i0 = queue.Dequeue();
                var d0 = result[i0];

                (int x0, int y0, int z0) = grid.ToGridSpace(i0);

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

                // -z
                if (z0 > 0)
                    TryUpdate(d0 + dz, i0 - nxy);

                // +z
                if (z0 < nz - 1)
                    TryUpdate(d0 + dz, i0 + nxy);

                // add to queue if less than current min
                void TryUpdate(double distance, int index)
                {
                    if (distance < result[index])
                    {
                        result[index] = distance;
                        queue.Enqueue(index);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the L1 (Manhattan) geodesic distance from the given sources.
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void CalculateL1(GridField3d<double> cost, IEnumerable<int> sources, double[] result, IEnumerable<int> exclude = null)
        {
            // impl ref
            // http://www.numerical-tours.com/matlab/fastmarching_0_implementing/

            var costVals = cost.Values;

            (var nx, var ny, var nz) = cost.Count;
            var nxy = cost.CountXY;

            (var dx, var dy, var dz) = Vector3d.Abs(cost.Scale);

            var queue = new PriorityQueue<double, int>();
            result.SetRange(double.PositiveInfinity, cost.CountXYZ);

            // enqueue sources
            foreach (int i in sources)
            {
                result[i] = 0.0;
                queue.Insert(0.0, i);
            }

            // exclude
            if (exclude != null)
            {
                foreach (int i in exclude)
                    result[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                (var d0, int i0) = queue.RemoveMin();
                if (result[i0] < d0) continue; // skip if lower value has been assigned

                (int x0, int y0, int z0) = cost.ToGridSpace(i0);

                // -x
                if (x0 > 0)
                    TryUpdate(d0 + dx * costVals[i0 - 1], i0 - 1);

                // +x
                if (x0 < nx - 1)
                    TryUpdate(d0 + dx * costVals[i0 + 1], i0 + 1);

                // -y
                if (y0 > 0)
                    TryUpdate(d0 + dy * costVals[i0 - nx], i0 - nx);

                // +y
                if (y0 < ny - 1)
                    TryUpdate(d0 + dy * costVals[i0 + nx], i0 + nx);

                // -z
                if (z0 > 0)
                    TryUpdate(d0 + dz * costVals[i0 - nxy], i0 - nxy);

                // +z
                if (z0 < nz - 1)
                    TryUpdate(d0 + dz * costVals[i0 + nxy], i0 + nxy);

                // add to queue if less than current min
                void TryUpdate(double distance, int index)
                {
                    if (distance < result[index])
                    {
                        result[index] = distance;
                        queue.Insert(distance, index);
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the L2 (Euclidiean) geodesic distance from the given sources.
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        /// <param name="exclude"></param>
        public static void CalculateL2(GridField3d<double> cost, IEnumerable<int> sources, double[] result, IEnumerable<int> exclude = null)
        {
            // impl ref
            // http://www.numerical-tours.com/matlab/fastmarching_0_implementing/

            var costVals = cost.Values;

            (var nx, var ny, var nz) = cost.Count;
            var nxy = cost.CountXY;

            (var dx, var dy, var dz) = Vector3d.Abs(cost.Scale);
            var eikonal = new Eikonal3d(dx, dy, dz);

            var queue = new PriorityQueue<double, int>();
            result.SetRange(double.PositiveInfinity, cost.CountXYZ);

            // enqueue sources
            foreach (int i in sources)
            {
                result[i] = 0.0;
                queue.Insert(0.0, i);
            }

            // exclude
            if (exclude != null)
            {
                foreach (var i in exclude)
                    result[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                (double d0, int i0) = queue.RemoveMin();
                if (result[i0] < d0) continue; // skip if lower value has been assigned

                (int x0, int y0, int z0) = cost.ToGridSpace(i0);

                if (x0 > 0)
                    TryUpdateX(i0 - 1);

                if (x0 < nx - 1)
                    TryUpdateX(i0 + 1);

                if (y0 > 0)
                    TryUpdateY(i0 - nx);

                if (y0 < ny - 1)
                    TryUpdateY(i0 + nx);

                if (z0 > 0)
                    TryUpdateZ(i0 - nxy);

                if (z0 < nz - 1)
                    TryUpdateZ(i0 + nxy);

                // process x neighbor
                void TryUpdateX(int index)
                {
                    var d1 = result[index];
                    if (d1 < d0) return; // no backtracking

                    double d2;
                    double minY = GetMinY(index); // will return infinity if neither neighbor has been visited
                    double minZ = GetMinZ(index); // ''
                    
                    if (minY > double.MaxValue || minZ > double.MaxValue || !eikonal.Solve(d0, minY, minZ, costVals[index], out d2))
                        d2 = d0 + dx * costVals[index];

                    // add to queue if less than current min
                    if (d2 < d1)
                    {
                        result[index] = d2;
                        queue.Insert(d2, index);
                    }
                }

                // process y neighbor
                void TryUpdateY(int index)
                {
                    var d1 = result[index];
                    if (d1 < d0) return; // no backtracking

                    double d2;
                    double minX = GetMinX(index); // will return infinity if neither neighbor has been visited
                    double minZ = GetMinZ(index); // ''
                    
                    if (minX > double.MaxValue || minZ > double.MaxValue || !eikonal.Solve(minX, d0, minZ, costVals[index], out d2))
                        d2 = d0 + dy * costVals[index];

                    // add to queue if less than current min
                    if (d2 < d1)
                    {
                        result[index] = d2;
                        queue.Insert(d2, index);
                    }
                }

                // process y neighbor
                void TryUpdateZ(int index)
                {
                    var d1 = result[index];
                    if (d1 < d0) return; // no backtracking

                    double d2;
                    double minX = GetMinX(index); // will return infinity if neither neighbor has been visited
                    double minY = GetMinY(index); // ''
                    
                    if (minX > double.MaxValue || minY > double.MaxValue || !eikonal.Solve(minX, minY, d0, costVals[index], out d2))
                        d2 = d0 + dy * costVals[index];

                    // add to queue if less than current min
                    if (d2 < d1)
                    {
                        result[index] = d2;
                        queue.Insert(d2, index);
                    }
                }

                // returns the minimum adjacent value in the x
                double GetMinX(int index)
                {
                    if (x0 == 0)
                        return result[index + 1];
                    else if (x0 == nx - 1)
                        return result[index - 1];

                    return Math.Min(result[index - 1], result[index + 1]);
                }

                // returns the minimum adjacent value in the y
                double GetMinY(int index)
                {
                    if (y0 == 0)
                        return result[index + nx];
                    else if (y0 == ny - 1)
                        return result[index - nx];

                    return Math.Min(result[index - nx], result[index + nx]);
                }

                // returns the minimum adjacent value in the z
                double GetMinZ(int index)
                {
                    if (z0 == 0)
                        return result[index + nxy];
                    else if (z0 == nz - 1)
                        return result[index - nxy];

                    return Math.Min(result[index - nxy], result[index + nxy]);
                }
            }
        }
    }
}
