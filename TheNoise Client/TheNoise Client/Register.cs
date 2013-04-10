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
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            // get a ok from server that username and password can be created
            //if vailadated
            MessageBox.Show(" Congraulations! You Are Now Register.");
            this.DialogResult = DialogResult.OK;

            //if not vailadated
            //MessageBox.Show("Username not Avaiable. ");
            //newCPWBox.Clear();
            //newUserNameBox.Clear();
            //NewPasswordBox.Clear();
        }
    }
}
