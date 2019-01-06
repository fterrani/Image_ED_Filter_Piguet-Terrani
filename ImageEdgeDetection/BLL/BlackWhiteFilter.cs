using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ImageEdgeDetection
{
    public class BlackWhiteFilter : PixelFilter
    {
        public BlackWhiteFilter(string name) : base(name)
        {

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

            int average = 0;

            // Converting each pixel to grayscale
            for (int k = 0; k < pixelBuffer.Length; k += 4)
            {
                // Computing arithmetic mean of red, green and blue channels
                average = pixelBuffer[k + 0];
                average += pixelBuffer[k + 1];
                average += pixelBuffer[k + 2];
                average /= 3;

                // Assigning the computed mean to all channels
                pixelBuffer[k + 0] = (byte) average;
                pixelBuffer[k + 1] = (byte) average;
                pixelBuffer[k + 2] = (byte) average;
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
