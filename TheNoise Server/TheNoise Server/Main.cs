using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using TcpTransmission;
using TcpTransmission.Server;
using TheNoise_DatabaseControl;
using TheNoise_SharedObjects;
using TheNoise_SharedObjects.GlobalEnumerations;

namespace TheNoise_Server
{
    public partial class Main : Form
    {
        private ServerManager server;
        private IPAddress serverIP = IPAddress.Parse("0.0.0.0"); // Default IP is loopback.
        private int serverPort = 9734;

        public Main()
        {
            InitializeComponent();

            server = new ServerManager(serverIP, serverPort);

            RefreshToolstrip();
        }

        private void RefreshToolstrip()
        {
            bool serverIsRunning = server.Running;

            // These menu items only enabled when server is running.
            stopServerToolStripMenuItem.Enabled = serverIsRunning;
            clientsToolStripMenuItem.Enabled = serverIsRunning;

            // These menu items only enabled when server is not running.
            startServerToolStripMenuItem.Enabled = !serverIsRunning;
            configureToolStripMenuItem.Enabled = !serverIsRunning;
        }

        private void StartServer()
        {
            // Create a new server
            server = new ServerManager(serverIP, serverPort);
            // Set the events
            server.dataReceived += server_dataReceived;
            server.clientConnected += server_clientConnected;
            server.clientDisconnected += server_clientDisconnected;
            // Start the server
            server.Start();
        }

        private void StopServer()
        {
            // Unhook the client disconnect event while we close potentially hundreds of connections.
            server.clientDisconnected -= server_clientDisconnected;

            // Send a message to all clients that the server is going down.
            ASCIIEncoding encoder = new ASCIIEncoding();
            server.Broadcast(encoder.GetBytes("The server is closing."));

            server.Stop();
        }

        private void server_clientDisconnected(object sender, IPEndPoint clientEndPoint)
        {
            // Invoke on the listbox thread if needed (yes).
            if (clientsListBox.InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate // Invoke a generic delegate using MethodInvoker
                {
                    server_clientDisconnected(sender, clientEndPoint);
                }));
                return;
            }

            if (clientsListBox.Items.Contains(clientEndPoint.ToString()))
            {
                clientsListBox.Items.Remove(clientEndPoint.ToString());

                ASCIIEncoding encoder = new ASCIIEncoding();
                string leaveMessage = clientEndPoint.ToString() + " Left";
                server.Broadcast(encoder.GetBytes(leaveMessage));
                messagesRichTextBox.AppendText(leaveMessage + "\n");
            }
        }

        private void server_clientConnected(object sender, IPEndPoint clientEndPoint)
        {
            // Invoke on the listbox thread if needed (yes).
            if (clientsListBox.InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate // Invoke a generic delegate using MethodInvoker
                {
                    server_clientConnected(sender, clientEndPoint);
                }));
                return;
            }

            clientsListBox.Items.Add(clientEndPoint.ToString());

            ASCIIEncoding encoder = new ASCIIEncoding();
            string joinMessage = clientEndPoint.ToString() + " Joined";
            server.Broadcast(encoder.GetBytes(joinMessage));
            messagesRichTextBox.AppendText(joinMessage + "\n");
        }

        private void server_dataReceived(object sender, IncomingMessageEventArgs e)
        {
            // Invoke on the listbox thread if needed (yes).
            if (messagesRichTextBox.InvokeRequired)
            {
                messagesRichTextBox.Invoke(new MethodInvoker(delegate // Invoke a generic delegate using MethodInvoker
                {
                    server_dataReceived(sender, e);
                }));
                return;
            }

            if (e == null)
            {
                // This is probably a disconnect.
                return;
            }

            if (e.Error != null)
            {
                var errClient = (TcpTransmission.Client.ClientConnection)sender;
                messagesRichTextBox.AppendText("Error on client " + errClient.RemoteEndPoint.ToString() + " : " + e.Error.Message + "\n");
            }

            if (e.Message != null)
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                string source = ((TcpTransmission.Client.ClientConnection)sender).RemoteEndPoint.ToString();
                string message = encoder.GetString(e.Message, 0, e.Message.Length);

                messagesRichTextBox.AppendText(source + ": " + message + "\n");

            }
        }

        private void startServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartServer();
            RefreshToolstrip();
            messagesRichTextBox.AppendText("Started server on " + server.IP.ToString() + ":" + server.Port.ToString() + "\n");
        }

        private void stopServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopServer();
            clientsListBox.Items.Clear();
            RefreshToolstrip();
            messagesRichTextBox.AppendText("Server is stopped.\n");
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show a config form and retrieve user settings from it.
            ConfigForm config = new ConfigForm(serverIP, serverPort);
            config.ShowDialog();

            // Only get results if the user pressed OK.
            if (config.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                serverIP = config.IpAddress;
                serverPort = config.Port;
            }
        }

        private void dropClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectedItem = clientsListBox.SelectedIndex;

            if (selectedItem != -1)
            {
                // Disconnect the selected client.
                IPEndPoint clientDrop = IPEndPointParser.Parse(clientsListBox.SelectedItem.ToString());
                server.DropClient(clientDrop);
            }
        }

        private void messagesRichTextBox_TextChanged(object sender, EventArgs e)
        {
            messagesRichTextBox.ScrollToCaret();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (server != null) { server.Dispose(); }
        }
    }
}
