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
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // picPreview
            // 
            this.picPreview.BackColor = System.Drawing.Color.Silver;
            this.picPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPreview.Cursor = System.Windows.Forms.Cursors.Cross;
            this.picPreview.Location = new System.Drawing.Point(12, 12);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(772, 600);
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
            this.btnSaveNewImage.Location = new System.Drawing.Point(620, 626);
            this.btnSaveNewImage.Name = "btnSaveNewImage";
            this.btnSaveNewImage.Size = new System.Drawing.Size(164, 47);
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
            this.cmbEdgeDetection.Location = new System.Drawing.Point(305, 679);
            this.cmbEdgeDetection.Name = "cmbEdgeDetection";
            this.cmbEdgeDetection.Size = new System.Drawing.Size(282, 37);
            this.cmbEdgeDetection.TabIndex = 20;
            this.cmbEdgeDetection.SelectedIndexChanged += new System.EventHandler(this.NeighbourCountValueChangedEventHandler);
            // 
            // labelEdge
            // 
            this.labelEdge.AutoSize = true;
            this.labelEdge.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEdge.Location = new System.Drawing.Point(201, 685);
            this.labelEdge.Name = "labelEdge";
            this.labelEdge.Size = new System.Drawing.Size(98, 25);
            this.labelEdge.TabIndex = 22;
            this.labelEdge.Text = "Edge filter";
            // 
            // labelColor
            // 
            this.labelColor.AutoSize = true;
            this.labelColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelColor.Location = new System.Drawing.Point(200, 632);
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
            this.cmbColorFilter.Location = new System.Drawing.Point(305, 626);
            this.cmbColorFilter.Name = "cmbColorFilter";
            this.cmbColorFilter.Size = new System.Drawing.Size(282, 37);
            this.cmbColorFilter.TabIndex = 24;
            this.cmbColorFilter.SelectedIndexChanged += new System.EventHandler(this.cmbColorFilter_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(660, 682);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 34);
            this.label1.TabIndex = 25;
            this.label1.Text = "No edge detection\r\nfilter applied";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(620, 685);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(796, 733);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbColorFilter);
            this.Controls.Add(this.labelColor);
            this.Controls.Add(this.labelEdge);
            this.Controls.Add(this.cmbEdgeDetection);
            this.Controls.Add(this.btnSaveNewImage);
            this.Controls.Add(this.btnOpenOriginal);
            this.Controls.Add(this.picPreview);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Edge Detection";
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

