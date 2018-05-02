namespace BlueMirrorIndexer.Components
{
    partial class Filters
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Volume A"}, 0, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F));
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Volume B"}, 0, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnSearch = new System.Windows.Forms.Button();
            this.fileNamePanel = new System.Windows.Forms.Panel();
            this.cbTreatFileMaskAsWildcard = new System.Windows.Forms.CheckBox();
            this.tbFileMask = new System.Windows.Forms.TextBox();
            this.llFileMask = new System.Windows.Forms.Label();
            this.datePanel = new System.Windows.Forms.Panel();
            this.cbDate = new System.Windows.Forms.CheckBox();
            this.dtpDateFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpDateTo = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.keywordPanel = new System.Windows.Forms.Panel();
            this.cbKeywords = new System.Windows.Forms.CheckBox();
            this.cbTreatKeywordAsWildcard = new System.Windows.Forms.CheckBox();
            this.cbCaseSensitiveKeywords = new System.Windows.Forms.CheckBox();
            this.tbKeywords = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbOneAll = new System.Windows.Forms.ComboBox();
            this.sizePanel = new System.Windows.Forms.Panel();
            this.tbSizeTo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSizeFrom = new System.Windows.Forms.TextBox();
            this.cbSize = new System.Windows.Forms.CheckBox();
            this.fileExpander = new BlueMirrorIndexer.Components.ExpanderLabel();
            this.dateExpander = new BlueMirrorIndexer.Components.ExpanderLabel();
            this.keywordExpander = new BlueMirrorIndexer.Components.ExpanderLabel();
            this.sizeExpander = new BlueMirrorIndexer.Components.ExpanderLabel();
            this.volExpander = new BlueMirrorIndexer.Components.ExpanderLabel();
            this.volumePanel = new System.Windows.Forms.Panel();
            this.lvVolumes = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1.SuspendLayout();
            this.fileNamePanel.SuspendLayout();
            this.datePanel.SuspendLayout();
            this.keywordPanel.SuspendLayout();
            this.sizePanel.SuspendLayout();
            this.volumePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.fileExpander, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BtnSearch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dateExpander, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.keywordExpander, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.fileNamePanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.datePanel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.keywordPanel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.sizeExpander, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.sizePanel, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.volExpander, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.volumePanel, 0, 10);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 11;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(279, 523);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // BtnSearch
            // 
            this.BtnSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSearch.Image = global::BlueMirrorIndexer.Properties.Resources.FindHS;
            this.BtnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnSearch.Location = new System.Drawing.Point(167, 3);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(109, 32);
            this.BtnSearch.TabIndex = 3;
            this.BtnSearch.Text = "&Search";
            this.BtnSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnSearch.UseVisualStyleBackColor = true;
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // fileNamePanel
            // 
            this.fileNamePanel.Controls.Add(this.cbTreatFileMaskAsWildcard);
            this.fileNamePanel.Controls.Add(this.tbFileMask);
            this.fileNamePanel.Controls.Add(this.llFileMask);
            this.fileNamePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileNamePanel.Location = new System.Drawing.Point(3, 73);
            this.fileNamePanel.Name = "fileNamePanel";
            this.fileNamePanel.Size = new System.Drawing.Size(273, 67);
            this.fileNamePanel.TabIndex = 3;
            // 
            // cbTreatFileMaskAsWildcard
            // 
            this.cbTreatFileMaskAsWildcard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTreatFileMaskAsWildcard.AutoSize = true;
            this.cbTreatFileMaskAsWildcard.Checked = true;
            this.cbTreatFileMaskAsWildcard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTreatFileMaskAsWildcard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbTreatFileMaskAsWildcard.Location = new System.Drawing.Point(36, 42);
            this.cbTreatFileMaskAsWildcard.Name = "cbTreatFileMaskAsWildcard";
            this.cbTreatFileMaskAsWildcard.Size = new System.Drawing.Size(107, 17);
            this.cbTreatFileMaskAsWildcard.TabIndex = 5;
            this.cbTreatFileMaskAsWildcard.Text = "Treat as wildcard";
            this.cbTreatFileMaskAsWildcard.UseVisualStyleBackColor = true;
            // 
            // tbFileMask
            // 
            this.tbFileMask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFileMask.Location = new System.Drawing.Point(12, 16);
            this.tbFileMask.Name = "tbFileMask";
            this.tbFileMask.Size = new System.Drawing.Size(251, 20);
            this.tbFileMask.TabIndex = 4;
            this.tbFileMask.Text = "*.*";
            this.tbFileMask.TextChanged += new System.EventHandler(this.tbFileMask_TextChanged);
            // 
            // llFileMask
            // 
            this.llFileMask.AutoSize = true;
            this.llFileMask.BackColor = System.Drawing.Color.Transparent;
            this.llFileMask.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.llFileMask.Location = new System.Drawing.Point(9, 0);
            this.llFileMask.Name = "llFileMask";
            this.llFileMask.Size = new System.Drawing.Size(54, 13);
            this.llFileMask.TabIndex = 3;
            this.llFileMask.Text = "&File mask:";
            // 
            // datePanel
            // 
            this.datePanel.Controls.Add(this.cbDate);
            this.datePanel.Controls.Add(this.dtpDateFrom);
            this.datePanel.Controls.Add(this.dtpDateTo);
            this.datePanel.Controls.Add(this.label1);
            this.datePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.datePanel.Location = new System.Drawing.Point(3, 178);
            this.datePanel.Name = "datePanel";
            this.datePanel.Size = new System.Drawing.Size(273, 80);
            this.datePanel.TabIndex = 4;
            // 
            // cbDate
            // 
            this.cbDate.AutoSize = true;
            this.cbDate.BackColor = System.Drawing.Color.Transparent;
            this.cbDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbDate.Location = new System.Drawing.Point(3, 12);
            this.cbDate.Name = "cbDate";
            this.cbDate.Size = new System.Drawing.Size(136, 17);
            this.cbDate.TabIndex = 6;
            this.cbDate.Text = "&Creation date between:";
            this.cbDate.UseVisualStyleBackColor = false;
            this.cbDate.CheckedChanged += new System.EventHandler(this.cbDate_CheckedChanged);
            // 
            // dtpDateFrom
            // 
            this.dtpDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateFrom.Location = new System.Drawing.Point(145, 12);
            this.dtpDateFrom.Name = "dtpDateFrom";
            this.dtpDateFrom.Size = new System.Drawing.Size(94, 20);
            this.dtpDateFrom.TabIndex = 7;
            this.dtpDateFrom.ValueChanged += new System.EventHandler(this.dtpDateFrom_ValueChanged);
            // 
            // dtpDateTo
            // 
            this.dtpDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateTo.Location = new System.Drawing.Point(145, 51);
            this.dtpDateTo.Name = "dtpDateTo";
            this.dtpDateTo.Size = new System.Drawing.Size(94, 20);
            this.dtpDateTo.TabIndex = 9;
            this.dtpDateTo.ValueChanged += new System.EventHandler(this.dtpDateTo_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(147, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "and";
            // 
            // keywordPanel
            // 
            this.keywordPanel.Controls.Add(this.cbKeywords);
            this.keywordPanel.Controls.Add(this.cbTreatKeywordAsWildcard);
            this.keywordPanel.Controls.Add(this.cbCaseSensitiveKeywords);
            this.keywordPanel.Controls.Add(this.tbKeywords);
            this.keywordPanel.Controls.Add(this.label4);
            this.keywordPanel.Controls.Add(this.cmbOneAll);
            this.keywordPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keywordPanel.Location = new System.Drawing.Point(3, 296);
            this.keywordPanel.Name = "keywordPanel";
            this.keywordPanel.Size = new System.Drawing.Size(273, 118);
            this.keywordPanel.TabIndex = 5;
            // 
            // cbKeywords
            // 
            this.cbKeywords.AutoSize = true;
            this.cbKeywords.Location = new System.Drawing.Point(12, 4);
            this.cbKeywords.Name = "cbKeywords";
            this.cbKeywords.Size = new System.Drawing.Size(99, 17);
            this.cbKeywords.TabIndex = 22;
            this.cbKeywords.Text = "Files containing";
            this.cbKeywords.UseVisualStyleBackColor = true;
            this.cbKeywords.CheckedChanged += new System.EventHandler(this.cbKeywords_CheckedChanged);
            // 
            // cbTreatKeywordAsWildcard
            // 
            this.cbTreatKeywordAsWildcard.AutoSize = true;
            this.cbTreatKeywordAsWildcard.Checked = true;
            this.cbTreatKeywordAsWildcard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTreatKeywordAsWildcard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbTreatKeywordAsWildcard.Location = new System.Drawing.Point(11, 97);
            this.cbTreatKeywordAsWildcard.Name = "cbTreatKeywordAsWildcard";
            this.cbTreatKeywordAsWildcard.Size = new System.Drawing.Size(107, 17);
            this.cbTreatKeywordAsWildcard.TabIndex = 21;
            this.cbTreatKeywordAsWildcard.Text = "Treat as wildcard";
            this.cbTreatKeywordAsWildcard.UseVisualStyleBackColor = true;
            // 
            // cbCaseSensitiveKeywords
            // 
            this.cbCaseSensitiveKeywords.AutoSize = true;
            this.cbCaseSensitiveKeywords.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbCaseSensitiveKeywords.Location = new System.Drawing.Point(11, 74);
            this.cbCaseSensitiveKeywords.Name = "cbCaseSensitiveKeywords";
            this.cbCaseSensitiveKeywords.Size = new System.Drawing.Size(160, 17);
            this.cbCaseSensitiveKeywords.TabIndex = 20;
            this.cbCaseSensitiveKeywords.Text = "Keywords are case sensitive";
            this.cbCaseSensitiveKeywords.UseVisualStyleBackColor = true;
            // 
            // tbKeywords
            // 
            this.tbKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbKeywords.Location = new System.Drawing.Point(11, 48);
            this.tbKeywords.Name = "tbKeywords";
            this.tbKeywords.Size = new System.Drawing.Size(238, 20);
            this.tbKeywords.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(9, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "of the following &keywords:";
            // 
            // cmbOneAll
            // 
            this.cmbOneAll.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOneAll.FormattingEnabled = true;
            this.cmbOneAll.Items.AddRange(new object[] {
            "one",
            "all"});
            this.cmbOneAll.Location = new System.Drawing.Point(141, 1);
            this.cmbOneAll.Name = "cmbOneAll";
            this.cmbOneAll.Size = new System.Drawing.Size(66, 21);
            this.cmbOneAll.TabIndex = 17;
            // 
            // sizePanel
            // 
            this.sizePanel.Controls.Add(this.tbSizeTo);
            this.sizePanel.Controls.Add(this.label2);
            this.sizePanel.Controls.Add(this.tbSizeFrom);
            this.sizePanel.Controls.Add(this.cbSize);
            this.sizePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sizePanel.Location = new System.Drawing.Point(3, 452);
            this.sizePanel.Name = "sizePanel";
            this.sizePanel.Size = new System.Drawing.Size(273, 103);
            this.sizePanel.TabIndex = 7;
            // 
            // tbSizeTo
            // 
            this.tbSizeTo.Location = new System.Drawing.Point(141, 42);
            this.tbSizeTo.Name = "tbSizeTo";
            this.tbSizeTo.Size = new System.Drawing.Size(94, 20);
            this.tbSizeTo.TabIndex = 10;
            this.tbSizeTo.Text = "0";
            this.tbSizeTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(142, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "and";
            // 
            // tbSizeFrom
            // 
            this.tbSizeFrom.Location = new System.Drawing.Point(141, 3);
            this.tbSizeFrom.MaxLength = 15;
            this.tbSizeFrom.Name = "tbSizeFrom";
            this.tbSizeFrom.Size = new System.Drawing.Size(94, 20);
            this.tbSizeFrom.TabIndex = 8;
            this.tbSizeFrom.Text = "0";
            this.tbSizeFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cbSize
            // 
            this.cbSize.AutoSize = true;
            this.cbSize.BackColor = System.Drawing.Color.Transparent;
            this.cbSize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbSize.Location = new System.Drawing.Point(3, 3);
            this.cbSize.Name = "cbSize";
            this.cbSize.Size = new System.Drawing.Size(132, 17);
            this.cbSize.TabIndex = 7;
            this.cbSize.Text = "File &size between [kB]:";
            this.cbSize.UseVisualStyleBackColor = false;
            this.cbSize.CheckedChanged += new System.EventHandler(this.cbSize_CheckedChanged);
            // 
            // fileExpander
            // 
            this.fileExpander.AutoSize = true;
            this.fileExpander.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fileExpander.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileExpander.Location = new System.Drawing.Point(3, 41);
            this.fileExpander.Name = "fileExpander";
            this.fileExpander.Size = new System.Drawing.Size(273, 26);
            this.fileExpander.TabIndex = 0;
            this.fileExpander.ExpandBtnClicked += new BlueMirrorIndexer.Components.ExpanderLabel.ExpandEventHandler(this.fileExpander_ExpandBtnClicked);
            // 
            // dateExpander
            // 
            this.dateExpander.AutoSize = true;
            this.dateExpander.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dateExpander.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateExpander.Location = new System.Drawing.Point(3, 146);
            this.dateExpander.Name = "dateExpander";
            this.dateExpander.Size = new System.Drawing.Size(273, 26);
            this.dateExpander.TabIndex = 1;
            this.dateExpander.ExpandBtnClicked += new BlueMirrorIndexer.Components.ExpanderLabel.ExpandEventHandler(this.dateExpander_ExpandBtnClicked);
            // 
            // keywordExpander
            // 
            this.keywordExpander.AutoSize = true;
            this.keywordExpander.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.keywordExpander.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keywordExpander.Location = new System.Drawing.Point(3, 264);
            this.keywordExpander.Name = "keywordExpander";
            this.keywordExpander.Size = new System.Drawing.Size(273, 26);
            this.keywordExpander.TabIndex = 2;
            this.keywordExpander.ExpandBtnClicked += new BlueMirrorIndexer.Components.ExpanderLabel.ExpandEventHandler(this.keywordExpander_ExpandBtnClicked);
            // 
            // sizeExpander
            // 
            this.sizeExpander.AutoSize = true;
            this.sizeExpander.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sizeExpander.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sizeExpander.Location = new System.Drawing.Point(3, 420);
            this.sizeExpander.Name = "sizeExpander";
            this.sizeExpander.Size = new System.Drawing.Size(273, 26);
            this.sizeExpander.TabIndex = 6;
            this.sizeExpander.ExpandBtnClicked += new BlueMirrorIndexer.Components.ExpanderLabel.ExpandEventHandler(this.sizeExpander_ExpandBtnClicked);
            // 
            // volExpander
            // 
            this.volExpander.AutoSize = true;
            this.volExpander.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.volExpander.Dock = System.Windows.Forms.DockStyle.Fill;
            this.volExpander.Location = new System.Drawing.Point(3, 561);
            this.volExpander.Name = "volExpander";
            this.volExpander.Size = new System.Drawing.Size(273, 26);
            this.volExpander.TabIndex = 8;
            this.volExpander.ExpandBtnClicked += new BlueMirrorIndexer.Components.ExpanderLabel.ExpandEventHandler(this.volExpander_ExpandBtnClicked);
            // 
            // volumePanel
            // 
            this.volumePanel.Controls.Add(this.lvVolumes);
            this.volumePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.volumePanel.Location = new System.Drawing.Point(3, 593);
            this.volumePanel.Name = "volumePanel";
            this.volumePanel.Size = new System.Drawing.Size(273, 14);
            this.volumePanel.TabIndex = 9;
            // 
            // lvVolumes
            // 
            this.lvVolumes.CheckBoxes = true;
            this.lvVolumes.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.StateImageIndex = 0;
            this.lvVolumes.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.lvVolumes.Location = new System.Drawing.Point(0, 0);
            this.lvVolumes.MultiSelect = false;
            this.lvVolumes.Name = "lvVolumes";
            this.lvVolumes.Size = new System.Drawing.Size(273, 14);
            this.lvVolumes.TabIndex = 1;
            this.lvVolumes.UseCompatibleStateImageBehavior = false;
            this.lvVolumes.View = System.Windows.Forms.View.List;
            // 
            // Filters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Filters";
            this.Size = new System.Drawing.Size(279, 553);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.fileNamePanel.ResumeLayout(false);
            this.fileNamePanel.PerformLayout();
            this.datePanel.ResumeLayout(false);
            this.datePanel.PerformLayout();
            this.keywordPanel.ResumeLayout(false);
            this.keywordPanel.PerformLayout();
            this.sizePanel.ResumeLayout(false);
            this.sizePanel.PerformLayout();
            this.volumePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ExpanderLabel fileExpander;
        private ExpanderLabel dateExpander;
        private ExpanderLabel keywordExpander;
        private System.Windows.Forms.Panel fileNamePanel;
        private System.Windows.Forms.CheckBox cbTreatFileMaskAsWildcard;
        private System.Windows.Forms.TextBox tbFileMask;
        private System.Windows.Forms.Label llFileMask;
        private System.Windows.Forms.Panel datePanel;
        private System.Windows.Forms.CheckBox cbDate;
        private System.Windows.Forms.DateTimePicker dtpDateFrom;
        private System.Windows.Forms.DateTimePicker dtpDateTo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel keywordPanel;
        private System.Windows.Forms.CheckBox cbTreatKeywordAsWildcard;
        private System.Windows.Forms.CheckBox cbCaseSensitiveKeywords;
        private System.Windows.Forms.TextBox tbKeywords;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbOneAll;
        private System.Windows.Forms.CheckBox cbKeywords;
        private ExpanderLabel sizeExpander;
        private System.Windows.Forms.Panel sizePanel;
        private System.Windows.Forms.CheckBox cbSize;
        private System.Windows.Forms.TextBox tbSizeFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSizeTo;
        internal System.Windows.Forms.Button BtnSearch;
        private ExpanderLabel volExpander;
        private System.Windows.Forms.Panel volumePanel;
        private System.Windows.Forms.ListView lvVolumes;
    }
}
