using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;
using wintogo.Utility;

namespace wintogo
{
    public partial class ErrorMsg : Form
    {
        string errmsg;
        bool critical;
        public ErrorMsg(string errmsg, bool critical)
        {
            Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            this.errmsg = errmsg;
            this.critical = critical;
            InitializeComponent();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WebUtility.VisitWeb("http://bbs.luobotou.org/forum-88-1.html");
        }

        private void error_Load(object sender, EventArgs e)
        {
            Text += Application.ProductName + Application.ProductVersion;
            textBox1.Text = errmsg;
            Log.WriteLog("Info_ErrMsg", errmsg);
            
            if (critical)
            {
                try
                {
                    if (File.Exists(Environment.GetEnvironmentVariable("windir") + @"\Logs\DISM\dism.log"))
                        File.Copy(Environment.GetEnvironmentVariable("windir")+ @"\Logs\DISM\dism.log", WTGModel.logPath + "\\" + "dism.log", true);
                }
                catch (Exception ex)
                {
                    Log.WriteLog("Info_DISMLOG", ex.ToString());
                }

                //Upload ErrorLog
                Thread t = new Thread(UploadLogs);
                t.Start();
            }

        }
        private void UploadLogs()
        {
            try
            {
                string tmpFile = Path.GetTempFileName();
                ZipFile.CreateFromDirectory(WTGModel.logPath, tmpFile);

                //ZipHelper.ZipFileDirectory(WTGModel.logPath, tmpFile);
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
            WebUtility.VisitWeb("http://bbs.luobotou.org/thread-8670-1-1.html");
        }
    }
}
