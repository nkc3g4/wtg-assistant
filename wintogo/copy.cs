using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
namespace wintogo
{
    public partial class copy : Form
    {
        Thread copyfile;
        String udisk;
        public copy(string ud)
        {
            InitializeComponent();
            udisk = ud;
        }

        private void copy_Load(object sender, EventArgs e)
        {
            copyfile = new Thread(new ThreadStart(copyfiles));
            copyfile.Start();
            
        
        }
        private void copyfiles() 
        {
            //MessageBox.Show("hekl");
            if (System.IO.File.Exists(Form1.vpath ))
            {
                System.Diagnostics.Process cp = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\fastcopy.exe", " /auto_close \"" + Form1.vpath+"\" /to=\"" + udisk + "\"");
                cp.WaitForExit();
            }
            if ((Form1.filetype == "vhd" && !Form1.vpath.EndsWith("win8.vhd")) || (Form1.filetype == "vhdx" && !Form1.vpath.EndsWith("win8.vhdx"))) 
            {
                //Rename
                try { File.Move(udisk + Form1.vpath.Substring(Form1.vpath.LastIndexOf("\\") + 1), udisk + Form1.win8vhdfile); }
                catch (Exception ex) { MessageBox.Show("重命名错误"+ex.ToString ()); }
            }
            //////////////////////////////////////////////////////////////
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("确认取消？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }

            System.Diagnostics.Process.Start("cmd.exe", "/c taskkill /f /IM fastcopy.exe");
        }

        private void win8PB1_Load(object sender, EventArgs e)
        {

        }
    }
}
