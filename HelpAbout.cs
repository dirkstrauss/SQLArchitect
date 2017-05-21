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
    public partial class HelpAbout : Form
    {
        public HelpAbout(string param = "")
        {

        }

        private void About_Load(object sender, EventArgs e)
        {
            lblAuthor.Text = $"Dirk Strauss {DateTime.Now.Year}";
        }
    }
}
