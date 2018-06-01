using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BlueMirrorIndexer {

	[Serializable]
    public abstract class ItemInDatabase
    {
	    public int DbId
	    {
	        get { return _dbId; }
	        set { _dbId = value; }
	    } // For SQLite restore

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

	    public string Keywords
	    {
	        get { return _keywords; }
	        set { _keywords = value; }
	    }

	    public string Description { get; set; }

	    public string Name
	    {
	        get { return _name; }
	        set { _name = value; }
	    }

	    public string FullName
	    {
	        get { return _fullName; }
	        set { _fullName = value; }
	    }

	    public FileAttributes Attributes
	    {
	        get { return _attributes; }
	        set { _attributes = value; }
	    }

	    public DateTime CreationTime
	    {
	        get { return _createTime; }
	        set { _createTime = value; }
	    }

	    //string extension;

	    // TODO on volume read, set extension to lowercase before storing
	    public string Extension
	    {
	        get { return _extension; }
	        set { _extension = value; }
	        //get { return extension; }
	        //set { extension = value.ToLower(); }
	    }

	    public DateTime LastAccessTime
	    {
	        get { return _accessTime; }
	        set { _accessTime = value; }
	    }

	    public DateTime LastWriteTime
	    {
	        get { return _writeTime; }
	        set { _writeTime = value; }
	    }

	    public virtual string GetVolumeUserName()
	    {
	        var iid = _parent as ItemInDatabase;
	        if (iid == null)
	            return "";
			return iid.GetVolumeUserName();
		}

        public virtual uint GetVolumeClusterSize()
        {
            // percolates up the parent chain until the disc is reached
            return (_parent as ItemInDatabase).GetVolumeClusterSize();
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

        // TODO Kill this!
	    public string FileVersion { get; set; }

        // TODO Kill this!
        public string FileDescription { get; set; }

	    protected long _length;
        public long Length { get { return _length; } set { _length = value; } }

	    public virtual void RemoveFromDatabase() {
            RemoveFromLogicalFolders();
        }

        #region Logical Folders

	    private List<LogicalFolder> logicalFolders; // = new List<LogicalFolder>();
	    protected string _name;
	    protected int _dbId;
	    protected string _extension;
	    protected FileAttributes _attributes;
	    protected string _fullName;
	    protected string _keywords;
	    protected DateTime _accessTime;
	    protected DateTime _writeTime;
	    protected DateTime _createTime;

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
