using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    class SimpleConvolutionFilter : BmpFilterDecorator
    {
        private double[,] filterMatrix;
        double factor;
        int bias;
        bool grayscale;

        public SimpleConvolutionFilter(string name, double[,] _filterMatrix, double _factor = 1, int _bias = 0, bool _grayscale = false) : base( name )
        {
            filterMatrix = _filterMatrix;
            factor = _factor;
            bias = _bias;
            grayscale = _grayscale;
        }

        public override Bitmap Apply(Bitmap bmp)
        {
            return ExtBitmap.ConvolutionFilter(bmp, filterMatrix, factor, bias, grayscale);
        }

        public override object Clone()
        {
            return new SimpleConvolutionFilter(Name, (double[,]) filterMatrix.Clone(), factor, bias, grayscale);
        }
    }
}
