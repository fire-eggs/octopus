using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlueMirrorIndexer.Components;
using BlueMirrorIndexer.Properties;
using Igorary.Forms;
using Igorary.Utils.Utils.Extensions;

namespace BlueMirrorIndexer
{
    public partial class FrmMain : Form
    {

        private ItemInDatabase getSelectedItemInSearch()
        {
            if (lvSearchResults.SelectedIndices.Count == 1)
            {
                int index = lvSearchResults.SelectedIndices[0];
                if ((index >= 0) && (index < searchResultList.Count))
                    return searchResultList[index];
            }
            return null;
        }

        private void SaveSearchSettings()
        {
            Settings.Default.SearchResultsColumnOrder = lvSearchResults.ColumnOrderArray;
            Settings.Default.SearchResultsColumnWidth = lvSearchResults.ColumnWidthArray;
            
        }

        IComparer<ItemInDatabase> searchListComparer = null;
        int lastColInSearchView = -1;
        private void lvSearchResults_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            int col = e.Column;
            bool ascending;
            if (lastColInSearchView == col)
            {
                ascending = false;
                lastColInSearchView = -1;
            }
            else
            {
                ascending = true;
                lastColInSearchView = col;
            }
            searchListComparer = new SearchResultComparer(col, ascending);
            displaySearchList();
        }

        private void displaySearchList()
        {
            lvSearchResults.VirtualListSize = 0;
            if (searchListComparer != null)
                searchResultList.Sort(searchListComparer);
            lvSearchResults.VirtualListSize = searchResultList.Count;
            updateStrip();
        }

        //private void lvSearchResults_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
        //    updateStrip();
        //}

        private readonly List<ItemInDatabase> searchResultList = new List<ItemInDatabase>();

        private void filesSearchCriteriaPanel_SearchBtnClicked(object sender, SearchEventArgs e)
        {
            search(e, searchResultList);
            displaySearchList();
        }

