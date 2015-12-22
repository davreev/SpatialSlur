using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public class VectorField3d : Field3d<Vec3d>
    {
        #region Static

        /// <summary>
        /// returns a vector field of normalized rgb values
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static VectorField3d CreateFromImages(Domain3d domain, IList<Bitmap> bitmaps)
        {
            throw new NotImplementedException();

            /*
            VectorField2d result = new VectorField2d(domain, bitmap.Width, bitmap.Height);

            unsafe
            {
                // get and check bytes per pixel
                int bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;

                // ensure valid color depth
                if (bytesPerPixel < 3)
                    throw new ArgumentException("the given image must have a color depth of at least 24 bits per pixel");

                // lock bits and get pointer to first
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                byte* first = (byte*)bitmapData.Scan0;

                int byteWidth = bitmap.Width * bytesPerPixel;
                int fieldIndex = 0;

                for (int i = 0; i < bitmap.Height; i++)
                {
                    byte* currentLine = first + (i * bitmapData.Stride);

                    for (int j = 0; j < byteWidth; j += bytesPerPixel, fieldIndex++)
                    {
                        int r = currentLine[j];
                        int g = currentLine[j + 1];
                        int b = currentLine[j + 2];

                        double t = 1.0 / 255.0;
                        result.Values[fieldIndex] = new Vec3d(r * t, g * t, b * t);
                    }
                }
            }

            return result;
            */
        }

        #endregion


        private Action<IList<Vec3d>> _getLaplacian;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="boundaryType"></param>
        public VectorField3d(Domain3d domain, int countX, int countY, int countZ, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : base(domain, countX, countY, countZ, boundaryType)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VectorField3d(Field3d other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public VectorField3d(VectorField3d other)
            : base(other)
        {
        }


        /// <summary>
        /// Updates any boundary sensitive methods
        /// </summary>
        protected override void OnBoundaryTypeChange()
        {
            base.OnBoundaryTypeChange();

            switch (BoundaryType)
            {
                case FieldBoundaryType.Constant:
                    _getLaplacian = GetLaplacianConstant;
                    break;
                case FieldBoundaryType.Equal:
                    _getLaplacian = GetLaplacianEqual;
                    break;
                case FieldBoundaryType.Periodic:
                    _getLaplacian = GetLaplacianPeriodic;
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Field3d Duplicate()
        {
            return new VectorField3d(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Vec3d Evaluate(FieldPoint3d point)
        {
            int[] corners = point.Corners;
            double[] weights = point.Weights;

            Vec3d result = new Vec3d();
            for (int i = 0; i < 8; i++)
                result += Values[corners[i]] * weights[i];

            return result;
        }


        /// <summary>
        /// gets the magnitudes of all vectors in the field
        /// </summary>
        /// <returns></returns>
        public ScalarField3d GetMagnitudes()
        {
            ScalarField3d result = new ScalarField3d(this);
            GetMagnitudes(result.Values);
            return result;
        }


        /// <summary>
        /// gets the magnitudes of all vectors in the field
        /// </summary>
        /// <param name="result"></param>
        public void GetMagnitudes(ScalarField3d result)
        {
            GetMagnitudes(result.Values);
        }


        /// <summary>
        /// gets the magnitudes of all vectors in the field
        /// </summary>
        /// <param name="result"></param>
        public void GetMagnitudes(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Values[i].Length;
            });
        }


        /// <summary>
        /// unitizes all vectors in the field
        /// </summary>
        public void Unitize()
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] /= Values[i].Length;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Cross(VectorField3d other)
        {
            Function(Vec3d.Cross, other);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Cross(VectorField3d fieldA, VectorField3d fieldB)
        {
            Function(Vec3d.Cross, fieldA, fieldB);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void LerpTo(Vec3d value, double factor)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Lerp(Values[i], value, factor);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void LerpTo(VectorField3d other, double factor)
        {
            SizeCheck(other);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Lerp(Values[i], other.Values[i], factor);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        public void LerpTo(Vec3d value, ScalarField3d factors)
        {
            SizeCheck(factors);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Lerp(Values[i], value, factors.Values[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factors"></param>
        public void LerpTo(VectorField3d other, ScalarField3d factors)
        {
            SizeCheck(other);
            SizeCheck(factors);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    Values[i] = Vec3d.Lerp(Values[i], other.Values[i], factors.Values[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public VectorField3d GetLaplacian()
        {
            VectorField3d result = new VectorField3d((Field3d)this);
            UpdateLaplacian(result.Values);
            return result;
        }


        [Obsolete("Use UpdateLaplacian")]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void GetLaplacian(VectorField3d result)
        {
            GetLaplacian(result.Values);
        }


        [Obsolete("Use UpdateLaplacian")]
        /// <summary>
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="result"></param>
        public void GetLaplacian(IList<Vec3d> result)
        {
            SizeCheck(result);
            _getLaplacian(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(VectorField3d result)
        {
            UpdateLaplacian(result.Values);
        }


        /// <summary>
        /// http://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(IList<Vec3d> result)
        {
            SizeCheck(result);
            _getLaplacian(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rate"></param>
        private void GetLaplacianConstant(IList<Vec3d> result)
        {
            // inverse square step size for each dimension
            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);
            double dz = 1.0 / (ScaleZ * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    Vec3d value = Values[index];
                    Vec3d sum = new Vec3d();

                    // x
                    if (i == 0)
                        sum += (BoundaryValue + Values[index + 1] - 2.0 * value) * dx;
                    else if (i == CountX - 1)
                        sum += (Values[index - 1] + BoundaryValue - 2.0 * value) * dx;
                    else
                        sum += (Values[index - 1] + Values[index + 1] - 2.0 * value) * dx;

                    // y
                    if (j == 0)
                        sum += (BoundaryValue + Values[index + CountX] - 2.0 * value) * dy;
                    else if (j == CountY - 1)
                        sum += (Values[index - CountX] + BoundaryValue - 2.0 * value) * dy;
                    else
                        sum += (Values[index - CountX] + Values[index + CountX] - 2.0 * value) * dy;

                    // z
                    if (k == 0)
                        sum += (BoundaryValue + Values[index + CountXY] - 2.0 * value) * dz;
                    else if (k == CountZ - 1)
                        sum += (Values[index - CountXY] + BoundaryValue - 2.0 * value) * dz;
                    else
                        sum += (Values[index - CountXY] + Values[index + CountXY] - 2.0 * value) * dz;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rate"></param>
        private void GetLaplacianEqual(IList<Vec3d> result)
        {
            // inverse square step size for each dimension
            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);
            double dz = 1.0 / (ScaleZ * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    Vec3d value = Values[index];
                    Vec3d sum = new Vec3d();

                    // x
                    if (i == 0)
                        sum += (Values[index + 1] - value) * dx;
                    else if (i == CountX - 1)
                        sum += (Values[index - 1] - value) * dx;
                    else
                        sum += (Values[index - 1] + Values[index + 1] - 2.0 * value) * dx;

                    // y
                    if (j == 0)
                        sum += (Values[index + CountX] - value) * dy;
                    else if (j == CountY - 1)
                        sum += (Values[index - CountX] - value) * dy;
                    else
                        sum += (Values[index - CountX] + Values[index + CountX] - 2.0 * value) * dy;

                    // z
                    if (k == 0)
                        sum += (Values[index + CountXY] - value) * dz;
                    else if (k == CountZ - 1)
                        sum += (Values[index - CountXY] - value) * dz;
                    else
                        sum += (Values[index - CountXY] + Values[index + CountXY] - 2.0 * value) * dz;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rate"></param>
        private void GetLaplacianPeriodic(IList<Vec3d> result)
        {
            // inverse square step size for each dimension
            double dx = 1.0 / (ScaleX * ScaleX);
            double dy = 1.0 / (ScaleY * ScaleY);
            double dz = 1.0 / (ScaleZ * ScaleZ);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    Vec3d value = Values[index];
                    Vec3d sum = new Vec3d();

                    // x
                    if (i == 0)
                        sum += (Values[index - 1 + CountX] + Values[index + 1] - 2.0 * value) * dx;
                    else if (i == CountX - 1)
                        sum += (Values[index - 1] + Values[index + 1 - CountX] - 2.0 * value) * dx;
                    else
                        sum += (Values[index - 1] + Values[index + 1] - 2.0 * value) * dx;

                    // y
                    if (j == 0)
                        sum += (Values[index - CountX + CountXY] + Values[index + CountX] - 2.0 * value) * dy;
                    else if (j == CountY - 1)
                        sum += (Values[index - CountX] + Values[index + CountX - CountXY] - 2.0 * value) * dy;
                    else
                        sum += (Values[index - CountX] + Values[index + CountX] - 2.0 * value) * dy;

                    // z
                    if (k == 0)
                        sum += (Values[index - CountXY + Count] + Values[index + CountXY] - 2.0 * value) * dz;
                    else if (k == CountZ - 1)
                        sum += (Values[index - CountXY] + Values[index + CountXY - Count] - 2.0 * value) * dz;
                    else
                        sum += (Values[index - CountXY] + Values[index + CountXY] - 2.0 * value) * dz;

                    result[index] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public ScalarField3d GetDivergence()
        {
            ScalarField3d result = new ScalarField3d(this);
            UpdateDivergence(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateDivergence(ScalarField3d result)
        {
            UpdateDivergence(result.Values);
        }


        /// <summary>
        /// TODO
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        /// <param name="result"></param>
        public void UpdateDivergence(IList<double> result)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public VectorField3d GetCurl()
        {
            VectorField3d result = new VectorField3d((Field3d)this);
            UpdateCurl(result.Values);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCurl(VectorField3d result)
        {
            UpdateCurl(result.Values);
        }


        /// <summary>
        /// TODO
        /// http://www.math.harvard.edu/archive/21a_spring_09/PDF/13-05-curl-and-divergence.pdf
        /// </summary>
        /// <param name="result"></param>
        public void UpdateCurl(IList<Vec3d> result)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("VectorField3d ({0} x {1} x {2})", CountX, CountY, CountZ);
        }
    }
}
