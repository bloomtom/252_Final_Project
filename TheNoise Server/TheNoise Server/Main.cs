using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheNoise_DatabaseControl;

namespace TheNoise_Server
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataAccessLayer dbInterface = new DataAccessLayer("", "");
            User user = new User(usernameTextBox.Text, passwordTextBox.Text);
            UserValidationResult result = dbInterface.validateUser(user);

            if (result == UserValidationResult.Success)
            {
                MessageBox.Show("Yay");
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            DataAccessLayer dbInterface = new DataAccessLayer("", "");
            User user = new User(usernameTextBox.Text, passwordTextBox.Text);
            UserAddResult result = dbInterface.addUser(user);

            if (result == UserAddResult.Success)
            {
                MessageBox.Show("Yay");
            }
        }

        private void listButton_Click(object sender, EventArgs e)
        {
            DataAccessLayer dbInterface = new DataAccessLayer("", "");
            var users = new System.Collections.ObjectModel.ObservableCollection<User>();
            dbInterface.readUsers(users);

            usersListView.Clear();
            foreach (var item in users)
            {
                usersListView.Items.Add(item.Username);
            }
        }
    }
}
