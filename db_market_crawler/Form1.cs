using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mm_db_market
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            forms.ucNShop myUserControl = new forms.ucNShop();
            myUserControl.Dock = DockStyle.Fill;
            panelMain.Controls.Add(myUserControl);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();

            if (tcMain.SelectedIndex == 0)
            {
                forms.ucNShop myUserControl = new forms.ucNShop();
                myUserControl.Dock = DockStyle.Fill;
                panelMain.Controls.Add(myUserControl);
            }
            else if (tcMain.SelectedIndex == 1)
            {
                forms.ucCoupang myUserControl = new forms.ucCoupang();
                myUserControl.Dock = DockStyle.Fill;
                panelMain.Controls.Add(myUserControl);
            }
            else if (tcMain.SelectedIndex == 2)
            {
                forms.ucAuction myUserControl = new forms.ucAuction();
                myUserControl.Dock = DockStyle.Fill;
                panelMain.Controls.Add(myUserControl);
            }
            else if (tcMain.SelectedIndex == 3)
            {
                forms.ucGmarket myUserControl = new forms.ucGmarket();
                myUserControl.Dock = DockStyle.Fill;
                panelMain.Controls.Add(myUserControl);
            }
            else if (tcMain.SelectedIndex == 4)
            {
                forms.uc11st myUserControl = new forms.uc11st();
                myUserControl.Dock = DockStyle.Fill;
                panelMain.Controls.Add(myUserControl);
            }
        }
    }
}
