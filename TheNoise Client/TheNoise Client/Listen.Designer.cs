namespace TheNoiseClient
{
    partial class Listen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Listen));
            this.musicFilesListBox = new System.Windows.Forms.ListBox();
            this.noiseBoxlabel = new System.Windows.Forms.Label();
            this.audioInfoBox = new System.Windows.Forms.TextBox();
            this.noiseInformationLabel = new System.Windows.Forms.Label();
            this.playButton = new System.Windows.Forms.Button();
            this.Stopbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // musicFilesListBox
            // 
            this.musicFilesListBox.FormattingEnabled = true;
            this.musicFilesListBox.Location = new System.Drawing.Point(239, 48);
            this.musicFilesListBox.Name = "musicFilesListBox";
            this.musicFilesListBox.Size = new System.Drawing.Size(159, 264);
            this.musicFilesListBox.TabIndex = 0;
            // 
            // noiseBoxlabel
            // 
            this.noiseBoxlabel.AutoSize = true;
            this.noiseBoxlabel.BackColor = System.Drawing.Color.Transparent;
            this.noiseBoxlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noiseBoxlabel.ForeColor = System.Drawing.Color.White;
            this.noiseBoxlabel.Location = new System.Drawing.Point(227, 25);
            this.noiseBoxlabel.Name = "noiseBoxlabel";
            this.noiseBoxlabel.Size = new System.Drawing.Size(171, 20);
            this.noiseBoxlabel.TabIndex = 1;
            this.noiseBoxlabel.Text = "Music Files Avaiable";
            // 
            // audioInfoBox
            // 
            this.audioInfoBox.Location = new System.Drawing.Point(12, 93);
            this.audioInfoBox.Multiline = true;
            this.audioInfoBox.Name = "audioInfoBox";
            this.audioInfoBox.Size = new System.Drawing.Size(196, 152);
            this.audioInfoBox.TabIndex = 2;
            // 
            // noiseInformationLabel
            // 
            this.noiseInformationLabel.AutoSize = true;
            this.noiseInformationLabel.BackColor = System.Drawing.Color.Transparent;
            this.noiseInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noiseInformationLabel.ForeColor = System.Drawing.Color.White;
            this.noiseInformationLabel.Location = new System.Drawing.Point(36, 70);
            this.noiseInformationLabel.Name = "noiseInformationLabel";
            this.noiseInformationLabel.Size = new System.Drawing.Size(152, 20);
            this.noiseInformationLabel.TabIndex = 3;
            this.noiseInformationLabel.Text = "Music Information";
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(12, 301);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 4;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            // 
            // Stopbutton
            // 
            this.Stopbutton.Location = new System.Drawing.Point(133, 301);
            this.Stopbutton.Name = "Stopbutton";
            this.Stopbutton.Size = new System.Drawing.Size(75, 23);
            this.Stopbutton.TabIndex = 5;
            this.Stopbutton.Text = "Stop";
            this.Stopbutton.UseVisualStyleBackColor = true;
            // 
            // Listen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImage = global::TheNoiseClient.Properties.Resources.listenPic;
            this.ClientSize = new System.Drawing.Size(410, 346);
            this.Controls.Add(this.Stopbutton);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.noiseInformationLabel);
            this.Controls.Add(this.audioInfoBox);
            this.Controls.Add(this.noiseBoxlabel);
            this.Controls.Add(this.musicFilesListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Listen";
            this.Text = "The Noise";
            this.Load += new System.EventHandler(this.Listen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox musicFilesListBox;
        private System.Windows.Forms.Label noiseBoxlabel;
        private System.Windows.Forms.TextBox audioInfoBox;
        private System.Windows.Forms.Label noiseInformationLabel;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button Stopbutton;
    }
}

