//************************************************************************************************
// Copyright © 2010 Steven M. Cohn. All Rights Reserved.
//
//************************************************************************************************

namespace iTuner
{
    using System;
    using System.Collections.Generic;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;


    /// <summary>
    /// Discover USB disk devices and monitor for device state changes.
    /// </summary>

    public class UsbManager : IDisposable
    {

        #region DriverWindow

        /// <summary>
        /// A native window used to monitor all device activity.
        /// </summary>

        private class DriverWindow : NativeWindow, IDisposable
        {
            // Contains information about a logical volume.
            [StructLayout(LayoutKind.Sequential)]
            public struct DEV_BROADCAST_VOLUME
            {
                public int dbcv_size;           // size of the struct
                public int dbcv_devicetype;     // DBT_DEVTYP_VOLUME
                public int dbcv_reserved;       // reserved; do not use
                public int dbcv_unitmask;       // Bit 0=A, bit 1=B, and so on (bitmask)
                public short dbcv_flags;        // DBTF_MEDIA=0x01, DBTF_NET=0x02 (bitmask)
            }


            private const int WM_DEVICECHANGE = 0x0219;             // device state change
            private const int DBT_DEVICEARRIVAL = 0x8000;           // detected a new device
            private const int DBT_DEVICEQUERYREMOVE = 0x8001;       // preparing to remove
            private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;    // removed 
            private const int DBT_DEVTYP_VOLUME = 0x00000002;       // logical volume


            public DriverWindow()
            {
                // create a generic window with no class name
                base.CreateHandle(new CreateParams());
            }


            public void Dispose()
            {
                base.DestroyHandle();
                GC.SuppressFinalize(this);
            }


            public event UsbStateChangedEventHandler StateChanged;


            protected override void WndProc(ref Message message)
            {
                base.WndProc(ref message);

                if ((message.Msg == WM_DEVICECHANGE) && (message.LParam != IntPtr.Zero))
                {
                    DEV_BROADCAST_VOLUME volume = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(
                        message.LParam, typeof(DEV_BROADCAST_VOLUME));

                    if (volume.dbcv_devicetype == DBT_DEVTYP_VOLUME)
                    {
                        switch (message.WParam.ToInt32())
                        {
                            case DBT_DEVICEARRIVAL:
                                SignalDeviceChange(UsbStateChange.Added, volume);
                                break;

                            case DBT_DEVICEQUERYREMOVE:
                                // can intercept
                                break;

                            case DBT_DEVICEREMOVECOMPLETE:
                                SignalDeviceChange(UsbStateChange.Removed, volume);
                                break;
                        }
                    }
                }
            }


            private void SignalDeviceChange(UsbStateChange state, DEV_BROADCAST_VOLUME volume)
            {
                string name = ToUnitName(volume.dbcv_unitmask);

                if (StateChanged != null)
                {
                    UsbDisk disk = new UsbDisk(name);
                    StateChanged(new UsbStateChangedEventArgs(state, disk));
                }
            }


            /// <summary>
            /// Translate the dbcv_unitmask bitmask to a drive letter by finding the first
            /// enabled low-order bit; its offset equals the letter where offset 0 is 'A'.
            /// </summary>
            /// <param name="mask"></param>
            /// <returns></returns>

            private string ToUnitName(int mask)
            {
                int offset = 0;
                while ((offset < 26) && ((mask & 0x00000001) == 0))
                {
                    mask = mask >> 1;
                    offset++;
                }

                if (offset < 26)
                {
                    return String.Format("{0}:", Convert.ToChar(Convert.ToInt32('A') + offset));
                }

                return "?:";
            }
        }

        #endregion WndProc Driver


        private delegate void GetDiskInformationDelegate(UsbDisk disk);


        private DriverWindow window;
        private UsbStateChangedEventHandler handler;
        private bool isDisposed;


        //========================================================================================
        // Constructor
        //========================================================================================

        /// <summary>
        /// Initialize a new instance.
        /// </summary>

