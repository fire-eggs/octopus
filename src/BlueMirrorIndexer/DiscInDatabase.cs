using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BlueMirrorIndexer.Components;
using Igorary.Forms;

namespace BlueMirrorIndexer
{

    [Serializable]
	public class DiscInDatabase : FolderInDatabase, IComparable<DiscInDatabase>
    {
        // for SQLite load
        public DiscInDatabase(uint dbid)
            : base(dbid, null)
        {
            Scanned = DateTime.Now;
            DriveType = DriveType.Unknown;
            VolumeLabel = "";
        }

        public DiscInDatabase() : base(null)
        {
            Scanned = DateTime.Now;
            DriveType = DriveType.Unknown;
            VolumeLabel = "";
        }

        public string DriveFormat { get; set; }

        public DriveType DriveType { get; set; }

        public long TotalFreeSpace { get; set; }

        public long TotalSize { get; set; }

        public string VolumeLabel { get; set; }

        public DateTime Scanned { get; set; }

        public int Flags { get; set; }

        private static int FLAG_CRC = 1;
        private static int FLAG_ZIP = 2;

        public bool ScannedCrc 
        {
            get { return (Flags & FLAG_CRC) == 1; }
            set { Flags = Flags ^ (value ? FLAG_CRC : 0); }
        }

        public bool ScannedZip
        {
            get { return (Flags & FLAG_ZIP) == 1; }
            set { Flags = Flags ^ (value ? FLAG_ZIP : 0); }
        }

        public string GetOptionsDescription() {
            string res = string.Empty;
            if (ScannedCrc)
                res += "CRC";
            if (ScannedZip) {
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

        public uint ClusterSize { get; set; }

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

        public override uint GetVolumeClusterSize()
        {
            return ClusterSize;
        }

        protected override Form CreateDialog() {
            return new DlgDiscProperties(this);
        }

        public override string ToString() {
            return Name;
        }

        // TODO KBR move to Win32.cs
        internal uint GetClusterSize(string drive)
        {
            uint sectorsPerCluster;
            uint bytesPerSector;
            uint junk1, junk2;
            Win32.GetDiskFreeSpace(drive, out sectorsPerCluster, out bytesPerSector, out junk1, out junk2);
            return sectorsPerCluster* bytesPerSector;
        }

        // TODO KBR move to FolderReader.cs
        internal void ReadFromDrive(string drive, List<string> excludedElements, DlgReadingProgress dlgReadingProgress, DiscInDatabase discToReplace)
        {
            var FR = new FolderReader(excludedElements, dlgReadingProgress, discToReplace);
            FR.ReadFromFolder(drive, this, discToReplace);

            DriveInfo di = new DriveInfo(drive);
            DriveFormat = di.DriveFormat;
            DriveType = di.DriveType;
            TotalFreeSpace = di.TotalFreeSpace;
            TotalSize = di.TotalSize;
            if (!string.IsNullOrEmpty(di.VolumeLabel))
                VolumeLabel = di.VolumeLabel;
            SerialNumber = Win32.GetVolumeSerialNumber(drive);
            ScannedCrc = Properties.Settings.Default.ComputeCrc;
            ScannedZip = Properties.Settings.Default.BrowseInsideCompressed;
            FromDrive = drive;

            // TODO KBR must calculate first and pass to FolderReader. That code must track running total cluster sizes.
            ClusterSize = GetClusterSize(drive);

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
