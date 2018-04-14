using BlueMirrorIndexer.Properties;
using Igorary.Forms;
using Igorary.Forms.Components;
using Igorary.Utils.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

// ReSharper disable InconsistentNaming

namespace BlueMirrorIndexer
{
    public partial class FrmMain : Form
    {

        public static FrmMain Instance;

        public FrmMain() {
            InitializeComponent();
            if (!Settings.Default.Updated) {
                Settings.Default.Upgrade();
                Settings.Default.Updated = true;
            }
            cmScanNewMedia.Checked = Settings.Default.ScanNewMedia;
            Text = string.Format("{0} {1}", ProductName, ProductVersion);
            btnSave.Enabled = cmSave.Enabled = false;

            charting.MainForm = this;
        }

        private void updateControls() {
            updateTree();
            updateLogicalFolders();
            UpdateCommands();
            clearSearchList();
        }

        private void updateVolumesInSearchCriterias() {
            filesSearchCriteriaPanel.UpdateVolumeList(Database);
        }

        private DiscInDatabase getSelectedDisc() {
            if ((tvDatabaseFolderTree.SelectedNode != null) && (tvDatabaseFolderTree.SelectedNode.Tag is DiscInDatabase))
                return tvDatabaseFolderTree.SelectedNode.Tag as DiscInDatabase;
            return null;
        }

        private FolderInDatabase getSelectedFolder() {
            if ((tvDatabaseFolderTree.SelectedNode != null) && (tvDatabaseFolderTree.SelectedNode.Tag is FolderInDatabase))
                return tvDatabaseFolderTree.SelectedNode.Tag as FolderInDatabase;
            return null;
        }

        private CompressedFile getSelectedCompressedFile() {
            if ((tvDatabaseFolderTree.SelectedNode != null) && (tvDatabaseFolderTree.SelectedNode.Tag is CompressedFile))
                return tvDatabaseFolderTree.SelectedNode.Tag as CompressedFile;
            return null;
        }

        private FileInDatabase getSelectedFile() {
            if (lvDatabaseItems.SelectedItems.Count == 1)
                return (lvDatabaseItems.SelectedItems[0].Tag as FileInDatabase);
            return null;
        }

        private ItemInDatabase getSelectedTreeItem()
        {
            if (tvDatabaseFolderTree.SelectedNode != null)
                return tvDatabaseFolderTree.SelectedNode.Tag as ItemInDatabase;
            return null;
        }

        #region Menu commands and events

        private void cmVolumeFolderProperties_Click(object sender, EventArgs e)
        {
            ItemInDatabase iid = getSelectedTreeItem();
            showItemProperties(iid);
            if (iid is DiscInDatabase)
            {
                // TODO KBR wipes volume size data!
                tvDatabaseFolderTree.SelectedNode.Text = iid.Name; // view any user changes to label
            }
        }

