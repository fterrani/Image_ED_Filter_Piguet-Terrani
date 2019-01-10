using ImageEDFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    // This abstract class is used to explicitly classify IBitmapFilter classes as edge detection filters
    public abstract class EdgeFilter : IBitmapFilter
    {
        public string Name { get; set; }

        public EdgeFilter(string name)
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
