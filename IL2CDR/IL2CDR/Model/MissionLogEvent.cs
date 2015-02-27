using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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
        UserId = 20,
    }
    public class MissionLogDataBuilder
    {
        private static Dictionary<EventType, Func<string,object>> dataFactory = new Dictionary<EventType,Func<string,object>>() {
            { EventType.MissionStart, (text) => {return new MissionLogEventStart(text);}},
            { EventType.Hit, (text) => {return new MissionLogEventHit(text);}},
            { EventType.Damage, (text) => {return new MissionLogEventDamage(text);}},
            { EventType.Kill, (text) => {return new MissionLogEventKill(text);}},
            { EventType.PlayerMissionEnd, (text) => {return new MissionLogEventPlayerMissionEnd(text);}},
            { EventType.TakeOff, (text) => {return new MissionLogEventTakeOff(text);}},
            { EventType.Landing, (text) => {return new MissionLogEventLanding(text);}},
            { EventType.MissionEnd, (text) => {return new MissionLogEventMissionEnd(text);}},
            { EventType.ObjectiveCompleted, (text) => {return new MissionLogEventObjectiveCompleted(text);}},
            { EventType.AirfieldInfo, (text) => {return new MissionLogEventAirfieldInfo(text);}},
            { EventType.PlayerPlaneSpawn, (text) => {return new MissionLogEventPlayerPlaneSpawn(text);}},
            { EventType.GroupInitInfo, (text) => {return new MissionLogEventGroupInitInfo(text);}},
            { EventType.GameObjectSpawn, (text) => {return new MissionLogEventGameObjectSpawn(text);}},
            { EventType.InfluenceAreaInfo, (text) => {return new MissionLogEventInfluenceAreaInfo(text);}},
            { EventType.InfluenceAreaBoundary, (text) => {return new MissionLogEventInfluenceAreaBoundary(text);}},
            { EventType.Version, (text) => {return new MissionLogEventVersion(text);}},
            { EventType.UserId, (text) => {return new MissionLogUserId(text);}},
        };

        public static object GetData(string text)
        {
            var header = new MissionLogEventBase(text);
            if (header.Type != EventType.Unknown &&
                header.Type != EventType.Version)
            {
                if (dataFactory.ContainsKey(header.Type))
                    return dataFactory[header.Type](text);
            }
            return null;
        }
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

    //T:28250 AType:21 USERID:a11b29de-ce4d-4a19-903f-a6f84a08bdf0 USERNICKID:1b6c2a5a-bfd0-45eb-855f-fff71cd38fbc


    //AType:20
    //USERID:xxxxxx-... USERNICKID:xxxxxx-...
    public class MissionLogUserId : MissionLogEventBase
    {
        public Guid NickGuid { get; set; } 
        public Guid LoginGuid { get; set; }

        public MissionLogUserId(string logLine)
            : base(logLine)
        {

            NickGuid = RawParameters.GetGuid("USERNICKID");
            LoginGuid = RawParameters.GetGuid("USERID");
        }
    }
    //Atype:16
    //T:28250 AType:16 BOTID:182273 POS(113655.180,129.202,243216.594)
    public class MissionLogBotSpawn : MissionLogEventBase
    {
        public int BotId { get; set; }
        public Vector3D Position {get;set;}

        public MissionLogBotSpawn(string logLine)
            : base(logLine)
        {
            string[] idLoc = RawParameters.GetString("BOTID").With( x => x.Split(' '));
            if( idLoc.Length != 2 )
                return;

            int id;
            int.TryParse(idLoc[0], out id);

            Position = Util.POSToVector3D(idLoc[1]);
        }
    }

    //AType:15
    //T:0 AType:15 VER:17
    public class MissionLogEventVersion : MissionLogEventBase
    {
        public string Version { get; set; }
        public MissionLogEventVersion(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:14
    //T:1 AType:14 AID:16384 BP((243.0,98.8,183.0),(230365.0,98.8,133.0),(230106.0,98.8,75641.0),(190900.0,98.8,73379.0),(163347.0,98.8,151570.0),(181083.0,98.8,166110.0),(183280.0,98.8,188216.0),(157123.0,98.8,221512.0),(149370.0,98.8,259567.0),(131925.0,98.8,253301.0),(121149.0,98.8,241594.0),(117297.0,98.8,226568.0),(124000.0,98.8,209280.0),(110644.0,98.8,197291.0),(83500.0,98.8,211773.0),(54934.0,98.8,215035.0),(29687.0,98.8,227373.0),(820.0,98.8,231540.0))
    //T:1 AType:14 AID:18432 BP((150876.0,0.9,262474.0),(159244.0,0.9,222558.0),(185432.0,0.9,188837.0),(183398.0,0.9,164959.0),(165913.0,0.9,150673.0),(192221.0,0.9,75572.0),(230054.0,0.9,77365.0),(230179.0,0.9,358210.0),(161.0,0.9,358111.0),(755.0,0.9,233576.0),(30336.0,0.9,229602.0),(55631.0,0.9,217230.0),(84031.0,0.9,214012.0),(110293.0,0.9,200406.0),(121516.0,0.9,210199.0),(115778.0,0.9,226024.0),(119800.0,0.9,242286.0),(131353.0,0.9,254862.0))
    public class MissionLogEventInfluenceAreaBoundary : MissionLogEventBase
    {
        public MissionLogEventInfluenceAreaBoundary(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:13
    //T:0 AType:13 AID:16384 COUNTRY:201 ENABLED:1 BC(0,0,0)
    //T:0 AType:13 AID:18432 COUNTRY:101 ENABLED:1 BC(0,0,0)
    public class MissionLogEventInfluenceAreaInfo : MissionLogEventBase
    {
        public MissionLogEventInfluenceAreaInfo(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:12
    //T:16459 AType:12 ID:886784 TYPE:ZiS-6 BM-13 COUNTRY:101 NAME:Vehicle PID:-1
    //T:16459 AType:12 ID:630784 TYPE:Sd Kfz 10 Flak 38 COUNTRY:201 NAME:Vehicle PID:-1
    public class MissionLogEventGameObjectSpawn : MissionLogEventBase
    {
        public MissionLogEventGameObjectSpawn(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:11
    //T:1 AType:11 GID:926720 IDS:532480,538624,547840,557056,563200,569344,575488 LID:532480
    //T:1 AType:11 GID:927744 IDS:640000,646144,655360,664576,670720,676864,683008 LID:640000
    public class MissionLogEventGroupInitInfo : MissionLogEventBase
    {
        public MissionLogEventGroupInitInfo(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:10
    //T:13129 AType:10 PLID:402433 PID:182273 BUL:1620 SH:0 BOMB:0 RCT:0 (113655.359,129.266,243216.766) IDS:1b6c2a5a-bfd0-45eb-855f-fff71cd38fbc LOGIN:a11b29de-ce4d-4a19-903f-a6f84a08bdf0 NAME:xedoc TYPE:Yak-1 ser.69 COUNTRY:101 FORM:0 FIELD:308224 INAIR:1 PARENT:-1 PAYLOAD:0 FUEL:1.000 SKIN: WM:1

    public class MissionLogEventPlayerPlaneSpawn : MissionLogEventBase
    {
        public MissionLogEventPlayerPlaneSpawn(string logLine)
            : base(logLine)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:9
    //T:10 AType:9 AID:96256 COUNTRY:201 POS(133798.813, 82.420, 185350.141) IDS()
    //T:10 AType:9 AID:98304 COUNTRY:101 POS(112253.711, 25.323, 260996.453) IDS()
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
    //T:16459 AType:3 AID:886784 TID:630784 POS(123722.586,132.251,239770.719)
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
    //T:0 AType:0 GDate:1942.11.19 GTime:14:30:0 MFile:Multiplayer/Ranked/BoS_MP_RD_Stalingrad.msnbin 
    //MID: GType:2 CNTRS:0:0,101:1,201:2,202:2 SETTS:1111110111101001000000011 MODS:0 PRESET:1 AQMID:0
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
