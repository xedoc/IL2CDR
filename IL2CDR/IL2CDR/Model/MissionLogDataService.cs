using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Text.RegularExpressions;

namespace IL2CDR.Model
{
    public class MissionLogDataService : IMissionLogDataService, IStopStart
    {
        private List<object> missionHistory;
        private const string mask = "missionReport(*)[*].txt";
        private string missionDateTime = String.Empty;
        private TextFileTracker tracker;
        private string folder;
        public MissionLogDataService(string folder)
        {
            this.folder = folder;
            Initialize();
        }
        public void Initialize()
        {
            missionHistory = new List<object>();
            tracker = new TextFileTracker(folder, mask);
            tracker.OnNewLine = (line) => {
                var data = MissionLogDataBuilder.GetData(line);
                if( data != null )
                {
                    //TODO: Send data to ActionManager
                }
            };
        }
        public void ReadMissionHistory()
        {
            missionDateTime = Util.GetNewestFilePath(folder, "missionReport(*)[0].txt")
                .With(x => Re.GetSubString(x, @".*?\((.*)?\)[0]\.txt"));

            var missionFiles = Util.GetFilesSortedByTime(folder, String.Format("missionReport({0})[*].txt", missionDateTime), true);

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
                            {
                                missionHistory.Add(data);
                            }
                        }
                    }
                }                
            });
        }
        public void Start()
        {
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
