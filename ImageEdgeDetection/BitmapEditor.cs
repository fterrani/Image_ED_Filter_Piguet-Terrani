using ImageEDFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    class BitmapEditor
    {
        public const int BMP_ORIGINAL = 1;
        public const int BMP_PREVIEW = 2;
        public const int BMP_FILTERED = 3;

        private Object view;
        private Object bitmapIO;

        private IBitmapFilter pixelFilter = null;
        private IBitmapFilter edgeFilter = null;


        private Bitmap original = null;
        private Bitmap preview = null;
        private Bitmap filtered = null;

        public BitmapEditor( Object _view )
        {
            view = _view;
        }

        public Bitmap GetBitmap( int bmpConstant )
        {
            switch (bmpConstant)
            {
                case BMP_ORIGINAL: return original;
                case BMP_PREVIEW: return preview;
                case BMP_FILTERED: return filtered;

                default: return filtered;
            }
        }

        public void SetBitmap( Bitmap bitmap )
        {
            original = bitmap;

            // TODO Regenerate preview
            // TODO Reset filtered
        }

        public void ReadFile(string srcFile)
        {
            Bitmap bmpFile = null;// bitmapIO.ReadBitmap( srcFile );

            SetBitmap(bmpFile);
        }

        public bool WriteFile(string destFile)
        {
            // bitmapIO.WriteBitmap( filtered, destFile );
            return false;
        }

        public void ApplyFilters()
        {
            FilterChain fc = new FilterChain(new IBitmapFilter[] { pixelFilter, edgeFilter });
            filtered = fc.Apply( original );
        }

        public static IBitmapFilter[] GetPixelFilters()
        {

        }
    }
}
