using Igorary.Forms;
using System;
using System.Collections.Generic;
using System.IO;
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
        public FolderInDatabase(int dbId, IFolder parent) : this(parent)
        {
            DbId = dbId; // TODO KBR for SQLite load
        }

	    readonly FolderImpl folderImpl;

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

	    public void AddToFiles(FileInDatabase file) {
            folderImpl.AddToFiles(file);
            file.Parent = this;
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

        public UInt64 TotalSizeUsed { get; set; }

        public void UpdateStats()
        {
            TotalSizeUsed = 0;
            TotalSizeUsed += (ulong)folderImpl.GetFilesSize();
            foreach (var folder in folderImpl.Folders)
            {
                folder.UpdateStats();
                TotalSizeUsed += folder.TotalSizeUsed;
            }
        }

    }

}
