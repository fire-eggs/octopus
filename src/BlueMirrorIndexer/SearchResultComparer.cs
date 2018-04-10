using System.Collections.Generic;

namespace BlueMirrorIndexer
{
    class SearchResultComparer : IComparer<ItemInDatabase>
    {
        private readonly int col;
        private readonly bool ascending;
        public SearchResultComparer() {
            col = 0;
        }

        public SearchResultComparer(int column, bool ascending) {
            col = column;
            this.ascending = ascending;
        }

        private ulong crc(ItemInDatabase itemInDatabase) {
            if (itemInDatabase is FileInDatabase)
                return (itemInDatabase as FileInDatabase).Hash;
            return 0;
        }

        #region IComparer<ItemInDatabase> Members

        public int Compare(ItemInDatabase x, ItemInDatabase y) {
            int res;

            // KBR TODO hard-coded column order!
            switch (col) {
                case 0: res = x.Name.CompareTo(y.Name); break;
                case 1: res = x.Length.CompareTo(y.Length); break;
                case 2: res = x.CreationTime.CompareTo(y.CreationTime); break;
                case 3: res = x.LastWriteTime.CompareTo(y.LastWriteTime); break;
                case 4: res = x.Attributes.CompareTo(y.Attributes); break;
                case 6: res = x.Extension.CompareTo(y.Extension); break;
                case 7: res = x.GetVolumeUserName().CompareTo(y.GetVolumeUserName()); break;
                case 8: res = x.GetPath().CompareTo(y.GetPath()); break;
                case 9: res = crc(x).CompareTo(crc(y)); break;
                case 5: res = x.Keywords.CompareTo(y.Keywords); break; // TODO KBR WTF? how is this column 5???
                default: res = 0; break;
            }
            if (!ascending)
                // res = res < 0 ? 1 : res > 0 ? -1 : 0; // byæ mo¿e najbardziej optymalnie
                res = -res;
            return res;
        }

        #endregion
    }
}
