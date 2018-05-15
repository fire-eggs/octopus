using BlueMirrorIndexer.SearchFilters;

namespace BlueMirrorIndexer.SearchPanel
{
    partial class SearchPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchPanel));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Volume", System.Windows.Forms.HorizontalAlignment.Left);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnSearch = new System.Windows.Forms.Button();
            this.accordion1 = new BlueMirrorIndexer.SearchFilters.Accordion();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lvSearchResults = new BlueMirror.Commons.Controls.ListViewVista();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSrAttributes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSrKeywords = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSrFileExtension = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSrVolume = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSrPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSrCrc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pmSearchList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmItemPropertiesFromSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.cmFindInDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.showInWindowsExplorerToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ilSystem = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pmSearchList.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.splitContainer1.Size = new System.Drawing.Size(769, 448);
            this.splitContainer1.SplitterDistance = 256;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.BtnSearch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.accordion1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(256, 448);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // BtnSearch
            // 
            this.BtnSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSearch.Image = global::BlueMirrorIndexer.Properties.Resources.FindHS;
            this.BtnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnSearch.Location = new System.Drawing.Point(144, 3);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(109, 32);
            this.BtnSearch.TabIndex = 4;
            this.BtnSearch.Text = "&Search";
            this.BtnSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnSearch.UseVisualStyleBackColor = true;
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // accordion1
            // 
            this.accordion1.AddResizeBars = true;
            this.accordion1.AllowMouseResize = true;
            this.accordion1.AnimateCloseEffect = ((BlueMirrorIndexer.SearchFilters.AnimateWindowFlags)(((BlueMirrorIndexer.SearchFilters.AnimateWindowFlags.VerticalNegative | BlueMirrorIndexer.SearchFilters.AnimateWindowFlags.Hide) 
            | BlueMirrorIndexer.SearchFilters.AnimateWindowFlags.Slide)));
            this.accordion1.AnimateCloseMillis = 300;
            this.accordion1.AnimateOpenEffect = ((BlueMirrorIndexer.SearchFilters.AnimateWindowFlags)(((BlueMirrorIndexer.SearchFilters.AnimateWindowFlags.VerticalPositive | BlueMirrorIndexer.SearchFilters.AnimateWindowFlags.Show) 
            | BlueMirrorIndexer.SearchFilters.AnimateWindowFlags.Slide)));
            this.accordion1.AnimateOpenMillis = 300;
            this.accordion1.AutoFixDockStyle = true;
            this.accordion1.AutoScroll = true;
            this.accordion1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.accordion1.CheckBoxFactory = null;
            this.accordion1.CheckBoxMargin = new System.Windows.Forms.Padding(0);
            this.accordion1.ContentBackColor = null;
            this.accordion1.ContentMargin = null;
            this.accordion1.ContentPadding = new System.Windows.Forms.Padding(5);
            this.accordion1.ControlBackColor = null;
            this.accordion1.ControlMinimumHeightIsItsPreferredHeight = true;
            this.accordion1.ControlMinimumWidthIsItsPreferredWidth = true;
            this.accordion1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accordion1.DownArrow = null;
            this.accordion1.FillHeight = true;
            this.accordion1.FillLastOpened = false;
            this.accordion1.FillModeGrowOnly = false;
            this.accordion1.FillResetOnCollapse = false;
            this.accordion1.FillWidth = true;
            this.accordion1.GrabCursor = System.Windows.Forms.Cursors.SizeNS;
            this.accordion1.GrabRequiresPositiveFillWeight = true;
            this.accordion1.GrabWidth = 6;
            this.accordion1.GrowAndShrink = true;
            this.accordion1.Insets = new System.Windows.Forms.Padding(0);
            this.accordion1.Location = new System.Drawing.Point(3, 41);
            this.accordion1.Name = "accordion1";
            this.accordion1.OpenOnAdd = false;
            this.accordion1.OpenOneOnly = false;
            this.accordion1.ResizeBarFactory = null;
            this.accordion1.ResizeBarsAlign = 0.5D;
            this.accordion1.ResizeBarsArrowKeyDelta = 10;
            this.accordion1.ResizeBarsFadeInMillis = 800;
            this.accordion1.ResizeBarsFadeOutMillis = 800;
            this.accordion1.ResizeBarsFadeProximity = 24;
            this.accordion1.ResizeBarsFill = 1D;
            this.accordion1.ResizeBarsKeepFocusAfterMouseDrag = false;
            this.accordion1.ResizeBarsKeepFocusIfControlOutOfView = true;
            this.accordion1.ResizeBarsKeepFocusOnClick = true;
            this.accordion1.ResizeBarsMargin = null;
            this.accordion1.ResizeBarsMinimumLength = 50;
            this.accordion1.ResizeBarsStayInViewOnArrowKey = true;
            this.accordion1.ResizeBarsStayInViewOnMouseDrag = true;
            this.accordion1.ResizeBarsStayVisibleIfFocused = true;
            this.accordion1.ResizeBarsTabStop = true;
            this.accordion1.ShowPartiallyVisibleResizeBars = false;
            this.accordion1.ShowToolMenu = true;
            this.accordion1.ShowToolMenuOnHoverWhenClosed = false;
            this.accordion1.ShowToolMenuOnRightClick = true;
            this.accordion1.ShowToolMenuRequiresPositiveFillWeight = false;
            this.accordion1.Size = new System.Drawing.Size(250, 404);
            this.accordion1.TabIndex = 5;
            this.accordion1.UpArrow = null;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lvSearchResults, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(509, 448);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lvSearchResults
            // 
            this.lvSearchResults.AllowColumnReorder = true;
            this.lvSearchResults.BackColor = System.Drawing.SystemColors.Window;
            this.lvSearchResults.ColumnOrderArray = ((System.Collections.ArrayList)(resources.GetObject("lvSearchResults.ColumnOrderArray")));
            this.lvSearchResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader9,
            this.chSrAttributes,
            this.chSrKeywords,
            this.chSrFileExtension,
            this.chSrVolume,
            this.chSrPath,
            this.chSrCrc});
            this.lvSearchResults.ColumnWidthArray = ((System.Collections.ArrayList)(resources.GetObject("lvSearchResults.ColumnWidthArray")));
            this.lvSearchResults.ContextMenuStrip = this.pmSearchList;
            this.lvSearchResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSearchResults.FullRowSelect = true;
            listViewGroup1.Header = "Volume";
            listViewGroup1.Name = "Volume";
            this.lvSearchResults.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1});
            this.lvSearchResults.HideSelection = false;
            this.lvSearchResults.Location = new System.Drawing.Point(3, 33);
            this.lvSearchResults.Name = "lvSearchResults";
            this.lvSearchResults.Size = new System.Drawing.Size(503, 412);
            this.lvSearchResults.SmallImageList = this.ilSystem;
            this.lvSearchResults.TabIndex = 2;
            this.lvSearchResults.UseCompatibleStateImageBehavior = false;
            this.lvSearchResults.View = System.Windows.Forms.View.Details;
            this.lvSearchResults.VirtualMode = true;
            this.lvSearchResults.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.lvSearchResults_CacheVirtualItems);
            this.lvSearchResults.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvSearchResults_ColumnClick);
            this.lvSearchResults.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvSearchResults_RetrieveVirtualItem);
            this.lvSearchResults.SelectedIndexChanged += new System.EventHandler(this.lvSearchResults_SelectedIndexChanged);
            this.lvSearchResults.DoubleClick += new System.EventHandler(this.lvSearchResults_DoubleClick);
            this.lvSearchResults.Enter += new System.EventHandler(this.lvSearchResults_Enter);
            this.lvSearchResults.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvSearchResults_KeyDown);
            this.lvSearchResults.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvSearchResults_MouseDown);
            this.lvSearchResults.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lvSearchResults_MouseMove);
            this.lvSearchResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvSearchResults_MouseUp);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "File/Folder Name";
            this.columnHeader5.Width = 182;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Size";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader6.Width = 91;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Created";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader7.Width = 125;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Modified";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader9.Width = 122;
            // 
            // chSrAttributes
            // 
            this.chSrAttributes.Text = "Attributes";
            this.chSrAttributes.Width = 70;
            // 
            // chSrKeywords
            // 
            this.chSrKeywords.Text = "Keywords";
            this.chSrKeywords.Width = 100;
            // 
            // chSrFileExtension
            // 
            this.chSrFileExtension.Text = "File Extension";
            this.chSrFileExtension.Width = 90;
            // 
            // chSrVolume
            // 
            this.chSrVolume.Text = "Volume";
            this.chSrVolume.Width = 90;
            // 
            // chSrPath
            // 
            this.chSrPath.Text = "Path";
            this.chSrPath.Width = 120;
            // 
            // chSrCrc
            // 
            this.chSrCrc.Text = "CRC";
            this.chSrCrc.Width = 80;
            // 
            // pmSearchList
            // 
            this.pmSearchList.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.pmSearchList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmItemPropertiesFromSearch,
            this.cmFindInDatabase,
            this.showInWindowsExplorerToolStripMenuItem2});
            this.pmSearchList.Name = "cmsSearchList";
            this.pmSearchList.Size = new System.Drawing.Size(218, 82);
            this.pmSearchList.Opening += new System.ComponentModel.CancelEventHandler(this.pmSearchList_Opening);
            // 
            // cmItemPropertiesFromSearch
            // 
            this.cmItemPropertiesFromSearch.Image = global::BlueMirrorIndexer.Properties.Resources.tag_blue_edit;
            this.cmItemPropertiesFromSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmItemPropertiesFromSearch.Name = "cmItemPropertiesFromSearch";
            this.cmItemPropertiesFromSearch.Size = new System.Drawing.Size(217, 26);
            this.cmItemPropertiesFromSearch.Text = "Item Properties";
            this.cmItemPropertiesFromSearch.Click += new System.EventHandler(this.cmItemPropertiesFromSearch_Click);
            // 
            // cmFindInDatabase
            // 
            this.cmFindInDatabase.Image = global::BlueMirrorIndexer.Properties.Resources.folder_find;
            this.cmFindInDatabase.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.cmFindInDatabase.Name = "cmFindInDatabase";
            this.cmFindInDatabase.Size = new System.Drawing.Size(217, 26);
            this.cmFindInDatabase.Text = "Find in Database";
            this.cmFindInDatabase.Click += new System.EventHandler(this.cmFindInDatabase_Click);
            // 
            // showInWindowsExplorerToolStripMenuItem2
            // 
            this.showInWindowsExplorerToolStripMenuItem2.Name = "showInWindowsExplorerToolStripMenuItem2";
            this.showInWindowsExplorerToolStripMenuItem2.Size = new System.Drawing.Size(217, 26);
            this.showInWindowsExplorerToolStripMenuItem2.Text = "Show In Windows E&xplorer";
            this.showInWindowsExplorerToolStripMenuItem2.Click += new System.EventHandler(this.showInWindowsExplorerToolStripMenuItem2_Click);
            // 
            // ilSystem
            // 
            this.ilSystem.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilSystem.ImageSize = new System.Drawing.Size(16, 16);
            this.ilSystem.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "current search filter display goes here";
            // 
            // SearchPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "SearchPanel";
            this.Size = new System.Drawing.Size(769, 448);
            this.Load += new System.EventHandler(this.SearchPanel_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.pmSearchList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        internal System.Windows.Forms.Button BtnSearch;
        private Accordion accordion1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private BlueMirror.Commons.Controls.ListViewVista lvSearchResults;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader chSrAttributes;
        private System.Windows.Forms.ColumnHeader chSrKeywords;
        private System.Windows.Forms.ColumnHeader chSrFileExtension;
        private System.Windows.Forms.ColumnHeader chSrVolume;
        private System.Windows.Forms.ColumnHeader chSrPath;
        private System.Windows.Forms.ColumnHeader chSrCrc;
        private System.Windows.Forms.ImageList ilSystem;
        private System.Windows.Forms.ContextMenuStrip pmSearchList;
        private System.Windows.Forms.ToolStripMenuItem cmItemPropertiesFromSearch;
        private System.Windows.Forms.ToolStripMenuItem cmFindInDatabase;
        private System.Windows.Forms.ToolStripMenuItem showInWindowsExplorerToolStripMenuItem2;
    }
}
