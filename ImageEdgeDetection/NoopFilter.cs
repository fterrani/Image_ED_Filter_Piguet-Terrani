using ImageEDFilter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    class NoopFilter : IBitmapFilter
    {
        public string Name { get; set; }

        public NoopFilter()
        {
            Name = "NOOP filter";
        }

        public Bitmap Apply(Bitmap bmp)
        {
            return bmp.Clone(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.PixelFormat.Format32bppRgb
            );
        }
    }
}
