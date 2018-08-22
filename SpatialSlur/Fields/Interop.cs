
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using SpatialSlur.Fields;

namespace SpatialSlur
{
    /// <summary>
    /// Static methods for importing from and exporting to external formats.
    /// </summary>
    public static partial class Interop
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Fields
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="field"></param>
            /// <param name="path"></param>
            /// <param name="mapper"></param>
            public static void SaveAsImageStack<T>(GridField3d<T> field, string path, Func<T, Color> mapper)
                where T : struct
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
            /// 
            /// </summary>
            /// <param name="field"></param>
            /// <param name="path"></param>
            /// <param name="mapper"></param>
            public static void SaveAsImage<T>(GridField2d<T> field, string path, Func<T, Color> mapper)
                where T : struct
            {
                using (Bitmap bmp = new Bitmap(field.CountX, field.CountY, PixelFormat.Format32bppArgb))
                {
                    WriteToImage(field, bmp, mapper);
                    bmp.Save(path);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="images"></param>
            /// <param name="field"></param>
            /// <param name="mapper"></param>
            public static void ReadFromImageStack<T>(IEnumerable<Bitmap> images, GridField3d<T> field, Func<Color, T> mapper)
                where T : struct
            {
                int nxy = field.CountXY;
                int index = 0;

                Parallel.ForEach(images, image =>
                {
                    ReadFromImage(image, field.Values, index, mapper);
                    index += nxy;
                });
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="field"></param>
            /// <param name="layer"></param>
            /// <param name="mapper"></param>
            /// <param name="image"></param>
            public static void ReadFromImage<T>(Bitmap image, GridField3d<T> field, int layer, Func<Color, T> mapper)
                where T : struct
            {
                ReadFromImage(image, field.Values, layer * field.CountXY, mapper);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="field"></param>
            /// <param name="mapper"></param>
            /// <param name="image"></param>
            public static void ReadFromImage<T>(Bitmap image, GridField2d<T> field, Func<Color, T> mapper)
                where T : struct
            {
                ReadFromImage(image, field.Values, 0, mapper);
            }


            /// <summary>
            /// Writes a layer of the given field to an existing image.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="field"></param>
            /// <param name="layer"></param>
            /// <param name="mapper"></param>
            /// <param name="image"></param>
            public static void WriteToImage<T>(GridField3d<T> field, int layer, Bitmap image, Func<T, Color> mapper)
                where T : struct
            {
                WriteToImage(field.Values, layer * field.CountXY, image, mapper);
            }


            /// <summary>
            /// Writes the given field to an existing image.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="field"></param>
            /// <param name="mapper"></param>
            /// <param name="image"></param>
            public static void WriteToImage<T>(GridField2d<T> field, Bitmap image, Func<T, Color> mapper)
                where T : struct
            {
                WriteToImage(field.Values, 0, image, mapper);
            }


            /// <summary>
            /// 
            /// </summary>
            private static void ReadFromImage<T>(Bitmap image, T[] values, int index, Func<Color, T> mapper)
            {
                PixelFormat pf = image.PixelFormat;
                int stride = Image.GetPixelFormatSize(pf) >> 3; // bytes per pixel
                PixelFormatCheck(stride); // ensure 4 bytes per pixel

                unsafe
                {
                    BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, pf);
                    byte* first = (byte*)bmpData.Scan0;

                    if (stride == 3)
                    {
                        for (int y = 0; y < image.Height; y++)
                        {
                            byte* currLn = first + (y * bmpData.Stride);

                            for (int x = 0; x < image.Width; x++)
                            {
                                int bx = x * stride;
                                byte b = currLn[bx];
                                byte g = currLn[bx + 1];
                                byte r = currLn[bx + 2];

                                values[index++] = mapper(Color.FromArgb(r, g, b));
                            }
                        }
                    }
                    else
                    {
                        for (int y = 0; y < image.Height; y++)
                        {
                            byte* currLn = first + (y * bmpData.Stride);

                            for (int x = 0; x < image.Width; x++)
                            {
                                int bx = x * stride;
                                byte b = currLn[bx];
                                byte g = currLn[bx + 1];
                                byte r = currLn[bx + 2];
                                byte a = currLn[bx + 3];

                                values[index++] = mapper(Color.FromArgb(a, r, g, b));
                            }
                        }

                    }

                    image.UnlockBits(bmpData);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static void WriteToImage<T>(T[] values, int index, Bitmap image, Func<T, Color> mapper)
            {
                PixelFormat pf = image.PixelFormat;
                int bpp = Image.GetPixelFormatSize(pf) >> 3; // bytes per pixel
                PixelFormatCheck(bpp);

                unsafe
                {
                    BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, pf);
                    byte* first = (byte*)bmpData.Scan0;

                    if (bpp == 3)
                    {
                        for (int y = 0; y < image.Height; y++)
                        {
                            byte* currLn = first + (y * bmpData.Stride);

                            for (int x = 0; x < image.Width; x++)
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
                        for (int y = 0; y < image.Height; y++)
                        {
                            byte* currLn = first + (y * bmpData.Stride);

                            for (int x = 0; x < image.Width; x++)
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

                    image.UnlockBits(bmpData);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static void PixelFormatCheck(int bytesPerPixel)
            {
                const string errorMessage = "The pixel format of the given bitmap is not supported.";

                if (bytesPerPixel < 3)
                    throw new NotSupportedException(errorMessage);
            }
        }
    }
}
