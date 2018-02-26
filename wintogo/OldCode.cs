//private void btnVhdTempPath_Click(object sender, EventArgs e)
//{
//    folderBrowserDialog1.ShowDialog();
//    if (Directory.Exists(folderBrowserDialog1.SelectedPath))
//    {
//        txtVhdTempPath.Text = folderBrowserDialog1.SelectedPath;
//    }
//private void 打开程序运行目录ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    Process.Start("explorer.exe", Application.StartupPath);
//}
//}
//private void checkBox1_CheckedChanged(object sender, EventArgs e)//克隆本机系统
//{
//if (checkBoxClone.Checked)
//{
//    MessageBox.Show("克隆本机系统须知：\n1.本机系统必须为Win10。\n2.使用的USB驱动器必须识别为本地磁盘。\n3.USB驱动器将会被重新分区，所有数据将丢失。\n4.克隆后WTG系统为UEFI+MBR启动模式。\n5.本功能为实验性功能。");
//    if (!File.Exists(Path.GetPathRoot(Environment.GetEnvironmentVariable("windir")) + "wtg_clone_source"))
//    {
//        File.Create(Path.GetPathRoot(Environment.GetEnvironmentVariable("windir")) + "wtg_clone_source");
//    }
//}
//else
//{
//    if (File.Exists(Path.GetPathRoot(Environment.GetEnvironmentVariable("windir")) + "wtg_clone_source"))
//    {
//        File.Delete(Path.GetPathRoot(Environment.GetEnvironmentVariable("windir")) + "wtg_clone_source");
//    }
//}
//lblWim.Enabled = !checkBoxClone.Checked;
//radiobtnVhd.Enabled = !checkBoxClone.Checked;
//radiobtnVhdx.Enabled = !checkBoxClone.Checked;
//checkBoxBitlocker.Enabled = !checkBoxClone.Checked;
//checkBoxUefigpt.Enabled = !checkBoxClone.Checked;
//checkBoxUefimbr.Enabled = !checkBoxClone.Checked;

//}


//private byte[] UlongToMBRBytes(ulong u)
//{
//    string convertTempString = Convert.ToString(Convert.ToInt32(u), 16);

//    for (int i = convertTempString.Length; i < 8; i++)
//    {
//        convertTempString = "0" + convertTempString;
//    }
//    //MessageBox.Show(convertTempString);

//    //MessageBox.Show(convertTempString.Length.ToString());
//    string hex1 = convertTempString[6].ToString() + convertTempString[7];
//    string hex2 = convertTempString[4].ToString() + convertTempString[5];
//    string hex3 = convertTempString[2].ToString() + convertTempString[3];
//    string hex4 = convertTempString[0].ToString() + convertTempString[1];
//    byte[] byts = new byte[4];
//    byts[0] = Convert.ToByte(Convert.ToInt32(hex1, 16));
//    byts[1] = Convert.ToByte(Convert.ToInt32(hex2, 16));
//    byts[2] = Convert.ToByte(Convert.ToInt32(hex3, 16));
//    byts[3] = Convert.ToByte(Convert.ToInt32(hex4, 16));
//    return byts;
//}
//private void RemoveLetterX()
//{
//    try
//    {
//        DiskpartScriptManager dsm = new DiskpartScriptManager();
//        StringBuilder args = new StringBuilder();
//        args.AppendLine("select volume x");
//        args.AppendLine("remove");
//        args.AppendLine("exit");
//        dsm.Args = args.ToString();
//        dsm.RunDiskpartScript();
//    }
//    catch (Exception ex)
//    {
//        //Console.WriteLine(ex.ToString());
//        Log.WriteLog("Err_removeLetterX", ex.ToString());
//    }
//    //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartScriptPath + "\\removex.txt\"");
//}

//private void wTG高级设定选项ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    WTGSettings ws = new WTGSettings();
//    ws.Show();
//}

//private void bootsectToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
//    //
//    System.Diagnostics.Process p1 = System.Diagnostics.Process.Start(WTGModel.applicationFilesPath + "\\" + "\\bootsect.exe", " /nt60 " + WTGModel.ud.Substring(0, 2) + " /force /mbr");
//    p1.WaitForExit();
//    MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
//}

//private void diskpart重新分区ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    DiskOperation.DiskPartRePartitionUD(WTGModel.partitionSize);
//    //diskPart();


//}

//private void 创建VHDToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
//    VHDOperation vo = new VHDOperation();
//    vo.CreateVHD();

//}

//private void 向V盘写入ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
//    System.Diagnostics.Process p = System.Diagnostics.Process.Start(WTGModel.applicationFilesPath + "\\" + WTGModel.imagexFileName, " /apply " + "\"" + lblWim.Text + "\"" + " " + WTGModel.wimPart.ToString() + " " + "v:\\");
//    p.WaitForExit();

//}

//private void 卸载V盘ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    VHDOperation vo = new VHDOperation();
//    vo.DetachVHD();

//}

//private void 复制VHD启动文件ToolStripMenuItem_Click(object sender, EventArgs e)
//{

//    //MsgManager.getResString("Msg_chooseud")
//    if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
//    WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
//    try
//    {
//        ProcessManager.ECMD("takeown.exe", " /f \"" + WTGModel.ud + "\\boot\\" + "\" /r /d y && icacls \"" + WTGModel.ud + "\\boot\\" + "\" /grant administrators:F /t");
//        ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + " /e /h /y");
//    }
//    catch (UserCancelException ex)
//    {
//        Log.WriteLog("Err_UserCancelException", ex.ToString());

//        ErrorMsg em = new ErrorMsg(ex.Message);
//        em.Show();
//    }

//    //copyvhdbootfile();

//}

//private void 复制win8vhdToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
//    WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
//    VHDOperation vo = new VHDOperation();
//    vo.CopyVHD();

//}

//private void 清理临时文件ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    VHDOperation.CleanTemp();
//}


//private void label1_Click(object sender, EventArgs e)
//{
//    VisitWeb("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427&extra=page%3D1");

//}

//private void imagex解压写入ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
//    if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
//    if (!File.Exists(lblWim.Text)) { MessageBox.Show(MsgManager.GetResString("Msg_chooseinstallwim", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
//    WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
//    if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci) + WTGModel.ud.Substring(0, 1) + MsgManager.GetResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1024 / 1024 + MsgManager.GetResString("Msg_FormatTip", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
//    Process p = Process.Start(WTGModel.applicationFilesPath + "\\" + WTGModel.imagexFileName, " /apply " + "\"" + lblWim.Text + "\"" + " " + WTGModel.wimPart + " " + WTGModel.ud);
//    p.WaitForExit();
//    //MsgManager.getResString("Msg_Complete")
//    MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
//}

//private void 写入引导文件ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }

//    WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
//    if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci) + WTGModel.ud.Substring(0, 1) + MsgManager.GetResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1024 / 1024 + MsgManager.GetResString("Msg_FormatTip", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
//    try
//    {
//        ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\" + WTGModel.bcdbootFileName, WTGModel.ud.Substring(0, 3) + "windows  /s  " + WTGModel.ud.Substring(0, 2) + " /f ALL");

//        BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, WTGModel.ud, FirmwareType.BIOS);
//        MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
//    }
//    catch (UserCancelException ex)
//    {
//        Log.WriteLog("Err_UserCancelException", ex.ToString());

//        ErrorMsg em = new ErrorMsg(ex.Message);
//        em.Show();
//    }
//}

//private void 设置活动分区ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }

//    WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘

//    if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci) + WTGModel.ud.Substring(0, 1) + MsgManager.GetResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1024 / 1024 + MsgManager.GetResString("Msg_FormatTip", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
//    BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);
//    //System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\bootice.exe", " /DEVICE=" + WTGOperation.ud.Substring(0, 2) + " /partitions /activate /quiet");
//    //p2.WaitForExit();
//    MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
//}

//private void 写入磁盘引导ToolStripMenuItem_Click(object sender, EventArgs e)
//{
//    if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }

//    WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
//    if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci) + WTGModel.ud.Substring(0, 1) + MsgManager.GetResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1024 / 1024 + MsgManager.GetResString("Msg_FormatTip", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
//    BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);
//    //System.Diagnostics.Process booice = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", (" /DEVICE=" + WTGOperation.ud.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
//    //booice.WaitForExit();
//    //System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\BOOTICE.exe", (" /DEVICE=" + WTGOperation.ud.Substring(0, 2) + " /pbr /install /type=bootmgr /quiet"));//写入引导
//    //pbr.WaitForExit();
//    MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
//}






//#region 七种写入模式
//private void RemoveableDiskUefiGpt()
//{
//    string tempFileName = WTGModel.diskpartScriptPath + "\\" + Guid.NewGuid().ToString() + ".txt";
//    Process diskInfo = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", " /diskinfo /find: /usbonly /file=" + tempFileName);
//    diskInfo.WaitForExit();

//    string tempUdiskInfo = File.ReadAllText(tempFileName);
//    Match match = Regex.Match(tempUdiskInfo, @"SET DRIVE([0-9])DESC=(.+)\r\nSET DRIVE([0-9])SIZE=(.+)\r\nSET DRIVE([0-9])LETTER=" + WTGModel.ud.Substring(0, 2).ToUpper(), RegexOptions.ECMAScript);
//    string UdiskNumber = match.Groups[1].Value;
//    if (DialogResult.No == MessageBox.Show(match.Groups[2].Value + "\n" + MsgManager.GetResString("Msg_TwiceConfirm", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
//    //59055800320
//    long udiskSize = long.Parse(match.Groups[4].Value);
//    string dptFile = string.Empty;
//    if (udiskSize > 118111600640)
//    {
//        dptFile = "110G";
//    }
//    else if (udiskSize > 59055800320)
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

//    if (radiobtnLegacy.Checked)
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



//private void UefiGptTypical()
//{
//    ImageOperation io = new ImageOperation();
//    io.imageX = WTGModel.imagexFileName;
//    io.imageFile = WTGModel.imageFilePath;
//    io.AutoChooseWimIndex();
//    io.ImageApplyToUD();
//    ImageOperation.ImageExtra(WTGModel.installDonet35, checkBoxSan_policy.Checked, WTGModel.disableWinRe, WTGModel.ud, lblWim.Text);
//    BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, @"X:\", FirmwareType.UEFI);
//    BootFileOperation.BcdeditFixBootFileTypical(@"X:\", WTGModel.ud, FirmwareType.UEFI);
//    RemoveLetterX();
//    FinishSuccessful();
//}

//private void UefiGptVhdVhdx()
//{
//    VHDOperation vo = new VHDOperation();
//    vo.Execute();

//    RemoveLetterX();

//    if (File.Exists(WTGModel.ud + WTGModel.win8VHDFileName))
//    {
//        FinishSuccessful();
//    }
//    else
//    {
//        Log.WriteLog("Err_VHDCreationError", "VHDCreationError");

//        //VHD文件创建出错！
//        ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
//        er.ShowDialog();
//        //MessageBox.Show("Win8 VHD文件不存在！，可到论坛发帖求助！\n建议将程序目录下logs文件夹打包上传，谢谢！","出错啦！",MessageBoxButtons .OK ,MessageBoxIcon.Error );

//    }
//}

//private void NonUEFIVHDVHDX(bool legacyUdiskUefi)
//{
//    VHDOperation vo = new VHDOperation();
//    vo.Execute();
//    if (!legacyUdiskUefi)
//    {
//        BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);
//    }
//    if (!File.Exists(WTGModel.ud + WTGModel.win8VHDFileName))
//    {
//        //Win8 VHD文件不存在！未知错误原因！
//        Log.WriteLog("Err_VHDCreationError", "!File.Exists(VHD)");

//        ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
//        er.ShowDialog();
//    }

//    else if (!File.Exists(WTGModel.ud + "\\Boot\\BCD"))
//    {
//        //VHD模式下BCDBOOT执行出错！
//        Log.WriteLog("Err_VHDCreationError", "!File.Exists(BCD)");

//        ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDBcdbootError", MsgManager.ci));
//        er.ShowDialog();
//    }
//    else if (!File.Exists(WTGModel.ud + "bootmgr"))
//    {
//        Log.WriteLog("Err_VHDCreationError", "!File.Exists(bootmgr)");

//        ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_bootmgrError", MsgManager.ci));
//        er.ShowDialog();
//        //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
//    }
//    else
//    {
//        if (!legacyUdiskUefi)
//        {
//            FinishSuccessful();
//        }
//    }

//}
//private void NonUEFITypical(bool legacyUdiskUefi)
//{
//    ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
//    ImageOperation.ImageApply(checkBoxWimboot.Checked, WTGModel.isEsd, WTGModel.imagexFileName, WTGModel.imageFilePath, WTGModel.wimPart, WTGModel.ud, WTGModel.ud);
//    if (WTGModel.win7togo != 0)
//    {
//        ImageOperation.Win7REG(WTGModel.ud);
//    }
//    if (WTGModel.win7togo == 0)
//    {
//        ImageOperation.ImageExtra(WTGModel.installDonet35, checkBoxSan_policy.Checked, WTGModel.disableWinRe, WTGModel.ud, lblWim.Text);
//    }
//    if (WTGModel.ntfsUefiSupport)
//    {
//        BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, WTGModel.ud, FirmwareType.ALL);
//        BootFileOperation.BcdeditFixBootFileTypical(WTGModel.ud, WTGModel.ud, FirmwareType.BIOS);
//        BootFileOperation.BcdeditFixBootFileTypical(WTGModel.ud, WTGModel.ud, FirmwareType.UEFI);

//    }
//    else
//    {
//        BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, WTGModel.ud, FirmwareType.BIOS);
//        BootFileOperation.BcdeditFixBootFileTypical(WTGModel.ud, WTGModel.ud, FirmwareType.BIOS);

//    }
//    if (!legacyUdiskUefi)
//    {
//        BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);


//        //ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\" + WTGModel.bcdbootFileName, WTGModel.ud.Substring(0, 3) + "windows  /s  " + WTGModel.ud.Substring(0, 2) + " /f ALL");


//        if (!System.IO.File.Exists(WTGModel.ud + "bootmgr"))
//        {
//            //MsgManager.getResString("Msg_bootmgrError")
//            //文件写入出错！bootmgr不存在！\n请检查写入过程是否中断
//            Log.WriteLog("Err_bootmgrError", "!File.Exists(bootmgr)");

