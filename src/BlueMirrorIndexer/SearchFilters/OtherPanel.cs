using System;

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class OtherPanel : BasePanel
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
            return "Other:" + (checkBox1.Checked ? "Duplicates" : "(None)");
        }
    }
}
