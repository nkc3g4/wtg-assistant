//************************************************************************************************
// Copyright © 2010 Steven M. Cohn. All Rights Reserved.
//
//************************************************************************************************

namespace iTuner
{
    using System;
    using System.Text;


    /// <summary>
    /// Represents the displayable information for a single USB disk.
    /// </summary>

    public class UsbDisk:IEquatable<UsbDisk>
    {
        private const int KB = 1024;
        private const int MB = KB * 1024;
        private const int GB = MB * 1024;


        /// <summary>
        /// Initialize a new instance with the given values.
        /// </summary>
        /// <param name="name">The Windows drive letter assigned to this device.</param>

        public UsbDisk(string name)
        {
            this.Name = name;
            this.Model = String.Empty;
            Volume = String.Empty;
            Index = String.Empty;
            this.VolumeName = String.Empty;
            this.FreeSpace = 0;
            this.Size = 0;
            this.DriveType = String.Empty;
        }


        /// <summary>
        /// Gets the available free space on the disk, specified in bytes.
        /// </summary>

        public ulong FreeSpace
        {
            get;
            internal set;
        }

        public string DriveType
        {
            get;
            internal set;
        }
        /// <summary>
        /// Get the model of this disk.  This is the manufacturer's name.
        /// </summary>
        /// <remarks>
        /// When this class is used to identify a removed USB device, the Model
        /// property is set to String.Empty.
        /// </remarks>

        public string Model
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets the name of this disk.  This is the Windows identifier, drive letter.
        /// </summary>

        public string Name
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the total size of the volume, specified in bytes.
        /// </summary>

        public ulong Size
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the total size of the disk, specified in bytes.
        /// </summary>

        public ulong DiskSize
        {
            get;
            internal set;
        }
        /// <summary>
        /// Get the volume name of this disk.  This is the friently name ("Stick").
        /// </summary>
        /// <remarks>
        /// When this class is used to identify a removed USB device, the Volume
        /// property is set to String.Empty.
        /// </remarks>
        public string Index
        {
            get;
            internal set;
        }
        public string Volume
        {
            get;
            internal set;
        }
        public string VolumeName
        {
            get;
            internal set;
        }
        public ulong TotalSectors
        {
            get;
            internal set;
        }

        /// <summary>
        /// Pretty print the disk.
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {

            //System.Windows.Forms.MessageBox.Show(this.Model); System.Windows.Forms.MessageBox.Show("Test");
            if (this.Model == string.Empty)
            {
                //System.Windows.Forms.MessageBox.Show(this.Name);
                return this.Name;
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                //builder.Append(Name);
                //builder.Append(" ");
                //builder.Append(" (");
                builder.Append(Model);
                //builder.Append(") ");
                builder.Append(" ");
                builder.Append(FormatByteCount(Size));
                builder.Append(" (");
                builder.Append(DriveType);
                builder.Append(") ");
                //builder.Append(" (");
                builder.Append(Volume);
                //builder.Append(VolumeName);
                //builder.Append(") ");
                return builder.ToString();
            }
        }
        public void SetVolume(string volume)
        {
            Volume = volume;
        }


        private string FormatByteCount(ulong bytes)
        {
            string format = null;

            if (bytes < KB)
            {
                format = String.Format("{0} Bytes", bytes);
            }
            else if (bytes < MB)
            {
                bytes = bytes / KB;
                format = String.Format("{0} KB", bytes.ToString("N"));
            }
            else if (bytes < GB)
            {
                double dree = bytes / MB;
                format = String.Format("{0} MB", dree.ToString("N1"));
            }
            else
            {
                double gree = bytes / GB;
                format = String.Format("{0} GB", gree.ToString("N1"));
            }

            return format;
        }

        public bool Equals(UsbDisk other)
        {
            if (other is null)
                return false;

            return ToString() == other.ToString();
        }
        public override bool Equals(object obj) => Equals(obj as UsbDisk);
        public override int GetHashCode() => ToString().GetHashCode();

    }
}
