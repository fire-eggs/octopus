using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Igorary.Forms;

namespace BlueMirrorIndexer
{

    [Serializable]
	public class DiscInDatabase : FolderInDatabase, IComparable<DiscInDatabase>
    {
        // for SQLite load
        public DiscInDatabase(int dbid)
            : base(dbid, null)
        {
            Scanned = DateTime.Now;
            DriveType = DriveType.Unknown;
        }

        public DiscInDatabase() : base(null)
        {
            Scanned = DateTime.Now;
            DriveType = DriveType.Unknown;
        }

        public string DriveFormat { get; set; }

        public DriveType DriveType { get; set; }

        public long TotalFreeSpace { get; set; }

        public long TotalSize { get; set; }

        public string VolumeLabel { get; set; }

        public DateTime Scanned { get; set; }

        [OptionalField]
        bool scannedCrc;

        public bool ScannedCrc {
            get { return scannedCrc; }
            set { scannedCrc = value; }
        }

        [OptionalField]
        bool scannedZip;

        public bool ScannedZip {
            get { return scannedZip; }
            set { scannedZip = value; }
        }

        [OptionalField]
        bool scannedFileVersion;

        public bool ScannedFileVersion {
            get { return scannedFileVersion; }
            set { scannedFileVersion = value; }
        }

        public string GetOptionsDescription() {
            string res = string.Empty;
            if (scannedCrc)
                res += "CRC";
            if (scannedFileVersion) {
                if (res != string.Empty)
                    res += ", ";
                res += "File versions";
            }
            if (scannedZip) {
                if (res != string.Empty)
                    res += ", ";
                res += "Browsed compressed files";
            }
            if (res == string.Empty)
                res = "(none)";
            return res;
        }

        public string SerialNumber { get; set; }

        public string PhysicalLocation { get; set; }

        public string FromDrive { get; set; }

        #region IComparable<DiscInDatabase> Members

		int IComparable<DiscInDatabase>.CompareTo(DiscInDatabase other) {
			return (Name.CompareTo(other.Name));
		}

		#endregion

        protected override string GetItemType() {
            return "Volume";
        }

        internal override string GetCsvLine() {
            return base.GetCsvLine() + "," + DriveFormat + "," + DriveType.ToString() + "," + TotalFreeSpace.ToString() + "," + TotalSize.ToString() + "," + VolumeLabel;
        }

        public override string GetVolumeUserName() {
            return Name;
        }

        protected override Form CreateDialog() {
            return new DlgDiscProperties(this);
        }

        public override string ToString() {
            return Name;
        }

        internal void ReadFromDrive(string drive, List<string> elementsToRead, ref long runningFileCount, ref long runningFileSize, bool useSize, DlgReadingProgress dlgReadingProgress, DiscInDatabase discToReplace) {
            ReadFromFolderKBR(drive, elementsToRead, ref runningFileCount, ref runningFileSize, useSize, dlgReadingProgress, discToReplace);
            DriveInfo di = new DriveInfo(drive);
            DriveFormat = di.DriveFormat;
            DriveType = di.DriveType;
            TotalFreeSpace = di.TotalFreeSpace;
            TotalSize = di.TotalSize;
            VolumeLabel = di.VolumeLabel;
            SerialNumber = Win32.GetVolumeSerialNumber(drive);
            scannedCrc = Properties.Settings.Default.ComputeCrc;
            scannedZip = Properties.Settings.Default.BrowseInsideCompressed;
            scannedFileVersion = Properties.Settings.Default.ReadFileInfo;
            FromDrive = drive;
            if (discToReplace != null) {
                if ((Keywords != string.Empty) && (discToReplace.Keywords != string.Empty))
                    Keywords = Keywords + ";";
                Keywords = Keywords + discToReplace.Keywords;
                if ((PhysicalLocation != string.Empty) && (discToReplace.PhysicalLocation != string.Empty))
                    PhysicalLocation = PhysicalLocation + ";";
                PhysicalLocation = PhysicalLocation + discToReplace.PhysicalLocation;
                foreach (LogicalFolder logicalFolder in discToReplace.LogicalFolders)
                    logicalFolder.AddItem(this);
            }
        }

        internal void RemoveFromDatabase(VolumeDatabase database) {
            RemoveFromLogicalFolders();
            database.RemoveDisc(this);
        }
        
    }

}
