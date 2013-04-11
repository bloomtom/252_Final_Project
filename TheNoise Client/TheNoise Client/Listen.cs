using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheNoiseAPI;
using TheNoise_SharedObjects.GlobalEnumerations;
using System.Net;

namespace Lab_7_Finial_Project
{
    public partial class Listen : Form
    {
        private IPAddress ip = IPAddress.Parse("127.0.0.1");//set ip address
        private int port = 9734;//set port number

        private string username;
        private string password;

        ServerConnection serverConnection;

        public Listen()
        {
            InitializeComponent();
        }

        private void Listen_Load(object sender, EventArgs e)
        {

            //load log in form
            LogInSingUp login = new LogInSingUp(ip, port);
            login.ShowDialog();

            //closes the main form
            if (login.DialogResult == DialogResult.OK)
            {
                username = login.Username;
                password = login.Username;

                //conect to server
                serverConnection = new ServerConnection(ip, port);

                if (serverConnection.Authenticate(username, password) == UserAuthenticationResult.Success)
                {

                }
                else
                {
                    MessageBox.Show("Unable to connect. ");
                }


            }
            else
            {
                this.Close();
            }
        }
    }
}
