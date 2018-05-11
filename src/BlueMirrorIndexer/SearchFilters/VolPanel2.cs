﻿using System.Collections.Generic;
using System.Windows.Forms;
using BlueMirrorIndexer.Components;

namespace BlueMirrorIndexer.SearchFilters
{
    public partial class VolPanel2 : BasePanel, IFilterPanel
    {
        public VolPanel2()
        {
            InitializeComponent();
        }

        public override string GetText()
        {
            string text = "Volumes:";

            bool any = false;
            bool all = true;
            foreach (ListViewItem lvi in lvVolumes.Items)
            {
                if (lvi.Checked)
                {
                    text += lvi.Text + ";";
                    any = true;
                }
                else
                {
                    all = false;
                }
            }
            if (!any || all)
                text = "Volumes: (All)";
            return text;
        }

        public void GetFilter(SearchEventArgs sea)
        {
            var vollist = sea.SearchInVolumes;
            vollist.Clear();
            bool none = true;
            foreach (ListViewItem item in lvVolumes.Items)
            {
                if (item.Checked)
                {
                    vollist.Add(item.Tag as DiscInDatabase);
                    none = false;
                }
            }

            if (none)
            {
                foreach (ListViewItem item in lvVolumes.Items)
                {
                    vollist.Add(item.Tag as DiscInDatabase);
                }
            }
        }

        internal void UpdateVolumeList(List<string> volnames)
        {
            lvVolumes.BeginUpdate();
            try
            {
                lvVolumes.Clear();
                foreach (var name in volnames)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = name;
                    lvi.ImageIndex = 0;
                    lvi.Checked = true;
                    //lvi.Tag = disc;
                    lvVolumes.Items.Add(lvi);
                }
            }
            finally
            {
                lvVolumes.EndUpdate();
            }
        }

        internal void UpdateVolumeList(VolumeDatabase database)
        {
            lvVolumes.BeginUpdate();
            try
            {
                lvVolumes.Clear();
                foreach (DiscInDatabase disc in database.GetDiscs())
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = disc.Name;
                    lvi.ImageIndex = 0;
                    lvi.Checked = true;
                    lvi.Tag = disc;
                    lvVolumes.Items.Add(lvi);
                }
            }
            finally
            {
                lvVolumes.EndUpdate();
            }
        }

        private void lvVolumes_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            Raise();
        }

        private void VolPanel_Load(object sender, System.EventArgs e)
        {
            // For some reason, having Raise() active during initialization
            // causes problems with the listview.
            lvVolumes.ItemChecked += lvVolumes_ItemChecked;
        }

    }
}