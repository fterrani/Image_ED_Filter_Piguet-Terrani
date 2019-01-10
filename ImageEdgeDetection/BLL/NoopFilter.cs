using ImageEDFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    // This class defines a IBitmapFilter that doesn't do anything. It can be used to apply a null object design pattern.
    // Note that this filter still clones the bitmap to stay coherent with other filters that actually do something and return a different bitmap instance.
    public class NoopFilter : IBitmapFilter
    {
        public string Name { get; set; }

        public NoopFilter( string name)
        {
            Name = name;
        }

        // The NoopFilter does nothing and simply returns a clone of the source Bitmap
        public Bitmap Apply(Bitmap bmp)
        {
            return bmp.Clone(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                bmp.PixelFormat
            );
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