//            ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_bootmgrError", MsgManager.ci));
//            er.ShowDialog();

//            //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
//        }
//        else if (!System.IO.File.Exists(WTGModel.ud + "\\Boot\\BCD"))
//        {
//            //MsgManager.getResString("Msg_BCDError")
//            //引导文件写入出错！boot文件夹不存在！
//            Log.WriteLog("Err_BCDError", "!File.Exists(BCD)");

//            ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_BCDError", MsgManager.ci));
//            er.ShowDialog();
//            //MessageBox.Show("引导文件写入出错！boot文件夹不存在\n请看论坛教程！", "出错啦", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            //System.Diagnostics.Process.Start("http://bbs.luobotou.org/thread-1625-1-1.html");
//        }
//        else
//        {
//            FinishSuccessful();
//        }
//    }
//}


//private void UefiMbrVHDVHDX()
//{
//    VHDOperation vo = new VHDOperation();
//    vo.Execute();

//    RemoveLetterX();

//    if (System.IO.File.Exists(WTGModel.ud + WTGModel.win8VHDFileName))
//    {
//        FinishSuccessful();
//        //finish f = new finish();
//        //f.ShowDialog();
//    }
//    else
//    {
//        Log.WriteLog("Err_VHDCreationError", "!File.Exists(VHD)");

//        ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
//        er.ShowDialog();
//        //shouldcontinue = false;
//    }
//    //removeLetterX();
//    //Finish f = new Finish();
//    //f.ShowDialog();
//}

//private void FinishSuccessful()
//{
//    if (WTGModel.noDefaultDriveLetter && !WTGModel.udString.Contains("Removable Disk"))
//    {
//        DiskOperation.SetNoDefaultDriveLetter(WTGModel.ud);
//    }
//    writeSw.Stop();

//    Finish f = new Finish(writeSw.Elapsed);
//    f.ShowDialog();
//}

//private void UEFIMBRTypical()
//{
//    ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
//    //IMAGEX解压
//    ImageOperation.ImageApply(checkBoxWimboot.Checked, WTGModel.isEsd, WTGModel.imagexFileName, WTGModel.imageFilePath, WTGModel.wimPart, WTGModel.ud, WTGModel.ud);

//    //安装EXTRA
//    ImageOperation.ImageExtra(WTGModel.installDonet35, checkBoxSan_policy.Checked, WTGModel.disableWinRe, WTGModel.ud, lblWim.Text);
//    //BCDBOOT WRITE BOOT FILE  
//    BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, @"X:\", FirmwareType.ALL);
//    //BootFileOperation.BcdbootWriteALLBootFileToXAndAct(WTGOperation.bcdbootFileName, WTGOperation.ud);
//    BootFileOperation.BcdeditFixBootFileTypical(@"X:\", WTGModel.ud, FirmwareType.UEFI);
//    BootFileOperation.BooticeAct(@"X:\");
//    RemoveLetterX();
//    FinishSuccessful();

//}



//#endregion

////#region Udisk
////public delegate void OutDelegate(bool isend, object dtSource);
////public void OutText(bool isend, object dtSource)
////{
////    //MessageBox.Show("Test");
////    if (comboBoxUd.InvokeRequired)
////    {
////        OutDelegate outdelegate = new OutDelegate(OutText);
////        BeginInvoke(outdelegate, new object[] { isend, dtSource });
////        return;
////    }
////    comboBoxUd.DataSource = null;
////    comboBoxUd.DataSource = dtSource;
////    if (comboBoxUd.Items.Count != 0)
////    {
////        comboBoxUd.SelectedIndex = 0;
////    }
////    if (isend)
////    {
////        comboBoxUd.SelectedIndex = comboBoxUd.Items.Count - 1;
////    }
////}

////private void GetUdiskInfo()
////{

////    string newlist = string.Empty;
////    UsbManager manager = new UsbManager();
////    try
////    {
////        diskCollection.Clear();
////        //UsbDiskCollection disks = manager.GetAvailableDisks();
////        UsbDisk udChoose = new UsbDisk(MsgManager.GetResString("Msg_chooseud", MsgManager.ci));
////        diskCollection.Add(udChoose);

////        //if (disks == null) { return; }
////        foreach (UsbDisk disk in manager.GetAvailableDisks())
////        {
////            diskCollection.Add(disk);
////            newlist += disk.ToString();

////        }
////        if (newlist != currentList)
////        {

////            currentList = newlist;

////            OutText(false, diskCollection);
////        }
////    }
////    catch (Exception ex) { Log.WriteLog("Err_GetUdiskInfo", ex.ToString()); }
////    finally
////    {

////        manager.Dispose();
////    }

////}

////private void LoadUDList()
////{
////    //Udlist = new Udlist();
////    tListUDisks = new Thread(GetUdiskInfo);
////    tListUDisks.Start();

////}
////private void timer1_Tick(object sender, EventArgs e)
////{
////    if (comboBoxUd.SelectedIndex == 0)
////    {
////        LoadUDList();

////    }
////}
////#endregion        //string currentList;//当前优盘列表
////private UsbDiskCollection diskCollection = new UsbDiskCollection();
////private Thread tListUDisks;

////if (textContains("Leaving")) { wp.Close(); }
////if (wp.t.extBox1.Lines.Length != 0)
////MessageBox.Show(text+"\n/////////////\n"+ wp.textBox1.Lines[wp.textBox1.Lines.Length - 2] + "\r\n");
////if (wp.textBox1.InvokeRequired)
////{
////    AppendTextCallback d = new AppendTextCallback(AppendText);
////    wp.textBox1.Invoke(d, text);
////}
////else
////{
////    wp.textBox1.AppendText(text);

////    //this.textBox1.AppendText(text);
////}

////// Invoke an anonymous method on the thread of the form.
////wp.Invoke((MethodInvoker)delegate
////{

////    wp.Close();

////});


//// 这里仅做输出的示例，实际上您可以根据情况取消获取命令行的内容  
//// 参考：process.CancelOutputRead()  
////if (!wp.IsHandleCreated) { wp.Show(); }

////private void button2_Click_1(object sender, EventArgs e)
////{
////    WTGSettings ws = new WTGSettings();
////    ws.Show();
////    //有些设置项目需要应用到WTGModel
////    WTGModel.wimPart = WTGModel.imageIndex.Substring(0, 1);
////}

////private void 更多高级选项ToolStripMenuItem_Click(object sender, EventArgs e)
////{
////    WTGSettings ws = new WTGSettings();
////    ws.Show();
////    //有些设置项目需要应用到WTGModel
////    WTGModel.wimPart = WTGModel.imageIndex.Substring(0, 1);
////}


////if (Environment.Is64BitOperatingSystem)
////{
////    WTGModel.imagexFileName = "imagex_x64.exe";
////}

////if (WTGOperation.forceFormat) //强制格式化
////{
////    System.Diagnostics.Process ud1 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\" + "\\fbinst.exe", (" " + WTGOperation.ud.Substring(0, 2) + " format -r -f"));//Format disk
////    ud1.WaitForExit();
////}


////File.Copy(WTGModel.ud.Substring(0, 2) + "\\EFI\\Boot\\bootx64.efi", WTGModel.diskpartScriptPath + "\\bootx64.efi", true);
////File.Copy(WTGModel.ud.Substring(0, 2) + "\\EFI\\Microsoft\\Boot\\BCD", WTGModel.diskpartScriptPath + "\\BCD", true);


////Directory.CreateDirectory(WTGModel.ud.Substring(0, 2) + "\\EFI");
////Directory.CreateDirectory(WTGModel.ud.Substring(0, 2) + "\\EFI\\Boot");
////Directory.CreateDirectory(WTGModel.ud.Substring(0, 2) + "\\EFI\\Microsoft");
////Directory.CreateDirectory(WTGModel.ud.Substring(0, 2) + "\\EFI\\Microsoft\\Boot");
////File.Copy(WTGModel.diskpartScriptPath + "\\bootx64.efi", WTGModel.ud.Substring(0, 2) + "\\EFI\\Boot\\bootx64.efi", true);
////File.Copy(WTGModel.diskpartScriptPath + "\\BCD", WTGModel.ud.Substring(0, 2) + "\\EFI\\Microsoft\\Boot\\BCD", true);

//#region VHDOperation
////private void createVHD(VHDOperation vo)
////{
////    if (WTGOperation.filetype == "vhd" || WTGOperation.filetype == "vhdx")
////    {
////        //vo.vhdPath = openFileDialog1.FileName;
////        //vpath = openFileDialog1.FileName;
////        //if (Directory.Exists(Application.StartupPath + "\\VHD"))
////        //{ Directory.Delete(Application.StartupPath + "\\VHD"); }
////        //Directory.CreateDirectory(Application.StartupPath + "\\VHD");
////        this.AttachVHD();
////        //ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\create.txt\"");
////        //
////    }
////    else
////    {

////        vo.SetVhdProp();
////        vo.ApplyToVdisk();
////        vo.UEFIAndWin7ToGo();
////    }
////    vo.WriteBootFiles();
////    #region OLDCODE
////    ////    ////////////////vhd设定///////////////////////
////    ////    string vhd_type = "expandable";
////    ////    vhd_size = "";
////    //    if (checkBoxfixed.Checked)
////    //    {
////    //        vo.vhdType = "fixed";
////    //    }
////    //    else 
////    //    {
////    //        vo.vhdType = "expandable";
////    //    }
////    //    if (numericUpDown1.Value != 0)
////    //    {
////    //        vo.vhdSize  = (numericUpDown1.Value * 1024).ToString();
////    //    }
////    //    else
////    //    {
////    //        if (!checkBoxwimboot.Checked)
////    //        {
////    //            if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 21504) { vo.vhdSize = "20480"; }
////    //            else { vo.vhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 500).ToString(); }
////    //        }
////    //        else
////    //        {
////    //            if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 24576) { vo.vhdSize = "20480"; }
////    //            else { vo.vhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 4096).ToString(); }
////    //        }
////    //    }
////    //    //needcopy = false;
////    //    WTGOperation.wimpart = ChoosePart.part;
////    //    ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
////    //    ////win7////
////    //    //int win7togo = iswin7(win8iso);
////    //    //if (win7togo != 0 && radiovhdx.Checked) { MessageBox.Show("WIN7 WTG系统不支持VHDX模式！"); return; }
////    //    //if (wimpart == 0)
////    //    //{//自动判断模式

////    //    //    if (win7togo == 1)
////    //    //    {//WIN7 32 bit

////    //    //        wimpart = 5;
////    //    //    }
////    //    //    else if (win7togo == 2)
////    //    //    { //WIN7 64 BIT

////    //    //        wimpart = 4;
////    //    //    }
////    //    //    else { wimpart = 1; }
////    //    //}
////    //    //MessageBox.Show(wimpart.ToString());
////    //    //////////////

////    //    ////////////////判断临时文件夹,VHD needcopy?///////////////////
////    //    int vhdmaxsize;
////    //    if (checkBoxfixed.Checked)
////    //    {
////    //        vhdmaxsize = System.Int32.Parse(vo.vhdSize ) * 1024 + 1024;
////    //    }
////    //    else
////    //    {
////    //        vhdmaxsize = 10485670;
////    //    }
////    //    if (DiskOperation.GetHardDiskFreeSpace(SetTempPath.temppath.Substring(0, 2) + "\\") <= vhdmaxsize || StringOperation.IsChina(SetTempPath.temppath) || checkBoxuefi.Checked || checkBoxuefimbr.Checked || checkBoxwimboot.Checked || checkBoxnotemp.Checked)
////    //    {
////    //        vo.needcopy = false;
////    //        //usetemp = false;
////    //    }
////    //    else 
////    //    {
////    //        vo.needcopy = true;
////    //        //usetemp = true;
////    //    }
////    //    if (vo.needcopy)
////    //    {
////    //        vo.needcopy = false;
////    //        //usetemp = false;
////    //        vo.vhdPath = WTGOperation.ud + win8vhdfile;
////    //    }
////    //    else
////    //    {
////    //        vo.vhdPath = Path.Combine(SetTempPath.temppath, win8vhdfile);
////    //        //SetTempPath.temppath + "\\" + win8vhdfile;
////    //        //needcopy = true;
////    //    }


////    //    /////////////////////////////////////////////////////

////    //    FileStream fs = new FileStream(diskpartscriptpath + "\\create.txt", FileMode.Create, FileAccess.Write);
////    //    fs.SetLength(0);
////    //    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
////    //    string ws = "";
////    //    try
////    //    {
////    //        ws = "create vdisk file=" + vo.vhdPath + " type=" + vhd_type + " maximum=" + vhd_size;
////    //        sw.WriteLine(ws);
////    //        ws = "select vdisk file=" + vo.vhdPath;
////    //        sw.WriteLine(ws);
////    //        ws = "attach vdisk";
////    //        sw.WriteLine(ws);
////    //        ws = "create partition primary";
////    //        sw.WriteLine(ws);
////    //        ws = "format fs=ntfs quick";
////    //        sw.WriteLine(ws);
////    //        ws = "assign letter=v";
////    //        sw.WriteLine(ws);
////    //        ws = "exit";
////    //        sw.WriteLine(ws);
////    //    }
////    //    catch { }
////    //    sw.Close();
////    //    ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\create.txt\"");


////    //    try
////    //    {
////    //        if (!System.IO.Directory.Exists("V:\\"))
////    //        {
////    //            ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
////    //            er.ShowDialog();
////    //            shouldcontinue = false;
////    //            return;
////    //        }
////    //    }
////    //    


////private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
////{
////    LoadUDList();
////}
////    //    {
////    //        //创建VHD失败，未知错误
////    //        ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
////    //        er.ShowDialog();
////    //        shouldcontinue = false;

////    //    }
////    //ImageOperation.ImageApply(checkBoxwimboot.Checked, WTGOperation.isesd, WTGOperation.imagex, WTGOperation.path, WTGOperation.wimpart, WTGOperation.ud, @"v:\");
////    //    //if (checkBoxwimboot.Checked)
////    //    //{
////    //    //    ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + win8iso + "\" /SourceIndex:" + wimpart.ToString() + " /DestinationImageFile:" + WTGOperation.ud + "wimboot.wim");
////    //    //    ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + WTGOperation.ud + "wimboot.wim" + "\" /ApplyDir:v: /Index:" + wimpart.ToString() + " /WIMBoot");

