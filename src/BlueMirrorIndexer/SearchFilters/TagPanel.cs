
using BlueMirrorIndexer.Components;

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class TagPanel : BasePanel, IFilterPanel
    {
        public TagPanel()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            checkBox1_CheckedChanged(null,null);
        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            var enabled = checkBox1.Checked;
            comboBox1.Enabled = textBox1.Enabled = checkBox2.Enabled = enabled;
            label1.Enabled = enabled;
            Raise();
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Raise();
        }

        public override string GetText()
        {
            string text = "Keywords: (none)";
            if (checkBox1.Checked)
            {
                text = string.Format("Keywords: {0} of \"{1}\"{2}", comboBox1.SelectedItem, textBox1.Text.Trim(),
                    checkBox2.Checked ? "(wild)" : "");
            }
            return text;
        }

        public void GetFilter(SearchEventArgs sea)
        {
            if (!checkBox1.Checked)
                sea.Keywords = null;
            else
            {
                sea.Keywords = textBox1.Text;
                // TODO keyword style
                sea.TreatKeywordsAsWildcard = checkBox2.Checked;
            }
        }
    }
}
