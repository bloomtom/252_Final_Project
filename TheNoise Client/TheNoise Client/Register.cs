using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheNoiseAPI;
using TheNoiseHLC.CommunicationObjects;
using System.Net;
using TheNoiseHLC.CommunicationObjects.GlobalEnumerations;

namespace TheNoiseClient
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
            int Num = 0;
            int Lett = 0;
            for (int i = 0; i < password.Length; i++)
            {
                if (char.IsNumber(password[i]))
                {
                    Num++;
                }
                else if (char.IsLetter(password[i]))
                {
                    Lett++;
                }

            }


            if (username == "" || password == "" || newCPWBox.Text == "")//checks for text in feilds
            {
                MessageBox.Show("Please enter information in the User Name, Password and Confirm Password Fields");
            }
            else if (password.Length < 6)//checks if pass word is longer then seven
            {
                MessageBox.Show("Password needs to be longer then 6 characters");
                NewPasswordBox.Clear();
                newCPWBox.Clear();
            }
            else if (Num < 2)
            { 
                MessageBox.Show("Need at least 2 numbers in password");
                NewPasswordBox.Clear();
                newCPWBox.Clear();
            
            }
            else if (Lett < 3)
            { 
                MessageBox.Show("Need at least 3 letters in password");
                NewPasswordBox.Clear();
                newCPWBox.Clear();
            
            }
            else if (password != newCPWBox.Text)//makes sure password is enter correct
            {
                MessageBox.Show("The Password Field and Confirm Password Field do not Match.\nPlease Try Again");
                NewPasswordBox.Clear();
                newCPWBox.Clear();
            }
            else
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
        }

        private void newCPWBox_KeyDown(object sender, KeyEventArgs e)//varify the information with a enter hit
        {
            if (e.KeyValue == 13)
            {
                registerButton_Click(sender, e);
            }
        }

        private void NewPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                registerButton_Click(sender, e);
            }
        }

        private void newUserNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                registerButton_Click(sender, e);
            }
        }
    }
}
