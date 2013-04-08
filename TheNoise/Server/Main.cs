using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheNoise_DatabaseControl;

namespace Server
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataAccessLayer validator = new DataAccessLayer("", "");
            User user = new User(textBox1.Text, textBox2.Text);
            UserValidationResult result = validator.validateUser(user);

            if (result == UserValidationResult.Success)
            {
                MessageBox.Show("Yay");
            }
        }
    }
}
