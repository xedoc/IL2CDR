using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
	public static class ProcessUtils
	{


		/// <summary>
		/// Obtaining command line of a process.
		///
		/// Implementation taken from StackOverflow (https://stackoverflow.com/a/2633674) 
		/// </summary>
		/// <param name="process"></param>
		/// <returns></returns>
		public static string CommandLine(this Process process)
		{

			try {
				using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
				using (ManagementObjectCollection objects = searcher.Get()) {
					return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
				}

			} catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005) {
				// Intentionally empty - no security access to the process.
				return null; 
			} catch (InvalidOperationException) {
				// Intentionally empty - the process exited before getting details.
				return null; 
			}


		}


	}
}