////    //    //}
////    //    //else
////    //    //{
////    //    //    if (isesd)
////    //    //    {
////    //    //        ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:v: /Index:" + wimpart.ToString());


////    //    //    }
////    //    //    else
////    //    //    {
////    //    //        ProcessManager.ECMD(Application.StartupPath + "\\files\\" + imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + "v:\\");

////    //    //    }
////    //    //}

////    //////////////
////    //if (WTGOperation.win7togo != 0) { ImageOperation.Win7REG("V:\\"); }
////    ////////////////
////    //if (checkBoxuefi.Checked)
////    //{
////    //    ImageOperation.Fixletter("C:", "V:");
////    //    //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:v  > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
////    //}
////    //}

////    //if (!usetemp)
////    //{
////    //    if (checkBoxuefi.Checked)
////    //    {
////    //        BootFileOperation.BcdbootWriteUEFIBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);

////    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f UEFI");


////    //    }
////    //    else if (checkBoxuefimbr.Checked)
////    //    {
////    //        BootFileOperation.BcdbootWriteALLBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);
////    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f ALL");


////    //    }
////    //    else if (checkBoxwimboot.Checked)
////    //    {
////    //        BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);

////    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");


////    //    }
////    //    else
////    //    {
////    //        if (!checkBoxcommon.Checked)
////    //        {
////    //            BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);

////    //            //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");

////    //        }
////    //        else
////    //        {
////    //            needcopyvhdbootfile = true;
////    //            //copyvhdbootfile();
////    //        }
////    //    }
////    //}
////    //else
////    //{
////    //    needcopyvhdbootfile = true;
////    //}
////    #endregion
////}
////private void copyVHD()
////{


////    if (usetemp)
////    {
////        if (System.IO.File.Exists(vo.vhdPath ))
////        {
////            //Application.DoEvents();
////            //Thread.Sleep(100);
////            //Application.DoEvents();

////            ProcessManager.ECMD(WTGOperation.filesPath+ "\\fastcopy.exe", " /auto_close \"" + vo.vhdPath  + "\" /to=\"" + WTGOperation.ud + "\"");
////            //MsgManager.getResString("Msg_Copy")
////            //复制文件中...大约需要10分钟~1小时，请耐心等待！\r\n
////            ProcessManager.AppendText (MsgManager.getResString("Msg_Copy", MsgManager.ci));


////            //AppendText("复制文件中...大约需要10分钟~1小时，请耐心等待！");
////            //wp.Show();
////            //Application.DoEvents();
////            //System.Diagnostics.Process cp = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\fastcopy.exe", " /auto_close \"" + Form1.vpath + "\" /to=\"" + ud + "\"");
////            //cp.WaitForExit();
////            //wp.Close();
////        }
////        if ((WTGOperation.filetype == "vhd" && !vo.vhdPath.EndsWith("win8.vhd")) || (WTGOperation.filetype == "vhdx" && !vo.vhdPath.EndsWith("win8.vhdx")))
////        {
////            //Rename
////            //MsgManager.getResString("Msg_RenameError")
////            //重命名错误
////            try { File.Move(WTGOperation.ud + vo.vhdPath.Substring(vo.vhdPath.LastIndexOf("\\") + 1), WTGOperation.ud + Form1.win8vhdfile); }
////            catch (Exception ex) { MessageBox.Show(MsgManager.getResString("Msg_RenameError", MsgManager.ci) + ex.ToString()); }
////        }

////        //copy cp = new copy(ud);
////        //cp.ShowDialog();

////    }

////}
////private void VHDExtra()
////{
////    ImageOperation.ImageExtra(checkBoxframework.Checked, checkBox_san_policy.Checked, checkBoxdiswinre.Checked, "v:", wimbox.Text);
////    //try
////    //{
////    //    ////////////.net 3.5//////////////////
////    //    if (checkBoxframework.Checked)
////    //    {
////    //        ProcessManager.ECMD("dism.exe", " /image:v: /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
////    //        

////    //    }
////    //    /////////////////屏蔽本机硬盘///////////////////////////////////
////    //    if (checkBox_san_policy.Checked)
////    //    {
////    //        ProcessManager.ECMD("dism.exe", " /image:v: /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
////    //        
////    //    }
////    //    /////////////////////禁用WINRE//////////////////////////////
////    //    if (checkBoxdiswinre.Checked)
////    //    {
////    //        File.Copy(Application.StartupPath + "\\files\\unattend.xml", "v:\\Windows\\System32\\sysprep\\unattend.xml");
////    //    }
////    //    //////////////
////    //}
////    //catch (Exception ex)
////    //{
////    //    MessageBox.Show(ex.ToString());
////    //}

////}

////private void copyVHDBootFiles()
////{
////    vo.CopyVHD();

////    ProcessManager.ECMD("xcopy.exe", "\"" + WTGOperation.filesPath+ "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + " /e /h /y");

////    if (radiovhdx.Checked)
////    {
////        ProcessManager.ECMD("xcopy.exe", "\"" + WTGOperation.filesPath+ "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + "\\boot\\ /e /h /y");

////    }
////    BootFileOperation.BooticeWriteMBRPBRAndAct(WTGOperation.ud);
////    ///////////////////////////////////////////////////////
////    //System.Diagnostics.Process booice = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", (" /DEVICE=" + ud.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
////    //booice.WaitForExit();
////    //System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", (" /DEVICE=" + ud.Substring(0, 2) + " /pbr /install /type=bootmgr /quiet"));//写入引导
////    //pbr.WaitForExit();
////    //System.Diagnostics.Process act = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\bootice.exe", " /DEVICE=" + ud.Substring(0, 2) + " /partitions /activate /quiet");
////    //act.WaitForExit();

////}
////private void detachVHDExtra() 
////{
////    if (File.Exists(diskpartscriptpath + "\\vdisklist.txt")) { File.Delete(diskpartscriptpath + "\\vdisklist.txt"); }
////    ///////////////////detach vdisk/////////////////////
////    FileStream fs1 = new FileStream(diskpartscriptpath + "\\vdisklist.txt", FileMode.Create, FileAccess.Write);
////    fs1.SetLength(0);
////    StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default);
////    string ws = "";
////    try
////    {

////        ws = "list vdisk";
////        sw1.WriteLine(ws);
////    }
////    catch { }
////    sw1.Close();
////    ProcessManager.SyncCMD("diskpart.exe /s \"" + diskpartscriptpath + "\\vdisklist.txt" + "\" > " + "\"" + diskpartscriptpath + "\\vhdlist.txt" + "\"");
////    FileStream fs2 = new FileStream(diskpartscriptpath + "\\vhdlist.txt", FileMode.Open);
////    StreamReader sr1 = new StreamReader(fs2,Encoding .Default );
////    string dpoutput = sr1.ReadToEnd();
////    int currentindex=0;
////    int win8vhdindex=0;
////    string vhdname = "";
////    if (dpoutput.Contains("win8.vhdx")) 
////    {
////        vhdname = "win8.vhdx";

////    }
////    else if (dpoutput.Contains("win8.vhd")) 
////    {
////        vhdname = "win8.vhd";
////    }
////    currentindex = dpoutput.IndexOf(vhdname) - 1;
////    win8vhdindex = currentindex;
////    while (currentindex > 0) 
////    {
////        if (dpoutput[currentindex].ToString () == ":") { break; }
////        currentindex--;
////    }
////    string vhdmountpath = "";
////    if (currentindex > 0)
////    {
////        vhdmountpath = dpoutput.Substring(currentindex - 1, win8vhdindex - currentindex + 2) + vhdname;
////        detachVHD(vhdmountpath);
////    }

////    //MessageBox.Show(vhdmountpath);


////}
////private void VHDDynamicSizeIns()
////{
////    //MsgManager.getResString("FileName_VHD_Dynamic")
////    //VHD模式说明.TXT
////    FileStream fs1 = new FileStream(ud + MsgManager.getResString("FileName_VHD_Dynamic", MsgManager.ci), FileMode.Create, FileAccess.Write);
////    fs1.SetLength(0);
////    StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default);
////    string ws1 = "";
////    try
////    {
////        //MsgManager.getResString("Msg_VHDDynamicSize")
////        //您创建的VHD为动态大小VHD，实际VHD容量：
////        ////MsgManager.getResString("Msg_VHDDynamicSize2")
////        //在VHD系统启动后将自动扩充为实际容量。请您在优盘留有足够空间确保系统正常启动！
////        ws1 = MsgManager.getResString("Msg_VHDDynamicSize", MsgManager.ci) + vhd_size + "MB\n";
////        sw1.WriteLine(ws1);
////        ws1 = MsgManager.getResString("Msg_VHDDynamicSize2", MsgManager.ci);
////        sw1.WriteLine(ws1);
////    }
////    catch { }
////    sw1.Close();

////}

//#endregion

//#region OLDCODE
//////判断是否WIN7，自动选择安装分卷
//////int win7togo = iswin7(win8iso);


////if (wimpart == 0)
////{//自动判断模式

////    if (win7togo == 1)
////    {//WIN7 32 bit

////        wimpart = 5;
////    }
////    else if (win7togo == 2)
////    { //WIN7 64 BIT

////        wimpart = 4;
////    }
////    else { wimpart = 1; }
////}

////if (checkBoxwimboot.Checked)
////{
////    ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + win8iso + "\" /SourceIndex:" + wimpart.ToString() + " /DestinationImageFile:" + ud + "wimboot.wim");
////    
////    ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + ud + "wimboot.wim" + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString() + " /WIMBoot");
////    

////}
////else
////{
////    if (isesd)
////    {
////        ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString());
////        
////    }
////    else
////    {
////        ProcessManager.ECMD(Application.StartupPath + "\\files\\" + imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + ud);
////        
////    }
////}


////if (checkBoxframework.Checked)
////{
////    ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
////    

////}
////if (checkBox_san_policy.Checked)
////{
////    ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
////    

////}

////if (checkBoxdiswinre.Checked)
////{
////    File.Copy(Application.StartupPath + "\\files\\unattend.xml", ud + "Windows\\System32\\sysprep\\unattend.xml");
////}


////ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, ud + "windows  /s  x: /f ALL");
////
////System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\bootice.exe", " /DEVICE=x: /partitions /activate  /quiet");
////p2.WaitForExit();

////finish f = new finish();
////f.ShowDialog();

//#endregion
////using (FileStream fs = new FileStream(diskpartscriptpath + "\\create.txt", FileMode.Create, FileAccess.Write))
////{
////    fs.SetLength(0);
////    using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
////    {
////        string ws = "";

////        ws = "create vdisk file=" + this.vhdPath + " type=" + this.vhdType + " maximum=" + this.vhdSize;
////        sw.WriteLine(ws);
////        ws = "select vdisk file=" + this.vhdPath;
////        sw.WriteLine(ws);
////        ws = "attach vdisk";
////        sw.WriteLine(ws);
////        ws = "create partition primary";
////        sw.WriteLine(ws);
////        ws = "format fs=ntfs quick";
////        sw.WriteLine(ws);
////        ws = "assign letter=v";
////        sw.WriteLine(ws);
////        ws = "exit";
////        sw.WriteLine(ws);

////    }
////}
////ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\create.txt\"");


////private string GetRegistData(string name)
////{
////    string registData;
////    RegistryKey hkml = Registry.LocalMachine;
////    RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
////    RegistryKey aimdir = software.OpenSubKey("XXX", true);
////    registData = aimdir.GetValue(name).ToString();
////    return registData;
////} 

////    ImageOperation.ImageExtra(checkBoxframework.Checked, checkBox_san_policy.Checked, checkBoxdiswinre.Checked, "v:", wimbox.Text);
////    //try
////    //{
////    //    ////////////.net 3.5//////////////////
////    //    if (checkBoxframework.Checked)
////    //    {
////    //        ProcessManager.ExecuteCMD("dism.exe", " /image:v: /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
////    //        wp.ShowDialog();

////    //    }
////    //    /////////////////屏蔽本机硬盘///////////////////////////////////
////    //    if (checkBox_san_policy.Checked)
////    //    {
////    //        ProcessManager.ExecuteCMD("dism.exe", " /image:v: /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
////    //        wp.ShowDialog();
////    //    }
////    //    /////////////////////禁用WINRE//////////////////////////////
////    //    if (checkBoxdiswinre.Checked)
////    //    {
////    //        File.Copy(Application.StartupPath + "\\files\\unattend.xml", "v:\\Windows\\System32\\sysprep\\unattend.xml");
////    //    }
////    //    //////////////
////    //}
////    //catch (Exception ex)
////    //{
////    //    MessageBox.Show(ex.ToString());
////    //}

////}
////private void createVHD()
////{
////    if (WTGOperation.filetype == "vhd" || WTGOperation.filetype == "vhdx")
////    {
////        this.AttachVHD();
////    }
////    else
////    {

////        this.SetVhdProp();
////        this.ApplyToVdisk();
////        this.UEFIAndWin7ToGo();
////    }
////    this.WriteBootFiles();
////    #region OLDCODE
////    ////    ////////////////vhd设定///////////////////////
////    ////    string vhd_type = "expandable";
////    ////    vhd_size = "";
////    //    if (checkBoxfixed.Checked)
////    //    {
////    //        vo.vhdType = "fixed";
////    //    }
////    //    else 
////    //    {
////    //        vo.vhdType = "expandable";
////    //    }
////    //    if (numericUpDown1.Value != 0)
////    //    {
////    //        vo.vhdSize  = (numericUpDown1.Value * 1024).ToString();
////    //    }
////    //    else
////    //    {
////    //        if (!checkBoxwimboot.Checked)
////    //        {
////    //            if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 21504) { vo.vhdSize = "20480"; }
////    //            else { vo.vhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 500).ToString(); }
////    //        }
////    //        else
////    //        {
////    //            if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 24576) { vo.vhdSize = "20480"; }
////    //            else { vo.vhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 4096).ToString(); }
////    //        }
////    //    }
////    //    //needcopy = false;
////    //    WTGOperation.wimpart = ChoosePart.part;
////    //    ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
////    //    ////win7////
////    //    //int win7togo = iswin7(win8iso);
////    //    //if (win7togo != 0 && radiovhdx.Checked) { MessageBox.Show("WIN7 WTG系统不支持VHDX模式！"); return; }
////    //    //if (wimpart == 0)
////    //    //{//自动判断模式

