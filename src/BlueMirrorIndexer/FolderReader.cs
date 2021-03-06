﻿/* 
 * MIT License. See license.txt for details.
 * 
 * Copyright © 2018 by github.com/fire-eggs.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Igorary.Forms;

// ReSharper disable InconsistentNaming

// TODO KBR folder to replace
// TODO KBR why is 'ComputeCRC' in default settings rather than some per-volume tracking?
// TODO KBR CRC was used for compatibility with ZIP files!

namespace BlueMirrorIndexer
{
    public static class FILETIMEExtensions
    {
        public static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME filetime)
        {
            long highBits = filetime.dwHighDateTime;
            highBits = highBits << 32;
            return DateTime.FromFileTimeUtc(highBits + (long)filetime.dwLowDateTime);
        }
    }

    public class FolderReader
    {
        private long _runningFileCount;
        private long _runningFileSize;
        private readonly List<string> _excludedItems;
        private readonly DlgReadingProgress _dlgReadingProgress;
        private FolderInDatabase _folderToReplace;

        public FolderReader(List<string> excludedItems, DlgReadingProgress dlgReadingProgress, FolderInDatabase folderToReplace)
        {
            _excludedItems = excludedItems;
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
                Win32.WIN32_FIND_DATAW findData;
                findHandle = Win32.FindFirstFileW(folder + @"\*", out findData);
                if (findHandle == INVALID_HANDLE_VALUE)
                    return;

                do
                {
                    if (findData.cFileName == "." || findData.cFileName == "..")
                        continue;

                    string fullpath = Path.Combine(folder, findData.cFileName).ToLower(); // folder + (folder.EndsWith("\\") ? "" : "\\") + findData.cFileName;

                    if (_excludedItems.Contains(fullpath))
                        continue;

                    // KBR TODO folder to replace

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
                while (Win32.FindNextFile(findHandle, out findData));
            }
            finally
            {
                if (findHandle != INVALID_HANDLE_VALUE) Win32.FindClose(findHandle);
            }
        }

        static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private MD5CryptoServiceProvider _md5;

        // Arbitrary threshold: don't calculate hash for files larger than 250M
        private const uint M250 = 250*1024*1024;

        internal FileInDatabase ProcessCompressed(FolderInDatabase owner, string fullpath)
        {
            FileInDatabase newFile;
            if (Properties.Settings.Default.BrowseInsideCompressed && (CompressedFile.IsCompressedFile(fullpath)))
            {
                newFile = new CompressedFile(owner);
                CompressedFile cf = newFile as CompressedFile;
                try
                {
                    cf.BrowseFiles(fullpath, null); //fileToReplace as CompressedFile);
                }
                catch (Exception ex)
                {
                    // TODO KBR how to replicate this?
                    //cf.Comments = ex.Message;
                }
                ((IFolder)owner).AddToFolders(cf);
            }
            else
            {
                newFile = new FileInDatabase(owner);
            }
            return newFile;
        }

        internal long ProcessFile(FolderInDatabase owner, Win32.WIN32_FIND_DATAW findData, string fullpath)
        {
            //var newFile = new FileInDatabase(owner);
            var newFile = ProcessCompressed(owner, fullpath);
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

            ((IFolder)owner).AddToFiles(newFile);
            return newFile.Length;
        }

        internal void ProcessFolder(FolderInDatabase owner, Win32.WIN32_FIND_DATAW findData, string fullpath)
        {
            FolderInDatabase newFolder = new FolderInDatabase(owner);

            ProcessCommon(newFolder, findData, fullpath);
            ((IFolder)owner).AddToFolders(newFolder);

            ReadFromFolder(fullpath, newFolder);
        }

        private static void ProcessCommon(ItemInDatabase item, Win32.WIN32_FIND_DATAW findData, string fullpath)
        {
            item.Name = findData.cFileName;
            item.Attributes = findData.dwFileAttributes;
            string tmp = Path.GetExtension(fullpath);
            if (!string.IsNullOrEmpty(tmp))
            {
                tmp = tmp.ToLower();
                if (tmp.StartsWith("."))
                    tmp = tmp.Substring(1);
            }
            item.Extension = tmp;

            item.FullName = fullpath;
            item.CreationTime = findData.ftCreationTime.ToDateTime();
            item.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
            item.LastWriteTime = findData.ftLastWriteTime.ToDateTime();
        }
    }
}

/* Original read code for hystorical reference
        internal void ReadFromFolder(string folder, List<string> excludedItems, ref long runningFileCount, ref long runningFileSize, bool useSize, DlgReadingProgress dlgReadingProgress, FolderInDatabase folderToReplace) {
            try {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folder);
                System.IO.DirectoryInfo[] subFolders = di.GetDirectories();
                foreach (System.IO.DirectoryInfo subFolder in subFolders) {
                    if (!excludedItems.Contains(subFolder.FullName.ToLower())) {
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
                        newFolder.ReadFromFolder(subFolder.FullName, excludedItems, ref runningFileCount, ref runningFileSize, useSize, dlgReadingProgress, subFolderToReplace);
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
                    if (!excludedItems.Contains(fileInFolder.FullName.ToLower())) {
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