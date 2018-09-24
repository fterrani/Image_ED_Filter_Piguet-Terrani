using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    class BitmapFilter : IBitmapFilter
    {
        public string Name { get; set; }
        private Func<Bitmap, Bitmap> pixelFunc;

        public BitmapFilter(string _name, Func<Bitmap, Bitmap> _pixelFunc)
        {
            Name = _name;
            pixelFunc = _pixelFunc;
        }

        public Bitmap Apply(Bitmap bmp)
        {
            return pixelFunc(bmp);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}