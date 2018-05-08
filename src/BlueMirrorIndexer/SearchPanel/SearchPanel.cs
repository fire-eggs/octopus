using BlueMirror.Commons.Controls;
using BlueMirrorIndexer.SearchFilters;
// TODO
using System.Drawing;
using System.Windows.Forms;

namespace BlueMirrorIndexer.SearchPanel
{
    public partial class SearchPanel : UserControl
    {
        private VolPanel2 volP;

        public SearchPanel()
        {
            InitializeComponent();

            accordion1.Font = new Font("Microsoft Sans Serif", 10);
            var fileP = new FilePanel();
            var sizeP = new SizePanel();
            var tagP = new TagPanel();
            var dateP = new DatePanel();
            volP = new VolPanel2();
            var miscP = new OtherPanel();

            // TODO restore last open/closed states
            var txt = fileP.GetText();
            accordion1.Add(fileP, txt, txt, open: true);
            txt = dateP.GetText();
            accordion1.Add(dateP, txt, txt, open: false);
            txt = sizeP.GetText();
            accordion1.Add(sizeP, txt, txt, open: false);
            txt = tagP.GetText();
            accordion1.Add(tagP, txt, txt, open: false);
            txt = volP.GetText();
            accordion1.Add(volP, txt, txt, open: false);
            txt = miscP.GetText();
            accordion1.Add(miscP, txt, txt, open: false);

            // TODO can this be automated somehow?
            fileP.FilterChange += FilterChange;
            sizeP.FilterChange += FilterChange;
            dateP.FilterChange += FilterChange;
            tagP.FilterChange += FilterChange;
            volP.FilterChange += FilterChange;
            miscP.FilterChange += FilterChange;

        }

        void FilterChange(object sender, FilterChangeArgs e)
        {
            accordion1.SetText(sender as Control, e.FilterText);
            accordion1.SetTooltip(sender as Control, e.FilterText);
        }

        public void UpdateVolumeList(VolumeDatabase database)
        {
            if (volP != null)
                volP.UpdateVolumeList(database);
        }

        public ListViewVista GetSearchList()
        {
            return lvSearchResults;
        }
    }
}
