using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    // This interface defines a BitmapFilter
    interface IBitmapFilter
    {
        // The name of the filter
        string Name { get; set; }

        // The method to call to apply the filter (should return a new Bitmap object containing the result)
        Bitmap Apply( Bitmap bmp );
    }
}
