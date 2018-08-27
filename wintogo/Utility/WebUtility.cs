using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wintogo
{
    public static class WebUtility
    {
        public static void VisitWeb(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                try
                {
                    RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                    string s = key.GetValue("").ToString();

                    Regex reg = new Regex("\"([^\"]+)\"");
                    MatchCollection matchs = reg.Matches(s);

                    string filename = "";
                    if (matchs.Count > 0)
                    {
                        filename = matchs[0].Groups[1].Value;
                        Process.Start(filename, url);
                    }
                }
                catch (Exception ex)
                {
                    //MsgManager.getResString("Msg_FatalError")
                    //程序遇到严重错误\n官方支持论坛：bbs.luobotou.org\n
                    MessageBox.Show("程序遇到严重错误\nFATAL ERROR!官方支持论坛：bbs.luobotou.org\n" + ex.ToString());

                }
            }


        }
    }
}