////    //    //    if (win7togo == 1)
////    //    //    {//WIN7 32 bit

////    //    //        wimpart = 5;
////    //    //    }
////    //    //    else if (win7togo == 2)
////    //    //    { //WIN7 64 BIT

////    //    //        wimpart = 4;
////    //    //    }
////    //    //    else { wimpart = 1; }
////    //    //}
////    //    //MessageBox.Show(wimpart.ToString());
////    //    //////////////

////    //    ////////////////判断临时文件夹,VHD needcopy?///////////////////
////    //    int vhdmaxsize;
////    //    if (checkBoxfixed.Checked)
////    //    {
////    //        vhdmaxsize = System.Int32.Parse(vo.vhdSize ) * 1024 + 1024;
////    //    }
////    //    else
////    //    {
////    //        vhdmaxsize = 10485670;
////    //    }
////    //    if (DiskOperation.GetHardDiskFreeSpace(SetTempPath.temppath.Substring(0, 2) + "\\") <= vhdmaxsize || StringOperation.IsChina(SetTempPath.temppath) || checkBoxuefi.Checked || checkBoxuefimbr.Checked || checkBoxwimboot.Checked || checkBoxnotemp.Checked)
////    //    {
////    //        vo.needcopy = false;
////    //        //usetemp = false;
////    //    }
////    //    else 
////    //    {
////    //        vo.needcopy = true;
////    //        //usetemp = true;
////    //    }
////    //    if (vo.needcopy)
////    //    {
////    //        vo.needcopy = false;
////    //        //usetemp = false;
////    //        vo.vhdPath = WTGOperation.ud + win8vhdfile;
////    //    }
////    //    else
////    //    {
////    //        vo.vhdPath = Path.Combine(SetTempPath.temppath, win8vhdfile);
////    //        //SetTempPath.temppath + "\\" + win8vhdfile;
////    //        //needcopy = true;
////    //    }


////    //    /////////////////////////////////////////////////////

////    //    FileStream fs = new FileStream(diskpartscriptpath + "\\create.txt", FileMode.Create, FileAccess.Write);
////    //    fs.SetLength(0);
////    //    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
////    //    string ws = "";
////    //    try
////    //    {
////    //        ws = "create vdisk file=" + vo.vhdPath + " type=" + vhd_type + " maximum=" + vhd_size;
////    //        sw.WriteLine(ws);
////    //        ws = "select vdisk file=" + vo.vhdPath;
////    //        sw.WriteLine(ws);
////    //        ws = "attach vdisk";
////    //        sw.WriteLine(ws);
////    //        ws = "create partition primary";
////    //        sw.WriteLine(ws);
////    //        ws = "format fs=ntfs quick";
////    //        sw.WriteLine(ws);
////    //        ws = "assign letter=v";
////    //        sw.WriteLine(ws);
////    //        ws = "exit";
////    //        sw.WriteLine(ws);
////    //    }
////    //    catch { }
////    //    sw.Close();
////    //    ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\create.txt\"");


////    //    try
////    //    {
////    //        if (!System.IO.Directory.Exists("V:\\"))
////    //        {
////    //            ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
////    //            er.ShowDialog();
////    //            shouldcontinue = false;
////    //            return;
////    //        }
////    //    }
////    //    catch
////    //    {
////    //        //创建VHD失败，未知错误
////    //        ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
////    //        er.ShowDialog();
////    //        shouldcontinue = false;

////    //    }
////    //ImageOperation.ImageApply(checkBoxwimboot.Checked, WTGOperation.isesd, WTGOperation.imagex, WTGOperation.path, WTGOperation.wimpart, WTGOperation.ud, @"v:\");
////    //    //if (checkBoxwimboot.Checked)
////    //    //{
////    //    //    ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + win8iso + "\" /SourceIndex:" + wimpart.ToString() + " /DestinationImageFile:" + WTGOperation.ud + "wimboot.wim");
////    //    //    ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + WTGOperation.ud + "wimboot.wim" + "\" /ApplyDir:v: /Index:" + wimpart.ToString() + " /WIMBoot");

////    //    //}
////    //    //else
////    //    //{
////    //    //    if (isesd)
////    //    //    {
////    //    //        ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:v: /Index:" + wimpart.ToString());


////    //    //    }
////    //    //    else
////    //    //    {
////    //    //        ProcessManager.ECMD(Application.StartupPath + "\\files\\" + imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + "v:\\");

////    //    //    }
////    //    //}

////    //////////////
////    //if (WTGOperation.win7togo != 0) { ImageOperation.Win7REG("V:\\"); }
////    ////////////////
////    //if (checkBoxuefi.Checked)
////    //{
////    //    ImageOperation.Fixletter("C:", "V:");
////    //    //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:v  > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
////    //}
////    //}

////    //if (!usetemp)
////    //{
////    //    if (checkBoxuefi.Checked)
////    //    {
////    //        BootFileOperation.BcdbootWriteUEFIBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);

////    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f UEFI");


////    //    }
////    //    else if (checkBoxuefimbr.Checked)
////    //    {
////    //        BootFileOperation.BcdbootWriteALLBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);
////    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f ALL");


////    //    }
////    //    else if (checkBoxwimboot.Checked)
////    //    {
////    //        BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);

////    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");


////    //    }
////    //    else
////    //    {
////    //        if (!checkBoxcommon.Checked)
////    //        {
////    //            BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);

////    //            //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");

////    //        }
////    //        else
////    //        {
////    //            needcopyvhdbootfile = true;
////    //            //copyvhdbootfile();
////    //        }
////    //    }
////    //}
////    //else
////    //{
////    //    needcopyvhdbootfile = true;
////    //}
////    #endregion
////}

//#region detachVHDExtra
////FileOperation.DeleteFile(diskpartscriptpath + "\\vdisklist.txt");
///////////////////////detach vdisk/////////////////////
////File.WriteAllText(diskpartscriptpath + "\\vdisklist.txt", "list vdisk", Encoding.Default);
//////FileStream fs1 = new FileStream(diskpartscriptpath + "\\vdisklist.txt", FileMode.Create, FileAccess.Write);
//////fs1.SetLength(0);
//////StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default);
//////string ws = "";
//////try
//////{

//////    ws = "list vdisk";
//////    sw1.WriteLine(ws);
//////}
//////catch { }
//////sw1.Close();
////ProcessManager.SyncCMD("diskpart.exe /s \"" + diskpartscriptpath + "\\vdisklist.txt" + "\" > " + "\"" + diskpartscriptpath + "\\vhdlist.txt" + "\"");
//////FileStream fs2 = new FileStream(diskpartscriptpath + "\\vhdlist.txt", FileMode.Open);
//////StreamReader sr1 = new StreamReader(fs2, Encoding.Default);
//////string dpoutput = sr1.ReadToEnd();


////int currentindex = 0;
////int win8vhdindex = 0;
////string vhdname = "";
////if (dpoutput.Contains("win8.vhdx"))
////{
////    vhdname = "win8.vhdx";

////}
////else if (dpoutput.Contains("win8.vhd"))
////{
////    vhdname = "win8.vhd";
////}
////currentindex = dpoutput.IndexOf(vhdname) - 1;
////win8vhdindex = currentindex;
////while (currentindex > 0)
////{
////    if (dpoutput[currentindex].ToString() == ":") { break; }
////    currentindex--;
////}
//////string vhdmountpath = "";
////if (currentindex > 0)
////{
////    this.vhdPath = dpoutput.Substring(currentindex - 1, win8vhdindex - currentindex + 2) + vhdname;
////    //vhdmountpath = 
////    detachVHD();
////}

////MessageBox.Show(vhdmountpath);
//#endregion
//#region cleanTempOldCode
////int vhdmaxsize;
////if (checkBoxfixed.Checked)
////{
////    vhdmaxsize = System.Int32.Parse(vhd_size) * 1048576 + 1024;
////}
////else
////{
////    vhdmaxsize = 10485670;
////}
////if (DiskOperation.GetHardDiskFreeSpace(SetTempPath.temppath.Substring(0, 2) + "\\") <= vhdmaxsize || IsChina(SetTempPath.temppath) || checkBoxuefi.Checked || checkBoxuefimbr.Checked || checkBoxwimboot.Checked)
////{
////    usetemp = false;
////}

////if (!usetemp)
////{
////    vpath = ud + win8vhdfile;
////}
////else
////{
////    vpath = SetTempPath.temppath + "\\" + win8vhdfile;
////    //needcopy = true;
////}
////detachVHD(vpath);
//#endregion
////public void GenerateAttachVHDScript(string vpath)
////{
////    StringBuilder sb = new StringBuilder();

////    sb.AppendLine("select vdisk file=" + vpath);
////    sb.AppendLine("attach vdisk");
////    sb.AppendLine("sel partition 1");
////    sb.AppendLine("assign letter=v");
////    sb.AppendLine("exit");
////    DiskpartScriptManager dsm = new DiskpartScriptManager();
////    dsm.args = sb.ToString();
////    dsm.RunDiskpartScript(false);

////    //using (FileStream fs0 = new FileStream(diskpartscriptpath + @"\attach.txt", FileMode.Create, FileAccess.Write))
////    //{
////    //    fs0.SetLength(0);
////    //    using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default))
////    //    {
////    //        //string ws0 = "";
////    //        //StringBuilder sb = new StringBuilder();
////    //        //sb.Append("select vdisk file=" + vpath + "\n");
////    //        //try
////    //        //{
////    //        //ws0 = "select vdisk file=" + vpath;
////    //        sw0.WriteLine("select vdisk file=" + vpath);
////    //        sw0.WriteLine("attach vdisk");
////    //        sw0.WriteLine("sel partition 1");
////    //        sw0.WriteLine("assign letter=v");
////    //        sw0.WriteLine("exit");
////    //    }
////    //}
////}
////private void ad()
////{
////    string pageHtml1;
////    //MessageBox.Show("Test");
////    try
////    {
////        //MessageBox.Show("Test");
////        WebClient MyWebClient = new WebClient();

////        MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

////        Byte[] pageData = MyWebClient.DownloadData("http://bbs.luobotou.org/app/wintogo.txt"); //从指定网站下载数据

////        pageHtml1 = Encoding.UTF8.GetString(pageData);
////        // MessageBox.Show(pageHtml1);
////        int index = pageHtml1.IndexOf("announcement=");
////        int indexbbs = pageHtml1.IndexOf("bbs=");
////        // MessageBox.Show(pageHtml1.Substring(index + 13, 1));
////        //MessageBox.Show(MsgManager.ci.EnglishName );
////        //CultureInfo ca = new CultureInfo("en");
////        if (pageHtml1.Substring(index + 13, 1) != "0" && MsgManager.ci.EnglishName != "English")
////        {
////            if (pageHtml1.Substring(indexbbs + 4, 1) == "1")
////            {
////                string pageHtml;


////                WebClient MyWebClient1 = new WebClient();

////                MyWebClient1.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

////                Byte[] pageData1 = MyWebClient1.DownloadData("http://bbs.luobotou.org/portal.php"); //从指定网站下载数据

////                pageHtml = Encoding.UTF8.GetString(pageData1);
////                //MessageBox.Show(pageHtml);
////                int index1 = pageHtml.IndexOf("<ul><li><a href=");
////                for (int i = 0; i < 10; i++)
////                {
////                    int LinkStartIndex = pageHtml.IndexOf("<li><a href=", index1) + 13;
////                    int LinkEndIndex = pageHtml.IndexOf("\"", LinkStartIndex);
////                    int TitleStartIndex = pageHtml.IndexOf("title=", LinkEndIndex) + 7;
////                    int TitleEndIndex = pageHtml.IndexOf("\"", TitleStartIndex);

////                    topiclink[i] = pageHtml.Substring(LinkStartIndex, LinkEndIndex - LinkStartIndex);
////                    topicname[i] = pageHtml.Substring(TitleStartIndex, TitleEndIndex - TitleStartIndex);
////                    //MessageBox.Show(topiclink[i] + topicname[i]);
////                    index1 = LinkEndIndex;
////                    //topicstring 
////                    //int adprogram = index1 + Application.ProductName.Length + 1;

////                }
////                //string portal_block = pageHtml.Substring;
////                //String adtitle;
////                ////MessageBox.Show(adprogram.ToString() + " " + startindex);
////                //adtitle = pageHtml.Substring(adprogram, startindex - adprogram);

////                //adlink = pageHtml.Substring(startindex + 1, endindex - startindex - 1);
////                //linkLabel2.Invoke(Set_Text, new object[] { adtitle });
////                //MessageBox.Show("");

////                //MessageBox.Show(adtitle + "     " + adlink);

////            }

////            {

////                //MessageBox.Show("Test");
////                string pageHtml;
////                WebClient MyWebClient1 = new WebClient();

////                MyWebClient1.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。
////                MyWebClient1.Encoding = Encoding.UTF8;
////                pageHtml = MyWebClient1.DownloadString("http://bbs.luobotou.org/app/announcement.txt");

////                //Byte[] pageData1 = MyWebClient1.DownloadData("http://bbs.luobotou.org/app/announcement.txt"); //从指定网站下载数据
////                //pageHtml = Encoding.UTF8.GetString(pageData1);
////                //MessageBox.Show(pageHtml);
////                //int index1 = pageHtml.IndexOf(Application.ProductName);
////                //int startindex = pageHtml.IndexOf("~", index1);
////                //int endindex = pageHtml.IndexOf("结束", index1);
////                //int adprogram = index1 + Application.ProductName.Length + 1;
////                Match match = Regex.Match(pageHtml, Application.ProductName + "=(.+)~(.+)结束");
////                //Set_Text(match.Groups[1].Value);
////                adlink = match.Groups[2].Value;
////                String adtitle;
////                adtitle = match.Groups[1].Value;
////                ////MessageBox.Show(adprogram.ToString() + " " + startindex);
////                //adtitle = pageHtml.Substring(adprogram, startindex - adprogram);
////                //adtitles = adtitle;
////                //adlink = pageHtml.Substring(startindex + 1, endindex - startindex - 1);
////                //Set_Text(adtitle);
////                linkLabel2.Invoke(Set_Text, new object[] { adtitle });
////                //linkLabel2(Set_Text);
////                //MessageBox.Show("");
////                //writeprogress .linklabel1
////                //MessageBox.Show(adtitle + "     " + adlink);

