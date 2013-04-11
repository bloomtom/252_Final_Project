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

        // verify log in information
        private void logInButton_Click(object sender, EventArgs e)
        {
            //conect to server
            UserAuthenticationResult result = UserAuthenticationResult.UnknownResult;
            using (ServerConnection serverConnection = new ServerConnection(ip, port))
            {
                //valadate log in information
                username = userNameBox.Text;
                password = passwordBox.Text;

                result = serverConnection.Authenticate(username, password);
            }

            switch (result)
            {
                case UserAuthenticationResult.UnknownResult:
                    MessageBox.Show("I'm not sure what happened.");
                    break;            
                case UserAuthenticationResult.Success://if log in info is valadated go to program
                   
                    this.DialogResult = DialogResult.OK;
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
    }
}
