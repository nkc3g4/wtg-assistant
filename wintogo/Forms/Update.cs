using System;
using System.Windows.Forms;
//using Microsoft.Win32;
namespace wintogo

{
    public partial class Update : Form
    {
        string args = null;
        public Update(string args)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            InitializeComponent();
            this.args = args;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true) { button1.Enabled = false; }
            if (checkBox1.Checked == false) { button1.Enabled = true; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                IniFile.WriteVal("Main", "AutoUpdate", "0", Application.StartupPath + "\\files\\settings.ini");
                //WTRegedit("nevercheckupdate", "1");
            }
            this.Close();
        }
        //private void WTRegedit(string name, string tovalue)
        //{
        //    RegistryKey hklm = Registry.CurrentUser ;
        //    RegistryKey software = hklm.OpenSubKey("SOFTWARE", true);
        //    RegistryKey aimdir = software.CreateSubKey(Application.ProductName);
        //    aimdir.SetValue(name, tovalue);
        //}

        private void update_Load(object sender, EventArgs e)
        {
            label1.Text += args;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebUtility.VisitWeb("http://bbs.luobotou.org/thread-761-1-1.html");
            this.Close();
        }
    }
}
