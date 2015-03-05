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
        private Server server;
        private Dictionary<EventType, Action<MissionLogEventHeader>> historyHandlers = new Dictionary<EventType, Action<MissionLogEventHeader>>()
        {
            { EventType.AirfieldInfo, (data) => { 
                data.With( x => x as MissionLogEventAirfieldInfo)
                    .With( x => x.AirField)
                    .Do( x => data.Server.AirFields[x.Id] = x);
            }},
            { EventType.PlaneSpawn, (data) => {
                data.With(x => x as MissionLogEventPlaneSpawn)
                    .With(x => x.Player)
                    .Do(x => data.Server.Players[x.Id] = x);
            }},
            { EventType.Leave, (data) => {
                data.With(x => x as MissionLogEventPlayerLeave)
                    .Do(x => {
                        data.Server.Players.PlayerLeave(x.NickId);
                    });

            }},
            { EventType.Kill, (data) => {
                data.With(x => x as MissionLogEventKill)
                    .Do(x => {
                        data.Server.Players.PlayerKilled(x.TargetId);
                    });
            }},

                
        };

        public DateTime MissionStartDateTime { get; set; }
        public string MissionLogFolder { get; set; }

        public MissionLogDataService(Server server)
        {
            this.server = server;
            MissionLogFolder = server.Rcon.Config.MissionTextLogFolder;
            Initialize();
        }
        public void Initialize()
        {
            
            missionHistory = new List<object>();

            tracker = new TextFileTracker(MissionLogFolder, mask);
            tracker.OnNewLine = (line) => {
                actionManager = (Application.Current as App).ActionManager;
                var data = MissionLogDataBuilder.GetData(line, MissionStartDateTime, GetCurrentEventNumber(), server);
                if( data != null && actionManager != null)
                {
                    AddHistory(data);
                    actionManager.ProcessAction(data);
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
                            var data = MissionLogDataBuilder.GetData(line, MissionStartDateTime, GetCurrentEventNumber(), server);
                            AddHistory(data);
                        }
                    }
                }                
            });
        }
        private int GetCurrentEventNumber()
        {
            int result;
            lock (lockHistory)
                result = missionHistory.Count;

            return result;
        }
        private void AddHistory( object data )
        {

            if (data == null )
                return;

            lock( lockHistory )
                missionHistory.Add(data);

            var header = (data as MissionLogEventHeader);
            if( header != null )
            {
                Action<MissionLogEventHeader> action;
                if( historyHandlers.TryGetValue( header.Type, out action ) )
                {
                    action(header);
                }
            }
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
