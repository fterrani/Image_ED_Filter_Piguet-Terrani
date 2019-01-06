using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ImageEdgeDetection
{
    class BitmapFileIO : IBitmapFileIO
    {
        public BitmapFileIO()
        { }

        public Bitmap ReadBitmap( string file )
        {
            return (Bitmap) Image.FromFile(file);
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

            // TODO Let this method return an exception and put a try/catch in the BitmapEditor
            try
            {
                bitmap.Save(streamWriter.BaseStream, imgFormat);
                streamWriter.Flush();
                streamWriter.Close();
                return true;
            }
            catch
            {
                streamWriter.Close();
                return false;
            }
        }
    }
}