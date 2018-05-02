using System;
using System.Collections.Generic;

namespace BlueMirrorIndexer.Components
{
    public class SearchEventArgs
    {

        public SearchEventArgs()
        {
            searchInVolumes = new List<DiscInDatabase>();
        }

        public SearchEventArgs(string fileMask, bool treatFileMaskAsWildcard, DateTime? dateFrom, DateTime? dateTo, long? sizeFrom, long? sizeTo, string keywords, bool allKeywordsNeeded, bool onlyDuplicates, List<DiscInDatabase> searchInVolumes, bool caseSensitiveKeywords, bool treatKeywordsAsWildcard) {
            this.fileMask = fileMask;
            this.dateFrom = dateFrom;
            this.dateTo = dateTo;
            this.sizeFrom = sizeFrom;
            this.sizeTo = sizeTo;
            this.keywords = keywords;
            this.allKeywordsNeeded = allKeywordsNeeded;
            this.onlyDuplicates = onlyDuplicates;
            this.searchInVolumes = searchInVolumes;
            this.caseSensitiveKeywords = caseSensitiveKeywords;
            this.treatKeywordsAsWildcard = treatKeywordsAsWildcard;
            this.treatFileMaskAsWildcard = treatFileMaskAsWildcard;
        }

        bool treatFileMaskAsWildcard;

        public bool TreatFileMaskAsWildcard
        {
            get { return treatFileMaskAsWildcard; }
            set { treatFileMaskAsWildcard = value; }
        }

        bool treatKeywordsAsWildcard;

        public bool TreatKeywordsAsWildcard
        {
            get { return treatKeywordsAsWildcard; }
            set { treatKeywordsAsWildcard = value; }
        }

        bool caseSensitiveKeywords;

        public bool CaseSensitiveKeywords {
            get { return caseSensitiveKeywords; }
        }

        List<DiscInDatabase> searchInVolumes;

        internal List<DiscInDatabase> SearchInVolumes {
            get { return searchInVolumes; }
        }

        bool onlyDuplicates;

        public bool OnlyDuplicates {
            get { return onlyDuplicates; }
        }

        private string fileMask;
        public string FileMask {
            get { return fileMask; }
            set { fileMask = value; }
        }

        private DateTime? dateFrom;
        public DateTime? DateFrom
        {
            get { return dateFrom; }
            set { dateFrom = value; }
        }

        private DateTime? dateTo;
        public DateTime? DateTo
        {
            get { return dateTo; }
            set { dateTo = value; }
        }

        private long? sizeFrom;
        public long? SizeFrom
        {
            get { return sizeFrom; }
            set { sizeFrom = value; }
        }

        private long? sizeTo;
        public long? SizeTo
        {
            get { return sizeTo; }
            set { sizeTo = value; }
        }

        private string keywords;
        public string Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }

        private bool allKeywordsNeeded;
        public bool AllKeywordsNeeded
        {
            get { return allKeywordsNeeded; }
            set { allKeywordsNeeded = value; }
        }
    }
}
