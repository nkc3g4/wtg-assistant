using iTuner;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace wintogo
{
    public class WTGModel
    {
        public static  string[] partitionSize;
        //public static string partitionSize1;
        //public static string partitionSize2;
        //public static string partitionSize3;
        public static bool doNotFormat;
        public static bool noDefaultDriveLetter;
        public static bool installDonet35;
        public static bool disableWinRe;
        public static string vhdTempPath;
        //public static string imageIndex;
        public static bool fixLetter;
        public static string vhdNameWithoutExt;
        public static string vhdPartitionType;
        public static string efiPartitionSize;
        public static UsbDisk UdObj;
        /// <summary>
        /// UserSetWTGSettingItems();
        /// </summary>
        //public static UserSetWTGSettingItems userSettings = new UserSetWTGSettingItems();
        //public static List<string> imagePartNames = new List<string>();
      
        /// <summary>
        /// 可使用ESD文件
        /// </summary>
        public static bool allowEsd = false;
        /// <summary>
        /// 优盘盘符
        /// </summary>
        public static string ud;
        /// <summary>
        /// 显示在ComboBox中的字符串信息
        /// </summary>
        public static string udString;
        public static bool isWimBoot;
        //public static bool isFramework;
        public static bool isBlockLocalDisk;
        //public static bool isDiswinre;
        /// <summary>
        /// 镜像文件路径
        /// </summary>
        public static string imageFilePath;
        /// <summary>
        /// WimIndex
        /// </summary>
        public static string wimPart = "0";

        public static bool isEsd = false;
        /// <summary>
        /// 默认为imagex_x86.exe
        /// </summary>
        public static string imagexFileName = "imagex_x86.exe";
        /// <summary>
        /// bcdboot文件名
        /// </summary>
        public static string bcdbootFileName;
        ///// <summary>
        ///// 强制格式化
        ///// </summary>
        //public static bool forceFormat = false;
        /// <summary>
        /// VHD文件用户设定大小
        /// </summary>
        public static int userSetSize;
        public static bool isFixedVHD;
        public static int win7togo;
        /// <summary>
        /// Application.StartupPath + "\\logs";
        /// </summary>
        public static string diskpartScriptPath = Path.GetTempPath();
        public static bool isUefiGpt;
        public static bool isUefiMbr;
        public static bool isNoTemp;
        public static bool isLegacyUdiskUefi;
        //public static bool isBlockLocalDisk;
        public static bool ntfsUefiSupport;
        /// <summary>
        ///  WTGOperation.filetype = Path.GetExtension(openFileDialog1.FileName.ToLower()).Substring(1);
        /// </summary>
        public static string choosedImageType;
        /// <summary>
        /// 是否勾选通用启动文件
        /// </summary>
        //public static bool commonBootFiles;
        /// <summary>
        /// Path.GetTempPath();
        /// </summary>
        //public static string scriptTempPath = Path.GetTempPath();
        /// <summary>
        /// win8.vhd
        /// </summary>
        public static string win8VHDFileName = "win8.vhd";
        /// <summary>
        /// Path.GetTempPath() + "\\WTGA";
        /// </summary>
        public static string applicationFilesPath = StringUtility.Combine(Path.GetTempPath(), "WTGA");
        /// <summary>
        /// Application.StartupPath + "\\logs";
        /// </summary>
        public static string logPath = Application.StartupPath + "\\logs";
        /// <summary>
        /// VHD OR VHDX
        /// </summary>
        public static string vhdExtension = "vhd";
        public static bool isCompactOS;
        public static bool isBitlocker;
        public static OS CurrentOS;
        public static ApplyMode CheckedMode;
        public static string CreateGuid;
        public static bool disableUasp;
        public static bool isUserSetEfiPartition;
        public static string efiPartition = string.Empty;
    }
    public enum OS
    {
        XP,
        Vista,
        Win7,
        Win8_without_update,
        Win8_1_with_update,
        Win10,
        Other
    }
    public enum ApplyMode
    {
        Legacy,
        VHD,
        VHDX
    }
}
