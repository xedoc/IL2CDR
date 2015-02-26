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
            tracker = new TextFileTracker(folder, mask);
            tracker.OnNewLine = (line) => {
                var header = new MissionLogEventBase(line);
                if( header.Type != EventType.Unknown && 
                    header.Type != EventType.Version )
                {
                    //TODO: create data object by given event type
                    //TODO: send data to ActionManager
                }
            };
        }
        public void ReadMissionHistory()
        {
            missionDateTime = Util.GetNewestFilePath(folder, "missionReport(*)[0].txt")
                .With(x => Re.GetSubString(x, @".*?\((.*)?\)[0]\.txt"));

            var missionFiles = Util.GetFilesSortedByTime(folder, String.Format("missionReport({0})[*].txt", missionDateTime), true);

            foreach( var file in missionFiles )
            {
               var lines = File.ReadAllLines(file);
               if( lines != null )
               {
                   foreach( var line in lines )
                   {
                       var header = new MissionLogEventBase(line);
                       if (header.Type != EventType.Unknown &&
                           header.Type != EventType.Version)
                       {
                           //TODO: create data object by given event type
                           //TODO: save history
                       }
                   }
               }
            }
        }
        public void Start()
        {
            ReadMissionHistory();
        }

        public void Stop()
        {
            
        }

        public void Restart()
        {
            
        }
    }
}
