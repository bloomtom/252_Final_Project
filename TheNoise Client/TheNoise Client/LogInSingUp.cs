using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheNoiseAPI;
using TheNoiseHLC;
using System.Net;
using TheNoiseHLC.CommunicationObjects.GlobalEnumerations;


namespace TheNoiseClient
{
    public partial class LogInSingUp : Form
    {
        private IPAddress ip;//declare signin ip
        private int port;//declare signin port

        private string username;
        public string Username
        {
            get { return username; }
            set {}
        }
        private string password;
        public string Password
        {
            get { return password; }
            set {}
        }

        public LogInSingUp(IPAddress ip, int port)//ip address and port
        {
            this.ip = ip;//set sign ip to passed ip
            this.port = port;//set log in port to passed port
            InitializeComponent();
        }

        // verify log in information when button clicked
        private void logInButton_Click(object sender, EventArgs e)
        {
            username = userNameBox.Text;
            password = passwordBox.Text;
         
            if (password == "" || username == "")// make sure textboxes are not empty
            {
                MessageBox.Show("Please enter information in the User Name and Password fields");
            }
            else//when information is correct
            {
            //connect to server
            UserAuthenticationResult result = UserAuthenticationResult.UnknownResult;
            try
            {
                // Open connection to server and attempt to validate the user.
                using (ServerConnection serverConnection = new ServerConnection(ip, port))
                {
                    //valadate log in information
                    serverConnection.OpenConnection();
                    result = serverConnection.Authenticate(username, password);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There was a problem connecting to the server.");
            }

            switch (result)
            {
                case UserAuthenticationResult.UnknownResult:
                    MessageBox.Show("I'm not sure what happened.");
                    break;
                case UserAuthenticationResult.Success://if log in info is valadated go to program

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
                case UserAuthenticationResult.InvalidUser://if username info not valadated

                    MessageBox.Show("User Name not Accepted");
                    userNameBox.Clear();
                    passwordBox.Clear();
                    break;
                case UserAuthenticationResult.InvalidPassword://if password info not valadated

                    MessageBox.Show("Password not Accepted");
                    userNameBox.Clear();
                    passwordBox.Clear();
                    break;
                default:
                    break;
                }
            }                   
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            //open register dialog box
            Register signup = new Register(ip,port);
            signup.ShowDialog();

            if (signup.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                username = signup.Username;
                password = signup.Password;
                this.Close();
            }
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                logInButton_Click(sender, e);
            }
        }

        private void userNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                logInButton_Click(sender, e);
            }
        }

        private void settings_Click(object sender, EventArgs e)
        {
            Settings ipinfo = new Settings();
            ipinfo.ShowDialog();

            if (ipinfo.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ip = ipinfo.IPAddressBox.Text;
            }
        }
    }
}
