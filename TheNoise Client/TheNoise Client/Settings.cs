using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace TheNoiseClient
{
    public partial class Settings : Form
    {
        private IPAddress ip = null;

        public IPAddress IP
        {
            get { return ip; }
            set { ip = value; }
        }

        public Settings(IPAddress defaultIP)
        {
            InitializeComponent();

            ip = defaultIP;
            IpAddressBox.Text = ip.ToString();
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            try
            {
                ip = IPAddress.Parse(IpAddressBox.Text);
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Incorrect IP Address");
            }
        }
    }
}
