using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageEdgeDetection;
using ImageEDFilter;
using System.IO;

namespace EDUnitTest
{
    [TestClass()]
    public class UnitTesting
    {
        public Bitmap RgbBytesToBitmap( byte[] data, int width, int height )
        {
            return null;
        }

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

            return AreBytesEqual(aData, bData);
        }

        // Returns TRUE if both byte arrays are the same length and
        // contain the same values, FALSE otherwise.
        public bool AreBytesEqual(byte[] a, byte[] b)
        {
            // If the bitmaps don't contain the same number of bytes, they are not equal
            if (a.Length != b.Length)
                return false;

            // Storing length to avoid accessing aData.Length at each iteration
            // (this probably doesn't happen nowadays #GoodCompilers)
            int length = a.Length;

            // If we encounter two bytes that aren't equal, the bitmaps are not equal
            for (int i = 0; i < length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }



        [TestMethod()]
        public void BitmapEditor_BitmapEditor_NullParameter()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            BitmapEditor editor;

            try
            {
                editor = new BitmapEditor(null, null);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }

            try
            {
                editor = new BitmapEditor(bitmapFileIO, null);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }

            try
            {
                editor = new BitmapEditor(null, view);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }

            try
            {
                editor = new BitmapEditor(bitmapFileIO, view);
            }
            catch
            {
                // The test simply failed if we arrived in the catch
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void BitmapFilter_Apply_ApplyMethod()
        {
            //TODO received in order 
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var filterEd = Substitute.For<IBitmapFilter>();
            var filterPx = Substitute.For<IBitmapFilter>();

            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // Received vérifie que les arguments passés à la fonction sont les bons (valeur exacte, type demandé, ...)
            // Returns vérifie 

            Bitmap bmp = new Bitmap(100, 100);
            //bitmapFileIO.ReadBitmap(Arg.Any<string>()).Returns(bmp);
            //editor.ReadFile("");
            editor.SetBitmap(bmp);

            filterPx.Received(0).Apply(Arg.Any<Bitmap>());
            filterEd.Received(0).Apply(Arg.Any<Bitmap>());
            filterPx.ClearReceivedCalls();

            editor.SetPixelFilter(filterPx);

            filterPx.Received(1).Apply(Arg.Any<Bitmap>());
            filterEd.Received(0).Apply(Arg.Any<Bitmap>());
            filterPx.ClearReceivedCalls();

            editor.SetEdgeFilter(filterEd);

            filterPx.Received(1).Apply(Arg.Any<Bitmap>());
            filterEd.Received(1).Apply(Arg.Any<Bitmap>());
        }



        [TestMethod()]
        public void BitmapEditor_GetPixelFilters_ArrayNotEmpty()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            Assert.IsInstanceOfType(editor.GetPixelFilters(), typeof(IBitmapFilter[]));
            Assert.AreNotEqual(0, editor.GetPixelFilters().Length);
        }

        [TestMethod()]
        public void BitmapEditor_GetEdgeFilters_ArrayNotEmpty()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            Assert.IsInstanceOfType(editor.GetEdgeFilters(), typeof(IBitmapFilter[]));
            Assert.AreNotEqual(0, editor.GetEdgeFilters().Length);
        }

        [TestMethod()]
        public void BitmapEditor_HasImage_ReturnTrueOrFalse()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            Assert.IsFalse(editor.HasImage());

            editor.SetBitmap(new Bitmap(100, 100));
            Assert.IsTrue(editor.HasImage());

            try
            {
                editor.SetBitmap(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }
        }

        [TestMethod()]
        public void BitmapEditor_HasPixelFilter_ReturnTrueOrFalse()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var pixelFilter = Substitute.For<IBitmapFilter>();
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            Assert.IsFalse(editor.HasPixelFilter());

            editor.SetPixelFilter(pixelFilter);
            Assert.IsTrue(editor.HasPixelFilter());

