using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lab_7_Finial_Project
{
    public partial class Listen : Form
    {
        public Listen()
        {
            InitializeComponent();
        }

        private void Listen_Load(object sender, EventArgs e)
        {

            //load log in form
            LogInSingUp login = new LogInSingUp();
            login.ShowDialog();

            //closes the main form
            if (login.DialogResult != DialogResult.OK)
            {
                this.Close();
            }
        }
    }
}
