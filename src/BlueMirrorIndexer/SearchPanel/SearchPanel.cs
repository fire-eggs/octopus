using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BlueMirror.Commons.Controls;
using BlueMirrorIndexer.Components;
using BlueMirrorIndexer.Properties;
using BlueMirrorIndexer.SearchFilters;
using System.Drawing;
using System.Windows.Forms;
using Igorary.Forms;
using Igorary.Utils.Utils.Extensions;

// ReSharper disable InconsistentNaming


namespace BlueMirrorIndexer.SearchPanel
{
    public partial class SearchPanel : UserControl
    {
        private readonly VolPanel2 volP;
        private readonly List<ItemInDatabase> searchResultList = new List<ItemInDatabase>();
        IComparer<ItemInDatabase> searchListComparer;
        int lastColInSearchView = -1;
        bool duringSelectAll;
        private readonly SearchEventArgs sea = new SearchEventArgs();

        public SearchPanel()
        {
            InitializeComponent();

            accordion1.Font = new Font("Microsoft Sans Serif", 10);
            var fileP = new FilePanel();
            var sizeP = new SizePanel();
            var tagP = new TagPanel();
            var dateP = new DatePanel();
            volP = new VolPanel2();
            var miscP = new OtherPanel();

            // TODO restore last open/closed states
            var txt = fileP.GetText();
            accordion1.Add(fileP, txt, txt, open: true);
            txt = dateP.GetText();
            accordion1.Add(dateP, txt, txt, open: false);
            txt = sizeP.GetText();
            accordion1.Add(sizeP, txt, txt, open: false);
            txt = tagP.GetText();
            accordion1.Add(tagP, txt, txt, open: false);
            txt = volP.GetText();
            accordion1.Add(volP, txt, txt, open: false);
            txt = miscP.GetText();
            accordion1.Add(miscP, txt, txt, open: false);

            // TODO can this be automated somehow?
            fileP.FilterChange += FilterChange;
            sizeP.FilterChange += FilterChange;
            dateP.FilterChange += FilterChange;
            tagP.FilterChange += FilterChange;
            volP.FilterChange += FilterChange;
            miscP.FilterChange += FilterChange;
        }

        void FilterChange(object sender, FilterChangeArgs e)
        {
            accordion1.SetText(sender as Control, e.FilterText);
            accordion1.SetTooltip(sender as Control, e.FilterText);
        }

        public void UpdateVolumeList(VolumeDatabase database)
        {
            if (volP != null)
                volP.UpdateVolumeList(database);
        }

        public ListViewVista GetSearchList()
        {
            return lvSearchResults;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            FetchSearchFilters();
            PerformSearch();
            ShowSearchResults();
        }

        private void FetchSearchFilters()
        {
            for (int i = 0; i < accordion1.Count; i++)
            {
                IFilterPanel ctl = accordion1.Content(i) as IFilterPanel;
                if (ctl != null)
                    ctl.GetFilter(sea);
            }
        }

        private void PerformSearch()
        {
            int i = 0;
            while ((i = sea.FileMask.IndexOf(".*", i, StringComparison.Ordinal)) > -1)
            {
                // i > -1
                if ((i > 0) && (sea.FileMask[i - 1] != ';') && ((i == sea.FileMask.Length - 2) || (sea.FileMask[i + 2] == ';')))
                    sea.FileMask = sea.FileMask.Substring(0, i) + sea.FileMask.Substring(i + 2);
            }

            Regex fileMaskRegex = new Regex(sea.FileMask.ToRegex(sea.TreatFileMaskAsWildcard), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            KeywordMatcher keywordMatcher = new KeywordMatcher(sea.Keywords, sea.AllKeywordsNeeded, sea.CaseSensitiveKeywords, sea.TreatKeywordsAsWildcard);

            searchResultList.Clear();

            long sizeFromBytes = sea.SizeFromBytes();
            long sizeToBytes = sea.SizeToBytes();

            // TODO duplicates
            // TODO new search options
            // TODO DiscInDatabase should return a list
            foreach (DiscInDatabase disc in sea.SearchInVolumes)
                disc.InsertFilesToList(fileMaskRegex, sea.DateType, sea.DateFrom, sea.DateTo, sea.SizeType, sizeFromBytes, sizeToBytes, keywordMatcher, searchResultList);
                //disc.InsertFilesToList(fileMaskRegex, sea.DateFrom, sea.DateTo, sea.SizeFrom, sea.SizeTo, keywordMatcher, searchResultList);
        }

        private void ShowSearchResults()
        {
            lvSearchResults.VirtualListSize = 0;
            if (searchListComparer != null)
                searchResultList.Sort(searchListComparer);
            lvSearchResults.VirtualListSize = searchResultList.Count;
            updateStrip();
        }

        private void lvSearchResults_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            int col = e.Column;

            // click on a new column, start in ascending
            bool ascending = lastColInSearchView != col;
            lastColInSearchView = (lastColInSearchView == col) ? -1 : col;
            searchListComparer = new SearchResultComparer(col, ascending);
            ShowSearchResults();
        }

        private void lvSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!duringSelectAll)
            {
                updateStrip();
                UpdateCommands(); // TODO ???
            }
        }

        private void lvSearchResults_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                e.SuppressKeyPress = true;
                duringSelectAll = true;
                try
                {
                    lvSearchResults.SelectAll();
                }
                finally
                {
                    duringSelectAll = false;
                }
                updateStrip();
            }
        }

        private void lvSearchResults_DoubleClick(object sender, EventArgs e)
        {
            // TODO cmItemPropertiesFromSearch_Click(sender, e);
        }

        #region Search result list virtual mode

        int firstCachedItem = -1;
        List<ListViewItem> cachedItems = null;

        private void lvSearchResults_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            firstCachedItem = e.StartIndex;
            cachedItems = new List<ListViewItem>();
            for (int i = firstCachedItem; i <= e.EndIndex; i++)
                cachedItems.Add(searchResultList[i].ToListViewItem());
            updateSearchListImages();
        }

        private void lvSearchResults_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if ((cachedItems != null) && (e.ItemIndex - firstCachedItem < cachedItems.Count) && (e.ItemIndex - firstCachedItem >= 0))
                e.Item = cachedItems[e.ItemIndex - firstCachedItem];
            else
                e.Item = searchResultList[e.ItemIndex].ToListViewItem();
        }

        private void updateSearchListImages()
        {
            Win32.UpdateSystemImageList(lvSearchResults.SmallImageList, Win32.FileIconSize.Small, false, Resources.delete);
        }

        #endregion


        private void updateStrip()
        {
            // TODO notify main form
        }

        private void UpdateCommands()
        {
            // TODO notify main form
        }
    }
}
