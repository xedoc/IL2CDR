using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows;

namespace IL2CDR.Model
{
    public class MissionLogDataService : IMissionLogDataService, IStopStart
    {
        private object lockHistory = new object();
        private List<object> missionHistory;
        private const string mask = "missionReport(*)[*].txt";
        private string missionDateTimePrefix = String.Empty;
        private TextFileTracker tracker;
        private ActionManager actionManager;

        public DateTime MissionStartDateTime { get; set; }
        public string MissionLogFolder { get; set; }

        public MissionLogDataService(string folder)
        {
            MissionLogFolder = folder;
            Initialize();
        }

        public void Initialize()
        {
            actionManager = (Application.Current as App).ActionManager;
            missionHistory = new List<object>();

            tracker = new TextFileTracker(MissionLogFolder, mask);
            tracker.OnNewLine = (line) => {
                var data = MissionLogDataBuilder.GetData(line, MissionStartDateTime);
                if( data != null && actionManager != null)
                {
                    actionManager.ProcessAction(data);
                    AddHistory(data);
                }
            };

            tracker.OnFileCreation = (filePath) =>
            {
                if (Regex.IsMatch(filePath, @"missionReport\([\d+|\-|_]+\)\[0\].txt", RegexOptions.IgnoreCase))
                {
                    //New mission log                    
                    ClearHistory();
                    StartNewMission(filePath);
                }
            };
        }
        private void StartNewMission(string logFilePath)
        {
            missionDateTimePrefix = Re.GetSubString(logFilePath, @"\((.*)?\)\[0\]\.txt");

            if (String.IsNullOrWhiteSpace(missionDateTimePrefix))
                return;

            MissionStartDateTime = Util.ParseDate(missionDateTimePrefix);
        }
        public void ReadMissionHistory()
        {
            //missionReport(2015-02-25_11-43-53)[0].txt

            var firstMissionLogFile = Util.GetNewestFilePath(MissionLogFolder, "missionReport(*)[0].txt");
            if( String.IsNullOrWhiteSpace( firstMissionLogFile ))
                return;
            
            StartNewMission(firstMissionLogFile);

            if (MissionStartDateTime.Equals(default(DateTime)))
                return;


            var missionFiles = Util.GetFilesSortedByTime(MissionLogFolder, String.Format("missionReport({0})[*].txt", missionDateTimePrefix), true);

            var readException = Util.Try(() => {
                foreach (var file in missionFiles)
                {
                    var lines = File.ReadAllLines(file);
                    if (lines != null)
                    {
                        foreach (var line in lines)
                        {
                            var data = MissionLogDataBuilder.GetData(line, MissionStartDateTime);
                            AddHistory(data);
                        }
                    }
                }                
            });
        }
        private void AddHistory( object data )
        {
            if (data == null )
                return;
            lock( lockHistory )
                missionHistory.Add(data);
        }
        private void ClearHistory()
        {
            lock( lockHistory )
                missionHistory.Clear();
        }
        public void Start()
        {
            if (String.IsNullOrWhiteSpace(MissionLogFolder))
                return;

            if (!Directory.Exists(MissionLogFolder))
                return;

            ClearHistory();
            ReadMissionHistory();
            tracker.Start();
        }

        public void Stop()
        {
            tracker.Stop();
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
