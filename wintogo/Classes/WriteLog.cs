using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using wintogo.Utility;

namespace wintogo
{
    public static class Log
    {
        public static void WriteProgramRunInfoToLog()
        {
            WTGModel.CreateGuid = Guid.NewGuid().ToString();
            //try
            //{

            Dictionary<string, string> infoDict = new Dictionary<string, string>();
            //infoDict.Add("App Path", Application.StartupPath);
            //infoDict.Add("OS Version", Environment.OSVersion.ToString());
            infoDict.Add("Dism Version", FileOperation.GetFileVersion(Environment.GetEnvironmentVariable("windir") + "\\System32\\dism.exe"));
            infoDict.Add("Wim file", WTGModel.imageFilePath);
            infoDict.Add("Usb Disk", WTGModel.udString);
            infoDict.Add("Mode", WTGModel.CheckedMode.ToString());
            infoDict.Add("VHDName", WTGModel.vhdNameWithoutExt);
            infoDict.Add("Re-Partition", WTGModel.rePartition.ToString());
            infoDict.Add("VHD Size Set", WTGModel.userSetSize.ToString());
            infoDict.Add("Fixed VHD", WTGModel.isFixedVHD.ToString());
            infoDict.Add("Donet", WTGModel.installDonet35.ToString());
            infoDict.Add("Disable-WinRE", WTGModel.disableWinRe.ToString());
            infoDict.Add("Block Local Disk", WTGModel.isBlockLocalDisk.ToString());
            infoDict.Add("NoTemp", WTGModel.isNoTemp.ToString());
            infoDict.Add("UEFI+GPT", WTGModel.isUefiGpt.ToString());
            infoDict.Add("UEFI+MBR", WTGModel.isUefiMbr.ToString());
            //infoDict.Add("WIMBOOT", WTGModel.isWimBoot.ToString());
            //infoDict.Add("CompactOS", WTGModel.isCompactOS.ToString());
            infoDict.Add("No-format", WTGModel.doNotFormat.ToString());
            infoDict.Add("NtfsUefiSupport", WTGModel.ntfsUefiSupport.ToString());
            infoDict.Add("FixLetter", WTGModel.fixLetter.ToString());
            infoDict.Add("SelectedPart", WTGModel.wimPart.ToString());
            infoDict.Add("Partitions", string.Join(",", WTGModel.partitionSize));
            infoDict.Add("NoDefalutLetter", WTGModel.noDefaultDriveLetter.ToString());
            infoDict.Add("Bitlocker", WTGModel.isBitlocker.ToString());
            infoDict.Add("SkipOOBE", WTGModel.skipOOBE.ToString());
            infoDict.Add("CreateGuid", WTGModel.CreateGuid);

            /*Thread t = new Thread(() =>
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
            t.Start();*/

            StringBuilder sb = new StringBuilder();
            foreach (var item in infoDict)
            {
                sb.AppendLine(item.Key + ":" + item.Value);
            }
            WriteLog("Environment", sb.ToString());
            
        }

        public static void AppendLog(string LogNameWithExt,string WriteInfo)
        {
            try
            {
                string LogName = LogNameWithExt;
                if (!Directory.Exists(WTGModel.logPath)) { Directory.CreateDirectory(WTGModel.logPath); }
                //if (File.Exists(WTGModel.logPath + "\\" + LogName)) { File.Delete(WTGModel.logPath + "\\" + LogName); }
                using (FileStream fs0 = new FileStream(WTGModel.logPath + "\\" + LogName, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.UTF8))
                    {
                        string ws0 = string.Empty;
                        ws0 = WriteInfo;
                        sw0.WriteLine(ws0);
                    }
                }
            }
            catch
            {
                //MessageBox.Show(e.ToString());
            }
        }
        /// <summary>
        /// 写入日志文件，可附加
        /// </summary>
        /// <param name="LogName">文件名</param>
        /// <param name="WriteInfo">日志信息</param>
        public static void WriteLog(string LogNameWithoutExt, string WriteInfo)
        {
            try
            {
                string LogName = LogNameWithoutExt + "_" + DateTime.Now.ToFileTime() + ".log";
                if (!Directory.Exists(WTGModel.logPath)) { Directory.CreateDirectory(WTGModel.logPath); }
                if (File.Exists(WTGModel.logPath + "\\" + LogName)) { File.Delete(WTGModel.logPath + "\\" + LogName); }
                using (FileStream fs0 = new FileStream(WTGModel.logPath + "\\" + LogName, FileMode.Append, FileAccess.Write))
                {
                    fs0.SetLength(0);
                    using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.UTF8))
                    {
                        string ws0 = string.Empty;

                        ws0 = Application.ProductName + Application.ProductVersion;
                        sw0.WriteLine(ws0);
                        ws0 = DateTime.Now.ToString();
                        sw0.WriteLine(ws0);
                        ws0 = WriteInfo;
                        sw0.WriteLine(ws0);

                    }
                }
            }
            catch
            {
                //MessageBox.Show(e.ToString());
            }

            //sw0.Close();


        }
        public static void DeleteAllLogs()
        {
            //ProcessManager.SyncCMD("del /f /s /q \"" + WTGModel.logPath + "\\*.*\"");
            try
            {
                new DirectoryInfo(WTGModel.logPath).EmptyDir();
            }
            catch (Exception ex)            
            {
                Console.WriteLine(ex);
            }
            //ProcessManager.ECMD("cmd.exe","/c del /f /s /q \"" + WTGModel.logPath + "\\*.*\"");
        }
        private static void EmptyDir(this DirectoryInfo directory)
        {
            foreach (FileInfo item in directory.GetFiles())
            {
                item.Delete();
            }
            foreach (DirectoryInfo item in directory.GetDirectories())
            {
                item.Delete(true);
            }

        }

    }
}