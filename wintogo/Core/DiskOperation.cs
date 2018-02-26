using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;


namespace wintogo
{
    public static class DiskOperation
    {
        public static void CheckDiskExists(string disk)
        {
            int c = 0;
            while (!Directory.Exists(disk))
            {
                Thread.Sleep(50);
                if (++c > 100)
                {
                    break;
                }
            }
            Log.WriteLog("Info_CheckDiskExists", disk + "   " + c.ToString());
        }
        public static void SetNoDefaultDriveLetter(string uDisk)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select volume " + uDisk.Substring(0, 1));
            sb.AppendLine("attributes volume set nodefaultdriveletter");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();
            //attributes volume set nodefaultdriveletter
        }
        /// <summary>
        /// WTGOperation.diskpartscriptpath + @"\uefi.txt"
        /// </summary>
        /// <param name="efiSize"></param>
        /// <param name="uDisk"></param>
        /// <returns>return WTGOperation.diskpartscriptpath + "\\uefi.txt";</returns>
        public static void DiskPartGPTAndUEFI(string efiSize, string uDisk, string[] partitionSize)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("select volume " + uDisk.Substring(0, 1));
            sb.AppendLine("clean");
            sb.AppendLine("convert gpt");
            sb.AppendLine("create partition efi size " + efiSize);


            List<string> partitionList = new List<string>();
            for (int i = 0; i < partitionSize.Length; i++)
            {
                int partSize = 0;
                int.TryParse(partitionSize[i], out partSize);
                if (partSize == 0)
                {
                    continue;
                }
                partitionList.Add(partitionSize[i]);
            }

            for (int i = 0; i < partitionList.Count - 1; i++)
            {
                sb.AppendLine("create partition primary size " + partitionList[i]);
            }

            sb.AppendLine("create partition primary");

            sb.AppendLine("select partition 2");
            sb.AppendLine("format fs=fat quick");
            sb.AppendLine("assign letter=x");
            sb.AppendLine("select partition 3");
            sb.AppendLine("format fs=ntfs quick");
            sb.AppendLine("assign letter=" + uDisk.Substring(0, 1));
            for (int i = 0; i < partitionList.Count - 1; i++)
            {
                sb.AppendLine("select partition " + (i + 4).ToString());
                sb.AppendLine("format fs=ntfs quick");
                sb.AppendLine("assign");
            }
            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();
            int c = 0;
            while (!Directory.Exists(WTGModel.ud))
            {
                c++;
                Thread.Sleep(50);
                if (c > 100)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// MBR+UEFI脚本Write到WTGOperation.diskpartscriptpath + @"\uefimbr.txt
        /// </summary>
        /// <param name="efisize">efisize(MB)</param>
        /// <param name="ud">优盘盘符，":"、"\"不必须</param>
        public static void GenerateMBRAndUEFIScript(string efisize, string ud, string[] partitionSize)
        {
            StringBuilder sb = new StringBuilder();

            //using (FileStream fs0 = new FileStream(WTGModel.diskpartScriptPath + @"\uefimbr.txt", FileMode.Create, FileAccess.Write))
            //{
            //    fs0.SetLength(0);
            //    using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default))
            //    {
            //string ws0 = "";

            sb.AppendLine("select volume " + ud.Substring(0, 1));
            sb.AppendLine("clean");
            sb.AppendLine("convert mbr");
            sb.AppendLine("create partition primary size " + efisize);

            List<string> partitionList = new List<string>();
            for (int i = 0; i < partitionSize.Length; i++)
            {
                int partSize = 0;
                int.TryParse(partitionSize[i], out partSize);
                if (partSize == 0)
                {
                    continue;
                }
                partitionList.Add(partitionSize[i]);
            }

            for (int i = 0; i < partitionList.Count - 1; i++)
            {
                sb.AppendLine("create partition primary size " + partitionList[i]);
            }


            sb.AppendLine("create partition primary");

            //for (int i = 0; i < partitionSize.Length - 1; i++)
            //{
            //    if (partitionSize[i] == "0")
            //    {
            //        continue;
            //    }
            //    sb.AppendLine("create partition primary size " + partitionSize[i]);
            //    partitionsCount++;
            //}
            //if (partitionSize[2] != "0")
            //{
            //    sb.AppendLine("create partition primary");
            //    partitionsCount++;
            //}
            sb.AppendLine("select partition 1");
            sb.AppendLine("format fs=fat quick");
            sb.AppendLine("active");
            sb.AppendLine("assign letter=x");
            sb.AppendLine("select partition 2");
            sb.AppendLine("format fs=ntfs quick");
            sb.AppendLine("assign letter=" + ud.Substring(0, 1));
            for (int i = 0; i < partitionList.Count - 1; i++)
            {
                sb.AppendLine("select partition " + (i + 3).ToString());
                sb.AppendLine("format fs=ntfs quick");
                sb.AppendLine("assign");
            }
            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();
            //    }

            //}
            //return WTGModel.diskpartScriptPath + @"\uefimbr.txt";
        }

        public static void DiskPartRePartitionUD(string[] partitionSize)
        {
            //int partitionsCount = 0;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select volume " + WTGModel.ud.Substring(0, 1));
            sb.AppendLine("clean");
            sb.AppendLine("convert mbr");
            List<string> partitionList = new List<string>();

            for (int i = 0; i < partitionSize.Length; i++)
            {
                int partSize = 0;
                int.TryParse(partitionSize[i], out partSize);
                if (partSize == 0)
                {
                    continue;
                }
                partitionList.Add(partitionSize[i]);
            }

            for (int i = 0; i < partitionList.Count - 1; i++)
            {
                sb.AppendLine("create partition primary size " + partitionList[i]);
            }

            sb.AppendLine("create partition primary");


            //if (partitionSize[partitionSize.Length - 1] != "0")
            //{
            //    sb.AppendLine("create partition primary");
            //    partitionsCount++;
            //}
            //if (partitionsCount == 0)
            //{
            //    sb.AppendLine("create partition primary");
            //}
            sb.AppendLine("select partition 1");
            sb.AppendLine("format fs=ntfs quick");
            sb.AppendLine("active");
            sb.AppendLine("assign letter=" + WTGModel.ud.Substring(0, 1));
            for (int i = 0; i < partitionList.Count - 1; i++)
            {
                sb.AppendLine("select partition " + (i + 2).ToString());
                sb.AppendLine("format fs=ntfs quick");
                sb.AppendLine("assign");
            }

            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();
            int c = 0;
            while (!Directory.Exists(WTGModel.ud))
            {
                c++;
                Thread.Sleep(50);
                if (c > 100)
                {
                    break;
                }
            }

        }

        /// <summary>
        ///  Gets the total size of storage space on a drive, in bytes.
        /// </summary>
        /// <param name="str_HardDiskName"></param>
        /// <returns></returns>
        public static long GetHardDiskSpace(string str_HardDiskName)
        {
            long totalSize = new long();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    totalSize = drive.TotalSize;

                }
            }
            return totalSize;
        }
        /// <summary>
        ///  Gets the total amount of free space available on a drive, in bytes.
        /// </summary>
        /// <param name="str_HardDiskName"></param>
        /// <returns></returns>
        public static long GetHardDiskFreeSpace(string str_HardDiskName)
        {
            long totalSize = new long();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    totalSize = drive.TotalFreeSpace;

                }
            }
            return totalSize;
        }
    }
}
