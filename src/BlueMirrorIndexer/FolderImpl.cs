using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BlueMirrorIndexer.Components;

namespace BlueMirrorIndexer
{
    [Serializable]
    public class FolderImpl
    {
        readonly ItemInDatabase owner;
        readonly int imageIndex;
        public FolderImpl(ItemInDatabase owner, int imageIndex) {
            this.owner = owner;
            this.imageIndex = imageIndex;
        }

        #region Files

        List<FileInDatabase> files = new List<FileInDatabase>();

        public FileInDatabase[] Files {
            get { return files.ToArray(); }
        }

        public void AddToFiles(FileInDatabase file) {
            files.Add(file);
        }

        public void RemoveFromFiles(FileInDatabase file) {
            files.Remove(file);
        }

        public FileInDatabase FindFile(string fileName) {
            fileName = fileName.ToLower();
            foreach (FileInDatabase file in files)
                if (file.Name.ToLower() == fileName)
                    return file;
            return null;
        }

        public int FileCount {
            get { return files.Count; }
        }

        public long GetFilesSize() {
            long size = 0;
            foreach (FileInDatabase fid in Files)
                size += fid.Length;
            return size;
        }

        #endregion

        #region Folders

        List<IFolder> folders = new List<IFolder>();

        public IFolder[] Folders {
            get { return folders.ToArray(); }
        }

        public void AddToFolders(IFolder folder) {
            folders.Add(folder);
        }

        public void RemoveFromFolders(IFolder folder) {
            folders.Remove(folder);
        }

        public FolderInDatabase FindFolder(string folderName) {
            folderName = folderName.ToLower();
            foreach (IFolder folder in folders)
                if ((folder as ItemInDatabase).Name.ToLower() == folderName)
                    return folder as FolderInDatabase;
            return null;
        }

        #endregion

        #region Searching

        /// <summary>
        /// Odnajduje tylko pliki (bez folderów).
        /// </summary>
        public void InsertFilesToList(Regex regex, DateTime? dateFrom, DateTime? dateTo, long? sizeFrom, long? sizeTo, KeywordMatcher keywordMatcher, List<FileInDatabase> listCrc, List<FileInDatabase> listNoCrc) {
            foreach (FileInDatabase file in files)
                if (regex.IsMatch(file.Name)
                    && ((dateFrom == null) || ((file.CreationTime >= dateFrom) && (file.CreationTime <= dateTo)))
                    && ((sizeFrom == null) || ((file.Length >= sizeFrom) && (file.Length <= sizeTo)))
                    && (keywordMatcher.IsMatch(file.Keywords))) {

                    if (file.CRC == 0)
                        listNoCrc.Add(file);
                    else
                        listCrc.Add(file);
                }

            foreach (IFolder folder in folders)
                folder.InsertFilesToList(regex, dateFrom, dateTo, sizeFrom, sizeTo, keywordMatcher, listCrc, listNoCrc);
        }

        public void InsertFilesToList(Regex regex, DateTime? dateFrom, DateTime? dateTo, long? sizeFrom, long? sizeTo, KeywordMatcher keywordMatcher, List<ItemInDatabase> list) {

            if (!(owner is CompressedFile) && regex.IsMatch(owner.Name)
                    && ((dateFrom == null) || ((owner.CreationTime >= dateFrom) && (owner.CreationTime <= dateTo)))
                    && (keywordMatcher.IsMatch(owner.Keywords))) {
                list.Add(owner);
            }

            foreach (FileInDatabase file in files)
                if (regex.IsMatch(file.Name)
                    && ((dateFrom == null) || ((file.CreationTime >= dateFrom) && (file.CreationTime <= dateTo)))
                    && ((sizeFrom == null) || ((file.Length >= sizeFrom) && (file.Length <= sizeTo)))
                    && (keywordMatcher.IsMatch(file.Keywords))) {
                    list.Add(file);
                }

            foreach (IFolder folder in folders)
                folder.InsertFilesToList(regex, dateFrom, dateTo, sizeFrom, sizeTo, keywordMatcher, list);
        }

        public void InsertFilesToList(Regex fileMaskRegex, SearchEventArgs.SearchDateType dateType, DateTime? dateFrom, DateTime? dateTo, SearchEventArgs.SearchSizeRange sizeType, long sizeFromBytes, long sizeToBytes, KeywordMatcher tagMatcher, List<ItemInDatabase> list)
        {
            // TODO have the filters return yes/no pass?

            // TODO owner
            foreach (var file in files)
            {
                if (dateType != SearchEventArgs.SearchDateType.None)
                {
                    // TODO modified date
                    switch (dateType)
                    {
                        case SearchEventArgs.SearchDateType.Before:
                            if (file.CreationTime >= dateFrom)
                                continue;
                            break;
                        case SearchEventArgs.SearchDateType.After:
                            if (file.CreationTime <= dateFrom)
                                continue;
                            break;
                        case SearchEventArgs.SearchDateType.Between:
                            if (file.CreationTime < dateFrom || file.CreationTime > dateTo)
                                continue;
                            break;
                    }
                }
                if (sizeType != SearchEventArgs.SearchSizeRange.None)
                {
                    switch (sizeType)
                    {
                        case SearchEventArgs.SearchSizeRange.LessThan:
                            if (file.Length >= sizeFromBytes)
                                continue;
                            break;
                        case SearchEventArgs.SearchSizeRange.MoreThan:
                            if (file.Length <= sizeFromBytes)
                                continue;
                            break;
                        case SearchEventArgs.SearchSizeRange.Between:
                            if (file.Length < sizeFromBytes || file.Length > sizeToBytes)
                                continue;
                            break;
                    }
                }

                if (!fileMaskRegex.IsMatch(file.Name)) // Test filename after faster filters
                    continue;

                // Passed all filters! 
                if (tagMatcher.IsMatch(file.Keywords))
                    list.Add(file);
            }

            foreach (IFolder folder in folders)
                folder.InsertFilesToList(fileMaskRegex, dateType, dateFrom, dateTo, sizeType, sizeFromBytes, sizeToBytes, tagMatcher, list);

        }


        #endregion

        #region Logical Folders

        public void RemoveFromLogicalFolders() {
            foreach (IFolder folder in folders)
                (folder as ItemInDatabase).RemoveFromLogicalFolders();
            foreach (FileInDatabase file in files)
                file.RemoveFromLogicalFolders();
        }

        #endregion

        public void CopyToNode(TreeNode treeNode)
        {
            string name = owner.Name;
            ulong size;
            if (owner is CompressedFile)
                size = (ulong) owner.Length;
            else
                size = (owner as FolderInDatabase).TotalSizeUsed;

            treeNode.Text = string.Format("{0} ({1})", name, FormatNice(size));
            treeNode.Tag = owner;
            treeNode.ImageIndex = imageIndex;
            treeNode.SelectedImageIndex = imageIndex;
            foreach (IFolder fid in Folders) {
                TreeNode tn = new TreeNode();
                fid.CopyToNode(tn);
                treeNode.Nodes.Add(tn);
            }
        }

        private static string FormatNice(ulong val)
        {
            if (val < 1024)
                return FormatAsKb(val);
            return FormatAsMb(val);
        }

        private static string FormatAsKb(ulong val)
        {
            double val2 = val / 1024.0;
            return string.Format("{0:0,0.##}K", val2);
        }

        private static string FormatAsMb(ulong val)
        {
            double val2 = val / 1024.0 / 1024.0;
            return string.Format("{0:0,0.##}M", val2);
        }
    }
}
