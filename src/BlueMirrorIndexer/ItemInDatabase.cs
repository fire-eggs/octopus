using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BlueMirrorIndexer {

	[Serializable]
    public abstract class ItemInDatabase
    {
        public int DbId { get; set; } // For SQLite restore

	    public ItemInDatabase()
	    {
	        
	    }

	    public ItemInDatabase(IFolder parent, string ext)
	    {
	        // for sqlite restore
	        _parent = parent;
	        Extension = ext;
	    }

		public ItemInDatabase(IFolder parent)
		{
		    Length = 0;
            _parent = parent;
            LastWriteTime = DateTime.Now;
            LastAccessTime = DateTime.Now;
            Keywords = "";
            Description = "";
            Name = "";
            CreationTime = DateTime.Now;
            Attributes = FileAttributes.Normal;
            FullName = "";
            FileDescription = "";
            FileVersion = "";
		    Extension = "";
		}

	    IFolder _parent;

        internal IFolder Parent {
			get { return _parent; }
            set { _parent = value; } // for sqlite
		}

	    public string Keywords { get; set; }

	    public string Description { get; set; }

	    public string Name { get; set; }

	    public string FullName { get; set; }

	    public FileAttributes Attributes { get; set; }

	    public DateTime CreationTime { get; set; }

	    //string extension;

        // TODO on volume read, set extension to lowercase before storing
		public string Extension
		{
		    get;
		    set;
            //get { return extension; }
            //set { extension = value.ToLower(); }
		}

	    public DateTime LastAccessTime { get; set; }

	    public DateTime LastWriteTime { get; set; }

	    public virtual string GetVolumeUserName() {
			return (_parent as ItemInDatabase).GetVolumeUserName();
		}

		public string GetPath()
		{
		    if ((_parent != null) && !(_parent is DiscInDatabase)) // inheritance
                return (_parent as ItemInDatabase).GetPath() + (_parent as ItemInDatabase).Name + "\\";
		    return "\\";
		}

	    protected virtual Form CreateDialog() {
            return new DlgItemProperties(this);
        }

        public bool EditPropertiesDlg() {
            return (CreateDialog().ShowDialog() == DialogResult.OK);
        }

        public void WriteToStream(StreamWriter sw) {
           sw.WriteLine(GetCsvLine());
        }

        internal virtual string GetCsvLine() {
            return GetItemType() + ",\"" + GetVolumeUserName() + "\",\"" + GetPath() + "\",\"" + Name + "\",\"" + Attributes.ToString() + "\"," + CreationTime.ToString() + ",\"" + Description + "\",\"" + Keywords + "\"," + LastAccessTime.ToString() + "," + LastWriteTime.ToString();
        }

        protected abstract string GetItemType();

        public void GetPath(List<ItemInDatabase> pathList) {
            if (_parent != null)
                (_parent as ItemInDatabase).GetPath(pathList);
            pathList.Add(this);
        }

        public abstract ListViewItem ToListViewItem();

	    public string FileVersion { get; set; }

	    public string FileDescription { get; set; }

	    public long Length { get; set; }

	    public virtual void RemoveFromDatabase() {
            RemoveFromLogicalFolders();
        }

        #region Logical Folders

	    private List<LogicalFolder> logicalFolders; // = new List<LogicalFolder>();

        public List<LogicalFolder> LogicalFolders {
            get { if (logicalFolders == null) logicalFolders = new List<LogicalFolder>();
                return logicalFolders;
            }
        }

        internal void ApplyFolders(LogicalFolder[] newLogicalFolders, bool clearOldFolders) {
            if (clearOldFolders) {
                // po to, ¿eby mo¿na by³o daæ contains()
                List<LogicalFolder> newLogicalFolderList = new List<LogicalFolder>(newLogicalFolders);
                // == usuwanie ==
                // tymczasowa lista, ¿eby nie operowaæ bezpoœrednio na logicalFolders - wywala 
                // enumeracjê
                List<LogicalFolder> foldersToDelete = new List<LogicalFolder>();
                foreach (LogicalFolder oldFolder in logicalFolders)
                    if (!newLogicalFolderList.Contains(oldFolder))
                        foldersToDelete.Add(oldFolder);
                foreach (LogicalFolder folderToDelete in foldersToDelete)
                    folderToDelete.RemoveItem(this);
            }
            // == dodawanie ==
            foreach (LogicalFolder newFolder in newLogicalFolders) {
                if (!logicalFolders.Contains(newFolder))
                    newFolder.AddItem(this); // w tym jest this.logicalFolders.Add(newFolder)
            }
        }

        public virtual void RemoveFromLogicalFolders()
        {
            if (logicalFolders == null)
                return;
            List<LogicalFolder> listToDelete = new List<LogicalFolder>(logicalFolders);
            foreach (LogicalFolder lf in listToDelete)
                lf.RemoveItem(this);
        }

        #endregion

    }
}
