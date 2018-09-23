using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    interface IBitmapFilter
    {
        string Name { get; set; }
        Bitmap Apply( Bitmap bmp );
    }
}
