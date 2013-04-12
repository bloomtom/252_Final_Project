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
    public partial class Register : Form
    {
        private IPAddress ip;
        private int port;

        private string username;
        public string Username
        {
            get { return username; }
            set { }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { }
        }

        public Register(IPAddress ip, int port)// sign in ip address and port
        {
            this.ip = ip;//set register ip to passed ip
            this.port = port;//set register port to passed port
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            password = NewPasswordBox.Text;
            username = newUserNameBox.Text;

            if (password == newCPWBox.Text)
            {
                UserAddResult result = UserAddResult.UnknownResult;
                try
                {
                    using (ServerConnection serverConnection = new ServerConnection(ip, port))
                    {
                        serverConnection.OpenConnection();
                        result = serverConnection.Register(username, password);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("There was a problem connecting to the server.");
                }

                switch (result)// get a resuslt from server that username and password can be created
                {
                    case UserAddResult.UnknownResult:

                        break;
                    case UserAddResult.Success://if vailadated

                        MessageBox.Show(" Congraulations! You Are Now Registered.");
                        this.DialogResult = DialogResult.OK;

                        break;
                    case UserAddResult.AlreadyExists://if user name is alredy in use
                        MessageBox.Show("Username not Avaiable. ");
                        newCPWBox.Clear();
                        newUserNameBox.Clear();
                        NewPasswordBox.Clear();

                        break;
                    case UserAddResult.InvalidPassword://if password is invalid password
                        MessageBox.Show("Invalied Password. ");
                        newCPWBox.Clear();
                        newUserNameBox.Clear();
                        NewPasswordBox.Clear();

                        break;
                    case UserAddResult.UsernameTooLong:// username to long
                        MessageBox.Show("User Name Too Long. ");
                        newCPWBox.Clear();
                        newUserNameBox.Clear();
                        NewPasswordBox.Clear();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                MessageBox.Show("The Password Field and Confirm Password Field do not Match./nPlease Try Again");
                NewPasswordBox.Clear();
                newCPWBox.Clear();
            }
        }
    }
}
