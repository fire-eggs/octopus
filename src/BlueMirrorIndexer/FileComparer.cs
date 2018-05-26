using System;
using System.Collections.Generic;

namespace BlueMirrorIndexer
{
    class FileComparer : IComparer<FileInDatabase>
    {
        readonly bool _crcComparing;

        public FileComparer(bool crcComparing) {
            _crcComparing = crcComparing;
        }

        #region IComparer<FileInDatabase> Members

        public int Compare(FileInDatabase x, FileInDatabase y) {
            if (_crcComparing)
                return x.CRC.CompareTo(y.CRC);
            return String.Compare(x.NameLengthKey, y.NameLengthKey, StringComparison.Ordinal);
        }

        #endregion
    }
}