////            }
////        }
////    }
////    catch { }


////}
////public void detachVHD()
////{
////    if (!ShouldContinue) return;
////    else ShouldContinue = false;
////    FileOperation.DeleteFile(diskpartscriptpath + "\\detach.txt");
////    ///////////////////detach vdisk/////////////////////
////    FileStream fs1 = new FileStream(diskpartscriptpath + "\\detach.txt", FileMode.Create, FileAccess.Write);
////    fs1.SetLength(0);
////    StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default);
////    string ws = "";
////    try
////    {
////        ws = "select vdisk file=" + VhdPath;
////        sw1.WriteLine(ws);
////        ws = "detach vdisk";
////        sw1.WriteLine(ws);
////    }
////    catch { }
////    sw1.Close();
////    ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\detach.txt\"");
////    ShouldContinue = true;
////}

////private void diskPart() 
////{

////    //ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
////    //if (DialogResult.No == MessageBox.Show("此操作将会清除移动磁盘所有分区的所有数据，确认？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
////    //if (DialogResult.No == MessageBox.Show("您确定要继续吗？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; } 

////    FileStream fs0 = new FileStream(WTGOperation . diskpartscriptpath + "\\dp.txt", FileMode.Create, FileAccess.Write);
////    fs0.SetLength(0);
////    StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default);
////    string ws0 = "";
////    try
////    {
////        ws0 = "select volume " + WTGOperation.ud.Substring(0, 1);
////        sw0.WriteLine(ws0);
////        ws0 = "clean";
////        sw0.WriteLine(ws0);
////        ws0 = "convert mbr";
////        sw0.WriteLine(ws0);
////        ws0 = "create partition primary";
////        sw0.WriteLine(ws0);
////        ws0 = "select partition 1";
////        sw0.WriteLine(ws0);
////        ws0 = "format fs=ntfs quick";
////        sw0.WriteLine(ws0);
////        ws0 = "active";
////        sw0.WriteLine(ws0);
////        ws0 = "assign letter=" + WTGOperation.ud.Substring(0, 1);
////        sw0.WriteLine(ws0);
////        ws0 = "exit";
////        sw0.WriteLine(ws0);
////    }
////    catch { }
////    sw0.Close();

////    ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartscriptpath + "\\dp.txt\"");

////    //System.Diagnostics.Process dpc = System.Diagnostics.Process.Start("diskpart.exe", " /s " + Application.StartupPath + "\\dp.txt");
////    //dpc.WaitForExit();
////}
////private void 错误提示测试ToolStripMenuItem_Click(object sender, EventArgs e)
////      {
////          //DiskpartScriptManager dsm = new DiskpartScriptManager();
////          //dsm.args = "list disk";
////          //dsm.RunDiskpartScript(true);
////          //MessageBox.Show(dsm.CreateScriptFile()); 
////          //vo.detachVHDExtra();

////          //ProcessManager.ECMD(WTGOperation.filesPath+ "\\fastcopy.exe", " /auto_close \"" + Form1.vpath + "\" /to=\"" + ud + "\"");
////          ////MsgManager.getResString("Msg_Copy")
////          ////复制文件中...大约需要10分钟~1小时，请耐心等待！\r\n
////          //AppendText(MsgManager.getResString("Msg_Copy", MsgManager.ci));

////          //
////          ////AppendText("test");
////          ////
////          ////WindowsImageContainer wic = new WindowsImageContainer("", WindowsImageContainer.CreateFileMode.OpenAlways, WindowsImageContainer.CreateFileAccess.Read);

////          //IImage im=null;

////          //im.Apply("");
////          //wic.a
////          //copyfile = new Thread(fc);
////          //copyfile.Start();

////          //System.Diagnostics.Process.Start("c:\\windows\\system32\\bcdboot.exe");
////          //MessageBox.Show(Environment.GetEnvironmentVariable("windir") + "\\system32\\bcdboot.exe");
////          //ProcessManager.ECMD(Environment.GetEnvironmentVariable("windir") + "\\system32\\bcdboot.exe", "  " + "V:\\" + "windows  /s  x: /f UEFI");
////          //
////          //MessageBox.Show(Application.ProductName);
////          //Fixletter("C:","J:");
////          //error ex = new error("测试错误信息！TEST!");
////          //ex.Show();
////      }
//private void toolStripMenuItemvhdx_Click(object sender, EventArgs e)
//{
//    if (comboBoxUd.SelectedIndex == 0)
//    {
//        MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return;
//    }
//    WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
//    try
//    {
//        ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + " /e /h /y");
//        ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + "\\boot\\ /e /h /y");
//    }
//    catch (Exception ex)
//    {
//        Log.WriteLog("Err_toolStripMenuItemvhdx_Click", ex.ToString());
//        ErrorMsg err = new ErrorMsg(ex.Message);
//        err.Show();
//    }

//}

////private void ProcessManager.ECMD(string StartFileName, string StartFileArg)
////{

////    Process process = new Process();
////    //

////    try
////    {
////        AppendText("Command:" + StartFileName + StartFileArg+"\r\n");
////        process.StartInfo.FileName = StartFileName;
////        process.StartInfo.Arguments = StartFileArg;
////        process.StartInfo.UseShellExecute = false;
////        process.StartInfo.RedirectStandardInput = true;
////        process.StartInfo.RedirectStandardOutput = true;
////        process.StartInfo.RedirectStandardError = true;
////        process.StartInfo.CreateNoWindow = true;
////        process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
////        process.EnableRaisingEvents = true;
////        process.Exited += new EventHandler(progress_Exited);

////        process.Start();


////        process.BeginOutputReadLine();


////    }
////    catch (Exception ex)
////    {
////        //MsgManager.getResString("Msg_Failure")
////        //操作失败
////        MessageBox.Show(MsgManager.getResString("Msg_Failure", MsgManager.ci) + ex.ToString());
////    }

////}
////private void report()
////{
////    string pageHtml;
////    try
////    {

////        WebClient MyWebClient = new WebClient();

////        MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

////        Byte[] pageData = MyWebClient.DownloadData("http://bbs.luobotou.org/app/wintogo.txt"); //从指定网站下载数据

////        pageHtml = Encoding.Default.GetString(pageData);
////        //MessageBox.Show(pageHtml);
////        int index = pageHtml.IndexOf("webreport=");

////        if (pageHtml.Substring(index + 10, 1) == "1")
////        {
////            string strURL = "http://myapp.luobotou.org/statistics.aspx?name=wtg&ver=" + Application.ProductVersion;
////            System.Net.HttpWebRequest request;
////            // 创建一个HTTP请求
////            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
////            //request.Method="get";
////            System.Net.HttpWebResponse response;
////            response = (System.Net.HttpWebResponse)request.GetResponse();
////            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
////            string responseText = myreader.ReadToEnd();
////            myreader.Close();

////        }


////    }
////    catch (WebException webEx)
////    {

////        Console.WriteLine(webEx.Message.ToString());

////    }
////}
////private void update()
////{
////    string autoup = IniFile.ReadVal("Main", "AutoUpdate", Application.StartupPath + "\\files\\settings.ini");
////    if (autoup == "0") { return; }
//////if (IsRegeditExit(Application.ProductName)) { if ((GetRegistData("nevercheckupdate")) == "1") { return; } }

////    string pageHtml;
////    try
////    {

////        WebClient MyWebClient = new WebClient();
////        //MyWebClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

////        MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

////        Byte[] pageData = MyWebClient.DownloadData("http://bbs.luobotou.org/app/wintogo.txt"); //从指定网站下载数据

////        pageHtml = Encoding.UTF8 .GetString(pageData);
////       //essageBox.Show(pageHtml );
////        int index = pageHtml.IndexOf("~");
////        String ver;

////        ver = pageHtml.Substring(index + 1, 7);
////        if (ver != Application.ProductVersion)
////        {
////            try
////            {
////                Update frmf = new Update(ver);
////                frmf.ShowDialog();
////            }
////            catch { }

////        }

////    } 
////    catch (WebException webEx)
////    {

////        Console.WriteLine(webEx.Message.ToString());

////    }
////}

//#region OLDCODE
////int win7togo = iswin7(win8iso);
////if (wimpart == 0)
////{//自动判断模式

////    if (win7togo == 1)
////    {//WIN7 32 bit

////        wimpart = 5;
////    }
////    else if (win7togo == 2)
////    { //WIN7 64 BIT

////        wimpart = 4;
////    }
////    else { wimpart = 1; }
////}
////if (checkBoxwimboot.Checked)
////{
////    ImageOperation.WimbootApply(win8iso, ud,wimpart );
////    //ProcessManager.ECMD("Dism.exe"," /Export-Image /WIMBoot /SourceImageFile:\""+win8iso+"\" /SourceIndex:"+wimpart .ToString ()+" /DestinationImageFile:"+ud+"wimboot.wim");
////    //
////    //ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + ud + "wimboot.wim" + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString() + " /WIMBoot");
////    //

////}
////else
////{
////    if (isesd)
////    {
////        ImageOperation.ESDApply(win8iso, ud, wimpart);
////        //ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString());
////        //
////    }
////    else
////    {
////        ProcessManager.ECMD(Application.StartupPath + "\\files\\" + imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + ud);
////        
////    }
////}
///////////////
////try
////{
////    System.Diagnostics.Process booice = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", (" /DEVICE=" + ud.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
////    booice.WaitForExit();
////    System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", (" /DEVICE=" + ud.Substring(0, 2) + " /pbr /install /type=bootmgr  /quiet"));//写入引导
////    pbr.WaitForExit();
////    System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", " /DEVICE=" + ud.Substring(0, 2) + " /partitions /activate  /quiet");
////    p2.WaitForExit();
////}
////catch (Exception ex) { MessageBox.Show(ex.ToString()); }
////if (checkBoxframework.Checked)
////{
////    if (win7togo == 0)
////    {
////        ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
////        
////    }

////}
///////////////////////////////////////////////
////if (checkBox_san_policy.Checked)
////{
////    ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
////    
////}
///////////////////////////////////////////////
////if (checkBoxdiswinre.Checked)
////{
////    File.Copy(Application.StartupPath + "\\files\\unattend.xml", ud + "Windows\\System32\\sysprep\\unattend.xml");
////}
/////////////////////////////////////////////
//#endregion
//#region DetachVHD
////FileOperation.DeleteFile(diskpartscriptpath + "\\detach.txt");
/////////////////////detach vdisk/////////////////////
////using (FileStream fs1 = new FileStream(diskpartscriptpath + "\\detach.txt", FileMode.Create, FileAccess.Write))
////{
////    fs1.SetLength(0);
////    using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default))
////    {
////        sw1.WriteLine("select vdisk file=" + vhdPath);
////        sw1.WriteLine("detach vdisk");
////    }
////}
////ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\detach.txt\"");
//#endregion
////private void NonUEFIVHDFinish()
////  {

////      if (!System.IO.File.Exists(WTGOperation.ud + WTGOperation.win8VhdFile))
////      {
////          //MsgManager.getResString("Msg_VHDCreationError")
////          //Win8 VHD文件不存在！未知错误原因！
////          ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
////          er.ShowDialog();
////          //MessageBox.Show("Win8 VHD文件不存在！可到论坛发帖求助！\n建议将logs文件夹打包上传！");
////          //System.Diagnostics.Process.Start("http://bbs.luobotou.org/forum-88-1.html");                
////      }

////      else if (!System.IO.File.Exists(WTGOperation.ud + "\\Boot\\BCD"))
////      {
////          //MsgManager.getResString("Msg_VHDBcdbootError")
////          //VHD模式下BCDBOOT执行出错！
////          ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDBcdbootError", MsgManager.ci));
////          er.ShowDialog();

////          //MessageBox.Show("VHD模式下BCDBOOT执行出错！\nboot文件夹不存在\n请看论坛教程！","出错啦",MessageBoxButtons .OK ,MessageBoxIcon.Error );
////          //System.Diagnostics.Process.Start("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=8561");
////      }
////      else if (!System.IO.File.Exists(WTGOperation.ud + "bootmgr"))
////      {
////          ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_bootmgrError", MsgManager.ci));
////          er.ShowDialog();

////          //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
////      }
////      else
////      {
////          Finish f = new Finish();
////          f.ShowDialog();
////      }
////  }


////private void Fixletter(string targetletter, string currentos) 
////{
////    try
////    {
////        byte[] registData;
////        RegistryKey hkml = Registry.LocalMachine;
////        RegistryKey software = hkml.OpenSubKey("SYSTEM", false);
////        RegistryKey aimdir = software.OpenSubKey("MountedDevices", false);
////        registData = (byte[])aimdir.GetValue("\\DosDevices\\" + currentos);
////        if (registData != null)
////        {
////            ProcessManager.SyncCMD("reg.exe load HKU\\TEMP " + currentos + "\\Windows\\System32\\Config\\SYSTEM  > \"" + Application.StartupPath + "\\logs\\loadreg.log\"");
////            RegistryKey hklm = Registry.Users;
////            RegistryKey temp = hklm.OpenSubKey("TEMP", true);
////            try
////            {
////                temp.DeleteSubKey("MountedDevices");
////            }
////            catch { }
////            RegistryKey wtgreg = temp.CreateSubKey("MountedDevices");
////            wtgreg.SetValue("\\DosDevices\\" + targetletter, registData, RegistryValueKind.Binary);
////            wtgreg.Close();
////            temp.Close();
////            ProcessManager.SyncCMD("reg.exe unload HKU\\TEMP > \"" + Application.StartupPath + "\\logs\\unloadreg.log\"");



////            //string code = ToHexString(registData);
////            ////for (int i = 0; i < registData.Length; i++) 
////            ////{
////            ////    code += ToHexString(registData);
////            ////}
////            //MessageBox.Show(code);

////        }
////    }
////    catch (Exception ex) { MessageBox.Show(ex.ToString()); }
////}

