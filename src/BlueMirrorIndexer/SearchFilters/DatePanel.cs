using System;
using BlueMirrorIndexer.Components;

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class DatePanel : BasePanel, IFilterPanel
    {
        public DatePanel()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

            updateControls();
            checkBox1_CheckedChanged(null,null);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Raise();
        }

        private void updateControls()
        {
            switch ((string)comboBox2.SelectedItem)
            {
                case "Between":
                    dateTimePicker2.Enabled = true;
                    label2.Enabled = true;
                    break;
                default:
                    dateTimePicker2.Enabled = false;
                    label2.Enabled = false;
                    break;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateControls();
            Raise();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = checkBox1.Checked;
            comboBox1.Enabled = comboBox2.Enabled = enabled;
            dateTimePicker1.Enabled = enabled;
            updateControls();
            Raise();
        }

        public override string GetText()
        {
            string text = "Date:(None)";
            if (checkBox1.Checked)
            {
                text = string.Format("Date:{0} {1} {2}", comboBox1.SelectedItem, comboBox2.SelectedItem, dateTimePicker1.Text);
                if ((string) comboBox2.SelectedItem == "Between")
                    text += " and " + dateTimePicker2.Text;
            }
            return text;
        }

        public void GetFilter(SearchEventArgs sea)
        {
            if (!checkBox1.Checked)
            {
                sea.DateTo = sea.DateFrom = null;
                sea.DateType = SearchEventArgs.SearchDateType.None;
                return;
            }

            SearchEventArgs.SearchDateType res;
            Enum.TryParse((string) comboBox2.SelectedItem, out res);
            sea.DateType = res;

            sea.DateFrom = dateTimePicker1.Value;
            sea.DateTo = dateTimePicker2.Enabled ? dateTimePicker2.Value : (DateTime?)null;
        }
    }
}
