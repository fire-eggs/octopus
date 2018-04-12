namespace BlueMirrorIndexer.Components
{
    partial class Charter
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRoot = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.chkFlatten = new System.Windows.Forms.CheckBox();
            this.cmbVolume = new System.Windows.Forms.ComboBox();
            this.chkClusters = new System.Windows.Forms.CheckBox();
            this.cmbChart = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.findInDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(766, 335);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRoot);
            this.panel1.Controls.Add(this.btnUp);
            this.panel1.Controls.Add(this.chkFlatten);
            this.panel1.Controls.Add(this.cmbVolume);
            this.panel1.Controls.Add(this.chkClusters);
            this.panel1.Controls.Add(this.cmbChart);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(185, 329);
            this.panel1.TabIndex = 0;
            // 
            // btnRoot
            // 
            this.btnRoot.Location = new System.Drawing.Point(133, 237);
            this.btnRoot.Name = "btnRoot";
            this.btnRoot.Size = new System.Drawing.Size(49, 23);
            this.btnRoot.TabIndex = 5;
            this.btnRoot.Text = "Root";
            this.btnRoot.UseVisualStyleBackColor = true;
            this.btnRoot.Click += new System.EventHandler(this.btnRoot_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(142, 207);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(40, 23);
            this.btnUp.TabIndex = 4;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // chkFlatten
            // 
            this.chkFlatten.AutoSize = true;
            this.chkFlatten.Location = new System.Drawing.Point(4, 231);
            this.chkFlatten.Name = "chkFlatten";
            this.chkFlatten.Size = new System.Drawing.Size(92, 17);
            this.chkFlatten.TabIndex = 3;
            this.chkFlatten.Text = "Flatten folders";
            this.chkFlatten.UseVisualStyleBackColor = true;
            this.chkFlatten.CheckedChanged += new System.EventHandler(this.chkFlatten_CheckedChanged);
            // 
            // cmbVolume
            // 
            this.cmbVolume.FormattingEnabled = true;
            this.cmbVolume.Location = new System.Drawing.Point(4, 45);
            this.cmbVolume.Name = "cmbVolume";
            this.cmbVolume.Size = new System.Drawing.Size(160, 21);
            this.cmbVolume.TabIndex = 2;
            this.cmbVolume.SelectedIndexChanged += new System.EventHandler(this.cmbVolume_SelectedIndexChanged);
            // 
            // chkClusters
            // 
            this.chkClusters.AutoSize = true;
            this.chkClusters.Location = new System.Drawing.Point(4, 207);
            this.chkClusters.Name = "chkClusters";
            this.chkClusters.Size = new System.Drawing.Size(79, 17);
            this.chkClusters.TabIndex = 1;
            this.chkClusters.Text = "Cluster size";
            this.chkClusters.UseVisualStyleBackColor = true;
            this.chkClusters.CheckedChanged += new System.EventHandler(this.chkClusters_CheckedChanged);
            // 
            // cmbChart
            // 
            this.cmbChart.FormattingEnabled = true;
            this.cmbChart.Location = new System.Drawing.Point(4, 4);
            this.cmbChart.Name = "cmbChart";
            this.cmbChart.Size = new System.Drawing.Size(160, 21);
            this.cmbChart.TabIndex = 0;
            this.cmbChart.SelectedIndexChanged += new System.EventHandler(this.cmbChart_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(194, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(569, 329);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            this.panel2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findInDatabaseToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(162, 26);
            // 
            // findInDatabaseToolStripMenuItem
            // 
            this.findInDatabaseToolStripMenuItem.Name = "findInDatabaseToolStripMenuItem";
            this.findInDatabaseToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.findInDatabaseToolStripMenuItem.Text = "Find In Database";
            // 
            // Charter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Charter";
            this.Size = new System.Drawing.Size(766, 335);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnRoot;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.CheckBox chkFlatten;
        private System.Windows.Forms.ComboBox cmbVolume;
        private System.Windows.Forms.CheckBox chkClusters;
        private System.Windows.Forms.ComboBox cmbChart;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem findInDatabaseToolStripMenuItem;
    }
}
