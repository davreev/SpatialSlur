using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using SpatialSlur.SlurCore;

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
        /// <summary>
        /// Saves the given field as a stack of images.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="path"></param>
        public static void SaveAsImageStack<T>(Field3d<T> field, string path, Func<T, Color> mapper)
        {
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            Parallel.For(0, field.CountZ, z =>
                {
                    using (Bitmap bmp = new Bitmap(field.CountX, field.CountY, PixelFormat.Format32bppArgb))
                    {
                        WriteToImage(field, z, bmp, mapper);
                        bmp.Save(String.Format(@"{0}\{1}_{2}{3}", dir, name, z, ext));
                    }
                });
        }


        /// <summary>
        /// Saves the given field as an image.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="path"></param>
        public static void SaveAsImage<T>(Field2d<T> field, string path, Func<T, Color> mapper)
        {
            using (Bitmap bmp = new Bitmap(field.CountX, field.CountY, PixelFormat.Format32bppArgb))
            {
                WriteToImage(field, bmp, mapper);
                bmp.Save(path);
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="bitmap"></param>
        /// <param name="mapper"></param>
        public static void ReadFromImage<T>(Field3d<T> field, Bitmap bitmap, Func<Color, T> mapper)
        {
            ReadFromImage(field.Values, 0, bitmap, mapper);

            // copy values of first layer to others 
            // Note could create undesired behaviour if T is a reference type
            var values = field.Values;
            int nxy = field.CountXY;

            for (int i = 1; i < field.CountZ; i++)
                Array.Copy(values, 0, values, i * nxy, nxy);
        }
        */
   

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
        private static void ReadFromImage<T>(IList<T> values, int index, Bitmap bitmap, Func<Color, T> mapper)
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
        private static void WriteToImage<T>(IList<T> values, int index, Bitmap bitmap, Func<T, Color> mapper)
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public static void WriteFGA(Field3d<Vec3d> field)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
