using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BlueMirrorIndexer.Components
{
    public partial class Filters : UserControl
    {
        private event SearchEventHandler searchBtnClicked;

        private ExpandEventArgs unexpand = new ExpandEventArgs {IsExpanded = false};

        public Filters()
        {
            InitializeComponent();

            // TODO why isn't designer setting of text 'sticking'?
            fileExpander.Text = "Filename";
            dateExpander.Text = "Date";
            keywordExpander.Text = "Keywords";
            sizeExpander.Text = "Size";
            volExpander.Text = "Volumes";

            dateExpander_ExpandBtnClicked(null, unexpand);
            keywordExpander_ExpandBtnClicked(null, unexpand);
            sizeExpander_ExpandBtnClicked(null, unexpand);
            volExpander_ExpandBtnClicked(null, unexpand);

            UpdateDateControls();
            UpdateKeywordControls();
            UpdateSizeControls();

            tbSizeFrom.KeyPress += sizeBox_KeyPress;
            tbSizeTo.KeyPress += sizeBox_KeyPress;
        }

        public event SearchEventHandler SearchBtnClicked
        {
            add { searchBtnClicked += value; }
            remove { searchBtnClicked -= value; }
        }

        private void dateExpander_ExpandBtnClicked(object sender, ExpandEventArgs e)
        {
            datePanel.Visible = e.IsExpanded;
        }

        private void fileExpander_ExpandBtnClicked(object sender, ExpandEventArgs e)
        {
            fileNamePanel.Visible = e.IsExpanded;
        }

        private void keywordExpander_ExpandBtnClicked(object sender, ExpandEventArgs e)
        {
            keywordPanel.Visible = e.IsExpanded;
        }

        private void UpdateDateControls()
        {
            dtpDateFrom.Enabled = dtpDateTo.Enabled = cbDate.Checked;
        }

        private void UpdateKeywordControls()
        {
            if (cmbOneAll.SelectedIndex == -1)
                cmbOneAll.SelectedIndex = 0;
            var enabled = cbKeywords.Checked;
            tbKeywords.Enabled = 
                cmbOneAll.Enabled = 
                cbCaseSensitiveKeywords.Enabled = 
                cbTreatKeywordAsWildcard.Enabled = enabled;
        }

        private void tbFileMask_TextChanged(object sender, System.EventArgs e)
        {
            FiltersChanged();
        }

        private void dtpDateFrom_ValueChanged(object sender, System.EventArgs e)
        {
            FiltersChanged();
        }

        private void dtpDateTo_ValueChanged(object sender, System.EventArgs e)
        {
            FiltersChanged();
        }

        private void cbDate_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateDateControls();
            FiltersChanged();
        }

        private void FiltersChanged()
        {
            // send event with latest filter settings
        }

        private void cbKeywords_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateKeywordControls();
            FiltersChanged();
        }

        private void sizeExpander_ExpandBtnClicked(object sender, ExpandEventArgs e)
        {
            sizePanel.Visible = e.IsExpanded;
        }

        private void cbSize_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateSizeControls();
            FiltersChanged();
        }

        private void UpdateSizeControls()
        {
            tbSizeFrom.Enabled = tbSizeTo.Enabled = cbSize.Checked;
        }

        private void sizeBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void BtnSearch_Click(object sender, System.EventArgs e)
        {
            if (searchBtnClicked == null)
                return;

            SearchEventArgs see = new SearchEventArgs();
            if (string.IsNullOrWhiteSpace(tbFileMask.Text))
                see.FileMask = "*.*";
            else
                see.FileMask = tbFileMask.Text;
            see.TreatFileMaskAsWildcard = cbTreatFileMaskAsWildcard.Checked;

            if (cbDate.Checked)
            {
                see.DateFrom = dtpDateFrom.Value;
                see.DateTo = dtpDateTo.Value;
                // TODO range check
            }
            if (cbKeywords.Checked)
            {
                see.Keywords = tbKeywords.Text;
                see.AllKeywordsNeeded = cmbOneAll.SelectedIndex == 1;
                see.TreatKeywordsAsWildcard = cbTreatKeywordAsWildcard.Checked;
            }
            if (cbSize.Checked)
            {
                see.SizeFrom = Convert.ToInt64(float.Parse(tbSizeFrom.Text) * 1024);
                see.SizeTo = Convert.ToInt64(float.Parse(tbSizeTo.Text) * 1024);

                // TODO range check
            }

            foreach (ListViewItem lvi in lvVolumes.Items)
                if (lvi.Checked)
                    see.SearchInVolumes.Add((DiscInDatabase)lvi.Tag);

            searchBtnClicked(this, see);
        }

        internal void UpdateVolumeList(VolumeDatabase database)
        {
            lvVolumes.BeginUpdate();
            try
            {
                lvVolumes.Clear();
                foreach (DiscInDatabase disc in database.GetDiscs())
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = disc.Name;
                    lvi.ImageIndex = 0;
                    lvi.Checked = true;
                    lvi.Tag = disc;
                    lvVolumes.Items.Add(lvi);
                }
            }
            finally
            {
                lvVolumes.EndUpdate();
            }
        }

        private void volExpander_ExpandBtnClicked(object sender, ExpandEventArgs e)
        {
            volumePanel.Visible = e.IsExpanded;
        }

    }
}
