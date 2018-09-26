/*
 * The Following Code was developed by Dewald Esterhuizen
 * View Documentation at: http://softwarebydefault.com
 * Licensed under Ms-PL 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace ImageEDFilter
{
    public static class ExtBitmap
    {
        // Copies sourceBitmap to a new Bitmap object (the image is resized; proportions are preserved)
        public static Bitmap CopyToSquareCanvas(this Bitmap sourceBitmap, int canvasWidthLength)
        {
            float ratio = 1.0f;
            int maxSide = sourceBitmap.Width > sourceBitmap.Height ?
                          sourceBitmap.Width : sourceBitmap.Height;

            ratio = (float)maxSide / (float)canvasWidthLength;

            Bitmap bitmapResult = (sourceBitmap.Width > sourceBitmap.Height ?
                                    new Bitmap(canvasWidthLength, (int)(sourceBitmap.Height / ratio))
                                    : new Bitmap((int)(sourceBitmap.Width / ratio), canvasWidthLength));

            using (Graphics graphicsResult = Graphics.FromImage(bitmapResult))
            {
                graphicsResult.CompositingQuality = CompositingQuality.HighQuality;
                graphicsResult.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsResult.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphicsResult.DrawImage(sourceBitmap,
                                        new Rectangle(0, 0,
                                            bitmapResult.Width, bitmapResult.Height),
                                        new Rectangle(0, 0,
                                            sourceBitmap.Width, sourceBitmap.Height),
                                            GraphicsUnit.Pixel);
                graphicsResult.Flush();
            }

            return bitmapResult;
        }

        // Applies a simple convolution filter on sourceBitmap
        public static Bitmap ConvolutionFilter(Bitmap sourceBitmap,
                                             double[,] filterMatrix,
                                                  double factor = 1,
                                                       int bias = 0,
                                             bool grayscale = false)
        {
            byte[] simpleConvFunc(byte[] pixelBuffer, int width, int height, int stride)
            {
                // We apply only one matrix on the buffer
                return SimpleConvolution(pixelBuffer, width, height, stride, filterMatrix, factor, bias);
            }

            return ApplyConvolutionFunc(sourceBitmap, simpleConvFunc, grayscale);
        }

        // Applies an XY convolution filter on sourceBitmap
        public static Bitmap ConvolutionFilter(Bitmap sourceBitmap,
                                                double[,] xFilterMatrix,
                                                double[,] yFilterMatrix,
                                                 bool grayscale = false)
        {
            byte[] xyConvFunc(byte[] pixelBuffer, int width, int height, int stride)
            {
                // We apply two matrices on the buffer (one for X, the other for Y)
                return XYConvolution(pixelBuffer, width, height, stride, xFilterMatrix, yFilterMatrix);
            }

            return ApplyConvolutionFunc(sourceBitmap, xyConvFunc, grayscale);
        }

        // Applies the provided convolution function on sourceBitmap
        private static Bitmap ApplyConvolutionFunc(Bitmap sourceBitmap, Func<byte[], int, int, int, byte[]> convolutionFunc, bool grayscale = false)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                     sourceBitmap.Width, sourceBitmap.Height),
                                                       ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            if (grayscale == true)
            {
                float rgb = 0;

                // Converting each pixel to grayscale
                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb =  pixelBuffer[k + 0] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;


                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }

            // We apply the provided convolution function
            byte[] resultBuffer = convolutionFunc( pixelBuffer, sourceBitmap.Width, sourceBitmap.Height, sourceData.Stride );
            

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                     resultBitmap.Width, resultBitmap.Height),
                                                      ImageLockMode.WriteOnly,
                                                 PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        // Applies a single matrix on pixelBuffer
        private static byte[] SimpleConvolution(byte[] pixelBuffer, int width, int height, int stride, double[,] filterMatrix, double factor = 1, int bias = 0)
        {
            byte[] resultBuffer = new byte[ pixelBuffer.Length ];
            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;

            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY <
                height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    width - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;

                    byteOffset = offsetY *
                                 stride +
                                 offsetX * 4;

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {

                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * stride);

                            blue += (double)(pixelBuffer[calcOffset]) *
                                    filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            green += (double)(pixelBuffer[calcOffset + 1]) *
                                     filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            red += (double)(pixelBuffer[calcOffset + 2]) *
                                   filterMatrix[filterY + filterOffset,
                                                      filterX + filterOffset];
                        }
                    }

                    blue = factor * blue + bias;
                    green = factor * green + bias;
                    red = factor * red + bias;

                    // Clamping blue, green and red between 0 and 255
                    if (blue > 255) blue = 255;
                    else if (blue < 0) blue = 0;

                    if (green > 255) green = 255;
                    else if (green < 0) green = 0;

                    if (red > 255) red = 255;
                    else if (red < 0) red = 0;

                    resultBuffer[byteOffset + 0] = (byte)(blue);
                    resultBuffer[byteOffset + 1] = (byte)(green);
                    resultBuffer[byteOffset + 2] = (byte)(red);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            return resultBuffer;
        }

        // Applies two matrices on pixelBuffer (one for X, the other for Y)
        private static byte[] XYConvolution( byte[] pixelBuffer, int width, int height, int stride, double[,] xFilterMatrix, double[,] yFilterMatrix )
        {
            byte[] resultBuffer = new byte[ pixelBuffer.Length ];

            double blueX = 0.0;
            double greenX = 0.0;
            double redX = 0.0;

            double blueY = 0.0;
            double greenY = 0.0;
            double redY = 0.0;

            double blueTotal = 0.0;
            double greenTotal = 0.0;
            double redTotal = 0.0;

            int filterOffset = 1;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY <
                height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    width - filterOffset; offsetX++)
                {
                    blueX = greenX = redX = 0;
                    blueY = greenY = redY = 0;

                    blueTotal = greenTotal = redTotal = 0.0;

                    byteOffset = offsetY *
                                 stride +
                                 offsetX * 4;

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * stride);

                            blueX += (double)(pixelBuffer[calcOffset]) *
                                      xFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            greenX += (double)(pixelBuffer[calcOffset + 1]) *
                                      xFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            redX += (double)(pixelBuffer[calcOffset + 2]) *
                                      xFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            blueY += (double)(pixelBuffer[calcOffset]) *
                                      yFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            greenY += (double)(pixelBuffer[calcOffset + 1]) *
                                      yFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            redY += (double)(pixelBuffer[calcOffset + 2]) *
                                      yFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];
                        }
                    }

                    blueTotal = Math.Sqrt((blueX * blueX) + (blueY * blueY));
                    greenTotal = Math.Sqrt((greenX * greenX) + (greenY * greenY));
                    redTotal = Math.Sqrt((redX * redX) + (redY * redY));

                    // Clamping blue, green and red between 0 and 255
                    if (blueTotal > 255) blueTotal = 255;
                    else if (blueTotal < 0) blueTotal = 0;

                    if (greenTotal > 255) greenTotal = 255;
                    else if (greenTotal < 0) greenTotal = 0;

                    if (redTotal > 255) redTotal = 255;
                    else if (redTotal < 0) redTotal = 0;


                    resultBuffer[byteOffset + 0] = (byte)(blueTotal);
                    resultBuffer[byteOffset + 1] = (byte)(greenTotal);
                    resultBuffer[byteOffset + 2] = (byte)(redTotal);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            return resultBuffer;
        }
    }  
}
