using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using wintogo.Forms;

namespace wintogo
{
    public static class CreateMain
    {
        private static Stopwatch writeSw = new Stopwatch();

        public static void GoWrite()
        {

            try
            {

                //wimpart = ChoosePart.part;//读取选择分卷，默认选择第一分卷
                #region 各种提示


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


                //GB 是将要写入的优盘或移动硬盘\n误格式化，后果自负！
                StringBuilder formatTip = new StringBuilder();
                formatTip.AppendLine(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci));
                formatTip.AppendFormat(WTGModel.udString);
                formatTip.AppendLine(MsgManager.GetResString("Msg_FormatTip", MsgManager.ci));
                if (WTGModel.rePartition)//勾选重新分区提示
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

                //ProcessManager.Do(() => {

                //});
                //ProcessManager.AppendText("Doing Benchmark!");
                

                Log.WriteProgramRunInfoToLog();

                writeSw.Restart();


                if (WTGModel.isUefiGpt)
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
                        if (WTGModel.CheckedMode==ApplyMode.Legacy)
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
                else if (WTGModel.isUefiMbr)
                {
                    //UEFI+MBR
                    if (WTGModel.udString.Contains("Removable Disk"))
                    {
                        WTGModel.isLegacyUdiskUefi = true;
                        Write.RemoveableDiskUefiMbr();
                        FinishSuccessful();

                    }
                    else
                    {
                        DiskpartScriptManager dsm = new DiskpartScriptManager();
                        DiskOperation.GenerateMBRAndUEFIScript(WTGModel.efiPartitionSize.ToString(), WTGModel.ud, WTGModel.partitionSize);

                        if (WTGModel.CheckedMode == ApplyMode.Legacy)
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
                    if (WTGModel.udString.Contains("Removable Disk") && WTGModel.CheckedMode == ApplyMode.Legacy)
                    {
                        if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_Legacywarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        {
                            return;
                        }
                    }

                    if (!WTGModel.rePartition && !WTGModel.doNotFormat)//普通格式化
                    {
                        ProcessManager.ECMD("cmd.exe", "/c format " + WTGModel.ud.Substring(0, 2) + "/FS:ntfs /q /V: /Y");
                        //
                    }
                    else if (WTGModel.rePartition)
                    {
                        DiskOperation.DiskPartRePartitionUD(WTGModel.partitionSize);

                    }

                    #endregion
                    ///////////////////////////////////正式开始////////////////////////////////////////////////
                    if (WTGModel.CheckedMode == ApplyMode.Legacy)
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

        public static void FinishSuccessful()
        {
            if (WTGModel.noDefaultDriveLetter && !WTGModel.udString.Contains("Removable Disk"))
            {
                DiskOperation.SetNoDefaultDriveLetter(WTGModel.ud);
            }
            writeSw.Stop();

            Finish f = new Finish(writeSw.Elapsed);
            f.ShowDialog();
        }
    }
}
