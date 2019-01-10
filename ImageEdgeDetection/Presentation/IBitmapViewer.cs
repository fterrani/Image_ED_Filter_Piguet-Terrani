using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    // This interface defines a view able to display information sent by an IBitmapEditor instance
    public interface IBitmapViewer
    {
        // Should display the preview sent by the editor
        void SetPreviewBitmap( Bitmap bitmap );

        // Should display the status and message sent by the editor
        void SetStatusMessage( BitmapEditorStatus status, string message );

        // Should enable or disable GUI controls
        void SetControlsEnabled( bool enabled );

        // Returns the square side length of the preview area to the editor
        int GetPreviewSquareSize();
    }
}