using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace wintogo
{
    public static class Log
    {
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
            ProcessManager.ECMD("cmd.exe","/c del /f /s /q \"" + WTGModel.logPath + "\\*.*\"");
        }

    }
}