using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wintogo
{
    public partial class ChoosePart : Form
    {
        //public static int part;
        public ChoosePart()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            InitializeComponent();
        }

        private void choosepart_Load(object sender, EventArgs e)
        {

            ////string choseWim = WTGOperation.imageFilePath;
            //if (!string.IsNullOrEmpty(choseWim))
            ////{
            //    string str=File.ReadAllText(@"c:\b.txt");
            //string str = @"Deployment Image Servicing and Management toolVersion: 6.3.9600.17031Details for image : E:\sources\install.wimIndex : 1Name : Windows 10 Pro Technical PreviewDescription : Windows 10 Pro Technical PreviewSize : 9, 338, 967, 521 bytesThe operation completed successfully.";

            //MessageBox.Show(mc[1].Value);

            //}
            numericUpDown1.Value = Int32.Parse(WTGOperation.wimPart);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WTGOperation.wimPart = numericUpDown1.Value.ToString();
            //part =(int) numericUpDown1.Value ;
            
            this.Close();
        }
    }
}
