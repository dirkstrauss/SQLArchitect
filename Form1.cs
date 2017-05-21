using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

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
            lblFileName.Text = "";
            lblFullOutputPath.Text = "";
            txtServerName.Text = Properties.Settings.Default.ServerName.ToString();
        }

        private async void btnReadFiles_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtSourcePath.Text))
            {
                if (Directory.Exists(txtOutputPath.Text))
                {
                    if (!(txtFileName.Text.Length == 0))
                    {
                        engine.DatabaseConnectionString = GetConnectionString();
                        engine.DatabaseName = txtDatabase.Text;
                        engine.FileName = txtFileName.Text;
                        await engine.ProcessFiles(@"C:\temp\CodeSmith");
                    }
                    else
                        MessageBox.Show("Please specify a file name");
                }
                else
                    MessageBox.Show("Please specify a valid output path");
            }
            else
                MessageBox.Show("Please specify a valid source path");
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

        #region Form Events
        private void rbUsernamePassword_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUsernamePassword.Checked == true)
            {
                txtUsername.ReadOnly = false;
                txtPassword.ReadOnly = false;
                Properties.Settings.Default.UsernameAndPassword = true;
            }
            else
            {
                txtUsername.Clear();
                txtPassword.Clear();
                txtUsername.ReadOnly = true;
                txtPassword.ReadOnly = true;
                Properties.Settings.Default.UsernameAndPassword = false;
            }
            Properties.Settings.Default.Save();
        }

        private void txtFileName_TextChanged(object sender, EventArgs e)
        {
            if (txtFileName.Text.Length > 0)
            {
                lblFileName.Text = $"The file will be created as: {txtFileName.Text}.sql";
                lblFullOutputPath.Text = Path.Combine(txtOutputPath.Text, $"{txtFileName.Text}.sql");
            }
            else
            {
                lblFileName.Text = "";
                lblFullOutputPath.Text = "";
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpAbout about = new HelpAbout();
            about.ShowDialog();
        }

        private void txtServerName_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ServerName = txtServerName.Text;
            Properties.Settings.Default.Save();
        }

        private void txtDatabase_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DatabaseName = txtDatabase.Text;
            Properties.Settings.Default.Save();
        }

        private void rbIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IntegratedSecurity = rbIntegratedSecurity.Checked;
            if (rbIntegratedSecurity.Checked)
            {
                txtUsername.Clear();
                txtPassword.Clear();
                Properties.Settings.Default.Username = "";
                Properties.Settings.Default.Password = "";
            }
            Properties.Settings.Default.Save();
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Username = txtUsername.Text;
            Properties.Settings.Default.Save();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Password = txtPassword.Text;
            Properties.Settings.Default.Save();
        }

        private void btnSelectSourceFolder_Click(object sender, EventArgs e)
        {
            txtSourcePath.Text = BrowseFolder();
        }

        private void btnSelectDestinationFolder_Click(object sender, EventArgs e)
        {
            txtOutputPath.Text = BrowseFolder();
        }
        #endregion

        private string BrowseFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            string selectedPath = "";
            if (result == DialogResult.OK)
            {
                selectedPath = fbd.SelectedPath;
            }
            return selectedPath;
        }
    }
}
