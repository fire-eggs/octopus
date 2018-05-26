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

        public void LoadSearchSettings()
        {
            lvSearchResults.ColumnOrderArray = Settings.Default.SearchResultsColumnOrder;
            lvSearchResults.ColumnWidthArray = Settings.Default.SearchResultsColumnWidth;
            if (Settings.Default.SearchResultsSplitterPos > 0)
                splitContainer1.SplitterDistance = Settings.Default.SearchResultsSplitterPos;
        }

        public void SaveSearchSettings()
        {
            Settings.Default.SearchResultsColumnOrder = lvSearchResults.ColumnOrderArray;
            Settings.Default.SearchResultsColumnWidth = lvSearchResults.ColumnWidthArray;
            Settings.Default.SearchResultsSplitterPos = splitContainer1.SplitterDistance;
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
            if (!e.Control || e.KeyCode != Keys.A) 
                return;

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

        public void cmItemPropertiesFromSearch_Click(object sender, EventArgs e)
        {
            ItemInDatabase item = getSelectedItemInSearch();
            if (item == null)
                return;
            FrmMain.Instance.showItemProperties(item);
        }

        internal ItemInDatabase getSelectedItemInSearch()
        {
            if (lvSearchResults.SelectedIndices.Count != 1) 
                return null;
            int index = lvSearchResults.SelectedIndices[0];
            if ((index >= 0) && (index < searchResultList.Count))
                return searchResultList[index];
            return null;
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

        public void clearSearchList()
        {
            searchResultList.Clear();
            ShowSearchResults();
        }

        internal void updateStrip()
        {
            int count = 0;
            long sum = 0;
            bool selected = false;
            if (lvSearchResults.SelectedIndices.Count > 0)
            {
                // selected items
                count = lvSearchResults.SelectedIndices.Count;
                selected = true;
                foreach (int index in lvSearchResults.SelectedIndices)
                {
                    sum += searchResultList[index].Length;
                }
            }
            else
            {
                count = searchResultList.Count;
                foreach (ItemInDatabase iid in searchResultList)
                    if (iid is FileInDatabase)
                        sum += (iid as FileInDatabase).Length;
            }
            FrmMain.Instance.UpdateStatusBar(selected, count, sum);
        }

        private void UpdateCommands()
        {
            FrmMain.Instance.UpdateCommands();
        }

        private void pmSearchList_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cmFindInDatabase.Enabled = cmItemPropertiesFromSearch.Enabled = lvSearchResults.SelectedIndices.Count == 1;
        }

        private void lvSearchResults_Enter(object sender, EventArgs e)
        {
            UpdateCommands();
        }

        private void lvSearchResults_MouseDown(object sender, MouseEventArgs e)
        {
            if (lvSearchResults.SelectedIndices.Count > 0)
            {
                var itemsToDrag = new List<ItemInDatabase>();
                foreach (int index in lvSearchResults.SelectedIndices)
                    itemsToDrag.Add(searchResultList[index]);
                Size dragSize = SystemInformation.DragSize;
                var dragRect = new Rectangle(new Point(e.X - (dragSize.Width/2), e.Y - (dragSize.Height/2)), dragSize);
                RaiseDragStart(itemsToDrag, dragRect);
            }
            else
                RaiseDragEnd();
        }

        private void RaiseDragStart(List<ItemInDatabase> itemsToDrag, Rectangle dragRect)
        {
            if (DragStartHandler != null)
                DragStartHandler(this, new DragStartArgs {ItemsToDrag = itemsToDrag, DragRect = dragRect});
        }

        private void lvSearchResults_MouseMove(object sender, MouseEventArgs e)
        {
            if (DraggingHandler != null)
                DraggingHandler(this, new DraggingArgs {MouseEvent = e, Control = lvSearchResults});
        }

        private void lvSearchResults_MouseUp(object sender, MouseEventArgs e)
        {
            RaiseDragEnd();
        }

        private void RaiseDragEnd()
        {
            if (DragEndHandler != null)
                DragEndHandler(this, null);
        }

        public delegate void DragStarted(object sender, DragStartArgs e);

        public delegate void Dragging(object sender, DraggingArgs e);

        private event DragStarted DragStartHandler;
        private event EventHandler DragEndHandler;
        private event Dragging DraggingHandler;

        public event DragStarted DragStart
        {
            add { DragStartHandler += value; }
            remove { DragStartHandler -= value; }
        }
        public event EventHandler DragEnd
        {
            add { DragEndHandler += value; }
            remove { DragEndHandler -= value; }
        }
        public event Dragging DragGoing
        {
            add { DraggingHandler += value; }
            remove { DraggingHandler -= value; }
        }

        private void cmFindInDatabase_Click(object sender, EventArgs e)
        {
            if (lvSearchResults.SelectedIndices.Count != 1) 
                return;
            int index = lvSearchResults.SelectedIndices[0];
            ItemInDatabase itemInDatabase = searchResultList[index];
            FrmMain.Instance.findInTree(itemInDatabase);
        }

        private void showInWindowsExplorerToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (lvSearchResults.SelectedIndices.Count != 1)
                return;
            int index = lvSearchResults.SelectedIndices[0];
            ItemInDatabase itemInDatabase = searchResultList[index];
            FrmMain.Instance.ShowInExplorer(itemInDatabase);
        }

        private void SearchPanel_Load(object sender, EventArgs e)
        {
            if (Settings.Default.SearchResultsSplitterPos > 0)
                splitContainer1.SplitterDistance = Settings.Default.SearchResultsSplitterPos;
        }
    }

    public class DraggingArgs
    {
        public MouseEventArgs MouseEvent { get; set; }
        public Control Control { get; set; }
    }

    public class DragStartArgs
    {
        public List<ItemInDatabase> ItemsToDrag { get; set; }
        public Rectangle DragRect { get; set; }
    }

}
