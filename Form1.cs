using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLArchitect
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void btnReadFiles_Click(object sender, EventArgs e)
        {
            engine.DatabaseConnectionString = GetConnectionString();
            engine.DatabaseName = txtDatabase.Text;
            await engine.ProcessFiles(@"C:\temp\CodeSmith");
        }


        private string GetConnectionString()
        {
            string strConnnection = $"Data Source={txtServerName.Text};Initial Catalog={txtDatabase.Text}; Connect Timeout=15; ";

            if (rbIntegratedSecurity.Checked == true)
                strConnnection += "Integrated Security=SSPI;";

            if (rbUsernamePassword.Checked == true)
                strConnnection += $"User ID={txtUsername.Text}; Password={txtPassword.Text};";

            return strConnnection;
        }

        private void rbUsernamePassword_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUsernamePassword.Checked == true)
            {
                txtUsername.ReadOnly = false;
                txtPassword.ReadOnly = false;
            }
            else
            {
                txtUsername.Clear();
                txtPassword.Clear();
                txtUsername.ReadOnly = true;
                txtPassword.ReadOnly = true;
            }
        }
    }
}
