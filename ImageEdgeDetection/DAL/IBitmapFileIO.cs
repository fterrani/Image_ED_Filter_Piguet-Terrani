using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    // This interface defines a class able to read Bitmap from files and write Bitmap to files
    public interface IBitmapFileIO
    {
        // Return a bitmap from a given path
        Bitmap ReadBitmap( string file );

        // Return true if the bitmap is successfully saved
        bool WriteBitmap( Bitmap bitmap, string file);
    }
}