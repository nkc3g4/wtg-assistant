using System;
using System.Threading;
using System.Windows.Forms;

namespace wintogo.Forms
{
    public partial class FormatAlert : Form
    {
        Thread t;
        public FormatAlert(string msg)
        {
            Thread.CurrentThread.CurrentUICulture = MsgManager.ci;
            InitializeComponent();
            lblTxt.Text = msg;
            DialogResult = DialogResult.No;
        }

        private void FormatAlert_Load(object sender, EventArgs e)
        {

            
            t = new Thread(() =>
            {

                string oriText = btnOk.Text;
                for (int i = 1; i >= 1; i--)
                {
                    btnOk.Invoke(new Action(() => { btnOk.Text = oriText+"(" + i.ToString() + ")"; }));
                    Thread.Sleep(1000);
                }
                btnOk.Invoke(new Action(() =>
                {
                    btnOk.Text = oriText;
                    btnOk.Enabled = true;
                }));

            });
            t.Start();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void FormatAlert_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (t != null)
                {
                    if (t.IsAlive)
                    {
                        t.Abort();
                    }
                }
            }
            catch { }
        }
    }
}
