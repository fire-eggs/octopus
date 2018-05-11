namespace BlueMirrorIndexer.SearchFilters
{
    partial class SizePanel
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.txtSizeFrom = new System.Windows.Forms.TextBox();
            this.cmbSizeTo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSizeTo = new System.Windows.Forms.TextBox();
            this.cmbSizeFrom = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(4, 4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(63, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "File size";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Less than",
            "More than",
            "Between"});
            this.comboBox1.Location = new System.Drawing.Point(74, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(102, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // txtSizeFrom
            // 
            this.txtSizeFrom.Location = new System.Drawing.Point(17, 32);
            this.txtSizeFrom.MaxLength = 10;
            this.txtSizeFrom.Name = "txtSizeFrom";
            this.txtSizeFrom.Size = new System.Drawing.Size(100, 20);
            this.txtSizeFrom.TabIndex = 2;
            this.txtSizeFrom.Text = "0";
            this.txtSizeFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSizeFrom.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.txtSizeFrom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sizeBox_KeyPress);
            // 
            // cmbSizeTo
            // 
            this.cmbSizeTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSizeTo.FormattingEnabled = true;
            this.cmbSizeTo.Items.AddRange(new object[] {
            "KB",
            "MB",
            "GB"});
            this.cmbSizeTo.Location = new System.Drawing.Point(134, 75);
            this.cmbSizeTo.Name = "cmbSizeTo";
            this.cmbSizeTo.Size = new System.Drawing.Size(63, 21);
            this.cmbSizeTo.TabIndex = 3;
            this.cmbSizeTo.SelectedIndexChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "and";
            // 
            // txtSizeTo
            // 
            this.txtSizeTo.Location = new System.Drawing.Point(17, 75);
            this.txtSizeTo.MaxLength = 10;
            this.txtSizeTo.Name = "txtSizeTo";
            this.txtSizeTo.Size = new System.Drawing.Size(100, 20);
            this.txtSizeTo.TabIndex = 5;
            this.txtSizeTo.Text = "0";
            this.txtSizeTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSizeTo.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.txtSizeTo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sizeBox_KeyPress);
            // 
            // cmbSizeFrom
            // 
            this.cmbSizeFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSizeFrom.FormattingEnabled = true;
            this.cmbSizeFrom.Items.AddRange(new object[] {
            "KB",
            "MB",
            "GB"});
            this.cmbSizeFrom.Location = new System.Drawing.Point(134, 32);
            this.cmbSizeFrom.Name = "cmbSizeFrom";
            this.cmbSizeFrom.Size = new System.Drawing.Size(63, 21);
            this.cmbSizeFrom.TabIndex = 3;
            this.cmbSizeFrom.SelectedIndexChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // SizePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.txtSizeTo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSizeFrom);
            this.Controls.Add(this.cmbSizeTo);
            this.Controls.Add(this.txtSizeFrom);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.checkBox1);
            this.Name = "SizePanel";
            this.Size = new System.Drawing.Size(1479, 603);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox txtSizeFrom;
        private System.Windows.Forms.ComboBox cmbSizeTo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSizeTo;
        private System.Windows.Forms.ComboBox cmbSizeFrom;
    }
}
