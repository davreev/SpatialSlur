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
    public static class FieldIO
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="path"></param>
        public static void SaveAsImageStack<T>(Field3d<T> field, Func<T, Color> mapper, string path)
        {
            Rectangle rect = new Rectangle(0, 0, field.CountX, field.CountY);
            PixelFormat pf = PixelFormat.Format32bppArgb;
            int bpp = Bitmap.GetPixelFormatSize(pf) / 8; // bytes per pixel

            Parallel.For(0, field.CountZ, z =>
                {
                    using (Bitmap bmp = new Bitmap(field.CountX, field.CountY, pf))
                    {
                        unsafe
                        {
                            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, pf);
                            byte* first = (byte*)bmpData.Scan0;

                            for (int y = 0; y < field.CountY; y++)
                            {
                                byte* currLn = first + (y * bmpData.Stride);

                                for (int x = 0; x < field.CountX; x++)
                                {
                                    Color c = mapper(field.Values[field.FlattenIndex(x, y, z)]);

                                    int bx = x * bpp;
                                    currLn[bx] = c.B;
                                    currLn[bx + 1] = c.G;
                                    currLn[bx + 2] = c.R;
                                    currLn[bx + 3] = c.A;
                                }
                            }

                            bmp.UnlockBits(bmpData);
                            bmp.Save(String.Format("{0}_{1}.png", path, z));
                        }
                    }
                });
        }


        /// <summary>
        /// Allows for supersampling of the field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="path"></param>
        public static void SaveAsImageStack<T>(Field3d<T> field, Func<T, Color> mapper, string path, int width, int height, int layers)
        {
            Rectangle rect = new Rectangle(0, 0, width, height);
            PixelFormat pf = PixelFormat.Format32bppArgb;
            int bpp = Bitmap.GetPixelFormatSize(pf) / 8; // bytes per pixel

            Domain3d domain = field.Domain;
            double tx = 1.0 / (width - 1);
            double ty = 1.0 / (height - 1);
            double tz = 1.0 / (layers - 1);

            Parallel.For(0, layers, z =>
                {
                    FieldPoint3d fp = new FieldPoint3d();
                    double w = z * tz;

                    using (Bitmap bmp = new Bitmap(width, height, pf))
                    {
                        unsafe
                        {
                            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, pf);
                            byte* first = (byte*)bmpData.Scan0;

                            for (int y = 0; y < height; y++)
                            {
                                byte* currLn = first + (y * bmpData.Stride);
                                double v = y * ty;

                                for (int x = 0; x < width; x++)
                                {
                                    double u = x * tx;
                                    field.FieldPointAt(domain.Evaluate(new Vec3d(u, v, w)), fp);
                                    Color c = mapper(field.Evaluate(fp));

                                    int bx = x * bpp;
                                    currLn[bx] = c.B;
                                    currLn[bx + 1] = c.G;
                                    currLn[bx + 2] = c.R;
                                    currLn[bx + 3] = c.A;
                                }
                            }

                            bmp.UnlockBits(bmpData);
                            bmp.Save(String.Format("{0}_{1}.png", path, z));
                        }
                    }
                });
        }


        /// <summary>
        /// TODO test
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="path"></param>
        public static void SaveAsImage<T>(Field2d<T> field, Func<T, Color> mapper, string path)
        {
            Rectangle rect = new Rectangle(0, 0, field.CountX, field.CountY);
            PixelFormat pf = PixelFormat.Format32bppArgb;
            int bpp = Bitmap.GetPixelFormatSize(pf) / 8; // bytes per pixel

            using (Bitmap bmp = new Bitmap(field.CountX, field.CountY, pf))
            {
                unsafe
                {
                    BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, pf);
                    byte* first = (byte*)bmpData.Scan0;

                    Parallel.For(0, field.CountY, y =>
                    {
                        byte* currLn = first + (y * bmpData.Stride);

                        for (int x = 0; x < field.CountX; x++)
                        {
                            Color c = mapper(field.Values[field.FlattenIndex(x, y)]);

                            int bx = x * bpp;
                            currLn[bx] = c.B;
                            currLn[bx + 1] = c.G;
                            currLn[bx + 2] = c.R;
                            currLn[bx + 3] = c.A;
                        }
                    });

                    bmp.UnlockBits(bmpData);
                    bmp.Save(String.Format("{0}.png", path));
                }
            }
        }


        /// <summary>
        /// TODO test
        /// Allows for supersampling of the field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="path"></param>
        public static void SaveAsImage<T>(Field2d<T> field, Func<T, Color> mapper, string path, int width, int height)
        {
            Rectangle rect = new Rectangle(0, 0, width, height);
            PixelFormat pf = PixelFormat.Format32bppArgb;
            int bpp = Bitmap.GetPixelFormatSize(pf) / 8; // bytes per pixel

            Domain2d domain = field.Domain;
            double tx = 1.0 / (width - 1);
            double ty = 1.0 / (height - 1);

            using (Bitmap bmp = new Bitmap(width, height, pf))
            {
                unsafe
                {
                    BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, pf);
                    byte* first = (byte*)bmpData.Scan0;

                    Parallel.For(0, height, y =>
                    {
                        FieldPoint2d fp = new FieldPoint2d();
                        byte* currLn = first + (y * bmpData.Stride);
                        double v = y * ty;

                        for (int x = 0; x < width; x++)
                        {
                            double u = x * tx;
                            field.FieldPointAt(domain.Evaluate(new Vec2d(u, v)), fp);
                            Color c = mapper(field.Evaluate(fp));

                            int bx = x * bpp;
                            currLn[bx] = c.B;
                            currLn[bx + 1] = c.G;
                            currLn[bx + 2] = c.R;
                            currLn[bx + 3] = c.A;
                        }
                    });

                    bmp.UnlockBits(bmpData);
                    bmp.Save(String.Format("{0}.png", path));
                }
            }
        }
        
    }
}
