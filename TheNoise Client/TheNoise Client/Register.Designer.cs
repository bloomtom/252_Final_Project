namespace TheNoiseClient
{
    partial class Register
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Register));
            this.newUserNameLabel = new System.Windows.Forms.Label();
            this.newPasswordLabel = new System.Windows.Forms.Label();
            this.newUserNameBox = new System.Windows.Forms.TextBox();
            this.NewPasswordBox = new System.Windows.Forms.TextBox();
            this.registerButton = new System.Windows.Forms.Button();
            this.confrimPWordlabel = new System.Windows.Forms.Label();
            this.newCPWBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // newUserNameLabel
            // 
            this.newUserNameLabel.AutoSize = true;
            this.newUserNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.newUserNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newUserNameLabel.ForeColor = System.Drawing.Color.Black;
            this.newUserNameLabel.Location = new System.Drawing.Point(24, 137);
            this.newUserNameLabel.Name = "newUserNameLabel";
            this.newUserNameLabel.Size = new System.Drawing.Size(73, 13);
            this.newUserNameLabel.TabIndex = 0;
            this.newUserNameLabel.Text = "User Name:";
            // 
            // newPasswordLabel
            // 
            this.newPasswordLabel.AutoSize = true;
            this.newPasswordLabel.BackColor = System.Drawing.Color.Transparent;
            this.newPasswordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newPasswordLabel.Location = new System.Drawing.Point(24, 175);
            this.newPasswordLabel.Name = "newPasswordLabel";
            this.newPasswordLabel.Size = new System.Drawing.Size(65, 13);
            this.newPasswordLabel.TabIndex = 2;
            this.newPasswordLabel.Text = "Password:";
            // 
            // newUserNameBox
            // 
            this.newUserNameBox.Location = new System.Drawing.Point(144, 134);
            this.newUserNameBox.Name = "newUserNameBox";
            this.newUserNameBox.Size = new System.Drawing.Size(100, 20);
            this.newUserNameBox.TabIndex = 1;
            this.newUserNameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newUserNameBox_KeyDown);
            // 
            // NewPasswordBox
            // 
            this.NewPasswordBox.Location = new System.Drawing.Point(144, 172);
            this.NewPasswordBox.Name = "NewPasswordBox";
            this.NewPasswordBox.PasswordChar = '?';
            this.NewPasswordBox.Size = new System.Drawing.Size(100, 20);
            this.NewPasswordBox.TabIndex = 3;
            this.NewPasswordBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NewPasswordBox_KeyDown);
            // 
            // registerButton
            // 
            this.registerButton.Location = new System.Drawing.Point(51, 262);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(160, 23);
            this.registerButton.TabIndex = 6;
            this.registerButton.Text = "Register";
            this.registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.registerButton_Click);
            // 
            // confrimPWordlabel
            // 
            this.confrimPWordlabel.AutoSize = true;
            this.confrimPWordlabel.BackColor = System.Drawing.Color.Transparent;
            this.confrimPWordlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.confrimPWordlabel.Location = new System.Drawing.Point(24, 214);
            this.confrimPWordlabel.Name = "confrimPWordlabel";
            this.confrimPWordlabel.Size = new System.Drawing.Size(111, 13);
            this.confrimPWordlabel.TabIndex = 4;
            this.confrimPWordlabel.Text = "Confirm Password:";
            // 
            // newCPWBox
            // 
            this.newCPWBox.Location = new System.Drawing.Point(144, 211);
            this.newCPWBox.Name = "newCPWBox";
            this.newCPWBox.PasswordChar = '?';
            this.newCPWBox.Size = new System.Drawing.Size(100, 20);
            this.newCPWBox.TabIndex = 5;
            this.newCPWBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newCPWBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(36, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(205, 52);
            this.label1.TabIndex = 7;
            this.label1.Text = "Please enter a user name and  \r\npassword that is seven characters \r\nlong and cont" +
    "ains at least three \r\nnumbers and four letters.";
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkOrange;
            this.BackgroundImage = global::TheNoiseClient.Properties.Resources.RegistrationPic;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(273, 324);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.newCPWBox);
            this.Controls.Add(this.confrimPWordlabel);
            this.Controls.Add(this.registerButton);
            this.Controls.Add(this.NewPasswordBox);
            this.Controls.Add(this.newUserNameBox);
            this.Controls.Add(this.newPasswordLabel);
            this.Controls.Add(this.newUserNameLabel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(279, 348);
            this.MinimumSize = new System.Drawing.Size(279, 348);
            this.Name = "Register";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label newUserNameLabel;
        private System.Windows.Forms.Label newPasswordLabel;
        private System.Windows.Forms.TextBox newUserNameBox;
        private System.Windows.Forms.TextBox NewPasswordBox;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Label confrimPWordlabel;
        private System.Windows.Forms.TextBox newCPWBox;
        private System.Windows.Forms.Label label1;
    }
}