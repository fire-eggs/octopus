
using BlueMirrorIndexer.Components;

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class FilePanel : BasePanel, IFilterPanel
    {
        public FilePanel()
        {
            InitializeComponent();
        }

        private string getMask()
        {
            string txt = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(txt))
                txt = "*.*";
            return txt;
        }

        private string getWild()
        {
            return checkBox1.Checked ? "(wild)" : "";
        }

        public override string GetText()
        {
            return string.Format("File: {0}{1}", getMask(), getWild());
        }

        public void GetFilter(SearchEventArgs sea)
        {
            sea.FileMask = getMask();
            sea.TreatFileMaskAsWildcard = checkBox1.Checked;
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            Raise();
        }
    }
}
