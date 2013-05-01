namespace TheNoise_Server
{
    partial class ConfigForm
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.localPortTextBox = new System.Windows.Forms.TextBox();
            this.connectionGroupBox = new System.Windows.Forms.GroupBox();
            this.localIpTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.winAuthRadioButton = new System.Windows.Forms.RadioButton();
            this.userAuthRadioButton = new System.Windows.Forms.RadioButton();
            this.sqlUsernameTextBox = new System.Windows.Forms.TextBox();
            this.sqlPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.sqlIpTextBox = new System.Windows.Forms.TextBox();
            this.sqlPortTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.sqlCredentialPanel = new System.Windows.Forms.Panel();
            this.connectionGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.sqlCredentialPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(114, 281);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(12, 281);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "&IP Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "&Port Number";
            // 
            // localPortTextBox
            // 
            this.localPortTextBox.Location = new System.Drawing.Point(73, 44);
            this.localPortTextBox.Name = "localPortTextBox";
            this.localPortTextBox.Size = new System.Drawing.Size(89, 20);
            this.localPortTextBox.TabIndex = 6;
            // 
            // connectionGroupBox
            // 
            this.connectionGroupBox.Controls.Add(this.localIpTextBox);
            this.connectionGroupBox.Controls.Add(this.localPortTextBox);
            this.connectionGroupBox.Controls.Add(this.label2);
            this.connectionGroupBox.Controls.Add(this.label1);
            this.connectionGroupBox.Location = new System.Drawing.Point(12, 12);
            this.connectionGroupBox.Name = "connectionGroupBox";
            this.connectionGroupBox.Size = new System.Drawing.Size(176, 76);
            this.connectionGroupBox.TabIndex = 7;
            this.connectionGroupBox.TabStop = false;
            this.connectionGroupBox.Text = "Local Binding";
            // 
            // localIpTextBox
            // 
            this.localIpTextBox.Location = new System.Drawing.Point(73, 18);
            this.localIpTextBox.Name = "localIpTextBox";
            this.localIpTextBox.Size = new System.Drawing.Size(89, 20);
            this.localIpTextBox.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sqlCredentialPanel);
            this.groupBox1.Controls.Add(this.sqlIpTextBox);
            this.groupBox1.Controls.Add(this.sqlPortTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.userAuthRadioButton);
            this.groupBox1.Controls.Add(this.winAuthRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(13, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(174, 180);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database Connection";
            // 
            // winAuthRadioButton
            // 
            this.winAuthRadioButton.AutoSize = true;
            this.winAuthRadioButton.Checked = true;
            this.winAuthRadioButton.Location = new System.Drawing.Point(8, 78);
            this.winAuthRadioButton.Name = "winAuthRadioButton";
            this.winAuthRadioButton.Size = new System.Drawing.Size(140, 17);
            this.winAuthRadioButton.TabIndex = 0;
            this.winAuthRadioButton.TabStop = true;
            this.winAuthRadioButton.Text = "&Windows Authentication";
            this.winAuthRadioButton.UseVisualStyleBackColor = true;
            // 
            // userAuthRadioButton
            // 
            this.userAuthRadioButton.AutoSize = true;
            this.userAuthRadioButton.Location = new System.Drawing.Point(8, 98);
            this.userAuthRadioButton.Name = "userAuthRadioButton";
            this.userAuthRadioButton.Size = new System.Drawing.Size(118, 17);
            this.userAuthRadioButton.TabIndex = 1;
            this.userAuthRadioButton.Text = "&User Authentication";
            this.userAuthRadioButton.UseVisualStyleBackColor = true;
            this.userAuthRadioButton.CheckedChanged += new System.EventHandler(this.userAuthRadioButton_CheckedChanged);
            // 
            // sqlUsernameTextBox
            // 
            this.sqlUsernameTextBox.Location = new System.Drawing.Point(66, 7);
            this.sqlUsernameTextBox.Name = "sqlUsernameTextBox";
            this.sqlUsernameTextBox.Size = new System.Drawing.Size(89, 20);
            this.sqlUsernameTextBox.TabIndex = 11;
            // 
            // sqlPasswordTextBox
            // 
            this.sqlPasswordTextBox.Location = new System.Drawing.Point(66, 33);
            this.sqlPasswordTextBox.Name = "sqlPasswordTextBox";
            this.sqlPasswordTextBox.PasswordChar = '•';
            this.sqlPasswordTextBox.Size = new System.Drawing.Size(89, 20);
            this.sqlPasswordTextBox.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Pa&ssword";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "User&name";
            // 
            // sqlIpTextBox
            // 
            this.sqlIpTextBox.Location = new System.Drawing.Point(72, 19);
            this.sqlIpTextBox.Name = "sqlIpTextBox";
            this.sqlIpTextBox.Size = new System.Drawing.Size(89, 20);
            this.sqlIpTextBox.TabIndex = 11;
            // 
            // sqlPortTextBox
            // 
            this.sqlPortTextBox.Location = new System.Drawing.Point(72, 45);
            this.sqlPortTextBox.Name = "sqlPortTextBox";
            this.sqlPortTextBox.Size = new System.Drawing.Size(89, 20);
            this.sqlPortTextBox.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "&Port Number";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "&IP Address";
            // 
            // sqlCredentialPanel
            // 
            this.sqlCredentialPanel.Controls.Add(this.sqlUsernameTextBox);
            this.sqlCredentialPanel.Controls.Add(this.sqlPasswordTextBox);
            this.sqlCredentialPanel.Controls.Add(this.label3);
            this.sqlCredentialPanel.Controls.Add(this.label4);
            this.sqlCredentialPanel.Enabled = false;
            this.sqlCredentialPanel.Location = new System.Drawing.Point(6, 114);
            this.sqlCredentialPanel.Name = "sqlCredentialPanel";
            this.sqlCredentialPanel.Size = new System.Drawing.Size(161, 59);
            this.sqlCredentialPanel.TabIndex = 12;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(201, 316);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.connectionGroupBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Server";
            this.connectionGroupBox.ResumeLayout(false);
            this.connectionGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.sqlCredentialPanel.ResumeLayout(false);
            this.sqlCredentialPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox localPortTextBox;
        private System.Windows.Forms.GroupBox connectionGroupBox;
        private System.Windows.Forms.TextBox localIpTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox sqlUsernameTextBox;
        private System.Windows.Forms.TextBox sqlPasswordTextBox;
        private System.Windows.Forms.RadioButton userAuthRadioButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton winAuthRadioButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel sqlCredentialPanel;
        private System.Windows.Forms.TextBox sqlIpTextBox;
        private System.Windows.Forms.TextBox sqlPortTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}