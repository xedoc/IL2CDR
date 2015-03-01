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
        MissionStart = 0,               //LET_MISSION_START 
        Hit = 1,                        //LET_HIT 
        Damage = 2,                     //LET_DAMAGE 
        Kill = 3,                       //LET_KILL 
        PlayerAmmo = 4,                 //LET_PLAYER_AMMO 
        TakeOff = 5,                    //LET_TAKEOFF 
        Landing = 6,                    //LET_LANDING 
        MissionEnd = 7,                 //LET_MISSION_END 
        ObjectiveCompleted = 8,         //LET_MISSION_OBJECT 
        AirfieldInfo = 9,               //LET_AIRFIELD 
        PlaneSpawn = 10,                //LET_CREATEPLANE 
        GroupInit = 11,                 //LET_GROUPINIT 
        GameObjectSpawn = 12,           //LET_GAMEOBJECTINVOLVED 
        InfluenceAreaInfo = 13,         //LET_INFLUENCEAREA_HEADER 
        InfluenceAreaBoundary = 14,     //LET_INFLUENCEAREA_BOUNDARY
        Version = 15,                   //LET_LOG_VERSION 
        Disconnect = 16,                //LET_DISCONNECT 
        Position = 17,                  //LET_POSITION
        Join = 20,                      //TODO: check meaning of AType:20
        Leave = 21,                     //TODO: check meaning of AType:21
    }                                   

    public class MissionLogDataBuilder
    {
        private static Dictionary<EventType, Func<MissionLogEventHeader, object>> dataFactory = new Dictionary<EventType, Func<MissionLogEventHeader, object>>() {
            { EventType.MissionStart, (header) => {return new MissionLogEventStart(header);}},
            { EventType.Hit, (header) => {return new MissionLogEventHit(header);}},
            { EventType.Damage, (header) => {return new MissionLogEventDamage(header);}},
            { EventType.Kill, (header) => {return new MissionLogEventKill(header);}},
            { EventType.PlayerAmmo, (header) => {return new MissionLogEventPlayerAmmo(header);}},
            { EventType.TakeOff, (header) => {return new MissionLogEventTakeOff(header);}},
            { EventType.Landing, (header) => {return new MissionLogEventLanding(header);}},
            { EventType.MissionEnd, (header) => {return new MissionLogEventMissionEnd(header);}},
            { EventType.ObjectiveCompleted, (header) => {return new MissionLogEventObjectiveCompleted(header);}},
            { EventType.AirfieldInfo, (header) => {return new MissionLogEventAirfieldInfo(header);}},
            { EventType.PlaneSpawn, (header) => {return new MissionLogEventPlaneSpawn(header);}},
            { EventType.GroupInit, (header) => {return new MissionLogEventGroupInitInfo(header);}},
            { EventType.GameObjectSpawn, (header) => {return new MissionLogEventGameObjectSpawn(header);}},
            { EventType.InfluenceAreaInfo, (header) => {return new MissionLogEventInfluenceAreaInfo(header);}},
            { EventType.InfluenceAreaBoundary, (header) => {return new MissionLogEventInfluenceAreaBoundary(header);}},
            { EventType.Version, (header) => {return new MissionLogEventVersion(header);}},
            { EventType.Join, (header) => {return new MissionJoin(header);}},
            { EventType.Leave, (header) => {return new MissionLeave(header);}},
        };

        public static object GetData(string text, DateTime missionStartTime)
        {
            var header = new MissionLogEventHeader(text, missionStartTime);
            if (header.Type != EventType.Unknown &&
                header.Type != EventType.Version)
            {
                if (dataFactory.ContainsKey(header.Type))
                    return dataFactory[header.Type](header);
            }
            return null;
        }
    }
    public class MissionLogEventHeader
    {
        private string logLine;
        public EventType Type { get; set; }
        public uint Ticks { get; set; } // 1/50 of second
        public long EventID { get; set; }
        public DateTime MissionStartTime { get; set; }
        public Dictionary<string, string> RawParameters { get; set; }

        public MissionLogEventHeader(MissionLogEventHeader header)
        {
            this.logLine = header.logLine;
            this.Type = header.Type;
            this.Ticks = header.Ticks;
            this.EventID = header.EventID;
            this.MissionStartTime = header.MissionStartTime;
            this.RawParameters = new Dictionary<string, string>(header.RawParameters);
        }
        public MissionLogEventHeader(string logLine, DateTime missionStartTime)
        {
            this.MissionStartTime = missionStartTime;
            this.logLine = logLine;
            Initialize(logLine);
        }
        private void Initialize(string logLine)
        {
            RawParameters = new Dictionary<string, string>();
            Type = EventType.Unknown;
            if( !String.IsNullOrWhiteSpace(logLine))
            {               
                //Parse space separated pairs of param:value and arrays like BC((x,y,z),..) 
                var matches = Regex.Matches(logLine, @"(\b\w+)[:]*(.*?(?=\s\w+:|$|\s\w+\())");

                if (matches.Count <= 0)
                    return;

                foreach( Match match in matches)
                    RawParameters.Add( match.Groups[1].Value, match.Groups[2].Value );

                int type = RawParameters.GetInt("AType");

                //TODO:Handle new ATypes here if EventType enum changed
                if((type >=0 && type <= 15) || type == 20 || type == 21)
                {
                    Type = (EventType)type;
                    Ticks = (uint)RawParameters.GetInt("T");
                }

            }
            else
            {
                RawParameters = new Dictionary<string, string>();
            }
        }

    }

    //AType:21
    //T:28250 AType:21 USERID:a11b29de-ce4d-4a19-903f-a6f84a08bdf0 USERNICKID:1b6c2a5a-bfd0-45eb-855f-fff71cd38fbc
    //TODO: handle AType:21 (I guess that is disconnect event)
    public class MissionLeave : MissionLogEventHeader
    {
        
        public Guid NickGuid { get; set; }
        public Guid LoginGuid { get; set; }

        public MissionLeave(MissionLogEventHeader header) 
            : base(header)
        {
            NickGuid = RawParameters.GetGuid("USERNICKID");
            LoginGuid = RawParameters.GetGuid("USERID");
        }
    }

    //AType:20
    //USERID:xxxxxx-... USERNICKID:xxxxxx-...
    //Identify AType:20
    public class MissionJoin : MissionLogEventHeader
    {
        
        public Guid NickGuid { get; set; } 
        public Guid LoginGuid { get; set; }

        public MissionJoin(MissionLogEventHeader header)
            : base(header)
        {

            NickGuid = RawParameters.GetGuid("USERNICKID");
            LoginGuid = RawParameters.GetGuid("USERID");
        }
    }
    //AType:16
    //T:28250 AType:16 BOTID:182273 POS(113655.180,129.202,243216.594)
    //TODO:Identify AType:16
    public class MissionLogBotSpawn : MissionLogEventHeader
    {
        
        public int BotId { get; set; }
        public Vector3D Position {get;set;}

        public MissionLogBotSpawn(MissionLogEventHeader header) 
            : base(header)
        {
            BotId = RawParameters.GetInt("BOTID");
            Position = Util.POSToVector3D("POS");
        }
    }

    //AType:15
    //T:0 AType:15 VER:17
    //Version info
    public class MissionLogEventVersion : MissionLogEventHeader
    {        
        public string Version { get; set; }
        public MissionLogEventVersion(MissionLogEventHeader header)
            : base(header)
        {
            Version = RawParameters.GetString("VER");
        }
    }
    //AType:14
    //Area boundary
    //T:1 AType:14 AID:16384 BP((243.0,98.8,183.0),(230365.0,98.8,133.0),(230106.0,98.8,75641.0),(190900.0,98.8,73379.0),(163347.0,98.8,151570.0),(181083.0,98.8,166110.0),(183280.0,98.8,188216.0),(157123.0,98.8,221512.0),(149370.0,98.8,259567.0),(131925.0,98.8,253301.0),(121149.0,98.8,241594.0),(117297.0,98.8,226568.0),(124000.0,98.8,209280.0),(110644.0,98.8,197291.0),(83500.0,98.8,211773.0),(54934.0,98.8,215035.0),(29687.0,98.8,227373.0),(820.0,98.8,231540.0))
    //T:1 AType:14 AID:18432 BP((150876.0,0.9,262474.0),(159244.0,0.9,222558.0),(185432.0,0.9,188837.0),(183398.0,0.9,164959.0),(165913.0,0.9,150673.0),(192221.0,0.9,75572.0),(230054.0,0.9,77365.0),(230179.0,0.9,358210.0),(161.0,0.9,358111.0),(755.0,0.9,233576.0),(30336.0,0.9,229602.0),(55631.0,0.9,217230.0),(84031.0,0.9,214012.0),(110293.0,0.9,200406.0),(121516.0,0.9,210199.0),(115778.0,0.9,226024.0),(119800.0,0.9,242286.0),(131353.0,0.9,254862.0))
    public class MissionLogEventInfluenceAreaBoundary : MissionLogEventHeader
    {
        public Vector3DCollection BoundaryPoints { get; set; }
        public int AreaId { get; set; }
        public MissionLogEventInfluenceAreaBoundary(MissionLogEventHeader header)
            : base(header)
        {
            AreaId = RawParameters.GetInt("AID");
            BoundaryPoints = Util.BoundaryPointsToVectorCollection("BP");
        }
    }
    //AType:13
    //Area info
    //T:0 AType:13 AID:16384 COUNTRY:201 ENABLED:1 BC(0,0,0)
    //T:0 AType:13 AID:18432 COUNTRY:101 ENABLED:1 BC(0,0,0)
    public class MissionLogEventInfluenceAreaInfo : MissionLogEventHeader
    {
        
        public int AirFieldId { get; set; }
        public Country Country { get; set; }
        public bool IsEnabled { get; set; }
        public List<CoalitionPlanesCount> PlanesByCoalition { get; set; }

        public MissionLogEventInfluenceAreaInfo(MissionLogEventHeader header)
            : base(header)
        {
            AirFieldId = RawParameters.GetInt("AID");
            IsEnabled = RawParameters.GetInt("ENABLED") == 1 ? true : false;

            Country = new Country()
            {
                Id = RawParameters.GetInt("COUNTRY"),
                Name = String.Empty,
                //TODO: Identify countries by ID 
            };
            PlanesByCoalition = new List<CoalitionPlanesCount>();
            var planesNumber = Util.SequenceToIntArray(RawParameters.GetString("BC"));
            //TODO:Check numbers meaning - where is DE/RU and neutral?
        }
    }
    //AType:12
    //Game object spawned
    //T:16459 AType:12 ID:886784 TYPE:ZiS-6 BM-13 COUNTRY:101 NAME:Vehicle PID:-1
    //T:16459 AType:12 ID:630784 TYPE:Sd Kfz 10 Flak 38 COUNTRY:201 NAME:Vehicle PID:-1
    public class MissionLogEventGameObjectSpawn : MissionLogEventHeader
    {
        
        public int ObjectId { get; set; }
        public string VehicleType { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }
        public int PlayerId { get; set; }

        public MissionLogEventGameObjectSpawn(MissionLogEventHeader header)
            : base(header)
        {
            ObjectId = RawParameters.GetInt("ID");
            VehicleType = RawParameters.GetString("TYPE");
            Name = RawParameters.GetString("NAME");
            Country = new Country() { 
                Id = RawParameters.GetInt("COUNTRY"),
                Name = String.Empty,
                //TODO: Identify country by id
            };
        }
    }
    //AType:11
    //Planes group information
    //T:1 AType:11 GID:926720 IDS:532480,538624,547840,557056,563200,569344,575488 LID:532480
    //T:1 AType:11 GID:927744 IDS:640000,646144,655360,664576,670720,676864,683008 LID:640000
    public class MissionLogEventGroupInitInfo : MissionLogEventHeader
    {
        
        public int GroupId { get; set; }
        public int[] ObjectIds { get; set; }
        public int LeaderId { get; set; }

        public MissionLogEventGroupInitInfo(MissionLogEventHeader header)
            : base(header)
        {
            GroupId = RawParameters.GetInt("GID");
            ObjectIds = Util.SequenceToIntArray(RawParameters.GetString("IDS"));
            LeaderId = RawParameters.GetInt("LID");
        }
    }
    //AType:10
    //Player plane spawn
    //T:13129 AType:10 PLID:402433 PID:182273 BUL:1620 SH:0 BOMB:0 RCT:0 (113655.359,129.266,243216.766) IDS:1b6c2a5a-bfd0-45eb-855f-fff71cd38fbc 
    //LOGIN:a11b29de-ce4d-4a19-903f-a6f84a08bdf0 NAME:xedoc TYPE:Yak-1 ser.69 COUNTRY:101 FORM:0 FIELD:308224 INAIR:1 PARENT:-1 
    //PAYLOAD:0 FUEL:1.000 SKIN: WM:1

    public class MissionLogEventPlaneSpawn : MissionLogEventHeader
    {
        
        public int PlayerId { get; set; }
        public int PlaneId { get; set; } //TODO: check if property is what I think
        //TODO: identify SH parameter
        public int Bombs { get; set; }
        //TODO: what is RCT ? Rectangle or rockets ?
        //RCT:0 (113655.359,129.266,243216.766)
        //public int Rockets { get; set; }
        public Guid NickGuid { get; set; }
        public Guid LoginGuid { get; set; }
        public string PlayerName { get; set; }
        public string VehicleType { get; set; }
        public Country Country { get; set; }
        public int Form { get; set; } //TODO: is it formation ?
        public int AirFieldId { get; set; }
        public bool IsInAir { get; set; }
        public int ParentId { get; set; }
        public int Payload { get; set; }
        public double Fuel { get; set; }
        //TODO: find type of Skin
        //public string Skin { get; set; }
        //TODO: Identify WM

        public MissionLogEventPlaneSpawn(MissionLogEventHeader header)
            : base(header)
        {
            //TODO:Parse key/value pairs
        }
    }
    //AType:9
    //Airfield info
    //T:10 AType:9 AID:96256 COUNTRY:201 POS(133798.813, 82.420, 185350.141) IDS()
    //T:10 AType:9 AID:98304 COUNTRY:101 POS(112253.711, 25.323, 260996.453) IDS()    
    public class MissionLogEventAirfieldInfo : MissionLogEventHeader
    {
        
        public int AirFieldId { get; set; }
        public Vector3D Position { get; set; }
        //TODO: what is ids ? Players ?

        public MissionLogEventAirfieldInfo(MissionLogEventHeader header)
            : base(header)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:8
    //Mission objective completed
    //TODO: RoF example - find BoS one
    //T:37907 AType:8 OBJID:39 POS(273490.000,32.018,95596.297) COAL:1 TYPE:0 RES:1
    //T:37907 AType:8 OBJID:40 POS(273513.000,32.018,95676.203) COAL:2 TYPE:0 RES:0
    //COAL - coalition number, usually 1 - Allies, 2 - Central Powers
    //TYPE - (mission objective type - primary/secondary)
    //RES - (mission result, completed/not completed)
    public class MissionLogEventObjectiveCompleted : MissionLogEventHeader
    {

        public MissionLogEventObjectiveCompleted(MissionLogEventHeader header)
            : base(header)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:7
    //End of mission
    //TODO: RoF example - find BoS one
    //T:38919 AType:7
    public class MissionLogEventMissionEnd : MissionLogEventHeader
    {

        public MissionLogEventMissionEnd(MissionLogEventHeader header) 
            : base(header)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:6
    //TODO: RoF example - find BoS one
    //T:24847 AType:6 PID:45073 POS(270413.250, 12.709, 94380.633)
    //Landing
    public class MissionLogEventLanding : MissionLogEventHeader
    {

        public MissionLogEventLanding(MissionLogEventHeader header)
            : base(header)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:5
    //TODO: RoF example - find BoS one
    //T:17512 AType:5 PID:45071 POS(272113.000, 33.734, 92909.977)
    //Takeoff 
    public class MissionLogEventTakeOff : MissionLogEventHeader
    {

        public MissionLogEventTakeOff(MissionLogEventHeader header)
            : base(header)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:4
    //TODO: RoF example - find BoS one
    //T:38910 AType:4 PLID:45067 PID:46091 BUL:271 SH:0 BOMB:0 RCT:0 (271304.063,111.273,95750.898)
    //Mission End
    //PLID - player plane id
    //PID - player (bot or player) plane id
    //BUL, BOMB - bullets and bpmbs left on the end of mission
    public class MissionLogEventPlayerAmmo : MissionLogEventHeader
    {

        public MissionLogEventPlayerAmmo(MissionLogEventHeader header)
            : base(header)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:3
    //T:16459 AType:3 AID:886784 TID:630784 POS(123722.586,132.251,239770.719)
    //Kill
    public class MissionLogEventKill : MissionLogEventHeader
    {
        
        public int AttackerId { get; set; }
        public int TargetId { get; set; }
        public Vector3D Position { get; set; }
        public MissionLogEventKill(MissionLogEventHeader header)
            : base(header)
        {
            AttackerId = RawParameters.GetInt("AID");
            TargetId = RawParameters.GetInt("TID");
            Position = Util.POSToVector3D(RawParameters.GetString("POS"));
        }
    }
    //AType:2
    //TODO: RoF example - find BoS one
    //T:23244 AType:2 DMG:0.008 AID:45068 TID:45071 POS(270402.344,467.873,93469.750)
    //Damage
    public class MissionLogEventDamage: MissionLogEventHeader
    {

        public MissionLogEventDamage(MissionLogEventHeader header)
            : base(header)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:1
    //TODO: RoF example - find BoS one
    //T:22188 AType:1 AMMO:BULLET_GBR_77x56R_MK7 AID:45067 TID:45073
    //Bullet hit on mission object
    public class MissionLogEventHit : MissionLogEventHeader
    {

        public MissionLogEventHit(MissionLogEventHeader header)
            : base(header)
        {
            //TODO: Parse key/value pairs
        }
    }
    //AType:0
    //Mission start, when player spawned
    //T:0 AType:0 GDate:1942.11.19 GTime:14:30:0 MFile:Multiplayer/Ranked/BoS_MP_RD_Stalingrad.msnbin 
    //MID: GType:2 CNTRS:0:0,101:1,201:2,202:2 SETTS:1111110111101001000000011 MODS:0 PRESET:1 AQMID:0
    public class MissionLogEventStart : MissionLogEventHeader
    {
        
        public DateTime GameDateTime { get; set; }
        public string MissionFile { get; set; }
        public int MissionID { get; set; }
        public int GameType { get; set; }
        public Dictionary<int, int> CountryCounters { get; set; }
        public bool[] SettingsFlags { get; set; }
        public int Mods { get; set; }
        public int Preset { get; set; }
        public int AQMId { get; set; }
        public MissionLogEventStart(MissionLogEventHeader header)
            : base(header)
        {
            //TODO: handle DateTime type
            MissionFile = RawParameters.GetString("MFile");
            //TODO: find example of MID
            //MissionID = RawParameters.GetString("MID")
            GameType = RawParameters.GetInt("GType");
            //TODO: handle dictionary values
            //TODO: handle bool array
            Mods = RawParameters.GetInt("MODS");
            Preset = RawParameters.GetInt("Preset");
            AQMId = RawParameters.GetInt("AWMID");
        }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Player
    {
        public Guid[] NickIds { get; set; }
        public int LoginId { get; set; }
        public string Name { get; set; }
    }

    public class CoalitionPlanesCount
    {
        public Country Country { get; set; }
        public int Count { get; set; }
    }
}
