using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public enum EventType
    {
        Unknown = -1,
        MissionStart = 0,
        Hit = 1,
        Damage = 2,
        Kill = 3,
        PlayerMissionEnd = 4,
        TakeOff = 5,
        Landing = 6,
        MissionEnd = 7,
        ObjectiveCompleted = 8,
        AirfieldInfo = 9,
        PlayerPlaneSpawn = 10,
        GroupInitInfo = 11,
        GameObjectSpawn = 12,
        InfluenceAreaInfo = 13,
        InfluenceAreaBoundary = 14,
        Version = 15,
    }
    public class MissionLogEventBase
    {
        private string logLine;
        public EventType Type { get; set; }
        public uint Ticks { get; set; } // 1/50 of second
        public Dictionary<string, string> RawParameters { get; set; }

        public MissionLogEventBase()
        {
            Initialize(String.Empty);
        }
        public MissionLogEventBase(string logLine)
        {
            this.logLine = logLine;
            Initialize(logLine);
        }
        private void Initialize( string logLine )
        {
            Type = EventType.Unknown;
            if( !String.IsNullOrWhiteSpace(logLine))
            {               
                //Parse pairs of parameter:value separated with space
                RawParameters = new Dictionary<string, string>( 
                    logLine.Split(' ')
                    .Select(pair => pair.Split(':'))
                    .Where(pair => pair.Length == 2 && !String.IsNullOrEmpty(pair[0].Trim()))
                    .ToDictionary( pair => pair[0], pair => pair[1] ));
            }
            else
            {
                RawParameters = new Dictionary<string, string>();
            }
        }

    }
    //AType:15
    public class MissionLogEventVersion : MissionLogEventBase
    {
        public MissionLogEventVersion(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:14
    public class MissionLogEventInfluenceAreaBoundary : MissionLogEventBase
    {
        public MissionLogEventInfluenceAreaBoundary(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:13
    public class MissionLogEventInfluenceAreaInfo : MissionLogEventBase
    {
        public MissionLogEventInfluenceAreaInfo(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:12
    public class MissionLogEventGameObjectSpawn : MissionLogEventBase
    {
        public MissionLogEventGameObjectSpawn(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:11
    public class MissionLogEventGroupInitInfo : MissionLogEventBase
    {
        public MissionLogEventGroupInitInfo(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:10
    public class MissionLogEventPlayerPlaneSpawn : MissionLogEventBase
    {
        public MissionLogEventPlayerPlaneSpawn(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:9
    public class MissionLogEventAirfieldInfo : MissionLogEventBase
    {
        public MissionLogEventAirfieldInfo(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:8
    public class MissionLogEventObjectiveCompleted : MissionLogEventBase
    {
        public MissionLogEventObjectiveCompleted(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:7
    public class MissionLogEventMissionEnd : MissionLogEventBase
    {
        public MissionLogEventMissionEnd(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:6
    public class MissionLogEventLanding : MissionLogEventBase
    {
        public MissionLogEventLanding(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:5
    public class MissionLogEventTakeOff : MissionLogEventBase
    {
        public MissionLogEventTakeOff(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:4
    public class MissionLogEventPlayerMissionEnd : MissionLogEventBase
    {
        public MissionLogEventPlayerMissionEnd(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:3
    public class MissionLogEventKill : MissionLogEventBase
    {
        public MissionLogEventKill(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:2
    public class MissionLogEventDamage: MissionLogEventBase
    {
        public MissionLogEventDamage(string logLine) : base (logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:1
    public class MissionLogEventHit : MissionLogEventBase
    {
        public MissionLogEventHit(string logLine) : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:0
    public class MissionLogEventStart : MissionLogEventBase
    {
        public DateTime GameDateTime { get; set; }
        public string MissionFile { get; set; }
        public int MissionID { get; set; }
        public int GameType { get; set; }
        public Dictionary<int, int> Counters { get; set; }
        public bool[] SettingsFlags { get; set; }
        public int Mods { get; set; }
        public int Preset { get; set; }
        public int AQMId { get; set; }
        public MissionLogEventStart(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }


}