        private void cmDeleteTreeItemPopup_Click(object sender, EventArgs e)
        {
            //ItemInDatabase iid = getSelectedTreeItem();
            //if (iid == null)
            //    return;
            //iid.DeleteWithConfirm();

            if (getSelectedDisc() != null) {
                if (MessageBox.Show(String.Format(Resources.AreUSureToDeleteVolume, getSelectedDisc().Name), ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    deleteCdInfo(getSelectedDisc());
                }
            }
            else if (getSelectedFolder() != null) {
                if (MessageBox.Show(String.Format(Resources.AreUSureToDeleteFolder, getSelectedFolder().Name), ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    deleteFolderInfo(getSelectedFolder());
                }
            }
            else {
                CompressedFile compressedFile = getSelectedCompressedFile();
                if (compressedFile != null) {
                    if (MessageBox.Show(String.Format(Resources.AreUSureToDeleteFile, compressedFile.Name), ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        deleteCompressedFile(compressedFile);
                    }
                }
            }
        }

        private void cmAbout_Click(object sender, EventArgs e) {
            using (DlgAbout dlg = new DlgAbout()) {
                dlg.ShowDialog(this);
            }
        }

        private void cmFileProperties_Click(object sender, EventArgs e) {
            if (getSelectedFile() != null)
                showItemProperties(getSelectedFile());
        }

        private void cmDeleteFileInfoPopup_Click(object sender, EventArgs e) {
            if (getSelectedFile() != null) {
                if (MessageBox.Show(String.Format(Resources.AreUSureToDeleteFile, getSelectedFile().Name), ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    deleteFileInfo(getSelectedFile());
            }
            else if (lvDatabaseItems.SelectedItems.Count > 0)
                if (MessageBox.Show("Are you sure to delete selected file information?", ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    bool compressedFileDeleted = false;
                    foreach (ListViewItem lvi in lvDatabaseItems.SelectedItems) {
                        FileInDatabase fid = lvi.Tag as FileInDatabase;
                        fid.RemoveFromDatabase();
                        if (fid is CompressedFile)
                            compressedFileDeleted = true;
                    }
                    if (compressedFileDeleted)
                        updateTree();
                    else
                        updateList();
                    fileOperations.Modified = true;
                    UpdateLogicalElements();
                }
        }

        private void cmProperties_Click(object sender, EventArgs e) {
            if (tvDatabaseFolderTree.Focused)
                cmVolumeFolderProperties_Click(sender, e);
            else
                if (lvDatabaseItems.Focused)
                    cmFileProperties_Click(sender, e);
                else
                    if (lvFolderElements.Focused)
                        cmItemPropertiesFromFolders_Click(sender, e);
                    else
                        if (lvSearchResults.Focused)
                            cmItemPropertiesFromSearch_Click(sender, e);
        }

        private void cmDelete_Click(object sender, EventArgs e) {
            if (tvDatabaseFolderTree.Focused)
                cmDeleteTreeItemPopup_Click(sender, e);
            else
                if (lvDatabaseItems.Focused)
                    cmDeleteFileInfoPopup_Click(sender, e);
        }

        private void cmHomePage_Click(object sender, EventArgs e)
        {
            Process.Start("http://covalition.github.io/octopus/");
        }

        private void cmFeatureRequests_Click(object sender, EventArgs e) {
            Process.Start("https://github.com/fire-eggs/octopus/issues");
        }

        private void cmWhatsNew_Click(object sender, EventArgs e) {
            Process.Start("https://github.com/fire-eggs/octopus/commits/master");
        }

        #endregion

        #region Updating controls

        public void UpdateCommands() {
            DiscInDatabase selectedDisc = getSelectedDisc();
            FolderInDatabase selectedFolder = getSelectedFolder();
            FileInDatabase selectedFile = getSelectedFile();
            CompressedFile selectedCompressedFile = getSelectedCompressedFile();
            ItemInDatabase selectedItemInSearch = getSelectedItemInSearch();
            ItemInDatabase selectedElementInFolders = getSelectedElementInFolder();
            if (selectedDisc != null) {
                cmDeleteTreeItemPopup.Text = Resources.DeleteVolume;
                cmTreeItemPropertiesPopup.Text = Resources.VolumeProperties;
            }
            else if (selectedFolder != null) {
                cmDeleteTreeItemPopup.Text = Resources.DeleteFolder;
                cmTreeItemPropertiesPopup.Text = Resources.FolderProperties;
            }
            else
                if (selectedCompressedFile != null) {
                    cmDeleteTreeItemPopup.Text = Resources.DeleteFile;
                    cmTreeItemPropertiesPopup.Text = Resources.FileProperties;
                }
                else {
                    // unknown item
                    cmDeleteTreeItemPopup.Text = "Delete";
                    cmTreeItemPropertiesPopup.Text = "Item Properties";
                }
            bool filesSelected = lvDatabaseItems.SelectedItems.Count > 0;
            cmDeleteTreeItemPopup.Enabled = (selectedFolder != null) || (selectedCompressedFile != null);
            cmTreeItemPropertiesPopup.Enabled = (selectedDisc != null) || (selectedFolder != null) || (selectedCompressedFile != null);

            cmItemPropertiesFromList.Enabled = selectedFile != null;
            cmDeleteListItemPopup.Enabled = filesSelected;
            btnProperties.Enabled = cmPropertiesFrm.Enabled = (tvDatabaseFolderTree.Focused && ((selectedDisc != null) || (selectedFolder != null))) || (lvDatabaseItems.Focused && (selectedFile != null)) || (lvFolderElements.Focused && (selectedElementInFolders != null)) || (lvSearchResults.Focused && (selectedItemInSearch != null));

            btnFindInDatabase.Enabled = cmFindInDatabaseFrm.Enabled = (lvFolderElements.Focused && (selectedElementInFolders != null)) || (lvSearchResults.Focused && (selectedItemInSearch != null));

            btnDelete.Enabled = cmDeleteFrm.Enabled = (tvDatabaseFolderTree.Focused && ((selectedDisc != null) || (selectedFolder != null))) || (lvDatabaseItems.Focused && filesSelected);

            cmMainRemoveFromFolder.Enabled = btnRemoveFromFolder.Enabled = lvFolderElements.Focused && (lvFolderElements.SelectedItems.Count > 0);
            cmRemoveFromFolder.Enabled = cmItemPropertiesFromFolders.Enabled = cmFindInDatabaseFromFolders.Enabled = lvFolderElements.SelectedItems.Count > 0;
        }

        private ItemInDatabase getSelectedElementInFolder() {
            if (lvFolderElements.SelectedItems.Count == 1)
                return (lvFolderElements.SelectedItems[0].Tag as ItemInDatabase);
            return null;
        }

        private void updateList() {
            lvDatabaseItems.Items.Clear();
            if (tvDatabaseFolderTree.SelectedNode != null) {
                IFolder fid = (IFolder)tvDatabaseFolderTree.SelectedNode.Tag;
                if (fid != null) {
                    Cursor c = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                    lvDatabaseItems.BeginUpdate();
                    try {
                        foreach (FileInDatabase fileid in fid.Files) {
                            ListViewItem lvi = fileid.ToListViewItem();
                            lvDatabaseItems.Items.Add(lvi);
                        }
                        Win32.UpdateSystemImageList(lvDatabaseItems.SmallImageList, Win32.FileIconSize.Small, false, Resources.delete);
                    }
                    finally {
                        lvDatabaseItems.EndUpdate();
                        Cursor.Current = c;
                    }
                }
            }
            updateStrip();
        }

        private void updateTree() {
            tvDatabaseFolderTree.BeginUpdate();
            try {
                tvDatabaseFolderTree.Nodes.Clear();
                using (new HourGlass()) {
                    foreach (DiscInDatabase fid in Database.GetDiscs()) {
                        TreeNode tn = new TreeNode();
                        fid.CopyToNode(tn);
                        tn.ImageIndex = 0;
                        tn.SelectedImageIndex = 0;
                        tvDatabaseFolderTree.Nodes.Add(tn);
                    }
                }
            }
            finally {
                tvDatabaseFolderTree.EndUpdate();
            }
            updateList();
            updateVolumesInSearchCriterias();
        }

        private void updateStrip() {
            if (tcMain.SelectedTab == tpDatabase) {
                if (lvDatabaseItems.SelectedItems.Count > 0) {
                    // selected items
                    sbFiles.Text = Resources.SelectedFiles + ": " + lvDatabaseItems.SelectedItems.Count;
                    long sum = 0;
                    foreach (ListViewItem lvi in lvDatabaseItems.SelectedItems)
                        sum += (lvi.Tag as FileInDatabase).Length;
                    sbSize.Text = Resources.Size + ": " + sum.ToKB();
                }
                else
                    if (tvDatabaseFolderTree.SelectedNode != null) {
                        // none is selected
                        IFolder fid = (IFolder)tvDatabaseFolderTree.SelectedNode.Tag;
                        if (fid != null) {
                            sbFiles.Text = Resources.Files + ": " + fid.FileCount;
                            sbSize.Text = Resources.Size + ": " + fid.GetFilesSize().ToKB();
                        }
                    }
                    else {
                        sbFiles.Text = Resources.NoFiles;
                        sbSize.Text = "";
                    }
            }
            else if (tcMain.SelectedTab == tpSearch) {
                updateStripSearch();
            }
        }

        #endregion

        #region Form events

        public static uint QueryCancelAutoPlay = 0;
        private void FrmMain_Load(object sender, EventArgs e) {
            updateVolumeButtons();

            lvDatabaseItems.ColumnOrderArray = Settings.Default.DatabaseItemsColumnOrder;
            lvDatabaseItems.ColumnWidthArray = Settings.Default.DatabaseItemsColumnWidth;

            lvFolderElements.ColumnOrderArray = Settings.Default.FolderElementsColumnOrder;
            lvFolderElements.ColumnWidthArray = Settings.Default.FolderElementsColumnWidth;

            lvSearchResults.ColumnOrderArray = Settings.Default.SearchResultsColumnOrder;
            lvSearchResults.ColumnWidthArray = Settings.Default.SearchResultsColumnWidth;

            startRefreshDiscs();
            QueryCancelAutoPlay = Win32.RegisterWindowMessage("QueryCancelAutoPlay");
            ilTree.ColorDepth = ColorDepth.Depth32Bit; // nic nie daje na razie
            ilTree.Images.Add(Win32.GetFileIconAsImage("test.zip", Win32.FileIconSize.Small));

            Application.DoEvents();
            string lastOpenedFile = Settings.Default.LastOpenedFile;
            fileOperations.OpenFile(lastOpenedFile);
        }

        private void updateVolumeButtons() {
            pmVolumes.DropDownItems.Clear();
            btnReadVolume.DropDownItems.Clear();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo di in drives) {
                ToolStripMenuItem bi1 = new ToolStripMenuItem(), bi2 = new ToolStripMenuItem();
                bi2.Text = bi1.Text = di.Name;
                bi2.Image = bi1.Image = Win32.GetFileIconAsImage(di.Name, Win32.FileIconSize.Small);
                bi2.Tag = bi1.Tag = di.Name;
                bi1.Click += cmReadVolume_Click;
                bi2.Click += cmReadVolume_Click;
                bi2.ToolTipText = bi1.ToolTipText = string.Format("Read from {0}", di.Name);
                try {
                    bi1.Name = createReadVolumeBtnName(di.Name);
                }
                catch { }
                pmVolumes.DropDownItems.Add(bi1);
                btnReadVolume.DropDownItems.Add(bi2);
            }
            UpdateReadVolumeButton();
        }

        private static string createReadVolumeBtnName(string drive) {
            drive = drive.Trim(':', '\\');
            return "cmReadVolumeFromDrive" + drive;
        }

        internal void UpdateReadVolumeButton() {
            string drive = Settings.Default.LastDrive;
            if ((cmReadVolume.Tag == null) || (drive.ToUpper() != cmReadVolume.Tag.ToString().ToUpper())) {
                btnReadVolume.ToolTipText = cmReadVolume.Text = string.Format("Read {0}...", drive);
                btnReadVolume.Image = cmReadVolume.Image = Win32.GetFileIconAsImage(drive, Win32.FileIconSize.Small);
                btnReadVolume.Tag = cmReadVolume.Tag = drive;
            }
        }

        void cmReadVolume_Click(object sender, EventArgs e) {
            readVolumeFromToolStripItemTag(sender as ToolStripItem);
        }

        private void readVolumeFromToolStripItemTag(ToolStripItem buttonItem) {
            if ((buttonItem.Tag != null) && (buttonItem.Tag is string)) {
                string drive = buttonItem.Tag as string;
                Settings.Default.LastDrive = drive;
                UpdateReadVolumeButton();
                startReading(drive);
            }
            else
                readVolume();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e) {
            try {
                breakCalculating = true;
                Settings.Default.DatabaseItemsColumnOrder = lvDatabaseItems.ColumnOrderArray;
                Settings.Default.FolderElementsColumnOrder = lvFolderElements.ColumnOrderArray;

                Settings.Default.DatabaseItemsColumnWidth = lvDatabaseItems.ColumnWidthArray;
                Settings.Default.FolderElementsColumnWidth = lvFolderElements.ColumnWidthArray;

                Settings.Default.LastOpenedFile = fileOperations.CurrentFilePath;

                SaveSearchSettings();
                Settings.Default.Save();
            }
            catch (Exception ex) {
                MessageBox.Show(string.Format("Error occurred during saving configuration: {0}", ex.Message), ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Control events

        private void tvDatabaseFolderTree_AfterSelect(object sender, TreeViewEventArgs e) {
            updateList();
            UpdateCommands();
        }

        private void lvDatabaseItems_SelectedIndexChanged(object sender, EventArgs e) {
            updateStrip();
            UpdateCommands();
        }

        private void tvDatabaseFolderTree_Enter(object sender, EventArgs e) {
            UpdateCommands();
        }

        private void tvDatabaseFolderTree_Leave(object sender, EventArgs e) {
            UpdateCommands();
        }

        private void lvDatabaseItems_Enter(object sender, EventArgs e) {
            UpdateCommands();
        }

        private void lvDatabaseItems_Leave(object sender, EventArgs e) {
            UpdateCommands();
        }

        private void tcMain_Selected(object sender, TabControlEventArgs e) {
            UpdateCommands();
            AcceptButton = tcMain.SelectedTab == tpSearch ? filesSearchCriteriaPanel.BtnSearch : null;
            updateStrip();
        }

        #region Search result list virtual mode

        int firstCachedItem = -1;
        List<ListViewItem> cachedItems = null;
        private void lvSearchResults_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e) {
            firstCachedItem = e.StartIndex;
            cachedItems = new List<ListViewItem>();
            for (int i = firstCachedItem; i <= e.EndIndex; i++)
                cachedItems.Add(searchResultList[i].ToListViewItem());
            updateSearchListImages();
        }

        private void lvSearchResults_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
            if ((cachedItems != null) && (e.ItemIndex - firstCachedItem < cachedItems.Count) && (e.ItemIndex - firstCachedItem >= 0))
                e.Item = cachedItems[e.ItemIndex - firstCachedItem];
            else
                e.Item = searchResultList[e.ItemIndex].ToListViewItem();
            //if (e.Item.SubItems.Count != 11)
            //    Debug.WriteLine(e.Item.Text + " " +  (e.Item.Tag as ItemInDatabase).GetCsvLine() + " " + e.Item.SubItems.Count);
        }

        #endregion

        private void lvDatabaseItems_DoubleClick(object sender, EventArgs e) {
            cmProperties_Click(sender, e);
        }

        int lastColInListView = -1;
        private void lvDatabaseItems_ColumnClick(object sender, ColumnClickEventArgs e) {
            int col = e.Column;
            bool ascending;
            if (lastColInListView == col) {
                ascending = false;
                lastColInListView = -1;
            }
            else {
                ascending = true;
                lastColInListView = col;
            }
            lvDatabaseItems.ListViewItemSorter = new DatabaseItemComparer(e.Column, ascending);
        }

        #endregion


        void closeOpenedProgressDialog() {
            if (openProgressDialog != null) {
                openProgressDialog.Close();
                openProgressDialog = null;
            }
        }

        /// <param name="progress">0..100</param>
        //void streamWithEvents_ProgressChanged(int progress) {
        //    if (openProgressDialog != null) {
        //        openProgressDialog.SetProgress(progress, null);
        //    }
        //}

        private static void saveAsCsv(string filePath) {
            try {
                Database.SaveAsCsv(filePath);
            }
            catch (Exception e) {
                MessageBox.Show(string.Format(Resources.ErrorSavingFile, filePath, e.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal static VolumeDatabase Database;

        #region Read Volume

        private void readVolume() {
            string drive;
            if (selectedDrive(out drive))
                startReading(drive);
        }

        bool duringRead = false;
        private void startReading(string drive) {
            if (duringRead)
                return;
            try {
                duringRead = true;
                List<string> excludedElements = new List<string>();
                LogicalFolder[] logicalFolders;
                DiscInDatabase discToReplace;
                DiscInDatabase discInDatabase = DlgReadVolume.GetOptions(excludedElements, drive, out logicalFolders, this, Database, out discToReplace);
                if (discInDatabase != null) {
                    readCdOnDrive(drive, discInDatabase, excludedElements, logicalFolders, discToReplace);
                    if (Settings.Default.AutoEject)
                        ejectCd(drive);
                    if (Settings.Default.AutosaveAfterReading)
                        // saveFile();
                        fileOperations.Save();
                }
            }
            catch (IOException e) {
                MessageBox.Show(string.Format(Resources.ErrorIO, e.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (AbortException) {
            }
            finally {
                duringRead = false;
            }
        }

        private void ejectCd(string drive) {
            try {
                Win32.Eject(drive);
            }
            catch (Exception ex) {
                showError(ex.Message);
            }
        }

        internal ProgressInfo ProgressInfo = null;
        private void readCdOnDrive(string drive, DiscInDatabase discToScan, List<string> excludedElements, LogicalFolder[] logicalFolders, DiscInDatabase discToReplace) {
            Cursor c = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try {
                excludedElements.Sort();

                Enabled = false;
                ProgressInfo = null;
                startCalculatingProgressInfo(drive, excludedElements);
                bool useSize = Settings.Default.ComputeCrc;
                openProgressDialog = new DlgReadingProgress("Reading Volume...", null, useSize);
                openProgressDialog.StartShowing(new TimeSpan(0, 0, 1));
                try {
                    if (!excludedElements.Contains(drive.ToLower())) // TODO KBR doesn't make sense? user chose to read this drive, should not be excluded?
                    {
                        try {
                            discToScan.ReadFromDrive(drive, excludedElements, openProgressDialog as DlgReadingProgress, discToReplace);
                            openProgressDialog.SetProgress(100, "Adding: " + discToScan.VolumeLabel);
                            Database.AddDisc(discToScan);
                        }
                        catch {
                            // jezeli wystapi³ blad podczas ReadFromDrive a jednoczeœnie przekopiowaliœmy z discToReplace foldery logiczne, to teraz wywal ze wszystkich folderów
                            if (discToReplace != null)
                                discToScan.RemoveFromLogicalFolders();
                            throw;
                        }
                        discToScan.ApplyFolders(logicalFolders, false);
                        if (discToReplace != null)
                            discToReplace.RemoveFromDatabase(Database);
                        sortByLabels();
                        fileOperations.Modified = true;
                    }
                }
                finally {
                    breakCalculating = true;
                    Enabled = true;
                    closeOpenedProgressDialog();
                }
            }
            finally {
                Cursor.Current = c;
            }
        }

        string calculatingDrive;
        List<string> calculatingExcludedElements;
        private void startCalculatingProgressInfo(string drive, List<string> excludedFolders) {
            calculatingDrive = drive;
            calculatingExcludedElements = excludedFolders;
            breakCalculating = false;
            ThreadStart calculateDelegate = calculateProgressInfo;
            Thread thread = new Thread(calculateDelegate);
            thread.Start();
        }

        private void calculateProgressInfo() {
            long fileCount = 0;
            long fileSizeSum = 0;
            try {
                calculateProgressInfo(calculatingDrive, calculatingExcludedElements, ref fileCount, ref fileSizeSum);
                lock (this) {
                    ProgressInfo = new ProgressInfo(fileCount, fileSizeSum);
                }
            }
            catch (AbortException) {
                ProgressInfo = null;
            }
        }

        bool breakCalculating = false;
        private void calculateProgressInfo(string calculatingFolder, List<string> calculatingExcludedElements, ref long fileCount, ref long fileSizeSum) {
            try {
                if (breakCalculating)
                    throw new AbortException();
                DirectoryInfo di = new DirectoryInfo(calculatingFolder);
                DirectoryInfo[] subFolders = di.GetDirectories();
                foreach (DirectoryInfo subFolder in subFolders) {
                    string subFolderName = subFolder.FullName.ToLower();
                    if (!calculatingExcludedElements.Contains(subFolderName)) {
                        calculateProgressInfo(subFolderName, calculatingExcludedElements, ref fileCount, ref fileSizeSum);
                    }
                }

                FileInfo[] filesInFolder = di.GetFiles();
                foreach (FileInfo fileInFolder in filesInFolder) {
                    if (!calculatingExcludedElements.Contains(fileInFolder.FullName.ToLower())) {
                        fileCount++;
                        fileSizeSum += fileInFolder.Length;
                    }
                }
            }
            catch (UnauthorizedAccessException) {
                // eat the exception
            }
        }

        private bool selectedDrive(out string drive) {
            return DlgSelectDrive.SelectDrive(out drive, this);
        }

        #endregion

        #region Database related

        private void sortByLabels() 
        {
            // TODO KBR needs better name. invoked after read-volume completed.


            Database.UpdateStats(); // TODO KBR need a subscriber model for chart
            charting.Database = Database;


            Database.SortDiscs();
            updateTree();

        }

        private void deleteCdInfo(DiscInDatabase cdInDatabase) {
            // CdsInDatabase.Remove(cdInDatabase);
            cdInDatabase.RemoveFromDatabase(Database);
            updateTree();
            UpdateLogicalElements();
            fileOperations.Modified = true;
        }

        private void deleteFolderInfo(FolderInDatabase folderInDatabase) {
            folderInDatabase.RemoveFromDatabase();
            updateTree();
            fileOperations.Modified = true;
        }

        private void deleteFileInfo(FileInDatabase fileInDatabase) {
            fileInDatabase.RemoveFromDatabase();
            if (fileInDatabase is CompressedFile) {
                //((CompressedFile)fileInDatabase).Parent.Folders.Remove((CompressedFile)fileInDatabase);
                updateTree();
            }
            else
                updateList();
            UpdateLogicalElements();
            fileOperations.Modified = true;
        }

        private void deleteCompressedFile(CompressedFile compressedFile) {
            compressedFile.RemoveFromDatabase(); // Parent.Folders.Remove(compressedFile);
            // compressedFile.Parent.Files.Remove(compressedFile);
            updateTree();
            UpdateLogicalElements();
            fileOperations.Modified = true;
        }

        #region Show properties

        private bool showItemProperties(ItemInDatabase itemInDatabase)
        {
            if (itemInDatabase == null)
                return false;
            bool result = itemInDatabase.EditPropertiesDlg();
            if (result) {
                fileOperations.Modified = true;
                UpdateLogicalElements();
            }
            return result;
        }

        #endregion

        #endregion


        bool duringSelectAll = false;

        public void findInTree(ItemInDatabase itemInDatabase) {
            List<ItemInDatabase> pathList = new List<ItemInDatabase>();
            itemInDatabase.GetPath(pathList);
            TreeNode lastNode = null;
            bool found = false;
            ListViewItem selectedItem = null;
            foreach (ItemInDatabase itemInPathList in pathList) {
                if (itemInPathList is IFolder)
                {
                    TreeNodeCollection nodes = lastNode == null ? tvDatabaseFolderTree.Nodes : lastNode.Nodes;
                    foreach (TreeNode node in nodes)
                        if (node.Tag == itemInPathList) {
                            lastNode = node;
                            found = true;
                            break;
                        }
                }
                else if (itemInPathList is FileInDatabase) {
                    if (lastNode != null)
                        tvDatabaseFolderTree.SelectedNode = lastNode;
                    if (found) { // folder found
                        found = false;
                        foreach (ListViewItem item in lvDatabaseItems.Items) {
                            if (item.Tag == itemInPathList) {
                                selectedItem = item;
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!found)
                MessageBox.Show(Resources.FileNotFoundInDatabase, ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            else {
                tcMain.SelectedTab = tpDatabase;
                if (selectedItem != null) { // file found
                    lvDatabaseItems.Focus();
                    lvDatabaseItems.SelectedItems.Clear();
                    selectedItem.Selected = true;
                    selectedItem.Focused = true;
                    selectedItem.EnsureVisible();
                }
                else { // folder found
                    tvDatabaseFolderTree.Focus();
                    tvDatabaseFolderTree.SelectedNode = lastNode; // set SelectedNode again, otherwise the node doesn't get focus
                    lastNode.EnsureVisible();
                }
            }
        }

        private void updateTitle() {
            Text = string.Format("{0} {1} [{2}{3}]", ProductName, ProductVersion, fileOperations.CurrentFilePath ?? "untitled", fileOperations.Modified ? " *" : string.Empty);
        }

        # region Export

        private void export() {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = Resources.ExportDatabaseTo;
            sfd.DefaultExt = DEF_EXT;
            sfd.Filter = Resources.ExportFilesFilter;
            if (sfd.ShowDialog() == DialogResult.OK) {
                saveAsCsv(sfd.FileName);
            }
        }

        private void cmDatabaseExport_Click(object sender, EventArgs e) {
            export();
        }

        #endregion

        private void cmSave_Click(object sender, EventArgs e) {
            // TODO KBR  fileOperations.Save();
            serialize(null);
            fileOperations.Modified = false; // TODO KBR hack
        }

        private void cmSaveAs_Click(object sender, EventArgs e) {
            fileOperations.SaveAs();
        }

        private void cmNew_Click(object sender, EventArgs e) {
            fileOperations.New();
        }

        private static void createNewVolumeDatabase() {
            Database = new VolumeDatabase(true);
        }

        private void cmOpen_Click(object sender, EventArgs e) {
            fileOperations.Open();
        }

        const string DEF_EXT = "bmin";

        #region Merge File

        private void mergeFile() {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = Resources.MergeWithFile;
                ofd.DefaultExt = DEF_EXT;
                ofd.Filter = Resources.IndexerFilesFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    VolumeDatabase cid = deserialize(ofd.FileName);
                    if (cid != null)
                    {
                        Database.MergeWith(cid);
                        updateTree();
                        UpdateCommands();
                        fileOperations.Modified = true;
                    }
                }
            }
        }

        private void cmMergeFile_Click(object sender, EventArgs e) {
            mergeFile();
        }

        #endregion

        private void cmExit_Click(object sender, EventArgs e) {
            Close();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = !fileOperations.SaveWithAsk();
        }

        private void showError(string message) {
            MessageBox.Show("Error: " + message, ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == QueryCancelAutoPlay) {
                m.Result = new IntPtr(1);
                return;
            }
            if ((m.Msg == Win32.WM_DEVICECHANGE) && Settings.Default.ScanNewMedia) {
                switch ((int)m.WParam) {
                    case Win32.DBT_DEVICEARRIVAL:
                        Win32.BroadcastHeader lBroadcastHeader = (Win32.BroadcastHeader)Marshal.PtrToStructure(m.LParam, typeof(Win32.BroadcastHeader));
                        if (lBroadcastHeader.Type == Win32.DeviceType.Volume) {
                            updateVolumeButtons();
                            Win32.Volume lVolume = (Win32.Volume)Marshal.PtrToStructure(m.LParam, typeof(Win32.Volume));
                            string drive = getDriveFromMask(lVolume.Mask);
                            if (drive != null) {
                                startReading(drive);
                            }
                        }
                        break;
                    case Win32.DBT_DEVICEREMOVECOMPLETE:
                        updateVolumeButtons();
                        startRefreshDiscs();
                        break;
                }

            }
            base.WndProc(ref m);
        }

        private static string getDriveFromMask(int mask) {
            try {
                int i = 0;
                for (; i < 26; ++i) {
                    if ((mask & 0x1) != 0)
                        break;
                    mask = mask >> 1;
                }
                int charCode = (i + 'A');
                return Convert.ToChar(charCode) + ":\\";
            }
            catch {
                return null;
            }
        }

        List<string> availableDrives = new List<string>();
        private void refreshDiscs() {
            refreshDiscs(false);
        }

        private void refreshDiscs(bool onDeviceArrival) {
            lock (availableDrives) {
                string addedDrive = null;
                Cursor oldCursor = Cursor;
                if (onDeviceArrival)
                    Cursor = Cursors.WaitCursor;
                try {
                    List<string> newAvailableDrives = new List<string>();
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    foreach (DriveInfo drive in drives) {
                        if (drive.IsReady) {
                            newAvailableDrives.Add(drive.Name);
                            if (onDeviceArrival && !availableDrives.Contains(drive.Name))
                                addedDrive = drive.Name;
                        }
                    }
                    availableDrives = newAvailableDrives;
                }
                finally {
                    if (onDeviceArrival)
                        Cursor = oldCursor;
                }
                if (addedDrive != null)
                    startReading(addedDrive);
            }
        }

        private void startRefreshDiscs() {
            ThreadStart scanMediaDelegate = refreshDiscs;
            Thread thread = new Thread(scanMediaDelegate);
            thread.Start();
        }

        private void cmScanNewMedia_Click(object sender, EventArgs e) {
            cmScanNewMedia.Checked = !cmScanNewMedia.Checked;
            Settings.Default.ScanNewMedia = cmScanNewMedia.Checked;
        }

        #region Background work

        public void SetToBackground(string startingBackgroundMsg) {
            Hide();
            niBackgroundProcess.BalloonTipTitle = ProductName;
            niBackgroundProcess.BalloonTipIcon = ToolTipIcon.Info;
            niBackgroundProcess.BalloonTipText = startingBackgroundMsg;
            niBackgroundProcess.Text = string.Format("{0}: {1}", ProductName, startingBackgroundMsg);
            niBackgroundProcess.Visible = true;
            niBackgroundProcess.ShowBalloonTip(10000);
        }

        private void cmRestoreWindow_Click(object sender, EventArgs e) {
            restoreWindow();
        }

        private void restoreWindow() {
            Show();
            niBackgroundProcess.Visible = false;
            if (openProgressDialog != null)
                openProgressDialog.Show();
        }

        private void FrmMain_Shown(object sender, EventArgs e) {
            niBackgroundProcess.Visible = false;
        }

        private void niBackgroundProcess_DoubleClick(object sender, EventArgs e) {
            restoreWindow();
        }

        #endregion

        #region Color scheme

        /*
        private void cmStyleBlue_Click(object sender, EventArgs e) {
            setScheme(eOffice2007ColorScheme.Blue, false);
        }

        private void cmStyleBlack_Click(object sender, EventArgs e) {
            setScheme(eOffice2007ColorScheme.Black, false);
        }

        private void cmStyleSilver_Click(object sender, EventArgs e) {
            setScheme(eOffice2007ColorScheme.Silver, false);
        }

        private void cmGlass_Click(object sender, EventArgs e) {
            setScheme(eOffice2007ColorScheme.VistaGlass, false);
        }

        private void setScheme(eOffice2007ColorScheme colorScheme, bool onLoad) {
            //ribbonControl.Office2007ColorTable = colorScheme;

            RibbonPredefinedColorSchemes.ChangeOffice2007ColorTable(colorScheme);

            baseColorScheme = colorScheme;
            cmStyleBlue.Checked = colorScheme == eOffice2007ColorScheme.Blue;
            cmStyleBlack.Checked = colorScheme == eOffice2007ColorScheme.Black;
            cmStyleSilver.Checked = colorScheme == eOffice2007ColorScheme.Silver;
            cmStyleGlass.Checked = colorScheme == eOffice2007ColorScheme.VistaGlass;
            string schemeName = cmStyleBlue.Checked ? cmStyleBlue.Text : cmStyleBlack.Checked ? cmStyleBlack.Text : cmStyleSilver.Checked ? cmStyleSilver.Text : cmStyleGlass.Text;
            pmCustomScheme.Text = string.Format("{0} (Custom Colors)", schemeName);
            if (!onLoad) {
                Properties.Settings.Default.ColorScheme = colorScheme;
                Properties.Settings.Default.UseCustomColorScheme = false;
            }
        }

        private void setCustomColorScheme(Color color, bool onLoad) {
            RibbonPredefinedColorSchemes.ChangeOffice2007ColorTable(this, baseColorScheme, color);
            if (!onLoad) {
                Properties.Settings.Default.UseCustomColorScheme = true;
                Properties.Settings.Default.CustomColorScheme = color;
            }
        }

        private eOffice2007ColorScheme baseColorScheme = eOffice2007ColorScheme.Silver;
        private void pmCustomScheme_SelectedColorChanged(object sender, EventArgs e) {
            setCustomColorScheme(pmCustomScheme.SelectedColor, false);
        } */

        #endregion

        #region Folders

        #region Logical Folder List View

        private void tvLogicalFolders_AfterSelect(object sender, TreeViewEventArgs e) {
            UpdateLogicalElements();
            UpdateCommands();
        }

        private void tvLogicalFolders_DragDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(typeof(List<ItemInDatabase>))) {
                Point targetPoint = tvLogicalFolders.PointToClient(new Point(e.X, e.Y));
                TreeNode targetNode = tvLogicalFolders.GetNodeAt(targetPoint);
                dropItems(e, targetNode);
            }
        }

        private void tvLogicalFolders_DragOver(object sender, DragEventArgs e) {
            Point targetPoint = tvLogicalFolders.PointToClient(new Point(e.X, e.Y));
            TreeNode treeNode = tvLogicalFolders.GetNodeAt(targetPoint);
            setEffect(e, treeNode);
        }

        #endregion

        private TreeNode getSelectedLogicalFolderTvItem() {
            return tvLogicalFolders.SelectedNode;
        }

        public bool duringUpdateLogicalFolders = false;
        private void updateLogicalFolders() {
            duringUpdateLogicalFolders = true;
            try {
                tvLogicalFolders.BeginUpdate();
                try {
                    tvLogicalFolders.Nodes.Clear();
                    foreach (LogicalFolder lf in Database.GetLogicalFolders()) {
                        TreeNode tn = new TreeNode();
                        lf.CopyToNode(tn);
                        tvLogicalFolders.Nodes.Add(tn);
                    }
                }
                finally {
                    tvLogicalFolders.EndUpdate();
                }
                if (tvLogicalFolders.Nodes.Count > 0)
                    tvLogicalFolders.SelectedNode = tvLogicalFolders.Nodes[0];
            }
            finally {
                duringUpdateLogicalFolders = false;
            }
            UpdateLogicalElements();
        }

        public void UpdateLogicalElements() {
            if (duringUpdateLogicalFolders)
                return;
            lvFolderElements.Items.Clear();
            if (tvLogicalFolders.SelectedNode != null) {
                lvFolderElements.Enabled = true;
                LogicalFolder lf = (LogicalFolder)tvLogicalFolders.SelectedNode.Tag;
                if (lf != null) {
                    Cursor c = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                    lvFolderElements.BeginUpdate();
                    try {
                        foreach (ItemInDatabase iid in lf.Items) {
                            ListViewItem lvi = iid.ToListViewItem();
                            lvFolderElements.Items.Add(lvi);
                        }
                        Win32.UpdateSystemImageList(lvFolderElements.SmallImageList, Win32.FileIconSize.Small, false, Resources.delete);
                    }
                    finally {
                        lvFolderElements.EndUpdate();
                        Cursor.Current = c;
                    }
                }
            }
            else
                lvFolderElements.Enabled = false;
        }

        private void cmRemoveFromFolder_Click(object sender, EventArgs e) {
            if (tvLogicalFolders.SelectedNode != null) {
                LogicalFolder lf = (LogicalFolder)tvLogicalFolders.SelectedNode.Tag;
                ListView.SelectedListViewItemCollection selectedItems = lvFolderElements.SelectedItems;
                lvFolderElements.BeginUpdate();
                try {
                    foreach (ListViewItem lvi in selectedItems) {
                        lvFolderElements.Items.Remove(lvi);
                        lf.RemoveItem(lvi.Tag as ItemInDatabase);
                        fileOperations.Modified = true;
                    }
                }
                finally {
                    lvFolderElements.EndUpdate();
                }
            }
        }

        private void lvLogicalFolderItems_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateCommands();
        }

        #endregion

        #region Drag & drop

        private Rectangle dragBoxFromMouseDown = Rectangle.Empty;
        private List<ItemInDatabase> itemsToDrag = null;

        private void lvDatabaseItems_MouseDown(object sender, MouseEventArgs e) {
            if (lvDatabaseItems.SelectedItems.Count > 0) {
                itemsToDrag = new List<ItemInDatabase>();
                foreach (ListViewItem lvi in lvDatabaseItems.SelectedItems)
                    itemsToDrag.Add(lvi.Tag as ItemInDatabase);
                Size dragSize = SystemInformation.DragSize;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void tvDatabaseFolderTree_MouseDown(object sender, MouseEventArgs e) {
            if (tvDatabaseFolderTree.SelectedNode != null) {
                itemsToDrag = new List<ItemInDatabase> {tvDatabaseFolderTree.SelectedNode.Tag as ItemInDatabase};
                Size dragSize = SystemInformation.DragSize;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void lvSearchResults_MouseDown(object sender, MouseEventArgs e) {
            if (lvSearchResults.SelectedIndices.Count > 0) {
                itemsToDrag = new List<ItemInDatabase>();
                foreach (int index in lvSearchResults.SelectedIndices)
                    itemsToDrag.Add(searchResultList[index]);
                Size dragSize = SystemInformation.DragSize;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void lvLogicalFolderItems_MouseDown(object sender, MouseEventArgs e) {
            if (lvFolderElements.SelectedItems.Count > 0) {
                itemsToDrag = new List<ItemInDatabase>();
                foreach (ListViewItem lvi in lvFolderElements.SelectedItems)
                    itemsToDrag.Add(lvi.Tag as ItemInDatabase);
                Size dragSize = SystemInformation.DragSize;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void emptyRectangle() {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void lvDatabaseItems_MouseUp(object sender, MouseEventArgs e) {
            emptyRectangle();
        }

        private void tvDatabaseFolderTree_MouseUp(object sender, MouseEventArgs e) {
            emptyRectangle();
        }

        private void lvSearchResults_MouseUp(object sender, MouseEventArgs e) {
            emptyRectangle();
        }

        private void lvLogicalFolderItems_MouseUp(object sender, MouseEventArgs e) {
            emptyRectangle();
        }

        private void startDragging(MouseEventArgs e, Control control) {
            if ((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right)) {
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y)) {
                    /* DragDropEffects dropEffect = */
                    control.DoDragDrop(itemsToDrag, DragDropEffects.All | DragDropEffects.Link);
                }
            }
        }

        private void lvDatabaseItems_MouseMove(object sender, MouseEventArgs e) {
            startDragging(e, lvDatabaseItems);
        }

        private void tvDatabaseFolderTree_MouseMove(object sender, MouseEventArgs e) {
            startDragging(e, tvDatabaseFolderTree);
        }

        private void lvSearchResults_MouseMove(object sender, MouseEventArgs e) {
            startDragging(e, lvSearchResults);
        }

        private void lvLogicalFolderItems_MouseMove(object sender, MouseEventArgs e) {
            startDragging(e, lvFolderElements);
        }

        private void lvLogicalFolderItems_DragOver(object sender, DragEventArgs e) {
            TreeNode treeNode = getSelectedLogicalFolderTvItem();
            setEffect(e, treeNode);
        }

        private void setEffect(DragEventArgs e, TreeNode treeNode) {
            if (!e.Data.GetDataPresent(typeof(List<ItemInDatabase>)) || (treeNode == null))
                e.Effect = DragDropEffects.None;
            else {
                if (((LogicalFolder)treeNode.Tag).FolderType == LogicalFolderType.DiscCatalog && onlyFiles((List<ItemInDatabase>)e.Data.GetData(typeof(List<ItemInDatabase>))))
                    e.Effect = DragDropEffects.None;
                else
                    if (MouseButtons == MouseButtons.Right)
                        e.Effect = DragDropEffects.All;
                    else
                        if ((e.KeyState & 8) == 8)
                            e.Effect = DragDropEffects.Copy;
                        else
                            e.Effect = DragDropEffects.Link;
            }
        }

        private static bool onlyFiles(List<ItemInDatabase> list) {
            foreach (ItemInDatabase item in list)
                if (item is IFolder)
                    return false;
            return true;
        }

        private void dropItems(DragEventArgs e, TreeNode targetNode) {
            try {
                DragDropEffects dragDropEffects = e.Effect;
                if ((targetNode != null) && ((dragDropEffects == DragDropEffects.Copy) || (dragDropEffects == DragDropEffects.Link) || (dragDropEffects == DragDropEffects.All))) {
                    List<ItemInDatabase> items = (List<ItemInDatabase>)e.Data.GetData(typeof(List<ItemInDatabase>));
                    LogicalFolder logicalFolder = (LogicalFolder)targetNode.Tag;

                    if (dragDropEffects == DragDropEffects.All) {
                        cmDropFolderAsItems.Enabled = !logicalFolder.IsPartOfDvd();
                        pmDrop.Tag = new DropInfo(targetNode, items, logicalFolder);
                        pmDrop.Show(MousePosition);
                        return;
                    }

                    startDropping(targetNode, dragDropEffects, items, logicalFolder);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void startDropping(TreeNode targetNode, DragDropEffects dragDropEffects, List<ItemInDatabase> items, LogicalFolder logicalFolder) {
            bool rereadFolders = false;
            bool partOfDvd = logicalFolder.IsPartOfDvd();
            bool intoDvdCatalog = logicalFolder.FolderType == LogicalFolderType.DiscCatalog;
            foreach (ItemInDatabase item in items) {
                if ((item is FolderInDatabase) && (partOfDvd || (dragDropEffects == DragDropEffects.Copy))) {
                    string asName = null;
                    if (partOfDvd)
                        asName = logicalFolder.GetValidSubFolderName(item.Name);
                    logicalFolder.AddItemAsFolder(item as FolderInDatabase, asName);

                    rereadFolders = true;
                    fileOperations.Modified = true;
                }
                else
                    if (intoDvdCatalog) {
                        if (item is FolderInDatabase) {
                            logicalFolder.AddItemAsDvd(item as FolderInDatabase);
                            rereadFolders = true;
                            fileOperations.Modified = true;
                        }
                        // else -> nic nie dodawaj
                    }
                    else
                        if (!logicalFolder.ContainsItem(item)) {
                            logicalFolder.AddItem(item);
                            fileOperations.Modified = true;
                        }
            }
            if (rereadFolders) {
                targetNode.Nodes.Clear();
                logicalFolder.CopyToNode(targetNode);
                targetNode.Expand();
            }
            UpdateLogicalElements();
        }

        private void startDroppingFromMenu(DragDropEffects effects) {
            DropInfo di = pmDrop.Tag as DropInfo;
            if (di != null) {
                pmDrop.Tag = null;
                startDropping(di.TargetNode, effects, di.Items, di.LogicalFolder);
            }
        }

        private void cmDropFolderAsItems_Click(object sender, EventArgs e) {
            startDroppingFromMenu(DragDropEffects.Link);
        }

        private void cmDropFoldersAsLogicalFolders_Click(object sender, EventArgs e) {
            startDroppingFromMenu(DragDropEffects.Copy);
        }

        private void lvLogicalFolderItems_DragDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(typeof(List<ItemInDatabase>))) {
                TreeNode targetNode = getSelectedLogicalFolderTvItem();
                dropItems(e, targetNode);
            }
        }

        #endregion


        private void cmItemPropertiesFromFolders_Click(object sender, EventArgs e) {
            ItemInDatabase item = getItemFromFolderElements();
            if (item != null)
                showItemProperties(item);
        }

        private ItemInDatabase getItemFromFolderElements()
        {
            if (lvFolderElements.SelectedItems.Count == 1)
                return (ItemInDatabase)lvFolderElements.SelectedItems[0].Tag;
            return null;
        }

        private void lvLogicalFolderItems_DoubleClick(object sender, EventArgs e) {
            cmItemPropertiesFromFolders_Click(sender, e);
        }

        private void cmFindInDatabaseFromFolders_Click(object sender, EventArgs e) {
            if (lvFolderElements.SelectedIndices.Count == 1) {
                ItemInDatabase itemInDatabase = getItemFromFolderElements();
                if (itemInDatabase != null)
                    findInTree(itemInDatabase);
            }
        }

        private void lvFolderElements_Enter(object sender, EventArgs e) {
            UpdateCommands();
        }

        private void cmFindInDatabaseFrm_Click(object sender, EventArgs e) {
            if (lvSearchResults.Focused)
                cmFindInDatabase_Click(sender, e);
            else
                if (lvFolderElements.Focused)
                    cmFindInDatabaseFromFolders_Click(sender, e);
        }

        private void lvDatabaseItems_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete)
                cmDeleteFileInfoPopup_Click(sender, EventArgs.Empty);
        }

        private void lvFolderElements_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete)
                cmRemoveFromFolder_Click(sender, EventArgs.Empty);
        }

        public void LogicalFoldersChanged() {
            fileOperations.Modified = true;
            updateLogicalFolders();
        }

        private void tvLogicalFolders_NewLogicalFolderAdded(object sender, EventArgs e) {
            fileOperations.Modified = true;
        }

        private void tvLogicalFolders_LogicalFolderUpdated(object sender, EventArgs e) {
            fileOperations.Modified = true;
        }

        private void tvLogicalFolders_LogicalFolderDeleted(object sender, EventArgs e) {
            UpdateLogicalElements();
            UpdateCommands();
        }

        private void btnNewFolder_Click(object sender, EventArgs e) {
            tvLogicalFolders.NewFolder();
        }

        private void btnEditFolder_Click(object sender, EventArgs e) {
            tvLogicalFolders.EditFolder();
        }

        private void btnDeleteFolder_Click(object sender, EventArgs e) {
            tvLogicalFolders.DeleteFolder();
        }

        #region File operations

        private void fileOperations_SaveToFile(object sender, SaveToFileEventArgs e) {
            serialize(e.FilePath);
        }

        private void serialize(string filePath) {
            Cursor oldCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            try {
                // TODO KBR write to SQLite
#if SQLITE
                SQLite.WriteToDb(Database);
#elif LITEDB
                LiteDB.WriteToDb(Database);
#endif
            }
            finally {
                Cursor = oldCursor;
            }
        }

        private void fileOperations_ModifiedChanged(object sender, EventArgs e) {
            btnSave.Enabled = cmSave.Enabled = fileOperations.Modified;
            updateTitle();
        }

        private void fileOperations_NewFile(object sender, EventArgs e) {
            createNewVolumeDatabase();
        }

        private void fileOperations_OpenFromFile(object sender, OpenFromFileEventArgs e) {
            Database = deserialize(e.FilePath);
            e.FileValid = Database != null;
            if (Database != null)
            {
                Database.UpdateStats(); // TODO switch to subscriber model
                charting.Database = Database;
            }
        }

        private DlgProgress openProgressDialog = null;

        private VolumeDatabase deserialize(string filePath)
        {
            Cursor oldCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            try
            {
                VolumeDatabase cid = SQLite.ReadFromDb(filePath);
                return cid;
            }
            finally
            {
                Cursor = oldCursor;
            }
        }

        private void fileOperations_CurrentFilePathChanged(object sender, EventArgs e) {
            updateTitle();
        }

        private void fileOperations_FileChanged(object sender, EventArgs e) {
            updateControls();
        }

        #endregion

        internal bool IsEmptyDatabase() {
            return Database.IsEmpty();
        }

        // TODO KBR getSelectedFile could hide search/db logic

        // TODO KBR disable menu when more than one item selected
        // TODO KBR disable menu when item selected is on a dismounted drive
        private void cmExplorer_Click(object sender, EventArgs e)
        {
            // Invoke windows explorer on the item.
            // N.B. assumes menu is disabled when more than one item selected
            ItemInDatabase fid;
            if (tcMain.SelectedTab == tpSearch)
                fid = getSearchSelectedItem();
            else
                fid = getSelectedFile();

            if (fid == null)
            {
                showInWindowsExplorerToolStripMenuItem1_Click(null, null);
                return;
            }

            // TODO KBR if item is a disc, fullname is empty. it should be set to name.
            var p = string.IsNullOrEmpty(fid.FullName) ? fid.Name : fid.FullName;
            InvokeExplorer(p);
        }

        private bool DriveMounted(string which)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (var driveInfo in allDrives)
            {
                if (driveInfo.Name == which)
                    return driveInfo.IsReady;
            }
            return false;
        }

        private void InvokeExplorer(string path)
        {
            string drive = Path.GetPathRoot(path).ToUpper();
            if (DriveMounted(drive))
                Process.Start("explorer.exe", string.Format("/select,\"{0}\"", path));
        }

        private void showInWindowsExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmExplorer_Click(sender, e);
        }

        private void showInWindowsExplorerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // tree volume/folder
            ItemInDatabase iid = getSelectedTreeItem();
            if (iid != null)
                InvokeExplorer(string.IsNullOrEmpty(iid.FullName) ? iid.Name : iid.FullName);
        }

        private void showInWindowsExplorerToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // search menu
            cmExplorer_Click(sender, e);
        }
    }

    class AbortException : Exception
    {
    }
}