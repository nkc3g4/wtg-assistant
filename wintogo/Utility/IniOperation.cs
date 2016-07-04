using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace wintogo
{
    class IniFile
    {
        //绝对路径（默认执行程序目录）
        public string FilePath { get; set; }

        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="section">段落名</param>
        /// <param name="key">键</param>
        /// <param name="defVal">缺省值</param>
        /// <param name="retVal">所对应的值，如果该key不存在则返回空值</param>
        /// <param name="size">值允许的大小</param>
        /// <param name="filePath">INI文件的完整路径和文件名</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string section, string key, string defVal,
            StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="section">段落名</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filePath">INI文件的完整路径和文件名</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string section, string key, string val, string filePath);

        #region 静态方法

        public static string ReadVal(string section, string key, string filePath)
        {
            string defVal = string.Empty;
            StringBuilder retVal = new StringBuilder(260);
            int size = 102400;
            string rt = string.Empty;
            try
            {
                GetPrivateProfileString(section, key, defVal, retVal, size, filePath);
                rt = retVal.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                
                rt = string.Empty;
            }
            return rt;
        }

        public static bool WriteVal(string section, string key, string val, string filePath)
        {
            try
            {
                if (WritePrivateProfileString(section, key, val, filePath) == 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 对象方法

        public string ReadVal(string section, string key)
        {
            string defVal = string.Empty;
            StringBuilder retVal = new StringBuilder();
            int size = 10240;
            string rt = string.Empty;
            try
            {
                GetPrivateProfileString(section, key,
                    defVal, retVal, size, this.FilePath);
                rt = retVal.ToString();
            }
            catch
            {
                rt = string.Empty;
            }
            return rt;
        }

        public bool WriteVal(string section, string key, string val)
        {
            try
            {
                WritePrivateProfileString(section, key, val, this.FilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}