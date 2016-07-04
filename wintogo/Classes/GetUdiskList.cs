using iTuner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace wintogo
{

    public static class GetUdiskList
    {
        private static Thread tListUDisks;
        static string currentList;//当前优盘列表
        public static UsbDiskCollection diskCollection = new UsbDiskCollection();

        #region Udisk
        public delegate void OutDelegate(bool isend, object dtSource,ComboBox combobox);
        public static  void OutText(bool isend, object dtSource, ComboBox comboBoxUd)
        {
            //MessageBox.Show("Test");
            if (comboBoxUd.InvokeRequired)
            {
                OutDelegate outdelegate = new OutDelegate(OutText);
                comboBoxUd.BeginInvoke(outdelegate, new object[] { isend, dtSource, comboBoxUd });
                return;
            }
            comboBoxUd.DataSource = null;
            comboBoxUd.DataSource = dtSource;
            
            if (comboBoxUd.Items.Count != 0)
            {
                comboBoxUd.SelectedIndex = 0;
            }
            if (isend)
            {
                comboBoxUd.SelectedIndex = comboBoxUd.Items.Count - 1;
            }
        }

        private static void GetUdiskInfo()
        {

            string newlist = string.Empty;
            UsbManager manager = new UsbManager();
            try
            {
                diskCollection.Clear();
                //UsbDiskCollection disks = manager.GetAvailableDisks();
                UsbDisk udChoose = new UsbDisk(MsgManager.GetResString("Msg_chooseud", MsgManager.ci));
                diskCollection.Add(udChoose);

                //if (disks == null) { return; }
                foreach (UsbDisk disk in manager.GetAvailableDisks())
                {
                    diskCollection.Add(disk);
                    newlist += disk.ToString();

                }
                if (newlist != currentList)
                {
                    currentList = newlist;
                    OutText(false, diskCollection, cbb);
                }
            }
            catch (Exception ex) { Log.WriteLog("Err_GetUdiskInfo", ex.ToString()); }
            finally
            {

                manager.Dispose();
            }

        }

        public static void LoadUDList(ComboBox comboBoxUd)
        {
            cbb = comboBoxUd;
            GetUdiskInfo();
            //Udlist = new Udlist();
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            timer1.Interval = 2000;
            timer1.Start();
        }
        static ComboBox cbb = null;
        private static void timer1_Tick(object sender, EventArgs e)
        {
            if (cbb.SelectedIndex == 0)
            {
                tListUDisks = new Thread(GetUdiskInfo);
                tListUDisks.Start();
            }
        }
        #endregion
    }

}
