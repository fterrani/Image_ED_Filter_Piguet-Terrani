using ImageEDFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    // A bitmap editor that implements the IBitmapEditor interface
    public class BitmapEditor : IBitmapEditor
    {
        // The view will display the editor's data
        private IBitmapViewer view;

        // The bitmapIO will read and write bitmap files
        private IBitmapFileIO bitmapIO;

        // By default, pixel and edge filters are set to a NoopFilter
        private IBitmapFilter pixelFilter = new NoopFilter("");
        private IBitmapFilter edgeFilter = new NoopFilter("");

        // Original, preview and filtered preview bitmaps
        private Bitmap original = null;
        private Bitmap preview = null;
        private Bitmap filteredPreview = null;

        // We build the editor with a bitmapIO and a view
        public BitmapEditor( IBitmapFileIO _bitmapIO, IBitmapViewer _view )
        {
            bitmapIO = _bitmapIO ?? throw new ArgumentNullException();
            view = _view ?? throw new ArgumentNullException();

            // Updating the view
            CheckEditorState();
        }

        // Returns the original bitmap with filters applied on it
        public Bitmap GetBitmap()
        {
            return ApplyFilters( original );
        }

        // Puts a new bitmap in the editor
        public void SetBitmap( Bitmap bitmap )
        {
            original = bitmap ?? throw new ArgumentNullException();

            // Creates a new preview
            preview = CreatePreview( original, view.GetPreviewSquareSize() );

            // Applies filters on the preview
            ApplyOnPreview();

            // Updates the view
            CheckEditorState();
        }

        // Returns a preview of _original Bitmap fitting in a square of length previewSize
        // (the image is resized; proportions are preserved)
        private Bitmap CreatePreview(Bitmap _original, int previewSize)
        {
            float ratio = 1.0f;
            int maxSide = _original.Width > _original.Height ?
                          _original.Width : _original.Height;

            ratio = (float)maxSide / (float)previewSize;

            Bitmap _preview = (_original.Width > _original.Height ?
                                    new Bitmap(previewSize, (int)(_original.Height / ratio))
                                    : new Bitmap((int)(_original.Width / ratio), previewSize));

            using (Graphics graphicsResult = Graphics.FromImage(_preview))
            {
                graphicsResult.CompositingQuality = CompositingQuality.HighQuality;
                graphicsResult.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsResult.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphicsResult.DrawImage(_original,
                                        new Rectangle(0, 0,
                                            _preview.Width, _preview.Height),
                                        new Rectangle(0, 0,
                                            _original.Width, _original.Height),
                                            GraphicsUnit.Pixel);
                graphicsResult.Flush();
            }

            return _preview;
        }

        // Reads the Bitmap file located at srcFile
        public void ReadFile(string srcFile)
        {
            Bitmap bmpFile = bitmapIO.ReadBitmap( srcFile );

            SetBitmap(bmpFile);
        }

        // Applies the filters to the original Bitmap and saves the result to destFile
        public bool WriteFile(string destFile)
        {
            Bitmap filteredOriginal = ApplyFilters( original );

            return bitmapIO.WriteBitmap(filteredOriginal, destFile);
        }

        // Returns a set of usable pre-configured pixel filters
        public IBitmapFilter[] GetPixelFilters()
        {
            return new IBitmapFilter[] {
                new NoopFilter("None"),
                new BlackWhiteFilter("Black and white"),
                new ThresoldFilter("Thresold filter (black)", 0.35f, 0.65f, Color.Black)
            };
        }

        // Returns a set of usable pre-configured edge filters
        public IBitmapFilter[] GetEdgeFilters()
        {
            return new IBitmapFilter[] {
                new NoopFilter("None"),
                new MatrixEdgeFilter("Laplacian 3x3", MatrixEdgeFilter.MATRIX_LAPLACIAN_3X3, false, 1.0, 0),
                new MatrixEdgeFilter("Laplacian 3x3 (grayscale)", MatrixEdgeFilter.MATRIX_LAPLACIAN_3X3, true, 1.0, 0),
                new MatrixEdgeFilter("Laplacian 5x5", MatrixEdgeFilter.MATRIX_LAPLACIAN_5X5, false, 1.0, 0),
                new MatrixEdgeFilter("Laplacian 5x5 (grayscale)", MatrixEdgeFilter.MATRIX_LAPLACIAN_5X5, true, 1.0, 0)
            };
        }

        // Applies the currently selected filters on the preview and sends the preview to the view
        public void ApplyOnPreview()
        {
            if (!HasImage()) return;

            filteredPreview = ApplyFilters( preview );

            view.SetPreviewBitmap( filteredPreview );
        }

        // Applies the currently selected filters on Bitmap bmp
        private Bitmap ApplyFilters( Bitmap bmp )
        {
            if (bmp == null)
                return null;
            else
                return edgeFilter.Apply(pixelFilter.Apply(bmp));
        }

        // Returns TRUE if the editor contains a bitmap, FALSE otherwise
        public bool HasImage()
        {
            return (original != null && preview != null);
        }

        // Returns TRUE if the editor has an edge filter selected, FALSE otherwise
        public bool HasEdgeFilter()
        {
            return !(edgeFilter is NoopFilter);
        }

        // Returns TRUE if the editor has an pixel filter selected, FALSE otherwise
        public bool HasPixelFilter()
        {
            return !(pixelFilter is NoopFilter);
        }

        // Selects a new pixel filter
        public void SetPixelFilter(IBitmapFilter _pixelFilter)
        {
            pixelFilter = _pixelFilter ?? new NoopFilter("");
            ApplyOnPreview();
            CheckEditorState();
        }

        // Selects a new edge filter
        public void SetEdgeFilter(IBitmapFilter _edgeFilter)
        {
            edgeFilter = _edgeFilter ?? new NoopFilter("");
            ApplyOnPreview();
            CheckEditorState();
        }

        // Checks the editor's current state and transfers data to the view (controls activation, status and message)
        public void CheckEditorState()
        {
            BitmapEditorStatus status = BitmapEditorStatus.OK;
            string message = "";
            bool controlsEnabled = true;

            if (!HasImage())
            {
                // No image was chosen
                status = BitmapEditorStatus.WARNING;
                message = "No image chosen";
                controlsEnabled = false;
            }

            else
            {
                // If we have an image,controls are enabled
                controlsEnabled = true;

                bool hasPixelFilter = HasPixelFilter();
                bool hasEdgeFilter = HasEdgeFilter();

                // Warning if no filter was selected
                if (!hasPixelFilter && !hasEdgeFilter)
                {
                    status = BitmapEditorStatus.WARNING;
                    message = "No filter applied";
                }

                else
                {
                    if (!hasEdgeFilter)
                    {
                        // Warning if no edge detection filter was applied
                        status = BitmapEditorStatus.WARNING;
                        message = "No edge detection\nfilter applied";
                    }

                    else
                    {
                        // OK status confirming everything needed was configured/set
                        status = BitmapEditorStatus.OK;
                        message = "Edge detection\napplied. Ready to save.";
                    }
                }
            }

            // Updating the view
            view.SetControlsEnabled( controlsEnabled );
            view.SetStatusMessage( status, message );
        }
    }
}