////private void ForeachDisk(string path) 
////{
////    DirectoryInfo dir = new DirectoryInfo(path);
////    try
////    {
////        foreach(FileInfo  d in dir.GetFiles())
////        {
////            MessageBox.Show(d.FullName);
////        }

////    }
////    catch { }
////}

////public void Win7REG(string installdrive) 
////{
////    //installdriver :ud  such as e:\
////    try
////    {
////        ProcessManager.ECMD("reg.exe", " load HKLM\\sys " + installdrive + "WINDOWS\\system32\\config\\system");
////        ProcessManager.ECMD("reg.exe", " import " + Application.StartupPath + "\\files\\usb.reg");
////        ProcessManager.ECMD("reg.exe", " unload HKLM\\sys");
////        Fixletter("C:", installdrive);
////        //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:" + ud.Substring(0, 1) + " > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
////    }
////    catch(Exception err) 
////    {
////        //MsgManager.getResString("Msg_win7usberror")
////        //处理WIN7 USB启动时出现问题
////        MessageBox.Show(MsgManager.getResString("Msg_win7usberror", MsgManager.ci) + err.ToString());
////    }
////}
////        #region not used USBDRIVER
////        //public void USBDrive()
////        //{
////        //    WindowsImageContainer image1 = new WindowsImageContainer("h:\\sources\\install.wim", WindowsImageContainer.CreateFileMode.OpenExisting, WindowsImageContainer.CreateFileAccess.Read);

////        //    manager = new UsbManager();
////        //    UsbDiskCollection disks = manager.GetAvailableDisks();
////        //    foreach (UsbDisk disk in disks)
////        //    {
////        //        MessageBox.Show(disk.ToString());
////        //        //textBox.AppendText(disk.ToString() + CR);
////        //    }
////        //    //manager.StateChanged += new UsbStateChangedEventHandler(DoStateChanged);

////        //}
////        private void DoStateChanged(UsbStateChangedEventArgs e)
////        {
////            MessageBox.Show(e.State.ToString ());

////            //foreach (UsbDisk disk in disks)
////            //{
////            //    MessageBox.Show(disk.ToString());
////            //    //textBox.AppendText(disk.ToString() + CR);
////            //}

////            //textBox.AppendText(e.State + " " + e.Disk.ToString() + CR);
////        }

////        public static string GetDriveInfoDetail(string driveName)
////        {
////            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_DiskDrive  WHERE Name = '{0}'", driveName.Substring(0, 2)));

////            ManagementObjectSearcher managerSearch = new ManagementObjectSearcher(wqlObjectQuery);

////            List<ulong> driveInfoList = new List<ulong>(2);

////            ManagementClass mc = new ManagementClass("Win32_DiskDrive");
////            ManagementObjectCollection moc = mc.GetInstances();

////            foreach (ManagementObject mobj in moc)
////            {
////                MessageBox.Show(mobj["DeviceID"].ToString());


////                return (mobj["Index"].ToString());
////                //Console.WriteLine("File system: " + mobj["FileSystem"]);

////                //Console.WriteLine("Free disk space: " + mobj["FreeSpace"]);

////                //Console.WriteLine("Size: " + mobj["Size"]);
////            }
////            return "ERROR";
////        }
////        public static string GetDriveWin32_DiskPartition(string driveName)
////        {
////            //MessageBox.Show(GetDriveInfoDetail(driveName));
////            //WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_PhysicalMedia   WHERE Name = '{0}'", GetDriveInfoDetail(driveName)));
////            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_DiskPartition   "));
////            ManagementObjectSearcher managerSearch = new ManagementObjectSearcher(wqlObjectQuery);

////            List<ulong> driveInfoList = new List<ulong>(2);

////            ManagementClass mc = new ManagementClass("Win32_DiskPartition");
////            ManagementObjectCollection moc = mc.GetInstances();
////            foreach (ManagementObject mobj in moc)
////            {
////                //MessageBox.Show("");
////                MessageBox.Show(mobj["Name"].ToString());
////                //MessageBox.Show("");

////                //return (mobj["Model"].ToString());

////                //Console.WriteLine("File system: " + mobj["FileSystem"]);

////                //Console.WriteLine("Free disk space: " + mobj["FreeSpace"]);

////                //Console.WriteLine("Size: " + mobj["Size"]);
////            }
////            return "ERROR";
////        }
////        public void Testdrive() 
////        {

////foreach(ManagementObject drive in new ManagementObjectSearcher(
////    "select * from Win32_DiskDrive where InterfaceType='USB'").Get())
////{
////    // assoMsgManager.ciate physical disks with partitions

////    foreach(ManagementObject partition in new ManagementObjectSearcher(
////        "ASSOMsgManager.ciATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"]+ "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
////    {
////        Console.WriteLine("Partition=" + partition["Name"]);

////        // assoMsgManager.ciate partitions with logical disks (drive letter volumes)

////        foreach(ManagementObject disk in new ManagementObjectSearcher(
////            "ASSOMsgManager.ciATORS OF {Win32_DiskPartition.DeviceID='"+ partition["DeviceID"]+ "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
////        {
////            //MessageBox.Show(disk["Name"].ToString ());
////            Console.WriteLine("Disk=" + disk["Name"]);
////        }
////    }

////    // this may display nothing if the physical disk

////    // does not have a hardware serial number

////    MessageBox.Show ("Serial="+ new ManagementObject("Win32_PhysicalMedia.Tag='"+ drive["DeviceID"] + "'")["SerialNumber"]);
////}

////        }
////        public static string GetDriveWin32_LogicalDisk(string driveName)
////        {
////            //MessageBox.Show(GetDriveInfoDetail(driveName));
////            //WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_PhysicalMedia   WHERE Name = '{0}'", GetDriveInfoDetail(driveName)));
////            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_LogicalDiskToPartition     "));
////            ManagementObjectSearcher managerSearch = new ManagementObjectSearcher(wqlObjectQuery);

////            List<ulong> driveInfoList = new List<ulong>(2);

////            ManagementClass mc = new ManagementClass("Win32_LogicalDiskToPartition");
////            ManagementObjectCollection moc = mc.GetInstances();
////            foreach (ManagementObject mobj in moc)
////            {
////                //MessageBox.Show("");
////                MessageBox.Show(mobj["Dependent"].ToString());
////                MessageBox.Show(mobj["Antecedent"].ToString());


////                //MessageBox.Show("");

////                //return (mobj["Model"].ToString());

////                //Console.WriteLine("File system: " + mobj["FileSystem"]);

////                //Console.WriteLine("Free disk space: " + mobj["FreeSpace"]);

////                //Console.WriteLine("Size: " + mobj["Size"]);
////            }
////            return "ERROR";
////        }
////        #endregion
////public int  Iswin7(string wimfile) 
////{
////    ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\"+imagex+"\"" + " /info \"" + wimfile + "\" /xml > " + "\""+Application.StartupPath + "\\logs\\wiminfo.xml\"");
////    XmlDocument xml = new XmlDocument();

////   System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
////    System.Xml.XmlNodeReader reader = null;
////    string strFilename = Application.StartupPath + "\\logs\\wiminfo.xml";
////    if (System.IO.File.Exists(strFilename) == false)
////    {
////        //MsgManager.getResString("Msg_wiminfoerror")
////        //WIM文件信息获取失败\n将按WIN8系统安装
////        MessageBox.Show(this, strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci), this.Text);
////        return 0;
////    }
////    try
////    {
////        doc.Load(strFilename);
////        reader = new System.Xml.XmlNodeReader(doc);
////        while (reader.Read())
////        {
////            if (reader.IsStartElement("NAME"
////                ))
////            {

////                //从找到的这个依次往下读取节点
////                System.Xml.XmlNode aa = doc.ReadNode(reader);
////                //MessageBox.Show(aa.InnerText);
////                //MessageBox.Show(aa.InnerText);
////                if (aa.InnerText == "Windows 7 STARTER")
////                {

////                    return 1;
////                    //break;
////                }
////                else if (aa.InnerText == "Windows 7 HOMEBASIC")
////                {
////                    //MessageBox.Show(aa.InnerText); 
////                    return 2;
////                }

////                else { return 0; }


////            }
////        }
////    }
////    catch (Exception  ex)
////    {
////        MessageBox.Show(this, strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci) + ex.ToString(), this.Text);
////        return 0;
////    }



////    return 0;
////}

////public string ReadSel(ArrayList text)
////{
////    //object ddd=new object[];
////    if (comboBox1.InvokeRequired)
////    {
////        OutDelegate outdelegate = new OutDelegate(ReadSel);
////        this.BeginInvoke(outdelegate, new object[] { text });
////        return 0;
////    }
////    //comboBox1.Items.Clear();
////    comboBox1.DataSource = null;
////    comboBox1.DataSource = text;
////    if (comboBox1.Items.Count != 0)
////    {
////        comboBox1.SelectedIndex = 0;
////    }

////    //comboBox1.AppendText("\t\n");
////}

//#region OLDCODE
////ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, WTGOperation.ud + "windows  /s  x: /f UEFI");

////ImageOperation.AutoChooseWimIndex(ref wimpart, win7togo);
////判断是否WIN7，自动选择安装分卷
////int win7togo = iswin7(win8iso);
////if (wimpart==0)
////{//自动判断模式

////    if (win7togo == 1)
////    {//WIN7 32 bit

////        wimpart = 5;
////    }
////    else if (win7togo == 2)
////    { //WIN7 64 BIT

////        wimpart = 4;
////    }
////    else { wimpart = 1; }
////}
////IMAGEX解压
////io.ImageApplyToUD (checkBoxwimboot.Checked,isesd,)
////ImageOperation.ImageApply(checkBoxwimboot.Checked, isesd, imagex, win8iso, wimpart, WTGOperation.ud);
////if (checkBoxwimboot.Checked)
////{
////    ImageOperation.WimbootApply(win8iso, ud, wimpart);
////    //ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + win8iso + "\" /SourceIndex:" + wimpart.ToString() + " /DestinationImageFile:" + ud + "wimboot.wim");
////    //
////    //ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + ud + "wimboot.wim" + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString() + " /WIMBoot");
////    //

////}
////else
////{
////    //dism /apply-image /imagefile:9600.17050.winblue_refresh.140317-1640_x64fre_client_Professional_zh-cn-ir3_cpra_x64frer_zh-cn_esd.esd /index:4 /applydir:G:\

////    if (isesd) 
////    {
////        ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString());
////        

////    }
////    else
////    {
////        ProcessManager.ECMD(Application.StartupPath + "\\files\\"+imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + ud);
////        
////    }
////}
////安装EXTRA
////if (checkBoxframework.Checked)
////{
////    ProcessManager.ECMD("dism.exe"," /image:" + ud.Substring(0, 2) + " /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
////    

////}
////if (checkBox_san_policy.Checked)//屏蔽本机硬盘
////{
////    ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
////    

////}

////if (checkBoxdiswinre.Checked)
////{

////    File.Copy(Application.StartupPath + "\\files\\unattend.xml", ud + "Windows\\System32\\sysprep\\unattend.xml");
////}
////BCDBOOT WRITE BOOT FILE    

//#endregion


////RegistryKey key = Registry.LocalMachine;
////RegistryKey sys = key.OpenSubKey("sys", true);
////RegistryKey CS001 = null;
////foreach (var item in sys.GetSubKeyNames())
////{
////    if (item == "ControlSet001")
////    {
////        CS001 = key.OpenSubKey("CS001"); break;
////    }
////}
////if (CS001 == null)
////{
////    CS001 = sys.CreateSubKey("ControlSet001");
////}
////Register reg = new Register("sys", RegDomain.LocalMachine);
////reg.SubKey = "ControlSet001";
//////reg.SubKey = "sys";
//////reg.RegeditKey = "";
//////reg.
////if (!reg.IsSubKeyExist())
////{
////    reg.CreateSubKey();
////}
//////reg.RegeditKey = "BootDriverFlags";
////reg.WriteRegeditKey("BootDriverFlags", "00000006", RegValueKind.DWord);
//////reg.SubKey 
////if (RegExportImport.ImportReg(Application.StartupPath + "\\files\\usb.reg", null) == 0) 
////{
////    throw new Exception("导入注册表出错！！！！！");
////}

////if (sys.GetSubKeyNames())

////ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:" + ud.Substring(0, 1) + " > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");



////Console.WriteLine("reg.exe load HKU\\sys " + installdrive + "Windows\\System32\\Config\\SYSTEM  > \"" + Application.StartupPath + "\\logs\\Win7REGLoad.log\"");
////Console.WriteLine("reg.exe import \"" + Application.StartupPath + "\\files\\usb.reg\" >nul &if %errorlevel% ==0 (echo 注册表导入成功) else (echo 注册表导入失败)" + " > \"" + Application.StartupPath + "\\logs\\Win7REGImport.log\"");
////ProcessManager.ECMD("reg.exe", " load HKLM\\sys " + installdrive + "WINDOWS\\system32\\config\\system");



////RegistryKey key = Registry.LocalMachine;
////RegistryKey software = key.
////installdriver :ud  such as e:\



////needcopy = false;
////WTGOperation.wimpart = ChoosePart.part;

//////win7////
////int win7togo = iswin7(win8iso);
////if (win7togo != 0 && radiovhdx.Checked) { MessageBox.Show("WIN7 WTG系统不支持VHDX模式！"); return; }
////if (wimpart == 0)
////{//自动判断模式

////    if (win7togo == 1)
////    {//WIN7 32 bit

////        wimpart = 5;
////    }
////    else if (win7togo == 2)
////    { //WIN7 64 BIT

////        wimpart = 4;
////    }
////    else { wimpart = 1; }
////}
////MessageBox.Show(wimpart.ToString());
////////////////


