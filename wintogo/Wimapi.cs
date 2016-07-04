using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace Microsoft.WimgApi
{
    ///<summary>
    ///Public interface to a WindowsImage object.
    ///</summary>
    public
    interface IImage
    {
        ///<summary>
        ///Gets image information from within a .wim file.
        ///</summary>
        XmlTextReader ImageInformation
        {
            get;
        }

        ///<summary>
        ///Sets image information about an image within a .wim file.
        ///</summary>
        ///<param name="imageInformation">The string being passed in should be in the form of a unicode XML file.
        ///Calling this function replaces any customized image data. To preserve existing XML information, call ImageInformation
        ///and append or edit the desired data.
        ///</param>
        void SetImageInformation(
            string imageInformation
        );

        ///<summary>
        ///Mounts an image in a .wim file to the specified directory.
        ///</summary>
        void Mount(
            string pathToMountTo
        );

        ///<summary>
        ///Retrieves the path to which an image has been mounted.
        ///</summary>
        string MountedPath
        {
            get;
        }

        ///<summary>
        ///Unmounts a mounted image in a .wim file from the specified directory.
        ///</summary>
        ///<param name="commitChanges">Indicates whether changes (if any) to the .wim file should be committed 
        ///before unmounting the .wim file. This flag will have no effect if the .wim file was mounted not to allow edits.
        ///</param>
        void DismountImage(
            bool commitChanges
        );

        ///<summary>
        ///Applies an image to a drive root or to a directory path from a .wim file.
        ///</summary>
        void Apply(
            string pathToApplyTo
        );
    }

    ///<summary>
    ///Class representing a .wim file.
    ///</summary>
    public
    sealed
    class
    WindowsImageContainer : IDisposable
    {
        ///<summary>
        ///Specifies the type of access to the .wim file.
        ///</summary>
        public
        enum
        CreateFileAccess
        {
            ///<summary>
            ///Specifies read-only access to the .wim file.
            ///</summary>
            Read,

            ///<summary>
            ///Specifies write access to the .wim file.
            ///Includes WIM_GENERIC_READ access to enable the apply and append operations to be used with existing images.
            ///</summary>
            Write
        }

        ///<summary>
        ///Specifies which action to take on files that exist and 
        ///which action to take when files do not exist.
        ///</summary>
        public
        enum
        CreateFileMode
        {
            ///<summary>
            ///RESERVED, DO NOT USE!
            ///</summary>
            None = 0,

            ///<summary>
            ///Creates a new .wim file. The function fails if the specified file already exists.
            ///</summary>
            CreateNew = 1,

            ///<summary>
            ///Creates a new .wim file. If the file exists, the function overwrites the file.
            ///</summary>
            CreateAlways = 2,

            ///<summary>
            ///Opens the .wim file. The function fails if the file does not exist.
            ///</summary>
            OpenExisting = 3,

            ///<summary>
            ///Opens the .wim file if it exists. If the file does not exist and the caller requests WIM_GENERIC_WRITE access, the 
            ///function creates the file.
            ///</summary>
            OpenAlways = 4
        }

        ///<summary>
        ///Public constructor to create a WIM object
        ///</summary>
        ///<param name="imageFilePath">Path of .wim file to create or to open.</param>
        ///<param name="mode">Specifies Open, Create, Create/Open disposition of the .wim file.</param>
        ///<param name="access">Specifies access level of Read Only or Write.</param>
        //[CLSCompliant(false)]
        public WindowsImageContainer(string imageFilePath, CreateFileMode mode, CreateFileAccess access)
        {
            CreateFileAccessPrivate fileAccess = GetMappedFileAccess(access);
            if (fileAccess == CreateFileAccessPrivate.Read && (!File.Exists(imageFilePath) || (CreateFileMode.OpenExisting != mode)))
            {
                throw new System.UnauthorizedAccessException(string.Format(CultureInfo.CurrentCulture,
                                 "Read access can be specified only with OpenExisting mode or OpenAlways mode when the .wim file does not exist."));
            }

            //
            //Imaging DLLs must be in the same directory.
            //
            try
            {
                m_ImageContainerHandle = NativeMethods.CreateFile(imageFilePath, (uint)fileAccess, (uint)mode);
                m_WindowsImageFilePath = imageFilePath;
            }
            catch (System.DllNotFoundException ex)
            {
                throw new System.DllNotFoundException(string.Format(CultureInfo.CurrentCulture,
                                  "Unable to load WIM libraries. Make sure the correct DLLs are present (Wimgapi.dll and Xmlrw.dll)."), ex.InnerException);
            }

            if (!m_ImageContainerHandle.Equals(IntPtr.Zero))
            {
                //
                //Set the temporary path so that we can write to an image. This
                //cannot be %TEMP% as it does not exist on Windows PE
                //
                string tempDirectory = System.Environment.GetEnvironmentVariable("systemdrive");
                NativeMethods.SetTemporaryPath(m_ImageContainerHandle, tempDirectory);

            }
            else
            {
                //
                //Throw an exception
                //
                throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                         "Unable to open  the .wim file {0}.", imageFilePath));
            }

            //
            //Finally, we must hook into the events.
            //
            m_MessageCallback = new NativeMethods.MessageCallback(ImageEventMessagePump);
            NativeMethods.RegisterCallback(m_MessageCallback);
        }

        ///<summary> Destructor to close the open handles.</summary>
        ~WindowsImageContainer()
        {
            DisposeInner();
        }

        ///<summary>
        ///Release all unmanaged resources
        ///</summary>
        public
        void
        Dispose()
        {
            foreach (WindowsImage wi in m_Images)
            {
                if (wi != null)
                {
                    wi.Dispose();
                }
            }
            DisposeInner();
            GC.SuppressFinalize(this);
        }

        private
        void
        DisposeInner()
        {
            if (m_ImageContainerHandle != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(m_ImageContainerHandle);
                m_ImageContainerHandle = IntPtr.Zero;
            }

            if (m_MessageCallback != null)
            {
                NativeMethods.UnregisterMessageCallback(m_MessageCallback);
                m_MessageCallback = null;
            }
            GC.KeepAlive(this);
        }

        ///<summary>
        ///Used to enumerate the WindowsImage array
        ///</summary>
        public
        IEnumerator
        GetEnumerator(
            )
        {
            return m_Images.GetEnumerator();
        }

        ///<summary>
        ///[] overload, used to enumerate the WindowsImage array. 
        ///</summary>
        public
        IImage
        this[int imageIndex]
        {
            get
            {
                //
                //Delay the loading of images.
                //
                if (imageIndex >= ImageCount)
                {
                    return null;
                }

                if (m_Images == null)
                {
                    m_Images = new WindowsImage[ImageCount];
                }
                if (m_Images[imageIndex] == null)
                {
                    m_Images[imageIndex] = new WindowsImage(m_ImageContainerHandle, m_WindowsImageFilePath, imageIndex + 1);
                }
                GC.KeepAlive(this);
                return m_Images[imageIndex];
            }
            set
            {
                m_Images[imageIndex] = (value as WindowsImage);
            }
        }

        ///<summary>
        ///Retrieve the number of images in the .wim file.
        ///</summary>
        public int ImageCount
        {
            get
            {
                //
                //Verify that there is an image count. If not, get an image count from the GetImageCount function.
                //
                if (m_ImageCount == 0)
                {
                    m_ImageCount = NativeMethods.GetImageCount(m_ImageContainerHandle);
                }

                GC.KeepAlive(this);
                return m_ImageCount;
            }
        }

        ///<summary>
        ///Capture an image from the root of a drive or from an individual directory.
        ///</summary>
        public void CaptureImage(string pathToCapture)
        {
            //
            //Capture the image.
            //
            IntPtr windowsImageHandle = NativeMethods.CaptureImage(m_ImageContainerHandle, pathToCapture);
            NativeMethods.CloseHandle(windowsImageHandle);
            GC.KeepAlive(this);
        }
       

        ///<summary>
        ///Default event handler
        ///</summary>
        //[CLSCompliant(false)]
        public delegate void DefaultImageEventHandler(object sender, DefaultImageEventArgs e);
        //public delegate void DefaultImageEventHandler(IntPtr wParam, IntPtr lParam, IntPtr UserData);
        ///<summary>
        ///ProcessFileEvent handler
        ///</summary>
        //[CLSCompliant(false)]
        public delegate void ProcessFileEventHandler(object sender, ProcessFileEventArgs e);
        //public delegate void ProcessFileEventHandler(ProcessFile fileToProcess);


        ///<summary>
        ///Indicate an update in the progress of an image application.
        ///</summary>
        //[CLSCompliant(false)]
        public event DefaultImageEventHandler ProgressEvent;
        ///<summary>
        ///Enable the caller to prevent a file or a directory from being captured or applied.
        ///</summary>
        //[CLSCompliant(false)]
        public event ProcessFileEventHandler ProcessFileEvent;
        ///<summary>
        ///Enable the caller to prevent a file resource from being compressed during a capture.
        ///</summary>
        //[CLSCompliant(false)]
        public event DefaultImageEventHandler CompressEvent;
        ///<summary>
        ///Alert the caller that an error has occurred while capturing or applying an image.
        ///</summary>
        //[CLSCompliant(false)]
        public event DefaultImageEventHandler ErrorEvent;
        ///<summary>
        ///Enable the caller to align a file resource on a particular alignment boundary.
        ///</summary>
        //[CLSCompliant(false)]
        public event DefaultImageEventHandler AlignmentEvent;
        ///<summary>
        ///Enable the caller to align a file resource on a particular alignment boundary.
        ///</summary>
        //[CLSCompliant(false)]
        public event DefaultImageEventHandler SplitEvent;
        ///<summary>
        ///Indicate that volume information is being gathered during an image capture.
        ///</summary>
        //[CLSCompliant(false)]
        public event DefaultImageEventHandler ScanningEvent;
        ///<summary>
        ///Indicate the number of files that will be captured or applied.
        ///</summary>
        //[CLSCompliant(false)]
        public event DefaultImageEventHandler SetRangeEvent;
        ///<summary>
        ///Indicate the number of files that have been captured or applied.
        ///</summary>
        //[CLSCompliant(false)]
        public event DefaultImageEventHandler SetPosEvent;
        ///<summary>
        ///Indicate that a file has been either captured or applied.
        ///</summary>
        //[CLSCompliant(false)]
        public event DefaultImageEventHandler StepItEvent;


        ///<summary>
        ///Event callback to the Wimgapi events
        ///</summary>
        private
        uint
        ImageEventMessagePump(
            uint MessageId,
            IntPtr wParam,
            IntPtr lParam,
            IntPtr UserData
        )
        {
            uint status = (uint)NativeMethods.WIMMessage.WIM_MSG_SUCCESS;
            DefaultImageEventArgs eventArgs = new DefaultImageEventArgs(wParam, lParam, UserData);

            switch ((ImageEventMessage)MessageId)
            {

                case ImageEventMessage.Progress:
                    ProgressEvent(this, eventArgs);
                    break;

                case ImageEventMessage.Process:
                    string fileToImage = Marshal.PtrToStringUni(wParam);
                    ProcessFileEventArgs fileToProcess = new ProcessFileEventArgs(fileToImage, lParam);
                    ProcessFileEvent(this, fileToProcess);
                    if (fileToProcess.Abort == true)
                    {
                        status = (uint)ImageEventMessage.Abort;
                    }
                    break;

                case ImageEventMessage.Compress:
                    CompressEvent(this, eventArgs);
                    break;

                case ImageEventMessage.Error:
                    ErrorEvent(this, eventArgs);
                    break;

                case ImageEventMessage.Alignment:
                    AlignmentEvent(this, eventArgs);
                    break;

                case ImageEventMessage.Split:
                    SplitEvent(this, eventArgs);
                    break;

                case ImageEventMessage.Scanning:
                    ScanningEvent(this, eventArgs);
                    break;

                case ImageEventMessage.SetRange:
                    SetRangeEvent(this, eventArgs);
                    break;

                case ImageEventMessage.SetPos:
                    SetPosEvent(this, eventArgs);
                    break;

                case ImageEventMessage.StepIt:
                    StepItEvent(this, eventArgs);
                    break;

                default:
                    break;
            }

            return status;
        }

        ///<summary>
        ///Image inside of a .wim file
        ///</summary>
        private
        class
        WindowsImage : IImage, IDisposable
        {
            ///<summary>
            ///Public constructor to create an image object from inside a .wim file
            ///</summary>
            public WindowsImage(IntPtr imageContainerHandle, string imageContainerFilePath, int imageIndex)
            {
                m_ParentWindowsImageHandle = imageContainerHandle;
                m_ParentWindowsImageFilePath = imageContainerFilePath;
                m_Index = imageIndex;

                //
                //Load the image and store the handle.
                //
                m_ImageHandle = NativeMethods.LoadImage(imageContainerHandle, imageIndex);
            }

            ///<summary> Destructor to close open handles.</summary>
            ~WindowsImage()
            {
                DisposeInner();
            }

            ///<summary>
            ///Release all unmanaged resources.
            ///</summary>
            public
            void
            Dispose()
            {
                DisposeInner();
                GC.SuppressFinalize(this);
            }

            private
            void
            DisposeInner()
            {
                //
                //Do not leave any open handles or mounted images.
                //
                if (m_ImageHandle != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(m_ImageHandle);
                    m_ImageHandle = IntPtr.Zero;
                }

                if (m_Mounted == true)
                {
                    //
                    //Never commit changes when destroying this object.
                    //
                    this.DismountImage(false);
                }
                GC.KeepAlive(this);
            }

            ///<summary>
            ///Gets an image information header.
            ///</summary>
            ///<value></value>
            public XmlTextReader ImageInformation
            {
                get
                {
                    //
                    //Always get the image header (even if we have it already), and remove the unicode file marker.
                    //
                    string str = NativeMethods.GetImageInformation(m_ImageHandle).Remove(0, 1);
                    GC.KeepAlive(this);

                    return ImageInformationHelper(str);
                }
            }

            ///<summary>
            ///Returns an XmlTextReader for a given node name.
            ///</summary>
            private XmlTextReader ImageInformationHelper(string raw)
            {

                XmlTextReader xmlTextReader = null;

                if (raw != null)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    try
                    {
                        xmlDocument.LoadXml(raw);

                        //
                        //Look at all nodes.
                        //
                        foreach (XmlNode node in xmlDocument.ChildNodes)
                        {

                            //
                            //Now, find the specified node.
                            //
                            foreach (XmlNode childNode in node)
                            {
                                if
(String.Equals(childNode.Name, node.ToString(), StringComparison.InvariantCultureIgnoreCase))
                                {

                                    StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
                                    XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

                                    xmlTextWriter.WriteStartElement(childNode.Name);
                                    //
                                    //We are in the specified node. Now, get all the attributes.
                                    //
                                    foreach (XmlAttribute attribute in childNode.Attributes)
                                    {
                                        xmlTextWriter.WriteAttributeString(attribute.Name, attribute.InnerXml);
                                    }
                                    xmlTextWriter.WriteEndElement();

                                    xmlTextReader = new XmlTextReader(null, new StringReader(stringWriter.ToString()));
                                }
                            }
                        }
                    }
                    catch (System.Xml.XmlException)
                    {
                        throw new System.Xml.XmlException(string.Format(CultureInfo.CurrentCulture,
                                                                        "Unable to read XML header information from {0}",
                                                                        this.m_ParentWindowsImageFilePath));
                    }
                }
                return xmlTextReader;
            }

            ///<summary>
            ///Retrieves the path to which an image has been mounted.
            /// </summary>
            public string MountedPath
            {
                get
                {
                    return (m_MountedPath != null) ? m_MountedPath : null;
                }
            }

            ///<summary>
            ///Sets the image information header
            ///</summary>
            ///<returns></returns>
            public void SetImageInformation(string imageInformation)
            {
                //
                //Format the incoming XML so that we can set the header. The XML must:
                //1. Begin with 0xFEFF
                //2. Be contained in <IMAGE></IMAGE> tags
                //
                string formattedXml = String.Format(CultureInfo.InvariantCulture,
                                                    "{0}{1}{2}{3}",
                                                    UNICODE_FILE_MARKER,
                                                    "<IMAGE>",
                                                    imageInformation,
                                                    "</IMAGE>");

                NativeMethods.SetImageInformation(m_ImageHandle, formattedXml);
                GC.KeepAlive(this);
            }

            ///<summary>
            ///Mounts an image to a directory.
            ///</summary>
            ///<returns></returns>
            public void Mount(string pathToMountTo)
            {
                //
                //Mount the image
                //
                m_MountedPath = pathToMountTo;
                NativeMethods.MountImage(pathToMountTo, m_ParentWindowsImageFilePath, m_Index);
                m_Mounted = true;
            }

            ///<summary>
            ///Unmounts an image from a directory.
            ///</summary>
            ///<returns></returns>
            public void DismountImage(bool commitChanges)
            {
                if (m_Mounted == true)
                {
                    NativeMethods.DismountImage(m_MountedPath, m_ParentWindowsImageFilePath, m_Index, commitChanges);
                }
            }

            ///<summary>
            ///Applies an image to a drive root or to a directory path.
            ///</summary>
            ///<returns></returns>
            public void Apply(string pathToApplyTo)
            {
                NativeMethods.ApplyImage(m_ImageHandle, pathToApplyTo);
                GC.KeepAlive(this);
            }

            private IntPtr m_ParentWindowsImageHandle = IntPtr.Zero;    //.wim file handle
            private string m_ParentWindowsImageFilePath;                //Path to .wim file

            private IntPtr m_ImageHandle = IntPtr.Zero;                 //Image handle
            private int m_Index;                                        //Index of image

            private string m_MountedPath;                               //Path to which the image has been mounted
            private bool m_Mounted;                                        //A Boolean set to true if image has been mounted

            //
            //DO NOT CHANGE! This controls the format of the image header
            //and it must be present.
            //
            private const string UNICODE_FILE_MARKER = "\uFEFF";
        }

        ///<summary>
        ///Interop to Wimgapi.dll
        ///</summary>
        private
        class
        NativeMethods
        {
            ///<summary>
            ///Private null constructor
            ///</summary>
            private
            NativeMethods() { }

            [DllImport("Wimgapi.dll", ExactSpelling = true,
                       EntryPoint = "WIMCreateFile",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            IntPtr
            WimCreateFile(
                [MarshalAs(UnmanagedType.LPWStr)] string WimPath,
                uint DesiredAccess,
                uint CreationDisposition,
                uint FlagsAndAttributes,
                uint CompressionType,
                out IntPtr CreationResult
            );

            ///<summary>
            ///Creates a new .wim file or opens an existing .wim file.
            ///</summary>
            ///<param name="imageFile">Path to the .wim file to open or to create.</param>
            ///<param name="access">Specifies the file access to grant the file.</param>
            ///<param name="mode">Specifies the mode in which the file should be opened or created.</param>
            ///<returns>If the function succeeds, the return value is an open handle to the specified image file.
            ///If the function fails, the return value is NULL.</returns>
            public
            static
            IntPtr
            CreateFile(string imageFile, uint access, uint mode)
            {
                IntPtr creationResult = IntPtr.Zero;
                IntPtr windowsImageHandle = IntPtr.Zero;
                int rc = -1;

                windowsImageHandle = NativeMethods.WimCreateFile(imageFile, access, mode, 0, 0, out creationResult);
                rc = Marshal.GetLastWin32Error();
                if (windowsImageHandle == IntPtr.Zero)
                {
                    //
                    //Function failed. Throw an exception
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                             "Unable to open/create .wim file {0}. Error = {1}",
                                                                             imageFile, rc));
                }

                return windowsImageHandle;
            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMCloseHandle",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            bool
            WimCloseHandle(
                IntPtr Handle
            );

            ///<summary>
            ///Closes an open .wim file or an image handle.
            ///</summary>
            ///<param name="handle">Handle to an open imaging-based object.</param>
            ///<returns>If the function succeeds, the return value is nonzero.
            ///If the function fails, the return value is zero. </returns>
            public
            static
            void
            CloseHandle(IntPtr handle)
            {
                bool status = NativeMethods.WimCloseHandle(handle);
                int rc = Marshal.GetLastWin32Error();
                if (status == false)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                         "Unable to close image handle. Error = {0}", rc));
                }
            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMSetTemporaryPath",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            bool
            WimSetTemporaryPath(
                IntPtr Handle,
                [MarshalAs(UnmanagedType.LPWStr)] string TemporaryPath
            );

            ///<summary>
            ///Sets the location where temporary imaging files are stored.
            ///</summary>
            ///<param name="handle">Handle to a .wim file returned by CreateFile</param>
            ///<param name="temporaryPath">String value of path to set as a temporary location.</param>
            ///<returns>If the function succeeds, the return value is nonzero.
            ///If the function fails, the return value is NULL.</returns>
            public
            static
            void
            SetTemporaryPath(IntPtr handle, string temporaryPath)
            {
                bool status = NativeMethods.WimSetTemporaryPath(handle, temporaryPath);
                int rc = Marshal.GetLastWin32Error();
                if (status == false)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to set temporary path. Error = {0}", rc));
                }
            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMLoadImage",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            IntPtr
            WimLoadImage(
                IntPtr Handle,
                uint ImageIndex
            );

            ///<summary>
            ///Loads a volume image from within a .wim file.
            ///</summary>
            ///<param name="handle">Wim handle.</param>
            ///<param name="imageIndex">Index of the image to load.</param>
            ///<returns>If the function succeeds, the return value is a handle to an object representing the volume image. 
            ///If the function fails, the return value is NULL. </returns>
            public
            static
            IntPtr
            LoadImage(IntPtr handle, int imageIndex)
            {
                //Load the image data based on the .wim handle
                //
                IntPtr hWim = NativeMethods.WimLoadImage(handle, (uint)imageIndex);
                int rc = Marshal.GetLastWin32Error();
                if (hWim == IntPtr.Zero)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to load image. Error = {0}", rc));
                }

                return hWim;

            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMCaptureImage",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            IntPtr
            WimCaptureImage(
                IntPtr Handle,
                [MarshalAs(UnmanagedType.LPWStr)] string Path,
                uint CaptureFlags
            );

            ///<summary>
            ///Captures an image from a drive root or from a directory path and stores it in an image file.
            ///</summary>
            ///<param name="handle">Handle to a .wim file returned by CreateFile.</param>
            ///<param name="path">Drive root or directory path from where the image data will be captured.</param>
            ///<returns>Handle to an object representing the volume image. If the function fails, the return value is NULL.</returns>
            public
            static
            IntPtr
            CaptureImage(IntPtr handle, string path)
            {
                IntPtr hImage = NativeMethods.WimCaptureImage(handle, path, 0);
                int rc = Marshal.GetLastWin32Error();
                if (hImage == IntPtr.Zero)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                             "Failed to capture image from {0}. Error = {1}", path, rc));
                }
                return hImage;
            }

            ///<summary>
            ///Gets the number of volume images stored in a .wim file.
            ///</summary>
            ///<param name="Handle">Handle to a .wim file returned by CreateFile.</param>
            ///<returns>The number of images within the .wim file. </returns>
            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMGetImageCount",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            int
            WimGetImageCount(
                IntPtr Handle
            );

            ///<summary>
            ///Returns the number of volume images stored in a .wim file.
            ///</summary>
            ///<param name="windowsImageHandle">Handle to a .wim file returned by WIMCreateFile.</param>
            ///<returns>The return value is the number of images within the .wim file.
            ///If this value is zero, then the .wim file is invalid or does not contain any images that can be applied.
            ///</returns>
            public
            static
            int
            GetImageCount(IntPtr windowsImageHandle)
            {
                int count = NativeMethods.WimGetImageCount(windowsImageHandle);
                int rc = Marshal.GetLastWin32Error();
                if (count == -1)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to get image count. Error = {0}", rc));
                }

                return count;
            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMMountImage",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            bool
            WimMountImage(
                [MarshalAs(UnmanagedType.LPWStr)] string MountPath,
                [MarshalAs(UnmanagedType.LPWStr)] string WimFileName,
                uint ImageIndex,
                [MarshalAs(UnmanagedType.LPWStr)] string TemporaryPath

            );

            ///<summary>
            ///Mounts an image in a .wim file to the specified directory.
            ///</summary>
            ///<returns>Returns TRUE and sets the LastError to ERROR_SUCCESS.
            ///Returns FALSE in case of a failure and the LastError is set to the appropriate Win32 error value.
            ///</returns>
            public
            static
            void
            MountImage(string mountPath, string windowsImageFileName, int imageIndex)
            {
                bool status = false;
                int rc;

                try
                {
                    status = NativeMethods.WimMountImage(mountPath,
                                                         windowsImageFileName,
                                                         (uint)imageIndex,
                                                         System.Environment.GetEnvironmentVariable("temp"));
                    rc = Marshal.GetLastWin32Error();
                }
                catch (System.StackOverflowException)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                             "Unable to mount image {0} to {1}.", windowsImageFileName, mountPath));
                }
                if (status == false)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                             "Unable to mount image {0} to {1}. Error = {2}",
                                                                             windowsImageFileName, mountPath, rc));
                }
            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMApplyImage",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            bool
            WimApplyImage(
                IntPtr Handle,
                [MarshalAs(UnmanagedType.LPWStr)] string Path,
                uint Flags
            );

            ///<summary>
            ///Applies an image to a drive root or to a directory path from a .wim file.
            ///</summary>
            ///<returns>If the function succeeds, the return value is nonzero.
            ///If the function fails, the return value is zero</returns>
            public
            static
            void
            ApplyImage(IntPtr imageHandle, string applicationPath)
            {
                //
                //Call WimApplyImage always with the Index flag for performance reasons.
                //
                bool status = NativeMethods.WimApplyImage(imageHandle, applicationPath, NativeMethods.WIM_FLAG_INDEX);
                int rc = Marshal.GetLastWin32Error();
                if (status == false)
                {
                    //
                    //Throw an exception
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                             "Unable to apply image to {0}. Error = {1}", applicationPath, rc));
                }
            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMGetImageInformation",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            bool
            WimGetImageInformation(
                IntPtr Handle,
                out IntPtr ImageInfo,
                out IntPtr SizeOfImageInfo
            );

            ///<summary>
            ///Returns information about an image within the .wim file.
            ///</summary>
            ///<param name="handle">Handle returned by CreateImage, LoadImage</param>
            ///<returns>If the function succeeds, the return value is nonzero.
            ///If the function fails, the return value is zero.
            ///</returns>
            public
            static
            string
            GetImageInformation(IntPtr handle)
            {
                IntPtr info = IntPtr.Zero, sizeOfInfo = IntPtr.Zero;
                bool status;

                status = NativeMethods.WimGetImageInformation(handle, out info, out sizeOfInfo);
                int rc = Marshal.GetLastWin32Error();

                if (status == false)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                             "Unable to get image information. Error = {0}", rc));
                }
                string s = Marshal.PtrToStringUni(info);

                //If the function succeeds, return the pointer to the string. Otherwise, return NULL.
                //
                return s;
            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMSetImageInformation",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            bool
            WimSetImageInformation(
                IntPtr Handle,
                IntPtr ImageInfo,
                uint SizeOfImageInfo
            );

            ///<summary>
            ///Stores information about an image within the .wim file.
            ///</summary>
            ///<param name="handle">Handle returned by CreateImage, LoadImage</param>
            ///<param name="imageInfo">String containing the unicode XML data to set.</param>
            ///<returns>If the function succeeds, the return value is nonzero.
            ///If the function fails, the return value is zero. </returns>
            public
            static
            void
            SetImageInformation(IntPtr handle, string imageInfo)
            {
                //Create a byte array for the stream, allocate some unmanaged memory, and then copy the bytes to the unmanaged memory.
                //
                byte[] byteBuffer = Encoding.Unicode.GetBytes(imageInfo);
                int byteBufferSize = byteBuffer.Length;
                IntPtr xmlBuffer = Marshal.AllocHGlobal(byteBufferSize);
                Marshal.Copy(byteBuffer, 0, xmlBuffer, byteBufferSize);

                bool status = NativeMethods.WimSetImageInformation(handle, xmlBuffer, (uint)byteBufferSize);
                int rc = Marshal.GetLastWin32Error();
                if (status == false)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                             "Unable to set image information. Error = {0}", rc));
                }
            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMUnmountImage",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            bool
            WimUnmountImage(
                [MarshalAs(UnmanagedType.LPWStr)] string MountPath,
                [MarshalAs(UnmanagedType.LPWStr)] string WimFileName,
                uint ImageIndex,
                bool CommitChanges
            );

            ///<summary>
            ///Unmounts a mounted image in a .wim file from the specified directory.
            ///</summary>
            ///<returns>Returns TRUE and sets the LastError to ERROR_SUCCESS. Returns FALSE in case of a failure and the LastError is set
            ///to the appropriate Win32 error value.</returns>
            public
            static
            void
            DismountImage(string mountPath, string wimdowsImageFileName, int imageIndex, bool commitChanges)
            {
                bool status = false;
                int rc;

                try
                {
                    status = NativeMethods.WimUnmountImage(mountPath, wimdowsImageFileName, (uint)imageIndex, commitChanges);
                    rc = Marshal.GetLastWin32Error();
                }
                catch (System.StackOverflowException ex)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.StackOverflowException(string.Format(CultureInfo.CurrentCulture,
                                                                          "Unable to unmount image {0} from {1}.", wimdowsImageFileName, mountPath),
                                                                          ex.InnerException);
                }
                if (status == false)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                             "Unable to unmount image {0} from {1}. Error = {2}",
                                                                             wimdowsImageFileName, mountPath, rc));
                }
            }

            ///<summary>
            ///User-defined function used with the RegisterMessageCallback or UnregisterMessageCallback function.
            ///</summary>
            ///<param name="MessageId">Specifies the message being sent.</param>
            ///<param name="wParam">Specifies additional message information. The contents of this parameter depend on the value of the
            ///MessageId parameter.</param>
            ///<param name="lParam">Specifies additional message information. The contents of this parameter depend on the value of the
            ///MessageId parameter.</param>
            ///<param name="UserData">Specifies the user-defined value passed to RegisterCallback.</param>
            ///<returns>
            ///To indicate success and to enable other subscribers to process the message return WIM_MSG_SUCCESS.
            ///To prevent other subscribers from receiving the message, return WIM_MSG_DONE.
            ///To cancel the application or capture of an image, return WIM_MSG_ABORT_IMAGE when handling the WIM_MSG_PROCESS message.
            ///</returns>
            public
            delegate
            uint
            MessageCallback(
                uint MessageId,
                IntPtr wParam,
                IntPtr lParam,
                IntPtr UserData
            );

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMRegisterMessageCallback",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            uint
            WimRegisterMessageCallback(
                IntPtr hWim,
                MessageCallback MessageProc,
                IntPtr ImageInfo
            );

            ///<summary>
            ///Registers a function to be called with imaging-specific data.
            ///</summary>
            public
            static
            void
            RegisterCallback(MessageCallback callback)
            {
                uint callbackZeroBasedIndex = NativeMethods.WimRegisterMessageCallback(IntPtr.Zero, callback, IntPtr.Zero);
                int rc = Marshal.GetLastWin32Error();
                if (rc != 0)
                {
                    //
                    //Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to register message callback."));
                }
            }

            [DllImport("Wimgapi.dll",
                       ExactSpelling = true,
                       EntryPoint = "WIMUnregisterMessageCallback",
                       CallingConvention = CallingConvention.StdCall,
                       SetLastError = true)]
            private static extern
            bool
            WimUnregisterMessageCallback(
                IntPtr hWim,
                MessageCallback MessageProc
            );

            ///<summary>
            ///Unregisters a function from being called with imaging-specific data.
            ///</summary>
            ///<param name="registeredCallback">The Callback function to be unregistered.</param>
            public
            static
            void
            UnregisterMessageCallback(MessageCallback registeredCallback)
            {
                bool status = NativeMethods.WimUnregisterMessageCallback(IntPtr.Zero, registeredCallback);
                int rc = Marshal.GetLastWin32Error();
                if (status != true)
                {
                    //
                    // Throw an exception.
                    //
                    throw new System.InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unable to unregister message callback."));
                }
            }

            private const uint WM_APP = 0x8000;

            ///<summary>
            ///Imaging Messages
            ///</summary>
            public enum WIMMessage : uint
            {
                WIM_MSG = WM_APP + 0x1476,
                WIM_MSG_TEXT,
                ///<summary>
                ///Indicates an update in the progress of an image application.
                ///</summary>
                WIM_MSG_PROGRESS,
                ///<summary>
                ///Enables the caller to prevent a file or a directory from being captured or applied.
                ///</summary>
                WIM_MSG_PROCESS,
                ///<summary>
                ///Indicates that volume information is being gathered during an image capture.
                ///</summary>
                WIM_MSG_SCANNING,
                ///<summary>
                ///Indicates the number of files that will be captured or applied.
                ///</summary>
                WIM_MSG_SETRANGE,
                ///<summary>
                ///Indicates the number of files that have been captured or applied.
                ///</summary>
                WIM_MSG_SETPOS,
                ///<summary>
                ///Indicates that a file has been either captured or applied.
                ///</summary>
                WIM_MSG_STEPIT,
                ///<summary>
                ///Enables the caller to prevent a file resource from being compressed during a capture.
                ///</summary>
                WIM_MSG_COMPRESS,
                ///<summary>
                ///Alerts the caller that an error has occurred while capturing or applying an image.
                ///</summary>
                WIM_MSG_ERROR,
                ///<summary>
                ///Enables the caller to align a file resource on a particular alignment boundary.
                ///</summary>
                WIM_MSG_ALIGNMENT,
                WIM_MSG_RETRY,
                ///<summary>
                ///Enables the caller to align a file resource on a particular alignment boundary.
                ///</summary>
                WIM_MSG_SPLIT,
                WIM_MSG_SUCCESS = 0x00000000,
                WIM_MSG_ABORT_IMAGE = 0xFFFFFFFF
            };

            ///<summary>
            ///The image capture will do a byte-by-byte verification of single instance files.
            ///</summary>
            public const uint WIM_FLAG_VERIFY = 0x00000002;
            ///<summary>
            ///Specifies that the image is to be sequentially read for caching or performance purposes.
            ///</summary>
            public const uint WIM_FLAG_INDEX = 0x00000004;

        }

        ///<summary>
        ///Maps CreateFileAccess to CreateFileAccessPrivate
        ///</summary>
        ///
        private
        CreateFileAccessPrivate
        GetMappedFileAccess(CreateFileAccess access)
        {
            //
            //Map the file access specified from an int to a uint.
            //
            CreateFileAccessPrivate fileAccess;
            switch (access)
            {
                case CreateFileAccess.Read:
                    fileAccess = CreateFileAccessPrivate.Read;
                    break;
                case CreateFileAccess.Write:
                    fileAccess = CreateFileAccessPrivate.Write;
                    break;
                default:
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "No file access level specified."));
            }
            return fileAccess;
        }

        //[CLSCompliant(false)]
        [FlagsAttribute]
        private
        enum
        CreateFileAccessPrivate : uint
        {
            ///<summary>
            ///Mapping from CreateFileAccess.Read
            ///</summary>
            Read = 0x80000000,
            ///<summary>
            ///Mapping from CreateFileAccess.Write
            ///</summary>
            Write = 0x40000000
        }

        ///<summary>
        ///Image event messages.
        ///</summary>
        //[CLSCompliant(false)]
        private
        enum
        ImageEventMessage : uint
        {
            ///<summary>
            ///Enables the caller to prevent a file or a directory from being captured or applied.
            ///</summary>
            Progress = NativeMethods.WIMMessage.WIM_MSG_PROGRESS,
            ///<summary>
            ///Notification sent to enable the caller to prevent a file or a directory from being captured or applied.
            ///To prevent a file or a directory from being captured or applied, call WindowsImageContainer.SkipFile().
            ///</summary>
            Process = NativeMethods.WIMMessage.WIM_MSG_PROCESS,
            ///<summary>
            ///Enables the caller to prevent a file resource from being compressed during a capture.
            ///</summary>
            Compress = NativeMethods.WIMMessage.WIM_MSG_COMPRESS,
            ///<summary>
            ///Alerts the caller that an error has occurred while capturing or applying an image.
            ///</summary>
            Error = NativeMethods.WIMMessage.WIM_MSG_ERROR,
            ///<summary>
            ///Enables the caller to align a file resource on a particular alignment boundary.
            ///</summary>
            Alignment = NativeMethods.WIMMessage.WIM_MSG_ALIGNMENT,
            ///<summary>
            ///Enables the caller to align a file resource on a particular alignment boundary.
            ///</summary>
            Split = NativeMethods.WIMMessage.WIM_MSG_SPLIT,
            ///<summary>
            ///Indicates that volume information is being gathered during an image capture.
            ///</summary>
            Scanning = NativeMethods.WIMMessage.WIM_MSG_SCANNING,
            ///<summary>
            ///Indicates the number of files that will be captured or applied.
            ///</summary>
            SetRange = NativeMethods.WIMMessage.WIM_MSG_SETRANGE,
            ///<summary>
            ///Indicates the number of files that have been captured or applied.
            /// </summary>
            SetPos = NativeMethods.WIMMessage.WIM_MSG_SETPOS,
            ///<summary>
            ///Indicates that a file has been either captured or applied.
            ///</summary>
            StepIt = NativeMethods.WIMMessage.WIM_MSG_STEPIT,
            ///<summary>
            ///Success.
            ///</summary>
            Success = NativeMethods.WIMMessage.WIM_MSG_SUCCESS,
            ///<summary>
            ///Abort.
            ///</summary>
            Abort = NativeMethods.WIMMessage.WIM_MSG_ABORT_IMAGE
        }

        //
        //WindowsImageContainer Member Data
        //
        private IntPtr m_ImageContainerHandle;  //Handle to the .wim file
        private string m_WindowsImageFilePath;  //Path to the .wim file

        private WindowsImage[] m_Images;        //Array of image objects inside a .wim file
        private int m_ImageCount;               //Number of images inside a .wim file

        //
        //DO NOT CHANGE!
        //
        private static NativeMethods.MessageCallback m_MessageCallback;
    }

    ///<summary>
    ///Describes the file that is being processed for the ProcessFileEvent.
    ///</summary>
    public
    class
    DefaultImageEventArgs : EventArgs
    {
        ///<summary>
        ///Default constructor.
        ///</summary>
        public
        DefaultImageEventArgs(IntPtr wideParameter, IntPtr leftParameter, IntPtr userData)
        {
            m_wParam = wideParameter;
            m_lParam = leftParameter;
            m_UserData = userData;


        }
        ///<summary>
        ///wParam
        ///</summary>
        public IntPtr WideParameter
        {
            get
            {
                return m_wParam;
            }
        }
        ///<summary>
        ///lParam
        ///</summary>
        public IntPtr LeftParameter
        {
            get
            {
                return m_lParam;
            }
        }
        ///<summary>
        ///UserData
        ///</summary>
        public IntPtr UserData
        {
            get
            {
                return m_UserData;
            }
        }

        private IntPtr m_wParam;
        private IntPtr m_lParam;
        private IntPtr m_UserData;
    }

    ///<summary>
    ///Describes the file that is being processed for the ProcessFileEvent.
    ///</summary>
    public
    class
    ProcessFileEventArgs : EventArgs
    {
        ///<summary>
        ///Default constructor.
        ///</summary>
        ///<param name="file">Fully qualified path and file name. For example: c:\file.sys.</param>
        ///<param name="skipFileFlag">Default is false - skip file and continue.
        ///Set to true to abort the entire image capture.</param>
        public
        ProcessFileEventArgs(string file, IntPtr skipFileFlag)
        {
            m_FilePath = file;
            m_SkipFileFlag = skipFileFlag;
        }

        ///<summary>
        ///Skip file from being imaged.
        ///</summary>
        public
        void
        SkipFile()
        {
            byte[] byteBuffer = {
                    0
                };
            int byteBufferSize = byteBuffer.Length;
            Marshal.Copy(byteBuffer, 0, m_SkipFileFlag, byteBufferSize);
        }

        ///<summary>
        ///Fully qualified path and file name.
        ///</summary>
        public string FilePath
        {
            get
            {
                string stringToReturn = "";
                if (m_FilePath != null)
                {
                    stringToReturn = m_FilePath;
                }
                return stringToReturn;
            }
        }

        ///<summary>
        ///Flag that indicates if the entire image capture should be aborted.
        ///Default is false - skip file and continue. Setting to true will
        ///abort the entire image capture.
        ///</summary>
        public bool Abort
        {
            set
            {
                m_Abort = value;
            }

            get
            {
                return m_Abort;
            }
        }
        private string m_FilePath;
        private bool m_Abort;
        private IntPtr m_SkipFileFlag;

    }
}
