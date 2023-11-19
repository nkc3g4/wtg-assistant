using iTuner;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using wintogo.Forms;
using wintogo.Utility;

namespace wintogo
{
    public partial class Form1 : Form
    {

        bool autoCheckUpdate = true;
        private Thread tWrite;

        Stopwatch writeSw = new Stopwatch();
        private int udSizeInMB = 0;
        private readonly string releaseUrl = "https://bbs.luobotou.org/app/wintogo.txt";
        List<Control> formControlList = new List<Control>();

        bool useiso = false;
        string isoPath = string.Empty;

        public Form1()
        {
            Config.ReadConfigFile(ref autoCheckUpdate);
            InitializeComponent();
        }





        private void SystemDetection(bool isInitialization)
        {

            checkBoxBitlocker.Enabled = false;
            if (!GetDotNetVersion.IsCorrectVersion())
            {
                ErrorMsg em = new ErrorMsg(MsgManager.GetResString("Msg_DotnetVersionError"), true);
                em.ShowDialog();
                Environment.Exit(0);
            }
            string osVersionStr = Environment.OSVersion.ToString();
            if (osVersionStr.Contains("6.1"))//WIN7 禁用功能
            {
                ErrorMsg em = new ErrorMsg("不支持Windows7系统！" + Environment.NewLine + "Windows7 is no longer supported!", false);
                em.ShowDialog();
                Environment.Exit(0);

                checkBoxUefigpt.Enabled = true;
                checkBoxUefimbr.Enabled = true;
                radiobtnLegacy.Enabled = true;
                labelDisFunc.Visible = true;
                radiobtnVirtualDisk.Enabled = false;
                WTGModel.CurrentOS = OS.Win7;
                tabPageBackup.Parent = null;
            }
            else if (osVersionStr.Contains("6.2") || osVersionStr.Contains("6.3"))//WIN8及以上
            {
                radiobtnLegacy.Enabled = true;
                //radiobtnVhd.Enabled = true;
                radiobtnVirtualDisk.Enabled = true;
                checkBoxUefigpt.Enabled = true;
                checkBoxUefimbr.Enabled = true;
                //WIN8.1 UPDATE1 WIMBOOT  已修复WIN10版本号问题
                try
                {
                    string dismversion = FileOperation.GetFileVersion(Environment.GetEnvironmentVariable("windir") + "\\System32\\dism.exe");

                    WTGModel.dismversion = new Version(Regex.Match(dismversion, @"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+").Value);
                    if (dismversion.Substring(0, 14) == "6.3.9600.17031" || dismversion.Substring(0, 3) == "6.4")
                    {
                        radiobtnLegacy.Enabled = true;
                        //radiobtnVhd.Enabled = true;
                        radiobtnVirtualDisk.Enabled = true;
                        checkBoxUefigpt.Enabled = true;
                        checkBoxUefimbr.Enabled = true;
                        //checkBoxWimboot.Enabled = true;
                        WTGModel.allowEsd = true;
                        labelDisFunc.Visible = true;
                        //labelDisFuncEM.Visible = true;
                        WTGModel.CurrentOS = OS.Win8_1_with_update;

                    }
                    else if (dismversion.Substring(0, 3) == "10.")
                    {
                        radiobtnLegacy.Enabled = true;
                        //radiobtnVhd.Enabled = true;
                        radiobtnVirtualDisk.Enabled = true;
                        checkBoxUefigpt.Enabled = true;
                        checkBoxUefimbr.Enabled = true;
                        //checkBoxWimboot.Enabled = true;
                        //checkBoxCompactOS.Enabled = true;
                        WTGModel.allowEsd = true;
                        WTGModel.CurrentOS = OS.Win10;

                    }
                    else
                    {
                        radiobtnLegacy.Enabled = true;
                        //radiobtnVhd.Enabled = true;
                        radiobtnVirtualDisk.Enabled = true;
                        checkBoxUefigpt.Enabled = true;
                        checkBoxUefimbr.Enabled = true;

                        WTGModel.CurrentOS = OS.Win8_without_update;
                    }

                    //radiobtnLegacy.Checked = true;

                }
                catch (Exception ex)
                {
                    ErrorMsg em = new ErrorMsg(ex.Message, true);
                    em.Show();
                }
            }

        }

