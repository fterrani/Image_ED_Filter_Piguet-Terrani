using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageEdgeDetection;
using ImageEDFilter;
using System.IO;
using System.Windows.Forms;

namespace EDUnitTest
{
    [TestClass()]
    public class UnitTesting
    {
        // Sets the bytes of bitmap b to the ones contained in pixels (4-byte BGRA format)
        public void SetBitmapBytes(Bitmap b, byte[] pixels)
        {
            // Preparing the Bitmap for writing
            BitmapData data = b.LockBits(
                new Rectangle(new Point(0, 0), b.Size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
            );

            int expectedLength = data.Stride * data.Height;
            bool correctLength = (pixels.Length == expectedLength);

            // Checking if the pixels array has the expected length
            if (correctLength)
            {
                // We extract the bitmap bytes from the data
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

        // Returns a byte array containing the bitmap's data in 4-byte BGRA format
        public byte[] GetBitmapBytes(Bitmap b)
        {
            // Preparing the bitmap for reading
            BitmapData data = b.LockBits(
                new Rectangle(new Point(0, 0), b.Size),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            // We extract the bitmap bytes from the data (even if Format32bppArgb is used, data are returned in BGRA format)
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
        // We try creating a BitmapEditor with null paramters
        public void BitmapEditor_constructor_NullAndValidParameter()
        {
            // Preparing substitutes to build the editor
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            BitmapEditor editor;

            // Creating with only null bitmapFileIO and view
            try
            {
                editor = new BitmapEditor(null, null);

                // If no exception is thrown, the test fails
                Assert.Fail();
            }
            catch (Exception e)
            {
                // The exception should be of the right type
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }



            // Creating with null view
            try
            {
                editor = new BitmapEditor(bitmapFileIO, null);

                // If no exception is thrown, the test fails
                Assert.Fail();
            }
            catch (Exception e)
            {
                // The exception should be of the right type
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }



            // Creating with a null bitmapFileIO
            try
            {
                editor = new BitmapEditor(null, view);

                // If no exception is thrown, the test fails
                Assert.Fail();
            }
            catch (Exception e)
            {
                // The exception should be of the right type
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }

            // Creating with valid parameters
            try
            {
                editor = new BitmapEditor(bitmapFileIO, view);
            }
            catch
            {
                // The test fails if an exception is thrown
                Assert.Fail();
            }
        }

        [TestMethod()]
        // We check if the editor applies the filter in the right order
        public void BitmapEditor_ApplyFilters_FiltersCalledInRightOrder()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var filterEd = Substitute.For<IBitmapFilter>();
            var filterPx = Substitute.For<IBitmapFilter>();

            // Substitutes should return some values for the editor to behave properly
            view.GetPreviewSquareSize().Returns(50);
            filterEd.Apply(Arg.Any<Bitmap>()).Returns(new Bitmap(100, 100));
            filterPx.Apply(Arg.Any<Bitmap>()).Returns(new Bitmap(100, 100));

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // Setting the test bitmap
            Bitmap bmp = new Bitmap(100, 100);
            editor.SetBitmap(bmp);

            // At this point, no filter should have been called yet
            filterPx.Received(0).Apply(Arg.Any<Bitmap>());
            filterEd.Received(0).Apply(Arg.Any<Bitmap>());
            filterPx.ClearReceivedCalls();
            filterEd.ClearReceivedCalls();

            // The editor now only has a pixel filter
            editor.SetPixelFilter(filterPx);

            // The pixel filter should be called but not the edge filter (not assigned yet)
            filterPx.Received(1).Apply(Arg.Any<Bitmap>());
            filterEd.Received(0).Apply(Arg.Any<Bitmap>());
            editor.SetPixelFilter(null);
            filterPx.ClearReceivedCalls();
            filterEd.ClearReceivedCalls();


            // The editor now only has a edge filter
            editor.SetEdgeFilter(filterEd);

            // The edge filter should be called but not the pixel filter (it was unassigned)
            filterPx.Received(0).Apply(Arg.Any<Bitmap>());
            filterEd.Received(1).Apply(Arg.Any<Bitmap>());
            filterPx.ClearReceivedCalls();
            filterEd.ClearReceivedCalls();

            // The editor now has two filters: a pixel filter and an edge filter
            editor.SetPixelFilter(filterPx);

            // Both filters should be called once
            filterPx.Received(1).Apply(Arg.Any<Bitmap>());
            filterEd.Received(1).Apply(Arg.Any<Bitmap>());

            // The pixel filter should be called first, and THEN the edge filter
            Received.InOrder(() =>
            {
                filterPx.Apply(Arg.Any<Bitmap>());
                filterEd.Apply(Arg.Any<Bitmap>());
            });
        }



        [TestMethod()]
        // We check that the editor returns a non-empty array of pixel filters
        public void BitmapEditor_GetPixelFilters_ArrayNotEmpty()
        {
            // Preparing substitutes to create the editor
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // We get the array and check that it is not empty
            IBitmapFilter[] filters = editor.GetPixelFilters();
            Assert.IsTrue( filters.Length > 0 );
        }

        [TestMethod()]
        // We check that the editor returns a non-empty array of edge filters
        public void BitmapEditor_GetEdgeFilters_ArrayNotEmpty()
        {
            // Preparing substitutes to create the editor
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // We get the array and check that it is not empty
            IBitmapFilter[] filters = editor.GetEdgeFilters();
            Assert.IsTrue(filters.Length > 0);
        }

        [TestMethod()]
        // We check that the HasImage method returns appropriate results when changing the editor's state
        public void BitmapEditor_HasImage_ReturnTrueOrFalse()
        {
            // Preparing substitutes to create the editor
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            // The view should return a preview square size to make the editor behave properly
            view.GetPreviewSquareSize().Returns(50);

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // When the editor was just created, it should have no image yet
            Assert.IsFalse(editor.HasImage());

            // Once we add a bitmap to the editor, it should have an image
            editor.SetBitmap(new Bitmap(100, 100));
            Assert.IsTrue(editor.HasImage());
        }

        [TestMethod()]
        // We check that the HasPixelFilter method returns appropriate results when changing the editor's state
        public void BitmapEditor_HasPixelFilter_ReturnTrueOrFalse()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var pixelFilter = Substitute.For<IBitmapFilter>();

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // By default the editor should have no pixel filter
            Assert.IsFalse(editor.HasPixelFilter());

            // Once we set the filter, HasPixelFilter() should return true
            editor.SetPixelFilter(pixelFilter);
            Assert.IsTrue(editor.HasPixelFilter());

            // If we set the filter to null, HasPixelFilter() should return false
            editor.SetPixelFilter(null);
            Assert.IsFalse(editor.HasPixelFilter());
        }

        [TestMethod()]
        // We check that the HasEdgeFilter method returns appropriate results when changing the editor's state
        public void BitmapEditor_HasEdgeFilter_ReturnTrueOrFalse()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var edgeFilter = Substitute.For<IBitmapFilter>();

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // By default the editor should have no edge filter
            Assert.IsFalse(editor.HasEdgeFilter());

            // Once we set the filter, HasEdgeFilter() should return true
            editor.SetEdgeFilter(edgeFilter);
            Assert.IsTrue(editor.HasEdgeFilter());

            // If we set the filter to null, HasEdgeFilter() should return false
            editor.SetEdgeFilter(null);
            Assert.IsFalse(editor.HasEdgeFilter());
        }

        [TestMethod()]
        // We check that the SetPixelFilter method has an effect on the editor
        public void BitmapEditor_SetPixelFilter_FilterDetectedAndApplied()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var pixelFilter = Substitute.For<IBitmapFilter>();

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // Setting the pixel filter
            editor.SetPixelFilter(pixelFilter);

            // The editor should now have a pixel filter and it should have been called once
            Assert.IsTrue(editor.HasPixelFilter());
            pixelFilter.Received(1).Apply(Arg.Any<Bitmap>());
        }

        [TestMethod()]
        // We check that the SetEdgeFilter method has an effect on the editor
        public void BitmapEditor_SetEdgeFilter_FilterDetectedAndApplied()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var edgeFilter = Substitute.For<IBitmapFilter>();

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // Setting the edge filter
            editor.SetEdgeFilter(edgeFilter);

            // The editor should now have an edge filter and it should have been called once
            Assert.IsTrue(editor.HasEdgeFilter());
            edgeFilter.Received(1).Apply(Arg.Any<Bitmap>());
        }

