using L4D2Cheat.L4D2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace L4D2Cheat
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if(Process.GetProcessesByName("left4dead2").Length != 0) // if "l4d2.exe" running
            {
                Globals.Start(); // start cheat
                MessageBox.Show("Cheat has been started", "L4D2 SDK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cbAutoBunny_CheckedChanged(object sender, EventArgs e)
        {
            Settings.blAutoBunny = cbAutoBunny.Checked;
        }
    }
}
