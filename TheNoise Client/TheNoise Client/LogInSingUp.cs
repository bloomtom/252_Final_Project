using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheNoiseAPI;
using System.Net;

namespace Lab_7_Finial_Project
{
    public partial class LogInSingUp : Form
    {

        public LogInSingUp()
        {
            InitializeComponent();
        }

        // verify log in information
        private void logInButton_Click(object sender, EventArgs e)
        {
            //conect to server

            //valadate log in information
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            ServerConnection serverConnection = new ServerConnection(ip, 8000);

            string username = userNameBox.Text;
            string password = passwordBox.Text;

            serverConnection.Authenticate(username, password);
            

            //if log in info is valadated go to program
            this.DialogResult = DialogResult.OK;

            //if log in info not valadated
            //MessageBox.Show("Incorrect Log in");
            //userNameBox.Clear();
            //passwordBox.Clear();
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            //open register dialog box
            Register singup = new Register();
            singup.ShowDialog();
        }
    }
}
