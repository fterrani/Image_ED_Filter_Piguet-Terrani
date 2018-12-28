using ImageEDFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    class BitmapEditor : IBitmapEditor
    {
        public const int BMP_ORIGINAL = 1;
        public const int BMP_PREVIEW = 2;
        public const int BMP_FILTERED = 3;

        public static readonly IBitmapFilter noopFilter = new NoopFilter();

        private IBitmapViewer view;
        private IBitmapFileIO bitmapIO;

        private IBitmapFilter pixelFilter = noopFilter;
        private IBitmapFilter edgeFilter = noopFilter;


        private Bitmap original = null;
        private Bitmap preview = null;
        private Bitmap filteredPreview = null;

        public BitmapEditor( IBitmapFileIO _bitmapIO, IBitmapViewer _view )
        {
            bitmapIO = _bitmapIO;
            view = _view;
        }

        public Bitmap GetBitmap()
        {
            return ApplyFilters( original );
        }

        public void SetBitmap( Bitmap bitmap )
        {
            original = bitmap;
            preview = CreatePreview( original, 200 );
            Apply();
        }

        // Copies sourceBitmap to a new Bitmap object (the image is resized; proportions are preserved)
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

        public void ReadFile(string srcFile)
        {
            Bitmap bmpFile = bitmapIO.ReadBitmap( srcFile );

            SetBitmap(bmpFile);
        }

        public bool WriteFile(string destFile)
        {
            FilterChain fc = new FilterChain(new IBitmapFilter[] { pixelFilter, edgeFilter });
            Bitmap filteredOriginal = fc.Apply(original);

            return bitmapIO.WriteBitmap(filteredOriginal, destFile );
        }

        public IBitmapFilter[] GetPixelFilters()
        {
            return new IBitmapFilter[] {
                new BlackWhiteFilter("Black and white"),
                new ThresoldFilter("Thresold filter (black)", 0.0f, 0.5f, Color.Black),
                new ThresoldFilter("Thresold filter (red)", 0.0f, 0.5f, Color.Red),
                new ThresoldFilter("Thresold filter (green)", 0.0f, 0.5f, Color.Green),
                new ThresoldFilter("Thresold filter (blue)", 0.0f, 0.5f, Color.Blue)
            };
        }


        public IBitmapFilter[] GetEdgeFilters()
        {
            return new IBitmapFilter[] {
                new MatrixEdgeFilter("Sobel 3x3", MatrixEdgeFilter.MATRIX_SOBEL_3X3_HORIZONTAL, MatrixEdgeFilter.MATRIX_SOBEL_3X3_VERTICAL, false),
                new MatrixEdgeFilter("Sobel 3x3 (grayscale)", MatrixEdgeFilter.MATRIX_SOBEL_3X3_HORIZONTAL, MatrixEdgeFilter.MATRIX_SOBEL_3X3_VERTICAL, true),
                new MatrixEdgeFilter("Laplacian 3x3", MatrixEdgeFilter.MATRIX_LAPLACIAN_3X3, false, 1.0, 0),
                new MatrixEdgeFilter("Laplacian 3x3 (grayscale)", MatrixEdgeFilter.MATRIX_LAPLACIAN_3X3, true, 1.0, 0)
            };
        }

        // TODO Apply on preview? on original? on both? call setpreviewbitmap?
        public void Apply()
        {
            filteredPreview = ApplyFilters( preview );

            view.SetPreviewBitmap( filteredPreview );
        }

        private Bitmap ApplyFilters( Bitmap bmp )
        {
            FilterChain fc = new FilterChain(new IBitmapFilter[] { pixelFilter, edgeFilter });

            return fc.Apply(bmp);
        }

        public bool HasImage()
        {
            return (original != null);
        }

        public bool HasEdgeFilter()
        {
            return (edgeFilter != null);
        }

        public bool HasPixelFilter()
        {
            return (pixelFilter != null);
        }

        public void SetPixelFilter(IBitmapFilter _pixelFilter)
        {
            pixelFilter = _pixelFilter ?? noopFilter;
        }

        public void SetEdgeFilter(IBitmapFilter _edgeFilter)
        {
            edgeFilter = _edgeFilter ?? noopFilter;
        }

        private void CheckEditorState()
        {
            BitmapEditorStatus status = BitmapEditorStatus.OK;
            string message = "";
            bool controlsEnabled = false;

            if (!HasImage())
            {
                status = BitmapEditorStatus.WARNING;
                message = "No image chosen";
            }

            else
            {
                controlsEnabled = true;

                bool hasPixelFilter = HasPixelFilter();
                bool hasEdgeFilter = HasEdgeFilter();

                if (!hasPixelFilter && !hasEdgeFilter)
                {
                    status = BitmapEditorStatus.WARNING;
                    message = "No filter applied";
                }

                else
                {
                    if (!hasEdgeFilter)
                    {
                        status = BitmapEditorStatus.WARNING;
                        message = "No edge detection\nfilter applied";
                    }

                    else
                    {
                        status = BitmapEditorStatus.OK;
                        message = "Edge detection\napplied. Ready to save.";
                    }
                }
            }

            view.SetControlsEnabled(controlsEnabled);
            view.SetStatusMessage(status, message);
        }
    }
}
