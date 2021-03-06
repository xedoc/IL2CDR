﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IL2CDR.Model
{
    public static class Log
    {
        private static string CurrentDateTime()
        {
            var curDT = DateTime.Now;
            return String.Concat(curDT.ToShortDateString(), " ", curDT.ToLongTimeString());
        }
        public static void WriteError(String message)
        {
            var text = String.Format("[{1}] Error: {0}", message, CurrentDateTime());
            AppendToLogFile(text);
        }
        public static void WriteWarning(String message)
        {
            var text = String.Format("[{1}] Warning: {0}", message, CurrentDateTime());
            AppendToLogFile(text);
        }
        public static void WriteInfo(String message)
        {
            var text = String.Format("[{1}] {0}", message, CurrentDateTime());
            AppendToLogFile(text);
        }

        public static void WriteError(String format, params object[] args)
        {
            var text = String.Format("[" + CurrentDateTime() + "] Error: " + format, args);
            AppendToLogFile(text);
        }
        public static void WriteWarning(String format, params object[] args)
        {
            var text = String.Format("[" + CurrentDateTime() + "] Warning: " + format, args);
            AppendToLogFile(text);
        }
        public static void WriteInfo(String format, params object[] args)
        {
            var text = String.Format("[" + CurrentDateTime() + "] " + format, args);
            AppendToLogFile(text);
        }

        private static void AppendToLogFile(string text)
        {
            if( Application.Current != null)
                (Application.Current as App).AppLogDataService.AddMessage(text);
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
