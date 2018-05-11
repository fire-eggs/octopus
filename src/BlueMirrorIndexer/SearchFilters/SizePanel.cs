using System;
using System.Windows.Forms;
using BlueMirrorIndexer.Components;

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class SizePanel : BasePanel, IFilterPanel
    {
        public SizePanel()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;
            cmbSizeTo.SelectedIndex = 0;
            cmbSizeFrom.SelectedIndex = 0;

            checkBox1_CheckedChanged(null,null);
        }

        private void UpdateControls(bool enabled=true)
        {
            switch ((string)comboBox1.SelectedItem)
            {
                case "Between":
                    label1.Enabled = true & enabled;
                    txtSizeTo.Enabled = true & enabled;
                    cmbSizeTo.Enabled = true & enabled;
                    break;
                default:
                    label1.Enabled = false;
                    txtSizeTo.Enabled = false;
                    cmbSizeTo.Enabled = false;
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
            Raise();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = checkBox1.Checked;
            comboBox1.Enabled = cmbSizeFrom.Enabled = enabled;
            txtSizeFrom.Enabled = enabled;
            UpdateControls(enabled);
            Raise();
        }

        public override string GetText()
        {
            string text = "Size:(None)";
            if (checkBox1.Checked)
            {
                text = string.Format("Size:{0} {1}{2}", comboBox1.SelectedItem, txtSizeFrom.Text.Trim(), cmbSizeFrom.SelectedItem);
                if ((string)comboBox1.SelectedItem == "Between")
                    text += " and " + txtSizeTo.Text.Trim() + cmbSizeTo.SelectedItem;
            }
            return text;
        }

        public void GetFilter(SearchEventArgs sea)
        {
            if (!checkBox1.Checked)
            {
                sea.SizeType = SearchEventArgs.SearchSizeRange.None;
                return;
            }

            SearchEventArgs.SearchSizeType res;
            switch ((string) comboBox1.SelectedItem)
            {
                case "Between":
                    sea.SizeType = SearchEventArgs.SearchSizeRange.Between;
                    sea.SizeTo = long.Parse(txtSizeTo.Text);
                    Enum.TryParse((string) cmbSizeTo.SelectedItem, out res);
                    sea.SizeToType = res;
                    break;
                case "Less than":
                    sea.SizeType = SearchEventArgs.SearchSizeRange.LessThan;
                    break;
                case "More than":
                    sea.SizeType = SearchEventArgs.SearchSizeRange.MoreThan;
                    break;
            }
            sea.SizeFrom = long.Parse(txtSizeFrom.Text);
            Enum.TryParse((string)cmbSizeFrom.SelectedItem, out res);
            sea.SizeFromType = res;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Raise();
        }
    }
}
