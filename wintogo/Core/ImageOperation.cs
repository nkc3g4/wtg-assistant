using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace wintogo
{
    public class ImageOperation
    {
        public string imageFile { get; set; }
        public string imageX { get; set; }
        //public bool isESD { get; set; }
        //public string imageIndex { get; set; }
        //public bool isWimboot { get; set; }
        //public int win7togo { get; set; }

        #region 静态方法
        public static List<string> DismGetImagePartsInfo(string imageFile)
        {
            List<string> list = new List<string>();
            string tempFile = Path.GetTempFileName();
            ProcessManager.SyncCMD("Dism.exe /Get-WimInfo  /WimFile:\"" + imageFile + "\" /english > \"" + tempFile + "\"");
            string partsInfo = File.ReadAllText(tempFile, Encoding.Default);
            //MessageBox.Show(partsInfo);
            MatchCollection matches = Regex.Matches(partsInfo, @"Index : ([0-9]+?)[\w\W]*?Name : (.+)", RegexOptions.ECMAScript);
            foreach (Match item in matches)
            {
                list.Add(item.Groups[1].Value + " : " + item.Groups[2].Value);
            }
            File.Delete(tempFile);
            return list;

        }
        public static string AutoChooseESDImageIndex(string esdPath)
        {
            string outputFilePath = Path.GetTempFileName();
            StringBuilder args = new StringBuilder();
            args.Append(" /get-wiminfo /wimfile:\"");
            args.Append(esdPath);
            args.Append("\" /english");
            args.Append(" > ");
            args.Append("\"");
            args.Append(outputFilePath);
            args.Append("\"");
            //ProcessManager.RunDism(args.ToString());
            ProcessManager.SyncCMD("dism.exe" + args.ToString());

            string outputFileText = File.ReadAllText(outputFilePath);
            MatchCollection mc = Regex.Matches(outputFileText, @"Index");
            if (mc.Count > 1) { FileOperation.DeleteFile(outputFilePath); return "4"; }
            else { FileOperation.DeleteFile(outputFilePath); return "1"; }
            //Match match = Regex.Match(outputFileText, @"Index :([1-9]).+Windows Technical Preview", RegexOptions.Singleline);
            //MessageBox.Show(match.Groups[1].Value);
            //ProcessManager.ECMD("diskpart.exe", args.ToString());

            //System.Console.WriteLine(File.ReadAllText (this.scriptPath));
            //System.Console.WriteLine(dpargs.ToString());
            //System.Windows.Forms.MessageBox.Show(dpargs.ToString());

            //System.Console.WriteLine(File.ReadAllText (this.outputFilePath));

        }

        /// <summary>
        /// framework3.5，屏蔽本机硬盘，禁用WINRE
        /// </summary>
        /// <param name="framework"></param>
        /// <param name="sanPolicy"></param>
        /// <param name="disWinre"></param>
        /// <param name="skipOOBE"></param>
        /// <param name="imageLetter">可以是U盘盘符或V盘</param>
        /// <param name="wimLocation">WIM文件路径</param>
        public static void ImageExtra(bool framework, bool sanPolicy, bool disWinre, bool skipOOBE, bool disUasp, string imageLetter, string wimLocation)
        {
            AddDrivers(imageLetter);
            Solve1809(imageLetter.Substring(0, 2));
            DriveIcon(imageLetter.Substring(0, 2));
            if (disUasp)
            {
                DisableUASP(imageLetter);
            }
            if (framework)
            {
                StringBuilder args = new StringBuilder();
                args.Append(" /image:");
                args.Append(imageLetter.Substring(0, 2));
                args.Append(" /enable-feature /featurename:NetFX3 /source:");
                args.Append(wimLocation.Substring(0, wimLocation.Length - 11));
                args.Append("sxs");
                //ProcessManager.RunDism(args.ToString());
                try
                {
                    ProcessManager.ECMD("dism.exe", args.ToString());
                }
                catch (Exception)
                {
                    //ProcessManager.KillProcessByName("dism.exe");
                }


            }
            if (sanPolicy)
            {
                ProcessManager.ECMD("dism.exe", " /image:" + imageLetter.Substring(0, 2) + " /Apply-Unattend:\"" + WTGModel.applicationFilesPath + "\\san_policy.xml\"");

            }

            if (disWinre || skipOOBE)
            {
                try
                {
                    if (Directory.Exists(imageLetter + "Windows\\System32\\sysprep\\"))
                    {
                        //File.Copy(WTGModel.applicationFilesPath + "\\unattend.xml", imageletter + "Windows\\System32\\sysprep\\unattend.xml");
                        string unattendTemplete = File.ReadAllText(WTGModel.applicationFilesPath + "\\unattend_templete.xml");
                        string unattendSettings = string.Empty;
                        if (disWinre)
                            unattendSettings += File.ReadAllText(WTGModel.applicationFilesPath + "\\unattend_winre.xml");
                        if (skipOOBE)
                            unattendSettings += File.ReadAllText(WTGModel.applicationFilesPath + "\\unattend_oobe.xml");

                        File.WriteAllText(imageLetter + "Windows\\System32\\sysprep\\unattend.xml", unattendTemplete.Replace("#", unattendSettings));
                    }
                }
                catch(Exception e)
                {
                    Log.WriteLog("Err_unattend", e.ToString());
                }
            }
        }
        public static void AutoChooseWimIndex(ref string wimpart, int win7togo)
        {
            if (wimpart == "0")
            {//自动判断模式
                if (WTGModel.isEsd)
                { wimpart = AutoChooseESDImageIndex(WTGModel.imageFilePath); }
                else
                {
                    if (win7togo == 1)
                    {//WIN7 32 bit

                        wimpart = "5";
                    }
                    else if (win7togo == 2)
                    { //WIN7 64 BIT

                        wimpart = "4";
                    }
                    else
                    {
                        if (DismGetImagePartsInfo(WTGModel.imageFilePath).Count == 3)
                        {
                            wimpart = "2";

                        }
                        else
                        {
                            wimpart = "1";
                        }
                    }
                }
            }
        }
        private static void WimbootApply(string sourceImageFile, string destinationImageDisk, string wimindex, string applyDir)
        {
            ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + sourceImageFile + "\" /SourceIndex:" + wimindex.ToString() + " /DestinationImageFile:" + destinationImageDisk + "wimboot.wim");
            ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + destinationImageDisk + "wimboot.wim" + "\" /ApplyDir:" + applyDir.Substring(0, 2) + " /Index:" + wimindex.ToString() + " /WIMBoot");
        }
        private static void DismApplyImage(string imageFile, string targetDisk, string wimIndex, bool isCompactOS)
        {
            if (isCompactOS)
            {
                ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + imageFile + "\" /ApplyDir:" + targetDisk.Substring(0, 2) + " /Index:" + wimIndex.ToString() + " /compact");
            }
            else
            {
                ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + imageFile + "\" /ApplyDir:" + targetDisk.Substring(0, 2) + " /Index:" + wimIndex.ToString());
            }
            //ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + imageFile + "\" /ApplyDir:" + targetDisk.Substring(0, 2) + " /Index:" + wimIndex.ToString());
            //wp.ShowDialog();

        }
        private static void ImageXApply(string imagex, string imageFile, string wimIndex, string targetDisk)
        {
            ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\" + imagex, " /apply " + "\"" + imageFile + "\"" + " " + wimIndex.ToString() + " " + targetDisk);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iswimboot">是否WIMBOOT</param>
        /// <param name="isesd">是否ESD</param>
        /// <param name="imagex">imagex路径</param>
        /// <param name="imageFile">镜像文件</param>
        /// <param name="wimIndex"></param>
        /// <param name="targetDisk">目标磁盘</param>
        /// <param name="wimbootApplyDir">/ImageFile:\"" + wimbootApplyDir + "wimboot.wim"</param>
        public static void ImageApply(bool iswimboot, bool isesd, string imagex, string imageFile, string wimIndex, string targetDisk, string wimbootApplyDir)
        {
            if (iswimboot)
            {
                WimbootApply(imageFile, wimbootApplyDir, wimIndex, targetDisk);
            }
            else
            {
                if (isesd || WTGModel.allowEsd)//allowEsd只是判断DISM版本,支持COMPACTOS必然AllowESD=TRUE
                {
                    DismApplyImage(imageFile, targetDisk, wimIndex, WTGModel.isCompactOS);
                }
                else
                {
                    ImageXApply(imagex, imageFile, wimIndex, targetDisk);
                }
            }
        }
        /// <summary>
        /// 判断是否为WIN7 以及32或64位
        /// </summary>
        /// <param name="imagex">imagex文件名，默认传imagex字段</param>
        /// <param name="wimfile">WIM文件路径</param>
        /// <returns>不是WIN7系统：0，Windows 7 STARTER（表示为32位系统镜像）：1，Windows 7 HOMEBASIC（表示为64位系统镜像）：2</returns>
        public static int Iswin7(string imagex, string wimfile)
        {
            ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\" + imagex + "\"" + " /info \"" + wimfile + "\" /xml > " + "\"" + WTGModel.logPath + "\\wiminfo.xml\"");
            XmlDocument xml = new XmlDocument();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlNodeReader reader = null;

            string strFilename = WTGModel.logPath + "\\wiminfo.xml";
            if (!File.Exists(strFilename))
            {
                //MsgManager.getResString("Msg_wiminfoerror")
                //WIM文件信息获取失败\n将按WIN8系统安装
                Log.WriteLog("Err_Iswin7", strFilename + "文件不存在");
                //MessageBox.Show(strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci));
                return 0;
            }
            try
            {
                doc.Load(strFilename);
                reader = new System.Xml.XmlNodeReader(doc);
                while (reader.Read())
                {
                    if (reader.IsStartElement("NAME"))
                    {

                        //从找到的这个依次往下读取节点
                        System.Xml.XmlNode aa = doc.ReadNode(reader);
                        if (aa.InnerText == "Windows 7 STARTER")
                        {
                            return 1;
                        }
                        else if (aa.InnerText == "Windows 7 HOMEBASIC")
                        {
                            return 2;
                        }
                        else { return 0; }
                    }
                }
            }

            catch (Exception ex)
            {
                Log.WriteLog("Err_Iswin7", strFilename + "\n" + ex.ToString());
                //MessageBox.Show(strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci) + ex.ToString());
                return 0;
            }



            return 0;
        }
        /// <summary>
        /// WIN7 TO GO注册表处理
        /// </summary>
        /// <param name="installdrive">系统盘所在盘盘符例如E:</param>
        public static void Win7REG(string installdrive)
        {
            try
            {
                ProcessManager.SyncCMD("reg.exe load HKU\\sys " + installdrive + "Windows\\System32\\Config\\SYSTEM  > \"" + WTGModel.logPath + "\\Win7REGLoad.log\"");
                int errorlevel = ProcessManager.SyncCMD("reg.exe import \"" + WTGModel.applicationFilesPath + "\\usb.reg\" >nul &if %errorlevel% ==0 (echo 注册表导入成功) else (echo 注册表导入失败)" + " > \"" + WTGModel.logPath + "\\Win7REGImport.log\"");
                ProcessManager.SyncCMD("reg.exe unload HKU\\sys " + " > \"" + WTGModel.logPath + "\\Win7REGUnLoad.log\"");
                Log.WriteLog("Info_ImportReg", errorlevel.ToString());
                Fixletter("C:", installdrive);

            }
            catch (Exception err)
            {
                //MsgManager.getResString("Msg_win7usberror")
                //处理WIN7 USB启动时出现问题
                MessageBox.Show(MsgManager.GetResString("Msg_win7usberror", MsgManager.ci) + err.ToString());
            }
        }
        /// <summary>
        /// 修复盘符
        /// </summary>
        /// <param name="targetletter">一般为"C:"</param>
        /// <param name="currentos">例如"V:"</param>
        public static void Fixletter(string targetletter, string currentos)
        {
            try
            {
                byte[] registData;
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey software = hkml.OpenSubKey("SYSTEM", false);
                RegistryKey aimdir = software.OpenSubKey("MountedDevices", false);
                registData = (byte[])aimdir.GetValue("\\DosDevices\\" + currentos);
                if (registData != null)
                {
                    ProcessManager.SyncCMD("reg.exe load HKU\\TEMP " + currentos + "\\Windows\\System32\\Config\\SYSTEM  > \"" + WTGModel.logPath + "\\loadreg.log\"");
                    RegistryKey hklm = Registry.Users;
                    RegistryKey temp = hklm.OpenSubKey("TEMP", true);
                    try
                    {
                        foreach (var item in temp.GetSubKeyNames())
                        {
                            if (item == "MountedDevices")
                            {
                                temp.DeleteSubKey("MountedDevices");
                            }
                        }

                    }
                    catch (Exception ex)
                    { Log.WriteLog("Err_FixletterDeleteSubKey", ex.ToString()); }
                    RegistryKey wtgreg = temp.CreateSubKey("MountedDevices");
                    wtgreg.SetValue("\\DosDevices\\" + targetletter, registData, RegistryValueKind.Binary);
                    wtgreg.Close();
                    temp.Close();
                    ProcessManager.SyncCMD("reg.exe unload HKU\\TEMP > \"" + WTGModel.logPath + "\\unloadreg.log\"");

                }
                else
                {
                    Log.WriteLog("Err_registDataNull", "\\DosDevices\\null");

                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Err_Fixletter", ex.ToString());
            }
        }
        public static void AddDrivers(string target)
        {
            if (Directory.Exists(Application.StartupPath + "\\Drivers"))
            {
                ProcessManager.ECMD("dism.exe", "/image:" + target + " /add-driver /driver:\"" + Application.StartupPath + "\\Drivers\"" + " /recurse /ForceUnsigned");
            }
        }
        #endregion
        public static void DisableUASP(string installdrive)
        {
            int errorlevel = ProcessManager.SyncCMD(WTGModel.applicationFilesPath + "\\UASP\\UASP.EXE " + WTGModel.ud.Substring(0, 2) + " " + installdrive.Substring(0, 2));

            Log.WriteLog("Info_DisUASP", errorlevel.ToString());


            //ProcessManager.SyncCMD("reg.exe load HKU\\def " + installdrive + "Windows\\System32\\Config\\DEFAULT  > \"" + WTGModel.logPath + "\\UASPREGLoad.log\"");
            //ProcessManager.SyncCMD("reg.exe import \"" + WTGModel.applicationFilesPath + "\\UASP\\Run.reg\"");
            //ProcessManager.SyncCMD("reg.exe unload HKU\\def " + " > \"" + WTGModel.logPath + "\\UASPREGUnLoad.log\"");
            //Log.WriteLog("Info_UASPImportReg", errorlevel.ToString());

        }
        public static void Solve1809(string installDrive)
        {

            string wppPath = installDrive + "\\Windows\\system32\\drivers\\wpprecorder.sys";
            try
            {
                int wppver = FileVersionInfo.GetVersionInfo(wppPath).ProductBuildPart;
                if (wppver == 17763)
                {
                    FileSecurity fileAcl = File.GetAccessControl(wppPath);
                    IdentityReference everyoneUser = new NTAccount("Everyone");
                    ProcessManager.SyncCMD("cmd.exe /c takeown /f " + wppPath + " && icacls " + wppPath + " /grant administrators:F");
                    //MessageBox.Show(WellKnownSidType.AccountAdministratorSid);
                    //fileAcl.SetOwner(new SecurityIdentifier(WellKnownSidType.AccountAdministratorSid.ToString()));
                    FileSystemAccessRule everyoneRule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow);
                    fileAcl.AddAccessRule(everyoneRule);

                    File.Copy(WTGModel.applicationFilesPath + "\\wpprecorder.sys", wppPath, true);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Solve1809", ex.ToString());
            }

        }
        public static void DriveIcon(string installDrive)
        {
            try
            {
                if (WTGModel.udString.ToUpper().Contains("CHIPFANCIER"))
                {
                    File.Copy(WTGModel.applicationFilesPath + "\\CF\\CHIPFANCIER.ico", installDrive + "\\CHIPFANCIER.ico", true);
                    File.Copy(WTGModel.applicationFilesPath + "\\CF\\Autorun.inf", installDrive + "\\Autorun.inf", true);

                }
            }
            catch(Exception ex)
            {
                Log.WriteLog("DriveIcon", ex.ToString());
            }
        }
        #region 对象方法
        public void AutoChooseWimIndex()
        {
            string tempindex = WTGModel.wimPart;
            AutoChooseWimIndex(ref tempindex, WTGModel.win7togo);
            WTGModel.wimPart = tempindex;
        }
        public void ImageApplyToUD()
        {
            ImageApply(WTGModel.isWimBoot, WTGModel.isEsd, imageX, imageFile, WTGModel.wimPart, WTGModel.ud, WTGModel.ud);
        }
        #endregion

    }
}
