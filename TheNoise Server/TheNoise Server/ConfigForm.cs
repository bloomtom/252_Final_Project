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
        private System.Net.IPAddress ipAddressServer;
        public System.Net.IPAddress IpAddressServer
        {
            get { return ipAddressServer; }
            set {  }
        }

        private int port;
        public int Port
        {
            get { return port; }
            set {  }
        }

        private System.Net.IPAddress ipAddressSQL;
        public System.Net.IPAddress IpAddressSQL
        {
            get { return ipAddressSQL; }
            set { }
        }

        private int portSQL;
        public int PortSQL
        {
            get { return portSQL; }
            set { }
        }

        public ConfigForm(System.Net.IPAddress defaultIPLocal, int defaultPortLocal, System.Net.IPAddress defaultIPSQL, int defaultPortSQL)
        {
            InitializeComponent();

            // Set the internal default and text boxes.
            ipAddressServer = defaultIPLocal;
            port = defaultPortLocal;
            localIpTextBox.Text = ipAddressServer.ToString();
            localPortTextBox.Text = port.ToString();

            ipAddressSQL = defaultIPSQL;
            portSQL = defaultPortSQL;
            

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
                if (localIpTextBox.Text.Length < 7)
                {
                    MessageBox.Show(this, "The entered IP address is invalid", this.ProductName,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ipAddressServer = System.Net.IPAddress.Parse(localIpTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "The entered IP address is invalid", this.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check port textbox for validity.
            int.TryParse(localPortTextBox.Text, out port);
            if ((port < 1023) || (port > 49151)) // Detect invalid range.
            {
                MessageBox.Show(this, "The entered port is invalid. The valid port range is 1024-49151", 
                    this.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void userAuthRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            sqlCredentialPanel.Enabled = userAuthRadioButton.Checked;
        }
    }
}
