using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageEDFilter
{
    class ImageFilters
    {
        // Rainbow Filter - Colors the left side of the image with colored bands
        public static Bitmap RainbowFilter(Bitmap bmp)
        {

            Bitmap temp = new Bitmap(bmp.Width, bmp.Height);
            int raz = bmp.Height / 4;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int x = 0; x < bmp.Height; x++)
                {

                    if (i < (raz))
                    {
                        // Cyan band
                        temp.SetPixel(i, x, Color.FromArgb(bmp.GetPixel(i, x).R / 5, bmp.GetPixel(i, x).G, bmp.GetPixel(i, x).B));
                    }
                    else if (i < (raz * 2))
                    {
                        // Magenta band
                        temp.SetPixel(i, x, Color.FromArgb(bmp.GetPixel(i, x).R, bmp.GetPixel(i, x).G / 5, bmp.GetPixel(i, x).B));
                    }
                    else if (i < (raz * 3))
                    {
                        // Yellow band
                        temp.SetPixel(i, x, Color.FromArgb(bmp.GetPixel(i, x).R, bmp.GetPixel(i, x).G, bmp.GetPixel(i, x).B / 5));
                    }
                    else if (i < (raz * 4))
                    {
                        // Green band
                        temp.SetPixel(i, x, Color.FromArgb(bmp.GetPixel(i, x).R / 5, bmp.GetPixel(i, x).G, bmp.GetPixel(i, x).B / 5));
                    }
                    else
                    {
                        // Remaining of the image is darkened
                        temp.SetPixel(i, x, Color.FromArgb(bmp.GetPixel(i, x).R / 5, bmp.GetPixel(i, x).G / 5, bmp.GetPixel(i, x).B / 5));
                    }
                }

            }
            return temp;
        }

        // Divides each channel value with the provided numbers (allowing to change the image's colors)
        public static Bitmap ApplyFilter(Bitmap bmp, int alpha, int red, int blue, int green)
        {

            Bitmap temp = new Bitmap(bmp.Width, bmp.Height);


            for (int i = 0; i < bmp.Width; i++)
            {
                for (int x = 0; x < bmp.Height; x++)
                {
                    Color c = bmp.GetPixel(i, x);
                    Color cLayer = Color.FromArgb(c.A / alpha, c.R / red, c.G / green, c.B / blue);
                    temp.SetPixel(i, x, cLayer);
                }

            }
            return temp;
        }

        // Black and white filter (grayscale)
        public static Bitmap BlackWhite(Bitmap bmp)
        {
            int rgb;
            Color c;
            Bitmap temp = new Bitmap(bmp.Width, bmp.Height);


            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    c = bmp.GetPixel(x, y);
                    rgb = (int)((c.R + c.G + c.B) / 3);
                    temp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }
            return temp;

        }

        // Swaps color channels (G->R, B->G, R->B)
        public static Bitmap ApplyFilterSwap(Bitmap bmp)
        {

            Bitmap temp = new Bitmap(bmp.Width, bmp.Height);


            for (int i = 0; i < bmp.Width; i++)
            {
                for (int x = 0; x < bmp.Height; x++)
                {
                    Color c = bmp.GetPixel(i, x);
                    Color cLayer = Color.FromArgb(c.A, c.G, c.B, c.R);
                    temp.SetPixel(i, x, cLayer);
                }

            }
            return temp;
        }

        // Divide each channel with a number (see ApplyFilter) and swaps color channels (G->R, B->G, R->B)
        public static Bitmap ApplyFilterSwapDivide(Bitmap bmp, int a, int r, int g, int b)
        {

            Bitmap temp = new Bitmap(bmp.Width, bmp.Height);


            for (int i = 0; i < bmp.Width; i++)
            {
                for (int x = 0; x < bmp.Height; x++)
                {
                    Color c = bmp.GetPixel(i, x);
                    Color cLayer = Color.FromArgb(c.A / a, c.G / g, c.B / b, c.R / r);
                    temp.SetPixel(i, x, cLayer);
                }

            }
            return temp;
        }


        // Turns pixels whose green channel value falls in the ]min;max[ interval to white
        // Sets other pixel's color to co
        public static Bitmap ApplyFilterMega(Bitmap bmp, int max, int min, Color co)
        {

            Bitmap temp = new Bitmap(bmp.Width, bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int x = 0; x < bmp.Height; x++)
                {

                    Color c = bmp.GetPixel(i, x);
                    if (c.G > min && c.G < max)
                    {
                        temp.SetPixel(i, x, Color.White);
                    }
                    else
                    {
                        temp.SetPixel(i, x, co);
                    }

                }

            }
            return temp;
        }

        // BUGGED FILTER! (works only on square images)
        // Apply magic mosaic. The image is divided in different parts. Some are mirrored along a diagonal, others are left as is.
        public static Bitmap DivideCrop(Bitmap bmp)
        {
            int razX = Convert.ToInt32(bmp.Width / 3);
            int razY = Convert.ToInt32(bmp.Height / 3);

            Bitmap temp = new Bitmap(bmp.Width, bmp.Height);


            for (int i = 0; i < bmp.Width - 1; i++)
            {
                for (int x = 0; x < bmp.Height - 1; x++)
                {
                    if (i < razX && x < razY)
                    {
                        temp.SetPixel(i, x, bmp.GetPixel(i, x));
                    }
                    else if (i < (razX * 2) && x < (razY))
                    {
                        temp.SetPixel(i, x, bmp.GetPixel(x, i));
                    }
                    else if (i < (razX * 3) && x < (razY))
                    {
                        temp.SetPixel(i, x, bmp.GetPixel(i, x));
                    }
                    else if (i < (razX) && x < (razY * 2))
                    {
                        temp.SetPixel(i, x, bmp.GetPixel(x, i));
                    }
                    else if (i < (razX) && x < (razY * 3))
                    {
                        temp.SetPixel(i, x, bmp.GetPixel(i, x));
                    }
                    else if (i < (razX * 2) && x < (razY * 2))
                    {
                        temp.SetPixel(i, x, bmp.GetPixel(i, x));
                    }
                    else if (i < (razX * 4) && x < (razY * 1))
                    {
                        temp.SetPixel(i, x, bmp.GetPixel(i, x));
                    }
                    else if (i < (razX * 4) && x < (razY * 2))
                    {
                        temp.SetPixel(i, x, bmp.GetPixel(x / 2, i / 2));
                    }
                    else if (i < (razX * 4) && x < (razY * 3))
                    {
                        temp.SetPixel(i, x, bmp.GetPixel(x / 3, i / 3));
                    }

                }

            }
            return temp;
        }

    }


}
