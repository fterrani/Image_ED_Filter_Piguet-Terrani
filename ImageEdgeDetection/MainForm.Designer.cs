namespace ImageEDFilter
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.btnOpenOriginal = new System.Windows.Forms.Button();
            this.btnSaveNewImage = new System.Windows.Forms.Button();
            this.cmbEdgeDetection = new System.Windows.Forms.ComboBox();
            this.labelEdge = new System.Windows.Forms.Label();
            this.labelColor = new System.Windows.Forms.Label();
            this.cmbColorFilter = new System.Windows.Forms.ComboBox();
            this.lbMessage = new System.Windows.Forms.Label();
            this.picMessageIcon = new System.Windows.Forms.PictureBox();
            this.pnlDarkBackground = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMessageIcon)).BeginInit();
            this.pnlDarkBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // picPreview
            // 
            this.picPreview.BackColor = System.Drawing.Color.Transparent;
            this.picPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picPreview.Cursor = System.Windows.Forms.Cursors.Cross;
            this.picPreview.Location = new System.Drawing.Point(36, 35);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(697, 525);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 13;
            this.picPreview.TabStop = false;
            // 
            // btnOpenOriginal
            // 
            this.btnOpenOriginal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenOriginal.Location = new System.Drawing.Point(12, 626);
            this.btnOpenOriginal.Name = "btnOpenOriginal";
            this.btnOpenOriginal.Size = new System.Drawing.Size(150, 95);
            this.btnOpenOriginal.TabIndex = 15;
            this.btnOpenOriginal.Text = "Load Image";
            this.btnOpenOriginal.UseVisualStyleBackColor = true;
            this.btnOpenOriginal.Click += new System.EventHandler(this.btnOpenOriginal_Click);
            // 
            // btnSaveNewImage
            // 
            this.btnSaveNewImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveNewImage.Location = new System.Drawing.Point(599, 684);
            this.btnSaveNewImage.Name = "btnSaveNewImage";
            this.btnSaveNewImage.Size = new System.Drawing.Size(185, 37);
            this.btnSaveNewImage.TabIndex = 16;
            this.btnSaveNewImage.Text = "Save";
            this.btnSaveNewImage.UseVisualStyleBackColor = true;
            this.btnSaveNewImage.Click += new System.EventHandler(this.btnSaveNewImage_Click);
            // 
            // cmbEdgeDetection
            // 
            this.cmbEdgeDetection.BackColor = System.Drawing.Color.White;
            this.cmbEdgeDetection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEdgeDetection.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEdgeDetection.FormattingEnabled = true;
            this.cmbEdgeDetection.Location = new System.Drawing.Point(288, 684);
            this.cmbEdgeDetection.Name = "cmbEdgeDetection";
            this.cmbEdgeDetection.Size = new System.Drawing.Size(282, 37);
            this.cmbEdgeDetection.TabIndex = 20;
            this.cmbEdgeDetection.SelectedIndexChanged += new System.EventHandler(this.cmbEdgeDetection_SelectedIndexChanged);
            // 
            // labelEdge
            // 
            this.labelEdge.AutoSize = true;
            this.labelEdge.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEdge.Location = new System.Drawing.Point(186, 690);
            this.labelEdge.Name = "labelEdge";
            this.labelEdge.Size = new System.Drawing.Size(98, 25);
            this.labelEdge.TabIndex = 22;
            this.labelEdge.Text = "Edge filter";
            // 
            // labelColor
            // 
            this.labelColor.AutoSize = true;
            this.labelColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelColor.Location = new System.Drawing.Point(185, 643);
            this.labelColor.Name = "labelColor";
            this.labelColor.Size = new System.Drawing.Size(99, 25);
            this.labelColor.TabIndex = 23;
            this.labelColor.Text = "Color filter";
            // 
            // cmbColorFilter
            // 
            this.cmbColorFilter.BackColor = System.Drawing.Color.White;
            this.cmbColorFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColorFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbColorFilter.FormattingEnabled = true;
            this.cmbColorFilter.Location = new System.Drawing.Point(288, 637);
            this.cmbColorFilter.Name = "cmbColorFilter";
            this.cmbColorFilter.Size = new System.Drawing.Size(282, 37);
            this.cmbColorFilter.TabIndex = 24;
            this.cmbColorFilter.SelectedIndexChanged += new System.EventHandler(this.cmbColorFilter_SelectedIndexChanged);
            // 
            // lbMessage
            // 
            this.lbMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMessage.Location = new System.Drawing.Point(637, 626);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(147, 48);
            this.lbMessage.TabIndex = 25;
            this.lbMessage.Text = "Status text";
            this.lbMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picMessageIcon
            // 
            this.picMessageIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picMessageIcon.Location = new System.Drawing.Point(600, 635);
            this.picMessageIcon.Name = "picMessageIcon";
            this.picMessageIcon.Size = new System.Drawing.Size(32, 32);
            this.picMessageIcon.TabIndex = 26;
            this.picMessageIcon.TabStop = false;
            // 
            // pnlDarkBackground
            // 
            this.pnlDarkBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlDarkBackground.Controls.Add(this.picPreview);
            this.pnlDarkBackground.Location = new System.Drawing.Point(12, 12);
            this.pnlDarkBackground.Name = "pnlDarkBackground";
            this.pnlDarkBackground.Size = new System.Drawing.Size(772, 599);
            this.pnlDarkBackground.TabIndex = 27;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(796, 733);
            this.Controls.Add(this.pnlDarkBackground);
            this.Controls.Add(this.picMessageIcon);
            this.Controls.Add(this.lbMessage);
            this.Controls.Add(this.cmbColorFilter);
            this.Controls.Add(this.labelColor);
            this.Controls.Add(this.labelEdge);
            this.Controls.Add(this.cmbEdgeDetection);
            this.Controls.Add(this.btnSaveNewImage);
            this.Controls.Add(this.btnOpenOriginal);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Edge Detection";
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMessageIcon)).EndInit();
            this.pnlDarkBackground.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Button btnOpenOriginal;
        private System.Windows.Forms.Button btnSaveNewImage;
        private System.Windows.Forms.ComboBox cmbEdgeDetection;
        private System.Windows.Forms.Label labelEdge;
        private System.Windows.Forms.Label labelColor;
        private System.Windows.Forms.ComboBox cmbColorFilter;
        private System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.PictureBox picMessageIcon;
        private System.Windows.Forms.Panel pnlDarkBackground;
    }
}

