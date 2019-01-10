using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ImageEdgeDetection
{
    public class MatrixEdgeFilter : EdgeFilter
    {
        // Fields used for simple convolution
        private double[,] matrix = null;
        private double factor = 1.0;
        private int bias = 0;

        // Boolean for conversion to grayscale (perceived luminance)
        private bool grayscale = false;

        // The convolution function to apply on the Bitmap. Can be a simple or XY convolution.
        Func<byte[], int, int, int, byte[]> convolutionFunc;

        public MatrixEdgeFilter(string name, double[,] _matrix, bool _grayscale, double _factor, int _bias) : base(name)
        {
            matrix = _matrix ?? throw new ArgumentNullException("_matrix");
            grayscale = _grayscale;
            factor = _factor;
            bias = _bias;

            byte[] simpleConvFunc(byte[] pixelBuffer, int width, int height, int stride)
            {
                // We apply only one matrix on the buffer
                return SimpleConvolution(pixelBuffer, width, height, stride, matrix, factor, bias);
            }

            convolutionFunc = simpleConvFunc;
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            
            return ApplyConvolutionFunc(bmp, convolutionFunc, grayscale);
        }


        // Applies the provided convolution function on sourceBitmap
        private Bitmap ApplyConvolutionFunc(Bitmap sourceBitmap, Func<byte[], int, int, int, byte[]> convolutionFunc, bool grayscale = false)
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
                    rgb = pixelBuffer[k + 0] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;


                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }

            // We apply the provided convolution function
            byte[] resultBuffer = convolutionFunc(pixelBuffer, sourceBitmap.Width, sourceBitmap.Height, sourceData.Stride);


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
        private byte[] SimpleConvolution(byte[] pixelBuffer, int width, int height, int stride, double[,] filterMatrix, double factor = 1, int bias = 0)
        {
            byte[] resultBuffer = new byte[pixelBuffer.Length];
            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;

            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY < height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < width - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;

                    byteOffset = offsetY *
                                 stride +
                                 offsetX * 4;

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {

                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * stride);

                            blue += (double)(pixelBuffer[calcOffset]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                            green += (double)(pixelBuffer[calcOffset + 1]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                            red += (double)(pixelBuffer[calcOffset + 2]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
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

        public static readonly double[,] MATRIX_LAPLACIAN_3X3 = new double[,] {
            { -1, -1, -1, },
            { -1,  8, -1, },
            { -1, -1, -1, }
        };

        public static readonly double[,] MATRIX_LAPLACIAN_5X5 = new double[,] {
            { -1, -1, -1, -1, -1, },
            { -1, -1, -1, -1, -1, },
            { -1, -1, 24, -1, -1, },
            { -1, -1, -1, -1, -1, },
            { -1, -1, -1, -1, -1 }
        };
    }
}
