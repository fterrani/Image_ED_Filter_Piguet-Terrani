using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ImageEdgeDetection
{
    class BitmapFileIO : IBitmapFileIO
    {
        public BitmapFileIO()
        { }

        public Bitmap ReadBitmap( string file )
        {
            // Loading selected image in originalBitmap
            StreamReader streamReader = new StreamReader(file);
            Bitmap originalBitmap = (Bitmap)Image.FromStream(streamReader.BaseStream);
            streamReader.Close();
            return originalBitmap;
        }

        public bool WriteBitmap( Bitmap bitmap, string file )
        {
            string fileExtension = Path.GetExtension(file).ToUpper();
            ImageFormat imgFormat = ImageFormat.Png;

            if (fileExtension == "BMP")
            {
                imgFormat = ImageFormat.Bmp;
            }
            else if (fileExtension == "JPG")
            {
                imgFormat = ImageFormat.Jpeg;
            }

            // Saving the result image in a file
            StreamWriter streamWriter = new StreamWriter(file, false);

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