        public UsbManager()
        {
            this.window = null;
            this.handler = null;
            this.isDisposed = false;
        }


        #region Lifecycle

        /// <summary>
        /// Destructor.
        /// </summary>

        ~UsbManager()
        {
            Dispose();
        }


        /// <summary>
        /// Must shutdown the driver window.
        /// </summary>

        public void Dispose()
        {
            if (!isDisposed)
            {
                if (window != null)
                {
                    window.StateChanged -= new UsbStateChangedEventHandler(DoStateChanged);
                    window.Dispose();
                    window = null;
                }

                isDisposed = true;

                GC.SuppressFinalize(this);
            }
        }

        #endregion Lifecycle


        //========================================================================================
        // Events/Properties
        //========================================================================================

        /// <summary>
        /// Add or remove a handler to listen for USB disk drive state changes.
        /// </summary>

        public event UsbStateChangedEventHandler StateChanged
        {
            add
            {
                if (window == null)
                {
                    // create the driver window once a consumer registers for notifications
                    window = new DriverWindow();
                    window.StateChanged += new UsbStateChangedEventHandler(DoStateChanged);
                }

                handler = (UsbStateChangedEventHandler)Delegate.Combine(handler, value);
            }

            remove
            {
                handler = (UsbStateChangedEventHandler)Delegate.Remove(handler, value);

                if (handler == null)
                {
                    // destroy the driver window once the consumer stops listening
                    window.StateChanged -= new UsbStateChangedEventHandler(DoStateChanged);
                    window.Dispose();
                    window = null;
                }
            }
        }


