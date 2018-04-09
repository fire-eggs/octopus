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
                return x.Hash.CompareTo(y.Hash);
            return String.Compare(x.NameLengthKey, y.NameLengthKey, System.StringComparison.Ordinal);
        }

        #endregion
    }
}
