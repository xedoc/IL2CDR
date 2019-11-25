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
		public static string OpenFolderDialog(string startFolder)
		{
			var dialog = new FolderBrowserDialog();

			if (!string.IsNullOrWhiteSpace(startFolder)) {
				dialog.SelectedPath = startFolder;
			}

			dialog.Description = "Please select Mission Log folder";
			dialog.ShowNewFolderButton = false;
			var result = dialog.ShowDialog();

			if (result == DialogResult.OK || result == DialogResult.Yes) {
				return dialog.SelectedPath;
			} else {
				return string.Empty;
			}
		}

		public static string OpenFileDialog(string startFolder = null)
		{
			var targetFolder = startFolder ?? Directory.GetCurrentDirectory(); 

			var fileDialog = new OpenFileDialog {
				InitialDirectory = targetFolder,
				AddExtension = true,
				Filter = "Mission report files (.txt)|*.txt",
				DefaultExt = ".txt",
				FileName = Path.Combine(targetFolder, "missionReport.txt")
			};



			DialogResult result;
			var func = new Func<string>(() => {
				result = fileDialog.ShowDialog();
				if (result == DialogResult.OK || result == DialogResult.Yes) {
					return fileDialog.FileName;
				} else {
					return null;
				}
			});
			if (System.Windows.Application.Current != null) {
				return UI.DispatchFunc(() => func());
			} else {
				return func();
			}
		}
	}
}