//#region REG operation
////private bool IsRegeditExit(string name)
////{
////    bool _exit = false;
////    string[] subkeyNames;
////    RegistryKey hkml = Registry.CurrentUser;
////    RegistryKey software = hkml.OpenSubKey("software", true);
////    subkeyNames = software.GetSubKeyNames();
////    foreach (string keyName in subkeyNames)
////    {
////        if (keyName == name)
////        {
////            _exit = true;
////            return _exit;
////        }
////    }
////    return _exit;
////}
////private string GetRegistData(string name)
////{
////    string registData;
////    RegistryKey hkml = Registry.CurrentUser;
////    RegistryKey software = hkml.OpenSubKey("software", true);
////    RegistryKey aimdir = software.OpenSubKey(Application.ProductName, true);
////    registData = aimdir.GetValue(name).ToString();
////    return registData;
////}
////private void WTRegedit(string name, string tovalue)
////{
////    RegistryKey hklm = Registry.CurrentUser;
////    RegistryKey software = hklm.OpenSubKey("SOFTWARE", true);
////    RegistryKey aimdir = software.CreateSubKey(Application.ProductName);
////    aimdir.SetValue(name, tovalue);
////}
//#endregion


//#region UnUsed
////public static string adlink;//公告
////public static string adtitles;
////public static string win8vhdfile = "win8.vhd";
////public static string vpath;//源VHD文件完整路径
////private  string[] topicname = new string[10];
////private  string[] topiclink = new string[10];
////string imagex = "imagex_x86.exe";
////WTGOperation wtgo = new WTGOperation();
////string msg = Properties.Resources.InfoMsg;
////string vhd_size = "";//VHD 文件尺寸
////string ud;//优盘盘符
////string bcdboot;//bcdboot文件名
////public static string filetype;//后缀名
////string win8iso;//选择的WIM
////int wimpart;//WIM分卷
////int force=0;//强制格式化
////bool usetemp = true;//使用临时文件夹
////Thread copyfile;
////bool needcopyvhdbootfile = false;
////WriteProgress wp;
////delegate void set_TextDelegate(string s); //定义委托
////set_TextDelegate Set_Text;
////Thread threadad;
////bool isesd = false;
////int win7togo;
////string udiskstring;
////string diskpartscriptpath = Application.StartupPath + "\\logs";

//#endregion


////private void cleanvhdtemp() 
////{
////    /////////////////////删除临时文件////////////////////
////    cleantemp();
////}

////System.Diagnostics.Process KILL = System.Diagnostics.Process.Start("cmd.exe", "/c taskkill /f /IM VD.exe");
////KILL.WaitForExit();
////MessageBox.Show(DiskOperation.GetHardDiskFreeSpace(SetTempPath.temppath.Substring(0, 3)).ToString());
////MsgManager.getResString("Msg_WriteProcessing")

////ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
////Operation o = new Operation();
////o.MyProperty = ud;


////private void radioButton1_CheckedChanged(object sender, EventArgs e)
////{
////    //if (radiovhdx.Checked) { WTGOperation.win8VhdFile = "win8.vhdx"; }
////    //else { WTGOperation.win8VhdFile = "win8.vhd"; }
////    //checkBoxuefi.Enabled = !radiovhdx.Checked;
////}

////private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
////{
////    //ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
////}

////private void radiovhd_CheckedChanged(object sender, EventArgs e)
////{
////    //if (radiovhdx.Checked) { WTGOperation.win8VhdFile = "win8.vhdx"; }
////    //else { WTGOperation.win8VhdFile = "win8.vhd"; }
////    //checkBoxuefi.Enabled = !radiovhd.Checked;
////}

////private void radiochuantong_CheckedChanged(object sender, EventArgs e)
////{
////    //if (radiovhdx.Checked) { WTGOperation.win8VhdFile = "win8.vhdx"; }
////    //else { WTGOperation.win8VhdFile = "win8.vhd"; }

////}


////private void button2_Click_1(object sender, EventArgs e)
////{
////    if (button2.Text == ">")
////    {
////        formwide = this.Width;
////        this.Width = (int)((double)this.Width / 0.675);
////        //MessageBox.Show((this.Width * 100 / 66).ToString());
////        button2.Text = "<";
////    }
////    else
////    {
////        this.Width = formwide;
////        //MessageBox.Show((this.Width * 66 / 100).ToString());

////        button2.Text = ">";

////    }
////}


//#region vHDUEFIBCDToolStripMenuItem_Click
////MessageBox.Show("/store X:\\efi\\microsoft\\boot\\bcd /set {92382214-91cb-4c08-bed7-5c48c55d46bc} device vhd=[" + ud.Substring(0, 2) + "]\\" + win8vhdfile);
////if (File.Exists(@"C:\Windows\WinSxS\amd64_microsoft-windows-b..iondata-cmdlinetool_31bf3856ad364e35_6.3.9600.16384_none_78e95cd07922a6bf\\bcdedit.exe")) { MessageBox.Show("存在"); } else { MessageBox.Show("不存在！"); }

////System.Diagnostics.Process cv = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\" + "\\bcdedit.exe", " /store X:\\efi\\microsoft\\boot\\bcd /set {92382214-91cb-4c08-bed7-5c48c55d46bc} device vhd=[" + ud.Substring(0, 2) + "]\\" + win8vhdfile);
////    cv.WaitForExit();
////    System.Diagnostics.Process cv1 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\" + "\\bcdedit.exe", " /store X:\\efi\\microsoft\\boot\\bcd /set {92382214-91cb-4c08-bed7-5c48c55d46bc} osdevice vhd=[" + ud.Substring(0, 2) + "]\\" + win8vhdfile);
////    cv1.WaitForExit();



////ProcessManager.ECMD("bcdedit /store X:\\efi\\microsoft\\boot\\bcd /set {92382214-91cb-4c08-bed7-5c48c55d46bc} device vhd=[" + ud.Substring(0, 2) + "]\\" + win8vhdfile);
////ProcessManager.ECMD("bcdedit /store X:\\efi\\microsoft\\boot\\bcd /set {92382214-91cb-4c08-bed7-5c48c55d46bc} osdevice vhd=[" + ud.Substring(0, 2) + "]\\" + win8vhdfile);
//#endregion


//#region imagex解压写入ToolStripMenuItem_Click
////wimpart = ChoosePart.part;
////if (wimpart == 0)
////{//自动判断模式
////    win7togo = Iswin7(win8iso);

////    if (win7togo == 1)
////    {//WIN7 32 bit

////        wimpart = 5;
////    }
////    else if (win7togo == 2)
////    { //WIN7 64 BIT

////        wimpart = 4;
////    }
////    else { wimpart = 1; }
////}
////MsgManager.getResString("Msg_FormatTip")
//#endregion


////private void 不格式化磁盘ToolStripMenuItem_Click(object sender, EventArgs e)
////{
////    //isformat = !isformat ;
////}


////private string Distinguish64or32System()
////{
////    try
////    {
////        string addressWidth = String.Empty;
////        ConnectionOptions mConnOption = new ConnectionOptions();
////        ManagementScope mMs = new ManagementScope("//localhost", mConnOption);
////        ObjectQuery mQuery = new ObjectQuery("select AddressWidth from Win32_Processor");
////        ManagementObjectSearcher mSearcher = new ManagementObjectSearcher(mMs, mQuery);
////        ManagementObjectCollection mObjectCollection = mSearcher.Get();
////        foreach (ManagementObject mObject in mObjectCollection)
////        {
////            addressWidth = mObject["AddressWidth"].ToString();
////        }
////        return addressWidth;
////    }
////    catch (Exception ex)
////    {
////        Log.WriteLog("Distinguish64or32System.log", ex.ToString());
////        return String.Empty;
////    }
////}

////MessageBox.Show("\"" + Application.StartupPath + "\\files\\" + bcdboot + "\"  " + ud.Substring(0, 3) + "windows  /s  " + ud.Substring(0, 2));

////ProcessManager.SyncCMD("\"" + Application.StartupPath + "\\files\\" + bcdboot + "\"  " + ud.Substring(0, 3) + "windows  /s  " + ud.Substring(0, 2)+" /f ALL");

////System.Diagnostics.Process p1 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\" + bcdboot, "  " + ud.Substring(0, 3) + "windows  /s  " + ud.Substring(0, 2));
////p1.WaitForExit();


////VHDOperation.CleanTemp();
////vo.CreateVHD();
////vo.VHDExtra();
////vo.DetachVHD();
////vo.CopyVHD();
////if (!checkBoxfixed.Checked)
////{
////    vo.VHDDynamicSizeIns(WTGOperation.ud);
////}
////if (checkBoxuefimbr.Checked)
////{
////    //BootFileOperation.BooticeWriteMBRPBRAndAct("X:");
////    //System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", (" /DEVICE=X: /pbr /install /type=bootmgr /quiet"));//写入引导
////    //pbr.WaitForExit();
////    //System.Diagnostics.Process act = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\bootice.exe", " /DEVICE=X: /partitions /activate /quiet");
////    //act.WaitForExit();
////}



////VHDOperation.CleanTemp();
////vo.CreateVHD();
////vo.VHDExtra();
////vo.WriteBootFiles();
////vo.DetachVHD();
////vo.CopyVHD();

////if (!checkBoxfixed.Checked)
////{
////    vo.VHDDynamicSizeIns(WTGOperation.ud);

////}
////NonUEFIVHDFinish();

////Copy(System.Environment.GetEnvironmentVariable("TEMP") + "\\" + win8vhdfile, ud);
////copy cp = new copy(ud);
////cp.ShowDialog();

////Copy(System.Environment.GetEnvironmentVariable("TEMP") + "\\" + win8vhdfile, ud + win8vhdfile);



////private void UnZipFiles()
////{
////    UnZipClass.UnZip(Application.StartupPath + "\\files.dat", StringOperation.Combine(Path.GetTempPath(), "WTGA"));
////}

////private void SerFiles()
////{
////    using (FileStream fsRead = File.OpenRead(@"E:\MyProgram\wintogo\wintogo\bin\Release\files\imagex_x64.exe"))
////    {



////            //byte[] byts = new byte[buffersize * 1024 * 1024];
////            //int r = 0;
////            //while ((r = fsRead.Read(byts, 0, byts.Length)) > 0)
////            //{
////            //    fsWrite.Write(byts, 0, r);
////            //    //Console.WriteLine(fsWrite.Position / (double)fsRead.Length * 100);
////            //    //r = fsRead.Read(byts, 0, byts.Length);
////            //}


////}
////using (FileStream fs = new FileStream(@"E:\MyProgram\wintogo\wintogo\bin\Release\files\imagex_x64.exe", FileMode.Open))
////{
////    byte[] byts = new byte[fs.Length];
////    fs.Read(byts, 0, byts.Length);
////    BinaryFormatter bf = new BinaryFormatter();
////    using (FileStream fsWrite =new FileStream(Application.StartupPath + @"\a.bin", FileMode.Create))
////    {
////        bf.Serialize(fsWrite, byts);
////    }


////}
////using (FileStream fs = new FileStream(Application.StartupPath + @"\a.bin", FileMode.Open))
////{
////    BinaryFormatter bf = new BinaryFormatter();
////    byte[] byts = bf.Deserialize(fs) as byte[];
////    using (FileStream fsWrite = new FileStream(Path.GetTempPath() + "\\a.exe", FileMode.Create))
////    {
////        fsWrite.Write(byts, 0, byts.Length);
////    }

////}
////}

////MessageBox.Show(WTGOperation.diskpartscriptpath);
////FileOperation.CleanLockStream(MsgManager.getResString("Msg_ntfsstream", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci));
////this.Width = (int)((double)this.Width * 0.675);
////MessageBox.Show(StringOperation.IsChina(@"C:\Users\Sugar-前妻\AppData\Local\Temp\").ToString ());


//#region OldCode
////using (StreamReader sr=new StreamReader (@"c:\My\b.txt"))
////{
////    string fileStr=sr.ReadToEnd();
////    MatchCollection mc = Regex.Matches(fileStr, @"Index : ([0-9]+)\nName : [\^]");
////    MessageBox.Show(mc[1].Value);

////}
////string fileStr=File.ReadAllText(@"c:\b.txt");
////UnZipFiles();
////SerFiles();

////WTGOperation.ud = "100";
////vo.WinbootImagex();
//#endregion
////Stopwatch sw = new Stopwatch();
////sw.Start();


////set_TextDelegate Set_Text = new set_TextDelegate(set_textboxText); ;

////Set_Text  //实例化
////sw.Stop();
////MessageBox.Show(sw.Elapsed.ToString ());

////private UsbManager manager;


////int formwide = 623;
////bool allowesd = false;//可使用ESD文件

////private string efisize = "350";



////VHDOperation.CleanTemp();
////vo.CreateVHD();
////vo.VHDExtra();
////vo.DetachVHD();
////vo.CopyVHD();
////if (!checkBoxfixed.Checked)
////{
////    vo.VHDDynamicSizeIns(WTGOperation.ud);
////}


//////finish f = new finish();
//////f.ShowDialog();


////private void LoadPlugins()
////{

////    string pluginPath = StringOperation.Combine(Application.StartupPath, "plugins");
////    //MessageBox.Show(pluginPath);
////    foreach (var item in Directory.GetFiles(pluginPath, "*.dll"))
////    {
////        try
////        {
////            Assembly asm = Assembly.LoadFile(item);
////            Type[] types = asm.GetExportedTypes();
////            Type typeIPlugin = typeof(InterfacePlugin);
////            foreach (var typeItem in types)
////            {
////                if (typeIPlugin.IsAssignableFrom(typeItem) && !typeItem.IsAbstract)
////                {
////                    InterfacePlugin iPlugin = (InterfacePlugin)Activator.CreateInstance(typeItem);
////                    ToolStripItem tsItem = 工具ToolStripMenuItem.DropDownItems.Add(iPlugin.PluginName);
////                    工具ToolStripMenuItem.Tag = iPlugin;
////                    tsItem.Click += tsItem_Click;
////                }
////            }
////        }
////        catch (Exception ex)
////        {
////            //MessageBox.Show("Test");
////            Log.WriteLog("LoadPlugins.log", ex.ToString());
////        }
////    }

////}

////void tsItem_Click(object sender, EventArgs e)
////{
////    InterfacePlugin iPlugin = 工具ToolStripMenuItem.Tag as InterfacePlugin;
////    if (iPlugin != null)
////    {
////        iPlugin.Execute();
////    }
////}

////UDList.Clear();
//////MsgManager.getResString("Msg_chooseud")
//////请选择可移动设备
////UDList.Add(MsgManager.getResString("Msg_chooseud", MsgManager.ci));

////foreach (UsbDisk disk in disks)
////{

