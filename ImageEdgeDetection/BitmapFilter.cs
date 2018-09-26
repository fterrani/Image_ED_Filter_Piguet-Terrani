using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    // This class provides a simple implementation of the IBitmapFilter interface
    class BitmapFilter : IBitmapFilter
    {
        public string Name { get; set; }
        private Func<Bitmap, Bitmap> pixelFunc;

        // Instances are build from a name and a function returning a Bitmap
        public BitmapFilter(string _name, Func<Bitmap, Bitmap> _pixelFunc)
        {
            Name = _name;
            pixelFunc = _pixelFunc;
        }

        public Bitmap Apply(Bitmap bmp)
        {
            // We apply the provided function on the Bitmap object
            return pixelFunc(bmp);
        }

        // When converting the filter to a string, its name is returned
        public override string ToString()
        {
            return Name;
        }
    }
}