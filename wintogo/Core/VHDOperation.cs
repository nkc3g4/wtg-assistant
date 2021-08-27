using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace wintogo
{
    public class VHDOperation
    {
        public VHDOperation()
        {
            //ShouldContinue = true;
            //NeedTwiceAttach = false;
            SetVhdProp();
        }
        //private bool NeedTwiceAttach { get; set; }
        /// <summary>
        /// vhd or vhdx
        /// </summary>
        /// 
        public string ExtensionType { get; set; }
        public string VhdPath { get; set; }
        public string VhdSize { get; set; }

        /// <summary>
        /// vhdType: fixed or ex...
        /// </summary>
        public string VhdType { get; set; }
        public bool NeedCopy { get; set; }
        public string VhdFileName { get; set; }
        private static readonly string vhdDefaultSize = "40960";

        public static void CleanTemp()
        {

            ProcessManager.SyncCMD("taskkill.exe /f /IM imagex_x86.exe");
            ProcessManager.SyncCMD("taskkill.exe /f /IM imagex_x64.exe");
            ProcessManager.SyncCMD("taskkill.exe /f /IM dism.exe");
            ProcessManager.SyncCMD("taskkill.exe /f /IM diskpart.exe");

            //KILL.Start();
            //KILL.WaitForExit();

            if (Directory.Exists("V:\\"))
            {
                DetachVHDExtra();

            }
            if (Directory.Exists("V:\\"))
            {
                DetachVHDExtra();
                //MsgManager.getResString("Msg_LetterV")
                //盘符V不能被占用！

            }
            //DiskOperation.CheckDiskExists(@"V:\\");
            if (Directory.Exists("V:\\"))
            {
                Log.WriteLog("Err_CleanTemp", "Directory.Exists(V)");

                //ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_LetterV", MsgManager.ci));
                //er.ShowDialog();
                throw new VHDException(MsgManager.GetResString("Msg_LetterV", MsgManager.ci));
            }
            //if (useiso) { ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\" + "\\isocmd.exe\" -eject 0: "); }
            try
            {
                //File.Delete ()

                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\create.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\removex.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\detach.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\uefi.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\uefimbr.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\dp.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\attach.txt");
                //FileOperation.DeleteFile(WTGModel.vhdTempPath + "\\win8.vhd");
                //FileOperation.DeleteFile(WTGModel.vhdTempPath + "\\win8.vhdx");
                FileOperation.DeleteFile(WTGModel.vhdTempPath + "\\" + WTGModel.vhdNameWithoutExt + ".vhd");
                FileOperation.DeleteFile(WTGModel.vhdTempPath + "\\" + WTGModel.vhdNameWithoutExt + ".vhdx");

            }
            catch (Exception ex)
            {
                //MsgManager.getResString("Msg_DeleteTempError")
                //程序删除临时文件出错！可重启程序或重启电脑重试！\n
                //ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_DeleteTempError", MsgManager.ci));
                //er.ShowDialog();
                Log.WriteLog("Err_DeleteVHDTemp", ex.ToString());

                throw new VHDException(MsgManager.GetResString("Msg_DeleteTempError", MsgManager.ci));

                //MessageBox.Show(MsgManager.GetResString("Msg_DeleteTempError", MsgManager.ci) + ex.ToString());
                //shouldcontinue = false;
            }

        }
        public static void DetachVHD(string vhdPath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select vdisk file=\"" + vhdPath + "\"");
            sb.AppendLine("detach vdisk");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

        }
        public void DetachVHD()
        {
            DetachVHD(this.VhdPath);
        }
        public static void DetachVHDExtra()
        {
            DiskpartScriptManager dsm = new DiskpartScriptManager(true);
            dsm.Args = "list vdisk";
            dsm.RunDiskpartScript();
            //MessageBox.Show(dsm.outputFilePath);
            string dpoutput = File.ReadAllText(dsm.OutputFilePath, Encoding.Default);
            //MessageBox.Show(dpoutput);
            //MessageBox.Show(dpoutput);
            MatchCollection mc = Regex.Matches(dpoutput, @"([a-z]:\\.*" + WTGModel.vhdNameWithoutExt + ".vhd[x]?)", RegexOptions.IgnoreCase);
            foreach (Match item in mc)
            {
                //MessageBox.Show(item.Groups[1].Value);
                DetachVHD(item.Groups[1].Value);
            }
            dsm.DeleteOutputFile();
        }


        public void VHDDynamicSizeIns()
        {
            //if (!this.ShouldContinue) return;
            //MsgManager.getResString("FileName_VHD_Dynamic")
            //VHD模式说明.TXT
            using (FileStream fs1 = new FileStream(WTGModel.ud + MsgManager.GetResString("FileName_VHD_Dynamic", MsgManager.ci), FileMode.Create, FileAccess.Write))
            {
                fs1.SetLength(0);
                using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default))
                {
                    //try
                    //{
                    //MsgManager.getResString("Msg_VHDDynamicSize")
                    //您创建的VHD为动态大小VHD，实际VHD容量：
                    ////MsgManager.getResString("Msg_VHDDynamicSize2")
                    //在VHD系统启动后将自动扩充为实际容量。请您在优盘留有足够空间确保系统正常启动！
                    sw1.WriteLine(MsgManager.GetResString("Msg_VHDDynamicSize", MsgManager.ci) + this.VhdSize + "MB\n");
                    sw1.WriteLine(MsgManager.GetResString("Msg_VHDDynamicSize2", MsgManager.ci));
                }
                //}
                //catch { }
                //sw1.Close();
            }

        }

        public void AttachVHD()
        {
            AttachVHD(this.VhdPath);
        }
        public void AttachVHD(string vhdPath)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("select vdisk file=\"" + vhdPath + "\"");
            sb.AppendLine("attach vdisk");
            sb.AppendLine("sel partition 1");
            sb.AppendLine("assign letter=v");
            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

            //GenerateAttachVHDScript(this.vhdPath);
            //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGModel.diskpartScriptPath + "\\attach.txt\"");
        }
        public void CreateVHD()
        {
            StringBuilder sb = new StringBuilder();
            if (WTGModel.choosedImageType == "vhd" || WTGModel.choosedImageType == "vhdx")
            {
                sb.AppendLine("select vdisk file=\"" + VhdPath + "\"");
                sb.AppendLine("attach vdisk");
                sb.AppendLine("assign letter=v");
                sb.AppendLine("exit");

            }
            else
            {
                sb.AppendLine("create vdisk file=\"" + this.VhdPath + "\" type=" + this.VhdType + " maximum=" + this.VhdSize);
                sb.AppendLine("select vdisk file=\"" + this.VhdPath + "\"");
                sb.AppendLine("attach vdisk");
                if (WTGModel.vhdPartitionType == 1)
                {
                    sb.AppendLine("convert gpt");
                }
                sb.AppendLine("create partition primary");
                sb.AppendLine("format fs=ntfs quick");
                sb.AppendLine("assign letter=v");
                sb.AppendLine("exit");
            }
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            Log.WriteLog("CMD_CreateVHDScript", sb.ToString());
            //MessageBox.Show(sb.ToString());
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

            //try
            //{
            if (!Directory.Exists("V:\\"))
            {
                throw new VHDException(MsgManager.GetResString("Msg_VHDCreationError"));

            }
            if (WTGModel.isBitlocker)
            {
                Write.EnableBitlocker("V:");
            }
            if (WTGModel.choosedImageType != "vhd" && WTGModel.choosedImageType != "vhdx")
            {
                ApplyToVdisk();
            }
            if (WTGModel.choosedImageType != "vhd" && WTGModel.choosedImageType != "vhdx" && !WTGModel.isWimBoot && !File.Exists(@"v:\windows\regedit.exe"))
            {
                throw new VHDException(MsgManager.GetResString("Msg_ApplyError"));
                //ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_ApplyError", MsgManager.ci));
                //er.ShowDialog();
                //this.ShouldContinue = false;

            }
            //else
            //{
            //    this.ShouldContinue = true;
            //}
        }
        public void VHDExtra()
        {
            ImageOperation.ImageExtra(WTGModel.installDonet35, WTGModel.isBlockLocalDisk, WTGModel.disableWinRe, WTGModel.skipOOBE, WTGModel.disableUasp, @"v:\", WTGModel.imageFilePath);
            UEFIAndWin7ToGo();
        }
        public void CopyVHD()
        {
            if (NeedCopy)
            {
                if (File.Exists(VhdPath))
                {
                    ProcessManager.AppendText(MsgManager.GetResString("Msg_Copy", MsgManager.ci));

                    if (File.Exists(Environment.GetEnvironmentVariable("windir") + "\\system32\\robocopy.exe"))
                    {
                        ProcessManager.ECMD(Environment.GetEnvironmentVariable("windir") + "\\system32\\robocopy.exe", "\"" + VhdPath.Substring(0, VhdPath.LastIndexOf("\\") + 1) + "\\\" " + WTGModel.ud + " " + Path.GetFileNameWithoutExtension(VhdPath) + "." + WTGModel.vhdExtension + " " + "/ETA ", MsgManager.GetResString("Msg_Copy", MsgManager.ci));

                    }
                    else
                    {
                        throw new FileNotFoundException("Can not find RoboCopy in %windir%");
                        //ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\fastcopy.exe", " /auto_close \"" + this.VhdPath + "\" /to=\"" + WTGModel.ud + "\"", MsgManager.GetResString("Msg_Copy", MsgManager.ci));
                    }

                    //BigFileCopy ()
                    //MsgManager.getResString("Msg_Copy")
                    //复制文件中...大约需要10分钟~1小时，请耐心等待！\r\n

                }
                if ((WTGModel.choosedImageType == "vhd" && !this.VhdPath.EndsWith("win8.vhd")) || (WTGModel.choosedImageType == "vhdx" && !this.VhdPath.EndsWith("win8.vhdx")))
                {
                    //Rename
                    //MsgManager.getResString("Msg_RenameError")
                    //重命名错误
                    try
                    {
                        File.Move(WTGModel.ud + this.VhdPath.Substring(this.VhdPath.LastIndexOf("\\") + 1), WTGModel.ud + WTGModel.vhdNameWithoutExt + "." + WTGModel.choosedImageType);
                    }
                    catch (Exception ex) { MessageBox.Show(MsgManager.GetResString("Msg_RenameError", MsgManager.ci) + ex.ToString()); }
                }
            }
        }
        public void WriteBootFiles()
        {
            if (!NeedCopy)//不需要拷贝，不需要二次加载
            {
                WriteBootFilesIntoUD();
            }
            //else
            //{
            //    NeedTwiceAttach = true;
            //}
        }
        private void FixBootFile()
        {
            if (WTGModel.isUefiGpt && !WTGModel.isLegacyUdiskUefi)
            {
                BootFileOperation.BcdeditFixBootFileVHD(WTGModel.espLetter+":\\", WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.UEFI);
            }
            else if (WTGModel.isUefiMbr && !WTGModel.isLegacyUdiskUefi)
            {
                BootFileOperation.BcdeditFixBootFileVHD(WTGModel.espLetter + ":\\", WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.UEFI);
                BootFileOperation.BcdeditFixBootFileVHD(WTGModel.espLetter + ":\\", WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.BIOS);
            }
            else if (WTGModel.isWimBoot)
            {
                BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.BIOS);
            }
            else
            {
                if (WTGModel.ntfsUefiSupport)
                {
                    BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.BIOS);
                    BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.UEFI);

                }
                else
                {
                    BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.BIOS);
                }
            }
        }
        private void WriteBootFilesIntoUD()
        {
            if (WTGModel.isUefiGpt && !WTGModel.isLegacyUdiskUefi)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.espLetter + ":\\", FirmwareType.UEFI);
                BootFileOperation.BcdeditFixBootFileVHD(WTGModel.espLetter + ":\\", WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.UEFI);
            }
            else if (WTGModel.isUefiMbr && !WTGModel.isLegacyUdiskUefi)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.espLetter + ":\\", FirmwareType.ALL);
                BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.espLetter + ":\\");
                BootFileOperation.BcdeditFixBootFileVHD(WTGModel.espLetter + ":\\", WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.UEFI);
                BootFileOperation.BcdeditFixBootFileVHD(WTGModel.espLetter + ":\\", WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.BIOS);
            }
            //else if (WTGModel.isWimBoot)
            //{
            //    BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.BIOS);
            //    BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.BIOS);
            //}
            else
            {
                if (WTGModel.isUserSetEfiPartition && Directory.Exists(WTGModel.efiPartition))
                {
                    BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.efiPartition, FirmwareType.ALL);
                    BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.efiPartition, WTGModel.win8VHDFileName, FirmwareType.BIOS);
                    BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.efiPartition, WTGModel.win8VHDFileName, FirmwareType.UEFI);

                }
                else
                {
                    if (WTGModel.ntfsUefiSupport)
                    {
                        BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.ALL);
                        BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.BIOS);
                        BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.UEFI);

                    }
                    else
                    {
                        BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.BIOS);
                        BootFileOperation.BcdeditFixBootFileVHD(WTGModel.ud, WTGModel.ud, WTGModel.win8VHDFileName, FirmwareType.BIOS);

                    }
                }
            }
        }

        public void Execute()
        {
            CleanTemp();
            try
            {
                if (WTGModel.choosedImageType == "vhd" || WTGModel.choosedImageType == "vhdx")
                {
                    CopyVHD();

                    TwiceAttachVHDAndWriteBootFile();

                }
                else
                {
                    CreateVHD();
                    VHDExtra();
                    WriteBootFiles();
                    DetachVHD();
                    CopyVHD();
                    Thread.Sleep(1500);
                    TwiceAttachVHDAndWriteBootFile();
                    if (VhdType != "fixed") VHDDynamicSizeIns();
                    FixBootFile();
                }
            }
            catch (UserCancelException)
            {
                throw;
            }
            catch (VHDException ex)
            {
                Log.WriteLog("Err_VHDException", ex.ToString());

                ErrorMsg em = new ErrorMsg(ex.Message, true);
                em.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("程序出现严重错误！\n" + ex.ToString());
                Log.WriteLog("Err_VHDFatalError", ex.ToString());
            }
        }

        private void TwiceAttachVHDAndWriteBootFile()
        {
            //MessageBox.Show(NeedTwiceAttach.ToString());
            if (!NeedCopy) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select vdisk file=" + StringUtility.Combine(WTGModel.ud, WTGModel.win8VHDFileName));
            sb.AppendLine("attach vdisk");
            sb.AppendLine("sel partition 1");
            sb.AppendLine("assign letter=v");
            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

            DiskOperation.CheckDiskExists("V:\\");

            if (!Directory.Exists("V:\\"))
            {
                Log.WriteLog("Err_TwiceAttachVhdError", "二次加载VHD失败");
                throw new VHDException(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
            }
            //需要二次加载，一定不是需要写入X盘的UEFI模式
            if (WTGModel.ntfsUefiSupport)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.ALL);

            }
            else
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.BIOS);
            }
            DetachVHD(StringUtility.Combine(WTGModel.ud, WTGModel.win8VHDFileName));

        }

        #region 私有方法
        private void SetVhdProp()
        {
            try
            {
                if (Path.GetExtension(WTGModel.imageFilePath) == ".vhd" || Path.GetExtension(WTGModel.imageFilePath) == ".vhdx")
                {
                    VhdType = string.Empty;
                    VhdSize = string.Empty;
                    VhdFileName = string.Empty;
                    ExtensionType = string.Empty;
                    VhdPath = WTGModel.imageFilePath;
                    NeedCopy = true;
                }
                else
                {
                    //    ////////////////vhd设定///////////////////////
                    if (WTGModel.isFixedVHD)
                    {
                        VhdType = "fixed";
                    }
                    else
                    {
                        VhdType = "expandable";
                    }

                    long hardDiskSpace = DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1048576;

                    if (WTGModel.userSetSize != 0)
                    {
                        //MessageBox.Show(hardDiskSpace.ToString());
                        //DiskOperation.GetHardDiskSpace(WTGModel.ud);
                        if (hardDiskSpace - 500 > 0 && WTGModel.userSetSize > hardDiskSpace - 500)
                        {
                            VhdSize = (hardDiskSpace - 500).ToString();
                        }
                        else
                        {
                            VhdSize = WTGModel.userSetSize.ToString();
                        }
                    }
                    else
                    {


                        if (hardDiskSpace >= (int.Parse(vhdDefaultSize) + 1024))
                        {
                            VhdSize = vhdDefaultSize;
                        }
                        else
                        {
                            VhdSize = hardDiskSpace - 500 > 0 ? vhdDefaultSize : (hardDiskSpace - 500).ToString();
                        }

                    }

                    ////////////////判断临时文件夹,VHD needcopy?///////////////////
                    int vhdmaxsize;
                    if (WTGModel.isFixedVHD)
                    {
                        vhdmaxsize = int.Parse(VhdSize) * 1024 + 1024;
                    }
                    else
                    {
                        vhdmaxsize = 10485670;//10GB
                    }

                    if (DiskOperation.GetHardDiskFreeSpace(WTGModel.vhdTempPath.Substring(0, 2) + "\\") <= vhdmaxsize * 1024L || StringUtility.IsChina(WTGModel.vhdTempPath) || (WTGModel.isUefiGpt && !WTGModel.isLegacyUdiskUefi) || (WTGModel.isUefiMbr && !WTGModel.isLegacyUdiskUefi) || WTGModel.isWimBoot || WTGModel.isNoTemp || WTGModel.CurrentOS == OS.Win7)
                    {
                        NeedCopy = false;
                        VhdPath = StringUtility.Combine(WTGModel.ud, WTGModel.win8VHDFileName);
                    }
                    else
                    {
                        NeedCopy = true;
                        VhdPath = StringUtility.Combine(WTGModel.vhdTempPath, WTGModel.win8VHDFileName);
                    }

                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(this.ExtensionType);
                sb.AppendLine(this.NeedCopy.ToString());
                sb.AppendLine(this.VhdFileName);
                sb.AppendLine(this.VhdPath);
                sb.AppendLine(this.VhdSize);
                sb.AppendLine(this.VhdType);
                Log.WriteLog("Info_VHDInfo", sb.ToString());
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_SetVhdProp", ex.ToString());
                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci), true);
                er.ShowDialog();
            }
        }

        private void ApplyToVdisk()
        {

            ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
            ImageOperation.ImageApply(WTGModel.isWimBoot, WTGModel.isEsd, WTGModel.imagexFileName, WTGModel.imageFilePath, WTGModel.wimPart, @"v:\", WTGModel.ud);

        }
        /// <summary>
        /// WIN7注册表和FixLetter 。所有VHD都需要FixLetter
        /// </summary>
        private void UEFIAndWin7ToGo()
        {
            if (WTGModel.win7togo != 0)
            {
                ImageOperation.Win7REG("V:\\");
            }
            if (WTGModel.fixLetter)
            {
                ImageOperation.Fixletter("C:", "V:");

            }
            //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:v  > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
            //}
        }

        //private void CopyVHDBootFiles()
        //{
        //    //this.CopyVHD();
        //    //if (!needcopyvhdbootfile) return;
        //    ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + " /e /h /y");

        //    if (this.ExtensionType == "vhdx")
        //    {
        //        ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + "\\boot\\ /e /h /y");
        //    }
        //    Log.WriteLog("CMD_CopyVHDBootFiles", "xcopy.exe" + "\"" + WTGModel.applicationFilesPath + "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + "\\boot\\ /e /h /y");
        //}
        private void BigFileCopy(string source, string target, int buffersize)
        {
            using (FileStream fsRead = File.OpenRead(source))
            {
                using (FileStream fsWrite = File.OpenWrite(target))
                {

                    byte[] byts = new byte[buffersize * 1024 * 1024];
                    int r = 0;
                    while ((r = fsRead.Read(byts, 0, byts.Length)) > 0)
                    {
                        fsWrite.Write(byts, 0, r);
                        //Console.WriteLine(fsWrite.Position / (double)fsRead.Length * 100);
                        //r = fsRead.Read(byts, 0, byts.Length);
                    }

                }
            }
        }
        #endregion
    }

    [Serializable]
    public class VHDException : Exception
    {
        public VHDException() { }
        public VHDException(string message) : base(message) { }
        public VHDException(string message, Exception inner) : base(message, inner) { }
        protected VHDException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