////    UDList.Add(disk.ToString());
////    //textBox.AppendText(disk.ToString() + CR);
////}

////List<string> list = new List<string>() { "a","b"};
////List<string> listb = new List<string>() { "a", "b" };
////if (list==listb)
////{
////    MessageBox.Show("Test");
////}

////UsbManager um = new UsbManager();
//#region OldCode
////ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
////if (DialogResult.No == MessageBox.Show("此操作将会清除移动磁盘所有分区的所有数据，确认？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
////if (DialogResult.No == MessageBox.Show("您确定要继续吗？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; } 

////FileStream fs0 = new FileStream(WTGOperation.diskpartscriptpath + "\\dp.txt", FileMode.Create, FileAccess.Write);
////fs0.SetLength(0);
////StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default);
////string ws0 = "";
////try
////{
////    ws0 = "select volume " + WTGOperation.ud.Substring(0, 1);
////    sw0.WriteLine(ws0);
////    ws0 = "clean";
////    sw0.WriteLine(ws0);
////    ws0 = "convert mbr";
////    sw0.WriteLine(ws0);
////    ws0 = "create partition primary";
////    sw0.WriteLine(ws0);
////    ws0 = "select partition 1";
////    sw0.WriteLine(ws0);
////    ws0 = "format fs=ntfs quick";
////    sw0.WriteLine(ws0);
////    ws0 = "active";
////    sw0.WriteLine(ws0);
////    ws0 = "assign letter=" + WTGOperation.ud.Substring(0, 1);
////    sw0.WriteLine(ws0);
////    ws0 = "exit";
////    sw0.WriteLine(ws0);
////}
////catch { }
////sw0.Close();

////ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartscriptpath + "\\dp.txt\"");

////System.Diagnostics.Process dpc = System.Diagnostics.Process.Start("diskpart.exe", " /s " + Application.StartupPath + "\\dp.txt");
////dpc.WaitForExit();
//#endregion

////List<string> sw_filelist = new List<string>();

//////string[] sw_fl = new string[13]; ;//software filelist
//////ArrayList sw_filelist = new ArrayList();
////sw_filelist.Add("\\files.dat");
//////sw_filelist.Add("\\files\\san_policy.xml");
//////sw_filelist.Add("\\files\\imagex_x86.exe");
//////sw_filelist.Add("\\files\\fbinst.exe");
//////sw_filelist.Add("\\files\\FastCopy.exe");
//////sw_filelist.Add("\\files\\bootsect.exe");
//////sw_filelist.Add("\\files\\BOOTICE.EXE");
//////sw_filelist.Add("\\files\\bcdboot.exe");
//////sw_filelist.Add("\\files\\bcdboot7601.exe");
//////sw_filelist.Add("\\files\\imagex_x64.exe");
//////sw_filelist.Add("\\files\\usb.reg");
//////sw_filelist.Add("\\files\\settings.ini");

////for (int i = 1; i < sw_filelist.Count; i++)
////{
////    if (!File.Exists(Application.StartupPath + sw_filelist[i]))
////    {
////        //MsgManager.getResString("Msg_filelist")

////        MessageBox.Show(MsgManager.getResString("Msg_filelist", MsgManager.ci) + Application.StartupPath + sw_filelist[i], MsgManager.getResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
////        VisitWeb("http://bbs.luobotou.org/thread-761-1-1.html");

////        Environment.Exit(0);
////        break;
////    }
////}

////LoadPlugins();
////wp = new WriteProgress();
////CheckForIllegalCrossThreadCalls = false;

////MessageBox.Show("Test");
////label4.Visible = true;

////List<string> UDList = new List<string>();


////ImageOperation io = new ImageOperation();
////io.imageFile = WTGOperation.path;
////io.imageX = WTGOperation.imagex;
////io.AutoChooseWimIndex();
////ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
////MessageBox.Show(WTGOperation.wimpart);
////MessageBox.Show(WTGOperation.wimpart);


////if (!this.NeedCopy)
////{
////    //this.NeedCopy = false;
////    //usetemp = false;

////    //this.VhdPath = WTGOperation.ud + this.VhdFileName;
////}
////else
////{
////    //SetTempPath.temppath + "\\" + win8vhdfile;
////    //needcopy = true;
////}


////AppendText("复制文件中...大约需要10分钟~1小时，请耐心等待！");
////wp.Show();
////Application.DoEvents();
////System.Diagnostics.Process cp = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\fastcopy.exe", " /auto_close \"" + Form1.vpath + "\" /to=\"" + ud + "\"");
////cp.WaitForExit();
////wp.Close();



//////MessageBox.Show(adprogram.ToString() + " " + startindex);
////adtitle = pageHtml.Substring(adprogram, startindex - adprogram);
////adtitles = adtitle;
////adlink = pageHtml.Substring(startindex + 1, endindex - startindex - 1);
////Set_Text(adtitle);
////Set_Text = new set_TextDelegate(set_textboxText); //实例化
////MessageBox.Show("Test");
////Form1.SetText(adtitle);'



////linkLabel.Invoke(setLinkLabel, new object[] { adtitle });


////linkLabel2(Set_Text);
////MessageBox.Show("");
////writeprogress .linklabel1
////MessageBox.Show(adtitle + "     " + adlink);


////Byte[] pageData1 = MyWebClient1.DownloadData("http://bbs.luobotou.org/app/announcement.txt"); //从指定网站下载数据
////pageHtml = Encoding.UTF8.GetString(pageData1);
////MessageBox.Show(pageHtml);
////int index1 = pageHtml.IndexOf(Application.ProductName);
////int startindex = pageHtml.IndexOf("~", index1);
////int endindex = pageHtml.IndexOf("结束", index1);
////int adprogram = index1 + Application.ProductName.Length + 1;

////Set_Text(match.Groups[1].Value);


//#region OldCode
////string portal_block = pageHtml.Substring;
////String adtitle;
//////MessageBox.Show(adprogram.ToString() + " " + startindex);
////adtitle = pageHtml.Substring(adprogram, startindex - adprogram);

////adlink = pageHtml.Substring(startindex + 1, endindex - startindex - 1);
////linkLabel2.Invoke(Set_Text, new object[] { adtitle });
////MessageBox.Show("");

////MessageBox.Show(adtitle + "     " + adlink);
//#endregion

////MessageBox.Show("Test");


////private Action<string> setLinkLabel;
////public Action<string> SetLinkLabel
////{
////    get { return setLinkLabel; }
////    set { setLinkLabel = value; }
////}
////private set_TextDelegate  set_Text;

////public set_TextDelegate  Set_text
////{
////    get { return set_Text; }
////    set { set_Text = value; }
////}


////public int MyProperty { get; set; }


///// <summary>
///// 结束进程
///// </summary>
///// 
////public static void ShowForm()
////{
////    Invoke(new MethodInvoker(Showd));

////}
////public static void End()
////{
////    Invoke(new MethodInvoker(DoEnd));

////}

////public static void DoEnd()
////{
////    wp.Close();
////}
////private void Showd()
////{
////    wp.ShowDialog();


////}
////        public static void RunDism(string StartFileArg)
////        {

////            string readtext = string.Empty;
////            Regex reg = new Regex(@"
////\=∗\s∗(\d1,3\.\d)
////");
////            Task task = new Task(() =>
////            {
////                Console.WriteLine("Start");
////                while (percentage < 99.9)
////                {
////                    Console.WriteLine("X");
////                    try
////                    {
////                        foreach (string line in DismWrapper.ReadFromBuffer(0, 5, (short)Console.BufferWidth, 1))
////                        {
////                            readtext = line;
////                        }
////                        //Console.Title = readtext;  
////                        if (reg.IsMatch(readtext))
////                        {
////                            percentage = double.Parse(reg.Match(readtext).Groups[1].Value);
////                            Console.WriteLine(percentage);

////                            //Console.Title = reg.Match(readtext).Groups[1].Value;
////                        }
////                    }
////                    catch(Exception ex)
////                    { Console.WriteLine(ex.ToString ()); }
////                }
////                //Console.WriteLine("Exit");
////            });
////            task.Start();


////            Process process = new Process();
////            //wp.ShowDialog();

////            try
////            {
////                AppendText("Command:DISM" + StartFileArg + "\r\n");
////                process.StartInfo.FileName = "dism.exe";
////                process.StartInfo.Arguments = StartFileArg;
////                process.StartInfo.UseShellExecute = false;
////                process.StartInfo.RedirectStandardInput = true;
////                process.StartInfo.RedirectStandardOutput = true;
////                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
////                //process.StartInfo.RedirectStandardError = true;
////                process.StartInfo.CreateNoWindow = false;
////                process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
////                process.EnableRaisingEvents = true;
////                process.Exited += new EventHandler(progress_Exited);

////                process.Start();


////                process.BeginOutputReadLine();


////            }
////            catch (Exception ex)
////            {
////                //MsgManager.getResString("Msg_Failure")
////                //操作失败
////                MessageBox.Show(MsgManager.getResString("Msg_Failure", MsgManager.ci) + ex.ToString());
////            }
////            wp.ShowDialog();

////            //CommandCaller _dismCaller = new CommandCaller(command);
////            //_dismCaller.Call(parameter);
//// 

////}



////private void set_textboxText(string s)
////{
////    this.linkLabel2.Text = s;

////    this.linkLabel2.Visible = true;
////}

////public static void CleanLockStream(string ErrorMsg, string ErrorTitle)
////{
////    ListFiles(new DirectoryInfo(Application.StartupPath + "\\files"), ErrorMsg, ErrorTitle);
////}
////public static void ListFiles(FileSystemInfo info, string ErrorMsg, string ErrorTitle)
////{
////    try
////    {
////        if (!info.Exists) return;
////        DirectoryInfo dir = info as DirectoryInfo;
////        //不是目录
////        if (dir == null) return;
////        FileSystemInfo[] files = dir.GetFileSystemInfos();
////        for (int i = 0; i < files.Length; i++)
////        {
////            FileInfo file = files[i] as FileInfo;
////            //是文件
////            if (file != null)
////            {
////                //FileInfo file = new FileInfo(@"d:\Hanye.chm");
////                //MessageBox.Show(file.FullName);
////                foreach (AlternateDataStreamInfo s in file.ListAlternateDataStreams())
////                {
////                    s.Delete();//删除流
////                }

////                //Console.WriteLine(file.FullName + "\t " + file.Length);
////                //if (file.FullName.Substring(file.FullName.LastIndexOf(".")) == ".jpg")
////                ////此处为显示JPG格式，不加IF可遍历所有格式的文件
////                //{
////                //    //this.list1.Items.Add(file);
////                //    //MessageBox.Show(file.FullName.Substring(file.FullName.LastIndexOf(".")));
////                //}
////            }
////            //对于子目录，进行递归调用
////            else
////            {
////                ListFiles(files[i], ErrorMsg, ErrorTitle);
////            }
////        }
////    }
////    catch (Exception ex)
////    {
////        //NTFS文件流异常\n请放心，此错误不影响正常使用
////        //GetResString("Msg_ntfsstream");
////        //MessageBox.Show(getResString("Msg_ntfsstream", ci) + ex.ToString(), getResString("Msg_warning", ci), MessageBoxButtons.OK, MessageBoxIcon.Information);
////        MessageBox.Show(ErrorMsg + ex.ToString(), ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
////        Log.WriteLog("CleatNtfsStream.log", ex.ToString());

////    }
////}
////public string Win8VHDFileName { get; set; }


////private bool ShouldContinue { get; set; }
////public int wimpart { get; set; }



////else
////{
////    //this.ShouldContinue = true;

////}
////}
////catch
////{
////    throw new VHDException(MsgManager.GetResString("Msg_VHDCreationError"));
////    //创建VHD失败，未知错误
////    //ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
////    //er.ShowDialog();
////    //this.ShouldContinue = false;
////    //shouldcontinue = false;

////}


////ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
////er.ShowDialog();
////this.ShouldContinue = false;

////shouldcontinue = false;
////return;


////needcopyvhdbootfile = true;
////copyvhdbootfile();
////WindowsImageContainer wic = new WindowsImageContainer("", WindowsImageContainer.CreateFileMode.OpenExisting, WindowsImageContainer.CreateFileAccess.Read);
////wic[0].Apply("");


////foreach (UsbDisk item in (UsbDiskCollection)dtSource)
////{
////    MessageBox.Show(item.ToString());
////}
////MessageBox.Show(((UsbDiskCollection)dtSource).ToString ());


////FormatAlert fa = new FormatAlert(
////    MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci)
////    +"\n"
////    + WTGModel.udString
////    +"\n"
////    //+ MsgManager.GetResString("Msg_Disk_Space", MsgManager.ci)
////    //+ DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1024 / 1024
////    + MsgManager.GetResString("Msg_FormatTip", MsgManager.ci));

//////if (DialogResult.No == fa.ShowDialog())
//////{
//////    return;
//////}


////MsgManager.getResString("Msg_DoWhat")
////如果您不清楚您在做什么，请立即停止操作！

////if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_DoWhat", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
////{
////    return;
////}


////MsgManager.getResString("Msg_FormatWarning")
////盘将会被格式化，此操作将不可恢复，您确定要继续吗？\n由于写入时间较长，请您耐心等待！\n写入过程中弹出写入可能无效属于正常现象，选是即可。
////if (DialogResult.No == MessageBox.Show(WTGModel.ud.Substring(0, 1) + MsgManager.GetResString("Msg_FormatWarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
////if (DialogResult.No == MessageBox.Show("如果您不清楚您在做什么，请立即停止操作！", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }


////ProcessManager.SyncCMD ("cmd.exe /c del /f /s /q \""+Application .StartupPath +"\\logs\\*.*\"");
////////////////将程序运行信息写入LOG


////using (FileStream fs0 = new FileStream(WTGOperation.diskpartScriptPath + "\\removex.txt", FileMode.Create, FileAccess.Write))
////{
////    fs0.SetLength(0);
////    using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default))
////    {
////        string ws0 = "";
////        ws0 = "select volume x";
////        sw0.WriteLine(ws0);
////        ws0 = "remove";
////        sw0.WriteLine(ws0);
////        ws0 = "exit";
////        sw0.WriteLine(ws0);
////    }
////}
