using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using wintogo.Utility;

namespace wintogo

{
    public partial class Finish : Form
    {
        public Finish(TimeSpan ts)
        {
            FormHelper.Closewp();
            Thread.CurrentThread.CurrentUICulture = MsgManager.ci;
            InitializeComponent();
            lblTime.Text = ts.ToString(@"hh\:mm\:ss");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebUtility.VisitWeb("https://bbs.luobotou.org/thread-804-1-1.html");

        }

        private void finish_Load(object sender, EventArgs e)
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    HttpPost.Post("https://laa.luobotou.org/wtgstats.ashx", new Dictionary<string, string>() {
                    {"type","finish" },
                    {"guid",WTGModel.CreateGuid },
                    {"timeElapsed",lblTime.Text}
                });
                }
                catch (Exception ex)
                {
                    Log.WriteLog("Err_finish", ex.Message);
                }
            });
            t.Start();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WebUtility.VisitWeb("https://bbs.luobotou.org/thread-5258-1-1.html");
            //System.Diagnostics.Process.Start("http://bbs.luobotou.org/thread-5258-1-1.html");
        }


    }
}
