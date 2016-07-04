using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace wintogo
{

    class CopyFileApi
    {
        public const short FILE_ATTRIBUTE_NORMAL = 0x80;
        public const short INVALID_HANDLE_VALUE = -1;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        [DllImport("kernel32", SetLastError = true,CallingConvention = CallingConvention.StdCall , CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(
     string lpFileName,
     uint dwDesiredAccess,
     uint dwShareMode,
     IntPtr SecurityAttributes,
     uint dwCreationDisposition,
     uint dwFlagsAndAttributes,
     IntPtr hTemplateFile
 );
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int GetFileSize(IntPtr hFile, int highSize);

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool ReadFile
   (
       System.IntPtr hFile,      // handle to file
       void* pBuffer,            // data buffer
       int NumberOfBytesToRead,  // number of bytes to read
       int* pNumberOfBytesRead,  // number of bytes read
       int Overlapped            // overlapped buffer
   );


        [DllImport("kernel32.dll")]
        static extern unsafe bool WriteFile(
            IntPtr hFile,
            void* lpBuffer,
    int nNumberOfBytesToWrite,
    int* lpNumberOfBytesWritten,
    int lpOverlapped);

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool CloseHandle
  (
      System.IntPtr hObject // handle to object
  );


        public static unsafe void CopyFile()
        {
            SafeFileHandle hSourceFile = CreateFile(@"\\.\\Harddisk2Partition1\\bootmgr", GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            SafeFileHandle hDestFile = CreateFile(@"\\.\\Harddisk2Partition2\\BCD", GENERIC_WRITE, 0, IntPtr.Zero, CREATE_NEW, 0, IntPtr.Zero);
            //System.Windows.Forms.MessageBox.Show(hSourceFile.ToString());
            if (hSourceFile.IsInvalid)
            {
                System.Windows.Forms.MessageBox.Show("Error hSourceFile.IsInvalid");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("OK");
            }
           
            if (hDestFile.IsInvalid)
            {
                System.Windows.Forms.MessageBox.Show("Error hDestFile.IsInvalid");

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("OK");
            }
            //if (hDestFile == IntPtr.Zero)
            //{
            //    System.Windows.Forms.MessageBox.Show("Error hDestFile.IsInvalid");

            //}
            //int dwRemainSize = GetFileSize(hSourceFile, 0); ;
            ////
            //System.Windows.Forms.MessageBox.Show(dwRemainSize.ToString());
            //byte[] buffer = new byte[1024];
            //while (dwRemainSize > 0)
            //{


            //    int dwActualRead = 0;
            //    fixed (byte* p = buffer)
            //    {
            //        ReadFile(hSourceFile, p, 1024, &dwActualRead, 0);
            //    }
            //    System.Windows.Forms.MessageBox.Show(dwActualRead.ToString());
            //    //ReadFile(hSourceFile.DangerousGetHandle(), buffer, 1024, &dwActualRead, 0);


            //    //System.Windows.Forms.MessageBox.Show(dwActualRead.ToString());


            //    dwRemainSize -= dwActualRead;
            //    int dwActualWrote = 0;
            //    while (dwActualWrote < dwActualRead)
            //    {
            //        int dwOnceWrote = 0;
            //        fixed (byte* p = buffer)
            //        {
            //            WriteFile(hDestFile, p + dwActualWrote, dwActualRead - dwActualWrote, &dwOnceWrote, 0);
            //        }
            //        dwActualWrote += dwOnceWrote;


            //    }

            //}
            //CloseHandle(hSourceFile);
            //CloseHandle(hDestFile);
            //System.Windows.Forms.MessageBox.Show("Test");



        }

    }
}
