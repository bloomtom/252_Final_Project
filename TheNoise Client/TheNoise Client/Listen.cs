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
using TheNoiseHLC.CommunicationObjects.AudioTrack;

namespace TheNoiseClient
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
            while (true)
            {
                LogInSingUp login = new LogInSingUp(ip, port);
                //load log in form
                login.ShowDialog();

                //closes the main form
                if (login.DialogResult == DialogResult.OK)
                {
                    username = login.Username;
                    password = login.Password;

                    UserAuthenticationResult result = UserAuthenticationResult.UnknownResult;
                    try
                    {

                        // Open connection to server and attempt to validate the user.
                        serverConnection = new ServerConnection(ip, port);
                        serverConnection.AudioListReceived += new ServerConnection.TrackListEventHandler(serverConnection_AudioListReceived);
                        serverConnection.AudioPacketReceived += new ServerConnection.DataReceivedEventHandler(serverConnection_AudioPacketReceived);
                        //valadate log in information
                        serverConnection.OpenConnection();
                        result = serverConnection.Authenticate(username, password);

                        if (result == UserAuthenticationResult.Success)
                        {
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("There was a problem connecting to the server.");
                    }

                    if (result != UserAuthenticationResult.Success)
                    {
                        MessageBox.Show("Connection or validation error.");
                    }

                }
                else
                {
                    this.Close();
                    break;
                }
            }
        }

        private void serverConnection_AudioPacketReceived(object sender, byte[] e)
        {

        }

        private void serverConnection_AudioListReceived(object sender, TrackList e)
        {

        }
    }
}
