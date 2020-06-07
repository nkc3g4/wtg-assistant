using System;
using System.IO;
using System.Text;

namespace wintogo
{
    public class DiskpartScriptManager
    {
        /// <summary>
        /// Diskpart命令
        /// </summary>
        public string Args { private get; set; }
        private string TempScriptFile
        {
            get; set;
        }
        public bool OutputToFile { get; set; }
        /// <summary>
        /// 输出文件路径
        /// </summary>
        public string OutputFilePath { get; private set; }
        private string CurrentGuid = Guid.NewGuid().ToString();
        public DiskpartScriptManager()
        {
            this.OutputToFile = false;
            TempScriptFile = StringUtility.Combine(WTGModel.diskpartScriptPath, "\\" + CurrentGuid);
            //TempScriptFile = WTGModel.diskpartScriptPath + "\\" + CurrentGuid;
        }
        public DiskpartScriptManager(bool outputToFile)
        {
            if (outputToFile) this.OutputToFile = true;
            TempScriptFile = StringUtility.Combine(WTGModel.diskpartScriptPath, "\\" + CurrentGuid);

            //TempScriptFile = WTGModel.diskpartScriptPath + "\\" + CurrentGuid;

        }
        private void CreateScriptFile()
        {

            using (FileStream fs = new FileStream(TempScriptFile, FileMode.Create, FileAccess.Write))
            {
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    string ws = Args;
                    //System.Windows.Forms.MessageBox.Show(args);
                    sw.Write(ws);
                }
            }
            if (!File.Exists(TempScriptFile))
            {
                Log.WriteLog("Err_diskpart_" + CurrentGuid, Args);
            }
            Log.WriteLog("CMD_diskpart_" + CurrentGuid, Args);
        }
        /// <summary>
        /// 执行Diskpart命令
        /// </summary>


        //public void RunDiskpartScriptByScriptFile(string scriptFile)
        //{
        //    StringBuilder dpargs = new StringBuilder();
        //    dpargs.Append(" /s \"");
        //    dpargs.Append(scriptFile);
        //    try
        //    {
        //        ProcessManager.ECMD("diskpart.exe", dpargs.ToString());
        //    }
        //    catch (Exception)
        //    {
        //        //ProcessManager.KillProcessByName("diskpart.exe");
        //        throw;

        //    }
        //}


        public void RunDiskpartScript()
        {
            OutputFilePath = Path.GetTempFileName();
            CreateScriptFile();
            StringBuilder dpargs = new StringBuilder();
            dpargs.Append(" /s \"");
            dpargs.Append(TempScriptFile);
            dpargs.Append("\"");
            if (this.OutputToFile)
            {
                dpargs.Append(" > ");
                dpargs.Append("\"");
                dpargs.Append(this.OutputFilePath);
                dpargs.Append("\"");
                ProcessManager.SyncCMD("diskpart.exe "+ dpargs.ToString());
            }
            else
            {
                try
                {
                    //Console.WriteLine("Diskpart.exe");
                    ProcessManager.ECMD("diskpart.exe", dpargs.ToString());
                }
                catch (Exception)
                {
                    //ProcessManager.KillProcessByName("diskpart.exe");
                    throw;

                }
            }
            //System.Console.WriteLine(File.ReadAllText (this.scriptPath));
            //System.Console.WriteLine(dpargs.ToString());
            //System.Windows.Forms.MessageBox.Show(dpargs.ToString());

            //System.Console.WriteLine(File.ReadAllText (this.outputFilePath));
            //FileOperation.DeleteFile(TempScriptFile);
        }
        /// <summary>
        /// 删除输出文件
        /// </summary>
        public void DeleteOutputFile()
        {
            FileOperation.DeleteFile(this.OutputFilePath);
        }



    }
}
