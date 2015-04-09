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
        private DateTime lastEventTime;
        private long lastTick;
        private Dictionary<EventType, Action<MissionLogEventHeader>> historyHandlers = new Dictionary<EventType, Action<MissionLogEventHeader>>()
        {
            { EventType.AirfieldInfo, (data) => data.With( x => x as MissionLogEventAirfieldInfo)
                    .With( x => x.AirField).Do( x => data.Server.AirFields[x.Id] = x)},
            { EventType.PlaneSpawn, (data) => data.With(x => x as MissionLogEventPlaneSpawn)
                    .With(x => x.Player).Do(x => data.Server.Players[x.Id] = x)},
            { EventType.GameObjectSpawn, (data) => data.With(x => x as MissionLogEventGameObjectSpawn)
                    .With(x => x.Object).Do(x => data.Server.GameObjects[x.Id] = x)},
            { EventType.Leave, (data) =>data.With(x => x as MissionLogEventPlayerLeave)
                    .Do(x => data.Server.Players.PlayerLeave(x.NickGuid))},
            { EventType.Kill, (data) => data.With(x => x as MissionLogEventKill)
                .Do(x => data.Server.DestroyObject(x.TargetId))},
            { EventType.MissionStart, (data) => data.With(x => x as MissionLogEventStart)
                .Do(x => data.Server.CoalitionIndexes = x.CoalitionIndexes)},
            { EventType.Hit, (data) => data.With(x => x as MissionLogEventHit)
                .Do(x => data.Server.AddHit( x as MissionLogEventHit))},
            { EventType.Damage, (data) => data.With(x => x as MissionLogEventDamage)
                .Do(x => data.Server.AddDamage( x as MissionLogEventDamage))},
        };

      
        public List<object> MissionHistory
        {
            get { return missionHistory; }
            set { missionHistory = value; }
        }
        

        public DateTime MissionStartDateTime { get; set; }
        public string MissionLogFolder { get; set; }

        public MissionLogDataService(Server server)
        {
            this.server = server;
            MissionLogFolder = server.Rcon.Config.MissionTextLogFolder;
            Initialize();
        }
        public MissionLogDataService(string missionLogFolder)
        {
            Log.WriteInfo("Start monitoring of {0} folder", missionLogFolder);
            MissionLogFolder = missionLogFolder;
            server = new Server("LogParser", true, true);
            missionHistory = new List<object>();
        }
        public void Initialize()
        {
            missionHistory = new List<object>();
            tracker = new TextFileTracker(MissionLogFolder, mask);
            tracker.OnNewLine = (line) => {
                var data = MissionLogDataBuilder.GetData(line, MissionStartDateTime, GetCurrentEventNumber(), server);
                
                if( data != null && actionManager != null)
                {
                    var header = data as MissionLogEventHeader;
                    header.MissionFile = Path.GetFileName(tracker.CurrentFileName);

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
            //Check if MissionEnd is sent
            if( missionHistory != null && missionHistory.Count > 0 )
            {
                var existing = missionHistory.FirstOrDefault(data => data is MissionLogEventMissionEnd);
                if( existing == null )
                {
                    var endMission = new MissionLogEventMissionEnd(new MissionLogEventHeader(String.Format("T:{0} AType:7", lastTick), lastEventTime));
                    AddHistory(endMission);
                    actionManager.ProcessAction(endMission);
                }
            }
            Log.WriteInfo("New mission started {0}", logFilePath);
            missionDateTimePrefix = Re.GetSubString(logFilePath, @"missionReport\((.*)?\)\[0\]\.txt");

            if (String.IsNullOrWhiteSpace(missionDateTimePrefix))
                return;
            
            server.ResetMission();

            server.CurrentMissionId = GuidUtility.Create(GuidUtility.IsoOidNamespace, String.Concat(server.ServerId, "_", missionDateTimePrefix)).ToString(); 
            MissionStartDateTime = Util.ParseDate(missionDateTimePrefix).ToUniversalTime();

            
        }
        public void ReadMissionHistory(string firstMissionLogFile = null)
        {
            //missionReport(2015-02-25_11-43-53)[0].txt

            if( firstMissionLogFile == null )
            {
                firstMissionLogFile = Util.GetNewestFilePath(MissionLogFolder, "missionReport(*)[0].txt");
                if (String.IsNullOrWhiteSpace(firstMissionLogFile))
                {
                    Log.WriteError("Mission log not found in {0}", MissionLogFolder);
                    return;
                }


            }
            else
            {
                firstMissionLogFile = this.With( x => Re.GetSubString(firstMissionLogFile, @"(missionReport\([\d+|\-|_]+\))\[\d+\].txt"))
                    .With(x => String.Concat(x,"[0].txt"));
            }


            if (String.IsNullOrWhiteSpace(firstMissionLogFile))
            {
                Log.WriteError("Malformed log filename {0}", firstMissionLogFile);
                return;
            }

            Log.WriteInfo("Reading events history from {0}", firstMissionLogFile);


            StartNewMission(firstMissionLogFile);

            if (MissionStartDateTime.Equals(default(DateTime)))
                return;


            var missionFiles = Util.GetFilesSortedByTime(MissionLogFolder, String.Format("missionReport({0})[*].txt", missionDateTimePrefix), true);
            
            var readException = Util.Try(() => {
                foreach (var file in missionFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if( fileInfo.Length > 0 && !String.IsNullOrWhiteSpace(file))
                    {
                        tracker.AddFileOffset(file, fileInfo.Length);
                        var lines = File.ReadAllLines(file);
                        if (lines != null)
                        {
                            foreach (var line in lines)
                            {
                                var data = MissionLogDataBuilder.GetData(line, MissionStartDateTime, GetCurrentEventNumber(), server);
                                if( data is MissionLogEventHeader )
                                {
                                    var header = data as MissionLogEventHeader;
                                    header.MissionFile = Path.GetFileName(file);
                                    AddHistory(data);
                                }
                            }
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
                lastEventTime = header.EventTime;
                lastTick = header.Ticks;

                Action<MissionLogEventHeader> action;
                if( historyHandlers.TryGetValue( header.Type, out action ) )
                {
                    action(header);
                }
            }
            if( actionManager != null )
                actionManager.ProcessHistory(data);
        }
        private void ClearHistory()
        {
            lock( lockHistory )
                missionHistory.Clear();
        }
        public void Start()
        {
            if (String.IsNullOrWhiteSpace(MissionLogFolder))
            {
                Log.WriteInfo("No mission folder specified!");
                return;
            }

            if (!Directory.Exists(MissionLogFolder))
            {
                Log.WriteInfo("Specified mission folder doesn't exist {0}", MissionLogFolder);
                return;
            }

            ClearHistory();

            if (Application.Current != null)
            {
                Log.WriteInfo("Initialize action manager for {0}", server.Name);
                actionManager = (Application.Current as App).ActionManager;
                actionManager.ProcessServerLogStart(server);
            }
            else
            {
                var scriptManager = new ScriptManager();
                scriptManager.LoadScripts();
                scriptManager.Start();
                actionManager = new ActionManager(new ScriptManager());
            }
            
            ReadMissionHistory();
            
            if( tracker != null )
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
