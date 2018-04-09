/* 
 * Copyright © 2018 by Kevin Routley.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// ReSharper disable InconsistentNaming

// TODO KBR folder to replace?
// TODO KBR why is 'ComputeCRC' in default settings rather than some per-volume tracking?
// TODO KBR CRC was used for compatibility with ZIP files!

namespace BlueMirrorIndexer
{
    public class FolderReader
    {
        private long _runningFileCount;
        private long _runningFileSize;
        private readonly List<string> _excludedFolders;
        private readonly DlgReadingProgress _dlgReadingProgress;
        private FolderInDatabase _folderToReplace; // TODO KBR is this at the disc-only level??

        public FolderReader(List<string> excludedFolders, DlgReadingProgress dlgReadingProgress, FolderInDatabase folderToReplace)
        {
            _excludedFolders = excludedFolders;
            _dlgReadingProgress = dlgReadingProgress;
            _folderToReplace = folderToReplace;

            _runningFileCount = 0;
            _runningFileSize = 0;

            if (Properties.Settings.Default.ComputeCrc)
            {
                _md5 = new MD5CryptoServiceProvider();
            }
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

        private MD5CryptoServiceProvider _md5;

        // Arbitrary threshold: don't calculate hash for files larger than 250M
        private const uint M250 = 250*1024*1024;

        internal long ProcessFile(FolderInDatabase owner, WIN32_FIND_DATAW findData, string fullpath)
        {
            var newFile = new FileInDatabase(owner);

            ProcessCommon(newFile, findData, fullpath);

            newFile.IsReadOnly = (findData.dwFileAttributes & FileAttributes.ReadOnly) != 0;

            long highSize = (uint)findData.nFileSizeHigh;
            highSize = highSize << 32;
            highSize += (uint)findData.nFileSizeLow;
            newFile.Length = highSize;

            if (Properties.Settings.Default.ComputeCrc &&
                highSize < M250)
            {
                try
                {
                    var buf = File.ReadAllBytes(fullpath);
                    var hash = _md5.ComputeHash(buf);
                    UInt64 hash2 = BitConverter.ToUInt64(hash, 0);
                    newFile.Hash = hash2;
                }
                catch (Exception)
                {
                    // File might be locked
                }
            }

            // TODO KBR compressed files


            ((IFolder)owner).AddToFiles(newFile);
            return newFile.Length;
        }

        internal void ProcessFolder(FolderInDatabase owner, WIN32_FIND_DATAW findData, string fullpath)
        {
            if (_excludedFolders.Contains(fullpath.ToLower()))
                return;

            FolderInDatabase newFolder = new FolderInDatabase(owner);

            ProcessCommon(newFolder, findData, fullpath);
            ((IFolder)owner).AddToFolders(newFolder);

            ReadFromFolder(fullpath, newFolder);
        }

        private void ProcessCommon(ItemInDatabase item, WIN32_FIND_DATAW findData, string fullpath)
        {
            item.Name = findData.cFileName;
            item.Attributes = findData.dwFileAttributes;
            string tmp = Path.GetExtension(fullpath);
            if (tmp.StartsWith("."))
                tmp = tmp.Substring(1);
            item.Extension = tmp;

            item.FullName = fullpath;
            item.CreationTime = findData.ftCreationTime.ToDateTime();
            item.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
            item.LastWriteTime = findData.ftLastWriteTime.ToDateTime();
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

/* Original read code for hystorical reference
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
 */