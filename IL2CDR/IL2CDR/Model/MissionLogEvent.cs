using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;

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
        BotPilotSpawn = 16,             //TODO: check if that parachute spawn
        Position = 17,                  //LET_POSITION
        Join = 20,                      
        Leave = 21,
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
            { EventType.BotPilotSpawn, (header) => {return new MissionLogBotSpawn(header);}},
            { EventType.Join, (header) => {return new MissionLogEventPlayerJoin(header);}},
            { EventType.Leave, (header) => {return new MissionLogEventPlayerLeave(header);}},
        };
        public static object GetData(string text, DateTime missionStartTime, int eventNumber, Server server)
        {
            var header = new MissionLogEventHeader(text, missionStartTime);
            header.Server = server;
            if (header.Type != EventType.Unknown )
            {
                header.EventID = GuidUtility.Create(GuidUtility.IsoOidNamespace, String.Concat(server.ServerId,"_",missionStartTime,"_",eventNumber));

                Func<MissionLogEventHeader, object> handler;
                if (dataFactory.TryGetValue(header.Type, out handler))
                    return handler(header);
            }
            return null;
        }
    }
    public class MissionLogEventHeader
    {
        private string logLine;

        public EventType Type { get; set; }
        public uint Ticks { get; set; } // 1/50 of second
        public Server Server { get; set; }
        public Guid EventID { get; set; }
        public DateTime EventTime { get; set; }
        public DateTime MissionStartTime { get; set; }
        public string MissionFile { get; set; }
        
        [JsonIgnore]
        public Dictionary<string, string> RawParameters { get; set; }

        public MissionLogEventHeader(MissionLogEventHeader header)
        {
            this.Server = header.Server;
            this.logLine = header.logLine;
            this.Type = header.Type;
            this.Ticks = header.Ticks;
            this.EventID = header.EventID;
            this.MissionStartTime = header.MissionStartTime;
            this.RawParameters = new Dictionary<string, string>(header.RawParameters);
            this.EventTime = header.MissionStartTime.AddMilliseconds( (double)Ticks / 50.0 * 1000.0);
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
                if((type >=0 && type <= 17) || 
                    type == 20 || 
                    type == 21)
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
    //T:28250 AType:21 USERID:00000000-0000-0000-0000-000000000000 USERNICKID:00000000-0000-0000-0000-000000000000
    public class MissionLogEventPlayerLeave : MissionLogEventHeader
    {
        public Guid NickId { get; set; }
        public Guid LoginId { get; set; }

        public MissionLogEventPlayerLeave(MissionLogEventHeader header) 
            : base(header)
        {
            Guid nickId, loginId;
            Guid.TryParse(RawParameters["USERNICKID"], out nickId);
            Guid.TryParse(RawParameters["USERID"], out loginId);
        }
    }

    //AType:20
    //USERID:xxxxxx-... USERNICKID:xxxxxx-...
    //Identify AType:20
    public class MissionLogEventPlayerJoin : MissionLogEventHeader
    {        
        public Guid NickGuid { get; set; } 
        public Guid LoginGuid { get; set; }

        public MissionLogEventPlayerJoin(MissionLogEventHeader header)
            : base(header)
        {

            NickGuid = RawParameters.GetGuid("USERNICKID");
            LoginGuid = RawParameters.GetGuid("USERID");
        }
    }

    //TODO: Handle Atype:19
    //T:180021 AType:19 

    //TODO: Handle AType:18
    //T:1982216 AType:18 BOTID:725017 PARENTID:723993 POS(112101.023,1238.855,99265.477)

    //AType:16
    //T:28250 AType:16 BOTID:182273 POS(113655.180,129.202,243216.594)
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
    //T:92117 AType:13 AID:16384 COUNTRY:201 ENABLED:1 BC(0,1,0)

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
            Country = new Country(RawParameters.GetInt("COUNTRY"));
            PlanesByCoalition = new List<CoalitionPlanesCount>();
            var planesNumber = Util.SequenceToIntArray(RawParameters.GetString("BC"));
        }
    }
    //AType:12
    //Game object spawned
    //T:16459 AType:12 ID:886784 TYPE:ZiS-6 BM-13 COUNTRY:101 NAME:Vehicle PID:-1
    //T:16459 AType:12 ID:630784 TYPE:Sd Kfz 10 Flak 38 COUNTRY:201 NAME:Vehicle PID:-1
    public class MissionLogEventGameObjectSpawn : MissionLogEventHeader
    {
        public GameObject Object { get; set; }
        public int PlayerId { get; set; }

        public MissionLogEventGameObjectSpawn(MissionLogEventHeader header)
            : base(header)
        {
            GameObjectItem purpose;
            GameInfo.ObjectsClassification.TryGetValue(RawParameters.GetString("TYPE"), out purpose);

            if (purpose == null)
                purpose = new GameObjectItem(GameObjectClass.Other, "Unknown");

            Object = new GameObject(RawParameters.GetInt("ID"), RawParameters.GetString("TYPE")) { 
                Classification = purpose.Classification.ToString("g"),
                Purpose = purpose.Purpose,
                Country = new Country(RawParameters.GetInt("COUNTRY")),
            };

            Object.CoalitionIndex = Server.GetCoalitionIndex(Object.Country);
            PlayerId = RawParameters.GetInt("PID");
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

    public enum WeaponMods
    {
        Default,
        Mod1,
        Mod2,
        Mod3,
        Mod4,
        Mod5,
        Mod6,
        Mod7
    }
    //AType:10
    //Player plane spawn
    //T:13129 AType:10 PLID:402433 PID:182273 BUL:1620 SH:0 BOMB:0 RCT:0 (113655.359,129.266,243216.766) IDS:00000000-0000-0000-0000-000000000000 
    //LOGIN:00000000-0000-0000-0000-000000000000 NAME:blahblah TYPE:Yak-1 ser.69 COUNTRY:101 FORM:0 FIELD:308224 INAIR:1 PARENT:-1 
    //PAYLOAD:0 FUEL:1.000 SKIN: WM:1
    //Another example with InAir=2
    //T:8120 AType:10 PLID:675841 PID:676865 BUL:1620 SH:0 BOMB:0 RCT:0 (133952.922,83.792,185683.047) IDS:00000000-0000-0000-0000-000000000000 LOGIN:00000000-0000-0000-0000-000000000000 NAME:NIK TYPE:Yak-1 ser.69 COUNTRY:101 FORM:0 FIELD:861184 INAIR:2 PARENT:-1 PAYLOAD:0 FUEL:0.380 SKIN: WM:1

    public class MissionLogEventPlaneSpawn : MissionLogEventHeader
    {        
        public Player Player { get; set; }
        public int Formation { get; set; }
        public MissionLogEventPlaneSpawn(MissionLogEventHeader header)
            : base(header)
        {
            GameObjectItem purpose;            
            GameInfo.ObjectsClassification.TryGetValue(RawParameters.GetString("TYPE"), out purpose);
            if (purpose == null)
                purpose = new GameObjectItem(GameObjectClass.Other, "Unknown");

            Player = new Player()
            {
                Id = RawParameters.GetInt("PID"),
                Country = new Country(RawParameters.GetInt("COUNTRY")),
                IsInAir = RawParameters.GetInt("INAIR") == 1 ? true : false,
                IsOnline = true,
                LoginId = RawParameters.GetGuid("LOGIN"),
                NickId = RawParameters.GetGuid("IDS"),
                NickName = RawParameters.GetString("NAME"),
                Plane = new Plane(RawParameters.GetInt("PLID"), RawParameters.GetString("TYPE"))
                {
                    Bullets = RawParameters.GetInt("BUL"),
                    Bombs = RawParameters.GetInt("BOMB"),
                    Classification = GameObjectClass.Plane.ToString("g"),
                    Fuel = RawParameters.GetDouble("FUEL"),
                    Payload = RawParameters.GetString("PAYLOAD"),
                    Purpose = purpose == null ? null : purpose.Purpose,
                    Shells = RawParameters.GetInt("SH"),
                    Skin = RawParameters.GetString("SKIN"),
                    WeaponMods = RawParameters.GetString("WM"),                    
                },
                BotPilot = new GameObject(RawParameters.GetInt("PID"), "BotPilot"),
                SortieId = EventID,
            };
            Player.CoalitionIndex = Server.GetCoalitionIndex(Player.Country);
        }
    }
    //AType:9
    //Airfield info
    //T:10 AType:9 AID:96256 COUNTRY:201 POS(133798.813, 82.420, 185350.141) IDS()
    //T:10 AType:9 AID:98304 COUNTRY:101 POS(112253.711, 25.323, 260996.453) IDS()    
    public class MissionLogEventAirfieldInfo : MissionLogEventHeader
    {
        public AirField AirField { get; set; }
        public MissionLogEventAirfieldInfo(MissionLogEventHeader header)
            : base(header)
        {
            AirField = new AirField() { 
                Country = new Country( RawParameters.GetInt("COUNTRY")),
                Id = RawParameters.GetInt("AID"),
                Position = RawParameters.GetVector3D("POS"),
            };
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
        public int ObjectiveId { get; set; }
        public Vector3D Position { get; set; }
        public bool IsPrimary { get; set; }
        public int CoalitionIndex { get; set; }
        public bool IsCompleted { get; set; }
        public MissionLogEventObjectiveCompleted(MissionLogEventHeader header)
            : base(header)
        {
            ObjectiveId = RawParameters.GetInt("OBJID");
            Position = RawParameters.GetVector3D("POS");
            IsPrimary = RawParameters.GetInt("TYPE") == 1 ? true : false;
            CoalitionIndex = RawParameters.GetInt("COAL");
            IsCompleted = RawParameters.GetInt("RES") == 1 ? true : false;
        }
    }
    //AType:7
    //End of mission
    //T:38919 AType:7
    public class MissionLogEventMissionEnd : MissionLogEventHeader
    {
        public DateTime MissionEndTime { get; set; }
        public MissionLogEventMissionEnd(MissionLogEventHeader header) 
            : base(header)
        {
            MissionEndTime = DateTime.UtcNow;
        }
    }
    //AType:6
    //T:30670 AType:6 PID:311297 POS(124200.461, 131.916, 240163.281)
    //Landing
    public class MissionLogEventLanding : MissionLogEventHeader
    {
        public int PlayerId { get; set; }
        public Vector3D Position { get; set; }
        public Player Player { get; set; }
        public GameObject Bot { get; set; }

        public MissionLogEventLanding(MissionLogEventHeader header)
            : base(header)
        {
            PlayerId = RawParameters.GetInt("PID");
            Position = RawParameters.GetVector3D("POS");
            Player = Server.Players[PlayerId];
            if (Player == null)
                Bot = Server.GameObjects[PlayerId];
        }
    }
    //AType:5
    //T:8500 AType:5 PID:311297 POS(112283.617, 46.384, 260226.063)    
    //Takeoff 
    public class MissionLogEventTakeOff : MissionLogEventHeader
    {
        public int PlayerId { get; set; }
        public Vector3D Position { get; set; }
        public Player Player { get; set; }
        public GameObject Bot { get; set; }

        public MissionLogEventTakeOff(MissionLogEventHeader header)
            : base(header)
        {
            PlayerId = RawParameters.GetInt("PID");
            Position = RawParameters.GetVector3D("POS");

            Player = Server.Players[PlayerId];
            if( Player == null )
                Bot = Server.GameObjects[PlayerId];
        }
    }
    //AType:4
    //T:31598 AType:4 PLID:311297 PID:394241 BUL:1158 SH:0 BOMB:0 RCT:0 (124204.711,131.871,240163.422)
    //Mission End
    //PLID - player plane id
    //PID - player (bot or player) plane id
    //BUL, BOMB - bullets and bpmbs left on the end of mission
    public class MissionLogEventPlayerAmmo : MissionLogEventHeader
    {
        public int PlaneId { get; set; }
        public int PlayerId { get; set; }
        public int Bullets { get; set; }
        public int Shells { get; set; }
        public int Bombs { get; set; }
        public int Rockets { get; set; }

        public Player Player { get; set; }
        public GameObject Bot { get; set; }

        public MissionLogEventPlayerAmmo(MissionLogEventHeader header)
            : base(header)
        {
            PlaneId = RawParameters.GetInt("PLID");
            PlayerId = RawParameters.GetInt("PID");
            Bullets = RawParameters.GetInt("BUL");
            Shells = RawParameters.GetInt("SH");
            Bombs = RawParameters.GetInt("BOMB");
            Rockets = RawParameters.GetInt("RCT");

            Player = Server.Players[PlaneId] ?? Server.Players[PlayerId];
            //if (Player == null)
            //    Bot = Server.GameObjects[PlaneId];
        }
    }
    //AType:3
    //T:16459 AType:3 AID:886784 TID:630784 POS(123722.586,132.251,239770.719)
    //Kill
    public class MissionLogEventKill : MissionLogEventHeader
    {
        public int AttackerId { get; set; } //Plane/vehicle ID
        public int TargetId { get; set; }   //Plane/vehicle ID
        public Vector3D Position { get; set; }
        public Player AttackerPlayer { get; set; }
        public Player TargetPlayer { get; set; }
        public GameObject AttackerObject { get; set; }
        public GameObject TargetObject { get; set; }
        public bool IsTeamKill
        {
            get
            {
                return AttackerCoalition == TargetCoalition;
            }
        }

        public int Hits
        {
            get
            {
                if (TargetObject != null && AttackerObject != null )
                    return TargetObject.GetHitsCountBy(AttackerObject);
                else
                    return 0;
            }

        }
        public double Damage
        {
            get
            {
                if (TargetObject != null && AttackerObject != null )
                    return TargetObject.GetDamageBy(AttackerObject);
                else
                    return 0;
            }

        }

        public int AttackerCoalition
        {
            get
            {
                if (AttackerPlayer != null)
                    return AttackerPlayer.CoalitionIndex;
                else if (AttackerObject != null)
                    return AttackerObject.CoalitionIndex;
                else
                    return 0;
            }
        }
        public int TargetCoalition
        {
            get
            {
                if (TargetPlayer != null)
                    return TargetPlayer.CoalitionIndex;
                else if (TargetObject != null)
                    return TargetObject.CoalitionIndex;
                else
                    return 0;
            }
        }

        public MissionLogEventKill(MissionLogEventHeader header)
            : base(header)
        {
            AttackerId = RawParameters.GetInt("AID");
            TargetId = RawParameters.GetInt("TID");
            Position = RawParameters.GetVector3D("POS");

            AttackerPlayer = Server.Players[AttackerId];
            AttackerObject = Server.GameObjects[AttackerId];   
            TargetPlayer = Server.Players[TargetId];
            TargetObject = Server.GameObjects[TargetId];     
        }
    }
    //AType:2
    //T:26263 AType:2 DMG:0.013 AID:612352 TID:311297 POS(123759.180,255.837,242953.906)
    //Damage
    public class MissionLogEventDamage: MissionLogEventHeader
    {
        public double Damage { get; set; }
        public int AttackerId { get; set; }
        public int TargetId { get; set; }
        public Vector3D Position { get; set; }
        public Player AttackerPlayer { get; set; }
        public Player TargetPlayer { get; set; }
        public GameObject AttackerObject { get; set; }
        public GameObject TargetObject { get; set; }

        public MissionLogEventDamage(MissionLogEventHeader header)
            : base(header)
        {
            AttackerId = RawParameters.GetInt("AID");
            TargetId = RawParameters.GetInt("TID");
            Damage = RawParameters.GetDouble("DMG");
            Position = RawParameters.GetVector3D("POS");

            AttackerPlayer = Server.Players[AttackerId];
            if (AttackerPlayer == null)
                AttackerObject = Server.GameObjects[AttackerId];

            TargetPlayer = Server.Players[TargetId];
            if (TargetPlayer == null)
                TargetObject = Server.GameObjects[TargetId];
        }
    }
    //AType:1
    //T:26455 AType:1 AMMO:BULLET_RUS_7-62x54_AP AID:311297 TID:454656
    //Bullet hit on mission object
    public class MissionLogEventHit : MissionLogEventHeader
    {
        public int AttackerId { get; set; }
        public int TargetId { get; set; }
        public Player AttackerPlayer { get; set; }
        public Player TargetPlayer { get; set; }
        public GameObject AttackerObject { get; set; }
        public GameObject TargetObject { get; set; }

        public string AmmoName { get; set; }
        public MissionLogEventHit(MissionLogEventHeader header)
            : base(header)
        {
            AttackerId = RawParameters.GetInt("AID");
            TargetId = RawParameters.GetInt("TID");
            AmmoName = RawParameters.GetString("AMMO");

            //AttackerPlayer = Server.Players[AttackerId];
            AttackerObject = Server.GameObjects[AttackerId];
            //TargetPlayer = Server.Players[TargetId];
            TargetObject = Server.GameObjects[TargetId];

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
        public string MissionID { get; set; }
        public int GameType { get; set; }
        public List<CoalitionIndex> CoalitionIndexes { get; set; }
        public string SettingsFlags { get; set; }
        public int Mods { get; set; }
        public int Preset { get; set; }
        public int AQMId { get; set; }
        public MissionLogEventStart(MissionLogEventHeader header)
            : base(header)
        {
            //TODO: handle DateTime type
            var gameDate = Regex.Match( RawParameters.GetString("GDate"), @"(\d+)\.(\d+)\.(\d+)");
            var gameTime = Regex.Match( RawParameters.GetString("GTime"), @"(\d+)\:(\d+)\:(\d+)");
            if( gameDate.Success && gameTime.Success)
            {
                GameDateTime = new DateTime(
                    int.Parse(gameDate.Groups[1].Value), 
                    int.Parse(gameDate.Groups[2].Value), 
                    int.Parse(gameDate.Groups[3].Value), 
                    int.Parse(gameTime.Groups[1].Value), 
                    int.Parse(gameTime.Groups[2].Value), 
                    int.Parse(gameTime.Groups[3].Value));
            }
            MissionFile = RawParameters.GetString("MFile");
            MissionID = RawParameters.GetString("MID");
            GameType = RawParameters.GetInt("GType");
            Mods = RawParameters.GetInt("MODS");
            Preset = RawParameters.GetInt("Preset");
            AQMId = RawParameters.GetInt("AWMID");
            SettingsFlags = RawParameters.GetString("SETTS");
            var coalitions = RawParameters.GetString("CNTRS");

            var countryPairs = coalitions.Split(',').Select(p => p.Split(':')).ToArray();
            CoalitionIndexes = new List<CoalitionIndex>();

            int country, index;
            for (int i = 0; i < countryPairs.Length; i++)
			{
                if (countryPairs[i].Length == 2 &&
                    int.TryParse(countryPairs[i][0], out country) &&
                    int.TryParse(countryPairs[i][1], out index))
                {
                    CoalitionIndexes.Add( new CoalitionIndex() 
                    { 
                        Country = new Country(country),
                        Index = index,
                    });
                }
            }
        }
    }

    public class CoalitionIndex
    {
        public Country Country { get; set; }
        public int Index { get; set; }
    }

    public class CoalitionPlanesCount
    {
        public Country Country { get; set; }
        public int Count { get; set; }
    }



}

