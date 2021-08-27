using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace wintogo
{
    public static class ProcessManager
    {
        //public static WriteProgress wp;
        public static StringBuilder output = new StringBuilder();
        //public delegate void AppendTextCallback(string text);
        //public static double percentage = -1;

        #region 解决多线程下控件访问的问题
        //private bool requiresClose = true;


        public static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            try
            {

                if (string.IsNullOrEmpty(e.Data) == false)
                { AppendText(e.Data + "\r\n"); }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        private static void progress_Exited(object sender, EventArgs e)
        {
            //Log.AppendLog("Output.log", output.ToString());
            try
            {
                Log.WriteLog("Output", output.ToString());
                output.Clear();

                /*
                wp.Invoke(new Action(() =>
                {
                    wp.IsUserClosing = false;
                    wp.Close();
                }));*/

            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_progress_Exited", ex.ToString());

            }
        }

        public static void AppendText(string text)
        {
           
            
            output.Append(text);
            //Log.AppendLog("Output.log", text);
            //try
            //{
            Thread t = new Thread(() =>
            {
                
                
                try
                {

                    
                    int c = 0;
                    while (WTGModel.wp == null || !WTGModel.wp.IsHandleCreated)
                    {
                        if (++c > 100)
                        {
                            return;
                        }
                        Thread.Sleep(50);
                    }
                    WTGModel.wp.textBox1.Invoke(new Action(() => {
                        WTGModel.wp.textBox1.AppendText(text);
                    }));
                    /*
                    //IntPtr IsHandleCreated = wp.Handle;
                    string[] txtLines = wp.textBox1.Lines;
                    if (txtLines.Length == 0 || txtLines.Length == 1 || (txtLines.Length - 2 >= 0 && text != txtLines[txtLines.Length - 2] + "\r\n"))
                    {
                        wp.textBox1.BeginInvoke(new Action(() => { wp.textBox1.AppendText(text); }));
                    }*/
                }
                catch (Exception ex)
                {
                    Log.WriteLog("Err_AppendText", ex.ToString());
                    Console.WriteLine(ex);
                    //MessageBox.Show(ex.ToString());
                }
            });
            t.Start();
            //if (!wp.IsHandleCreated) return;


        }

        #endregion
        public static void SyncCMD(List<string> cmds)
        {
            Process process = new Process();

            try
            {
                process.StartInfo.FileName = "cmd.exe";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                foreach (var cmd in cmds)
                {
                    process.StandardInput.WriteLine(cmd);
                }
                process.StandardInput.WriteLine("exit");

                process.WaitForExit();

            }
            catch (Exception ex)
            {
                MessageBox.Show(MsgManager.GetResString("Msg_Failure", MsgManager.ci) + ex.ToString());
            }
            finally
            {
                process.Close();

            }
        }
        public static int SyncCMD(string cmd)
        {
            Process process = new Process();
            int exitcode = 1;
            try
            {
                process.StartInfo.FileName = "cmd.exe";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.WriteLine(cmd);

                process.StandardInput.WriteLine("exit");

                process.WaitForExit();
                exitcode = process.ExitCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(MsgManager.GetResString("Msg_Failure", MsgManager.ci) + ex.ToString());
            }
            finally
            {
                process.Close();

            }
            return exitcode;
        }
        private static void ExecuteCMD(string StartFileName, string StartFileArg, params string[] Txt)
        {

            Process process = new Process();
            //wp.ShowDialog();

            try
            {
                AppendText("Command:" + StartFileName + StartFileArg + "\r\n");
                for (int i = 0; i < Txt.Length; i++)
                {
                    AppendText(Txt[i]);
                }
                process.StartInfo.FileName = StartFileName;
                process.StartInfo.Arguments = StartFileArg;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(progress_Exited);
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
                int exitCode = process.ExitCode;
                if (exitCode != 0)
                {
                    Log.WriteLog("Err_ProcessExit", exitCode.ToString());
                }
                Console.WriteLine("Exit: " + process.ExitCode);
            }
            catch (Exception ex)
            {
                //MsgManager.getResString("Msg_Failure")
                //操作失败
                MessageBox.Show(MsgManager.GetResString("Msg_Failure", MsgManager.ci) + " " + StartFileName + " " + ex.ToString());
            }

        }
        /*public static void Do(ThreadStart ts)
        {

            wp = new WriteProgress();
            wp.IsUserClosing = true;
            Thread t = new Thread(ts);
            t.Start();
            wp.ShowDialog();

        }*/
        public static void ECMD(string StartFileName, string StartFileArg, params string[] TextAppend)
        {
            try
            {
                if (WTGModel.wp == null)
                {
                    //WTGModel.wp.s
                    Thread twp = new Thread(new ThreadStart(() =>
                    {
                        WTGModel.wp = new WriteProgress();
                        WTGModel.wp.IsUserClosing = true;
                        WTGModel.wp.ShowDialog();
                    }));
                    twp.Start();
                }
                AppendText("Command:" + StartFileName + StartFileArg + "\r\n");
                Thread.Sleep(3000);
                ExecuteCMD(StartFileName, StartFileArg);

                //wp.ShowDialog();
                
                /*if (wp.OnClosingException != null)
                {
                    throw wp.OnClosingException;
                }*/
            }
            catch
            {
                //MessageBox.Show("Test");
                KillProcessByName(Path.GetFileName(StartFileName));
                throw;
            }

        }
        public static void KillProcessByName(string pName)
        {
            try
            {
                Process[] ps = Process.GetProcesses();
                foreach (Process item in ps)
                {
                    if (item.ProcessName == pName)
                    {
                        item.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_KillProcessByName", ex.ToString());
            }

        }

    }

    [Serializable]
    public class UserCancelException : Exception
    {
        public UserCancelException() : base("用户取消操作！")
        {
            VHDOperation.CleanTemp();
        }
        public UserCancelException(string message) : base(message) { }
        public UserCancelException(string message, Exception inner) : base(message, inner) { }
        protected UserCancelException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
