using System;
using System.Collections.Generic;

namespace BlueMirrorIndexer.Components
{
    public class SearchEventArgs
    {
        public enum SearchDateType
        {
            None,
            CreateBetween,
            CreateBefore,
            CreateAfter,
            ModBetween,
            ModBefore,
            ModAfter,
        }

        public enum SearchSizeRange
        {
            Between,
            LessThan,
            MoreThan,
            None
        }

        public enum SearchSizeType
        {
            KB,
            MB,
            GB
        }

        public SearchEventArgs()
        {
            searchInVolumes = new List<DiscInDatabase>();
        }

        public SearchEventArgs(string fileMask, bool treatFileMaskAsWildcard, DateTime? dateFrom, DateTime? dateTo, long? sizeFrom, long? sizeTo, string keywords, bool allKeywordsNeeded, bool onlyDuplicates, List<DiscInDatabase> searchInVolumes, bool caseSensitiveKeywords, bool treatKeywordsAsWildcard) {
            this.FileMask = fileMask;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.SizeFrom = sizeFrom;
            this.SizeTo = sizeTo;
            this.Keywords = keywords;
            this.AllKeywordsNeeded = allKeywordsNeeded;
            this.OnlyDuplicates = onlyDuplicates;
            this.searchInVolumes = searchInVolumes;
            this.CaseSensitiveKeywords = caseSensitiveKeywords;
            this.TreatKeywordsAsWildcard = treatKeywordsAsWildcard;
            this.TreatFileMaskAsWildcard = treatFileMaskAsWildcard;
        }

        public bool TreatFileMaskAsWildcard { get; set; }

        public bool TreatKeywordsAsWildcard { get; set; }

        public bool CaseSensitiveKeywords { get; set; }

        readonly List<DiscInDatabase> searchInVolumes;

        internal List<DiscInDatabase> SearchInVolumes {
            get { return searchInVolumes; }
        }

        public bool OnlyDuplicates { get; set; }

        public string FileMask { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public long? SizeFrom { get; set; }

        public long? SizeTo { get; set; }

        public string Keywords { get; set; }

        public bool AllKeywordsNeeded { get; set; }

        public SearchDateType DateType { get; set; }

        public SearchSizeRange SizeType { get; set; }

        public SearchSizeType SizeFromType { get; set; }

        public SearchSizeType SizeToType { get; set; }

        private long sizeInBytes(long? val, SearchSizeType type)
        {
            if (!val.HasValue)
                return 0;

            switch (type)
            {
                case SearchSizeType.KB:
                    return val.Value * 1024;
                case SearchSizeType.MB:
                    return val.Value * 1024 * 1024;
                case SearchSizeType.GB:
                    return val.Value * 1024 * 1024 * 1024;
            }
            return 0;
        }

        public long SizeFromBytes()
        {
            return sizeInBytes(SizeFrom, SizeFromType);
        }

        public long SizeToBytes()
        {
            return sizeInBytes(SizeTo, SizeToType);
        }
    }
}
