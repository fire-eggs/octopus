using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using SizeCont = System.Tuple<BlueMirrorIndexer.IFolder, ulong>;

namespace BlueMirrorIndexer.Components
{
    public partial class Charter : UserControl
    {
        public Charter()
        {
            InitializeComponent();

            ChartItem ci = new ChartItem();
            ci.text = "Bar: Total file size";
            ci.tag = 0;
            cmbChart.Items.Add(ci);
            ci = new ChartItem();
            ci.text = "Bar: Total file count";
            ci.tag = 1;
            cmbChart.Items.Add(ci);

            chartType = 0;
        }

        private VolumeDatabase _database;

        public VolumeDatabase Database
        {
            private get { return _database; }
            set
            {
                _database = value;
                folds = null;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (folds == null)
                CalcChart();

            if (folds == null || folds.Count < 1)
            {
                e.Graphics.Clear(Color.Red);
                return;
            }

            ulong first = sort[0].Item2;             //ulong superTotal = Database.TotalSizeUsed;

            int y = spacing;
            int maxW = panel2.Width - 6;
            int dex = 0;

            Brush textb = new SolidBrush(Color.CadetBlue);
            Pen dPen = new Pen(Color.Blue);

            var g = e.Graphics;
            g.Clear(Color.Azure);
            do
            {
                int w = (int)(maxW * ((double)sort[dex].Item2 / (double)first));
                g.DrawRectangle(dPen, spacing, y, w, rectH);

                var fold = sort[dex].Item1 as FolderInDatabase;
                string disp = "";
                if (chartType == 0)
                    disp = string.Format("{0}{1} - {2}", fold.GetVolumeUserName(), fold.Name, FormatAsMb(sort[dex].Item2));
                else if (chartType == 1)
                    disp = string.Format("{0}{1} - {2:n0}", fold.GetVolumeUserName(), fold.Name, sort[dex].Item2);

                g.DrawString(disp, DefaultFont, textb, spacing, y);

                y += rectH;
                y += spacing;
                dex++;
            }
            while (y < panel2.Height && dex < sort.Count);
        }

        private List<SizeCont> folds;
        private List<SizeCont> sort;
        private ItemInDatabase baseFold;
        private int chartType;

        private void addFold(IFolder fold, uint csize)
        {
            ulong size = 0;
            switch (chartType)
            {
            case 0:
                size = chkClusters.Checked
                    ? sizeCluster(fold.TotalSizeUsed, csize)
                    : fold.TotalSizeUsed;
                    break;
            case 1:
                size = fold.TotalFileCount;
                break;
            }
            SizeCont tup = new SizeCont(fold, size);
            folds.Add(tup);
        }

        private void CalcChart()
        {
            if (Database == null)
                return;
            folds = new List<SizeCont>();
            if (baseFold == null)
            {
                foreach (var discInDatabase in Database.GetDiscs())
                {
                    foreach (var fold in ((IFolder) discInDatabase).Folders)
                    {
                        addFold(fold, discInDatabase.ClusterSize);
                    }
                }
            }
            else
            {
                uint csize = baseFold.GetVolumeClusterSize();
                foreach (var fold in ((IFolder)baseFold).Folders)
                {
                    addFold(fold, csize);
                }
            }
            sort = folds.OrderByDescending(x => x.Item2).ToList();
        }

        private ulong sizeCluster(ulong size, uint cSize)
        {
            ulong cSizeL = (ulong) cSize;
            ulong clusters = size/cSizeL;
            if (size%cSizeL != 0)
                clusters++;
            return clusters*cSize;
        }

        const int rectH = 15; // TODO derive from font when text drawn
        const int spacing = 3;

        private string FormatAsMb(ulong val)
        {
            double val2 = val / 1024.0 / 1024.0;
            return string.Format("{0:0,0.##}M", val2);
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            if (folds == null || folds.Count < 1)
                return;

            int dex = e.Y / (spacing + rectH);
            if (dex >= folds.Count)
                return;

            IFolder fold = sort[dex].Item1;
            if (e.Button == MouseButtons.Left)
            {
                baseFold = fold as ItemInDatabase;
                reChart();
            }
            else
            {
                RightMenu(fold, e.Location);
            }
        }

        private void reChart()
        {
            folds = null;
            panel2.Invalidate();
        }

        private void btnRoot_Click(object sender, EventArgs e)
        {
            baseFold = null;
            reChart();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (baseFold == null)
                return;

            baseFold = baseFold.Parent as ItemInDatabase;
            if (baseFold is DiscInDatabase)
                baseFold = null;
            reChart();
        }

        private void chkClusters_CheckedChanged(object sender, EventArgs e)
        {
            folds = null;
            panel2.Invalidate();
        }

        private void chkFlatten_CheckedChanged(object sender, EventArgs e)
        {
            folds = null;
            panel2.Invalidate();
        }

        private void cmbChart_SelectedIndexChanged(object sender, EventArgs e)
        {
            chartType = (cmbChart.SelectedItem as ChartItem).tag;
            folds = null;
            panel2.Invalidate();
        }

        private void cmbVolume_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void RightMenu(IFolder fold, Point loc )
        {
            ContextMenu.Tag = fold;
            ContextMenu.Show(contextMenuStrip1, loc);
        }
    }

    class ChartItem
    {
        public string text;
        public int tag;

        public override string ToString()
        {
            return text;
        }
    }
}
