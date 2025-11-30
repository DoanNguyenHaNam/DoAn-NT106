using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostEZ.Log;

namespace PostEZ.Main
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            Login.LoadFromUrl("https://pminmod.site/doannt106/logo.png", pic_logo);
        }

        private void btn_main_Click(object sender, EventArgs e)
        {

        }
    }
}
