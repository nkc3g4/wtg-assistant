using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace wintogo
{
    public partial class SetTempPath : Form
    {
        public static string temppath = System.Environment.GetEnvironmentVariable("TEMP");
        public SetTempPath()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (Directory .Exists (folderBrowserDialog1 .SelectedPath ))
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.EndsWith("\\"))
            {
                temppath = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                
            }
            else { temppath = textBox1.Text; }
            IniFile.WriteVal("Main", "TempPath", temppath, Application.StartupPath + "\\files\\settings.ini");
            this.Close();

        }

        private void SetTempPath_Load(object sender, EventArgs e)
        {
            textBox1.Text = temppath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            temppath = System.Environment.GetEnvironmentVariable("TEMP");
            textBox1.Text = temppath;

        }
    }
}
