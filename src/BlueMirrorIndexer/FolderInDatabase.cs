using Igorary.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BlueMirrorIndexer {

    public static class FILETIMEExtensions
    {
        public static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME filetime)
        {
            long highBits = filetime.dwHighDateTime;
            highBits = highBits << 32;
            return DateTime.FromFileTimeUtc(highBits + (long)filetime.dwLowDateTime);
        }
    }

	[Serializable]
    public class FolderInDatabase : ItemInDatabase, IFolder
    {
        public int DbId { get; private set; }
        public FolderInDatabase(int dbId, IFolder parent) : this(parent)
        {
            DbId = dbId; // TODO KBR for SQLite load
        }


        FolderImpl folderImpl;

		public FolderInDatabase(IFolder parent): base(parent) {
            folderImpl = new FolderImpl(this, 1);
		}

        public override ListViewItem ToListViewItem() {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = Name;
            lvi.Tag = this;
            lvi.ImageIndex = Win32.GetFolderIconIndex("", Win32.FileIconSize.Small);
            lvi.SubItems.Add(string.Empty);
            lvi.SubItems.Add(CreationTime.ToString("g"));
            lvi.SubItems.Add(Attributes.ToString());

            lvi.SubItems.Add(Keywords);
            lvi.SubItems.Add(Extension);
            lvi.SubItems.Add(FileDescription);
            lvi.SubItems.Add(FileVersion);

            lvi.SubItems.Add(GetVolumeUserName());
            lvi.SubItems.Add(GetPath());
            
            lvi.SubItems.Add(string.Empty);
            return lvi;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll")]
        public static extern bool FindClose(IntPtr hFindFile);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_DATAW
        {
            public FileAttributes dwFileAttributes;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        // TODO KBR remove runningFile*

        internal void ReadFromFolderKBR(string folder, List<string> excludedFolders, ref long runningFileCount, ref long runningFileSize, bool useSize, DlgReadingProgress dlgReadingProgress, FolderInDatabase folderToReplace) 
	    {
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            WIN32_FIND_DATAW findData;
            IntPtr findHandle = INVALID_HANDLE_VALUE;

            try
            {
                findHandle = FindFirstFileW(folder + @"\*", out findData);
                if (findHandle == INVALID_HANDLE_VALUE) 
                    return;

                do
                {
                    if (findData.cFileName == "." || findData.cFileName == "..") 
                        continue;

                    string fullpath = folder + (folder.EndsWith("\\") ? "" : "\\") + findData.cFileName;

                    if ((findData.dwFileAttributes & FileAttributes.Directory) != 0)
                    {
                        FolderInDatabase newFolder = new FolderInDatabase(this);
                        newFolder.Name = findData.cFileName; //subFolder.Name;
                        newFolder.Attributes = findData.dwFileAttributes; //subFolder.Attributes;
                        string tmp = Path.GetExtension(fullpath);
                        if (tmp.StartsWith("."))
                            tmp = tmp.Substring(1);
                        newFolder.Extension = tmp;//subFolder.Extension;
                        newFolder.FullName = fullpath; //findData.cFileName; //subFolder.FullName;
                        newFolder.CreationTime = findData.ftCreationTime.ToDateTime(); // subFolder.CreationTime;
                        newFolder.LastAccessTime = findData.ftLastAccessTime.ToDateTime(); //subFolder.LastAccessTime;
                        newFolder.LastWriteTime = findData.ftLastWriteTime.ToDateTime(); //subFolder.LastWriteTime;

                        newFolder.ReadFromFolderKBR(fullpath, excludedFolders, ref runningFileCount, ref runningFileSize, useSize, dlgReadingProgress, null);

                        AddToFolders(newFolder);
                    }
                    else
                    {
                        var newFile = new FileInDatabase(this);
                        newFile.FullName = fullpath;

                        newFile.Name = findData.cFileName;
                        newFile.Attributes = findData.dwFileAttributes;
                        string tmp = Path.GetExtension(fullpath);
                        if (tmp.StartsWith("."))
                            tmp = tmp.Substring(1);
                        newFile.Extension = tmp;//subFolder.Extension;

                        //newFile.IsReadOnly = fileInFolder.IsReadOnly;

                        long highSize = (uint)findData.nFileSizeHigh;
                        highSize = highSize << 32;
                        highSize += (uint) findData.nFileSizeLow;
                        newFile.Length = highSize; // TODO Length field needs to be unsigned?

                        newFile.CreationTime = findData.ftCreationTime.ToDateTime(); // subFolder.CreationTime;
                        newFile.LastAccessTime = findData.ftLastAccessTime.ToDateTime(); //subFolder.LastAccessTime;
                        newFile.LastWriteTime = findData.ftLastWriteTime.ToDateTime(); //subFolder.LastWriteTime;

                        AddToFiles(newFile);

                        runningFileCount++;
                        runningFileSize += newFile.Length;
                        if (runningFileCount % 5 == 1 )
                            dlgReadingProgress.SetReadingProgress(runningFileCount, runningFileSize, newFile.FullName, "Adding...");

                    }
                }
                while (FindNextFile(findHandle, out findData));
            }
            finally
            {
                if (findHandle != INVALID_HANDLE_VALUE) FindClose(findHandle);
            }
	        
	    }

	    public void AddToFiles(FileInDatabase fid)
	    {
	        folderImpl.AddToFiles(fid);
	    }

	    public void AddToFolders(FolderInDatabase newFolder)
	    {
            folderImpl.AddToFolders(newFolder);
	    }

        internal void ReadFromFolder(string folder, List<string> excludedFolders, ref long runningFileCount, ref long runningFileSize, bool useSize, DlgReadingProgress dlgReadingProgress, FolderInDatabase folderToReplace) {
            try {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folder);
                System.IO.DirectoryInfo[] subFolders = di.GetDirectories();
                foreach (System.IO.DirectoryInfo subFolder in subFolders) {
                    if (!excludedFolders.Contains(subFolder.FullName.ToLower())) {
                        FolderInDatabase newFolder = new FolderInDatabase(this);
                        newFolder.Name = subFolder.Name;
                        newFolder.Attributes = subFolder.Attributes;
                        newFolder.CreationTime = subFolder.CreationTime;
                        newFolder.Extension = subFolder.Extension;
                        newFolder.FullName = subFolder.FullName;
                        newFolder.LastAccessTime = subFolder.LastAccessTime;
                        newFolder.LastWriteTime = subFolder.LastWriteTime;
                        FolderInDatabase subFolderToReplace;
                        if (folderToReplace != null)
                            subFolderToReplace = folderToReplace.findFolder(subFolder.Name);
                        else
                            subFolderToReplace = null;
                        newFolder.ReadFromFolder(subFolder.FullName, excludedFolders, ref runningFileCount, ref runningFileSize, useSize, dlgReadingProgress, subFolderToReplace);
                        if (subFolderToReplace != null) {
                            newFolder.Keywords = subFolderToReplace.Keywords;
                            foreach (LogicalFolder logicalFolder in subFolderToReplace.LogicalFolders)
                                logicalFolder.AddItem(newFolder);
                        }
                        folderImpl.AddToFolders(newFolder);

                        //FrmMain.dlgProgress.SetReadingProgress(0, "Adding: " + newFolder.FullName);
                    }
                }

                System.IO.FileInfo[] filesInFolder = di.GetFiles();
                foreach (System.IO.FileInfo fileInFolder in filesInFolder) {
                    if (!excludedFolders.Contains(fileInFolder.FullName.ToLower())) {
                        FileInDatabase newFile;
                        FileInDatabase fileToReplace;
                        if (folderToReplace != null)
                            fileToReplace = folderToReplace.findFile(fileInFolder.Name);
                        else
                            fileToReplace = null;
                        if (Properties.Settings.Default.BrowseInsideCompressed && (CompressedFile.IsCompressedFile(fileInFolder.Name))) {
                            CompressedFile compressedFile = new CompressedFile(this);
                            try {
                                compressedFile.BrowseFiles(fileInFolder.FullName, fileToReplace as CompressedFile);
                            }
                            catch (Exception ex) {
                                compressedFile.Comments = ex.Message;
                            }
                            // tu idzie jako katalog
                            folderImpl.AddToFolders(compressedFile);

                            // a teraz jako plik
                            newFile = compressedFile;
                        }
                        else {
                            newFile = new FileInDatabase(this);
                            newFile.FullName = fileInFolder.FullName;
                        }

                        newFile.Name = fileInFolder.Name;
                        newFile.Attributes = fileInFolder.Attributes;
                        newFile.CreationTime = fileInFolder.CreationTime;
                        newFile.Extension = fileInFolder.Extension;

                        newFile.LastAccessTime = fileInFolder.LastAccessTime;
                        newFile.LastWriteTime = fileInFolder.LastWriteTime;
                        newFile.IsReadOnly = fileInFolder.IsReadOnly;
                        newFile.Length = fileInFolder.Length;
                        if (Properties.Settings.Default.ReadFileInfo) {
                            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(fileInFolder.FullName);
                            newFile.Comments = fvi.Comments;
                            newFile.CompanyName = fvi.CompanyName;
                            newFile.FileVersion = fvi.FileVersion;
                            newFile.FileDescription = fvi.FileDescription;
                            newFile.LegalCopyright = fvi.LegalCopyright;
                            newFile.ProductName = fvi.ProductName;
                        }

                        if (Properties.Settings.Default.ComputeCrc) {
                            Crc32 crc32 = new Crc32(dlgReadingProgress, runningFileCount, runningFileSize, newFile.FullName);
                            try {
                                using (FileStream inputStream = new FileStream(newFile.FullName, FileMode.Open, FileAccess.Read)) {
                                    crc32.ComputeHash(inputStream);
                                    newFile.Crc = crc32.CrcValue;
                                }
                            }
                            catch (IOException) {
                                // eat the exception
                            }
                        }

                        if (fileToReplace != null) {
                            newFile.Keywords = fileToReplace.Keywords;
                            foreach (LogicalFolder logicalFolder in fileToReplace.LogicalFolders)
                                logicalFolder.AddItem(newFile);
                        }

                        folderImpl.AddToFiles(newFile);

                        runningFileCount++;
                        runningFileSize += fileInFolder.Length;
                        dlgReadingProgress.SetReadingProgress(runningFileCount, runningFileSize, newFile.FullName, "Adding...");
                    }
                }
            }
            catch (UnauthorizedAccessException) {
                // eat the exception
            }
		}

        private FileInDatabase findFile(string fileName) {
            return folderImpl.FindFile(fileName);
        }

        private FolderInDatabase findFolder(string folderName) {
            return folderImpl.FindFolder(folderName);
        }

        internal void SaveAsCsv(StreamWriter sw) {
            WriteToStream(sw);
            foreach (FolderInDatabase fid in folderImpl.Folders)
                fid.SaveAsCsv(sw);
            foreach (FileInDatabase fid in folderImpl.Files)
                fid.WriteToStream(sw);
        }

        protected override string GetItemType() {
            return "Folder";
        }

        protected override Form CreateDialog() {
            return new DlgFolderProperties(this);
        }

        public override void RemoveFromDatabase() {
            base.RemoveFromDatabase();
            Parent.RemoveFromFolders(this);
        }

        public override void RemoveFromLogicalFolders() {
            base.RemoveFromLogicalFolders();
            folderImpl.RemoveFromLogicalFolders();
        }

        internal void CopyAdditionalInfo(FolderInDatabase folderToReplace) {
            foreach (FolderInDatabase folder in folderImpl.Folders) {
                FolderInDatabase subFolderToReplace = folderToReplace.findFolder(folder.Name);
                if (subFolderToReplace != null) {
                    folder.CopyAdditionalInfo(subFolderToReplace);
                }
            }
            foreach (FileInDatabase file in folderImpl.Files) {
                FileInDatabase fileToReplace = folderToReplace.findFile(file.Name);
                if (fileToReplace != null) {
                    file.Keywords = fileToReplace.Keywords;
                    foreach (LogicalFolder logicalFolder in fileToReplace.LogicalFolders)
                        logicalFolder.AddItem(file);
                }
            }
            Keywords = folderToReplace.Keywords;
            foreach (LogicalFolder logicalFolder in folderToReplace.LogicalFolders)
                logicalFolder.AddItem(this);
        }

        #region IFolder Members

        FileInDatabase[] IFolder.Files {
            get {
                return folderImpl.Files;
            }
        }

        void IFolder.RemoveFromFiles(FileInDatabase file) {
            folderImpl.RemoveFromFiles(file);
        }

        void IFolder.AddToFiles(FileInDatabase file) {
            folderImpl.AddToFiles(file);
        }

        int IFolder.FileCount {
            get {
                return folderImpl.FileCount;
            }
        }

        IFolder[] IFolder.Folders {
            get {
                return folderImpl.Folders;
            }
        }

        void IFolder.RemoveFromFolders(IFolder folder) {
            folderImpl.RemoveFromFolders(folder);
        }

        void IFolder.AddToFolders(IFolder folder) {
            folderImpl.AddToFolders(folder);
        }

        long IFolder.GetFilesSize() {
            return folderImpl.GetFilesSize();
        }

        public void InsertFilesToList(Regex regex, DateTime? dateFrom, DateTime? dateTo, long? sizeFrom, long? sizeTo, KeywordMatcher keywordMatcher, List<FileInDatabase> listCrc, List<FileInDatabase> listNoCrc) {
            folderImpl.InsertFilesToList(regex, dateFrom, dateTo, sizeFrom, sizeTo, keywordMatcher, listCrc, listNoCrc);
        }

        public void InsertFilesToList(Regex regex, DateTime? dateFrom, DateTime? dateTo, long? sizeFrom, long? sizeTo, KeywordMatcher keywordMatcher, List<ItemInDatabase> list) {
            folderImpl.InsertFilesToList(regex, dateFrom, dateTo, sizeFrom, sizeTo, keywordMatcher, list);
        }

        public void CopyToNode(TreeNode treeNode) {
            folderImpl.CopyToNode(treeNode);
        }

        #endregion

    }

}
