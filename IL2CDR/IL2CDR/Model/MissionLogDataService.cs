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
        private List<object> missionHistory;
        private const string mask = "missionReport(*)[*].txt";
        private string missionDateTime = String.Empty;
        private TextFileTracker tracker;
        private ActionManager actionManager;
        public MissionLogDataService(string folder)
        {
            MissionLogFolder = folder;
            Initialize();
        }
        public DateTime MissionStartDateTime { get; set; }
        public string MissionLogFolder { get; set; }
        public void Initialize()
        {
            actionManager = (Application.Current as App).ActionManager;
            missionHistory = new List<object>();
            tracker = new TextFileTracker(MissionLogFolder, mask);
            tracker.OnNewLine = (line) => {
                var data = MissionLogDataBuilder.GetData(line);
                if( data != null && actionManager != null)
                {
                    actionManager.ProcessAction(data);
                    missionHistory.Add(data);
                }
            };
        }
        public void ReadMissionHistory()
        {
            //missionReport(2015-02-25_11-43-53)[0].txt

            var firstMissionLogFile = Util.GetNewestFilePath(MissionLogFolder, "missionReport(*)[0].txt");
            if( String.IsNullOrWhiteSpace( firstMissionLogFile ))
                return;

            missionDateTime = Re.GetSubString(firstMissionLogFile, @".*?\((.*)?\)[0]\.txt");
            
            if( String.IsNullOrWhiteSpace( missionDateTime))
                return;



            var missionFiles = Util.GetFilesSortedByTime(MissionLogFolder, String.Format("missionReport({0})[*].txt", missionDateTime), true);

            var readException = Util.Try(() => {
                foreach (var file in missionFiles)
                {
                    var lines = File.ReadAllLines(file);
                    if (lines != null)
                    {
                        foreach (var line in lines)
                        {
                            var data = MissionLogDataBuilder.GetData(line);
                            if (data != null)
                                missionHistory.Add(data);
                        }
                    }
                }                
            });
        }
        public void Start()
        {
            if (String.IsNullOrWhiteSpace(MissionLogFolder))
                return;

            if (!Directory.Exists(MissionLogFolder))
                return;

            missionHistory.Clear();
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
