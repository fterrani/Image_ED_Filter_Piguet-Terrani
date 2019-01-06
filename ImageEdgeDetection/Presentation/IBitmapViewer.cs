using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    public interface IBitmapViewer
    {
        void SetPreviewBitmap( Bitmap bitmap );

        void SetStatusMessage( BitmapEditorStatus status, string message );

        void SetControlsEnabled( bool enabled );

        int GetPreviewSquareSize();
    }
}