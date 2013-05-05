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
        private IPAddress ip = IPAddress.Parse("127.0.0.1");//set ip address
        private int port = 9734;//set port number
        private TrackList tracks;// copy of TrackList

        private string username;
        private string password;

        ServerConnection serverConnection;
        AudioPlayer noiseMaker;

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
                    trackCountLabel.Text = "Tracks: " + musicFilesListBox.Items.Count;
                }));
                return;
            }
        }

        private void musicFilesListBox_DoubleClick(object sender, EventArgs e)
        {
            if (TryCheckConnection())
            {
                TrackStreamRequestResult result = serverConnection.StartAudioStream(tracks.Tracks[musicFilesListBox.SelectedIndex], new IPEndPoint(ip, 9001));
                switch (result)
                {
                    case TrackStreamRequestResult.Success:
                        try
                        {
                            noiseMaker = new AudioPlayer();
                            noiseMaker.StartPosition = FormStartPosition.CenterScreen;
                            noiseMaker.Show();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Dun Dun Dun!!!!!\nI am the Socket Troll and I hate your music.\n" + ex.ToString());
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
        }

        private void musicFilesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (musicFilesListBox.SelectedIndex > -1)
            {
                Namelabel.Text = tracks.Tracks[musicFilesListBox.SelectedIndex].TrackName;
                TimeLabel.Text = tracks.Tracks[musicFilesListBox.SelectedIndex].TrackLength.ToString();
            }
        }

        private void refreshTracksButton_Click(object sender, EventArgs e)
        {
            if (TryCheckConnection())
            {
                serverConnection.RequestAudioList();
            }
        }

        private bool TryCheckConnection()
        {
            if (serverConnection.Connected)
            {
                return true;
            }

            try
            {
                serverConnection = new ServerConnection(ip, port);
                serverConnection.AudioListReceived += new ServerConnection.TrackListEventHandler(serverConnection_AudioListReceived);
                serverConnection.AudioPacketReceived += new ServerConnection.DataReceivedEventHandler(serverConnection_AudioPacketReceived);
                serverConnection.OpenConnection();

                if (serverConnection.Authenticate(username, password) == UserAuthenticationResult.Success)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem with the connection to the audio server: \n" + ex.Message);
            }

            return false;
        }

        private void Listen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverConnection != null)
            {
                serverConnection.CloseConnection();
            }

            if (noiseMaker != null && !(noiseMaker.Disposing || noiseMaker.IsDisposed))
            {
                noiseMaker.Dispose();
            }
        }
    }
}
