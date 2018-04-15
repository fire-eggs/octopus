using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Igorary.Forms.Forms;

namespace BlueMirrorIndexer
{
    public partial class DlgAbout : FormDialogList
    {
        
        public DlgAbout() {
            InitializeComponent();
            llCopyright.Text = assemblyCopyright;
            llVersion.Text = String.Format(llVersion.Text, assemblyVersion);
            llTitle.Text = ProductName;
            tbHistory.Text = Properties.Resources.History;
            tbLicense.Text = Properties.Resources.License;
        }

        private void linkLabel1_Click(object sender, EventArgs e) {
            Process navigate = new Process();
            navigate.StartInfo.FileName = "https://github.com/fire-eggs/octopus/issues";
            navigate.Start();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process navigate = new Process();
            navigate.StartInfo.FileName = "https://github.com/fire-eggs/octopus";
            navigate.Start();
        }

        private static string assemblyCopyright {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                    return "";
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        private static string assemblyVersion {
            get {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}

