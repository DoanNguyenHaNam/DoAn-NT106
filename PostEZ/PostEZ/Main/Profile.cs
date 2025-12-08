using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostEZ.Main
{
    public partial class Profile : Form
    {
        private readonly string _username;
        public Profile(string username)
        {
            InitializeComponent();
            _username = username;
        }

        private void Profile_Load(object sender, EventArgs e)
        {

        }
    }
}
