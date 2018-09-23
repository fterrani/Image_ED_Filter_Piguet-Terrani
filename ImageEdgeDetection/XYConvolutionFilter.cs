using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    class XYConvolutionFilter : BmpFilterDecorator
    {
        private double[,] xFilterMatrix;
        private double[,] yFilterMatrix;
        double factor;
        int bias;
        bool grayscale;

        public XYConvolutionFilter(string name, double[,] _xFilterMatrix, double[,] _yFilterMatrix, bool _grayscale = false) : base( name )
        {
            xFilterMatrix = _xFilterMatrix;
            yFilterMatrix = _yFilterMatrix;
            grayscale = _grayscale;
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            return ExtBitmap.ConvolutionFilter(bmp, xFilterMatrix, yFilterMatrix, grayscale);
        }

        public override object Clone()
        {
            return new XYConvolutionFilter(Name, (double[,]) xFilterMatrix.Clone(), (double[,]) yFilterMatrix.Clone(), grayscale);
        }
    }
}
