using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;


namespace wintogo
{

    public static class FileInitialization
    {
        public static List<string> appFileList = new List<string> { "bootice.exe", "BitlockerConfig_x64.exe", "BitlockerConfig_x86.exe", "imagex_x86.exe", "san_policy.xml", "unattend_oobe.xml", "unattend_templete.xml", "unattend_winre.xml" };
        public static void FileValidation()
        {
            if (StringUtility.IsChina(WTGModel.diskpartScriptPath))
            {
                if (StringUtility.IsChina(Application.StartupPath))
                {
                    Log.WriteLog("Err_IsChinaOrContainSpace", "FileValidationErr");
                    ErrorMsg er = new ErrorMsg(MsgManager.GetResString("IsChinaMsg", MsgManager.ci), false);
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
                if (Directory.Exists(WTGModel.applicationFilesPath))
                {
                    Directory.Delete(WTGModel.applicationFilesPath, true);
                }
                ZipFile.ExtractToDirectory(Application.StartupPath + "\\files.dat", WTGModel.applicationFilesPath);

                //ZipHelper.UnZip(Application.StartupPath + "\\files.dat", WTGModel.applicationFilesPath);
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_Unzip", ex.ToString());
            }
            //Validate Files
            foreach (var item in appFileList)
            {
                if (!File.Exists(WTGModel.applicationFilesPath + "\\" + item))
                {
                    ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_FileBroken", MsgManager.ci), false);
                    er.ShowDialog();
                    Environment.Exit(0);

                }
            }
        }

    }
    public static class Config
    {
        public static string settingFilePath = Application.StartupPath + "\\settings.ini";

        public static void ReadConfigFile(ref bool autoCheckUpdate)
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

    }


}
