using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;

namespace wintogo
{
    public enum FirmwareType
    {
        BIOS,
        UEFI,
        ALL
    }
    public static class BootFileOperation
    {
        ///// <summary>
        ///// windows  /s  x: /f ALL
        ///// </summary>
        ///// <param name="bcdboot"></param>
        ///// <param name="ud"></param>
        //public static void BcdbootWriteALLBootFileToXAndAct(string bcdboot,string ud) 
        //{
        //    ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, ud + "windows  /s  x: /f ALL");
        //    System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(WTGOperation.applicationFilesPath+ "\\bootice.exe", " /DEVICE=x: /partitions /activate  /quiet");
        //    p2.WaitForExit();
        //}
        public static void BooticeWriteMBRPBRAndAct(string targetDisk)
        {
            BooticeMbr(targetDisk);
            BooticePbr(targetDisk);
            BooticeAct(targetDisk);
        }
        //public static void BooticeWritePbrAndAct
        public static void BooticeMbr(string targetDisk)
        {
            Process booice = Process.Start(WTGModel.applicationFilesPath + "\\BOOTICE.exe", (" /DEVICE=" + targetDisk.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
            booice.WaitForExit();
        }
        public static void BooticePbr(string targetDisk)
        {
            Process pbr = Process.Start(WTGModel.applicationFilesPath + "\\BOOTICE.exe", (" /DEVICE=" + targetDisk.Substring(0, 2) + " /pbr /install /type=bootmgr /quiet"));//写入引导
            pbr.WaitForExit();
        }
        public static void BooticeAct(string targetDisk)
        {
            Process act = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", " /DEVICE=" + targetDisk.Substring(0, 2) + " /partitions /activate /quiet");
            act.WaitForExit();

        }
        ///// <summary>
        ///// /f ALL参数
        ///// </summary>
        ///// <param name="sourceDisk">指定 windows 系统根目录</param>
        ///// <param name="targetDisk">该参数用于指定要将启动环境文件复制到哪个目标系统分区。</param>
        ///// <param name="bcdboot">bcdboot文件名，默认传bcdboot字段</param>
        //public static void BcdbootWriteALLBootFile(string sourceDisk,string targetDisk,string bcdboot) 
        //{
        //    ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, sourceDisk + "windows  /s  " + targetDisk.Substring(0, 2) + " /f BIOS");
        //}
        ///// <summary>
        ///// /f UEFI参数
        ///// </summary>
        ///// <param name="sourceDisk">指定 windows 系统根目录</param>
        ///// <param name="targetDisk">该参数用于指定要将启动环境文件复制到哪个目标系统分区。</param>
        ///// <param name="bcdboot">bcdboot文件名，默认传bcdboot字段</param>
        //public static void BcdbootWriteUEFIBootFile(string sourceDisk, string targetDisk, string bcdboot)
        //{
        //    ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, sourceDisk + "windows  /s  " + targetDisk.Substring(0, 2) + " /f UEFI");
        //}

        public static void BcdeditFixBootFileTypical(string BCDDisk, string osdevice, FirmwareType fwType)
        {
            int c = 0;
            while (Process.GetProcessesByName("bcdboot.exe").Length != 0)
            {
                Thread.Sleep(50);
                if (++c > 100)
                {
                    break;
                }
            }
            string BCDPath = "\\Boot\\BCD";
            if (fwType == FirmwareType.UEFI)
            {
                BCDPath = "\\EFI\\Microsoft\\Boot\\BCD";
            }
            if (!File.Exists(BCDDisk + BCDPath))
            {
                return;
            }
            StringBuilder args = new StringBuilder();
            args.Append("/store ");
            args.Append(BCDDisk.Substring(0, 2) + BCDPath);
            args.Append(" /set {bootmgr} device partition=");
            args.Append(BCDDisk.Substring(0, 2));
            ////执行一次
            ProcessManager.ECMD("bcdedit.exe", args.ToString());

            args.Clear();
            args.Append("/store ");
            args.Append(BCDDisk.Substring(0, 2) + BCDPath);
            args.Append(" /set {default} device partition=");
            args.Append(osdevice.Substring(0, 2));
            ////执行一次
            ProcessManager.ECMD("bcdedit.exe", args.ToString());
            args.Clear();
            args.Append("/store ");
            args.Append(BCDDisk.Substring(0, 2) + BCDPath);
            args.Append(" /set {default} osdevice partition=");
            args.Append(osdevice.Substring(0, 2));
            ProcessManager.ECMD("bcdedit.exe", args.ToString());

        }
        public static void BcdeditFixBootFileVHD(string BCDDisk, string osdevice, string VHDFileNameWithExt, FirmwareType fwType)
        {
            int c = 0;
            while (Process.GetProcessesByName("bcdboot.exe").Length != 0)
            {
                Thread.Sleep(50);
                if (++c > 100)
                {
                    break;
                }
            }
            string BCDPath = "\\Boot\\BCD";
            if (fwType == FirmwareType.UEFI)
            {
                BCDPath = "\\EFI\\Microsoft\\Boot\\BCD";
            }
            if (!File.Exists(BCDDisk + BCDPath))
            {
                return;
            }

            StringBuilder args = new StringBuilder();

            args.Append("/store ");
            args.Append(BCDDisk.Substring(0, 2) + BCDPath);
            //if (BCDDisk.Substring(0, 2) == osdevice.Substring(0, 2) && fwType == FirmwareType.BIOS)
            //{
            //    args.Append(" /set {bootmgr} device boot");
            //}
            //else
            //{
            args.Append(" /set {bootmgr} device  partition=" + BCDDisk.Substring(0, 2));
            //}
            ////执行一次
            ProcessManager.ECMD("bcdedit.exe", args.ToString());

            args.Clear();
            args.Append("/store ");
            args.Append(BCDDisk.Substring(0, 2) + BCDPath);
            //if (BCDDisk.Substring(0, 2) == osdevice.Substring(0, 2) && fwType == FirmwareType.BIOS)
            //{
            //    args.Append(" /set {default} device  vhd=[locate]\\" + VHDFileNameWithExt);
            //}
            //else
            //{
            args.Append(" /set {default} device  vhd=[" + osdevice.Substring(0, 2) + "]\\" + VHDFileNameWithExt);
            //}
            ////执行一次
            ProcessManager.ECMD("bcdedit.exe", args.ToString());
            args.Clear();
            args.Append("/store ");
            args.Append(BCDDisk.Substring(0, 2) + BCDPath);
            //if (BCDDisk.Substring(0, 2) == osdevice.Substring(0, 2) && fwType == FirmwareType.BIOS)
            //{
            //    args.Append(" /set {default} osdevice  vhd=[locate]\\" + VHDFileNameWithExt);
            //}
            //else
            //{
            args.Append(" /set {default} osdevice  vhd=[" + osdevice.Substring(0, 2) + "]\\" + VHDFileNameWithExt);
            //}
            //args.Append(" /set {default} osdevice  vhd=[" + osdevice.Substring(0, 2) + "]\\" + VHDFileNameWithExt);
            ProcessManager.ECMD("bcdedit.exe", args.ToString());

        }

        /// <summary>
        /// BCDBOOT写入引导文件
        /// </summary>
        /// <param name="sourceDisk">例如E:\</param>
        /// <param name="targetDisk">例如E:</param>
        /// <param name="bcdbootFileName">例如bcdboot.exe</param>
        /// <param name="fwType"></param>
        public static void BcdbootWriteBootFile(string sourceDisk, string targetDisk, FirmwareType fwType)
        {

            StringBuilder args = new StringBuilder();
            args.Append(sourceDisk);
            args.Append("windows /s ");
            args.Append(targetDisk.Substring(0, 2));

            if (fwType == FirmwareType.ALL)
            {
                args.Append(" /f all ");
            }
            else if (fwType == FirmwareType.BIOS)
            {
                args.Append(" /f bios ");
            }
            else
            {
                args.Append(" /f uefi ");
            }

            args.Append(" /l zh-CN ");
            args.Append(" /v ");

            if (WTGModel.CurrentOS == OS.Win7)
            {
                ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\bcdboot.exe", args.ToString());
            }
            else
            {
                ProcessManager.ECMD("bcdboot.exe", args.ToString());
            }

            //}
        }
    }
}
