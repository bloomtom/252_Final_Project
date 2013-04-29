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
            this.TimeofSong = new System.Windows.Forms.Label();
            this.TrackName = new System.Windows.Forms.Label();
            this.Namelabel = new System.Windows.Forms.Label();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.trackInfo = new System.Windows.Forms.GroupBox();
            this.trackInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // musicFilesListBox
            // 
            this.musicFilesListBox.FormattingEnabled = true;
            this.musicFilesListBox.Location = new System.Drawing.Point(407, 48);
            this.musicFilesListBox.Name = "musicFilesListBox";
            this.musicFilesListBox.Size = new System.Drawing.Size(279, 264);
            this.musicFilesListBox.TabIndex = 0;
            this.musicFilesListBox.SelectedIndexChanged += new System.EventHandler(this.musicFilesListBox_SelectedIndexChanged);
            this.musicFilesListBox.DoubleClick += new System.EventHandler(this.musicFilesListBox_DoubleClick);
            // 
            // noiseBoxlabel
            // 
            this.noiseBoxlabel.AutoSize = true;
            this.noiseBoxlabel.BackColor = System.Drawing.Color.Transparent;
            this.noiseBoxlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noiseBoxlabel.ForeColor = System.Drawing.Color.GhostWhite;
            this.noiseBoxlabel.Location = new System.Drawing.Point(446, 25);
            this.noiseBoxlabel.Name = "noiseBoxlabel";
            this.noiseBoxlabel.Size = new System.Drawing.Size(171, 20);
            this.noiseBoxlabel.TabIndex = 1;
            this.noiseBoxlabel.Text = "Music Files Avaiable";
            // 
            // TimeofSong
            // 
            this.TimeofSong.AutoSize = true;
            this.TimeofSong.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimeofSong.Location = new System.Drawing.Point(73, 72);
            this.TimeofSong.Name = "TimeofSong";
            this.TimeofSong.Size = new System.Drawing.Size(47, 16);
            this.TimeofSong.TabIndex = 4;
            this.TimeofSong.Text = "Time:";
            // 
            // TrackName
            // 
            this.TrackName.AutoSize = true;
            this.TrackName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackName.Location = new System.Drawing.Point(6, 39);
            this.TrackName.Name = "TrackName";
            this.TrackName.Size = new System.Drawing.Size(114, 16);
            this.TrackName.TabIndex = 6;
            this.TrackName.Text = "Name of Track:";
            // 
            // Namelabel
            // 
            this.Namelabel.AutoSize = true;
            this.Namelabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Namelabel.Location = new System.Drawing.Point(142, 39);
            this.Namelabel.Name = "Namelabel";
            this.Namelabel.Size = new System.Drawing.Size(0, 16);
            this.Namelabel.TabIndex = 7;
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimeLabel.Location = new System.Drawing.Point(141, 72);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(0, 16);
            this.TimeLabel.TabIndex = 8;
            // 
            // trackInfo
            // 
            this.trackInfo.BackColor = System.Drawing.Color.Transparent;
            this.trackInfo.BackgroundImage = global::TheNoiseClient.Properties.Resources.trackinfo_back;
            this.trackInfo.Controls.Add(this.TimeLabel);
            this.trackInfo.Controls.Add(this.Namelabel);
            this.trackInfo.Controls.Add(this.TrackName);
            this.trackInfo.Controls.Add(this.TimeofSong);
            this.trackInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackInfo.ForeColor = System.Drawing.Color.Wheat;
            this.trackInfo.Location = new System.Drawing.Point(12, 48);
            this.trackInfo.Name = "trackInfo";
            this.trackInfo.Size = new System.Drawing.Size(365, 114);
            this.trackInfo.TabIndex = 8;
            this.trackInfo.TabStop = false;
            this.trackInfo.Text = "Track Information";
            // 
            // Listen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImage = global::TheNoiseClient.Properties.Resources.listenPic;
            this.ClientSize = new System.Drawing.Size(728, 354);
            this.Controls.Add(this.trackInfo);
            this.Controls.Add(this.noiseBoxlabel);
            this.Controls.Add(this.musicFilesListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(734, 378);
            this.MinimumSize = new System.Drawing.Size(734, 378);
            this.Name = "Listen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "The Noise";
            this.Load += new System.EventHandler(this.Listen_Load);
            this.trackInfo.ResumeLayout(false);
            this.trackInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox musicFilesListBox;
        private System.Windows.Forms.Label noiseBoxlabel;
        private System.Windows.Forms.Label TimeofSong;
        private System.Windows.Forms.Label TrackName;
        private System.Windows.Forms.Label Namelabel;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.GroupBox trackInfo;
    }
}

