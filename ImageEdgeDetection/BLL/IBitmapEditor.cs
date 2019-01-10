using ImageEDFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    // This interface defines a bitmap editor able to apply filters on a Bitmap, show a preview of it and read/save Bitmaps to files
    public interface IBitmapEditor
    {
        // Getter & setter for the full-sized bitmap
        Bitmap GetBitmap();
        void SetBitmap(Bitmap bitmap);

        // Read & write bitmap from & to a given path
        void ReadFile(string file);
        bool WriteFile(string file);

        // Applies the filter on the preview and sends it to the view
        void ApplyOnPreview();

        // Returns all the pixel and edge filter in arrays
        IBitmapFilter[] GetPixelFilters();
        IBitmapFilter[] GetEdgeFilters();

        // Set the current filters of the editor to apply on the Bitmap
        void SetPixelFilter(IBitmapFilter pixelFilter);
        void SetEdgeFilter(IBitmapFilter edgeFilter);

        // Booleans that return information about the editor's state
        bool HasImage();
        bool HasEdgeFilter();
        bool HasPixelFilter();

        // Updates the view
        void CheckEditorState();
    }
}