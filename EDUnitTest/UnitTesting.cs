﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDUnitTest
{
    [TestClass]
    public class UnitTesting
    {
        // Returns a byte array containing the bitmap's data in 4-byte ARGB format
        public void SetBitmapBytes(Bitmap b, byte[] pixels)
        {
            // We access the whole bitmap's data in 4-byte ARGB format
            BitmapData data = b.LockBits(
                new Rectangle(new Point(0, 0), b.Size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
            );

            // We check that we have
            int expectedLength = data.Stride * data.Height;
            bool correctLength = (pixels.Length == expectedLength);

            if (correctLength)
            {
                // We extract the bitmap bytes from the data
                byte[] bmpBytes = new byte[data.Stride * data.Height];
                Marshal.Copy(data.Scan0, bmpBytes, 0, bmpBytes.Length);
                Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            }

            // We unlock the bits we accessed
            b.UnlockBits(data);

            if (!correctLength)
            {
                throw new ArgumentException(
                    "Wrong pixel data length (expected = " + pixels.Length + " ; actual = " + expectedLength + ")"
                );
            }
        }

        // Returns a byte array containing the bitmap's data in 4-byte ARGB format
        public byte[] GetBitmapBytes(Bitmap b)
        {
            // We access the whole bitmap's data in 4-byte ARGB format
            BitmapData data = b.LockBits(
                new Rectangle(new Point(0, 0), b.Size),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            // We extract the bitmap bytes from the data
            byte[] bmpBytes = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, bmpBytes, 0, bmpBytes.Length);

            // We unlock the bits we accessed
            b.UnlockBits(data);

            return bmpBytes;
        }

        // Returns TRUE if the bitmaps are the same size and contain the exact same data, FALSE otherwise.
        public bool AreBitmapEquals(Bitmap a, Bitmap b)
        {
            // If bitmaps aren't the same size, they are not equal
            if (!a.Size.Equals(b.Size))
                return false;

            // We get each bitmap's bytes
            byte[] aData = GetBitmapBytes(a);
            byte[] bData = GetBitmapBytes(b);

            // If the bitmaps don't contain the same number of bytes, they are not equal
            if (aData.Length != bData.Length)
                return false;

            // Storing length to avoid accessing aData.Length at each iteration
            // (this probably doesn't happen nowadays #GoodCompilers)
            int length = aData.Length;

            // If we encounter two bytes that aren't equal, the bitmaps are not equal
            for (int i = 0; i < length; i++)
            {
                if (aData[i] != bData[i])
                    return false;
            }

            return true;
        }

        // Returns TRUE if both byte arrays are the same length and
        // contain the same values, FALSE otherwise.
        public bool AreBytesEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int length = a.Length;

            for (int i = 0; i < length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        // We test the average black and white filter
        [TestMethod]
        public void ImageFilters_BlackWhite_Average()
        {
            Bitmap original = new Bitmap("./16777216colors.png");
            Bitmap expected = (Bitmap)original.Clone();

            byte[] bytes = GetBitmapBytes(expected);
            int a, r, g, b, avg;

            for (int i = 0; i < bytes.Length; i += 4)
            {
                r = i;
                g = i + 1;
                b = i + 2;
                a = i + 3;

                avg = (bytes[r] + bytes[g] + bytes[b]) / 3;

                bytes[r] = (byte)avg;
                bytes[g] = (byte)avg;
                bytes[b] = (byte)avg;
            }

            SetBitmapBytes(expected, bytes);

            Bitmap actual = ImageEDFilter.ImageFilters.BlackWhite(original);

            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }

        // We test if a Bitmap too wide copied to a square area has the right dimensions
        [TestMethod]
        public void ImageEDFilter_ExtBitmap_CopyToSquareCanvas_TooWide()
        {
            int imgWidth = 857;
            int imgHeight = 251;
            int squareSide = 409;

            Bitmap bmp = new Bitmap(imgWidth, imgHeight);

            int expectedWidth = squareSide;
            int expectedHeight = (int)(((float)squareSide / imgWidth) * imgHeight);

            bmp = ImageEDFilter.ExtBitmap.CopyToSquareCanvas(bmp, squareSide);

            Assert.AreEqual(expectedWidth, bmp.Size.Width);
            Assert.AreEqual(expectedHeight, bmp.Size.Height);
        }

        // We test if a Bitmap too high copied to a square area has the right dimensions
        [TestMethod]
        public void ImageEDFilter_ExtBitmap_CopyToSquareCanvas_TooHigh()
        {
            int imgWidth = 227;
            int imgHeight = 907;
            int squareSide = 367;

            Bitmap bmp = new Bitmap(imgWidth, imgHeight);

            int expectedWidth = (int)(((float)squareSide / imgHeight) * imgWidth);
            int expectedHeight = squareSide;

            bmp = ImageEDFilter.ExtBitmap.CopyToSquareCanvas(bmp, squareSide);

            Assert.AreEqual(expectedWidth, bmp.Size.Width);
            Assert.AreEqual(expectedHeight, bmp.Size.Height);
        }

        // We test if an already squared Bitmap is resized to the right dimensions in
        // the squared area
        [TestMethod]
        public void ImageEDFilter_ExtBitmap_CopyToSquareCanvas_AlreadySquared()
        {
            int imgSide = 773;
            int squareSide = 367;

            Bitmap bmp = new Bitmap(imgSide, imgSide);

            int expectedWidth = squareSide;
            int expectedHeight = squareSide;

            bmp = ImageEDFilter.ExtBitmap.CopyToSquareCanvas(bmp, squareSide);

            Assert.AreEqual(expectedWidth, bmp.Size.Width);
            Assert.AreEqual(expectedHeight, bmp.Size.Height);
        }

        // We test the simple convolution computation with all values in range
        [TestMethod]
        public void TestImageED_ExtBitmap_SimpleConvolution_InRange()
        {
            // We use a test byte array and a test matrix
            // (no Bitmap object involved)

            // 3x3 image
            byte[] bytes = {
                2,3,5,255, 7,11,13,255, 17,19,23,255,
                29,31,37,255, 89,101,103,255, 53,59,61,255,
               67,71,73,255, 79,83,89,255, 41,43,47,255
            };

            double[,] matrix = new double[,] {
                {-11, -19, -17},
                {-13, 23, -7},
                {-5, -3, -2}
            };

            // The outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,
                        // Expecting 201
                        (-11*2) + (-19*7) + (-17*17)
                        + (-13*29) + (23*89) + (-7*53)
                        + (-5*67) + (-3*79) + (-2*41),

                        // Expecting 252
                        (-11*3) + (-19*11) + (-17*19)
                        + (-13*31) + (23*101) + (-7*59)
                        + (-5*71) + (-3*83) + (-2*43),

                        // Expecting 42
                        (-11*5) + (-19*13) + (-17*23)
                        + (-13*37) + (23*103) + (-7*61)
                        + (-5*73) + (-3*89) + (-2*47),

                        255,

                                      0,0,0,0,
                0,0,0,0, 0,0,0,0, 0,0,0,0
            };

            byte[] actualBytes = ImageEDFilter.ExtBitmap.SimpleConvolution(bytes, 3, 3, 3 * 4, matrix);

            Assert.IsTrue(AreBytesEqual(expectedBytes, actualBytes));
        }

        // Simple convolution with computed values above range and set to 255
        [TestMethod]
        public void TestImageED_ExtBitmap_SimpleConvolution_AboveRange()
        {
            // 3x3 image
            byte[] bytes = {
                2,3,5,255, 7,11,13,255, 17,19,23,255,
                29,31,37,255, 233,239,241,255, 53,59,61,255,
               67,71,73,255, 79,83,89,255, 41,43,47,255
            };

            double[,] matrix = new double[,] {
                {-11, -19, -17},
                {-13, 23, -7},
                {-5, -3, -2}
            };

            // The outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,
                        
                        // All channels should be above range
                        255,255,255,255,

                                  0,0,0,0,
                0,0,0,0, 0,0,0,0, 0,0,0,0
            };

            byte[] actualBytes = ImageEDFilter.ExtBitmap.SimpleConvolution(bytes, 3, 3, 3 * 4, matrix);

            Assert.IsTrue(AreBytesEqual(expectedBytes, actualBytes));
        }

        // Simple convolution with computed values below range and set to 0
        [TestMethod]
        public void TestImageED_ExtBitmap_SimpleConvolution_BelowRange()
        {
            // 3x3 image
            byte[] bytes = {
                2,3,5,255, 7,11,13,255, 17,19,23,255,
                29,31,37,255, 2,3,5,255, 53,59,61,255,
               67,71,73,255, 79,83,89,255, 41,43,47,255
            };

            double[,] matrix = new double[,] {
                {-11, -19, -17},
                {-13, 23, -7},
                {-5, -3, -2}
            };

            // The outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,

                        // All channels should be below range
                        0,0,0,255,

                                  0,0,0,0,
                0,0,0,0, 0,0,0,0, 0,0,0,0
            };

            byte[] actualBytes = ImageEDFilter.ExtBitmap.SimpleConvolution(bytes, 3, 3, 3 * 4, matrix);

            Assert.IsTrue(AreBytesEqual(expectedBytes, actualBytes));
        }

        // We verify that the ApplyConvolutionFunc leaves colors untouched if grayscale = false
        [TestMethod]
        public void TestImageED_ApplyConvolutionFunc_Color()
        {
            // 3x3 image
            byte[] originalBytes = {
                2,3,5,255, 7,11,13,255, 17,19,23,255,
                29,31,37,255, 39,41,43,255, 53,59,61,255,
               67,71,73,255, 79,83,89,255, 41,43,47,255
            };

            // This matrix does nothing to the pixels it applies to
            double[,] noopMatrix = new double[,] {
                {0, 0, 0},
                {0, 1, 0},
                {0, 0, 0}
            };

            // The outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,

                        // All channels should be untouched
                        39,41,43,255,

                                  0,0,0,0,
                0,0,0,0, 0,0,0,0, 0,0,0,0
            };

            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes( original, originalBytes );

            Bitmap expected = new Bitmap(3, 3);
            SetBitmapBytes( expected, expectedBytes );

            byte[] NoopConvolution( byte[] arrBytes, int w, int h, int stride )
            {
                return ImageEDFilter.ExtBitmap.SimpleConvolution(arrBytes, 3, 3, 3 * 4, noopMatrix );
            }

            Bitmap actual = ImageEDFilter.ExtBitmap.ApplyConvolutionFunc(original, NoopConvolution, false);

            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }

        // We verify that the ApplyConvolutionFunc properly computes
        // the perceived luminance value if grayscale = true
        [TestMethod]
        public void TestImageED_ApplyConvolutionFunc_PerceivedLuminance()
        {
            // 3x3 image (note that the average of RGB channels is equal to 68.666...)
            byte[] originalBytes = {
                2,3,5,255, 7,11,13,255, 17,19,23,255,
                29,31,37,255, 34,154,18,255, 53,59,61,255,
               67,71,73,255, 79,83,89,255, 41,43,47,255
            };

            // This matrix does nothing to the pixels it applies to
            double[,] noopMatrix = new double[,] {
                {0, 0, 0},
                {0, 1, 0},
                {0, 0, 0}
            };

            // The outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,

                        // The center pixel should be grayscaled using
                        // perceived luminance RGB coefficients (0.11, 0.59, 0.30).
                        // With numbers (34; 154; 18) the perceived luminance
                        // gives us (100; 100; 100).
                        100,100,100,255,

                                  0,0,0,0,
                0,0,0,0, 0,0,0,0, 0,0,0,0
            };

            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, originalBytes);

            Bitmap expected = new Bitmap(3, 3);
            SetBitmapBytes(expected, expectedBytes);

            byte[] NoopConvolution(byte[] arrBytes, int w, int h, int stride)
            {
                return ImageEDFilter.ExtBitmap.SimpleConvolution(arrBytes, 3, 3, 3 * 4, noopMatrix);
            }

            Bitmap actual = ImageEDFilter.ExtBitmap.ApplyConvolutionFunc(original, NoopConvolution, true);

            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }




        [TestMethod]
        public void TestApplyFilterSwap()
        {

            Bitmap original = new Bitmap("./landscape.png");

            // We could have done instead : new Bitmap(original);
            // Bitmap filtered = (Bitmap) original.Clone();
            Bitmap filtered = ImageFilters.ApplyFilterSwap(original);


            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {

                    bool greenToRed = (original.GetPixel(i, j).G == filtered.GetPixel(i, j).R);
                    bool blueToGreen = (original.GetPixel(i, j).B == filtered.GetPixel(i, j).G);
                    bool redToBlue = (original.GetPixel(i, j).R == filtered.GetPixel(i, j).B);

                    // If the swap is not done correctly we stop the test
                    Assert.IsTrue(greenToRed && blueToGreen && redToBlue);
                }
            }
        }
    }
}