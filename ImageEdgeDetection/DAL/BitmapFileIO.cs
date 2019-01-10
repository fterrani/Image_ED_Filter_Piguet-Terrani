using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ImageEdgeDetection
{
    // This class reads Bitmap instances from files and writes Bitmap instances to files
    public class BitmapFileIO : IBitmapFileIO
    {
        public BitmapFileIO()
        { }

        // Tries reading an image from the provided file path.
        // Can throw an exception if the file is not found
        public Bitmap ReadBitmap( string file )
        {
            return (Bitmap) Image.FromFile(file);
        }

        // Writes a Bitmap instance to a file, saving in the format deduced from the destination path
        // Can throw an exception if the file is not found
        public bool WriteBitmap( Bitmap bitmap, string file )
        {
            string fileExtension = Path.GetExtension(file).ToUpper();
            ImageFormat imgFormat = ImageFormat.Png;

            if (fileExtension == ".BMP")
            {
                imgFormat = ImageFormat.Bmp;
            }
            else if (fileExtension == ".JPG")
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
                streamWriter.Close();
                return false;
            }
        }
    }
}