using Igorary.Forms;
using Igorary.Utils.Utils.Extensions;

namespace BlueMirrorIndexer
{
    public partial class DlgFileProperties : DlgItemProperties
    {

        public DlgFileProperties(FileInDatabase fileInDatabase)
            : base(fileInDatabase) {
            InitializeComponent();
            if (fileInDatabase.CRC != 0)
                llCrc.Text = fileInDatabase.CRC.ToString("X8");
            else
                llCrc.Text = "";
            llFileSize.Text = fileInDatabase.Length.ToKBAndB();
            llFileDescription.Text = string.IsNullOrEmpty(fileInDatabase.FileDescription) ? "(empty)" : fileInDatabase.FileDescription;
            llFileVersion.Text = string.IsNullOrEmpty(fileInDatabase.FileVersion) ? "(empty)" : fileInDatabase.FileVersion;
            pbIcon.Image = Win32.GetFileIcon(fileInDatabase.Name, Win32.FileIconSize.Large).ToBitmap();
        }
        
    }
}

