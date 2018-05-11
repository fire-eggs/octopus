using System;
using BlueMirrorIndexer.Components;

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class OtherPanel : BasePanel, IFilterPanel
    {
        public OtherPanel()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Raise();
        }

        public override string GetText()
        {
            var txt = "Other:";
            if (checkBox1.Checked)
                txt += "Duplicates;";
            if (checkBox2.Checked)
                txt += "No folders";
            if (!checkBox1.Checked && !checkBox2.Checked)
                txt += "(None)";
            return txt;
        }

        public void GetFilter(SearchEventArgs sea)
        {
            sea.OnlyDuplicates = checkBox1.Checked;
        }
    }
}
