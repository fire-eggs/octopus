﻿using System;
using System.Windows.Forms;

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class BasePanel : UserControl
    {
        public BasePanel()
        {
            InitializeComponent();
            if (!DesignMode)
                Dock = DockStyle.Fill;
        }

        private event FilterChangeHandler searchBtnClicked;

        public event FilterChangeHandler FilterChange
        {
            add { searchBtnClicked += value; }
            remove { searchBtnClicked -= value; }
        }

        public virtual void SomethingChanged(object sender, EventArgs e)
        {
            Raise();
        }

        protected void Raise()
        {
            if (searchBtnClicked == null)
                return;

            FilterChangeArgs fca = new FilterChangeArgs { FilterText = GetText() };
            searchBtnClicked(this, fca);
        }

        public virtual string GetText()
        {
            return "";
        }
    }

    public delegate void FilterChangeHandler(object sender, FilterChangeArgs e);

    public class FilterChangeArgs
    {
        public string FilterText { get; set; }
    }

}
