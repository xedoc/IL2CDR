using System;
using System.Diagnostics;
using System.Windows;

namespace IL2CDR.Model
{
	public static class Log
	{
		private static string CurrentDateTime()
		{
			var curDT = DateTime.Now;
			return string.Concat(curDT.ToShortDateString(), " ", curDT.ToLongTimeString());
		}

		public static void WriteError(string message)
		{
			var text = string.Format("[{1}] Error: {0}", message, CurrentDateTime());
			AppendToLogFile(text);
		}

		public static void WriteWarning(string message)
		{
			var text = string.Format("[{1}] Warning: {0}", message, CurrentDateTime());
			AppendToLogFile(text);
		}

		public static void WriteInfo(string message)
		{
			var text = string.Format("[{1}] {0}", message, CurrentDateTime());
			AppendToLogFile(text);
		}

		public static void WriteError(string format, params object[] args)
		{
			var text = string.Format("[" + CurrentDateTime() + "] Error: " + format, args);
			AppendToLogFile(text);
		}

		public static void WriteWarning(string format, params object[] args)
		{
			var text = string.Format("[" + CurrentDateTime() + "] Warning: " + format, args);
			AppendToLogFile(text);
		}

		public static void WriteInfo(string format, params object[] args)
		{
			var text = string.Format("[" + CurrentDateTime() + "] " + format, args);
			AppendToLogFile(text);
		}

		private static void AppendToLogFile(string text)
		{
			if (Application.Current != null && Application.Current is App app) {
				app.AppLogDataService.AddMessage(text);
			}

			//try
			//{
			//    File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\il2cdr_log.txt", text + Environment.NewLine);
			//}
			//catch (Exception e)
			//{
			//    Debug.Print("Error adding record to the log file");
			//}
			Debug.Print(text);
		}
	}
}