namespace TheNoise_Server
{
    partial class Main
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.configureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clientsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectedGroupBox = new System.Windows.Forms.GroupBox();
            this.clientsListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.messagePanel = new System.Windows.Forms.Panel();
            this.messagesRichTextBox = new System.Windows.Forms.RichTextBox();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.connectedGroupBox.SuspendLayout();
            this.messagePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverToolStripMenuItem,
            this.clientsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(461, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // serverToolStripMenuItem
            // 
            this.serverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startServerToolStripMenuItem,
            this.stopServerToolStripMenuItem,
            this.toolStripSeparator1,
            this.configureToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            this.serverToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.serverToolStripMenuItem.Text = "&Server";
            // 
            // startServerToolStripMenuItem
            // 
            this.startServerToolStripMenuItem.Name = "startServerToolStripMenuItem";
            this.startServerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startServerToolStripMenuItem.Text = "S&tart Server";
            this.startServerToolStripMenuItem.Click += new System.EventHandler(this.startServerToolStripMenuItem_Click);
            // 
            // stopServerToolStripMenuItem
            // 
            this.stopServerToolStripMenuItem.Enabled = false;
            this.stopServerToolStripMenuItem.Name = "stopServerToolStripMenuItem";
            this.stopServerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopServerToolStripMenuItem.Text = "Sto&p Server";
            this.stopServerToolStripMenuItem.Click += new System.EventHandler(this.stopServerToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // configureToolStripMenuItem
            // 
            this.configureToolStripMenuItem.Name = "configureToolStripMenuItem";
            this.configureToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.configureToolStripMenuItem.Text = "&Configure";
            this.configureToolStripMenuItem.Click += new System.EventHandler(this.configureToolStripMenuItem_Click);
            // 
            // clientsToolStripMenuItem
            // 
            this.clientsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dropClientToolStripMenuItem});
            this.clientsToolStripMenuItem.Enabled = false;
            this.clientsToolStripMenuItem.Name = "clientsToolStripMenuItem";
            this.clientsToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.clientsToolStripMenuItem.Text = "&Clients";
            // 
            // dropClientToolStripMenuItem
            // 
            this.dropClientToolStripMenuItem.Enabled = false;
            this.dropClientToolStripMenuItem.Name = "dropClientToolStripMenuItem";
            this.dropClientToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.dropClientToolStripMenuItem.Text = "&Drop Client";
            this.dropClientToolStripMenuItem.Click += new System.EventHandler(this.dropClientToolStripMenuItem_Click);
            // 
            // connectedGroupBox
            // 
            this.connectedGroupBox.Controls.Add(this.clientsListBox);
            this.connectedGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectedGroupBox.Location = new System.Drawing.Point(0, 24);
            this.connectedGroupBox.Name = "connectedGroupBox";
            this.connectedGroupBox.Size = new System.Drawing.Size(461, 110);
            this.connectedGroupBox.TabIndex = 3;
            this.connectedGroupBox.TabStop = false;
            this.connectedGroupBox.Text = "Connected Clients";
            // 
            // clientsListBox
            // 
            this.clientsListBox.BackColor = System.Drawing.SystemColors.Control;
            this.clientsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clientsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientsListBox.FormattingEnabled = true;
            this.clientsListBox.Location = new System.Drawing.Point(3, 16);
            this.clientsListBox.MultiColumn = true;
            this.clientsListBox.Name = "clientsListBox";
            this.clientsListBox.Size = new System.Drawing.Size(455, 91);
            this.clientsListBox.TabIndex = 1;
            this.clientsListBox.SelectedIndexChanged += new System.EventHandler(this.clientsListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 134);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(461, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Messages";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // messagePanel
            // 
            this.messagePanel.Controls.Add(this.messagesRichTextBox);
            this.messagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagePanel.Location = new System.Drawing.Point(0, 150);
            this.messagePanel.Margin = new System.Windows.Forms.Padding(0);
            this.messagePanel.Name = "messagePanel";
            this.messagePanel.Padding = new System.Windows.Forms.Padding(5);
            this.messagePanel.Size = new System.Drawing.Size(461, 183);
            this.messagePanel.TabIndex = 5;
            // 
            // messagesRichTextBox
            // 
            this.messagesRichTextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.messagesRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.messagesRichTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.messagesRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesRichTextBox.Location = new System.Drawing.Point(5, 5);
            this.messagesRichTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.messagesRichTextBox.Name = "messagesRichTextBox";
            this.messagesRichTextBox.ReadOnly = true;
            this.messagesRichTextBox.Size = new System.Drawing.Size(451, 173);
            this.messagesRichTextBox.TabIndex = 0;
            this.messagesRichTextBox.Text = "";
            this.messagesRichTextBox.TextChanged += new System.EventHandler(this.messagesRichTextBox_TextChanged);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.debugToolStripMenuItem.Text = "Debug Mode";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 333);
            this.Controls.Add(this.messagePanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.connectedGroupBox);
            this.Controls.Add(this.menuStrip1);
            this.MinimumSize = new System.Drawing.Size(266, 267);
            this.Name = "Main";
            this.Text = "TheNoise Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.connectedGroupBox.ResumeLayout(false);
            this.messagePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem configureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clientsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dropClientToolStripMenuItem;
        private System.Windows.Forms.GroupBox connectedGroupBox;
        private System.Windows.Forms.ListBox clientsListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel messagePanel;
        private System.Windows.Forms.RichTextBox messagesRichTextBox;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;

    }
}

