
/*
 * Notes 
 */

using System;
using System.IO;
using System.Text;

using SpatialSlur;
using SpatialSlur.Fields;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField3dFactoryExtensions
    {
        #region Vector3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GridField3d<Vector3d> CreateFromFGA(this GridField3dFactory<Vector3d> factory, string path)
        {
            var content = File.ReadAllText(path, Encoding.ASCII);
            var values = content.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int nx = int.Parse(values[0]);
            int ny = int.Parse(values[1]);
            int nz = int.Parse(values[2]);

            Vector3d p0 = new Vector3d(
                double.Parse(values[3]),
                double.Parse(values[4]),
                double.Parse(values[5]));

            Vector3d p1 = new Vector3d(
               double.Parse(values[6]),
               double.Parse(values[7]),
               double.Parse(values[8]));

            var result = factory.Create(nx, ny, nz, new Interval3d(p0, p1));
            var vecs = result.Values;
            int index = 0;

            for (int i = 9; i < values.Length; i += 3)
            {
                vecs[index++] = new Vector3d(
                    double.Parse(values[i]),
                    double.Parse(values[i + 1]),
                    double.Parse(values[i + 2]));
            }

            return result;
        }

        #endregion
    }
}