        #region MenuItemClick/Miscellaneous
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            WebUtility.VisitWeb("https://bbs.luobotou.org/thread-2427-1-1.html");


        }


        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string adLink = linkLabel2.Tag as string;
            if (!string.IsNullOrEmpty(adLink)) WebUtility.VisitWeb(adLink);
        }




        private void 萝卜头IT论坛ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebUtility.VisitWeb("https://bbs.luobotou.org");
        }

        private void checkBoxuefi_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUefimbr.Checked && checkBoxUefigpt.Checked)
            {
                checkBoxUefimbr.Checked = false;
                checkBoxUefigpt.Checked = true;
            }
            if (checkBoxUefigpt.Checked || checkBoxUefimbr.Checked)
            {
                checkBoxDiskpart.Checked = true;
            }
            checkBoxDiskpart.Enabled = !checkBoxUefigpt.Checked;
            //checkBoxDiskpart.Checked = checkBoxUefigpt.Checked;
            if (checkBoxBitlocker.Checked)
            {
                radiobtnLegacy.Checked = true;
                //radiobtnVhd.Enabled = false;
                radiobtnVirtualDisk.Enabled = false;
            }

            if ((checkBoxUefigpt.Checked || checkBoxUefimbr.Checked) && !radiobtnVirtualDisk.Checked)
            {
                checkBoxBitlocker.Enabled = true;
            }
            else
            {
                checkBoxBitlocker.Checked = false;
                checkBoxBitlocker.Enabled = false;
            }
        }





        private void 在线帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebUtility.VisitWeb("https://bbs.luobotou.org/thread-2427-1-1.html");

        }

        private void 官方论坛ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebUtility.VisitWeb("https://bbs.luobotou.org/forum-88-1.html");

        }

        private void diskpart重新分区ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MsgManager.getResString("Msg_chooseud")
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            //WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            //MsgManager.getResString("Msg_XPNotCOMP")
            //XP系统不支持此操作
            //MsgManager.getResString("Msg_ClearPartition")
            //此操作将会清除移动磁盘所有分区的所有数据，确认？
            if (System.Environment.OSVersion.ToString().Contains("5.1") || System.Environment.OSVersion.ToString().Contains("5.2")) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ClearPartition", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }

            //if (DialogResult.No == MessageBox.Show("您确定要继续吗？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; } 
            //Msg_Complete
            try
            {
                int targetIndex = int.Parse(WTGModel.UdObj.Index);
                DiskOperation.RepartitionAndAutoAssignDriveLetter(WTGModel.UdObj.Index);
                //DiskOperation.DiskPartRePartitionUD(WTGModel.partitionSize);
                //diskPart();
                FormHelper.Closewp();
                MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
                comboBoxUd.SelectedIndex = 0;
                GetUdiskList.GetUdiskInfo();
                //Console.WriteLine(GetUdiskList.diskCollection[targetIndex]); 
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_Exception", ex.ToString());

                ErrorMsg em = new ErrorMsg(ex.Message, true);
                em.Show();
            }

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void 打开程序运行目录ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath);
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox abx = new AboutBox();
            abx.Show();
        }



        private void 自动检查更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SWOnline swo = new SWOnline(releaseUrl);
            Thread threadUpdate = new Thread(swo.Update);
            threadUpdate.Start();
            //若无弹出窗口，则当前程序已是最新版本！
            MessageBox.Show(MsgManager.GetResString("Msg_UpdateTip", MsgManager.ci));
        }

        private void checkBoxdiskpart_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDiskpart.Checked)
            {
                if (checkBoxNoFormat.Checked)
                {
                    MessageBox.Show(MsgManager.GetResString("Msg_SelectBothNoformatAndRepartition", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    checkBoxDiskpart.Checked = false;
                    return;
                }
                //Msg_Repartition
                //MessageBox.Show(MsgManager.GetResString("Msg_Repartition", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                WTGModel.doNotFormat = false;
                //checkBoxDoNotFormat.Checked = false;
                //checkBoxDoNotFormat.Enabled = false;
            }
            else
            {
                //checkBoxDoNotFormat.Enabled = true;
            }
        }

        private void comboBox1_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip1.SetToolTip(this.comboBoxUd, comboBoxUd.SelectedItem.ToString()); ;
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_comboBox1_MouseHover", ex.ToString());
            }
        }

        private void bOOTICEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(WTGModel.applicationFilesPath + "\\BOOTICE.EXE");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(WTGModel.logPath);

        }

        private void checkBoxuefimbr_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUefigpt.Checked && checkBoxUefimbr.Checked) { checkBoxUefigpt.Checked = false; checkBoxUefimbr.Checked = true; }
            if (checkBoxUefimbr.Checked) { WTGModel.disableWinRe = true; }
            if (checkBoxUefimbr.Checked)
            {
                checkBoxDiskpart.Checked = true;
            }
            checkBoxDiskpart.Enabled = !checkBoxUefimbr.Checked;
            if (checkBoxBitlocker.Checked)
            {
                radiobtnLegacy.Checked = true;
                //radiobtnVhd.Enabled = false;
                radiobtnVirtualDisk.Enabled = false;
            }

            if ((checkBoxUefigpt.Checked || checkBoxUefimbr.Checked) && !radiobtnVirtualDisk.Checked)
            {
                checkBoxBitlocker.Enabled = true;
            }
            else
            {
                checkBoxBitlocker.Checked = false;
                checkBoxBitlocker.Enabled = false;
            }
        }

        private void toolStripMenuItem3_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItem3.Checked)
            {
                IniFile.WriteVal("Main", "AutoUpdate", "1", Config.settingFilePath);
            }
            else
            {
                IniFile.WriteVal("Main", "AutoUpdate", "0", Config.settingFilePath);

            }
        }


        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            WebUtility.VisitWeb("https://bbs.luobotou.org/thread-1625-1-1.html");

        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("This program will restart,continue?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }

            IniFile.WriteVal("Main", "Language", "EN", Config.settingFilePath);
            Application.Restart();
        }

        private void chineseSimpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("程序将会重启，确认继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }
            IniFile.WriteVal("Main", "Language", "ZH-HANS", Config.settingFilePath);
            Application.Restart();

        }

        private void 繁体中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("程序將會重啟，確認繼續？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }
            IniFile.WriteVal("Main", "Language", "ZH-HANT", Config.settingFilePath);
            Application.Restart();

        }

        private void 修复盘符ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            string vhdPath = string.Empty;
            if (File.Exists(vhdPath = WTGModel.ud + "win8.vhd") || File.Exists(vhdPath = WTGModel.ud + "win8.vhdx"))
            {
                VHDOperation vo = new VHDOperation();
                vo.AttachVHD(vhdPath);
                ImageOperation.Fixletter("C:", "V:");
                vo.DetachVHD();
            }
            else
            {
                ImageOperation.Fixletter("C:", WTGModel.ud.Substring(0, 2));
            }
            MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
        }

        #endregion


        #region UserControls

        private void Form1_Load(object sender, EventArgs e)
        {
            Graphics graphics = CreateGraphics();
            float dpiX = graphics.DpiX;

            Width = (int)(720 * (dpiX / 96.0));

            toolStripMenuItem3.Checked = autoCheckUpdate;
            FileInitialization.FileValidation();
            ReadPerference();
            SystemDetection(true);
            Thread autodetectWim = new Thread(AutoDetectInstallWim);
            autodetectWim.Start();
            //linkLabel7_LinkClicked(null, null);

            SWOnline swo = new SWOnline(releaseUrl);
            swo.TopicLink = WriteProgress.topicLink;
            swo.TopicName = WriteProgress.topicName;
            swo.Linklabel = linkLabel2;
            int currentRevision = Assembly.GetExecutingAssembly().GetName().Version.Revision;
            if (currentRevision == 9)
            {
                Text += "Preview Bulit:" + File.GetLastWriteTime(GetType().Assembly.Location);
            }
            else
            {
                Text += Application.ProductVersion;
                if (currentRevision == 0)
                {
                    if (autoCheckUpdate)
                    {
                        Thread threadUpdate = new Thread(swo.Update);
                        threadUpdate.Start();
                    }
                    Thread threadShowad = new Thread(swo.Showad);
                    threadShowad.Start();
                }
            }


            //Thread threadReport = new Thread(swo.Report);
            //threadReport.Start();


            GetUdiskList.LoadUDList(comboBoxUd);
            comboBoxParts.Items.Clear();
            comboBoxParts.Items.Add("自动选择安装分卷");
            comboBoxParts.SelectedIndex = 0;
            comboBoxVhdPartitionType.SelectedIndex = 1;
            comboBoxGb.SelectedIndex = 1;
            radiobtnLegacy.Checked = true;

            //txtVhdTempPath.text

        }

        private void ReadPerference()
        {
            txtwim.Text = ReadSet("WimPath");

            //foreach (Control item in tabPage2.Controls)
            //{
            //    formControlList.Add(item);
            //}
            foreach (Control item in tabPageCommon.Controls)
            {
                formControlList.Add(item);
            }

            foreach (var item in formControlList)
            {
                if (item.GetType() == typeof(CheckBox))
                {
                    string val = ReadSet(((CheckBox)item).Name);
                    if (val == "True")
                    {
                        ((CheckBox)item).Checked = true;
                    }
                    else if (val == "False")
                    {
                        ((CheckBox)item).Checked = false;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                if (tWrite != null && tWrite.IsAlive)
                {
                    if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_WritingAbort", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo))
                    {
                        return;
                    }
                    tWrite.Abort();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Msg_WriteProcessing", ex.ToString());
            }
            if (radiobtnLegacy.Checked)
            {
                WTGModel.CheckedMode = ApplyMode.Legacy;
            }
            else
            {
                if (radioButtonVHD.Checked)
                {
                    WTGModel.CheckedMode = ApplyMode.VHD;
                }
                else
                {
                    WTGModel.CheckedMode = ApplyMode.VHDX;
                }
            }

            AssignUDVariable();
            WTGModel.isBitlocker = checkBoxBitlocker.Checked;
            WTGModel.isWimBoot = false;
            WTGModel.isBlockLocalDisk = checkBoxSan_policy.Checked;
            //WTGModel.imageFilePath = txtwim.Text;
            WTGModel.isCompactOS = false;
            if (comboBoxGb.SelectedIndex == 1)
            {
                WTGModel.userSetSize = (int)numericUpDown1.Value * 1024;
            }
            else if (comboBoxGb.SelectedIndex == 2)
            {
                WTGModel.userSetSize = (int)numericUpDown1.Value * 1024 * 1024;
            }
            else
            {
                WTGModel.userSetSize = (int)numericUpDown1.Value;
            }
            WTGModel.rePartition = checkBoxDiskpart.Checked;
            WTGModel.isFixedVHD = checkBoxFixed.Checked;
            WTGModel.isUefiGpt = checkBoxUefigpt.Checked;
            WTGModel.isUefiMbr = checkBoxUefimbr.Checked;
            //WTGModel.isNoTemp = checkBoxNotemp.Checked;
            WTGModel.ntfsUefiSupport = true;
            WTGModel.doNotFormat = checkBoxNoFormat.Checked;
            WTGModel.vhdNameWithoutExt = txtVhdNameWithoutExt.Text;
            WTGModel.installDonet35 = checkBoxDonet.Checked;
            //WTGModel.fixLetter = checkBoxFixLetter.Checked;
            WTGModel.noDefaultDriveLetter = checkBoxNoDefaultLetter.Checked;
            WTGModel.disableWinRe = checkBoxDisWinre.Checked;
            WTGModel.disableUasp = checkBoxDisUasp.Checked;
            WTGModel.efiPartitionSize = txtEfiSize.Text;
            //MessageBox.Show(comboBoxVhdPartitionType.SelectedIndex.ToString());
            WTGModel.vhdPartitionType = comboBoxVhdPartitionType.SelectedIndex;
            //WTGModel.vhdTempPath = txtVhdTempPath.Text;
            WTGModel.partitionSize = new string[3];
            WTGModel.partitionSize[0] = txtPartitionSize1.Text;
            WTGModel.partitionSize[1] = txtPartitionSize2.Text;
            WTGModel.partitionSize[2] = txtPartitionSize3.Text;
            WTGModel.wimPart = comboBoxParts.SelectedIndex.ToString();
            WTGModel.skipOOBE = checkBoxOOBE.Checked;

            if (radiobtnVirtualDisk.Checked)
            {
                if (radioButtonVHD.Checked)
                    WTGModel.vhdExtension = "vhd";
                else
                    WTGModel.vhdExtension = "vhdx";
            }
            WTGModel.win8VHDFileName = WTGModel.vhdNameWithoutExt + "." + WTGModel.vhdExtension;
            tWrite = new Thread(new ThreadStart(CreateMain.GoWrite));
            tWrite.Start();

        }
        private void AssignUDVariable()
        {
            WTGModel.UdObj = (UsbDisk)comboBoxUd.SelectedItem;
            //Assign letter moved to CreateMain
            WTGModel.udString = comboBoxUd.SelectedItem.ToString();

        }

        private void ManualSelectUdisk()
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath.Length != 3)
            {
                if (folderBrowserDialog1.SelectedPath.Length != 0)
                {
                    //请选择优盘根目录
                    MessageBox.Show(MsgManager.GetResString("Msg_UDRoot", MsgManager.ci));
                }
                return;

            }
            UsbDisk udM = new UsbDisk(folderBrowserDialog1.SelectedPath);
            //udM.Volume = folderBrowserDialog1.SelectedPath;
            GetUdiskList.diskCollection.Add(udM);
            //UDList.Add(folderBrowserDialog1.SelectedPath);
            GetUdiskList.OutText(true, GetUdiskList.diskCollection, comboBoxUd);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (tWrite != null && tWrite.IsAlive)
                {
                    //MsgManager.getResString("Msg_WritingAbort")
                    //正在写入，您确定要取消吗？
                    if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_WritingAbort", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { e.Cancel = true; return; }
                    Environment.Exit(0);
                    //threadwrite.Abort();
                }
                if (useiso)
                {
                    ISOHelper.DismountISO(txtwim.Text);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_Form1_FormClosing", ex.ToString());
                //Console.WriteLine(ex);
            }
            VHDOperation.CleanTemp();
            try
            {
                Directory.Delete(Path.GetTempPath() + "\\WTGA", true);
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_DeleteTempPath", ex.ToString());
            }
            Environment.Exit(0);
        }
        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


        private void 错误提示测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageOperation.Solve1809("W:");
            //WriteProgress wp = new WriteProgress();
            //wp.Show();
            // MessageBox.Show(WTGModel.UdObj.Model);

            //Benchmark bm = new Benchmark();
            //WTGModel.UdObj = (UsbDisk)comboBoxUd.SelectedItem;
            ////MessageBox.Show);
            //BenchmarkResult bmr = bm.DoBenchmark("D:");
            //MessageBox.Show(bmr.Write4K.ToString());
            //MessageBox.Show(bmr.WriteSeq.ToString());
            //Write.EnableBitlocker("V:");
            //USBAudioDeviceItems dict = USBAudioDeviceItems.USBDevices;
            //MessageBox.Show(dict.Count.ToString());
            //foreach (var item in dict)
            //{
            //    MessageBox.Show(item.Key + " " + item.Value);
            ////}
            //WTGModel.UdObj = (UsbDisk)comboBoxUd.SelectedItem;
            //MessageBox.Show(WTGModel.UdObj.DiskSize.ToString());
            //ulong bootPrtitionSectors = 1021951;
            //ulong mainPartitionSectors = WTGModel.UdObj.TotalSectors - bootPrtitionSectors - 202049;
            //ulong precedingSectors = WTGModel.UdObj.TotalSectors - bootPrtitionSectors - 200001;
            ////string convertTempString = Convert.ToString(Convert.ToInt32(mainPartitionSecors), 16);
            ////MessageBox.Show(mainPartitionSectors.ToString());
            ////string hex1 = convertTempString[6].ToString() + convertTempString[7];
            ////string hex2 = convertTempString[4].ToString() + convertTempString[5];
            ////string hex3 = convertTempString[2].ToString() + convertTempString[3];
            ////string hex4 = convertTempString[0].ToString() + convertTempString[1];
            ////byte byteHex1 = Convert.ToByte(Convert.ToInt32(hex1, 16));
            ////byte byteHex2 = Convert.ToByte(Convert.ToInt32(hex1, 16));
            ////byte byteHex3 = Convert.ToByte(Convert.ToInt32(hex1, 16));
            ////byte byteHex4 = Convert.ToByte(Convert.ToInt32(hex1, 16));
            //byte[] newMainPartitionSectorsByts = UlongToMBRBytes(mainPartitionSectors);
            //byte[] newPrecedingSectorsByts = UlongToMBRBytes(precedingSectors);
            ////string completedHex = hex1 + hex2 + hex3 + hex4;

            //byte[] byts = File.ReadAllBytes(@"C:\Users\Li\Documents\MBR\57G-1.dpt");
            //byts[730] = newMainPartitionSectorsByts[0];
            //byts[731] = newMainPartitionSectorsByts[1];
            //byts[732] = newMainPartitionSectorsByts[2];
            //byts[733] = newMainPartitionSectorsByts[3];
            //byts[742] = newPrecedingSectorsByts[0];
            //byts[743] = newPrecedingSectorsByts[1];
            //byts[744] = newPrecedingSectorsByts[2];
            //byts[745] = newPrecedingSectorsByts[3];

            //byte[] byts2 = File.ReadAllBytes(@"C:\Users\Li\Documents\MBR\57G-2.dpt");
            //byts[726] = newPrecedingSectorsByts[0];
            //byts[727] = newPrecedingSectorsByts[1];
            //byts[728] = newPrecedingSectorsByts[2];
            //byts[729] = newPrecedingSectorsByts[3];


            //File.WriteAllBytes(@"C:\Users\Li\Documents\MBR\NEW-ex-1.dpt", byts);
            //File.WriteAllBytes(@"C:\Users\Li\Documents\MBR\NEW-ex-2.dpt", byts2);

            //MessageBox.Show(Convert.ToString(Convert.ToInt32(byts[731]), 16));
            //int[] ints = new int[byts.Length];
            //for (int i = 0; i < byts.Length; i++)
            //{
            //    //ints[i] = Convert.ToInt32(byts[i]);
            //    //MessageBox.Show(Convert.ToInt32(byts[i]).ToString());
            //}
            //MessageBox.Show(byts.Length.ToString());

            //Write.EnableBitlocker();
            //BootFileOperation.BcdeditFixBootFileVHD("E:\\", "E:\\", "wtg.vhdx", FirmwareType.BIOS);
        }

        private void radiochuantong_CheckedChanged(object sender, EventArgs e)
        {

            numericUpDown1.Enabled = false;
            checkBoxFixed.Enabled = false;
            //checkBoxNotemp.Enabled = false;
            lblVhdSize.Enabled = false;

            lblVhdPartitionTableType.Enabled = false;
            comboBoxVhdPartitionType.Enabled = false;
            //lblVhdTempPath.Enabled = false;
            //txtVhdTempPath.Enabled = false;
            //btnVhdTempPath.Enabled = false;

            comboBoxGb.Enabled = false;
            trackBar1.Enabled = false;
            lblVhdName.Enabled = false;
            txtVhdNameWithoutExt.Enabled = false;

            if (checkBoxBitlocker.Checked)
            {
                radiobtnLegacy.Checked = true;
                //radiobtnVhd.Enabled = false;
                radiobtnVirtualDisk.Enabled = false;
            }

            if ((checkBoxUefigpt.Checked || checkBoxUefimbr.Checked) && !radiobtnVirtualDisk.Checked)
            {
                checkBoxBitlocker.Enabled = true;
            }
            else
            {
                checkBoxBitlocker.Checked = false;
                checkBoxBitlocker.Enabled = false;
            }
        }

      

        private void radiovhdx_CheckedChanged(object sender, EventArgs e)
        {
            if (radiobtnVirtualDisk.Checked && DialogResult.Cancel == MessageBox.Show(MsgManager.GetResString("Msg_VHDXWarning", MsgManager.ci), "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
            {
                radiobtnVirtualDisk.Checked = false;
                radiobtnLegacy.Checked = true;
                return;
            }
            checkBoxBitlocker.Checked = false;
            checkBoxBitlocker.Enabled = false;
            numericUpDown1.Enabled = true;
            checkBoxFixed.Enabled = true;
            //checkBoxNotemp.Enabled = true;
            lblVhdSize.Enabled = true;
            lblVhdPartitionTableType.Enabled = true;
            comboBoxVhdPartitionType.Enabled = true;
            //lblVhdTempPath.Enabled = true;
            //txtVhdTempPath.Enabled = true;
            //btnVhdTempPath.Enabled = true;
            if (comboBoxUd.SelectedIndex != 0 && comboBoxUd.SelectedIndex != -1)
            {
                trackBar1.Enabled = true;
            }
            comboBoxGb.Enabled = true;
            lblVhdName.Enabled = true;
            txtVhdNameWithoutExt.Enabled = true;

        }

        #endregion




        private void txtPartitionSize1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != 8)
            {
                e.Handled = true;
            }
            else
            {
                checkBoxDiskpart.Checked = true;
            }
        }

        private void txtEfiSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtPartitionSize2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != 8)
            {
                e.Handled = true;
            }
            else
            {
                checkBoxDiskpart.Checked = true;
            }
        }


        private void txtPartitionSize3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void linklblRestoreMultiPartition_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtEfiSize.Text = "350";
            txtPartitionSize3.Text = "0";
            txtPartitionSize2.Text = "0";

        }



        private void comboBoxUd_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBoxUd.SelectedIndex.ToString());

            if (comboBoxUd.SelectedIndex != 0 && comboBoxUd.SelectedIndex != -1)
            {
                btnGo.Enabled = true;
                groupBoxadv.Enabled = true;

                SystemDetection(false);
                WTGModel.UdObj = (UsbDisk)comboBoxUd.SelectedItem;
                udSizeInMB = (int)(WTGModel.UdObj.DiskSize / 1048576);
                txtPartitionSize1.Text = udSizeInMB.ToString();
                txtPartitionSize2.Text = "0";
                txtPartitionSize3.Text = "0";



                if (radiobtnVirtualDisk.Checked)
                {
                    trackBar1.Enabled = true;
                }
                if (WTGModel.UdObj.DriveType.Contains("Removable"))
                {
                    checkBoxUefimbr.Checked = true;
                }
                txtPartitionSize3.Enabled = true;
                txtPartitionSize2.Enabled = true;
                txtPartitionSize1.Enabled = true;

                //if (!WTGModel.UdObj.DriveType.Contains("Removable Disk"))
                //{
                //    txtPartitionSize3.Enabled = true;
                //    txtPartitionSize2.Enabled = true;
                //    txtPartitionSize1.Enabled = true;
                //    if (checkBoxUefimbr.Enabled)
                //    {
                //        checkBoxUefimbr.Checked = true;
                //    }

                //}
                //else
                //{
                //    txtPartitionSize3.Enabled = false;
                //    txtPartitionSize2.Enabled = false;
                //    txtPartitionSize1.Enabled = false;
                //}

                if (WTGModel.UdObj.DiskSize == 0)
                {
                    checkBoxUefimbr.Checked = false;
                    checkBoxUefigpt.Checked = false;
                    checkBoxUefimbr.Enabled = false;
                    checkBoxUefigpt.Enabled = false;
                }
                //else
                //{
                //    checkBoxUefimbr.Enabled = true;
                //    checkBoxUefigpt.Enabled = true;
                //}


            }
            else
            {
                WTGModel.UdObj = null;
                txtPartitionSize3.Enabled = false;
                txtPartitionSize2.Enabled = false;
                txtPartitionSize1.Enabled = false;
            }



        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void txtPartitionSize1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPartitionSize3.Text))
            {
                txtPartitionSize3.Text = "0";
            }
            int remain = udSizeInMB - int.Parse(txtPartitionSize2.Text) - int.Parse(txtPartitionSize3.Text);
            if (remain < 0)
            {
                txtPartitionSize3.Text = (udSizeInMB - int.Parse(txtPartitionSize2.Text)).ToString();
                txtPartitionSize1.Text = "0";
            }
            else
            {
                txtPartitionSize1.Text = remain.ToString();
            }
        }

        private void txtPartitionSize2_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPartitionSize2.Text))
            {
                txtPartitionSize2.Text = "0";
            }
            int remain = udSizeInMB - int.Parse(txtPartitionSize2.Text) - int.Parse(txtPartitionSize3.Text);
            if (remain < 0)
            {
                txtPartitionSize2.Text = (udSizeInMB - int.Parse(txtPartitionSize3.Text)).ToString();
                txtPartitionSize1.Text = "0";

            }
            else
            {
                txtPartitionSize1.Text = (udSizeInMB - int.Parse(txtPartitionSize2.Text) - int.Parse(txtPartitionSize3.Text)).ToString();
            }

        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("https://bbs.luobotou.org");
        }



        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://bbs.luobotou.org/thread-6098-1-1.html");

        }

  

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://bbs.luobotou.org/thread-6098-1-1.html");

        }

        private void comboBoxParts_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip1.SetToolTip(comboBoxParts, comboBoxParts.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_comboBoxParts_MouseHover", ex.ToString());
            }
        }

        private void checkBoxBitlocker_CheckedChanged(object sender, EventArgs e)
        {

            //SystemDetection();
            if (checkBoxBitlocker.Checked)
            {
                radiobtnLegacy.Checked = true;
                //radiobtnVhd.Enabled = false;
                radiobtnVirtualDisk.Enabled = false;
            }
            else
            {
                SystemDetection(false);
            }

            if ((checkBoxUefigpt.Checked || checkBoxUefimbr.Checked) && !radiobtnVirtualDisk.Checked)
            {
                checkBoxBitlocker.Enabled = true;
            }
            else
            {
                checkBoxBitlocker.Checked = false;
                checkBoxBitlocker.Enabled = false;
            }
            //if (checkBoxBitlocker.Checked)
            //{
            //    //if (!checkBoxUefigpt.Checked && !checkBoxUefimbr.Checked)
            //    //{
            //    //    checkBoxUefimbr.Checked = true;
            //    //}
            //    //checkBoxNotemp.Checked = true;
            //    //checkBoxNotemp.Enabled = false;
            //    radiobtnVhd.Enabled = false;
            //    radiobtnVhdx.Enabled = false;
            //    radiobtnLegacy.Checked = true;

            //}
            //else
            //{
            //    //if (!radiobtnLegacy.Checked)
            //    //{
            //    //    checkBoxNotemp.Enabled = true;
            //    //}
            //    SystemDetection();

            //}
        }

        private void 手动选择优盘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManualSelectUdisk();
        }

        private void checkBoxBitlocker_Click(object sender, EventArgs e)
        {
            MessageBox.Show(MsgManager.GetResString("Msg_BitlockerUnusable", MsgManager.ci));

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            AutoSetNumbericVhdSize();
        }

        private void comboBoxGb_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoSetNumbericVhdSize();
        }
        private void AutoSetNumbericVhdSize()
        {
            if (comboBoxUd.SelectedIndex != 0 && comboBoxUd.SelectedIndex != -1 && WTGModel.UdObj != null && udSizeInMB != 0)
            {
                if (comboBoxGb.SelectedIndex == 0)
                {
                    numericUpDown1.Value = (udSizeInMB / 10) * trackBar1.Value;
                }
                else if (comboBoxGb.SelectedIndex == 1)
                {
                    numericUpDown1.Value = (int)(udSizeInMB / 10.0 * trackBar1.Value / 1024);
                }
            }
        }



        private void 克隆本机系统ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (DialogResult.No == new Clone().ShowDialog())
            {
                return;
            }
            AssignUDVariable();

            /*if (!Directory.Exists(Application.StartupPath + "\\wtg_clone"))
            {
                MessageBox.Show("请在官方论坛下载克隆支持文件！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/

            if (WTGModel.UdObj.Size == 0)
            {
                MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci) + "!", MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }//是否选择优盘


            tWrite = new Thread(new ThreadStart(() =>
            {
                try
                {
                    if (WTGModel.UdObj.Volume == string.Empty)
                    {
                        WTGModel.ud = "W:\\";//
                    }
                    else
                    {
                        WTGModel.ud = WTGModel.UdObj.Volume.Substring(0, 2) + "\\";
                    }


                    DiskOperation.AssignDriveLetterFAT32(WTGModel.UdObj.Index, WTGModel.ud.Substring(0, 1));
                    WTGModel.UdObj.SetVolume(WTGModel.ud.Substring(0, 1));

                    //Copy and mount WINRE
                    ProcessManager.ECMD("attrib.exe", @" -h -s c:\Recovery /s /d");
                    Directory.CreateDirectory(Path.GetTempPath() + "\\WTGA\\WinRE");
                    ProcessManager.ECMD("xcopy.exe", @"C:\Recovery\WindowsRE\Winre.wim " + Path.GetTempPath() + "WTGA\\WinRE\\ /H /Y");
                    //File.Copy(@"C:\Recovery\WindowsRE\Winre.wim", Path.GetTempPath() + "\\WTGA\\WinRE\\winre.wim");
                    string mountPath = Path.GetTempPath() + "\\WTGA\\WinRE\\mount";
                    ProcessManager.ECMD("dism.exe", @" /mount-image /imagefile:" + Path.GetTempPath() + "\\WTGA\\WinRE\\Winre.wim" + @" /index:1 /mountdir:" + mountPath);
                    //Modify WinRE 
                    ProcessManager.ECMD("xcopy.exe", WTGModel.applicationFilesPath + "\\wtgclone\\* " + mountPath + "\\windows\\system32\\ -Y");
                    ProcessManager.ECMD("dism.exe", "/unmount-image /mountdir:" + mountPath + " /commit");
                    //Config Udisk boot item

                    BootFileOperation.BcdbootWriteBootFile(@"C:\", WTGModel.ud.Substring(0, 2) + "\\", FirmwareType.UEFI);
                    File.Copy(WTGModel.applicationFilesPath + "\\bcd", WTGModel.ud.Substring(0, 2) + "\\EFI\\Microsoft\\Boot\\BCD", true);
                    Directory.CreateDirectory(WTGModel.ud.Substring(0, 2) + "\\Boot");
                    File.Copy(@"C:\Windows\System32\Boot.sdi", WTGModel.ud.Substring(0, 2) + "\\Boot\\Boot.sdi", true);
                    Directory.CreateDirectory(WTGModel.ud.Substring(0, 2) + "\\Sources");
                    ProcessManager.ECMD("xcopy.exe", Path.GetTempPath() + "WTGA\\WinRE\\winre.wim " + WTGModel.ud.Substring(0, 2) + "\\Sources\\ /H");
                    File.Move(WTGModel.ud.Substring(0, 2) + "\\Sources\\winre.wim", WTGModel.ud.Substring(0, 2) + "\\Sources\\boot.wim");
                    //ProcessManager.SyncCMD("ren " + WTGModel.ud.Substring(0, 2) + "\\Sources\\winre.wim boot.wim");
                    //Mark source and destination
                    if (!File.Exists(Path.GetPathRoot(Environment.GetEnvironmentVariable("windir")) + "wtg_clone_source"))
                    {
                        File.Create(Path.GetPathRoot(Environment.GetEnvironmentVariable("windir")) + "wtg_clone_source");
                    }
                    File.Create(WTGModel.ud + "wtg_clone_destination");
                    //Clean 
                    ProcessManager.SyncCMD(@"attrib +h +s c:\Recovery /s /d");
                    File.Delete(Path.GetTempPath() + "\\WTGA\\WinRE\\boot.wim");
                    FormHelper.Closewp();
                    if (File.Exists(WTGModel.ud.Substring(0, 2) + "\\Sources\\boot.wim"))
                        MessageBox.Show("请从优盘启动进行克隆！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        ErrorMsg em = new ErrorMsg("出错了，未能创建！", true);
                        em.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg em = new ErrorMsg(ex.ToString(), true);
                    em.ShowDialog();
                }
            }));
            tWrite.Start();




            //ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\MakeWinPEMedia.cmd", "/UFD /f \"" + Application.StartupPath + "\\wtg_clone\" " + WTGModel.ud.Substring(0, 2));
            //ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\bootsect.exe", "/nt60 " + WTGModel.ud.Substring(0, 2) + " /force /mbr");





        }

    


        private void btnwim_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (useiso)
            {
                try
                {
                    ISOHelper.DismountISO(isoPath);
                }
                catch { }
                useiso = false;
                isoPath = string.Empty;
            }
            if (File.Exists(openFileDialog1.FileName))
            {
                txtwim.Text = openFileDialog1.FileName;
                WTGModel.imageFilePath = openFileDialog1.FileName;
                WTGModel.choosedImageType = Path.GetExtension(openFileDialog1.FileName.ToLower()).Substring(1);
                if (WTGModel.choosedImageType == "iso")
                {

                    bool mount_successfully = false;
                    try
                    {
                        ISOHelper.MountISO(WTGModel.imageFilePath);
                    }
                    catch { }


                    for (int i = 68; i <= 90; i++)
                    {
                        string ascll_to_eng = Convert.ToChar(i).ToString();
                        if (File.Exists(ascll_to_eng + ":\\sources\\install.wim"))
                        {
                            //txtwim.Text = ascll_to_eng + ":\\sources\\install.wim";
                            WTGModel.imageFilePath = ascll_to_eng + ":\\sources\\install.wim";
                            WTGModel.choosedImageType = "wim";

                            mount_successfully = true;
                            break;
                        }
                    }
                    if (!mount_successfully)
                    {
                        //请选择install.wim
                        MessageBox.Show(MsgManager.GetResString("Msg_ImageError", MsgManager.ci));
                        txtwim.Text = string.Empty;
                        WTGModel.imageFilePath = string.Empty;
                        return;
                    }
                    else
                    {
                        useiso = true;
                        isoPath = txtwim.Text;
                    }
                }
                if (WTGModel.choosedImageType == "esd")
                {
                    if (!WTGModel.allowEsd)
                    {
                        //MsgManager.getResString("Msg_ESDError")
                        //此系统不支持ESD文件处理！");
                        MessageBox.Show(MsgManager.GetResString("Msg_ESDError", MsgManager.ci));

                        return;
                    }
                    else
                    {
                        SystemDetection(false);
                        WTGModel.isEsd = true;
                        //checkBoxWimboot.Checked = false;
                        //checkBoxWimboot.Enabled = false;
                    }

                }
                else if (WTGModel.choosedImageType == "vhd" || WTGModel.choosedImageType == "vhdx")
                {
                    if (!radiobtnVirtualDisk.Enabled)
                    {
                        radiobtnVirtualDisk.Checked = true;
                        radiobtnLegacy.Enabled = false;
                        //radiobtnVhd.Enabled = false;

                        //checkBoxCompactOS.Checked = true;
                        //checkBoxCompactOS.Enabled = false;

                    }
                    else
                    {
                        radiobtnLegacy.Enabled = false;
                        //radiobtnVhd.Enabled = false;

                        radiobtnVirtualDisk.Checked = true;
                        //MessageBox.Show("Test");
                    }

                }
                else
                {
                    WTGModel.win7togo = ImageOperation.Iswin7(WTGModel.imagexFileName, WTGModel.imageFilePath);
                    if (WTGModel.win7togo != 0) //WIN7 cannot comptible with VHDX disk or wimboot
                    {

                        radiobtnVirtualDisk.Enabled = false;
                        //checkBoxWimboot.Checked = false;
                        //checkBoxWimboot.Enabled = false;
                    }
                }
                DisplayImagePartsInfo();
                comboBoxUd.Enabled = true;
            }
        }
        private void DisplayImagePartsInfo()
        {
            if (Regex.IsMatch(WTGModel.choosedImageType, @"wim|esd") && WTGModel.CurrentOS != OS.XP && WTGModel.CurrentOS != OS.Vista)
            {
                List<string> partsInfoList = ImageOperation.DismGetImagePartsInfo(WTGModel.imageFilePath);
                comboBoxParts.Invoke(new Action(() =>
                {
                    comboBoxParts.Items.Clear();
                    comboBoxParts.Items.Add("自动选择安装分卷");
                    comboBoxParts.Items.AddRange(partsInfoList.ToArray());
                    comboBoxParts.SelectedIndex = 0;
                }));
            }
        }

        private void txtPartitionSize3_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://bbs.luobotou.org/thread-6098-1-1.html");

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }




        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }
        private void AutoDetectInstallWim()
        {
            bool detectSuccess = false;
            for (int i = 68; i <= 90; i++)
            {
                string ascll_to_eng = Convert.ToChar(i).ToString();
                if (File.Exists(ascll_to_eng + ":\\sources\\install.wim"))
                {
                    txtwim.Invoke(new Action(() => { txtwim.Text = ascll_to_eng + ":\\sources\\install.wim"; }));
                    WTGModel.imageFilePath = ascll_to_eng + ":\\sources\\install.wim";
                    WTGModel.choosedImageType = "wim";

                    detectSuccess = true;
                    break;
                }
            }
            if (detectSuccess)
                DisplayImagePartsInfo();
        }
        private string ReadSet(string name)
        {
            return IniFile.ReadVal("Perference", name, Config.settingFilePath);
        }
        private void SaveSet(string name, string val)
        {
            IniFile.WriteVal("Perference", name, val, Config.settingFilePath);
        }
        private void 保存当前设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IniFile.WriteVal("Perference", "WimPath", txtwim.Text, Config.settingFilePath);
            foreach (var item in formControlList)
            {
                if (item.GetType() == typeof(CheckBox))
                {
                    SaveSet(((CheckBox)item).Name, ((CheckBox)item).Checked.ToString());
                }
            }
        }

     

        private void checkBoxNoDefaultLetter_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtwim_TextChanged(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void comboBoxParts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxNoFormat_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBoxNoFormat.Checked && (checkBoxDiskpart.Checked || checkBoxUefigpt.Checked || checkBoxUefimbr.Checked))
            {
                MessageBox.Show(MsgManager.GetResString("Msg_SelectBothNoformatAndRepartition", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkBoxNoFormat.Checked = false;
            }
        }

        private void linkLabel3_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void buttonFFUBrowse_Click(object sender, EventArgs e)
        {

            if (radioButtonBackup.Checked)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel) return;

                if (Directory.GetParent(saveFileDialog1.FileName).Exists)
                {
                    WTGModel.ffuFilePath = saveFileDialog1.FileName;
                    textBoxFFU.Text = saveFileDialog1.FileName;
                }

            }
            else
            {
                if (openFileDialogFFU.ShowDialog() == DialogResult.Cancel) return;

                if (File.Exists(openFileDialogFFU.FileName))
                {
                    WTGModel.ffuFilePath = openFileDialogFFU.FileName;
                    textBoxFFU.Text = openFileDialogFFU.FileName;
                }
            }
        }

        private void buttonBackup_Click(object sender, EventArgs e)
        {

            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            if (string.IsNullOrEmpty(WTGModel.ffuFilePath)) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }

            if (radioButtonRestore.Checked)
            {
                StringBuilder formatTip = new StringBuilder();
                formatTip.AppendLine(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci));
                formatTip.Append(WTGModel.UdObj.Model);
                formatTip.AppendLine(MsgManager.GetResString("Msg_FormatTip", MsgManager.ci));
                //formatTip.AppendLine(MsgManager.GetResString("Msg_Repartition", MsgManager.ci));
                FormatAlert fa = new FormatAlert(formatTip.ToString());
                if (DialogResult.Yes != fa.ShowDialog())
                {
                    return;
                }
            }




            Thread tCommand = new Thread(() =>
            {
                string dismArgs = string.Empty;
                if (radioButtonBackup.Checked)
                {
                    dismArgs = string.Format(@"/capture-ffu /imagefile={0} /capturedrive=\\.\PhysicalDrive{1} /name:WTGBackup", WTGModel.ffuFilePath, WTGModel.UdObj.Index);

                }
                else
                {
                    dismArgs = string.Format(@"/apply-ffu /ImageFile={0} /ApplyDrive:\\.\PhysicalDrive{1}", WTGModel.ffuFilePath, WTGModel.UdObj.Index);
                }
                ProcessManager.ECMD("dism.exe", dismArgs);
                bool failed = false;

                if (File.Exists(Environment.GetEnvironmentVariable("windir") + @"\Logs\DISM\dism.log"))
                {
                    string[] dismLog = File.ReadAllLines(Environment.GetEnvironmentVariable("windir") + @"\Logs\DISM\dism.log");
                    for (int i = 1; i < 20; i++)
                    {
                        if (dismLog[dismLog.Length - i].Contains("Failed"))
                        {
                            failed = true;
                            MessageBox.Show(MsgManager.GetResString("Msg_Failure", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci),MessageBoxButtons.OK,MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            FormHelper.Closewp();
                            break;
                        }
                    }
                }
                if (!failed)
                {
                    FormHelper.Closewp();
                    MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci), MsgManager.GetResString("Msg_Complete", MsgManager.ci),MessageBoxButtons.OK,MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            });
            tCommand.Start();

        }

        private void radioButtonRestore_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel5_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://bbs.luobotou.org/thread-46043-1-1.html");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void radioButtonBackup_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
