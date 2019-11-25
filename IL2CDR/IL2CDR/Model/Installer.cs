using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Diagnostics;

namespace IL2CDR
{
	public class Installer
	{
		private const string UNINSTALL_REGISTRY_KEY = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

		public static string GetDirectoryByDisplayName(string partialName)
		{
			if (string.IsNullOrWhiteSpace(partialName)) {
				return null;
			}

			var rootKey = Registry.LocalMachine.OpenSubKey(UNINSTALL_REGISTRY_KEY);
			if (rootKey == null) {
				return null; 
			}

			string result = null;

			var nameList = rootKey.GetSubKeyNames();

			Parallel.For(0, nameList.Length, (i, loopState) => {
				var regKey = rootKey.OpenSubKey(nameList[i]);
				var displayName = regKey?.GetValue("DisplayName");
				if (displayName == null || !displayName.ToString().Contains(partialName)) {
					return;
				}

				var installLocation = regKey.GetValue("InstallLocation");
				if (installLocation == null) {
					return;
				}

				result = regKey.GetValue("InstallLocation").ToString();
				loopState.Stop();
			});

			return result;
		}
	}
}