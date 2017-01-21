using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using wintogo.Utility;

namespace wintogo
{
    public partial class ErrorMsg : Form
    {
        string errmsg;
        public ErrorMsg(string errmsg)
        {
            Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            this.errmsg = errmsg;
            InitializeComponent();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1.VisitWeb("http://bbs.luobotou.org/forum-88-1.html");
        }

        private void error_Load(object sender, System.EventArgs e)
        {
            this.Text += Application.ProductName + Application.ProductVersion;
            label1.Text += errmsg;
            Log.WriteLog("Info_ErrMsg", errmsg);
            //Upload ErrorLog
            Thread t = new Thread(UploadLogs);
            t.Start();

        }
        private void UploadLogs()
        {
            try
            {

                string tmpFile = Path.GetTempFileName();
                ZipHelper.ZipFileDirectory(WTGModel.logPath, tmpFile);
                HttpPost.HttpUploadFile(@"http://laa.luobotou.org/wtgreport.ashx", tmpFile);
                File.Delete(tmpFile);



                HttpPost.Post("http://laa.luobotou.org/wtgstats.ashx", new Dictionary<string, string>() {
                    {"type","error" },
                    {"guid",WTGModel.CreateGuid },
                    {"errorMsg",errmsg}
                });


            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_UploadLogs", ex.ToString());
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void label3_Click(object sender, System.EventArgs e)
        {
            Form1.VisitWeb("http://bbs.luobotou.org/thread-8670-1-1.html");
        }
    }
}
