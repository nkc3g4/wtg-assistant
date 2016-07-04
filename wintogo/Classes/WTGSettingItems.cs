using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace wintogo
{
    public class UserSetWTGSettingItems
    {
        //public string ActPartition { get; set; }
        //private bool ntfsUefiSupport = false;
        ////[DisplayName(MsgManager.GetResString("Msg_NtfsUefiSupport"))]
        //[Description("启用在NTFS分区的UEFI支持"), Category("分区")]
        //public bool NtfsUefiSupport
        //{
        //    get { return ntfsUefiSupport; }
        //    set { ntfsUefiSupport = value; }
        //}
        private int efiPartitionSize = 350;

        [Description("设定EFI分区大小"), Category("分区")]
        public int EFIPartitionSize
        {
            get { return efiPartitionSize; }
            set { efiPartitionSize = value; }
        }

        private PartitionTableType vhdPartitionType = PartitionTableType.MBR;

        [Description("虚拟硬盘格式"), Category("虚拟硬盘")]
        public PartitionTableType VHDPartitionType
        {
            get { return vhdPartitionType; }
            set { vhdPartitionType = value; }
        }

        private string vhdNameWithoutExt = "win8";

        [Description("虚拟硬盘文件名"), Category("虚拟硬盘")]
        public string VHDNameWithoutExt
        {
            get { return vhdNameWithoutExt; }
            set { vhdNameWithoutExt = value; }
        }
        private bool fixLetter = true;
        [Description("修复盘符"), Category("系统")]

        public bool FixLetter
        {
            get { return fixLetter; }
            set { fixLetter = value; }
        }

        private string imageIndex = "0 : 自动选择";

        [TypeConverter(typeof(ImagePartNameConverter)), Description("安装的镜像分卷"), Category("镜像")]
        public string ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = value; }
        }
        //private List<TestClass> myVar = new List<TestClass>() { new TestClass(1) };

        //public List<TestClass> MyProperty
        //{
        //    get { return myVar = new List<TestClass>() { new TestClass(1) }; }
        //    set { myVar = new List<TestClass>() { new TestClass(1) } = value; }
        //}
        //private TestClass[] myVar = new TestClass[] { new TestClass(1) };

        //public TestClass[] MyProperty
        //{
        //    get { return myVar = new TestClass[] { new TestClass(1) }; }
        //    set { myVar = new TestClass[] { new TestClass(1) } = value; }
        //}

        //[TypeConverter(typeof(TestConverter))]
        //public TestClass MyProperty
        //{
        //    get;
        //    set;
        //}
        private string vhdTempPath = Path.GetTempPath();
        [Editor(typeof(PropertyGridFolderBrowserDialogItem),
typeof(System.Drawing.Design.UITypeEditor)), Description("虚拟硬盘临时目录"), Category("虚拟硬盘")]
        public string VHDTempPath
        {
            get { return vhdTempPath; }
            set { vhdTempPath = value; }
        }


        private bool disableWinRe = false;
        [Description("禁用Windows Recovery Environment"), Category("系统")]

        public bool DisableWinRe
        {
            get { return disableWinRe; }
            set { disableWinRe = value; }
        }
        private bool installDonet35;
        [Description("安装.NET3.5"), Category("系统")]
        public bool InstallDonet35
        {
            get { return installDonet35; }
            set { installDonet35 = value; }
        }
        private bool noDefaultDriveLetter;
        [Description("不自动分配盘符"), Category("分区")]
        public bool NoDefaultDriveLetter
        {
            get { return noDefaultDriveLetter; }
            set { noDefaultDriveLetter = value; }
        }


        private bool commonBootFiles;
        [Description("通用启动文件"), Category("系统")]
        public bool CommonBootFiles
        {
            get { return commonBootFiles; }
            set { commonBootFiles = value; }

        }
        private bool doNotFormat = false;
        [Description("不格式化移动磁盘,仅在非UEFI模式下有效"), Category("分区")]
        public bool DoNotFormat
        {
            get { return doNotFormat; }
            set { doNotFormat = value; }
        }

    }
    public enum PartitionTableType
    {
        GPT,
        MBR
    }
}