        [TestMethod()]
        // We check that the GetBitmap method returns a bitmap
        public void BitmapEditor_GetBitmap_ReturnBitmap()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            // The view should return a preview square size to make the editor behave properly
            view.GetPreviewSquareSize().Returns(50);

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // If no bitmap was set yet, GeBitmap() should return null
            Assert.IsNull( editor.GetBitmap() );

            // Once a bitmap was set, GetBitmap() should return a Bitmap object (not the same Bitmap instance)
            editor.SetBitmap(new Bitmap(100, 100));
            Assert.IsInstanceOfType(editor.GetBitmap(), typeof(Bitmap));
        }

        [TestMethod()]
        // We check that the view receives the right calls
        public void BitmapEditor_SetBitmap_CallsToTheView()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            // The view should return a preview square size to make the editor behave properly
            view.GetPreviewSquareSize().Returns(50);

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            try
            {
                // After setting a bitmap, the editor should have an image
                editor.SetBitmap(new Bitmap(100, 100));
                Assert.IsTrue(editor.HasImage());
            }

            catch (Exception e)
            {
                // If an exception is thrown, the test fails
                Assert.Fail();
            }

            // The view should have received the following calls in this order
            Received.InOrder(() =>
            {
                // Controls disabled by default, warning status since there is no image
                view.SetControlsEnabled(false);
                view.SetStatusMessage(BitmapEditorStatus.WARNING, Arg.Any<string>());

                // Once an image was set, a preview should be sent to the view, controls should be enabled, a new warning status should be sent
                view.SetPreviewBitmap(Arg.Any<Bitmap>());
                view.SetControlsEnabled(true);
                view.SetStatusMessage(BitmapEditorStatus.WARNING, Arg.Any<string>());
            });
        }

        [TestMethod()]
        // We check that an exception is thrown if the editor's bitmap is set to null
        public void BitmapEditor_SetBitmap_NullBitmapThrowsException()
        {
            // Preparing substitutes to create the editor
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            // The view should return a preview square size to make the editor behave properly
            view.GetPreviewSquareSize().Returns(50);

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // If we try to set a null bitmap, an exception should be thrown
            try
            {
                editor.SetBitmap(null);

                // The test fails if no exception was thrown
                Assert.Fail();
            }
            catch (Exception e)
            {
                // The exception should be of the right type
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }
        }

        [TestMethod()]
        // We check that the editor passes the right state information to the view
        public void BitmapEditor_CheckEditorState_VerifyStates()
        {
            // Two things are tested: the controls activation and the editor's status
            // (messages are not tested but they could be by storing them in variables)
            bool controlsEnabled = true;
            BitmapEditorStatus status = BitmapEditorStatus.OK;

            // Preparing substitutes for testing
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();
            var filterEd = Substitute.For<IBitmapFilter>();
            var filterPx = Substitute.For<IBitmapFilter>();

            // The view should return a preview square size to make the editor behave properly
            view.GetPreviewSquareSize().Returns(50);

            // Filters should return a bitmap to make the editor behave properly
            filterEd.Apply(Arg.Any<Bitmap>()).Returns(new Bitmap(100, 100));
            filterPx.Apply(Arg.Any<Bitmap>()).Returns(new Bitmap(100, 100));

            // When the view methods are called, received parameters are stored in the two variables declared above
            view.SetControlsEnabled(Arg.Do<bool>(b => controlsEnabled = b));
            view.SetStatusMessage(Arg.Do<BitmapEditorStatus>(s => status = s), Arg.Any<string>());

            // Creating an editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // Controls should be disabled and a warning be displayed (no image selected)
            Assert.IsFalse(controlsEnabled);
            Assert.AreEqual(BitmapEditorStatus.WARNING, status);

            // Setting an image
            editor.SetBitmap(new Bitmap(100, 100));

            // Controls should now be enabled and a warning be displayed (no filter selected)
            Assert.IsTrue(controlsEnabled);
            Assert.AreEqual(BitmapEditorStatus.WARNING, status);

            // Setting a pixel filter
            editor.SetPixelFilter(filterPx);

            // Controls should be enabled and a warning be displayed (no edge detection filter selected)
            Assert.IsTrue(controlsEnabled);
            Assert.AreEqual(BitmapEditorStatus.WARNING, status);

            // Setting an edge detection filter
            editor.SetEdgeFilter(filterEd);

            // Controls should be enabled. The editor be in OK status and show a confirmation that the right parameters are selected
            Assert.IsTrue(controlsEnabled);
            Assert.AreEqual(BitmapEditorStatus.OK, status);
        }

        [TestMethod()]
        // We check that the ApplyOnPreview method triggers the sending of a new preview to the view
        public void BitmapEditor_ApplyOnPreview_ApplyFilter()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            // The view should return a preview square size to make the editor to behave properly
            view.GetPreviewSquareSize().Returns(50);

            // Creating the editor and setting an image
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);
            editor.SetBitmap(new Bitmap(100, 100));

            // We reset calls to the view
            view.ClearReceivedCalls();

            // When ApplyOnPreview is called, the view should receive exactly one new preview bitmap
            editor.ApplyOnPreview();
            view.Received(1).SetPreviewBitmap(Arg.Any<Bitmap>());
        }

        [TestMethod()]
        // We check that the readFile method behaves properly with valid and invalid file paths
        public void BitmapEditor_ReadFile_CorrectAndWrongReading()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            // The view should return a preview square size to make the editor to behave properly
            view.GetPreviewSquareSize().Returns(50);

            // Creating the editor
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);

            // Without doing anything more, the view should display a warning that no file was selected.
            // Controls should be disabled
            view.Received(1).SetControlsEnabled(Arg.Any<bool>());
            view.Received(1).SetStatusMessage(Arg.Any<BitmapEditorStatus>(), Arg.Any<string>());

            // Clearing calls to the view and bitmapFileIO
            view.ClearReceivedCalls();
            bitmapFileIO.ClearReceivedCalls();

            // We prepare the bitmapFileIO substitute to return a bitmap if the file path is valid
            Bitmap bmp = new Bitmap(100, 100);
            bitmapFileIO.ReadBitmap(@"C:\valid\file\path").Returns(bmp);

            // Calling the ReadFile method
            editor.ReadFile(@"C:\valid\file\path");

            // If the file was read properly, we should have called ReadBitmap once
            bitmapFileIO.Received(1).ReadBitmap(Arg.Any<string>());

            // The view should have received calls to set its preview, controls and status message
            view.Received(1).SetPreviewBitmap(Arg.Any<Bitmap>());
            view.Received(1).SetControlsEnabled(Arg.Any<bool>());
            view.Received(1).SetStatusMessage(Arg.Any<BitmapEditorStatus>(), Arg.Any<string>());

            // Clearing calls to prepare for the invalid path testing
            view.ClearReceivedCalls();
            bitmapFileIO.ClearReceivedCalls();

            // The bitmapFileIO substitute should now throw an exception if the file path is invalid
            bitmapFileIO.ReadBitmap("INVALID file path").Returns(x => { throw new FileNotFoundException(); });

            try
            {
                // If we try to read an invalid file path, the editor should transfer the exception
                editor.ReadFile("INVALID file path");

                // If it does not, the test fails
                Assert.Fail();
            }
            catch (Exception e)
            {
                // The exception should be of the right type
                Assert.IsInstanceOfType(e, typeof(FileNotFoundException));

                // If we tried reading an invalid path, ReadBitmap should have been called once
                bitmapFileIO.Received(1).ReadBitmap(Arg.Any<string>());

                // ... but the view shouldn't have received any call
                view.Received(0).SetPreviewBitmap(Arg.Any<Bitmap>());
                view.Received(0).SetControlsEnabled(Arg.Any<bool>());
                view.Received(0).SetStatusMessage(Arg.Any<BitmapEditorStatus>(), Arg.Any<string>());
            }
        }

        [TestMethod()]
        // We check that the WriteFile method behaves properly with valid and invalid file paths
        public void BitmapEditor_WriteFile_CorrectAndWrongWriting()
        {
            // Preparing substitutes
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            var view = Substitute.For<IBitmapViewer>();

            // The view should return a preview square size to make the editor to behave properly
            view.GetPreviewSquareSize().Returns(50);

            // Preparing a bitmap to write
            Bitmap bmp = new Bitmap(100, 100);

            // We prepare a substitute filter that will be applied on the bitmap
            IBitmapFilter dummyFilter = Substitute.For<IBitmapFilter>();
            dummyFilter.Apply(Arg.Any<Bitmap>()).Returns(bmp);

            // We create the editor and set its image and filters
            BitmapEditor editor = new BitmapEditor(bitmapFileIO, view);
            editor.SetBitmap(bmp);
            editor.SetPixelFilter(dummyFilter);
            editor.SetEdgeFilter(dummyFilter);

            // Clearing calls to the substitutes
            view.ClearReceivedCalls();
            bitmapFileIO.ClearReceivedCalls();

            try
            {
                // We try writing using a valid file path
                editor.WriteFile(@"C:\valid\file\path");
            }

            catch (Exception e)
            {
                // If an exception is thrown, the test fails
                Assert.Fail();
            }

            // If the file path is valid, WriteBitmap should have been called once
            bitmapFileIO.Received(1).WriteBitmap(bmp, @"C:\valid\file\path");

            // ... but the view should not have been called at all
            view.Received(0).SetPreviewBitmap(Arg.Any<Bitmap>());
            view.Received(0).SetControlsEnabled(Arg.Any<bool>());
            view.Received(0).SetStatusMessage(Arg.Any<BitmapEditorStatus>(), Arg.Any<string>());

            // Clearing calls to substitutes
            view.ClearReceivedCalls();
            bitmapFileIO.ClearReceivedCalls();

            // If the file path is wrong, we ask the substitute to throw an exception
            bitmapFileIO.WriteBitmap(bmp, "INVALID file path").Returns(x => { throw new FileNotFoundException(); });

            try
            {
                // We try writing to an invalid path, the editor should transfer the exception
                editor.WriteFile("INVALID file path");

                // If if does not, the test fails
                Assert.Fail();
            }
            catch (Exception e)
            {
                // The exception should be of the right type
                Assert.IsInstanceOfType(e, typeof(FileNotFoundException));

                // The WriteBitmap method should have been called once
                bitmapFileIO.Received(1).WriteBitmap(bmp, "INVALID file path");

                // ... but he view shouldn't have received any call
                view.Received(0).SetPreviewBitmap(Arg.Any<Bitmap>());
                view.Received(0).SetControlsEnabled(Arg.Any<bool>());
                view.Received(0).SetStatusMessage(Arg.Any<BitmapEditorStatus>(), Arg.Any<string>());
            }
        }


        
        [TestMethod]
        // We check that the CreatePreview method behaves properly if the image is too wide
        public void BitmapEditor_CreatePreview_TooWide()
        {
            // The original image's dimensions
            int imgWidth = 857;
            int imgHeight = 251;

            // The side of the view's preview square area
            int squareSide = 409;

            // Computing the expected dimensions
            int expectedWidth = squareSide;
            int expectedHeight = (int)(((float)squareSide / imgWidth) * imgHeight);

            // Preparing variables for the original bitmap and the preview
            Bitmap original = new Bitmap(imgWidth, imgHeight);
            Bitmap preview = null;

            // We tell bitmapFileIO to return the original bitmap when reading it
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            bitmapFileIO.ReadBitmap(@"C:\valid\file\path").Returns(original);

            // The view should return the square side value we need for testing
            var view = Substitute.For<IBitmapViewer>();
            view.GetPreviewSquareSize().Returns(squareSide);

            // We store the preview image received by the view in a local variable
            view.SetPreviewBitmap(Arg.Do<Bitmap>(b => preview = b));

            // Creating the editor and reading the original bitmap
            var editor = new BitmapEditor(bitmapFileIO, view);
            editor.ReadFile(@"C:\valid\file\path");

            if (preview != null)
            {
                // We check that the preview has the expected dimensions we computed above
                Assert.AreEqual(expectedWidth, preview.Size.Width);
                Assert.AreEqual(expectedHeight, preview.Size.Height);
            }

            else
            {
                // If the preview is null, the test fails
                Assert.Fail();
            }
        }

        [TestMethod]
        // We check that the CreatePreview method behaves properly if the image is too high
        public void BitmapEditor_CreatePreview_TooHigh()
        {
            // The original image's dimensions
            int imgWidth = 227;
            int imgHeight = 907;

            // The side of the view's preview square area
            int squareSide = 367;

            // Computing the expected dimensions
            int expectedWidth = (int)(((float)squareSide / imgHeight) * imgWidth);
            int expectedHeight = squareSide;

            // Preparing variables for the original bitmap and the preview
            Bitmap original = new Bitmap(imgWidth, imgHeight);
            Bitmap preview = null;

            // We tell bitmapFileIO to return the original bitmap when reading it
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            bitmapFileIO.ReadBitmap(@"C:\valid\file\path").Returns(original);

            // The view should return the square side value we need for testing
            var view = Substitute.For<IBitmapViewer>();
            view.GetPreviewSquareSize().Returns(squareSide);

            // We store the preview image received by the view in a local variable
            view.SetPreviewBitmap(Arg.Do<Bitmap>(b => preview = b));

            // Creating the editor and reading the original bitmap
            var editor = new BitmapEditor(bitmapFileIO, view);
            editor.ReadFile(@"C:\valid\file\path");

            if (preview != null)
            {
                // We check that the preview has the expected dimensions we computed above
                Assert.AreEqual(expectedWidth, preview.Size.Width);
                Assert.AreEqual(expectedHeight, preview.Size.Height);
            }

            else
            {
                // If the preview is null, the test fails
                Assert.Fail();
            }
        }

        
        [TestMethod]
        // We check that the CreatePreview method behaves properly if the image is already squared
        public void BitmapEditor_CreatePreview_AlreadySquared()
        {
            // The image's side length
            int imgSide = 773;

            // The preview area's side length
            int squareSide = 367;

            // The expected width and height are both equal to the preview area's side length
            int expectedWidth = squareSide;
            int expectedHeight = squareSide;

            // Preparing variables for the original bitmap and the preview
            Bitmap original = new Bitmap(imgSide, imgSide);
            Bitmap preview = null;

            // We tell bitmapFileIO to return the original bitmap when reading it
            var bitmapFileIO = Substitute.For<IBitmapFileIO>();
            bitmapFileIO.ReadBitmap(Arg.Any<string>()).Returns(original);

            // The view should return the square side value we need for testing
            var view = Substitute.For<IBitmapViewer>();
            view.GetPreviewSquareSize().Returns(squareSide);

            // We store the preview image received by the view in a local variable
            view.SetPreviewBitmap(Arg.Do<Bitmap>(b => preview = b));

            // Creating the editor and reading the original bitmap
            var editor = new BitmapEditor(bitmapFileIO, view);
            editor.ReadFile(@"C:\valid\file\path");

            if (preview != null)
            {
                // We check that the preview has the expected dimensions we computed above
                Assert.AreEqual(expectedWidth, preview.Size.Width);
                Assert.AreEqual(expectedHeight, preview.Size.Height);
            }

            else
            {
                // If the preview is null, the test fails
                Assert.Fail();
            }
        }

        
        [TestMethod]
        // We check that the MatrixEdgeFilter behaves properly if resulting RGB values are in range. The computation is also checked
        public void MatrixEdgeFilter_Apply_InRangeAndRightComputation()
        {
            // 3x3 original image bytes
            byte[] bytes = {
                2,3,5,255, 7,11,13,255, 17,19,23,255,
                29,31,37,255, 89,101,103,255, 53,59,61,255,
                67,71,73,255, 79,83,89,255, 41,43,47,255
            };

            // The matrix used by the filter
            double[,] matrix = new double[,] {
                {-11, -19, -17},
                {-13, 23, -7},
                {-5, -3, -2}
            };

            // The expected outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,

                        // The blue, green and red channels should not be set to 255 but to these values:
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

            // Creating the original bitmap and setting its bytes
            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, bytes);

            // Calling the MatrixEdgeFilter and applying it
            MatrixEdgeFilter mef = new MatrixEdgeFilter("", matrix, false, 1.0, 0);
            byte[] actualBytes = GetBitmapBytes(mef.Apply(original));

            // If the expected bytes match the actual bytes, the test passes
            Assert.IsTrue(AreBytesEqual(expectedBytes, actualBytes));
        }

        [TestMethod]
        // We check that the MatrixEdgeFilter behaves properly if resulting RGB values are above 255
        public void MatrixEdgeFilter_Apply_AboveRange()
        {
            // 3x3 original image bytes
            byte[] bytes = {
                2,3,5,255, 7,11,13,255, 17,19,23,255,
                29,31,37,255, 233,239,241,255, 53,59,61,255, // Note that the middle pixel has high values
                67,71,73,255, 79,83,89,255, 41,43,47,255
            };

            // The matrix used by the filter
            double[,] matrix = new double[,] {
                {-11, -19, -17},
                {-13, 23, -7}, // Note that the matrix will make these high values even higher and go beyond 255
                {-5, -3, -2}
            };

            // The expected outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,

                        // All channels should be above range
                        255,255,255,255,

                                    0,0,0,0,
                0,0,0,0, 0,0,0,0, 0,0,0,0
            };

            // Creating the original bitmap and setting its bytes
            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, bytes);

            // Calling the MatrixEdgeFilter and applying it
            MatrixEdgeFilter mef = new MatrixEdgeFilter("", matrix, false, 1.0, 0);
            byte[] actualBytes = GetBitmapBytes(mef.Apply(original));

            // If the expected bytes match the actual bytes, the test passes
            Assert.IsTrue(AreBytesEqual(expectedBytes, actualBytes));
        }

        // Simple convolution with computed values below range and set to 0
        [TestMethod]
        // We check that the MatrixEdgeFilter behaves properly if resulting RGB values are below 0
        public void MatrixEdgeFilter_Apply_BelowRange()
        {
            // 3x3 original image bytes
            byte[] bytes = {
                2,3,5,255, 7,11,13,255, 17,19,23,255,
                29,31,37,255, 2,3,5,255, 53,59,61,255, // Note that the middle pixel has low values
                67,71,73,255, 79,83,89,255, 41,43,47,255 // However all other pixels surrounding it have high values on average
            };

            // The matrix used by the filter
            double[,] matrix = new double[,] {
                {-11, -19, -17},
                {-13, 23, -7}, // The matrix will make the middle values go below 0
                {-5, -3, -2}
            };

            // The expected outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,

                        // All channels should be below range
                        0,0,0,255,

                                    0,0,0,0,
                0,0,0,0, 0,0,0,0, 0,0,0,0
            };

            // Creating the original bitmap and setting its bytes
            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, bytes);

            // Calling the MatrixEdgeFilter and applying it
            MatrixEdgeFilter mef = new MatrixEdgeFilter("", matrix, false, 1.0, 0);
            byte[] actualBytes = GetBitmapBytes(mef.Apply(original));

            // If the expected bytes match the actual bytes, the test passes
            Assert.IsTrue(AreBytesEqual(expectedBytes, actualBytes));
        }

        
        [TestMethod]
        // We check that MatrixEdgeFilter properly computes pixel values
        public void MatrixEdgeFilter_Apply_ProperComputation()
        {
            // 3x3 original image bytes
            byte[] originalBytes = {
                2,3,5,255, 7,11,13,255, 17,19,23,255,
                29,31,37,255, 89,101,103,255, 53,59,61,255,
                67,71,73,255, 79,83,89,255, 41,43,47,255
            };

            // The matrix used by the filter
            double[,] matrix = new double[,] {
                {-11, -19, -17},
                {-13, 23, -7},
                {-5, -3, -2}
            };

            // The expected outer border is transparent black
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

            // Creating the original bitmap
            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, originalBytes);

            // Creating the expected bitmap
            Bitmap expected = new Bitmap(3, 3);
            SetBitmapBytes(expected, expectedBytes);

            // Applying the MatrixEdgeFilter
            MatrixEdgeFilter mef = new MatrixEdgeFilter("", matrix, false, 1.0, 0);
            Bitmap actual = mef.Apply(original);

            // If the expected bitmap matches the actual one, the test passes
            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }

        
        [TestMethod]
        // We verify that MatrixEdgeFilter leaves colors untouched if grayscale = false
        public void MatrixEdgeFilter_Apply_GrayscaleDisabled()
        {
            // 3x3 original image bytes
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

            // The expected outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,

                        // All channels should be left untouched
                        39,41,43,255,

                                    0,0,0,0,
                0,0,0,0, 0,0,0,0, 0,0,0,0
            };

            // Creating the original bitmap
            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, originalBytes);

            // Creating the expected bitmap
            Bitmap expected = new Bitmap(3, 3);
            SetBitmapBytes(expected, expectedBytes);

            // Applying the MatrixEdgeFilter
            MatrixEdgeFilter mef = new MatrixEdgeFilter("", noopMatrix, false, 1.0, 0);
            Bitmap actual = mef.Apply(original);

            // If the expected bitmap matches the actual one, the test passes
            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }

        [TestMethod]
        // We verify that MatrixEdgeFilter computes perceived luminance properly if grayscale = true
        public void MatrixEdgeFilter_Apply_GrayscaleEnabledPerceivedLuminance()
        {
            // 3x3 original image bytes (note that the average of RGB channels is equal to 68.666...)
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

            // The expected outer border is transparent black
            byte[] expectedBytes = {
                0,0,0,0, 0,0,0,0, 0,0,0,0,
                0,0,0,0,

                        // The center pixel should be grayscaled using
                        // perceived luminance BGR coefficients (0.11, 0.59, 0.30).
                        // With numbers (34; 154; 18) the perceived luminance
                        // gives us (100; 100; 100).
                        100,100,100,255,

                                    0,0,0,0,
                0,0,0,0, 0,0,0,0, 0,0,0,0
            };

            // Creating the original bitmap
            Bitmap original = new Bitmap(3, 3);
            SetBitmapBytes(original, originalBytes);

            // Creating the expected bitmap
            Bitmap expected = new Bitmap(3, 3);
            SetBitmapBytes(expected, expectedBytes);

            // Applying the MatrixEdgeFilter
            MatrixEdgeFilter mef = new MatrixEdgeFilter("", noopMatrix, true, 1.0, 0);
            Bitmap actual = mef.Apply(original);

            // If the expected bitmap matches the actual one, the test passes
            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }


        [TestMethod]
        // We verify that ThresoldFilter applies its two colors properly depending on pixel luminance
        public void ThresoldFilter_Apply_RightColorsCheck()
        {
            // Custom filter color
            Color filterColor = Color.FromArgb(83, 191, 113);

            // Minimum and maximum luminance (defines the range of values that will color a pixel in white)
            float min = 0.35f;
            float max = 0.65f;

            // CAREFUL! Bitmap bytes are stored in BGRA even if we specified the ARGB format.
            byte[] originalBytes = new byte[] {
                // Bytes with luminance below range
                0, 0, 0, 255,

                // Bytes with luminance in range
                127, 127, 127, 255,

                // Bytes with luminance above range
                255, 255, 255, 255
            };

            // Expected bytes
            byte[] expectedBytes = new byte[] {
                // Bytes with luminance below range should be set to the CUSTOM COLOR
                filterColor.B, filterColor.G, filterColor.R, 255,

                // Bytes with luminance in range should be set to WHITE
                255, 255, 255, 255,

                // Bytes with luminance above range should be set to the CUSTOM COLOR
                filterColor.B, filterColor.G, filterColor.R, 255
            };


            // Creating an original and expected 1x3 image
            Bitmap original = new Bitmap(1, 3);
            SetBitmapBytes(original, originalBytes);

            Bitmap expected = new Bitmap(1, 3);
            SetBitmapBytes(expected, expectedBytes);

            // Applying the thresold filter
            ThresoldFilter tf = new ThresoldFilter("", min, max, filterColor);
            Bitmap actual = tf.Apply(original);

            // If the expected bitmap matches the actual one, the test passes
            Assert.IsTrue( AreBitmapEquals(expected, actual) );
        }


        [TestMethod]
        // We check that BlackWhiteFilter computes the average of color channels on a test image
        public void BlackWhiteFilter_Apply_AverageOfChannelsOnImage()
        {
            // We clone the original image
            Bitmap original = new Bitmap("./landscape.png");
            Bitmap expected = (Bitmap)original.Clone();

            // We then computes the values that the filter should produce
            byte[] bytes = GetBitmapBytes(expected);
            int a, r, g, b, avg;

            for (int i = 0; i < bytes.Length; i += 4)
            {
                b = i;
                g = i + 1;
                r = i + 2;
                a = i + 3;

                // The BlackWhiteFilter uses arithmetic mean to produce grayscale images
                avg = (bytes[r] + bytes[g] + bytes[b]) / 3;

                bytes[r] = (byte)avg;
                bytes[g] = (byte)avg;
                bytes[b] = (byte)avg;
            }

            // Updating the expected bitmap with the right bytes
            SetBitmapBytes(expected, bytes);

            // Applying the black and white filter on the original bitmap
            BlackWhiteFilter bwf = new BlackWhiteFilter("");
            Bitmap actual = bwf.Apply(original);

            // if the originalbitmap matches the actual one, the test passes
            Assert.IsTrue(AreBitmapEquals(expected, actual));
        }


        [TestMethod]
        // We check that pixel filters return their custom name when converted to strings
        public void PixelFilter_ToString_DisplaysCustomName()
        {
            string customName = "Custom name for pixel filter";

            PixelFilter bwFilter = new BlackWhiteFilter( customName );

            Assert.AreEqual( customName, bwFilter.ToString() );
        }

        [TestMethod]
        // We check that edge filters return their custom name when converted to strings
        public void EdgeFilter_ToString_DisplaysCustomName()
        {
            string customName = "Custom name for edge filter";

            EdgeFilter laplacianFilter = new MatrixEdgeFilter(customName, MatrixEdgeFilter.MATRIX_LAPLACIAN_3X3, false, 1.0, 0);

            Assert.AreEqual(customName, laplacianFilter.ToString());
        }

        [TestMethod]
        // We check that NoopFilter instances return their custom name when converted to strings
        public void NoopFilter_ToString_DisplaysCustomName()
        {
            string customName = "Custom name for NOOP filter";

            NoopFilter noopFilter = new NoopFilter(customName);

            Assert.AreEqual(customName, noopFilter.ToString());
        }

        [TestMethod]
        // We check that MatrixEdgeFilter throws an exception if it is built with a null matrix
        public void MatrixEdgeFilter_constructor_NullMatrix()
        {
            try
            {
                // Constructing MatrixEdgeFilter with a null matrix
                MatrixEdgeFilter mef = new MatrixEdgeFilter("Filter name", null, false, 1.0, 0);

                // If no exception was thrown, the test fails
                Assert.Fail();
            }

            catch (Exception e)
            {
                // The exception should be of the right type
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }
        }
    }
}