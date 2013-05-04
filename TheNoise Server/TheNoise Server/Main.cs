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
using TheNoiseHLC;
using TheNoiseHLC.CommunicationObjects.GlobalEnumerations;
using TheNoise_Server.Properties;

namespace TheNoise_Server
{
    public partial class Main : Form
    {
        // List of users by IP+Port and index in the user listbox.
        private Dictionary<IPEndPoint, int> userList = new Dictionary<IPEndPoint,int>();

        private TheNoiseServer server;
        private IPAddress serverIP = IPAddress.Parse(Resources.defaultServerIP); // Default IP is loopback.
        private int serverPort = int.Parse(Resources.defaultServerPort);

        private IPAddress sqlIP = IPAddress.Parse(Resources.defaultSqlIP);
        private int sqlPort = int.Parse(Resources.defaultSqlPort);
        private string sqlUsername = string.Empty;
        private string sqlPassword = string.Empty;
        private bool sqlIntegratedSecurity = true;

        private string databaseName = Resources.sqlTable;

        private string audioPath = AppDomain.CurrentDomain.BaseDirectory + "audio\\";

        public Main()
        {
            InitializeComponent();

            server = new TheNoiseServer(serverIP, serverPort, audioPath);

            RefreshToolStrip();
        }

        private void RefreshToolStrip()
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
            server = new TheNoiseServer(serverIP, serverPort, audioPath);
            server.DatabaseAddress = sqlIP;
            server.DatabasePort = sqlPort;
            server.DatabaseName = databaseName;
            server.DatabaseUseIntegratedSecurity = sqlIntegratedSecurity;
            server.DatabaseUsername = sqlUsername;
            server.DatabasePassword = sqlPassword;

            // Set the events
            server.DataReceived += server_dataReceived;
            server.ClientConnected += server_clientConnected;
            server.ClientDisconnected += server_clientDisconnected;
            server.ClientAuthenticated += new EventHandler<ClientAuthEventArgs>(server_clientAuthenticated);
            server.GeneralEvent += server_GeneralEvent;
            // Start the server
            server.Start();
        }

        private void server_GeneralEvent(object sender, GeneralEventArgs e)
        {
            if (messagesRichTextBox.InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate // Invoke a generic delegate using MethodInvoker
                {
                    server_GeneralEvent(sender, e);
                }));
                return;
            }

            // The ToString() gives us a nice, friendly string to stick right on the UI.
            messagesRichTextBox.AppendText(e.ToString() + '\n');
        }

        private void StopServer()
        {
            // Unhook the client disconnect event while we close potentially hundreds of connections.
            server.ClientDisconnected -= server_clientDisconnected;

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

            // Remove this client from the UI.
            if (clientsListBox.Items.Contains(clientEndPoint.ToString()))
            {
                clientsListBox.Items.Remove(clientEndPoint.ToString());

                ASCIIEncoding encoder = new ASCIIEncoding();
                string leaveMessage = clientEndPoint.ToString() + " Left";
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

            // A client has connected, put the client in the UI for visualization.
            userList.Add(clientEndPoint, 0);
            clientsListBox.Items.Add(clientEndPoint.ToString());

            ASCIIEncoding encoder = new ASCIIEncoding();
            string joinMessage = clientEndPoint.ToString() + " Joined";
            messagesRichTextBox.AppendText(joinMessage + "\n");
        }

        private void server_clientAuthenticated(object sender, ClientAuthEventArgs e)
        {
            // Invoke on the listbox thread if needed (yes).
            if (clientsListBox.InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate // Invoke a generic delegate using MethodInvoker
                {
                    server_clientAuthenticated(sender, e);
                }));
                return;
            }

            messagesRichTextBox.AppendText(e.ClientIPE + " was authenticated as " + e.Username + '\n');
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
            // Start the server and set UI components.
            StartServer();
            RefreshToolStrip();
            messagesRichTextBox.AppendText("Started server on " + server.IP.ToString() + ":" + server.Port.ToString() + "\n");
        }

        private void stopServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Stop the server and set UI components.
            StopServer();
            clientsListBox.Items.Clear();
            RefreshToolStrip();
            messagesRichTextBox.AppendText("Server is stopped.\n");
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show a config form and retrieve user settings from it.
            ConfigForm config = new ConfigForm(serverIP, serverPort, sqlIP, sqlPort);
            config.ShowDialog();

            // Only get results if the user pressed OK.
            if (config.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                serverIP = config.IpAddressServer;
                serverPort = config.Port;
                sqlIP = config.IpAddressSQL;
                sqlPort = config.PortSQL;
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

        private void clientsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (server.Running)
            {
                dropClientToolStripMenuItem.Enabled = (clientsListBox.SelectedIndex != -1);
            }
        }
    }
}
