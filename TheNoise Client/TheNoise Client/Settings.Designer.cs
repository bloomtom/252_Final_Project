namespace TheNoiseClient
{
    partial class Settings
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
            this.okbutton = new System.Windows.Forms.Button();
            this.ipLabel = new System.Windows.Forms.Label();
            this.IpAddressBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // okbutton
            // 
            this.okbutton.Location = new System.Drawing.Point(52, 68);
            this.okbutton.Name = "okbutton";
            this.okbutton.Size = new System.Drawing.Size(80, 30);
            this.okbutton.TabIndex = 0;
            this.okbutton.Text = "OK";
            this.okbutton.UseVisualStyleBackColor = true;
            this.okbutton.Click += new System.EventHandler(this.okbutton_Click);
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(12, 29);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(61, 13);
            this.ipLabel.TabIndex = 1;
            this.ipLabel.Text = "IP Address:";
            // 
            // IpAddressBox
            // 
            this.IpAddressBox.Location = new System.Drawing.Point(86, 26);
            this.IpAddressBox.Name = "IpAddressBox";
            this.IpAddressBox.Size = new System.Drawing.Size(100, 20);
            this.IpAddressBox.TabIndex = 2;
            this.IpAddressBox.Text = "127.0.0.1";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 110);
            this.Controls.Add(this.IpAddressBox);
            this.Controls.Add(this.ipLabel);
            this.Controls.Add(this.okbutton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okbutton;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.TextBox IpAddressBox;
    }
}