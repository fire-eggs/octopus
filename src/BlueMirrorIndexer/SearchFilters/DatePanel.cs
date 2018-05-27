using System;
using BlueMirrorIndexer.Components;

// TODO I18N: use of english strings for combobox selection tests

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class DatePanel : BasePanel, IFilterPanel
    {
        public DatePanel()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

            UpdateControls();
            checkBox1_CheckedChanged(null,null);

            // Insure the date picker set to midnight, otherwise before/after comparison later makes incorrect matches
            var today = DateTime.Now;
            dateTimePicker1.Value = new DateTime(today.Year,today.Month,today.Day);
            dateTimePicker2.Value = new DateTime(today.Year, today.Month, today.Day);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Raise();
        }

        private void UpdateControls()
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
            UpdateControls();
            Raise();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = checkBox1.Checked;
            comboBox1.Enabled = comboBox2.Enabled = enabled;
            dateTimePicker1.Enabled = enabled;
            UpdateControls();
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

            if ((string)comboBox1.SelectedItem == "Created Date")
            {
                switch ((string) comboBox2.SelectedItem)
                {
                    case "Before":
                        sea.DateType = SearchEventArgs.SearchDateType.CreateBefore;
                        break;
                    case "After":
                        sea.DateType = SearchEventArgs.SearchDateType.CreateAfter;
                        break;
                    case "Between":
                        sea.DateType = SearchEventArgs.SearchDateType.CreateBetween;
                        break;
                }
            }
            else if ((string)comboBox1.SelectedItem == "Modified Date")
            {
                switch ((string)comboBox2.SelectedItem)
                {
                    case "Before":
                        sea.DateType = SearchEventArgs.SearchDateType.ModBefore;
                        break;
                    case "After":
                        sea.DateType = SearchEventArgs.SearchDateType.ModAfter;
                        break;
                    case "Between":
                        sea.DateType = SearchEventArgs.SearchDateType.ModBetween;
                        break;
                }
            }
            else
            {
                sea.DateType = SearchEventArgs.SearchDateType.None;
                return;
            }

            sea.DateFrom = dateTimePicker1.Value;
            sea.DateTo = dateTimePicker2.Enabled ? dateTimePicker2.Value : (DateTime?)null;
        }
    }
}
