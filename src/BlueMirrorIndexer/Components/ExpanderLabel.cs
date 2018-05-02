using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BlueMirrorIndexer.Components
{
    public partial class ExpanderLabel : UserControl
    {
        public delegate void ExpandEventHandler(object sender, ExpandEventArgs e);

        public ExpanderLabel()
        {
            InitializeComponent();
        }

        [Description("Expander Label Text"),Browsable(true),EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        private event ExpandEventHandler _handler;
        public event ExpandEventHandler ExpandBtnClicked
        {
            add { _handler += value; }
            remove { _handler -= value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool expanded = ToggleButton();
            if (_handler != null)
                _handler.Invoke(this, new ExpandEventArgs{IsExpanded = expanded});
        }

        private bool ToggleButton()
        {
            bool isExpanded = button1.Text == "-";
            button1.Text = isExpanded ? "+" : "-";
            return !isExpanded;
        }
    }

    public class ExpandEventArgs
    {
        public bool IsExpanded { get; set; }
    }

}
