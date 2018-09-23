using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    class EdgeDetectionFilters
    {
        public static BmpFilterDecorator laplacian3x3 = new SimpleConvolutionFilter("Laplacian 3x3", Matrix.Laplacian3x3, 1.0, 0, false);
        public static BmpFilterDecorator laplacian3x3Gray = new SimpleConvolutionFilter("Laplacian 3x3 Grayscale", Matrix.Laplacian3x3, 1.0, 0, true);
        public static BmpFilterDecorator laplacian5x5 = new SimpleConvolutionFilter("Laplacian 5x5", Matrix.Laplacian5x5, 1.0, 0, false);
        public static BmpFilterDecorator laplacian5x5Gray = new SimpleConvolutionFilter("Laplacian 5x5 Grayscale", Matrix.Laplacian5x5, 1.0, 0, true);
        public static BmpFilterDecorator laplacianOfGaussian = new SimpleConvolutionFilter("Laplacian of Gaussian", Matrix.LaplacianOfGaussian, 1.0, 0, true);
        public static BmpFilterDecorator gaussian3x3 = new SimpleConvolutionFilter("Gaussian 3x3", Matrix.Gaussian3x3, 1.0 / 16.0, 0, false);
        public static BmpFilterDecorator gaussian5x5Type1 = new SimpleConvolutionFilter("Gaussian 5x5 (Type 1)", Matrix.Gaussian5x5Type1, 1.0 / 159.0, 0, true);
        public static BmpFilterDecorator gaussian5x5Type2 = new SimpleConvolutionFilter("Gaussian 5x5 (Type 2)", Matrix.Gaussian5x5Type2, 1.0 / 256.0, 0, true);
        public static BmpFilterDecorator sobel = new XYConvolutionFilter("Sobel", Matrix.Sobel3x3Horizontal, Matrix.Sobel3x3Vertical, false);
        public static BmpFilterDecorator sobelGray = new XYConvolutionFilter("Sobel Grayscale", Matrix.Sobel3x3Horizontal, Matrix.Sobel3x3Vertical, true);
        public static BmpFilterDecorator prewitt = new XYConvolutionFilter("Prewitt", Matrix.Prewitt3x3Horizontal, Matrix.Prewitt3x3Vertical, false);
        public static BmpFilterDecorator prewittGray = new XYConvolutionFilter("Prewitt Grayscale", Matrix.Prewitt3x3Horizontal, Matrix.Prewitt3x3Vertical, true);
        public static BmpFilterDecorator kirsch = new XYConvolutionFilter("Kirsch", Matrix.Kirsch3x3Horizontal, Matrix.Kirsch3x3Vertical, false);
        public static BmpFilterDecorator kirschGray = new XYConvolutionFilter("Kirsch Grayscale", Matrix.Kirsch3x3Horizontal, Matrix.Kirsch3x3Vertical, true);
    }
}
