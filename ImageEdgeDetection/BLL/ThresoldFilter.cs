using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ImageEdgeDetection
{
    // This filter colors the pixels of a bitmap in white or in a custom color depending on the pixel's perceived luminance value
    public class ThresoldFilter : PixelFilter
    {
        // Min and max define a luminance interval where pixels will be colored in white
        private float min;
        private float max;

        // If the luminance is not in the defined interval, a custom color is applied
        private Color color;

        public ThresoldFilter(string name, float _min, float _max, Color _color) : base(name)
        {
            min = _min;
            max = _max;
            color = _color;
        }

        public override Bitmap Apply(Bitmap sourceBitmap)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                     sourceBitmap.Width, sourceBitmap.Height),
                                                       ImageLockMode.ReadOnly,
                                                       PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            float luminance = 0.0f;
            byte red, green, blue;

            // Converting each pixel to grayscale
            for (int k = 0; k < pixelBuffer.Length; k += 4)
            {
                // Computing weighted arithmetic mean of red, green and blue channels
                luminance = pixelBuffer[k + 0]/255.0f * 0.11f;
                luminance += pixelBuffer[k + 1]/255.0f * 0.59f;
                luminance += pixelBuffer[k + 2]/255.0f * 0.3f;

                // If luminance is in range,
                if (luminance >= min && luminance <= max)
                {
                    // we set the pixel to white
                    red = 255;
                    green = 255;
                    blue = 255;
                }

                else
                {
                    // otherwise we apply the custom color
                    red = color.R;
                    green = color.G;
                    blue = color.B;
                }

                // Assigning the right color to the current pixel
                pixelBuffer[k + 0] = blue;
                pixelBuffer[k + 1] = green;
                pixelBuffer[k + 2] = red;
                pixelBuffer[k + 3] = 255;
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                     resultBitmap.Width, resultBitmap.Height),
                                                      ImageLockMode.WriteOnly,
                                                      PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
    }
}
