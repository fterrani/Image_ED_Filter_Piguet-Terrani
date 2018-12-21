using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ImageEdgeDetection
{
    class BitmapFileIO : IBitmapFileIO
    {
        private Bitmap originalBitmap = null; // Original image (original size)
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private ImageFormat imgFormat;

        public Bitmap ReadBitmap( string file )
        {           
            // Loading selected image in originalBitmap
            streamReader = new StreamReader(file);
            originalBitmap = (Bitmap)Image.FromStream(streamReader.BaseStream);
            streamReader.Close();                
            return originalBitmap;
        }

        public bool WriteBitmap( Bitmap bitmap, string file )
        {         
            string fileExtension = Path.GetExtension(file).ToUpper();
            imgFormat = ImageFormat.Png;

            if (fileExtension == "BMP")
            {
                imgFormat = ImageFormat.Bmp;
            }
            else if (fileExtension == "JPG")
            {
                imgFormat = ImageFormat.Jpeg;
            }

            // Saving the result image in a file
            streamWriter = new StreamWriter(file, false);
            try
            {
                bitmap.Save(streamWriter.BaseStream, imgFormat);
                streamWriter.Flush();
                streamWriter.Close();
                return true;
            }
            catch
            {
                return false;
            }            
        }
    }
}