using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace wintogo
{
    public static class StringUtility
    {
        public static bool IsChina(string CString)
        {
            bool BoolValue = false;
            //if (CString.IndexOf(' ') >= 0)
            //{
            //    return true;
            //}
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) > Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = true;
                }

            }
            return BoolValue;
        }
        public static bool IsChinaOrContainSpace(string CString)
        {
            bool BoolValue = false;
            if (CString.IndexOf(' ') >= 0)
            {
                return true;
            }
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) > Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = true;
                }

            }
            return BoolValue;
        }

        public static string Combine(params string[] paths)
        {
            if (paths.Length == 0)
            {
                throw new ArgumentException("please input path");
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                string spliter = "\\";

                string firstPath = paths[0];

                if (firstPath.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
                {
                    spliter = "/";
                }

                if (!firstPath.EndsWith(spliter))
                {
                    firstPath = firstPath + spliter;
                }
                builder.Append(firstPath);

                for (int i = 1; i < paths.Length; i++)
                {
                    string nextPath = paths[i];
                    if (nextPath.StartsWith("/") || nextPath.StartsWith("\\"))
                    {
                        nextPath = nextPath.Substring(1);
                    }

                    if (i != paths.Length - 1)//not the last one
                    {
                        if (nextPath.EndsWith("/") || nextPath.EndsWith("\\"))
                        {
                            nextPath = nextPath.Substring(0, nextPath.Length - 1) + spliter;
                        }
                        else
                        {
                            nextPath = nextPath + spliter;
                        }
                    }

                    builder.Append(nextPath);
                }

                return builder.ToString();
            }
        }
        /// <summary>
        /// // 0xae00cf => "AE00CF " 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes) 
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

    }
}
