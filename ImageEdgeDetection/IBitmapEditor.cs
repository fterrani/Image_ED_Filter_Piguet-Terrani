using ImageEDFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    interface IBitmapEditor
    {
        // Getter & setter for bitmap
        Bitmap GetBitmap();
        void SetBitmap(Bitmap bitmap);

        // Read & write bitmap from a given path
        void ReadFile(string file);
        bool WriteFile(string file);

        // ?
        void Apply();

        // Get all the pixel and edge filter in arrays
        IBitmapFilter[] GetPixelFilters();
        IBitmapFilter[] GetEdgeFilters();

        // Booleans that return some states  
        bool HasImage();
        bool HasEdgeFilter();
        bool HasPixelFilter();
    }
}