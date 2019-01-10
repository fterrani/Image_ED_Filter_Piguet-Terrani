using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageEdgeDetection
{
    // This enum defines the status of a BitmapEditor object
    public enum BitmapEditorStatus
    {
        OK, // Means the editor has an image and an edge filter was selected
        WARNING // Means the editor is missing an image or that no edge filter was selected
    }
}