            editor.SetPixelFilter(null);
            Assert.IsFalse(editor.HasPixelFilter());
        }

        [TestMethod()]
        public void BitmapEditor_HasEdgeFilter_ReturnTrueOrFalse()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var edgeFilter = Substitute.For<IBitmapFilter>();
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            Assert.IsFalse(editor.HasEdgeFilter());

            editor.SetEdgeFilter(edgeFilter);
            Assert.IsTrue(editor.HasEdgeFilter());

            editor.SetEdgeFilter(null);
            Assert.IsFalse(editor.HasEdgeFilter());
        }

        [TestMethod()]
        public void BitmapEditor_SetPixelFilter_ReturnTrueOrFalse()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var pixelFilter = Substitute.For<IBitmapFilter>();
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            Assert.IsFalse(editor.HasPixelFilter());

            editor.SetPixelFilter(pixelFilter);
            Assert.IsTrue(editor.HasPixelFilter());
        }

        [TestMethod()]
        public void BitmapEditor_SetEdgeFilter_ReturnTrueOrFalse()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var edgeFilter = Substitute.For<IBitmapFilter>();
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            editor.SetEdgeFilter(edgeFilter);
            Assert.IsTrue(editor.HasEdgeFilter());
        }

        [TestMethod()]
        public void BitmapEditor_GetBitmap_ReturnBitmap()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);
            editor.SetBitmap(new Bitmap(100, 100));
            Assert.IsInstanceOfType(editor.GetBitmap(), typeof(Bitmap));
        }

        [TestMethod()]
        public void BitmapEditor_SetBitmap_CheckBitmapExist()
        {
            // TODO receive in order 
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var ibitmapeditor = Substitute.For < IBitmapEditor>();
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            try
            {
                editor.SetBitmap(new Bitmap(100, 100));
                Assert.IsTrue(editor.HasImage());
                /*
                Received.InOrder(() =>
                {
                    ibitmapeditor.ApplyOnPreview();
                    ibitmapeditor.CheckEditorState();
                });
                */
            }
            catch
            {
                Assert.Fail();
            }

            
        }

        [TestMethod()]
        public void BitmapEditor_CheckEditorState_VerifyStates()
        {
            bool controlsEnabled = true;
            BitmapEditorStatus status = BitmapEditorStatus.OK;

            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var filterEd = Substitute.For<IBitmapFilter>();
            var filterPx = Substitute.For<IBitmapFilter>();

            // Tell Apply method to return a Bitmap instead of null
            filterEd.Apply(Arg.Any<Bitmap>()).Returns(new Bitmap(100,100));
            filterPx.Apply(Arg.Any<Bitmap>()).Returns(new Bitmap(100, 100));

            view.SetControlsEnabled(Arg.Do<bool>(b => controlsEnabled = b));
            view.SetStatusMessage(Arg.Do<BitmapEditorStatus>(s => status = s), Arg.Any<string>() );

            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            Assert.IsFalse(controlsEnabled);
            Assert.AreEqual(BitmapEditorStatus.WARNING, status);

            editor.SetBitmap(new Bitmap(100,100));

            // asserts
            Assert.IsTrue(controlsEnabled);
            Assert.AreEqual(BitmapEditorStatus.WARNING, status);

            editor.SetPixelFilter(filterPx);

            // asserts
            Assert.IsTrue(controlsEnabled);
            Assert.AreEqual(BitmapEditorStatus.WARNING, status);

            editor.SetEdgeFilter(filterEd);

            // asserts
            Assert.IsTrue(controlsEnabled);
            Assert.AreEqual(BitmapEditorStatus.OK, status);
        }

        [TestMethod()]
        public void BitmapEditor_ApplyOnPreview_ApplyFilter()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);
            editor.SetBitmap(new Bitmap(100,100));
            editor.ApplyOnPreview();
            view.Received().SetPreviewBitmap(Arg.Any<Bitmap>());
        }

        [TestMethod()]
        public void BitmapEditor_ReadFile_CorrectAndWrongReading()
        {
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);
            view.Received(1).SetControlsEnabled(Arg.Any<bool>());
            view.Received(1).SetStatusMessage(Arg.Any<BitmapEditorStatus>(), Arg.Any<string>());

            Bitmap bmp = new Bitmap(100, 100);
            bitmapFileIO.ReadBitmap(@"C:\valid\file\path").Returns(bmp);

            editor.ReadFile(@"C:\valid\file\path");
            bitmapFileIO.Received(1).ReadBitmap(Arg.Any<string>());
            view.Received(1).SetPreviewBitmap(Arg.Any<Bitmap>());
            view.Received(2).SetControlsEnabled(Arg.Any<bool>());
            view.Received(2).SetStatusMessage(Arg.Any<BitmapEditorStatus>(), Arg.Any<string>());

            // If the file path is wrong
            bitmapFileIO.ReadBitmap(@"C:\invalid\file\path").Returns(x => { throw new FileNotFoundException(); });
        }



















        // We test if a Bitmap too wide copied to a square area has the right dimensions
        [TestMethod]
        public void BitmapEditor_CreatePreview_TooWide()
        {
            int imgWidth = 857;
            int imgHeight = 251;
            int squareSide = 409;

            int expectedWidth = squareSide;
            int expectedHeight = (int)(((float)squareSide / imgWidth) * imgHeight);

            Bitmap original = new Bitmap(imgWidth, imgHeight);
            Bitmap preview = null;

            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            bitmapFileIO.ReadBitmap(Arg.Any<string>()).Returns(original);

            var view = Substitute.For<IBitmapViewer>();
            view.SetPreviewBitmap(Arg.Do<Bitmap>(b => preview = b));

            var editor = new BitmapEditor(bitmapFileIO, view);
            editor.ReadFile(@"C:\valid\file\path");

            if (preview != null)
            {
                Assert.AreEqual(expectedWidth, preview.Size.Width);
                Assert.AreEqual(expectedHeight, preview.Size.Height);
            }

            else
            {
                Assert.Fail();
            }
        }
        
        // We test if a Bitmap too high copied to a square area has the right dimensions
        [TestMethod]
        public void ExtBitmap_CopyToSquareCanvas_TooHigh()
        {
            int imgWidth = 227;
            int imgHeight = 907;
            int squareSide = 367;

            int expectedWidth = (int)(((float)squareSide / imgHeight) * imgWidth);
            int expectedHeight = squareSide;

            Bitmap original = new Bitmap(imgWidth, imgHeight);
            Bitmap preview = null;

            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            bitmapFileIO.ReadBitmap(Arg.Any<string>()).Returns(original);

            var view = Substitute.For<IBitmapViewer>();
            view.SetPreviewBitmap(Arg.Do<Bitmap>(b => preview = b));

            var editor = new BitmapEditor(bitmapFileIO, view);
            editor.ReadFile(@"C:\valid\file\path");

            if (preview != null)
            {
                Assert.AreEqual(expectedWidth, preview.Size.Width);
                Assert.AreEqual(expectedHeight, preview.Size.Height);
            }

            else
            {
                Assert.Fail();
            }
        }
        
        // We test if an already squared Bitmap is resized to the right dimensions in
        // the squared area
        [TestMethod]
        public void ExtBitmap_CopyToSquareCanvas_AlreadySquared()
        {
            int imgSide = 773;
            int squareSide = 367;

            int expectedWidth = squareSide;
            int expectedHeight = squareSide;

            Bitmap original = new Bitmap(imgSide, imgSide);
            Bitmap preview = null;

            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            bitmapFileIO.ReadBitmap(Arg.Any<string>()).Returns(original);

            var view = Substitute.For<IBitmapViewer>();
            view.SetPreviewBitmap(Arg.Do<Bitmap>(b => preview = b));
            view.GetPreviewSquareSize().Returns(squareSide);

            var editor = new BitmapEditor(bitmapFileIO, view);
            editor.ReadFile(@"C:\valid\file\path");

            if (preview != null)
            {
                Assert.AreEqual(expectedWidth, preview.Size.Width);
                Assert.AreEqual(expectedHeight, preview.Size.Height);
            }

            else
            {
                Assert.Fail();
            }
        }
        
        // We test the simple convolution computation with all values in range
        [TestMethod]
        public void MatrixEdgeFilter_InRange()
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

            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes( original, bytes );

            MatrixEdgeFilter mef = new MatrixEdgeFilter("", matrix, false, 1.0, 0);
            byte[] actualBytes = GetBitmapBytes( mef.Apply(original) );

            Assert.IsTrue(AreBytesEqual(expectedBytes, actualBytes));
        }
        
        // Simple convolution with computed values above range and set to 255
        [TestMethod]
        public void ExtBitmap_SimpleConvolution_AboveRange()
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

            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, bytes);

            MatrixEdgeFilter mef = new MatrixEdgeFilter("", matrix, false, 1.0, 0);
            byte[] actualBytes = GetBitmapBytes(mef.Apply(original));

            Assert.IsTrue(AreBytesEqual(expectedBytes, actualBytes));
        }

        // Simple convolution with computed values below range and set to 0
        [TestMethod]
        public void ExtBitmap_SimpleConvolution_BelowRange()
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

            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, bytes);

            MatrixEdgeFilter mef = new MatrixEdgeFilter("", matrix, false, 1.0, 0);
            byte[] actualBytes = GetBitmapBytes(mef.Apply(original));

            Assert.IsTrue(AreBytesEqual(expectedBytes, actualBytes));
        }

        // We verify that the ApplyConvolutionFunc properly computes pixel values
        [TestMethod]
        public void ExtBitmap_ApplyConvolutionFunc_Color()
        {
            // 3x3 image
            byte[] originalBytes = {
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

            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, originalBytes);

            Bitmap expected = new Bitmap(3, 3);
            SetBitmapBytes(expected, expectedBytes);

            MatrixEdgeFilter mef = new MatrixEdgeFilter("", matrix, false, 1.0, 0);
            Bitmap actual = mef.Apply(original);

            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }

        // We verify that the ApplyConvolutionFunc leaves colors untouched if grayscale = false
        [TestMethod]
        public void ExtBitmap_ApplyConvolutionFunc_Noop()
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
            SetBitmapBytes(original, originalBytes);

            Bitmap expected = new Bitmap(3, 3);
            SetBitmapBytes(expected, expectedBytes);

            MatrixEdgeFilter mef = new MatrixEdgeFilter("", noopMatrix, false, 1.0, 0);
            Bitmap actual = mef.Apply(original);

            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }

        // We verify that the ApplyConvolutionFunc properly computes
        // the perceived luminance value if grayscale = true
        [TestMethod]
        public void ExtBitmap_ApplyConvolutionFunc_PerceivedLuminance()
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

            MatrixEdgeFilter mef = new MatrixEdgeFilter("", noopMatrix, false, 1.0, 0);
            Bitmap actual = mef.Apply(original);

            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }


        [TestMethod]
        public void ImageFilters_ApplyFilterMega_ColorCheck()
        {
            Color filterColor = Color.FromArgb(19, 29, 37);

            float min = 0.35f;
            float max = 0.65f;
            Color originalColor, expectedColor, actualColor;

            Bitmap original = new Bitmap("./landscape.png");

            ThresoldFilter tf = new ThresoldFilter("", min, max, filterColor);
            Bitmap filtered = tf.Apply(original);

            
            float computeLuminance(Color c)
            {
                float luminance = 0.0f;

                // Computing weighted arithmetic mean of red, green and blue channels
                luminance = c.R / 255.0f * 0.11f;
                luminance += c.G / 255.0f * 0.59f;
                luminance += c.B / 255.0f * 0.3f;

                return luminance;
            }

            float computedLuminance;

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    originalColor = original.GetPixel(x, y);
                    actualColor = filtered.GetPixel(x, y);

                    computedLuminance = computeLuminance(originalColor);

                    if (computedLuminance >= min && computedLuminance <= max)
                    {
                        expectedColor = Color.White;
                    }
                    else
                    {
                        expectedColor = filterColor;
                    }

                    Assert.AreEqual(expectedColor.ToArgb(), actualColor.ToArgb());
                }
            }
        }


        // We test the average black and white filter
        [TestMethod]
        public void ImageFilters_BlackWhite_Average()
        {
            Bitmap original = new Bitmap("./landscape.png");
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

            BlackWhiteFilter bwf = new BlackWhiteFilter("");
            Bitmap actual = bwf.Apply(original);

            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }
        
    }
}
