using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class Isosurface
    {
        // table of vertices belonging to each edge (12 x 2)
        private static readonly int[] _edgeTable = 
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
            4, 5,
            5, 6,
            6, 7,
            7, 4,
            0, 4,
            1, 5,
            2, 6,
            3, 7
        };

        // table of edges belonging to each case (256 x 16)
        private static readonly int[] _caseTable = 
        { 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1,
            3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1,
            3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1,
            3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1,
            9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1,
            1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1,
            9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1,
            2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1,
            8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1,
            9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1,
            4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1,
            3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1,
            1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1,
            4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1,
            4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1,
            9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1,
            1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1,
            5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1,
            2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1,
            9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1,
            0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1,
            2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1,
            10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1,
            4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1,
            5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1,
            5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1,
            9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1,
            0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1,
            1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1,
            10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1,
            8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1,
            2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1,
            7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1,
            9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1,
            2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1,
            11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1,
            9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1,
            5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1,
            11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1,
            11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1,
            1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1,
            9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1,
            5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1,
            2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1,
            0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1,
            5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1,
            6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1,
            0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1,
            3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1,
            6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1,
            5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1,
            1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1,
            10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1,
            6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1,
            1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1,
            8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1,
            7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1,
            3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1,
            5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1,
            0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1,
            9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1,
            8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1,
            5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1,
            0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1,
            6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1,
            10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1,
            10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1,
            8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1,
            1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1,
            3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1,
            0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1,
            10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1,
            0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1,
            3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1,
            6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1,
            9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1,
            8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1,
            3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1,
            6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1,
            0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1,
            10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1,
            10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1,
            1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1,
            2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1,
            7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1,
            7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1,
            2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1,
            1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1,
            11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1,
            8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1,
            0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1,
            7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1,
            10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1,
            2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1,
            6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1,
            7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1,
            2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1,
            1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1,
            10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1,
            10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1,
            0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1,
            7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1,
            6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1,
            8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1,
            9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1,
            6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1,
            1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1,
            4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1,
            10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1,
            8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1,
            0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1,
            1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1,
            8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1,
            10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1,
            4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1,
            10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1,
            5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1,
            11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1,
            9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1,
            6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1,
            7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1,
            3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1,
            7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1,
            9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1,
            3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1,
            6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1,
            9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1,
            1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1,
            4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1,
            7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1,
            6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1,
            3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1,
            0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1,
            6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1,
            1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1,
            0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1,
            11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1,
            6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1,
            5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1,
            9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1,
            1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1,
            1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1,
            10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1,
            0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1,
            5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1,
            10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1,
            11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1,
            0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1,
            9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1,
            7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1,
            2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1,
            8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1,
            9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1,
            9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1,
            1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1,
            9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1,
            9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1,
            5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1,
            0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1,
            10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1,
            2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1,
            0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1,
            0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1,
            9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1,
            5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1,
            3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1,
            5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1,
            8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1,
            0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1,
            9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1,
            0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1,
            1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1,
            3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1,
            4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1,
            9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1,
            11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1,
            11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1,
            2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1,
            9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1,
            3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1,
            1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1,
            4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1,
            4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1,
            0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1,
            3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1,
            3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1,
            0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1,
            9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1,
            1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
        };


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(ScalarField3d field, double thresh)
        {
            Mesh result = Evaluate(field.Values, field.ScaleX, field.ScaleY, field.ScaleZ, field.CountX, field.CountY, field.CountZ, thresh);

            // move to domain origin
            Vec3d v = field.Domain.From;
            result.Translate(v.x, v.y, v.z);
            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="gradient"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(ScalarField3d field, VectorField3d gradient, double thresh)
        {
            Mesh result = Evaluate(field.Values, gradient.Values, field.ScaleX, field.ScaleY, field.ScaleZ, field.CountX, field.CountY, field.CountZ, thresh);

            // move to domain origin
            Vec3d v = field.Domain.From;
            result.Translate(v.x, v.y, v.z);
            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="box"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<double> values, Domain3d domain, int nx, int ny, int nz, double thresh)
        {
            // get voxel dimensions
            double dx = domain.x.Span / (nx - 1);
            double dy = domain.y.Span / (ny - 1);
            double dz = domain.z.Span / (nz - 1);

            // get isosurface
            Mesh result = Evaluate(values, dx, dy, dz, nx, ny, nz, thresh);

            // move to domain origin
            Vec3d v = domain.From;
            result.Translate(v.x, v.y, v.z);
            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="normals"></param>
        /// <param name="domain"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<double> values, IList<Vec3d> normals, Domain3d domain, int nx, int ny, int nz, double thresh)
        {
            // get voxel dimensions
            double dx = domain.x.Span / (nx - 1);
            double dy = domain.y.Span / (ny - 1);
            double dz = domain.z.Span / (nz - 1);

            // get isosurface
            Mesh result = Evaluate(values, normals, dx, dy, dz, nx, ny, nz, thresh);

            // move to domain origin
            Vec3d v = domain.From;
            result.Translate(v.x, v.y, v.z);
            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// Assumes cubic voxels of size 1.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<double> values, int nx, int ny, int nz, double thresh)
        {
            return Evaluate(values, 1.0, 1.0, 1.0, nx, ny, nz, thresh);
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// Assumes cubic voxels of size 1.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="normals"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<double> values, IList<Vec3d> normals, int nx, int ny, int nz, double thresh)
        {
            return Evaluate(values, normals, 1.0, 1.0, 1.0, nx, ny, nz, thresh);
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<double> values, double dx, double dy, double dz, int nx, int ny, int nz, double thresh)
        {
            // ensure the number of values matches the given dimensions
            int nxy = nx * ny;
            int n = nxy * nz;
            if (values.Count != n)
                throw new System.ArgumentException("The specified dimensions must match the number of values.");

            // resulting mesh
            Mesh result = new Mesh();
            Object locker = new Object();

            // triangulate voxels in chunks
            Parallel.ForEach(Partitioner.Create(0, n - nxy), range =>
            {
                Vec3d[] voxelCorners = new Vec3d[8];
                double[] voxelValues = new double[8];
                Mesh chunk = new Mesh();

                // expand index
                int k = range.Item1 / nxy;
                int i = range.Item1 - k * nxy; // store remainder in i temporarily
                int j = i / nx;
                i -= j * nx;

                // flatten loop for better parallelization
                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    // increment 3d index
                    if (i == nx) { i = 0; j++; }
                    if (j == ny) { j = 0; k++; }
                    if (i == nx - 1 || j == ny - 1) continue; // skip last in each dimension

                    // set voxel values
                    voxelValues[0] = values[index];
                    voxelValues[1] = values[index + 1];
                    voxelValues[2] = values[index + 1 + nx];
                    voxelValues[3] = values[index + nx];
                    voxelValues[4] = values[index + nxy];
                    voxelValues[5] = values[index + 1 + nxy];
                    voxelValues[6] = values[index + 1 + nx + nxy];
                    voxelValues[7] = values[index + nx + nxy];

                    //get case index
                    int caseIndex = 0;
                    if (voxelValues[0] < thresh) caseIndex |= 1;
                    if (voxelValues[1] < thresh) caseIndex |= 2;
                    if (voxelValues[2] < thresh) caseIndex |= 4;
                    if (voxelValues[3] < thresh) caseIndex |= 8;
                    if (voxelValues[4] < thresh) caseIndex |= 16;
                    if (voxelValues[5] < thresh) caseIndex |= 32;
                    if (voxelValues[6] < thresh) caseIndex |= 64;
                    if (voxelValues[7] < thresh) caseIndex |= 128;

                    // if current voxel isn't on thresholdold move to the next
                    if (caseIndex == 0 || caseIndex == 255) continue;

                    // set voxel corners
                    double x0 = i * dx;
                    double y0 = j * dy;
                    double z0 = k * dz;
                    double x1 = x0 + dx;
                    double y1 = y0 + dy;
                    double z1 = z0 + dz;

                    voxelCorners[0].Set(x0, y0, z0);
                    voxelCorners[1].Set(x1, y0, z0);
                    voxelCorners[2].Set(x1, y1, z0);
                    voxelCorners[3].Set(x0, y1, z0);
                    voxelCorners[4].Set(x0, y0, z1);
                    voxelCorners[5].Set(x1, y0, z1);
                    voxelCorners[6].Set(x1, y1, z1);
                    voxelCorners[7].Set(x0, y1, z1);

                    // triangulate the current voxel
                    AddVoxelVertices(voxelCorners, voxelValues, caseIndex, thresh, chunk);
                }

                // append chunk
                if (chunk.Vertices.Count > 0)
                {
                    // add mesh faces
                    var faces = chunk.Faces;
                    for (i = 0; i < chunk.Vertices.Count; i += 3)
                        faces.AddFace(i, i + 1, i + 2);

                    lock (locker) 
                        result.Append(chunk); 
                }
            });

            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="normals"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<double> values, IList<Vec3d> normals, double dx, double dy, double dz, int nx, int ny, int nz, double thresh)
        {
            if (values.Count != normals.Count)
                throw new System.ArgumentException("Must provide an equal number of values and normals.");

            // ensure the number of values matches the given dimensions
            int nxy = nx * ny;
            int n = nxy * nz;
            if (values.Count != n)
                throw new System.ArgumentException("The specified dimensions must match the number of values.");

            // resulting mesh
            Mesh result = new Mesh();
            Object locker = new Object();

            // triangulate voxels in chunks
            Parallel.ForEach(Partitioner.Create(0, n - nxy), range =>
            {
                Vec3d[] voxelCorners = new Vec3d[8];
                Vec3d[] voxelNormals = new Vec3d[8];
                double[] voxelValues = new double[8];
                Mesh chunk = new Mesh();

                // expand index
                int k = range.Item1 / nxy;
                int i = range.Item1 - k * nxy; // store remainder in i temporarily
                int j = i / nx;
                i -= j * nx;

                // flatten loop for better parallelization
                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    // increment 3d index
                    if (i == nx) { i = 0; j++; }
                    if (j == ny) { j = 0; k++; }
                    if (i == nx - 1 || j == ny - 1) continue; // skip last in each dimension

                    // set voxel values
                    voxelValues[0] = values[index];
                    voxelValues[1] = values[index + 1];
                    voxelValues[2] = values[index + 1 + nx];
                    voxelValues[3] = values[index + nx];
                    voxelValues[4] = values[index + nxy];
                    voxelValues[5] = values[index + 1 + nxy];
                    voxelValues[6] = values[index + 1 + nx + nxy];
                    voxelValues[7] = values[index + nx + nxy];

                    //get case index
                    int caseIndex = 0;
                    if (voxelValues[0] < thresh) caseIndex |= 1;
                    if (voxelValues[1] < thresh) caseIndex |= 2;
                    if (voxelValues[2] < thresh) caseIndex |= 4;
                    if (voxelValues[3] < thresh) caseIndex |= 8;
                    if (voxelValues[4] < thresh) caseIndex |= 16;
                    if (voxelValues[5] < thresh) caseIndex |= 32;
                    if (voxelValues[6] < thresh) caseIndex |= 64;
                    if (voxelValues[7] < thresh) caseIndex |= 128;

                    // if current voxel isn't on thresholdold move to the next
                    if (caseIndex == 0 || caseIndex == 255) continue;

                    // set voxel normals
                    voxelNormals[0] = normals[index];
                    voxelNormals[1] = normals[index + 1];
                    voxelNormals[2] = normals[index + 1 + nx];
                    voxelNormals[3] = normals[index + nx];
                    voxelNormals[4] = normals[index + nxy];
                    voxelNormals[5] = normals[index + 1 + nxy];
                    voxelNormals[6] = normals[index + 1 + nx + nxy];
                    voxelNormals[7] = normals[index + nx + nxy];

                    // set voxel corners
                    double x0 = i * dx;
                    double y0 = j * dy;
                    double z0 = k * dz;
                    double x1 = x0 + dx;
                    double y1 = y0 + dy;
                    double z1 = z0 + dz;

                    voxelCorners[0].Set(x0, y0, z0);
                    voxelCorners[1].Set(x1, y0, z0);
                    voxelCorners[2].Set(x1, y1, z0);
                    voxelCorners[3].Set(x0, y1, z0);
                    voxelCorners[4].Set(x0, y0, z1);
                    voxelCorners[5].Set(x1, y0, z1);
                    voxelCorners[6].Set(x1, y1, z1);
                    voxelCorners[7].Set(x0, y1, z1);

                    // triangulate the current voxel
                    AddVoxelVertices(voxelCorners, voxelValues, voxelNormals, caseIndex, thresh, chunk);
                }

                // append chunk
                if (chunk.Vertices.Count > 0)
                {
                    // unitize normals
                    chunk.Normals.UnitizeNormals();

                    // add mesh faces
                    var faces = chunk.Faces;
                    for (i = 0; i < chunk.Vertices.Count; i += 3)
                        faces.AddFace(i, i + 1, i + 2);

                    lock (locker)
                        result.Append(chunk);
                }
            });

            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="domain"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<IList<double>> values, Domain3d domain, int nx, int ny, double thresh)
        {
            // get voxel dimensions
            double dx = domain.x.Span / (nx - 1);
            double dy = domain.y.Span / (ny - 1);
            double dz = domain.z.Span / (values.Count - 1);

            // get isosurface
            Mesh result = Evaluate(values, dx, dy, dz, nx, ny, thresh);

            // move to domain origin
            Vec3d v = domain.From;
            result.Translate(v.x, v.y, v.z);
            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="normals"></param>
        /// <param name="domain"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<IList<double>> values, IList<IList<Vec3d>> normals, Domain3d domain, int nx, int ny, double thresh)
        {
            // get voxel dimensions
            double dx = domain.x.Span / (nx - 1);
            double dy = domain.y.Span / (ny - 1);
            double dz = domain.z.Span / (values.Count - 1);

            // get isosurface
            Mesh result = Evaluate(values, normals, dx, dy, dz, nx, ny, thresh);

            // move to domain origin
            Vec3d v = domain.From;
            result.Translate(v.x, v.y, v.z);
            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<IList<double>> values, int nx, int ny, double thresh)
        {
            return Evaluate(values, 1.0, 1.0, 1.0, nx, ny, thresh);
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="normals"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<IList<double>> values, IList<IList<Vec3d>> normals, int nx, int ny, double thresh)
        {
            return Evaluate(values, normals, 1.0, 1.0, 1.0, nx, ny, thresh);
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<IList<double>> values, double dx, double dy, double dz, int nx, int ny, double thresh)
        {
            // expected number of values per layer
            int nxy = nx * ny;
     
            // resulting mesh
            Mesh result = new Mesh();
            Object locker = new Object();

            // triangulate voxels in layers
            Parallel.ForEach(Partitioner.Create(0, values.Count-1), range =>
            {
                Vec3d[] voxelCorners = new Vec3d[8];
                double[] voxelValues = new double[8];
                Mesh layer = new Mesh();

                for (int k = range.Item1; k < range.Item2; k++)
                {
                    IList<double> vals0 = values[k];
                    IList<double> vals1 = values[k + 1];

                    // check layer size
                    if(vals0.Count != nxy)
                        throw new System.ArgumentException("The specified dimensions must match the number of values in each layer.");
                    
                    for (int j = 0; j < ny - 1; j++)
                    {
                        for (int i = 0; i < nx - 1; i++)
                        {
                            int index = i + j * nx;

                            // set voxel values
                            voxelValues[0] = vals0[index];
                            voxelValues[1] = vals0[index + 1];
                            voxelValues[2] = vals0[index + 1 + nx];
                            voxelValues[3] = vals0[index + nx];
                            voxelValues[4] = vals1[index];
                            voxelValues[5] = vals1[index + 1];
                            voxelValues[6] = vals1[index + 1 + nx];
                            voxelValues[7] = vals1[index + nx];

                            //get case index
                            int caseIndex = 0;
                            if (voxelValues[0] < thresh) caseIndex |= 1;
                            if (voxelValues[1] < thresh) caseIndex |= 2;
                            if (voxelValues[2] < thresh) caseIndex |= 4;
                            if (voxelValues[3] < thresh) caseIndex |= 8;
                            if (voxelValues[4] < thresh) caseIndex |= 16;
                            if (voxelValues[5] < thresh) caseIndex |= 32;
                            if (voxelValues[6] < thresh) caseIndex |= 64;
                            if (voxelValues[7] < thresh) caseIndex |= 128;

                            // if current voxel isn't on thresholdold move to the next
                            if (caseIndex == 0 || caseIndex == 255) continue;

                            // set voxel corners
                            double x0 = i * dx;
                            double y0 = j * dy;
                            double z0 = k * dz;
                            double x1 = x0 + dx;
                            double y1 = y0 + dy;
                            double z1 = z0 + dz;

                            voxelCorners[0].Set(x0, y0, z0);
                            voxelCorners[1].Set(x1, y0, z0);
                            voxelCorners[2].Set(x1, y1, z0);
                            voxelCorners[3].Set(x0, y1, z0);
                            voxelCorners[4].Set(x0, y0, z1);
                            voxelCorners[5].Set(x1, y0, z1);
                            voxelCorners[6].Set(x1, y1, z1);
                            voxelCorners[7].Set(x0, y1, z1);

                            // triangulate the current voxel
                            AddVoxelVertices(voxelCorners, voxelValues, caseIndex, thresh, layer);
                        }
                    }
                }
          
                // append layer
                if (layer.Vertices.Count > 0)
                {
                    // add mesh faces
                    var faces = layer.Faces;
                    for (int i = 0; i < layer.Vertices.Count; i += 3)
                        faces.AddFace(i, i + 1, i + 2);

                    lock (locker) 
                        result.Append(layer);
                }
            });

            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="normals"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<IList<double>> values, IList<IList<Vec3d>> normals, double dx, double dy, double dz, int nx, int ny, double thresh)
        {
            // expected number of values per layer
            int nxy = nx * ny;

            // resulting mesh
            Mesh result = new Mesh();
            Object locker = new Object();

            // triangulate voxels in layers
            Parallel.ForEach(Partitioner.Create(0, values.Count - 1), range =>
            {
                Vec3d[] voxelCorners = new Vec3d[8];
                Vec3d[] voxelNormals = new Vec3d[8];
                double[] voxelValues = new double[8];
                Mesh layer = new Mesh();

                for (int k = range.Item1; k < range.Item2; k++)
                {
                    IList<double> vals0 = values[k];
                    IList<Vec3d> norms0 = normals[k];

                    IList<double> vals1 = values[k + 1];
                    IList<Vec3d> norms1 = normals[k + 1];

                    // check layer size
                    if (vals0.Count != nxy || norms0.Count != nxy)
                        throw new System.ArgumentException("The specified dimensions must match the number of values in each layer.");

                    for (int j = 0; j < ny - 1; j++)
                    {
                        for (int i = 0; i < nx - 1; i++)
                        {
                            int index = i + j * nx;

                            // set voxel values
                            voxelValues[0] = vals0[index];
                            voxelValues[1] = vals0[index + 1];
                            voxelValues[2] = vals0[index + 1 + nx];
                            voxelValues[3] = vals0[index + nx];
                            voxelValues[4] = vals1[index];
                            voxelValues[5] = vals1[index + 1];
                            voxelValues[6] = vals1[index + 1 + nx];
                            voxelValues[7] = vals1[index + nx];

                            //get case index
                            int caseIndex = 0;
                            if (voxelValues[0] < thresh) caseIndex |= 1;
                            if (voxelValues[1] < thresh) caseIndex |= 2;
                            if (voxelValues[2] < thresh) caseIndex |= 4;
                            if (voxelValues[3] < thresh) caseIndex |= 8;
                            if (voxelValues[4] < thresh) caseIndex |= 16;
                            if (voxelValues[5] < thresh) caseIndex |= 32;
                            if (voxelValues[6] < thresh) caseIndex |= 64;
                            if (voxelValues[7] < thresh) caseIndex |= 128;

                            // if current voxel isn't on thresholdold move to the next
                            if (caseIndex == 0 || caseIndex == 255) continue;

                            // set voxel normals
                            voxelNormals[0] = norms0[index];
                            voxelNormals[1] = norms0[index + 1];
                            voxelNormals[2] = norms0[index + 1 + nx];
                            voxelNormals[3] = norms0[index + nx];
                            voxelNormals[4] = norms1[index];
                            voxelNormals[5] = norms1[index + 1];
                            voxelNormals[6] = norms1[index + 1 + nx];
                            voxelNormals[7] = norms1[index + nx];

                            // set voxel corners
                            double x0 = i * dx;
                            double y0 = j * dy;
                            double z0 = k * dz;
                            double x1 = x0 + dx;
                            double y1 = y0 + dy;
                            double z1 = z0 + dz;

                            voxelCorners[0].Set(x0, y0, z0);
                            voxelCorners[1].Set(x1, y0, z0);
                            voxelCorners[2].Set(x1, y1, z0);
                            voxelCorners[3].Set(x0, y1, z0);
                            voxelCorners[4].Set(x0, y0, z1);
                            voxelCorners[5].Set(x1, y0, z1);
                            voxelCorners[6].Set(x1, y1, z1);
                            voxelCorners[7].Set(x0, y1, z1);

                            // triangulate the current voxel
                            AddVoxelVertices(voxelCorners, voxelValues, voxelNormals, caseIndex, thresh, layer);
                        }
                    }
                }

                // append layer
                if (layer.Vertices.Count > 0)
                {
                    layer.Normals.UnitizeNormals();

                    // add mesh faces
                    var faces = layer.Faces;
                    for (int i = 0; i < layer.Vertices.Count; i += 3)
                        faces.AddFace(i, i + 1, i + 2);

                    lock (locker)
                        result.Append(layer);
                }
            });

            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="values"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<Vec3d> points, IList<double> values, int nx, int ny, int nz, double thresh)
        {
            // must have an equal number of values and points
            if (values.Count != points.Count)
                throw new System.ArgumentException("Must provide an equal number of values and points.");

            // ensure the number of values matches the given dimensions
            int nxy = nx * ny;
            int n = nxy * nz;
            if (values.Count != n)
                throw new System.ArgumentException("The specified dimensions must match the number of values.");

            // resulting mesh
            Mesh result = new Mesh();
            Object locker = new Object();

            // triangulate voxels in chunks
            Parallel.ForEach(Partitioner.Create(0, n - nxy), range =>
            {
                Vec3d[] voxelCorners = new Vec3d[8];
                double[] voxelValues = new double[8];
                Mesh chunk = new Mesh();

                // expand index
                int k = range.Item1 / nxy;
                int i = range.Item1 - k * nxy; // store remainder in i temporarily
                int j = i / nx;
                i -= j * nx;

                // flatten loop for better parallelization
                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    // increment 3d index
                    if (i == nx) { i = 0; j++; }
                    if (j == ny) { j = 0; k++; }
                    if (i == nx - 1 || j == ny - 1) continue; // skip last in each dimension
             
                    // set voxel values
                    voxelValues[0] = values[index];
                    voxelValues[1] = values[index + 1];
                    voxelValues[2] = values[index + 1 + nx];
                    voxelValues[3] = values[index + nx];
                    voxelValues[4] = values[index + nxy];
                    voxelValues[5] = values[index + 1 + nxy];
                    voxelValues[6] = values[index + 1 + nx + nxy];
                    voxelValues[7] = values[index + nx + nxy];

                    //get case index
                    int caseIndex = 0;
                    if (voxelValues[0] < thresh) caseIndex |= 1;
                    if (voxelValues[1] < thresh) caseIndex |= 2;
                    if (voxelValues[2] < thresh) caseIndex |= 4;
                    if (voxelValues[3] < thresh) caseIndex |= 8;
                    if (voxelValues[4] < thresh) caseIndex |= 16;
                    if (voxelValues[5] < thresh) caseIndex |= 32;
                    if (voxelValues[6] < thresh) caseIndex |= 64;
                    if (voxelValues[7] < thresh) caseIndex |= 128;

                    // if current voxel isn't on thresholdold move to the next
                    if (caseIndex == 0 || caseIndex == 255) continue;

                    // set voxel corners
                    voxelCorners[0] = points[index];
                    voxelCorners[1] = points[index + 1];
                    voxelCorners[2] = points[index + 1 + nx];
                    voxelCorners[3] = points[index + nx];
                    voxelCorners[4] = points[index + nxy];
                    voxelCorners[5] = points[index + 1 + nxy];
                    voxelCorners[6] = points[index + 1 + nx + nxy];
                    voxelCorners[7] = points[index + nx + nxy];

                    // triangulate the current voxel
                    AddVoxelVertices(voxelCorners, voxelValues, caseIndex, thresh, chunk);
                }

                // append chunk
                if (chunk.Vertices.Count > 0)
                {
                    // add mesh faces
                    var faces = chunk.Faces;
                    for (i = 0; i < chunk.Vertices.Count; i += 3)
                        chunk.Faces.AddFace(i, i + 1, i + 2);

                    lock (locker) 
                        result.Append(chunk);
                }
            });

            return result;
        }


        /// <summary>
        /// Returns an isosurface mesh at the given threshold.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="values"></param>
        /// <param name="normals"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="thresh"></param>
        /// <returns></returns>
        public static Mesh Evaluate(IList<Vec3d> points, IList<double> values, IList<Vec3d> normals, int nx, int ny, int nz, double thresh)
        {
            if (values.Count != normals.Count)
                throw new System.ArgumentException("Must provide an equal number of values and normals.");

            if (values.Count != points.Count)
                throw new System.ArgumentException("Must provide an equal number of values and points.");

            // ensure the number of values matches the given dimensions
            int nxy = nx * ny;
            int n = nxy * nz;
            if (values.Count != n)
                throw new System.ArgumentException("The specified dimensions must match the number of values.");

            // resulting mesh
            Mesh result = new Mesh();
            Object locker = new Object();

            // triangulate voxels in chunks
            Parallel.ForEach(Partitioner.Create(0, n - nxy), range =>
            {
                Vec3d[] voxelCorners = new Vec3d[8];
                Vec3d[] voxelNormals = new Vec3d[8];
                double[] voxelValues = new double[8];
                Mesh chunk = new Mesh();

                // expand index
                int k = range.Item1 / nxy;
                int i = range.Item1 - k * nxy; // store remainder in i temporarily
                int j = i / nx;
                i -= j * nx;

                // flatten loop for better parallelization
                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    // increment 3d index
                    if (i == nx) { i = 0; j++; }
                    if (j == ny) { j = 0; k++; }
                    if (i == nx - 1 || j == ny - 1) continue; // skip last in each dimension

                    // set voxel values
                    voxelValues[0] = values[index];
                    voxelValues[1] = values[index + 1];
                    voxelValues[2] = values[index + 1 + nx];
                    voxelValues[3] = values[index + nx];
                    voxelValues[4] = values[index + nxy];
                    voxelValues[5] = values[index + 1 + nxy];
                    voxelValues[6] = values[index + 1 + nx + nxy];
                    voxelValues[7] = values[index + nx + nxy];

                    //get case index
                    int caseIndex = 0;
                    if (voxelValues[0] < thresh) caseIndex |= 1;
                    if (voxelValues[1] < thresh) caseIndex |= 2;
                    if (voxelValues[2] < thresh) caseIndex |= 4;
                    if (voxelValues[3] < thresh) caseIndex |= 8;
                    if (voxelValues[4] < thresh) caseIndex |= 16;
                    if (voxelValues[5] < thresh) caseIndex |= 32;
                    if (voxelValues[6] < thresh) caseIndex |= 64;
                    if (voxelValues[7] < thresh) caseIndex |= 128;

                    // if current voxel isn't on thresholdold move to the next
                    if (caseIndex == 0 || caseIndex == 255) continue;

                    // set voxel normals
                    voxelNormals[0] = normals[index];
                    voxelNormals[1] = normals[index + 1];
                    voxelNormals[2] = normals[index + 1 + nx];
                    voxelNormals[3] = normals[index + nx];
                    voxelNormals[4] = normals[index + nxy];
                    voxelNormals[5] = normals[index + 1 + nxy];
                    voxelNormals[6] = normals[index + 1 + nx + nxy];
                    voxelNormals[7] = normals[index + nx + nxy];

                    // set voxel corners
                    voxelCorners[0] = points[index];
                    voxelCorners[1] = points[index + 1];
                    voxelCorners[2] = points[index + 1 + nx];
                    voxelCorners[3] = points[index + nx];
                    voxelCorners[4] = points[index + nxy];
                    voxelCorners[5] = points[index + 1 + nxy];
                    voxelCorners[6] = points[index + 1 + nx + nxy];
                    voxelCorners[7] = points[index + nx + nxy];

                    // triangulate the current voxel
                    AddVoxelVertices(voxelCorners, voxelValues, voxelNormals, caseIndex, thresh, chunk);
                }

                // append chunk
                if (chunk.Vertices.Count > 0)
                {
                    result.Normals.UnitizeNormals();

                    // add mesh faces
                    var faces = chunk.Faces;
                    for (i = 0; i < chunk.Vertices.Count; i += 3)
                        faces.AddFace(i, i + 1, i + 2);

                    lock (locker)
                        result.Append(chunk);
                }
            });

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="corners"></param>
        /// <param name="values"></param>
        /// <param name="caseIndex"></param>
        /// <param name="thresh"></param>
        /// <param name="result"></param>
        private static void AddVoxelVertices(Vec3d[] corners, double[] values, int caseIndex, double thresh, Mesh result)
        {
            var verts = result.Vertices;
            int start = caseIndex << 4;

            for (int i = 0; i < 16; i++)
            {
                // get edge index
                int edge = _caseTable[i + start];
                if (edge == -1) break; // break if no more edges

                // get corner indices from edge table
                edge <<= 1;
                int i0 = _edgeTable[edge];
                int i1 = _edgeTable[edge + 1];

                // interpolate edge
                double t = SlurMath.Normalize(thresh, values[i0], values[i1]);
                Vec3d p = Vec3d.Lerp(corners[i0], corners[i1], t);
                verts.Add(p.x, p.y, p.z);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="corners"></param>
        /// <param name="values"></param>
        /// <param name="normals"></param>
        /// <param name="caseIndex"></param>
        /// <param name="thresh"></param>
        /// <param name="result"></param>
        private static void AddVoxelVertices(Vec3d[] corners, double[] values, Vec3d[] normals, int caseIndex, double thresh, Mesh result)
        {
            var verts = result.Vertices;
            var norms = result.Normals;
            int start = caseIndex << 4;
     
            for (int i = 0; i < 16; i++)
            {
                // get edge index
                int edge = _caseTable[i + start];
                if (edge == -1) break; // break if no more edges

                // get corner indices from edge table
                edge <<= 1;
                int i0 = _edgeTable[edge];
                int i1 = _edgeTable[edge + 1];

                // interpolate edge
                double t = SlurMath.Normalize(thresh, values[i0], values[i1]);
                Vec3d p = Vec3d.Lerp(corners[i0], corners[i1], t);
                Vec3d n = Vec3d.Lerp(normals[i0], normals[i1], t);
                verts.Add(p.x, p.y, p.z);
                norms.Add(n.x, n.y, n.z);
            }
        }
    }

}
