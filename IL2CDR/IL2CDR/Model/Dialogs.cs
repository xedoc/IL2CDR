using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.IO;

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

        public static string OpenFileDialog( string startFolder = null )
        {
            OpenFileDialog fileDialog = new OpenFileDialog();


            fileDialog.InitialDirectory = startFolder;
            fileDialog.AddExtension = true;
            fileDialog.Filter = "Mission report files (.txt)|*.txt";
            fileDialog.DefaultExt = ".txt";
            fileDialog.FileName = Path.Combine(startFolder, "missionReport.txt");

            DialogResult result;
            var func = new Func<string>(()=> {
                    result = fileDialog.ShowDialog();
                    if (result == DialogResult.OK || result == DialogResult.Yes)
                        return fileDialog.FileName;
                    else
                        return null;
                });
            if( System.Windows.Application.Current != null )
            {
                return UI.DispatchFunc(() =>
                {
                    return func();
                });
            }
            else
            {
                return func();
            }
        }
    }
}
