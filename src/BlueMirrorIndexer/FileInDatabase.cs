using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Igorary.Forms;
using Igorary.Utils.Utils.Extensions;

namespace BlueMirrorIndexer
{

    [Serializable]
	public class FileInDatabase : ItemInDatabase {

        public FileInDatabase()
        {
        }

        public FileInDatabase(IFolder parent, string ext) : base(parent, ext)
        {
            // for sqlite load
        }

		public FileInDatabase(IFolder parent)
			: base(parent) {
		}

        public FileInDatabase(int dbid, string name, string ext, string fullname, 
            FileAttributes attrib, long len, string keyw)
        {
            _dbId = dbid;
            _name = name;
            _extension = ext;
            _fullName = fullname;
            _attributes = attrib;
            _length = len;
            _keywords = keyw;
        }

        public void SetTimes(Int64 createTicks, Int64 accessTicks, Int64 writeTicks)
        {
            _createTime = new DateTime(createTicks);
            _accessTime = new DateTime(accessTicks);
            _writeTime = new DateTime(writeTicks);
        }
        public bool IsReadOnly { get; set; }

        protected override string GetItemType() {
            return "File";
        }

        internal override string GetCsvLine() {
            return base.GetCsvLine() + "," + Length;
        }

        //public ulong Hash { get; set; }

        public uint CRC { get; set; }

        public string NameLengthKey {
            get {
                return Name + Length;
            }
        }

        private static readonly Dictionary<string,int> iconIndex = new Dictionary<string, int>();
        private int getFileIconIndex(string name, string ext)
        {
            int dex;
            if (iconIndex.TryGetValue(ext, out dex))
                return dex;

            dex = Win32.GetFileIconIndex(name, Win32.FileIconSize.Small);
            iconIndex[ext] = dex;
            return dex;
        }

        // KBR TODO duplicated code?
        public override ListViewItem ToListViewItem() {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = Name;
            lvi.Tag = this;
            lvi.ImageIndex = getFileIconIndex(Name, Extension);
            lvi.SubItems.Add(Length.ToKB());
            lvi.SubItems.Add(CreationTime.ToString("g"));
            lvi.SubItems.Add(LastWriteTime.ToString("g"));
            lvi.SubItems.Add(Attributes.ToString());

            lvi.SubItems.Add(Keywords);
            lvi.SubItems.Add(Extension);

            lvi.SubItems.Add(GetVolumeUserName());
            lvi.SubItems.Add(GetPath());
            if(CRC != 0)
                lvi.SubItems.Add(CRC.ToString("X8"));
            else
                lvi.SubItems.Add(string.Empty);
            return lvi;
        }

        protected override Form CreateDialog() {
            return new DlgFileProperties(this);
        }

        public override void RemoveFromDatabase() {
            base.RemoveFromDatabase();
            Parent.RemoveFromFiles(this);
        }

        //internal virtual void RemoveFromDatabase() {
        //    RemoveFromAllLogicalFolders();
        //    Parent.RemoveFromFiles(this);
        //}
    }

}
