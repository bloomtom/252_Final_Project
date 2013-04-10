/*
 * Name:    Thomas Bloom
 * Class:   CP-252
 * Project: Lab 06 - Client Server
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TheNoise_Server
{
    public partial class ConfigForm : Form
    {
        private System.Net.IPAddress ipAddress;
        public System.Net.IPAddress IpAddress
        {
            get { return ipAddress; }
            set {  }
        }

        private int port = 9734;
        public int Port
        {
            get { return port; }
            set {  }
        }

        public ConfigForm(System.Net.IPAddress defaultIP, int defaultPort)
        {
            InitializeComponent();

            // Set the internal default and text boxes.
            ipAddress = defaultIP;
            port = defaultPort;
            ipTextBox.Text = ipAddress.ToString();
            portTextBox.Text = port.ToString();

            DialogResult = System.Windows.Forms.DialogResult.Cancel; // Assume cancel until ok button is pressed.
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Since DialogResult == Cancel by default, the caller will know to ignore ipAddress and port.
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Check the IP textbox for validity and set it into ipAddress.
            try
            {
                if (ipTextBox.Text.Length < 7)
                {
                    MessageBox.Show(this, "The entered IP address is invalid", this.ProductName,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ipAddress = System.Net.IPAddress.Parse(ipTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "The entered IP address is invalid", this.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check port textbox for validity.
            int.TryParse(portTextBox.Text, out port);
            if ((port < 1023) || (port > 49151)) // Detect invalid range.
            {
                MessageBox.Show(this, "The entered port is invalid. The valid port range is 1024-49151", 
                    this.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
