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
using AwesomeAudio;
using NAudio;

namespace TheNoiseClient
{
    public partial class Listen : Form
    {
        private IPAddress ip = IPAddress.Parse("172.0.0.1");//set ip address
        private int port = 9734;//set port number
        private TrackList tracks;// copy of TrackList

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
                    ip = login.IP;
        

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
                            serverConnection.RequestAudioList();
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
            if (musicFilesListBox.InvokeRequired)
            {
                tracks = e;

                Invoke(new MethodInvoker(delegate // Invoke a generic delegate using MethodInvoker
                {
                    musicFilesListBox.Items.Clear();
                    for (int i = 0; i < e.Tracks.Length; i++)
                    {
                        musicFilesListBox.Items.Add(e.Tracks[i].ToString());
                    }
                }));
                return;
            }
        }

        private void musicFilesListBox_DoubleClick(object sender, EventArgs e)
        {
            TrackStreamRequestResult result = serverConnection.StartAudioStream(tracks.Tracks[musicFilesListBox.SelectedIndex], new IPEndPoint(ip, 9001));
            switch (result)
            {
                case TrackStreamRequestResult.Success:
                    try
                    {
                        AudioPlayer noiseMaker = new AudioPlayer();
                        noiseMaker.StartPosition = FormStartPosition.CenterScreen;
                        noiseMaker.Show();
                    }
                    catch
                    {
                        MessageBox.Show("Dun Dun Dun!!!!!\nI am the Socket Troll and I hate your music");
                    }
                    break;
                case TrackStreamRequestResult.InvalidFileName:
                    MessageBox.Show("Invalid Track");
                    break;
                case TrackStreamRequestResult.InvalidConnection:
                    MessageBox.Show("Invalid Connection");
                    break;
                case TrackStreamRequestResult.UnknownResult:
                    MessageBox.Show("Failure");
                    break;
                default:
                    break;
            }
        }

        private void musicFilesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (musicFilesListBox.SelectedIndex == -1)
            {

            }
            else
            {

                Namelabel.Text = tracks.Tracks[musicFilesListBox.SelectedIndex].TrackName;
                TimeLabel.Text = tracks.Tracks[musicFilesListBox.SelectedIndex].TrackLength.ToString();


            }
        }
    }
}
