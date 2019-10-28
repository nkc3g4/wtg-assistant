using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wintogo
{

    public static class FileInitialization
    {
        public static void FileValidation()
        {
            if (StringUtility.IsChina(WTGModel.diskpartScriptPath))
            {
                if (StringUtility.IsChina(Application.StartupPath))
                {
                    Log.WriteLog("Err_IsChinaOrContainSpace", "FileValidationErr");
                    ErrorMsg er = new ErrorMsg(MsgManager.GetResString("IsChinaMsg", MsgManager.ci),false);
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
