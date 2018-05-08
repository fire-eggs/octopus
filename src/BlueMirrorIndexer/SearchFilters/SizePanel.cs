using System;
using System.Windows.Forms;

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class SizePanel : BasePanel
    {
        public SizePanel()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;

            checkBox1_CheckedChanged(null,null);
        }

        private void updateControls()
        {
            switch ((string)comboBox1.SelectedItem)
            {
                case "Between":
                    label1.Enabled = true;
                    textBox2.Enabled = true;
                    comboBox2.Enabled = true;
                    break;
                default:
                    label1.Enabled = false;
                    textBox2.Enabled = false;
                    comboBox2.Enabled = false;
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateControls();
            Raise();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = checkBox1.Checked;
            comboBox1.Enabled = comboBox3.Enabled = enabled;
            textBox1.Enabled = enabled;
            updateControls();
            Raise();
        }

        public override string GetText()
        {
            string text = "Size:(None)";
            if (checkBox1.Checked)
            {
                text = string.Format("Size:{0} {1}{2}", comboBox1.SelectedItem, textBox1.Text.Trim(), comboBox3.SelectedItem);
                if ((string)comboBox1.SelectedItem == "Between")
                    text += " and " + textBox2.Text.Trim() + comboBox2.SelectedItem;
            }
            return text;
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
