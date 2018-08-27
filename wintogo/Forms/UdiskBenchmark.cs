using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace wintogo.Forms
{
    public partial class UdiskBenchmark : Form
    {
        private int lev = 0;
        private string udString = string.Empty;
        public UdiskBenchmark(string ud,int level)
        {
            Thread.CurrentThread.CurrentUICulture = MsgManager.ci;
            InitializeComponent();
            Text = ud;
            lev = level;
        }
        
        private void UdiskBenchmark_Load(object sender, EventArgs e)
        {
            string ln = "Error";
            Color lc = Color.Yellow;
            if (lev == 1)
            {
                ln = "Steel";
                lc = Color.SteelBlue;
            }
            else if(lev == 2)
            {
                ln = "Bronze";
                lc = Color.Crimson;
            }else if(lev == 3)
            {
                ln = "Silver";
                lc = Color.Silver;
            }else if (lev == 4)
            {
                ln = "Gold";
                lc = Color.Gold;
            }
            else if (lev == 5)
            {
                ln = "Platinum";
                lc = Color.White;
            }
            labelLevel.Text = ln;
            labelLevel.ForeColor = lc;
            //labelLevel.Location.X = Width / 2 - labelLevel.Width / 2;
            labelLevel.Location = new Point(Width / 2 - labelLevel.Width / 2, labelLevel.Location.Y);
            button1.Location = new Point(Width / 2 - button1.Width / 2, button1.Location.Y);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
