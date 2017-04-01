using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Various methods for exporting and importing field data.
    /// </summary>
    public static class FieldIO
    {
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="mapper"></param>
        /// <param name="domain"></param>
        /// <param name="wrapX"></param>
        /// <param name="wrapY"></param>
        /// <param name="wrapZ"></param>
        /// <returns></returns>
        public static F CreateFromImageStack<F, T>(string path, Func<Color, T> mapper, Domain3d domain, FieldWrapMode wrapX = FieldWrapMode.Clamp, FieldWrapMode wrapY = FieldWrapMode.Clamp, FieldWrapMode wrapZ = FieldWrapMode.Clamp)
            where F : Field3d<T>
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <param name="mapper"></param>
        /// <param name="domain"></param>
        /// <param name="wrapX"></param>
        /// <param name="wrapY"></param>
        /// <param name="wrapZ"></param>
        /// <returns></returns>
        public static F CreateFromImageStack<F, T>(IReadOnlyList<Bitmap> bitmaps, Func<Color, T> mapper, Domain3d domain, FieldWrapMode wrapX = FieldWrapMode.Clamp, FieldWrapMode wrapY = FieldWrapMode.Clamp, FieldWrapMode wrapZ = FieldWrapMode.Clamp)
            where F : Field3d<T>
        {
            var bmp0 = bitmaps[0];
            int nx = bmp0.Width;
            int ny = bmp0.Height;
            int nz = bitmaps.Count;

            var result = (F)Activator.CreateInstance(typeof(F), new object[] { domain, nx, ny, nz, wrapX, wrapY, wrapZ });
            FieldIO.ReadFromImageStack<T>(result, bitmaps, mapper);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="mapper"></param>
        /// <param name="domain"></param>
        /// <param name="wrapX"></param>
        /// <param name="wrapY"></param>
        /// <returns></returns>
        public static F CreateFromImage<F, T>(string path, Func<Color, T> mapper, Domain2d domain, FieldWrapMode wrapX = FieldWrapMode.Clamp, FieldWrapMode wrapY = FieldWrapMode.Clamp)
            where F : Field2d<T>
        {
            using (Bitmap bmp = new Bitmap(path))
            {
                return CreateFromImage<F,T>(bmp, mapper, domain, wrapX, wrapX);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="bitmap"></param>
        /// <param name="mapper"></param>
        /// <param name="domain"></param>
        /// <param name="wrapX"></param>
        /// <param name="wrapY"></param>
        /// <returns></returns>
        public static F CreateFromImage<F, T>(Bitmap bitmap, Func<Color, T> mapper, Domain2d domain, FieldWrapMode wrapX = FieldWrapMode.Clamp, FieldWrapMode wrapY = FieldWrapMode.Clamp)
            where F : Field2d<T>
        {
            int nx = bitmap.Width;
            int ny = bitmap.Height;
     
            var result = (F)Activator.CreateInstance(typeof(F), new object[] { domain, nx, ny, wrapX, wrapY});
            FieldIO.ReadFromImage<T>(result, bitmap, mapper);

            return result;
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="bitmaps"></param>
        /// <param name="mapper"></param>
        public static void ReadFromImageStack<T>(Field3d<T> field, IEnumerable<Bitmap> bitmaps, Func<Color, T> mapper)
        {
            int nxy = field.CountXY;
            int count = 0;

            Parallel.ForEach(bitmaps, bitmap =>
            {
                ReadFromImage(field.Values, count * nxy, bitmap, mapper);
            });

        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="layer"></param>
        /// <param name="mapper"></param>
        /// <param name="bitmap"></param>
        public static void ReadFromImage<T>(Field3d<T> field, int layer, Bitmap bitmap, Func<Color, T> mapper)
        {
            ReadFromImage(field.Values, layer * field.CountXY, bitmap, mapper);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="bitmap"></param>
        public static void ReadFromImage<T>(Field2d<T> field, Bitmap bitmap, Func<Color, T> mapper)
        {
            ReadFromImage(field.Values, 0, bitmap, mapper);
        }


        /// <summary>
        /// Writes a layer of the given field to an existing image.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="layer"></param>
        /// <param name="mapper"></param>
        /// <param name="bitmap"></param>
        public static void WriteToImage<T>(Field3d<T> field, int layer, Bitmap bitmap, Func<T, Color> mapper)
        {
            WriteToImage(field.Values, layer * field.CountXY, bitmap, mapper);
        }


        /// <summary>
        /// Writes the given field to an existing image.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="bitmap"></param>
        public static void WriteToImage<T>(Field2d<T> field, Bitmap bitmap, Func<T, Color> mapper)
        {
            WriteToImage(field.Values, 0, bitmap, mapper);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void ReadFromImage<T>(T[] values, int index, Bitmap bitmap, Func<Color, T> mapper)
        {
            PixelFormat pf = bitmap.PixelFormat;
            int bpp = Bitmap.GetPixelFormatSize(pf) >> 3; // bytes per pixel
            PixelFormatCheck(bpp); // ensure 4 bytes per pixel

            unsafe
            {
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, pf);
                byte* first = (byte*)bmpData.Scan0;

                if (bpp == 3)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        byte* currLn = first + (y * bmpData.Stride);

                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            int bx = x * bpp;
                            byte b = currLn[bx];
                            byte g = currLn[bx + 1];
                            byte r = currLn[bx + 2];

                            values[index++] = mapper(Color.FromArgb(r, g, b));
                        }
                    }
                }
                else
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        byte* currLn = first + (y * bmpData.Stride);

                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            int bx = x * bpp;
                            byte b = currLn[bx];
                            byte g = currLn[bx + 1];
                            byte r = currLn[bx + 2];
                            byte a = currLn[bx + 3];

                            values[index++] = mapper(Color.FromArgb(a, r, g, b));
                        }
                    }

                }

                bitmap.UnlockBits(bmpData);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void WriteToImage<T>(T[] values, int index, Bitmap bitmap, Func<T, Color> mapper)
        {
            PixelFormat pf = bitmap.PixelFormat;
            int bpp = Bitmap.GetPixelFormatSize(pf) >> 3; // bytes per pixel
            PixelFormatCheck(bpp);

            unsafe
            {
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, pf);
                byte* first = (byte*)bmpData.Scan0;

                if (bpp == 3)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        byte* currLn = first + (y * bmpData.Stride);

                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            Color c = mapper(values[index++]);

                            int bx = x * bpp;
                            currLn[bx] = c.B;
                            currLn[bx + 1] = c.G;
                            currLn[bx + 2] = c.R;
                        }
                    }
                }
                else
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        byte* currLn = first + (y * bmpData.Stride);

                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            Color c = mapper(values[index++]);

                            int bx = x * bpp;
                            currLn[bx] = c.B;
                            currLn[bx + 1] = c.G;
                            currLn[bx + 2] = c.R;
                            currLn[bx + 3] = c.A;
                        }
                    }
                }

                bitmap.UnlockBits(bmpData);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void PixelFormatCheck(int bytesPerPixel)
        {
            if (bytesPerPixel < 3)
                throw new NotSupportedException("The pixel format of the given bitmap is not supported.");
        }
    }
}