        private void search(SearchEventArgs e, List<ItemInDatabase> list)
        {
            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // usuwanie podtekstów ".*", gdy przed tekstem nie ma œrednika lub pocz¹tku tekstu, a za tekstem jest œrednik lub koniec tekstu
                int i = 0;
                while ((i = e.FileMask.IndexOf(".*", i, StringComparison.Ordinal)) > -1)
                {
                    // i > -1
                    if ((i > 0) && (e.FileMask[i - 1] != ';') && ((i == e.FileMask.Length - 2) || (e.FileMask[i + 2] == ';')))
                        e.FileMask = e.FileMask.Substring(0, i) + e.FileMask.Substring(i + 2);
                }

                Regex fileMaskRegex = new Regex(e.FileMask.ToRegex(e.TreatFileMaskAsWildcard), RegexOptions.Compiled | RegexOptions.IgnoreCase);

                KeywordMatcher keywordMatcher = new KeywordMatcher(e.Keywords, e.AllKeywordsNeeded, e.CaseSensitiveKeywords, e.TreatKeywordsAsWildcard);

                list.Clear();

                if (e.OnlyDuplicates)
                {
                    List<FileInDatabase> foundFilesCrc = new List<FileInDatabase>();
                    List<FileInDatabase> foundFilesNoCrc = new List<FileInDatabase>();

                    foreach (DiscInDatabase disc in e.SearchInVolumes)
                        disc.InsertFilesToList(fileMaskRegex, e.DateFrom, e.DateTo, e.SizeFrom, e.SizeTo, keywordMatcher, foundFilesCrc, foundFilesNoCrc);

                    foundFilesCrc.Sort(new FileComparer(true));
                    FileComparer noCrcComparer = new FileComparer(false);
                    foundFilesNoCrc.Sort(noCrcComparer);
                    FileInDatabase lastFile = null; ulong lastCrc = 0;
                    foreach (FileInDatabase file in foundFilesCrc)
                    {
                        if (file.Hash != 0)
                        {
                            if (lastCrc != file.Hash)
                            {
                                lastCrc = file.Hash;
                                lastFile = file;
                            }
                            else
                            {
                                if (lastFile != null)
                                { // lastFile dodajemy tylko raz
                                    insertSimilarToList(lastFile, foundFilesNoCrc, list, noCrcComparer);
                                    list.Add(lastFile);
                                    lastFile = null;
                                }
                                list.Add(file);
                                insertSimilarToList(file, foundFilesNoCrc, list, noCrcComparer);
                            }
                        }
                    }
                    lastFile = null; string lastKey = null;
                    foreach (FileInDatabase file in foundFilesNoCrc)
                    {
                        if (lastKey != file.NameLengthKey)
                        {
                            lastKey = file.NameLengthKey;
                            lastFile = file;
                        }
                        else
                        {
                            if (lastFile != null)
                            { // lastFile dodajemy tylko raz
                                list.Add(lastFile);
                                lastFile = null;
                            }
                            list.Add(file);
                        }
                    }
                }
                else
                    foreach (DiscInDatabase disc in e.SearchInVolumes)
                        disc.InsertFilesToList(fileMaskRegex, e.DateFrom, e.DateTo, e.SizeFrom, e.SizeTo, keywordMatcher, list /*, lvSearchResults*/);
            }
            finally
            {
                Cursor.Current = oldCursor;
            }
        }

        private static void insertSimilarToList(FileInDatabase file, List<FileInDatabase> foundFilesNoCrc, List<ItemInDatabase> list, FileComparer noCrcComparer)
        {
            int index;
            do
            {
                index = foundFilesNoCrc.BinarySearch(file, noCrcComparer);
                if (index >= 0)
                {
                    list.Add(foundFilesNoCrc[index]);
                    foundFilesNoCrc.RemoveAt(index);
                }
            }
            while (index > 0);
        }

        private void updateSearchListImages()
        {
            Win32.UpdateSystemImageList(lvSearchResults.SmallImageList, Win32.FileIconSize.Small, false, Resources.delete);
        }

        private void lvSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!duringSelectAll)
            {
                updateStrip();
                UpdateCommands();
            }
        }

        private void updateStripSearch()
        {
            long sum = 0;
            if (lvSearchResults.SelectedIndices.Count > 0)
            {
                // selected items
                sbFiles.Text = Resources.SelectedFiles + ": " + lvSearchResults.SelectedIndices.Count;

                foreach (int index in lvSearchResults.SelectedIndices)
                {
                    sum += searchResultList[index].Length;
                }
            }
            else
            {
                sbFiles.Text = Resources.Files + ": " + searchResultList.Count;
                foreach (ItemInDatabase iid in searchResultList)
                    if (iid is FileInDatabase)
                        sum += (iid as FileInDatabase).Length;
            }
            sbSize.Text = Resources.Size + ": " + sum.ToKB();
        }

        private void clearSearchList()
        {
            searchResultList.Clear();
            displaySearchList();
        }

        private void cmFindInDatabase_Click(object sender, EventArgs e)
        {
            if (lvSearchResults.SelectedIndices.Count == 1)
            {
                int index = lvSearchResults.SelectedIndices[0];
                ItemInDatabase itemInDatabase = searchResultList[index];
                findInTree(itemInDatabase);
            }
        }

        private void cmsSearchList_Opening(object sender, CancelEventArgs e)
        {
            cmFindInDatabase.Enabled = cmItemPropertiesFromSearch.Enabled = lvSearchResults.SelectedIndices.Count == 1;
        }

        private void cmItemPropertiesFromSearch_Click(object sender, EventArgs e)
        {
            ItemInDatabase item = getSearchSelectedItem();
            if (item != null)
                showItemProperties(item);
        }

        private ItemInDatabase getSearchSelectedItem()
        {
            if (lvSearchResults.SelectedIndices.Count == 1)
            {
                int index = lvSearchResults.SelectedIndices[0];
                if ((index >= 0) && (index < searchResultList.Count))
                    return searchResultList[index];
            }
            return null;
        }

        private void lvSearchResults_DoubleClick(object sender, EventArgs e)
        {
            cmItemPropertiesFromSearch_Click(sender, e);
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

        private void lvSearchResults_Enter(object sender, EventArgs e)
        {
            UpdateCommands();
        }

    }
}
