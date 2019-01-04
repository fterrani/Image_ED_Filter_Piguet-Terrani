/*
 * The Following Code was developed by Dewald Esterhuizen
 * View Documentation at: http://softwarebydefault.com
 * Licensed under Ms-PL 
*/
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ImageEdgeDetection;

namespace ImageEDFilter
{
    public partial class MainForm : Form, IBitmapViewer
    {
        private IBitmapEditor editor;

        // Text colors for warning and success
        public static Color COLOR_WARNING = Color.FromArgb(70, 40, 0);
        public static Color COLOR_OK = Color.FromArgb(0, 190, 40);

        // Icon paths for warning and success
        public static string ICON_WARNING = ".\\warning-32x32.png";
        public static string ICON_OK = ".\\ok-32x32.png";

       
        public MainForm()
        {
            // Initializes controls edited with the WinForm designer (must be done BEFORE creating the BitmapEditor)
            InitializeComponent();

            // Instantiating BitmapEditor (must be done AFTER InitializeComponent() !!)
            IBitmapFileIO bitmapIO = new BitmapFileIO();
            editor = new BitmapEditor(bitmapIO, this);

            // Populates the comboboxes with a set of IBitmapFilter objects
            PrepareFilterComboboxes();
        }


        // Populates filter comboboxes with color and ED filters
        private void PrepareFilterComboboxes()
        {
            // Color filter combobox
            cmbColorFilter.Items.AddRange(editor.GetPixelFilters());

            // Edge detection filter combobox
            cmbEdgeDetection.Items.AddRange(editor.GetEdgeFilters());

            // First filter of each list selected by default (None filter)
            cmbColorFilter.SelectedIndex = 0;
            cmbEdgeDetection.SelectedIndex = 0;
        }

        // Shows an "Open..." window and displays the chosen file in the window
        private void btnOpenOriginal_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select an image file.";

            ofd.Filter = "All image files|*.png;*.jpg;*.bmp";
            ofd.Filter += "|PNG Images(*.png)|*.png";
            ofd.Filter += "|JPEG Images(*.jpg)|*.jpg";
            ofd.Filter += "|Bitmap Images(*.bmp)|*.bmp";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                editor.ReadFile(ofd.FileName);
            }
        }

        // Shows a "Save as..." window and saves the filtered image
        private void btnSaveNewImage_Click(object sender, EventArgs e)
        {

            if (picPreview.Image != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Specify a file name and file path";
                sfd.Filter = "PNG Images(*.png)|*.png";
                sfd.Filter += "|JPEG Images(*.jpg)|*.jpg";
                sfd.Filter += "|Bitmap Images(*.bmp)|*.bmp";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    editor.WriteFile(sfd.FileName);
                }
            }
        }

        // The color filter was changed
        private void cmbColorFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            editor.SetPixelFilter(cmbColorFilter.SelectedItem as IBitmapFilter);
        }

        // The edge detection filter was changed
        private void cmbEdgeDetection_SelectedIndexChanged(object sender, EventArgs e)
        {
            editor.SetEdgeFilter(cmbEdgeDetection.SelectedItem as IBitmapFilter);
        }

        public void SetPreviewBitmap(Bitmap bitmap)
        {
            picPreview.Image = bitmap;
        }

        public void SetStatusMessage(BitmapEditorStatus status, string message)
        {
            string iconPath = "";
            Color msgColor = DefaultForeColor;

            if (status == BitmapEditorStatus.OK)
            {
                iconPath = ICON_OK;
                msgColor = COLOR_OK;
            }
            else if (status == BitmapEditorStatus.WARNING)
            {
                iconPath = ICON_WARNING;
                msgColor = COLOR_WARNING;
            }

            StreamReader streamReader = new StreamReader(iconPath);
            picMessageIcon.Image = (Bitmap)Bitmap.FromStream(streamReader.BaseStream);
            lbMessage.ForeColor = msgColor;
            lbMessage.Text = message;
        }

        public void SetControlsEnabled(bool enabled)
        {
            cmbColorFilter.Enabled = enabled;
            cmbEdgeDetection.Enabled = enabled;
            btnSaveNewImage.Enabled = enabled;
        }
    }
}