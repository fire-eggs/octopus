using System;
using System.Windows.Forms;
using Igorary.Forms;
using Igorary.Utils.Extensions;

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

		public bool IsReadOnly { get; set; }

        protected override string GetItemType() {
            return "File";
        }

        internal override string GetCsvLine() {
            return base.GetCsvLine() + "," + Length;
        }

        public ulong Hash { get; set; }

        public string NameLengthKey {
            get {
                return Name + Length;
            }
        }

        // KBR TODO duplicated code?
        public override ListViewItem ToListViewItem() {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = Name;
            lvi.Tag = this;
            lvi.ImageIndex = Win32.GetFileIconIndex(Name, Win32.FileIconSize.Small);
            lvi.SubItems.Add(Length.ToKB());
            lvi.SubItems.Add(CreationTime.ToString("g"));
            lvi.SubItems.Add(LastWriteTime.ToString("g"));
            lvi.SubItems.Add(Attributes.ToString());

            lvi.SubItems.Add(Keywords);
            lvi.SubItems.Add(Extension);

            lvi.SubItems.Add(GetVolumeUserName());
            lvi.SubItems.Add(GetPath());
            if(Hash != 0)
                lvi.SubItems.Add(Hash.ToString("X"));
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
