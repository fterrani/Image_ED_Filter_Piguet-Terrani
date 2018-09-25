﻿/*
 * The Following Code was developed by Dewald Esterhuizen
 * View Documentation at: http://softwarebydefault.com
 * Licensed under Ms-PL 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace ImageEDFilter
{
    public partial class MainForm : Form
    {
        private Bitmap originalBitmap = null;
        private Bitmap previewBitmap = null;
        private Bitmap resultBitmap = null;

        private string msgNoImage = "No image chosen";
        private string msgNoFilter = "No filter applied";
        private string msgNoEDFilter = "No edge detection\nfilter applied";
        private string msgOk = "Ready to save";

        private bool ImageChosen()
        {
            return (originalBitmap != null);
        }

        private bool NoFilterChosen()
        {
            return (cmbColorFilter.SelectedIndex == 0 && cmbEdgeDetection.SelectedIndex == 0);
        }

        private bool NoEdgeDetection()
        {
            return (cmbEdgeDetection.SelectedIndex == 0);
        }
        
        public MainForm()
        {
            InitializeComponent();

            // Populates the comboboxes with a set of IBitmapFilter objects
            PrepareFilters();
            
            this.cmbColorFilter.SelectedIndex = 0;
            this.cmbEdgeDetection.SelectedIndex = 0;
        }

        private void PrepareFilters()
        {
            // Basic edge detection filters
            IBitmapFilter laplacian3x3 = new BitmapFilter("Laplacian 3x3", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Laplacian3x3, 1.0, 0, false));
            IBitmapFilter laplacian3x3Gray = new BitmapFilter("Laplacian 3x3 Grayscale", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Laplacian3x3, 1.0, 0, true));
            IBitmapFilter laplacian5x5 = new BitmapFilter("Laplacian 5x5", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Laplacian5x5, 1.0, 0, false));
            IBitmapFilter laplacian5x5Gray = new BitmapFilter("Laplacian 5x5 Grayscale", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Laplacian5x5, 1.0, 0, true));
            IBitmapFilter laplacianOfGaussian = new BitmapFilter("Laplacian of Gaussian", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.LaplacianOfGaussian, 1.0, 0, true));
            IBitmapFilter gaussian3x3 = new BitmapFilter("Gaussian 3x3", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Gaussian3x3, 1.0 / 16.0, 0, false));
            IBitmapFilter gaussian5x5Type1 = new BitmapFilter("Gaussian 5x5 (Type 1)", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Gaussian5x5Type1, 1.0 / 159.0, 0, true));
            IBitmapFilter gaussian5x5Type2 = new BitmapFilter("Gaussian 5x5 (Type 2)", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Gaussian5x5Type2, 1.0 / 256, 0, true));
            IBitmapFilter sobel = new BitmapFilter("Sobel", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Sobel3x3Horizontal, Matrix.Sobel3x3Vertical, false));
            IBitmapFilter sobelGray = new BitmapFilter("Sobel Grayscale", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Sobel3x3Horizontal, Matrix.Sobel3x3Vertical, true));
            IBitmapFilter prewitt = new BitmapFilter("Prewitt", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Prewitt3x3Horizontal, Matrix.Prewitt3x3Vertical, false));
            IBitmapFilter prewittGray = new BitmapFilter("Prewitt Grayscale", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Prewitt3x3Horizontal, Matrix.Prewitt3x3Vertical, true));
            IBitmapFilter kirsch = new BitmapFilter("Kirsch", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Kirsch3x3Horizontal, Matrix.Kirsch3x3Vertical, false));
            IBitmapFilter kirschGray = new BitmapFilter("Kirsch Grayscale", bmp => ExtBitmap.ConvolutionFilter(bmp, Matrix.Kirsch3x3Horizontal, Matrix.Kirsch3x3Vertical, true));

            // Edge detection filter combinations
            FilterChain laplacian3x3OfGaussian3x3 = new FilterChain(gaussian3x3, laplacian3x3);
            FilterChain laplacian3x3OfGaussian5x5Type1 = new FilterChain(gaussian5x5Type1, laplacian3x3);
            FilterChain laplacian3x3OfGaussian5x5Type2 = new FilterChain(gaussian5x5Type2, laplacian3x3);
            FilterChain laplacian5x5OfGaussian3x3 = new FilterChain(gaussian3x3, laplacian5x5);
            FilterChain laplacian5x5OfGaussian5x5Type1 = new FilterChain(gaussian5x5Type1, laplacian5x5);
            FilterChain laplacian5x5OfGaussian5x5Type2 = new FilterChain(gaussian5x5Type2, laplacian5x5);

            // Basic pixel filters
            IBitmapFilter rainbowFilter = new BitmapFilter("Rainbow filter", ImageFilters.RainbowFilter);
            IBitmapFilter blackWhiteFilter = new BitmapFilter("Black and white", ImageFilters.BlackWhite);
            IBitmapFilter swapFilter = new BitmapFilter("Swap filter", ImageFilters.ApplyFilterSwap);

            // BUGGED filter! (works only on square images)
            //IBitmapFilter magicMosaic = new BitmapFilter("Magic mosaic", ImageFilters.DivideCrop);

            IBitmapFilter zenFilter = new BitmapFilter("Zen filter", bmp => ImageFilters.ApplyFilter(bmp, 1, 10, 1, 1));
            IBitmapFilter miamiFilter = new BitmapFilter("Miami filter", bmp => ImageFilters.ApplyFilter(bmp, 1, 1, 10, 1));
            IBitmapFilter hellFilter = new BitmapFilter("Hell filter", bmp => ImageFilters.ApplyFilter(bmp, 1, 1, 10, 15));
            IBitmapFilter nightFilter = new BitmapFilter("Night filter", bmp => ImageFilters.ApplyFilter(bmp, 1, 1, 1, 25));
            IBitmapFilter megaGreenFilter = new BitmapFilter("Mega filter green", bmp => ImageFilters.ApplyFilterMega(bmp, 230, 110, Color.Green));
            IBitmapFilter megaOrangeFilter = new BitmapFilter("Mega filter orange", bmp => ImageFilters.ApplyFilterMega(bmp, 230, 110, Color.Orange));
            IBitmapFilter megaPinkFilter = new BitmapFilter("Mega filter pink", bmp => ImageFilters.ApplyFilterMega(bmp, 230, 110, Color.Pink));
            IBitmapFilter megaBlackFilter = new BitmapFilter("Mega filter custom", bmp => ImageFilters.ApplyFilterMega(bmp, 230, 110, Color.Black));
            IBitmapFilter crazySwapDivide = new BitmapFilter("Swap divide", bmp => ImageFilters.ApplyFilterSwapDivide(bmp, 1, 1, 2, 1));

            // Pixel filter combinations
            FilterChain crazyFilter = new FilterChain("Crazy filter", crazySwapDivide, swapFilter);


            IBitmapFilter noopFilter = new BitmapFilter("None", bmp => new Bitmap(bmp));

            this.cmbColorFilter.Items.AddRange(new IBitmapFilter[] {
                noopFilter, rainbowFilter, blackWhiteFilter, swapFilter, /*magicMosaic,*/
                zenFilter, miamiFilter, hellFilter, nightFilter, megaGreenFilter,
                megaOrangeFilter, megaPinkFilter, megaBlackFilter, crazyFilter
            });

            this.cmbEdgeDetection.Items.AddRange(new IBitmapFilter[] {
                noopFilter, laplacian3x3, laplacian3x3Gray, laplacian5x5, laplacian5x5Gray,
                laplacianOfGaussian, laplacian3x3OfGaussian3x3, laplacian3x3OfGaussian5x5Type1,
                laplacian3x3OfGaussian5x5Type2, laplacian5x5OfGaussian3x3, laplacian5x5OfGaussian5x5Type1,
                laplacian5x5OfGaussian5x5Type2, sobel, sobelGray, prewitt, prewittGray, kirsch, kirschGray
            });
        }

        // Shows an "Open..." window and displays the chosen file in the window
        private void btnOpenOriginal_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select an image file.";

            ofd.Filter = "Png Images(*.png)|*.png";
            ofd.Filter += "|Jpeg Images(*.jpg)|*.jpg";
            ofd.Filter += "|Bitmap Images(*.bmp)|*.bmp";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader streamReader = new StreamReader(ofd.FileName);
                originalBitmap = (Bitmap)Bitmap.FromStream(streamReader.BaseStream);
                streamReader.Close();

                previewBitmap = originalBitmap.CopyToSquareCanvas(picPreview.Width);
                picPreview.Image = previewBitmap;

                ApplyFilters(true);
            }
        }

        // Shows a "Save as..." window and saves the filtered image
        private void btnSaveNewImage_Click(object sender, EventArgs e)
        {
            ApplyFilters(false);

            if (resultBitmap != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Specify a file name and file path";
                sfd.Filter = "Png Images(*.png)|*.png|Jpeg Images(*.jpg)|*.jpg";
                sfd.Filter += "|Bitmap Images(*.bmp)|*.bmp";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileExtension = Path.GetExtension(sfd.FileName).ToUpper();
                    ImageFormat imgFormat = ImageFormat.Png;

                    if (fileExtension == "BMP")
                    {
                        imgFormat = ImageFormat.Bmp;
                    }
                    else if (fileExtension == "JPG")
                    {
                        imgFormat = ImageFormat.Jpeg;
                    }

                    StreamWriter streamWriter = new StreamWriter(sfd.FileName, false);
                    resultBitmap.Save(streamWriter.BaseStream, imgFormat);
                    streamWriter.Flush();
                    streamWriter.Close();

                    resultBitmap = null;
                }
            }
        }

        private void ApplyFilters(bool preview)
        {
            // We don't do anything if we cannot find the preview bitmap or if no filter was selected
            if (previewBitmap == null || cmbEdgeDetection.SelectedIndex == -1)
            {
                return;
            }

            Bitmap selectedSource = null;
            Bitmap bitmapResult = null;

            // The source is the preview bitmap if previewing
            // It is the original bitmap if saving to a file
            selectedSource = (preview ? previewBitmap : originalBitmap);
            
            // If the selected source is not null, we apply the filters
            if (selectedSource != null)
            {
                IBitmapFilter selectedEDFilter = this.cmbEdgeDetection.SelectedItem as IBitmapFilter;
                IBitmapFilter selectedColorFilter = this.cmbColorFilter.SelectedItem as IBitmapFilter;

                bitmapResult = selectedSource;

                if (selectedEDFilter != null)
                {
                    bitmapResult = selectedEDFilter.Apply(bitmapResult);
                }

                if (selectedColorFilter != null)
                {
                    bitmapResult = selectedColorFilter.Apply(bitmapResult);
                }
            }

            // We check if we have a result
            if (bitmapResult != null)
            {
                if (preview == true)
                {
                    // We display the result in the window (if previewing)
                    picPreview.Image = bitmapResult;
                }
                else
                {
                    // Or we store the result in resultBitmap (if saving to a file)
                    resultBitmap = bitmapResult;
                }
            }
        }

        private void NeighbourCountValueChangedEventHandler(object sender, EventArgs e)
        {
            ApplyFilters(true);
        }

        private void cmbColorFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters(true);
        }
    }
}
