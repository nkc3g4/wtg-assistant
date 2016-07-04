using iTuner;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
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
        private readonly string releaseUrl = "http://bbs.luobotou.org/app/wintogo.txt";
        private readonly string reportUrl = "http://myapp.luobotou.org/statistics.aspx?name=wtg&ver=";
        private readonly string settingFilePath = Application.StartupPath + "\\settings.ini";


        public Form1()
        {
            ReadConfigFile();

            InitializeComponent();
            txtVhdTempPath.Text = WTGModel.vhdTempPath;
        }



        private void FileValidation()
        {
            if (StringUtility.IsChinaOrContainSpace(WTGModel.diskpartScriptPath))
            {
                if (StringUtility.IsChinaOrContainSpace(Application.StartupPath))
                {
                    Log.WriteLog("Err_IsChinaOrContainSpace", "FileValidationErr");
                    ErrorMsg er = new ErrorMsg(MsgManager.GetResString("IsChinaMsg", MsgManager.ci));
                    er.ShowDialog();
                    Environment.Exit(0);
                }
                else
                {
                    WTGModel.diskpartScriptPath = WTGModel.logPath;
                }
            }
            ProcessManager.SyncCMD("taskkill.exe /f /IM BOOTICE.exe");
            //ProcessManager.KillProcessByName("bootice.exe");
            //解压文件
            try
            {
                ZipHelper.UnZip(Application.StartupPath + "\\files.dat", WTGModel.applicationFilesPath);
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_Unzip", ex.ToString());
            }
        }

        private void SystemDetection(bool isInitialization)
        {
            checkBoxBitlocker.Enabled = false;
            WTGModel.bcdbootFileName = "bcdboot.exe";
            string osVersionStr = Environment.OSVersion.ToString();
            if (osVersionStr.Contains("5.1") || osVersionStr.Contains("6.0")) //XP 禁用功能
            {
                radiobtnLegacy.Enabled = true;
                radiobtnVhd.Enabled = false;
                radiobtnVhdx.Enabled = false;
                if (isInitialization)
                {
                    radiobtnLegacy.Checked = true;
                }
                groupBoxAdv.Enabled = false;
                checkBoxDiskpart.Checked = false;
                checkBoxDiskpart.Enabled = false;
                labelDisFunc.Visible = true;
                WTGModel.CurrentOS = OS.XP;
            }
            else if (osVersionStr.Contains("6.1"))//WIN7
            {
                //labelDisFuncEM.Visible = true;
                radiobtnLegacy.Enabled = true;
                radiobtnVhd.Enabled = true;
                labelDisFunc.Visible = true;
                radiobtnVhd.Checked = true;
                radiobtnVhdx.Enabled = false;
                WTGModel.CurrentOS = OS.Win7;
            }
            else if (osVersionStr.Contains("6.2") || osVersionStr.Contains("6.3"))//WIN8及以上
            {
                radiobtnLegacy.Enabled = true;
                radiobtnVhd.Enabled = true;
                radiobtnVhdx.Enabled = true;
                checkBoxUefigpt.Enabled = true;
                checkBoxUefimbr.Enabled = true;
                if (isInitialization)
                {
                    radiobtnVhd.Checked = true;
                }
                //WIN8.1 UPDATE1 WIMBOOT  已修复WIN10版本号问题
                string dismversion = FileOperation.GetFileVersion(System.Environment.GetEnvironmentVariable("windir") + "\\System32\\dism.exe");
                if (dismversion.Substring(0, 14) == "6.3.9600.17031" || dismversion.Substring(0, 3) == "6.4")
                {
                    radiobtnLegacy.Enabled = true;
                    radiobtnVhd.Enabled = true;
                    radiobtnVhdx.Enabled = true;
                    checkBoxUefigpt.Enabled = true;
                    checkBoxUefimbr.Enabled = true;
                    checkBoxWimboot.Enabled = true;
                    WTGModel.allowEsd = true;
                    labelDisFunc.Visible = true;
                    //labelDisFuncEM.Visible = true;
                    WTGModel.CurrentOS = OS.Win8_1_with_update;

                }
                else if (dismversion.Substring(0, 3) == "10.")
                {
                    radiobtnLegacy.Enabled = true;
                    radiobtnVhd.Enabled = true;
                    radiobtnVhdx.Enabled = true;
                    checkBoxUefigpt.Enabled = true;
                    checkBoxUefimbr.Enabled = true;
                    checkBoxWimboot.Enabled = true;
                    checkBoxCompactOS.Enabled = true;
                    WTGModel.allowEsd = true;
                    WTGModel.CurrentOS = OS.Win10;

                }
                else
                {
                    radiobtnLegacy.Enabled = true;
                    radiobtnVhd.Enabled = true;
                    radiobtnVhdx.Enabled = true;
                    checkBoxUefigpt.Enabled = true;
                    checkBoxUefimbr.Enabled = true;

                    WTGModel.CurrentOS = OS.Win8_without_update;
                }
            }

        }

        private void ReadConfigFile()
        {
            string autoup;
            string tp;
            string language;
            autoup = IniFile.ReadVal("Main", "AutoUpdate", settingFilePath);
            tp = IniFile.ReadVal("Main", "TempPath", settingFilePath);
            language = IniFile.ReadVal("Main", "Language", settingFilePath);
            if (autoup == "0") { autoCheckUpdate = false; }
            if (!string.IsNullOrEmpty(tp))
            {
                WTGModel.vhdTempPath = tp;
            }
            else
            {
                WTGModel.vhdTempPath = Path.GetTempPath();
            }
            if (language == "EN")
            {
                MsgManager.ci = new System.Globalization.CultureInfo("en");

                Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            }
            else if (language == "ZH-HANS")
            {
                MsgManager.ci = new System.Globalization.CultureInfo("zh-cn");

                Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            }
            else if (language == "ZH-HANT")
            {
                MsgManager.ci = new System.Globalization.CultureInfo("zh-Hant");

                Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            }
        }



        #region CORE

        private void GoWrite()
        {

            try
            {
                //wimpart = ChoosePart.part;//读取选择分卷，默认选择第一分卷
                #region 各种提示
                //各种提示
                if (lblWim.Text.ToLower().Substring(lblWim.Text.Length - 3, 3) != "wim" && lblWim.Text.ToLower().Substring(lblWim.Text.Length - 3, 3) != "esd" && lblWim.Text.ToLower().Substring(lblWim.Text.Length - 3, 3) != "vhd" && lblWim.Text.ToLower().Substring(lblWim.Text.Length - 4, 4) != "vhdx")//不是WIM文件
                {
                    //镜像文件选择错误！请选择install.wim！
                    MessageBox.Show(MsgManager.GetResString("Msg_chooseinstallwim", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    //请选择install.wim文件
                    if (!File.Exists(lblWim.Text))
                    {
                        MessageBox.Show(MsgManager.GetResString("Msg_chooseinstallwim", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }//文件不存在.
                    WTGModel.imageFilePath = lblWim.Text;
                }


                if (!Directory.Exists(WTGModel.ud))
                {
                    MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci) + "!", MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }//是否选择优盘
                if (DiskOperation.GetHardDiskSpace(WTGModel.ud) <= (12L * 1024 * 1024 * 1024)) //优盘容量<12 GB提示
                {
                    //MsgManager.getResString("Msg_DiskSpaceWarning") 
                    //可移动磁盘容量不足16G，继续写入可能会导致程序出错！您确定要继续吗？
                    if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_DiskSpaceWarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        return;
                    }
                }

                if (StringUtility.IsChinaOrContainSpace(WTGModel.vhdNameWithoutExt))
                {
                    MessageBox.Show(MsgManager.GetResString("Msg_VHDNameIllegal", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
                //if (DiskOperation.GetHardDiskSpace(WTGModel.ud) <= numericUpDown1.Value * 1048576)
                //{
                //    //优盘容量小于VHD设定大小，请修改设置！
                //    //MsgManager.getResString("Msg_DiskSpaceError")
                //    MessageBox.Show(MsgManager.GetResString("Msg_DiskSpaceError", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                //MsgManager.getResString("Msg_ConfirmChoose")
                //请确认您所选择的
                //MsgManager.getResString("Msg_Disk_Space") 盘，容量
                //Msg_FormatTip

                //GB 是将要写入的优盘或移动硬盘\n误格式化，后果自负！
                StringBuilder formatTip = new StringBuilder();
                formatTip.AppendLine(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci));
                formatTip.AppendFormat(WTGModel.udString);
                formatTip.AppendLine(MsgManager.GetResString("Msg_FormatTip", MsgManager.ci));
                if (checkBoxDiskpart.Checked)//勾选重新分区提示
                {
                    formatTip.AppendLine(MsgManager.GetResString("Msg_Repartition", MsgManager.ci));
                    FormatAlert fa = new FormatAlert(formatTip.ToString());
                    //MsgManager.getResString("Msg_Repartition")
                    //您勾选了重新分区，优盘或移动硬盘上的所有文件将被删除！\n注意是整个磁盘，不是一个分区！
                    if (DialogResult.Yes != fa.ShowDialog())
                    {
                        return;
                    }

                }
                else//普通格式化提示
                {

                    if (!WTGModel.doNotFormat)
                    {
                        formatTip.AppendLine(MsgManager.GetResString("Msg_FormatWarning", MsgManager.ci));
                        FormatAlert fa = new FormatAlert(formatTip.ToString());
                        if (DialogResult.Yes != fa.ShowDialog())
                        {
                            return;
                        }
                    }
                    else
                    {
                        FormatAlert fa = new FormatAlert(formatTip.ToString());
                        if (DialogResult.Yes != fa.ShowDialog())
                        {
                            return;
                        }
                    }
                }
                #endregion

                SystemSleepManagement.PreventSleep();

                //删除旧LOG文件
                VHDOperation.CleanTemp();
                Log.DeleteAllLogs();
                ProcessManager.KillProcessByName("bootice.exe");
                WriteProgramRunInfoToLog();

                writeSw.Restart();

                if (checkBoxUefigpt.Checked)
                {
                    //UEFI+GPT
                    if (Environment.OSVersion.ToString().Contains("5.1") || System.Environment.OSVersion.ToString().Contains("5.2"))
                    {
                        //MsgManager.getResString("Msg_XPUefiError")
                        //XP系统不支持UEFI模式写入
                        MessageBox.Show(MsgManager.GetResString("Msg_XPUefiError", MsgManager.ci)); return;
                    }
                    if (WTGModel.udString.Contains("Removable Disk"))
                    {
                        //普通优盘UEFI
                        WTGModel.isLegacyUdiskUefi = true;
                        Write.RemoveableDiskUefiGpt();
                        FinishSuccessful();


                    }
                    else
                    {
                        //MsgManager.getResString("Msg_UefiFormatWarning")
                        //您所选择的是UEFI模式，此模式将会格式化您的整个移动磁盘！\n注意是整个磁盘！！！\n程序将会删除所有优盘分区！
                        //if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_UefiFormatWarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }



                        DiskOperation.DiskPartGPTAndUEFI(WTGModel.efiPartitionSize.ToString(), WTGModel.ud, WTGModel.partitionSize);

                        //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartScriptPath + "\\uefi.txt\"");
                        if (radiobtnLegacy.Checked)
                        {
                            //UEFI+GPT 传统
                            if (Write.UefiGptTypical())
                            { FinishSuccessful(); }


                        }
                        else // UEFI+GPT VHD、VHDX模式
                        {

                            if (Write.UefiGptVhdVhdx())
                            { FinishSuccessful(); }
                        }
                    }
                }
                else if (checkBoxUefimbr.Checked)
                {
                    //UEFI+MBR
                    if (WTGModel.udString.Contains("Removable Disk"))
                    {
                        WTGModel.isLegacyUdiskUefi = true;
                        Write.RemoveableDiskUefiMbr();
                        FinishSuccessful();

                        //MessageBox.Show(MsgManager.GetResString("Msg_UefiError", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //VisitWeb("http://bbs.luobotou.org/thread-6506-1-1.html");
                        //return;
                    }
                    //if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_UefiFormatWarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
                    else
                    {
                        DiskpartScriptManager dsm = new DiskpartScriptManager();
                        DiskOperation.GenerateMBRAndUEFIScript(WTGModel.efiPartitionSize.ToString(), WTGModel.ud, WTGModel.partitionSize);

                        //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartScriptPath + "\\uefimbr.txt\"");

                        if (radiobtnLegacy.Checked)
                        {
                            if (Write.UEFIMBRTypical())
                            {
                                FinishSuccessful();
                            }

                        }
                        else //uefi MBR VHD、VHDX模式
                        {
                            if (Write.UefiMbrVHDVHDX())
                            {
                                FinishSuccessful();
                            }
                        }
                    }
                    //MessageBox.Show("UEFI模式写入完成！\n请重启电脑用优盘启动\n如有问题，可去论坛反馈！", "完成啦！", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else //非UEFI模式
                {
                    //传统
                    #region 格式化
                    if (WTGModel.udString.Contains("Removable Disk") && radiobtnLegacy.Checked)
                    {
                        if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_Legacywarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        {
                            return;
                        }
                    }

                    if (!checkBoxDiskpart.Checked && !WTGModel.doNotFormat)//普通格式化
                    {
                        ProcessManager.ECMD("cmd.exe", "/c format " + WTGModel.ud.Substring(0, 2) + "/FS:ntfs /q /V: /Y");
                        //
                    }
                    else if (checkBoxDiskpart.Checked)
                    {
                        DiskOperation.DiskPartRePartitionUD(WTGModel.partitionSize);

                    }

                    #endregion
                    ///////////////////////////////////正式开始////////////////////////////////////////////////
                    if (radiobtnLegacy.Checked)
                    {
                        if (Write.NonUEFITypical(false))
                        {
                            FinishSuccessful();
                        }

                    }
                    else //非UEFI VHD VHDX
                    {
                        if (Write.NonUEFIVHDVHDX(false))
                        {
                            FinishSuccessful();
                        }
                    }
                }

            }
            catch (UserCancelException ex)
            {
                Log.WriteLog("Err_UserCancelException", ex.ToString());
                ErrorMsg em = new ErrorMsg(ex.Message);
                em.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_Exception", ex.ToString());

                ErrorMsg em = new ErrorMsg(ex.Message);
                em.ShowDialog();
            }
            finally
            {
                writeSw.Stop();
                SystemSleepManagement.ResotreSleep();
            }
        }

        private void FinishSuccessful()
        {
            if (WTGModel.noDefaultDriveLetter && !WTGModel.udString.Contains("Removable Disk"))
            {
                DiskOperation.SetNoDefaultDriveLetter(WTGModel.ud);
            }
            writeSw.Stop();

            Finish f = new Finish(writeSw.Elapsed);
            f.ShowDialog();
        }


        private void WriteProgramRunInfoToLog()
        {
            WTGModel.CreateGuid = Guid.NewGuid().ToString();
            //try
            //{

            Dictionary<string, string> infoDict = new Dictionary<string, string>();
            infoDict.Add("App Path", Application.StartupPath);
            infoDict.Add("OS Version", Environment.OSVersion.ToString());
            infoDict.Add("Dism Version", FileOperation.GetFileVersion(Environment.GetEnvironmentVariable("windir") + "\\System32\\dism.exe"));
            infoDict.Add("Wim file", WTGModel.imageFilePath);
            infoDict.Add("Usb Disk", WTGModel.udString);
            infoDict.Add("Typical", radiobtnLegacy.Checked.ToString());
            infoDict.Add("VHD", radiobtnVhd.Checked.ToString());
            infoDict.Add("VHDX", radiobtnVhdx.Checked.ToString());
            infoDict.Add("VHDName", WTGModel.vhdNameWithoutExt);
            infoDict.Add("Re-Partition", checkBoxDiskpart.Checked.ToString());
            infoDict.Add("VHD Size Set", WTGModel.userSetSize.ToString());
            infoDict.Add("Fixed VHD", WTGModel.isFixedVHD.ToString());
            infoDict.Add("Donet", WTGModel.installDonet35.ToString());
            infoDict.Add("Disable-WinRE", WTGModel.disableWinRe.ToString());
            infoDict.Add("Block Local Disk", WTGModel.isBlockLocalDisk.ToString());
            infoDict.Add("NoTemp", WTGModel.isNoTemp.ToString());
            infoDict.Add("UEFI+GPT", checkBoxUefigpt.Checked.ToString());
            infoDict.Add("UEFI+MBR", checkBoxUefimbr.Checked.ToString());
            infoDict.Add("WIMBOOT", WTGModel.isWimBoot.ToString());
            infoDict.Add("CompactOS", WTGModel.isCompactOS.ToString());
            infoDict.Add("No-format", WTGModel.doNotFormat.ToString());
            infoDict.Add("NtfsUefiSupport", WTGModel.ntfsUefiSupport.ToString());
            infoDict.Add("FixLetter", WTGModel.fixLetter.ToString());
            infoDict.Add("SelectedPart", WTGModel.wimPart.ToString());
            infoDict.Add("Partitions", string.Join(",", WTGModel.partitionSize));
            infoDict.Add("NoDefalutLetter", WTGModel.noDefaultDriveLetter.ToString());
            infoDict.Add("Bitlocker", WTGModel.isBitlocker.ToString());
            infoDict.Add("CreateGuid", WTGModel.CreateGuid);

            Thread t = new Thread(() =>
            {
                try
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();

                    HttpPost.Post("http://laa.luobotou.org/wtgstats.ashx", new Dictionary<string, string>() {
                    {"type","create" },
                    {"guid",WTGModel.CreateGuid },
                    {"paras",jss.Serialize(infoDict) }

            });
                }
                catch (Exception ex)
                {
                    Log.WriteLog("Err_Stats", ex.ToString());
                }
            });
            t.Start();

            StringBuilder sb = new StringBuilder();
            foreach (var item in infoDict)
            {
                sb.AppendLine(item.Key + ":" + item.Value);
            }
            Log.WriteLog("Environment", sb.ToString());
            //Log.WriteLog("Environment",
            //    "App Path:" + Application.StartupPath +
            //    "\r\nOSVersion:" + Environment.OSVersion.ToString() +
            //    "\r\nDism Version:" + FileOperation.GetFileVersion(System.Environment.GetEnvironmentVariable("windir") + "\\System32\\dism.exe") +
            //    "\r\nWim file:" + lblWim.Text +
            //    "\r\nUsb Disk:" + WTGModel.udString +
            //    "\r\nTypical:" + radiobtnLegacy.Checked.ToString() +
            //    "\r\nVHD:" + radiobtnVhd.Checked.ToString() +
            //    "\r\nVHDX:" + radiobtnVhdx.Checked.ToString() +
            //    "\r\nVHDName: " + WTGModel.vhdNameWithoutExt +
            //    "\r\nRe-Partition:" + checkBoxDiskpart.Checked +
            //    "\r\nVHD Size Set:" + numericUpDown1.Value.ToString() +
            //    "\r\nFixed VHD:" + checkBoxFixed.Checked.ToString() +
            //    "\r\nDonet:" + WTGModel.installDonet35.ToString() +
            //    "\r\nDisable-WinRE:" + WTGModel.disableWinRe.ToString() +
            //    "\r\nBlock Local Disk:" + checkBoxSan_policy.Checked.ToString() +
            //    "\r\nNoTemp:" + checkBoxNotemp.Checked.ToString() +
            //    "\r\nUEFI+GPT:" + checkBoxUefigpt.Checked.ToString() +
            //    "\r\nUEFI+MBR:" + checkBoxUefimbr.Checked.ToString() +
            //    "\r\nWIMBOOT:" + checkBoxWimboot.Checked.ToString() +
            //    "\r\nCompactOS:" + checkBoxCompactOS.Checked.ToString() +
            //    "\r\nNo-format:" + WTGModel.doNotFormat.ToString() +
            //    "\r\nNtfsUefiSupport:" + WTGModel.ntfsUefiSupport.ToString() +
            //    "\r\nFixLetter:" + WTGModel.fixLetter.ToString() +
            //    "\r\nSelectedPart:" + WTGModel.wimPart.ToString() +
            //    "\r\nPartitions:" + string.Join(",", WTGModel.partitionSize) +
            //    "\r\nNoDefalutLetter: " + WTGModel.noDefaultDriveLetter.ToString()

            //    //    );
            //}
            //catch (Exception ex) { MessageBox.Show("Error!\n" + ex.ToString()); }

            //if (File.Exists(Environment.GetEnvironmentVariable("windir") + "\\Logs\\DISM\\dism.log"))
            //{
            //    File.Copy(Environment.GetEnvironmentVariable("windir") + "\\Logs\\DISM\\dism.log", WTGModel.logPath + "\\dism.log");
            //}
        }





        #endregion



        #region MenuItemClick/Miscellaneous
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427");
            //System.Diagnostics.Process.Start("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427");
        }
        private void 打开程序运行目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string adLink = linkLabel2.Tag as string;
            if (!string.IsNullOrEmpty(adLink)) VisitWeb(adLink);
        }

        private void wimbox_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            //bool mount_successfully = false;

            if (File.Exists(openFileDialog1.FileName))
            {
                //SystemDetection();
                lblWim.Text = openFileDialog1.FileName;
                WTGModel.imageFilePath = openFileDialog1.FileName;
                //Regex.Match ("",@".+\.[")
                WTGModel.choosedImageType = Path.GetExtension(openFileDialog1.FileName.ToLower()).Substring(1);
                //MessageBox.Show(WTGOperation.filetype);
                //if (WTGModel.choosedImageType == "iso")
                //{

                //    //ProcessManager.ECMD(Application.StartupPath + "\\isocmd.exe  -i");
                //    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -i");
                //    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -s");
                //    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -NUMBER 1");
                //    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -eject 0: ");
                //    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -MOUNT 0: \"" + openFileDialog1.FileName + "\"");
                //    //mount.WaitForExit();
                //    for (int i = 68; i <= 90; i++)
                //    {
                //        string ascll_to_eng = Convert.ToChar(i).ToString();
                //        if (File.Exists(ascll_to_eng + ":\\sources\\install.wim"))
                //        {
                //            lblWim.Text = ascll_to_eng + ":\\sources\\install.wim";
                //            mount_successfully = true;
                //            break;
                //        }
                //    }
                //    if (!mount_successfully)
                //    {
                //        MessageBox.Show("虚拟光驱加载失败，请手动加载，之后选择install.wim");
                //    }
                //    else
                //    {
                //        //useiso = true;
                //    }
                //}
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
                        //MessageBox.Show("Test");
                        SystemDetection(false);
                        WTGModel.isEsd = true;
                        checkBoxWimboot.Checked = false;
                        checkBoxWimboot.Enabled = false;
                    }

                }
                else if (WTGModel.choosedImageType == "vhd")
                {
                    if (!radiobtnVhd.Enabled)
                    {
                        radiobtnVhd.Checked = true;
                        radiobtnLegacy.Enabled = false;
                        radiobtnVhdx.Enabled = false;
                    }
                    else
                    {
                        radiobtnVhd.Checked = true;
                    }
                }
                else if (WTGModel.choosedImageType == "vhdx")
                {
                    if (!radiobtnVhdx.Enabled)
                    {
                        radiobtnVhdx.Checked = true;
                        radiobtnLegacy.Enabled = false;
                        radiobtnVhd.Enabled = false;

                        checkBoxCompactOS.Checked = true;
                        checkBoxCompactOS.Enabled = false;

                    }
                    else
                    {
                        radiobtnLegacy.Enabled = false;
                        radiobtnVhd.Enabled = false;

                        radiobtnVhdx.Checked = true;
                        //MessageBox.Show("Test");
                    }

                }
                else
                {
                    SystemDetection(false);
                    WTGModel.win7togo = ImageOperation.Iswin7(WTGModel.imagexFileName, lblWim.Text);
                    if (WTGModel.win7togo != 0) //WIN7 cannot comptible with VHDX disk or wimboot
                    {
                        if (radiobtnVhdx.Checked)
                        {
                            radiobtnVhd.Checked = true;
                        }
                        radiobtnVhdx.Enabled = false;
                        checkBoxWimboot.Checked = false;
                        checkBoxWimboot.Enabled = false;
                    }
                }
                if (Regex.IsMatch(WTGModel.choosedImageType, @"wim|esd") && WTGModel.CurrentOS != OS.XP && WTGModel.CurrentOS != OS.Vista)
                {
                    comboBoxParts.Items.Clear();
                    comboBoxParts.Items.Add("0 : 自动选择");
                    comboBoxParts.Items.AddRange(ImageOperation.DismGetImagePartsInfo(lblWim.Text).ToArray());
                    comboBoxParts.SelectedIndex = 0;
                    //////////////////////////////////dism / Get - ImageInfo
                    //////////////////////////////////#warning 为实现此代码
                }

            }

        }



        private void 萝卜头IT论坛ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org");
        }

        private void checkBoxuefi_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUefimbr.Checked && checkBoxUefigpt.Checked)
            {
                checkBoxUefimbr.Checked = false;
                checkBoxUefigpt.Checked = true;
            }
            checkBoxDiskpart.Enabled = !checkBoxUefigpt.Checked;
            checkBoxDiskpart.Checked = checkBoxUefigpt.Checked;
            if (checkBoxBitlocker.Checked)
            {
                radiobtnLegacy.Checked = true;
                radiobtnVhd.Enabled = false;
                radiobtnVhdx.Enabled = false;
            }

            if ((checkBoxUefigpt.Checked || checkBoxUefimbr.Checked) && !radiobtnVhdx.Checked && !radiobtnVhd.Checked)
            {
                checkBoxBitlocker.Enabled = true;
            }
            else
            {
                checkBoxBitlocker.Checked = false;
                checkBoxBitlocker.Enabled = false;
            }
        }

        private void radiovhd_EnabledChanged(object sender, EventArgs e)
        {

            radiobtnLegacy.Checked = true;
        }



        private void linkLabel3_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-3566-1-1.html");
            //System.Diagnostics.Process.Start("http://bbs.luobotou.org/thread-3566-1-1.html");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-6098-1-1.html");
        }

        private void 在线帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427");

        }

        private void 官方论坛ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/forum-88-1.html");

        }

        private void diskpart重新分区ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MsgManager.getResString("Msg_chooseud")
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
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
                DiskOperation.DiskPartRePartitionUD(WTGModel.partitionSize);
                //diskPart();
                MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_Exception", ex.ToString());

                ErrorMsg em = new ErrorMsg(ex.Message);
                em.Show();
            }

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void 打开程序运行目录ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath);
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("萝卜头IT论坛 nkc3g4制作\nQQ:1443112740\nEmail:microsoft5133@126.com","关于");
            AboutBox abx = new AboutBox();
            abx.Show();
        }

        private void vHDUEFIBCDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
        }

        private void 自动检查更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SWOnline swo = new SWOnline(releaseUrl, reportUrl);
            Thread threadUpdate = new Thread(swo.Update);
            threadUpdate.Start();

            //threadupdate = new Thread(SWOnline.update);
            //threadupdate.Start();
            //MsgManager.getResString("Msg_UpdateTip")
            //若无弹出窗口，则当前程序已是最新版本！
            MessageBox.Show(MsgManager.GetResString("Msg_UpdateTip", MsgManager.ci));
        }

        private void checkBoxdiskpart_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDiskpart.Checked)
            {
                //Msg_Repartition
                //MessageBox.Show(MsgManager.GetResString("Msg_Repartition", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                WTGModel.doNotFormat = false;
                checkBoxDoNotFormat.Checked = false;
                checkBoxDoNotFormat.Enabled = false;
            }
            else
            {
                checkBoxDoNotFormat.Enabled = true;
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

        private void wIN7USBBOOTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";
            ImageOperation.Win7REG(@"V:\\");
        }

        private void bOOTICEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(WTGModel.applicationFilesPath + "\\BOOTICE.EXE");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(WTGModel.logPath);

        }

        private void checkBoxuefimbr_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUefigpt.Checked && checkBoxUefimbr.Checked) { checkBoxUefigpt.Checked = false; checkBoxUefimbr.Checked = true; }
            if (checkBoxUefimbr.Checked) { WTGModel.disableWinRe = true; }
            checkBoxDiskpart.Enabled = !checkBoxUefimbr.Checked;
            checkBoxDiskpart.Checked = checkBoxUefimbr.Checked;
            if (checkBoxBitlocker.Checked)
            {
                radiobtnLegacy.Checked = true;
                radiobtnVhd.Enabled = false;
                radiobtnVhdx.Enabled = false;
            }

            if ((checkBoxUefigpt.Checked || checkBoxUefimbr.Checked) && !radiobtnVhdx.Checked && !radiobtnVhd.Checked)
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
                IniFile.WriteVal("Main", "AutoUpdate", "1", settingFilePath);
            }
            else
            {
                IniFile.WriteVal("Main", "AutoUpdate", "0", settingFilePath);

            }
        }


        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-1625-1-1.html");

        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("This program will restart,continue?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }

            IniFile.WriteVal("Main", "Language", "EN", settingFilePath);
            Application.Restart();
        }

        private void chineseSimpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("程序将会重启，确认继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }
            IniFile.WriteVal("Main", "Language", "ZH-HANS", settingFilePath);
            Application.Restart();

        }

        private void 繁体中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("程序將會重啟，確認繼續？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }
            IniFile.WriteVal("Main", "Language", "ZH-HANT", settingFilePath);
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

        private void linkLabel3_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-3566-1-1.html");

        }

        private void linkLabel5_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-6098-1-1.html");

        }
        #endregion

        public static void VisitWeb(string url)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                string s = key.GetValue("").ToString();

                Regex reg = new Regex("\"([^\"]+)\"");
                MatchCollection matchs = reg.Matches(s);

                string filename = "";
                if (matchs.Count > 0)
                {
                    filename = matchs[0].Groups[1].Value;
                    System.Diagnostics.Process.Start(filename, url);
                }
            }
            catch (Exception ex)
            {
                //MsgManager.getResString("Msg_FatalError")
                //程序遇到严重错误\n官方支持论坛：bbs.luobotou.org\n
                MessageBox.Show("程序遇到严重错误\nFATAL ERROR!官方支持论坛：bbs.luobotou.org\n" + ex.ToString());

            }


        }
        #region UserControls

        private void Form1_Load(object sender, EventArgs e)
        {

            toolStripMenuItem3.Checked = autoCheckUpdate;
            FileValidation();
            SystemDetection(true);

            //timer1.Start();//UdList 刷新

            SWOnline swo = new SWOnline(releaseUrl, reportUrl);
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


            Thread threadReport = new Thread(swo.Report);
            threadReport.Start();


            GetUdiskList.LoadUDList(comboBoxUd);
            comboBoxParts.Items.Clear();
            comboBoxParts.Items.Add("0 : 自动选择");
            comboBoxParts.SelectedIndex = 0;
            comboBoxVhdPartitionType.SelectedIndex = 1;
            comboBoxGb.SelectedIndex = 1;
            //txtVhdTempPath.text

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
                    //MessageBox.Show(MsgManager.GetResString("Msg_WriteProcessing", MsgManager.ci));
                    //return;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Msg_WriteProcessing", ex.ToString());
                //Console.WriteLine(ex);
            }
            if (radiobtnLegacy.Checked)
            {
                WTGModel.CheckedMode = ApplyMode.Legacy;
            }
            else if (radiobtnVhd.Checked)
            {
                WTGModel.CheckedMode = ApplyMode.VHD;
            }
            else
            {
                WTGModel.CheckedMode = ApplyMode.VHDX;
            }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//
            WTGModel.UdObj = (UsbDisk)comboBoxUd.SelectedItem;
            WTGModel.udString = comboBoxUd.SelectedItem.ToString();
            WTGModel.isBitlocker = checkBoxBitlocker.Checked;
            WTGModel.isWimBoot = checkBoxWimboot.Checked;
            WTGModel.isBlockLocalDisk = checkBoxSan_policy.Checked;
            WTGModel.imageFilePath = lblWim.Text;
            WTGModel.isCompactOS = checkBoxCompactOS.Checked;
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
            WTGModel.isFixedVHD = checkBoxFixed.Checked;
            WTGModel.isUefiGpt = checkBoxUefigpt.Checked;
            WTGModel.isUefiMbr = checkBoxUefimbr.Checked;
            WTGModel.isNoTemp = checkBoxNotemp.Checked;
            WTGModel.ntfsUefiSupport = checkBoxNtfsUefi.Checked;
            WTGModel.doNotFormat = checkBoxDoNotFormat.Checked;
            WTGModel.vhdNameWithoutExt = txtVhdNameWithoutExt.Text;
            WTGModel.installDonet35 = checkBoxDonet.Checked;
            WTGModel.fixLetter = checkBoxFixLetter.Checked;
            WTGModel.noDefaultDriveLetter = checkBoxNoDefaultLetter.Checked;
            WTGModel.disableWinRe = checkBoxDisWinre.Checked;
            WTGModel.efiPartitionSize = txtEfiSize.Text;
            WTGModel.vhdPartitionType = comboBoxVhdPartitionType.SelectedText;
            WTGModel.vhdTempPath = txtVhdTempPath.Text;
            WTGModel.partitionSize = new string[3];
            WTGModel.partitionSize[0] = txtPartitionSize1.Text;
            WTGModel.partitionSize[1] = txtPartitionSize2.Text;
            WTGModel.partitionSize[2] = txtPartitionSize3.Text;
            WTGModel.wimPart = comboBoxParts.SelectedItem.ToString().Substring(0, 1);
            if (radiobtnVhdx.Checked)
            {
                WTGModel.vhdExtension = "vhdx";
            }
            WTGModel.win8VHDFileName = WTGModel.vhdNameWithoutExt + "." + WTGModel.vhdExtension;
            tWrite = new Thread(new ThreadStart(GoWrite));
            tWrite.Start();

        }

        private void isobutton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (File.Exists(openFileDialog1.FileName)) { lblWim.Text = openFileDialog1.FileName; }
        }
        private void ManualSelectUdisk()
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath.Length != 3)
            {
                if (folderBrowserDialog1.SelectedPath.Length != 0)
                {
                    //MsgManager.getResString("Msg_UDRoot")
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
        private void button2_Click(object sender, EventArgs e)
        {


            //checkBoxUefigpt.Enabled = false;
            //checkBoxUefimbr.Enabled = false;
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
        private byte[] UlongToMBRBytes(ulong u)
        {
            string convertTempString = Convert.ToString(Convert.ToInt32(u), 16);

            for (int i = convertTempString.Length; i < 8; i++)
            {
                convertTempString = "0" + convertTempString;
            }
            //MessageBox.Show(convertTempString);

            //MessageBox.Show(convertTempString.Length.ToString());
            string hex1 = convertTempString[6].ToString() + convertTempString[7];
            string hex2 = convertTempString[4].ToString() + convertTempString[5];
            string hex3 = convertTempString[2].ToString() + convertTempString[3];
            string hex4 = convertTempString[0].ToString() + convertTempString[1];
            byte[] byts = new byte[4];
            byts[0] = Convert.ToByte(Convert.ToInt32(hex1, 16));
            byts[1] = Convert.ToByte(Convert.ToInt32(hex2, 16));
            byts[2] = Convert.ToByte(Convert.ToInt32(hex3, 16));
            byts[3] = Convert.ToByte(Convert.ToInt32(hex4, 16));
            return byts;
        }

        private void 错误提示测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            checkBoxNotemp.Enabled = false;
            lblVhdSize.Enabled = false;

            lblVhdPartitionTableType.Enabled = false;
            comboBoxVhdPartitionType.Enabled = false;
            lblVhdTempPath.Enabled = false;
            txtVhdTempPath.Enabled = false;
            btnVhdTempPath.Enabled = false;

            comboBoxGb.Enabled = false;
            trackBar1.Enabled = false;
            lblVhdName.Enabled = false;
            txtVhdNameWithoutExt.Enabled = false;

            if (checkBoxBitlocker.Checked)
            {
                radiobtnLegacy.Checked = true;
                radiobtnVhd.Enabled = false;
                radiobtnVhdx.Enabled = false;
            }

            if ((checkBoxUefigpt.Checked || checkBoxUefimbr.Checked) && !radiobtnVhdx.Checked && !radiobtnVhd.Checked)
            {
                checkBoxBitlocker.Enabled = true;
            }
            else
            {
                checkBoxBitlocker.Checked = false;
                checkBoxBitlocker.Enabled = false;
            }
        }

        private void radiovhd_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxBitlocker.Checked = false;
            checkBoxBitlocker.Enabled = false;
            numericUpDown1.Enabled = true;
            checkBoxFixed.Enabled = true;
            checkBoxNotemp.Enabled = true;
            lblVhdSize.Enabled = true;
            comboBoxGb.Enabled = true;
            lblVhdPartitionTableType.Enabled = true;
            comboBoxVhdPartitionType.Enabled = true;
            lblVhdTempPath.Enabled = true;
            txtVhdTempPath.Enabled = true;
            btnVhdTempPath.Enabled = true;

            if (comboBoxUd.SelectedIndex != 0 && comboBoxUd.SelectedIndex != -1)
            {
                trackBar1.Enabled = true;
            }
            lblVhdName.Enabled = true;
            txtVhdNameWithoutExt.Enabled = true;


        }

        private void radiovhdx_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxBitlocker.Checked = false;
            checkBoxBitlocker.Enabled = false;
            numericUpDown1.Enabled = true;
            checkBoxFixed.Enabled = true;
            checkBoxNotemp.Enabled = true;
            lblVhdSize.Enabled = true;
            lblVhdPartitionTableType.Enabled = true;
            comboBoxVhdPartitionType.Enabled = true;
            lblVhdTempPath.Enabled = true;
            txtVhdTempPath.Enabled = true;
            btnVhdTempPath.Enabled = true;
            if (comboBoxUd.SelectedIndex != 0 && comboBoxUd.SelectedIndex != -1)
            {
                trackBar1.Enabled = true;
            }
            comboBoxGb.Enabled = true;
            lblVhdName.Enabled = true;
            txtVhdNameWithoutExt.Enabled = true;

        }

        #endregion



        private void txtVhdTempPath_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip1.SetToolTip(txtVhdTempPath, txtVhdTempPath.Text); ;
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_txtVhdTempPath_MouseHover", ex.ToString());
            }
        }

        private void btnVhdTempPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                txtVhdTempPath.Text = folderBrowserDialog1.SelectedPath;
            }

        }

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
            txtPartitionSize1.Text = "0";
            txtPartitionSize2.Text = "0";

        }



        private void comboBoxUd_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBoxUd.SelectedIndex.ToString());

            if (comboBoxUd.SelectedIndex != 0 && comboBoxUd.SelectedIndex != -1)
            {
                WTGModel.UdObj = (UsbDisk)comboBoxUd.SelectedItem;
                udSizeInMB = (int)(WTGModel.UdObj.DiskSize / 1048576);
                txtPartitionSize3.Text = udSizeInMB.ToString();
                trackBar1.Enabled = true;
                if (!WTGModel.UdObj.DriveType.Contains("Removable Disk"))
                {
                    txtPartitionSize1.Enabled = true;
                    txtPartitionSize2.Enabled = true;
                    txtPartitionSize3.Enabled = true;
                }
                else
                {
                    txtPartitionSize1.Enabled = false;
                    txtPartitionSize2.Enabled = false;
                    txtPartitionSize3.Enabled = false;

                }
                if (WTGModel.UdObj.DiskSize == 0)
                {
                    checkBoxUefimbr.Checked = false;
                    checkBoxUefigpt.Checked = false;
                    checkBoxUefimbr.Enabled = false;
                    checkBoxUefigpt.Enabled = false;
                }
                else
                {
                    checkBoxUefimbr.Enabled = true;
                    checkBoxUefigpt.Enabled = true;
                }
            }
            else
            {
                txtPartitionSize1.Enabled = false;
                txtPartitionSize2.Enabled = false;
                txtPartitionSize3.Enabled = false;

            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void txtPartitionSize1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPartitionSize1.Text))
            {
                txtPartitionSize1.Text = "0";
            }
            int remain = udSizeInMB - int.Parse(txtPartitionSize2.Text) - int.Parse(txtPartitionSize1.Text);
            if (remain < 0)
            {
                txtPartitionSize1.Text = (udSizeInMB - int.Parse(txtPartitionSize2.Text)).ToString();
                txtPartitionSize3.Text = "0";
            }
            else
            {
                txtPartitionSize3.Text = remain.ToString();
            }
        }

        private void txtPartitionSize2_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPartitionSize2.Text))
            {
                txtPartitionSize2.Text = "0";
            }
            int remain = udSizeInMB - int.Parse(txtPartitionSize2.Text) - int.Parse(txtPartitionSize1.Text);
            if (remain < 0)
            {
                txtPartitionSize2.Text = (udSizeInMB - int.Parse(txtPartitionSize1.Text)).ToString();
                txtPartitionSize3.Text = "0";

            }
            else
            {
                txtPartitionSize3.Text = (udSizeInMB - int.Parse(txtPartitionSize2.Text) - int.Parse(txtPartitionSize1.Text)).ToString();
            }

        }

        private void linklblTabPage4Resotre_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            comboBoxVhdPartitionType.SelectedIndex = 1;
            txtVhdTempPath.Text = Path.GetTempPath();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("http://bbs.luobotou.org");
        }

        private void linkLabel5_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=6098");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=6098");

        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=6098");

        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=6098");

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
                radiobtnVhd.Enabled = false;
                radiobtnVhdx.Enabled = false;
            }
            else
            {
                SystemDetection(false);
            }

            if ((checkBoxUefigpt.Checked || checkBoxUefimbr.Checked) && !radiobtnVhdx.Checked && !radiobtnVhd.Checked)
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
    }
}
