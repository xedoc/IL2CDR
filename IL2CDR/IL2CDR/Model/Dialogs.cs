using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public static class Dialogs
    {
        public static string OpenFolderDialog( string startFolder )
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (!String.IsNullOrWhiteSpace(startFolder))
                dialog.SelectedPath = startFolder;

            dialog.Description = "Please select Mission Log folder";
            dialog.ShowNewFolderButton = false;
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK || result == DialogResult.Yes)
                return dialog.SelectedPath;
            else
                return String.Empty;
        }
    }
}