        //========================================================================================
        // Methods
        //========================================================================================
        public List<string> GetVolumns(int index,string model)
        {
            List<string> vols = new List<string>();
            foreach (ManagementObject drive in
              new ManagementObjectSearcher(
                  "select DeviceID,Model,Size,Index,MediaType,Size,InterfaceType from Win32_DiskDrive where Index="+ index).Get())
            {
                if (drive["Model"].ToString() != model)
                    continue;
                try
                {
                    // associate physical disks with partitions
                    foreach (ManagementObject partition in new ManagementObjectSearcher(string.Format(
                        "associators of {{Win32_DiskDrive.DeviceID='{0}'}} where AssocClass = Win32_DiskDriveToDiskPartition",
                        drive["DeviceID"])).Get())
                    {
                        try
                        {
                            if (partition != null)
                            {
                                // associate partitions with logical disks (drive letter volumes)
                                ManagementObject logical = new ManagementObjectSearcher(string.Format(
                                    "associators of {{Win32_DiskPartition.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition",
                                    partition["DeviceID"])).First();
                                //MessageBox.Show(logical.ToString());
                                if (logical != null)
                                {
                                    // finally find the logical disk entry to determine the volume name
                                    ManagementObject volume = new ManagementObjectSearcher(string.Format(
                                        "select FreeSpace, Size, VolumeName, DriveType from Win32_LogicalDisk where Name='{0}'",
                                        logical["Name"])).First();
                                    vols.Add(logical["Name"].ToString());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            //disks.Add(new UsbDisk(ex.ToString()));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //MessageBox.Show(e.ToString());
                }
            }
            return vols;

        }





        /// <summary>
        /// Gets a collection of all available USB disk drives currently mounted.
        /// </summary>
        /// <returns>
        /// A UsbDiskCollection containing the USB disk drives.
        /// </returns>

        public UsbDiskCollection GetAvailableDisks()
        {
            UsbDiskCollection disks = new UsbDiskCollection();

            // browse all USB WMI physical disks
            foreach (ManagementObject drive in
                new ManagementObjectSearcher(
                    "select DeviceID,Model,Size,Index,MediaType,Size,InterfaceType from Win32_DiskDrive").Get())
            {
                try
                {
                    if (drive["InterfaceType"].ToString() != "USB" && drive["InterfaceType"].ToString() != "SCSI") continue;
                    if (drive["Model"].ToString().Contains("APPLE SD")) continue;
                    UsbDisk disk = new UsbDisk(drive["DeviceID"].ToString());
                    
                    disk.Model = drive["Model"].ToString();
                    disk.Index = drive["Index"].ToString();
                    disk.DiskSize = (ulong)drive["Size"];
                    //MessageBox.Show(drive["MediaType"].ToString() + " " + drive["InterfaceType"].ToString());
                    //disk.DriveType = (drive["MediaType"].ToString() .Contains("Removable")) ? "可移动磁盘" : "本地磁盘";
                    disk.DriveType = drive["MediaType"].ToString();
                    disk.Size = (ulong)drive["Size"];
                    bool hasAdded = false;
                    // associate physical disks with partitions
                    foreach (ManagementObject partition in new ManagementObjectSearcher(string.Format(
                        "associators of {{Win32_DiskDrive.DeviceID='{0}'}} where AssocClass = Win32_DiskDriveToDiskPartition",
                        drive["DeviceID"])).Get())
                    {
                        try
                        {
                            if (partition != null)
                            {
                                // associate partitions with logical disks (drive letter volumes)
                                ManagementObject logical = new ManagementObjectSearcher(string.Format(
                                    "associators of {{Win32_DiskPartition.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition",
                                    partition["DeviceID"])).First();
                                //MessageBox.Show(logical.ToString());
                                if (logical != null)
                                {
                                    // finally find the logical disk entry to determine the volume name
                                    ManagementObject volume = new ManagementObjectSearcher(string.Format(
                                        "select FreeSpace, Size, VolumeName, DriveType from Win32_LogicalDisk where Name='{0}'",
                                        logical["Name"])).First();

                                    if (logical["Name"].ToString().Contains("X"))
                                        continue;
                                    disk.Volume = logical["Name"].ToString();
                                    //disk.VolumeName = volume["VolumeName"].ToString();
                                    disk.FreeSpace = (ulong)volume["FreeSpace"];

                                    //disk.DriveType = Drivetypeconvert(int.Parse(volume["DriveType"].ToString()));
                                    //disk.TotalSectors = (ulong)drive["TotalSectors"];
                                    disks.Add(disk);
                                    hasAdded = true;
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            //disks.Add(new UsbDisk(ex.ToString()));
                        }
                    }
                    if (!hasAdded) disks.Add(disk);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //MessageBox.Show(e.ToString());
                }
            }

            return disks;
        }


        /// <summary>
        /// Internally handle state changes and notify listeners.
        /// </summary>
        /// <param name="e"></param>

        private void DoStateChanged(UsbStateChangedEventArgs e)
        {
            if (handler != null)
            {
                UsbDisk disk = e.Disk;

                // we can only interrogate drives that are added...
                // cannot see something that is no longer there!

                if ((e.State == UsbStateChange.Added) && (e.Disk.Name[0] != '?'))
                {
                    // the following Begin/End invokes looks strange but are required
                    // to resolve a "DisconnectedContext was detected" exception which
                    // occurs when the current thread terminates before the WMI queries
                    // can complete.  I'm not exactly sure why that would happen...

                    GetDiskInformationDelegate gdi = new GetDiskInformationDelegate(GetDiskInformation);
                    IAsyncResult result = gdi.BeginInvoke(e.Disk, null, null);
                    gdi.EndInvoke(result);
                }

                handler(e);
            }
        }


        /// <summary>
        /// Populate the missing properties of the given disk before sending to listeners
        /// </summary>
        /// <param name="disk"></param>

        private void GetDiskInformation(UsbDisk disk)
        {
            ManagementObject partition = new ManagementObjectSearcher(String.Format(
                "associators of {{Win32_LogicalDisk.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition",
                disk.Name)).First();

            if (partition != null)
            {
                ManagementObject drive = new ManagementObjectSearcher(String.Format(
                    "associators of {{Win32_DiskPartition.DeviceID='{0}'}}  where resultClass = Win32_DiskDrive",
                    partition["DeviceID"])).First();
                //MessageBox.Show(drive.ToString());

                if (drive != null)
                {
                    disk.Model = drive["Model"].ToString();
                }

                ManagementObject volume = new ManagementObjectSearcher(String.Format(
                    "select FreeSpace, Size, VolumeName from Win32_LogicalDisk where Name='{0}'",
                    disk.Name)).First();

                if (volume != null)
                {
                    disk.VolumeName = volume["VolumeName"].ToString();
                    disk.FreeSpace = (ulong)volume["FreeSpace"];
                    disk.Size = (ulong)volume["Size"];
                    disk.DriveType = Drivetypeconvert(Int32.Parse(volume["DriveType"].ToString()));
                    //MessageBox.Show(disk.DriveType);
                }
            }
        }
        private string Drivetypeconvert(int code)
        {
            switch (code)
            {
                case 0:
                    return "Unknown";
                case 1:
                    return "No Root Directory";
                case 2:
                    return "Removable Disk";
                case 3:
                    return "Local Disk";
                case 4:
                    return "Network Drive";
                case 5:
                    return "Compact Disc";
                case 6:
                    return "RAM Disk";
                default:
                    return "Type Error";
            }
        }
    }
    public class USBAudioDeviceItems : Dictionary<string, string>
    {
        protected USBAudioDeviceItems()
        {

        }
        protected USBAudioDeviceItems(IDictionary<string, string> dictionary)
            : base(dictionary)
        {

        }
        private readonly static object devices = new object();
        private static volatile USBAudioDeviceItems instane = null;
        public static USBAudioDeviceItems USBDevices
        {
            get
            {
                System.Threading.Mutex mutex = new System.Threading.Mutex();
                mutex.WaitOne();
                if (instane == null)  //双检查
                {
                    lock (devices)
                    {
                        if (instane == null)
                        {
                            instane = new USBAudioDeviceItems();
                            ManagementObjectSearcher mo_search1 = new ManagementObjectSearcher("Select * From Win32_SoundDevice");
                            ManagementObjectSearcher mo_search2 = new ManagementObjectSearcher("Select * From CIM_USBDevice");
                            foreach (ManagementObject sound_info in mo_search1.Get())
                            {
                                if (sound_info["Name"] == null) continue;
                                if (sound_info["DeviceID"] == null) continue;
                                string sound_device_name = sound_info["Name"].ToString();
                                string sound_device_id = sound_info["DeviceID"].ToString();
                                if (sound_device_id.Split('\\').Length != 3) continue;
                                if (sound_device_id.Split('\\')[1].Split('&').Length != 3) continue;

                                foreach (ManagementObject usb_info in mo_search2.Get())
                                {
                                    if (usb_info["Name"] == null) continue;
                                    if (usb_info["DeviceID"] == null) continue;
                                    string usb_device_name = usb_info["Name"].ToString();
                                    if (!usb_device_name.ToLower().Contains("usb composite device")) continue;
                                    string usb_device_id = usb_info["DeviceID"].ToString();
                                    if (usb_device_id.Split('\\').Length != 3) continue;
                                    if (usb_device_id.Split('\\')[1].Split('&').Length != 2) continue;

                                    if (sound_device_id.Split('\\')[1].Split('&')[0] == usb_device_id.Split('\\')[1].Split('&')[0]
                                    && sound_device_id.Split('\\')[1].Split('&')[1] == usb_device_id.Split('\\')[1].Split('&')[1])
                                    {
                                        string vid = sound_device_id.Split('\\')[1].Split('&')[0];
                                        string pid = sound_device_id.Split('\\')[1].Split('&')[1];
                                        //MessageBox.Show("", sound_device_name + "|" + usb_device_name);
                                        instane.Add(sound_device_name, sound_device_id);
                                    }

                                }
                            }
                        }
                    }
                }
                mutex.Close();
                return instane;

            }
        }
    }

}