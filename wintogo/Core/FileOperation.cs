using System;
using System.Diagnostics;
using System.IO;
//using System.Threading.Tasks;
namespace wintogo
{
    public static class FileOperation
    {
        public static void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir)) //如果存在这个文件夹删除之     
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        FileInfo fi = new FileInfo(d);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;

                        File.Delete(d); //直接删除其中的文件    
                    }
                    else
                        DeleteFolder(d); //递归删除子文件夹     
                }
                Directory.Delete(dir, true); //删除已空文件夹                     
            }
        }
        public static string GetFileVersion(string path)
        {
            try
            {
                // Get the file version for the notepad.   
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(path);
                return myFileVersionInfo.FileVersion;
                // Print the file name and version number.   
                //textBox1.Text = "File: " + myFileVersionInfo.FileDescription + '\n' +
                //"Version number: " + myFileVersionInfo.FileVersion;
            }
            catch(Exception ex)
            {
                Log.WriteLog("Err_GetFileVersion", ex.ToString());
                return string.Empty; 
            }
        }


        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

    }
}
