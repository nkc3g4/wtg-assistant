using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace wintogo
{
    public static class Write
    {
        //// 关闭64位（文件系统）的操作转向
        //[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        //// 开启64位（文件系统）的操作转向
        //[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);


        #region 七种写入模式
        //public static void RemoveableDiskUefiGpt()
        //{
        //    string tempFileName = WTGModel.diskpartScriptPath + "\\" + Guid.NewGuid().ToString() + ".txt";
        //    Process diskInfo = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", " /diskinfo /find: /usbonly /file=\"" + tempFileName + "\"");
        //    diskInfo.WaitForExit();

        //    string tempUdiskInfo = File.ReadAllText(tempFileName);
        //    Log.WriteLog("Bootice_UsbInfo", tempUdiskInfo);

        //    Match match = Regex.Match(tempUdiskInfo, @"SET DRIVE([0-9])DESC=(.+)\r\nSET DRIVE([0-9])SIZE=(.+)\r\nSET DRIVE([0-9])LETTER=" + WTGModel.ud.Substring(0, 2).ToUpper(), RegexOptions.ECMAScript);
        //    string UdiskNumber = match.Groups[1].Value;
        //    if (DialogResult.No == MessageBox.Show(match.Groups[2].Value + "\n" + MsgManager.GetResString("Msg_TwiceConfirm", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
        //    //59055800320
        //    long udiskSize = long.Parse(match.Groups[4].Value);
        //    string dptFile = string.Empty;
        //    if (udiskSize > 59055800320)
        //    {
        //        dptFile = "55G";
        //    }
        //    else if (udiskSize > 28991029248)
        //    {
        //        dptFile = "27G";
        //    }
        //    else if (udiskSize > 13958643712)
        //    {
        //        dptFile = "13G";
        //    }
        //    else
        //    {
        //        throw new NotSupportedException("Your Usb Key is not suppotred");
        //    }
        //    //MessageBox.Show(WTGModel.applicationFilesPath + "\\DPTs\\" + dptFile + "-1.dpt");
        //    Process p1 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=\"" + WTGModel.applicationFilesPath + "\\DPTs\\" + dptFile + "-1.dpt\"");
        //    p1.WaitForExit();
        //    ProcessManager.ECMD("cmd.exe", "/c format " + WTGModel.ud.Substring(0, 2) + "/FS:ntfs /q /V: /Y");
        //    WTGModel.ntfsUefiSupport = true;

        //    if (WTGModel.CheckedMode == ApplyMode.Legacy)
        //    {
        //        NonUEFITypical(true);

        //    }
        //    else //非UEFI VHD VHDX
        //    {
        //        NonUEFIVHDVHDX(true);
        //    }
        //    FileOperation.DeleteFolder(WTGModel.diskpartScriptPath + "\\EFI\\");

        //    ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.ud.Substring(0, 2) + "\\EFI\\*.*" + "\"" + " \"" + WTGModel.diskpartScriptPath + "\\EFI\\\" /e /h /y");
        //    Process p2 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=" + WTGModel.applicationFilesPath + "\\DPTs\\" + dptFile + "-2.dpt");
        //    p2.WaitForExit();
        //    ProcessManager.ECMD("cmd.exe", "/c format " + WTGModel.ud.Substring(0, 2) + "/FS:fat /q /V: /Y");
        //    ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.diskpartScriptPath + "\\EFI\\*.*" + "\"" + " \"" + WTGModel.ud.Substring(0, 2) + "\\EFI\\\" /e /h /y");

        //    Process p3 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=" + WTGModel.applicationFilesPath + "\\DPTs\\" + dptFile + "-1.dpt");
        //    p3.WaitForExit();
        //    FileOperation.DeleteFolder(WTGModel.diskpartScriptPath + "\\EFI\\");

        //    ////takeown /f e:\boot /r
        //    //ProcessManager.SyncCMD("takeown.exe /f " + WTGModel.ud.Substring(0, 2) + "\\EFI /r");
        //    //ProcessManager.SyncCMD("takeown.exe /f " + WTGModel.ud.Substring(0, 2) + "\\Boot /r");

        //    //ProcessManager.SyncCMD("cacls.exe " + WTGModel.ud.Substring(0, 2) + "\\EFI /t /e /c /g everyone:f");
        //    //ProcessManager.SyncCMD("cacls.exe " + WTGModel.ud.Substring(0, 2) + "\\Boot /t /e /c /g everyone:f");
        //    //FileOperation.DeleteFolder(WTGModel.ud.Substring(0, 2) + "\\EFI");
        //    //FileOperation.DeleteFolder(WTGModel.ud.Substring(0, 2) + "\\Boot");



        //}
        //public static void RemoveableDiskUefiMbr()
        //{
        //    string tempFileName = WTGModel.diskpartScriptPath + "\\" + Guid.NewGuid().ToString() + ".txt";
        //    Process diskInfo = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", " /diskinfo /find: /usbonly /file=\"" + tempFileName + "\"");
        //    diskInfo.WaitForExit();

        //    string tempUdiskInfo = File.ReadAllText(tempFileName);
        //    Match match = Regex.Match(tempUdiskInfo, @"SET DRIVE([0-9])DESC=(.+)\r\nSET DRIVE([0-9])SIZE=(.+)\r\nSET DRIVE([0-9])LETTER=" + WTGModel.ud.Substring(0, 2).ToUpper(), RegexOptions.ECMAScript);
        //    string UdiskNumber = match.Groups[1].Value;
        //    if (DialogResult.No == MessageBox.Show(match.Groups[2].Value + "\n" + MsgManager.GetResString("Msg_TwiceConfirm", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
        //    //59055800320
        //    long udiskSize = long.Parse(match.Groups[4].Value);
        //    string dptFile = string.Empty;
        //    if (udiskSize > 123480309760)
        //    {
        //        dptFile = "115G";
        //    }
        //    else if (udiskSize > 62277025792)
        //    {
        //        dptFile = "58G";
        //    }
        //    else if (udiskSize > 31138512896)
        //    {
        //        dptFile = "29G";
        //    }
        //    else if (udiskSize > 15032385536)
        //    {
        //        dptFile = "14G";
        //    }
        //    else
        //    {
        //        throw new NotSupportedException("Your Usb Key is not suppotred");
        //    }
        //    //Process p = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /repartition /usb-fdd /fstype=ntfs /quiet");
        //    //p.WaitForExit();
        //    DiskOperation.DiskPartRePartitionUD(WTGModel.partitionSize);


        //    //MessageBox.Show(WTGModel.applicationFilesPath + "\\DPTs\\" + dptFile + "-1.dpt");
        //    Process p1 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=\"" + WTGModel.applicationFilesPath + "\\DPTs\\UEFIMBR\\" + dptFile + "-1.dpt\"");
        //    p1.WaitForExit();
        //    ProcessManager.ECMD("cmd.exe", "/c format " + WTGModel.ud.Substring(0, 2) + "/FS:ntfs /q /V: /Y");



        //    WTGModel.ntfsUefiSupport = true;

        //    if (WTGModel.CheckedMode == ApplyMode.Legacy)
        //    {
        //        NonUEFITypical(true);

        //    }
        //    else //非UEFI VHD VHDX
        //    {
        //        NonUEFIVHDVHDX(true);
        //    }
        //    FileOperation.DeleteFolder(WTGModel.diskpartScriptPath + "\\EFI\\");

        //    ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.ud.Substring(0, 2) + "\\EFI\\*.*" + "\"" + " \"" + WTGModel.diskpartScriptPath + "\\EFI\\\" /e /h /y");
        //    ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.ud.Substring(0, 2) + "\\Boot\\*.*" + "\"" + " \"" + WTGModel.diskpartScriptPath + "\\Boot\\\" /e /h /y");
        //    File.Copy(WTGModel.ud.Substring(0, 2) + "\\bootmgr", WTGModel.diskpartScriptPath + "\\EFI\\bootmgr", true);
        //    File.Copy(WTGModel.ud.Substring(0, 2) + "\\BOOTNXT", WTGModel.diskpartScriptPath + "\\EFI\\BOOTNXT", true);

        //    Process p2 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /backup_dpt=\"" + WTGModel.diskpartScriptPath + "\\" + dptFile + "-3.dpt\"");
        //    p2.WaitForExit();




        //    Process p3 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=" + WTGModel.applicationFilesPath + "\\DPTs\\UEFIMBR\\" + dptFile + "-2.dpt");
        //    p3.WaitForExit();



        //    ProcessManager.ECMD("cmd.exe", "/c format " + WTGModel.ud.Substring(0, 2) + "/FS:fat /q /V: /Y");

        //    Process pRE = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=" + WTGModel.applicationFilesPath + "\\DPTs\\UEFIMBR\\" + dptFile + "-2.dpt");
        //    pRE.WaitForExit();


        //    ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.diskpartScriptPath + "\\EFI\\*.*" + "\"" + " \"" + WTGModel.ud.Substring(0, 2) + "\\EFI\\\" /e /h /y");
        //    ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.diskpartScriptPath + "\\Boot\\*.*" + "\"" + " \"" + WTGModel.ud.Substring(0, 2) + "\\Boot\\\" /e /h /y");
        //    File.Copy(WTGModel.diskpartScriptPath + "\\EFI\\bootmgr", WTGModel.ud.Substring(0, 2) + "\\bootmgr", true);
        //    File.Copy(WTGModel.diskpartScriptPath + "\\EFI\\BOOTNXT", WTGModel.ud.Substring(0, 2) + "\\BOOTNXT", true);

        //    BootFileOperation.BooticeAct(WTGModel.ud);


        //    Process p4 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=" + WTGModel.diskpartScriptPath + "\\" + dptFile + "-3.dpt");
        //    p4.WaitForExit();

        //    FileOperation.DeleteFolder(WTGModel.diskpartScriptPath + "\\EFI\\");
        //    FileOperation.DeleteFolder(WTGModel.diskpartScriptPath + "\\Boot\\");

        //}


        public static bool UefiGptTypical()
        {
            if (WTGModel.isBitlocker)
            {
                Write.EnableBitlocker(WTGModel.ud);
            }
            ImageOperation io = new ImageOperation();
            io.imageX = WTGModel.imagexFileName;
            io.imageFile = WTGModel.imageFilePath;
            io.AutoChooseWimIndex();
            io.ImageApplyToUD();
            if (!VerifySysFiles(WTGModel.ud))
            {
                throw new Exception(MsgManager.GetResString("Msg_bootmgrError", MsgManager.ci));
            }

            ImageOperation.ImageExtra(WTGModel.installDonet35, WTGModel.isBlockLocalDisk, WTGModel.disableWinRe,WTGModel.skipOOBE, WTGModel.disableUasp, WTGModel.ud, WTGModel.imageFilePath);
            BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, WTGModel.espLetter.Substring(0,1)+":\\", FirmwareType.UEFI);
            BootFileOperation.BcdeditFixBootFileTypical(WTGModel.espLetter.Substring(0, 1) + ":\\", WTGModel.ud, FirmwareType.UEFI);
            RemoveLetterX();

            return true;
        }



        public static bool UefiGptVhdVhdx()
        {
            VHDOperation vo = new VHDOperation();
            vo.Execute();

            RemoveLetterX();

            if (File.Exists(WTGModel.ud + WTGModel.win8VHDFileName))
            {
                return true;
            }
            else
            {
                Log.WriteLog("Err_VHDCreationError", "VHDCreationError");

                //VHD文件创建出错！
                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci), true);
                er.ShowDialog();
                return false;
                //MessageBox.Show("Win8 VHD文件不存在！，可到论坛发帖求助！\n建议将程序目录下logs文件夹打包上传，谢谢！","出错啦！",MessageBoxButtons .OK ,MessageBoxIcon.Error );

            }
        }

        public static bool NonUEFIVHDVHDX(bool legacyUdiskUefi)
        {
            VHDOperation vo = new VHDOperation();
            vo.Execute();
            if (!legacyUdiskUefi)
            {
                BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);
            }
            if (!File.Exists(WTGModel.ud + WTGModel.win8VHDFileName))
            {
                //Win8 VHD文件不存在！未知错误原因！
                Log.WriteLog("Err_VHDCreationError", "!File.Exists(VHD)");

                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci), true);
                er.ShowDialog();
                return false;
            }

            else if (!WTGModel.isUserSetEfiPartition && !File.Exists(WTGModel.ud + "\\Boot\\BCD"))
            {
                //VHD模式下BCDBOOT执行出错！
                Log.WriteLog("Err_VHDCreationError", "!File.Exists(BCD)");

                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDBcdbootError", MsgManager.ci), true);
                er.ShowDialog();
                return false;
            }
            else if (!File.Exists(WTGModel.ud + "bootmgr"))
            {
                Log.WriteLog("Err_VHDCreationError", "!File.Exists(bootmgr)");

                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_bootmgrError", MsgManager.ci), true);
                er.ShowDialog();
                return false;
                //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
            }
            else
            {
                if (!legacyUdiskUefi)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public static bool NonUEFITypical(bool legacyUdiskUefi)
        {
            //Bitlocker
            if (WTGModel.isBitlocker)
            {
                Write.EnableBitlocker(WTGModel.ud);
            }
            ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
            ImageOperation.ImageApply(WTGModel.isWimBoot, WTGModel.isEsd, WTGModel.imagexFileName, WTGModel.imageFilePath, WTGModel.wimPart, WTGModel.ud, WTGModel.ud);
            if (WTGModel.win7togo != 0)
            {
                ImageOperation.Win7REG(WTGModel.ud);
            }
            if (WTGModel.win7togo == 0)
            {
                ImageOperation.ImageExtra(WTGModel.installDonet35, WTGModel.isBlockLocalDisk, WTGModel.disableWinRe, WTGModel.skipOOBE, WTGModel.disableUasp, WTGModel.ud, WTGModel.imageFilePath);
            }

            if (WTGModel.isUserSetEfiPartition && Directory.Exists(WTGModel.efiPartition))
            {
                BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, WTGModel.efiPartition, FirmwareType.ALL);
                BootFileOperation.BcdeditFixBootFileTypical(WTGModel.ud, WTGModel.efiPartition, FirmwareType.BIOS);
                BootFileOperation.BcdeditFixBootFileTypical(WTGModel.ud, WTGModel.efiPartition, FirmwareType.UEFI);
            }
            else
            {
                if (WTGModel.ntfsUefiSupport)
                {
                    BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, WTGModel.ud, FirmwareType.ALL);
                    BootFileOperation.BcdeditFixBootFileTypical(WTGModel.ud, WTGModel.ud, FirmwareType.BIOS);
                    BootFileOperation.BcdeditFixBootFileTypical(WTGModel.ud, WTGModel.ud, FirmwareType.UEFI);

                }
                else
                {
                    BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, WTGModel.ud, FirmwareType.BIOS);
                    BootFileOperation.BcdeditFixBootFileTypical(WTGModel.ud, WTGModel.ud, FirmwareType.BIOS);

                }
            }
            if (!legacyUdiskUefi)
            {
                if (WTGModel.isUserSetEfiPartition)
                {
                    BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.efiPartition);

                }
                else
                {
                    BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);

                }

                //ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\" + WTGModel.bcdbootFileName, WTGModel.ud.Substring(0, 3) + "windows  /s  " + WTGModel.ud.Substring(0, 2) + " /f ALL");


                if (!File.Exists(WTGModel.ud + "bootmgr"))
                {
                    //MsgManager.getResString("Msg_bootmgrError")
                    //文件写入出错！bootmgr不存在！\n请检查写入过程是否中断
                    Log.WriteLog("Err_bootmgrError", "!File.Exists(bootmgr)");

                    ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_bootmgrError", MsgManager.ci), true);
                    er.ShowDialog();
                    return false;
                    //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
                }
                else if (!WTGModel.isUserSetEfiPartition && !File.Exists(WTGModel.ud + "\\Boot\\BCD"))
                {
                    //MsgManager.getResString("Msg_BCDError")
                    //引导文件写入出错！boot文件夹不存在！
                    Log.WriteLog("Err_BCDError", "!File.Exists(BCD)");

                    ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_BCDError", MsgManager.ci), true);
                    er.ShowDialog();
                    return false;
                    //MessageBox.Show("引导文件写入出错！boot文件夹不存在\n请看论坛教程！", "出错啦", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //System.Diagnostics.Process.Start("http://bbs.luobotou.org/thread-1625-1-1.html");
                }
                else
                {
                    return true;
                    //FinishSuccessful();
                }
            }
            return true;
        }


        public static bool UefiMbrVHDVHDX()
        {
            VHDOperation vo = new VHDOperation();
            vo.Execute();

            RemoveLetterX();

            if (File.Exists(WTGModel.ud + WTGModel.win8VHDFileName))
            {
                return true;
                //FinishSuccessful();
                //finish f = new finish();
                //f.ShowDialog();
            }
            else
            {
                Log.WriteLog("Err_VHDCreationError", "!File.Exists(VHD)");

                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci), true);
                er.ShowDialog();
                return false;
                //shouldcontinue = false;
            }
            //removeLetterX();
            //Finish f = new Finish();
            //f.ShowDialog();
        }

        //private static void FinishSuccessful()
        //{
        //    if (WTGModel.noDefaultDriveLetter && !WTGModel.udString.Contains("Removable Disk"))
        //    {
        //        DiskOperation.SetNoDefaultDriveLetter(WTGModel.ud);
        //    }
        //    writeSw.Stop();

        //    Finish f = new Finish(writeSw.Elapsed);
        //    f.ShowDialog();
        //}

        public static bool UEFIMBRTypical()
        {
            ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
            if (WTGModel.isBitlocker)
            {
                EnableBitlocker(WTGModel.ud);
            }
            //IMAGEX解压
            ImageOperation.ImageApply(WTGModel.isWimBoot, WTGModel.isEsd, WTGModel.imagexFileName, WTGModel.imageFilePath, WTGModel.wimPart, WTGModel.ud, WTGModel.ud);
            if (!VerifySysFiles(WTGModel.ud))
            {
                throw new Exception(MsgManager.GetResString("Msg_bootmgrError", MsgManager.ci));
            }
            //安装EXTRA
            ImageOperation.ImageExtra(WTGModel.installDonet35, WTGModel.isBlockLocalDisk, WTGModel.disableWinRe, WTGModel.skipOOBE, WTGModel.disableUasp, WTGModel.ud, WTGModel.imageFilePath);
            //BCDBOOT WRITE BOOT FILE  
            BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, WTGModel.espLetter.Substring(0, 1) + ":\\", FirmwareType.ALL);
            //BootFileOperation.BcdbootWriteALLBootFileToXAndAct(WTGOperation.bcdbootFileName, WTGOperation.ud);
            BootFileOperation.BcdeditFixBootFileTypical(WTGModel.espLetter.Substring(0, 1) + ":\\", WTGModel.ud, FirmwareType.UEFI);
            //BootFileOperation.BooticeAct(WTGModel.espLetter.Substring(0, 1) + ":\\");
            RemoveLetterX();

            return true;
            //FinishSuccessful();

        }
        private static bool VerifySysFiles(string ud)
        {
            if(!File.Exists(ud+ "\\Windows\\system32\\ntoskrnl.exe"))
            {
                return false;
            }
            return true;
        }

        public static void RemoveLetterX()
        {
            try
            {
                DiskpartScriptManager dsm = new DiskpartScriptManager();
                StringBuilder args = new StringBuilder();
                args.AppendLine("select volume "+WTGModel.espLetter.Substring(0, 1));
                args.AppendLine("remove");
                args.AppendLine("exit");
                dsm.Args = args.ToString();
                dsm.RunDiskpartScript();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                Log.WriteLog("Err_removeLetterX", ex.ToString());
            }
            //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartScriptPath + "\\removex.txt\"");
        }
        public static void EnableBitlocker(string target)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                Process p = Process.Start(WTGModel.applicationFilesPath + "\\BitlockerConfig_x64.exe", target);
                p.WaitForExit();
            }
            else
            {
                Process p = Process.Start(WTGModel.applicationFilesPath + "\\BitlockerConfig_x86.exe", target);
                p.WaitForExit();

            }
            //IntPtr oldWOW64State = new IntPtr();
            //if (Wow64DisableWow64FsRedirection(ref oldWOW64State))
            //{
            //    Process.Start(Environment.GetEnvironmentVariable("windir") + "\\system32\\BitlockerWizard.exe");
            //    Process.Start("cmd.exe", "/k ");

            //}
            //打开文件系统转向

            //BitlockerConfig bc = new BitlockerConfig();
            //bc.ShowDialog();
            //Wow64RevertWow64FsRedirection(oldWOW64State);

            //ProcessManager.SyncCMD("BitlockerWizard.exe " + WTGModel.ud.Substring(0, 2) + " T");
        }

        #endregion

    }
}
