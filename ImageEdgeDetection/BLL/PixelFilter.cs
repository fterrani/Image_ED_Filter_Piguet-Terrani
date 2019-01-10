using ImageEDFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    // This abstract class is used to explicitly classify IBitmapFilter classes as pixel filters
    public abstract class PixelFilter : IBitmapFilter
    {
        public string Name { get; set; }

        public PixelFilter( string name )
        {
            Name = name;
        }

        public abstract Bitmap Apply(Bitmap bmp);

        
        public override string ToString()
        {
            return Name;
        }
    }
}
