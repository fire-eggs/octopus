using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

// TODO KBR most of ProcessFolder/ProcessFile is common
// TODO KBR excluded folders
// TODO KBR folder to replace

namespace BlueMirrorIndexer
{
    public class FolderReader
    {
        private long _runningFileCount;
        private long _runningFileSize;
        private List<string> _excludedFolders;
        private DlgReadingProgress _dlgReadingProgress;
        private FolderInDatabase _folderToReplace;

        public FolderReader(List<string> excludedFolders, DlgReadingProgress dlgReadingProgress, FolderInDatabase folderToReplace)
        {
            _excludedFolders = excludedFolders;
            _dlgReadingProgress = dlgReadingProgress;
            _folderToReplace = folderToReplace;

            _runningFileCount = 0;
            _runningFileSize = 0;
        }

        public void ReadFromFolder(string folder, FolderInDatabase owner)
        {
            IntPtr findHandle = INVALID_HANDLE_VALUE;
            try
            {
                WIN32_FIND_DATAW findData;
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
                        ProcessFolder(owner, findData, fullpath);
                    }
                    else
                    {
                        long len = ProcessFile(owner, findData, fullpath);
                        _runningFileCount++;
                        _runningFileSize += len;
                        if (_runningFileCount % 5 == 1)
                            _dlgReadingProgress.SetReadingProgress(_runningFileCount, _runningFileSize, fullpath, "Adding...");

                    }
                }
                while (FindNextFile(findHandle, out findData));
            }
            finally
            {
                if (findHandle != INVALID_HANDLE_VALUE) FindClose(findHandle);
            }
        }

        static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        internal long ProcessFile(FolderInDatabase owner, WIN32_FIND_DATAW findData, string fullpath)
        {
            var newFile = new FileInDatabase(owner);
            newFile.FullName = fullpath;

            newFile.Name = findData.cFileName;
            newFile.Attributes = findData.dwFileAttributes;
            string tmp = Path.GetExtension(fullpath);
            if (tmp.StartsWith("."))
                tmp = tmp.Substring(1);
            newFile.Extension = tmp;

            //newFile.IsReadOnly = fileInFolder.IsReadOnly;

            long highSize = (uint)findData.nFileSizeHigh;
            highSize = highSize << 32;
            highSize += (uint)findData.nFileSizeLow;
            newFile.Length = highSize; // TODO Length field needs to be unsigned?

            newFile.CreationTime = findData.ftCreationTime.ToDateTime();
            newFile.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
            newFile.LastWriteTime = findData.ftLastWriteTime.ToDateTime();

            ((IFolder)owner).AddToFiles(newFile);

            return newFile.Length;
        }

        internal void ProcessFolder(FolderInDatabase owner, WIN32_FIND_DATAW findData, string fullpath)
        {
            FolderInDatabase newFolder = new FolderInDatabase(owner);

            newFolder.Name = findData.cFileName;
            newFolder.Attributes = findData.dwFileAttributes;
            string tmp = Path.GetExtension(fullpath);
            if (tmp.StartsWith("."))
                tmp = tmp.Substring(1);
            newFolder.Extension = tmp;

            newFolder.FullName = fullpath;
            newFolder.CreationTime = findData.ftCreationTime.ToDateTime();
            newFolder.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
            newFolder.LastWriteTime = findData.ftLastWriteTime.ToDateTime();

            ((IFolder)owner).AddToFolders(newFolder);

            ReadFromFolder(fullpath, newFolder);
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

    }
}
