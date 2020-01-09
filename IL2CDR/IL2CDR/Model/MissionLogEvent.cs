using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;

namespace IL2CDR.Model
{
	public enum EventType
	{
		Unknown = -1,
		MissionStart = 0, //LET_MISSION_START 
		Hit = 1, //LET_HIT 
		Damage = 2, //LET_DAMAGE 
		Kill = 3, //LET_KILL 
		PlayerAmmo = 4, //LET_PLAYER_AMMO 
		TakeOff = 5, //LET_TAKEOFF 
		Landing = 6, //LET_LANDING 
		MissionEnd = 7, //LET_MISSION_END 
		ObjectiveCompleted = 8, //LET_MISSION_OBJECT 
		AirfieldInfo = 9, //LET_AIRFIELD 
		PlaneSpawn = 10, //LET_CREATEPLANE 
		GroupInit = 11, //LET_GROUPINIT 
		GameObjectSpawn = 12, //LET_GAMEOBJECTINVOLVED 
		InfluenceAreaInfo = 13, //LET_INFLUENCEAREA_HEADER 
		InfluenceAreaBoundary = 14, //LET_INFLUENCEAREA_BOUNDARY
		Version = 15, //LET_LOG_VERSION 
		BotPilotSpawn = 16, //TODO: check if that parachute spawn
		Position = 17, //LET_POSITION
		Join = 20,
		Leave = 21,
	}

	public class MissionLogDataBuilder
	{
		private static Dictionary<EventType, Func<MissionLogEventHeader, object>> dataFactory =
			new Dictionary<EventType, Func<MissionLogEventHeader, object>>() {
				{EventType.MissionStart, (header) => new MissionLogEventStart(header)},
				{EventType.Hit, (header) => new MissionLogEventHit(header)},
				{EventType.Damage, (header) => new MissionLogEventDamage(header)},
				{EventType.Kill, (header) => new MissionLogEventKill(header)},
				{EventType.PlayerAmmo, (header) => new MissionLogEventPlayerAmmo(header)},
				{EventType.TakeOff, (header) => new MissionLogEventTakeOff(header)},
				{EventType.Landing, (header) => new MissionLogEventLanding(header)},
				{EventType.MissionEnd, (header) => new MissionLogEventMissionEnd(header)},
				{EventType.ObjectiveCompleted, (header) => new MissionLogEventObjectiveCompleted(header)},
				{EventType.AirfieldInfo, (header) => new MissionLogEventAirfieldInfo(header)},
				{EventType.PlaneSpawn, (header) => new MissionLogEventPlaneSpawn(header)},
				{EventType.GroupInit, (header) => new MissionLogEventGroupInitInfo(header)},
				{EventType.GameObjectSpawn, (header) => new MissionLogEventGameObjectSpawn(header)},
				{EventType.InfluenceAreaInfo, (header) => new MissionLogEventInfluenceAreaInfo(header)},
				{ EventType.InfluenceAreaBoundary, (header) => new MissionLogEventInfluenceAreaBoundary(header)},
				{EventType.Version, (header) => new MissionLogEventVersion(header)},
				{EventType.BotPilotSpawn, (header) => new MissionLogRemoveBot(header)},
				{EventType.Join, (header) => new MissionLogEventPlayerJoin(header)},
				{EventType.Leave, (header) => new MissionLogEventPlayerLeave(header)},
			};

		public static object GetData(string text, DateTime missionStartTime, int eventNumber, Server server)
		{
			var header = new MissionLogEventHeader(text, missionStartTime);
			header.Server = server;
			if (header.Type != EventType.Unknown) {
				header.EventID = GuidUtility.Create(GuidUtility.IsoOidNamespace,
					string.Concat(server.ServerId, "_", missionStartTime, "_", eventNumber));

				if (dataFactory.TryGetValue(header.Type, out var handler)) {
					return handler(header);
				}
			}

			return null;
		}
	}

	public class MissionLogEventHeader
	{
		private readonly string logLine;

		public EventType Type { get; set; }
		public uint Ticks { get; set; } // 1/50 of second
		public Server Server { get; set; }
		public Guid EventID { get; set; }
		public DateTime EventTime { get; set; }
		public DateTime MissionStartTime { get; set; }
		public string MissionFile { get; set; }

		[JsonIgnore] public Dictionary<string, string> RawParameters { get; set; }

		public MissionLogEventHeader(MissionLogEventHeader header)
		{
			this.Server = header.Server;
			this.logLine = header.logLine;
			this.Type = header.Type;
			this.Ticks = header.Ticks;
			this.EventID = header.EventID;
			this.MissionStartTime = header.MissionStartTime;
			this.RawParameters = new Dictionary<string, string>(header.RawParameters);
			this.EventTime = header.MissionStartTime.AddMilliseconds(this.Ticks / 50.0 * 1000.0);
		}

		public MissionLogEventHeader(string logLine, DateTime missionStartTime)
		{
			this.MissionStartTime = missionStartTime;
			this.logLine = logLine;
			this.Initialize(logLine);
		}

		private void Initialize(string logLine)
		{
			this.RawParameters = new Dictionary<string, string>();
			this.Type = EventType.Unknown;
			if (!string.IsNullOrWhiteSpace(logLine)) {
				//Parse space separated pairs of param:value and arrays like BC((x,y,z),..) 
				var matches = Regex.Matches(logLine, @"(\b\w+)[:]*(.*?(?=\s\w+:|$|\s\w+\())");

				if (matches.Count <= 0) {
					return;
				}

				foreach (Match match in matches) {
					this.RawParameters.Add(match.Groups[1].Value, match.Groups[2].Value);
				}

				var type = this.RawParameters.GetInt("AType");

				//TODO:Handle new ATypes here if EventType enum changed
				if (type >= 0 && type <= 17 ||
					type == 20 ||
					type == 21) {
					this.Type = (EventType) type;
					this.Ticks = (uint) this.RawParameters.GetInt("T");
				}
			} else {
				this.RawParameters = new Dictionary<string, string>();
			}
		}
	}

	//AType:21 Player leave
	//T:28250 AType:21 USERID:00000000-0000-0000-0000-000000000000 USERNICKID:00000000-0000-0000-0000-000000000000
	public class MissionLogEventPlayerLeave : MissionLogEventHeader
	{
		public Player Player { get; set; }
		public Guid NickGuid { get; set; }
		public Guid LoginGuid { get; set; }

		public MissionLogEventPlayerLeave(MissionLogEventHeader header)
			: base(header)
		{
			this.NickGuid = this.RawParameters.GetGuid("USERNICKID");
			this.LoginGuid = this.RawParameters.GetGuid("USERID");
			this.Player = this.Server.Players.FindPlayerByGuid(this.NickGuid);
		}
	}

	//AType:20 Player join
	//USERID:xxxxxx-... USERNICKID:xxxxxx-...
	//Identify AType:20
	public class MissionLogEventPlayerJoin : MissionLogEventHeader
	{
		public Guid NickGuid { get; set; }
		public Guid LoginGuid { get; set; }

		public MissionLogEventPlayerJoin(MissionLogEventHeader header)
			: base(header)
		{
			this.NickGuid = this.RawParameters.GetGuid("USERNICKID");
			this.LoginGuid = this.RawParameters.GetGuid("USERID");
		}
	}

	//TODO: Handle Atype:19
	//T:180021 AType:19 

	//TODO: Handle AType:18
	//T:1982216 AType:18 BOTID:725017 PARENTID:723993 POS(112101.023,1238.855,99265.477)
	public class MissionLogEject : MissionLogEventHeader
	{
		public Player Player { get; set; }
		public bool IsFriendlyArea { get; set; }
		public Vector3D Position { get; set; }

		public MissionLogEject(MissionLogEventHeader header)
			: base(header)
		{
			this.Position = this.RawParameters.GetVector3D("POS");
			var area = this.Server.Areas.FindAreaByPos(this.RawParameters.GetVector3D("POS"));

			this.Player = this.Server.Players[this.RawParameters.GetInt("PARENTID")] ??
						this.Server.Players[this.RawParameters.GetInt("BOTID")];
			if (this.Player != null && area != null) {
				this.IsFriendlyArea = area.Coalition == this.Player.CoalitionIndex;
			}
		}
	}

	//AType:16 
	//T:28250 AType:16 BOTID:182273 POS(113655.180,129.202,243216.594)
	public class MissionLogRemoveBot : MissionLogEventHeader
	{
		public Player Player { get; set; }
		public GameObject Bot { get; set; }
		public int BotId { get; set; }
		public bool IsFriendlyArea { get; set; }
		public Vector3D Position { get; set; }

		public MissionLogRemoveBot(MissionLogEventHeader header)
			: base(header)
		{
			this.Position = this.RawParameters.GetVector3D("POS");
			var area = this.Server.Areas.FindAreaByPos(this.RawParameters.GetVector3D("POS"));

			this.Player = this.Server.Players[this.RawParameters.GetInt("BOTID")];

			if (area != null) {
				if (this.Player == null) {
					this.Bot = this.Server.GameObjects[this.RawParameters.GetInt("BOTID")];
					this.IsFriendlyArea = area.Coalition == this.Bot.CoalitionIndex;
				} else {
					this.IsFriendlyArea = area.Coalition == this.Player.CoalitionIndex;
				}
			}
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
			this.Version = this.RawParameters.GetString("VER");
		}
	}

	//AType:14
	//Area boundary
	//T:1 AType:14 AID:16384 BP((243.0,98.8,183.0),(230365.0,98.8,133.0),(230106.0,98.8,75641.0),(190900.0,98.8,73379.0),(163347.0,98.8,151570.0),(181083.0,98.8,166110.0),(183280.0,98.8,188216.0),(157123.0,98.8,221512.0),(149370.0,98.8,259567.0),(131925.0,98.8,253301.0),(121149.0,98.8,241594.0),(117297.0,98.8,226568.0),(124000.0,98.8,209280.0),(110644.0,98.8,197291.0),(83500.0,98.8,211773.0),(54934.0,98.8,215035.0),(29687.0,98.8,227373.0),(820.0,98.8,231540.0))
	//T:1 AType:14 AID:18432 BP((150876.0,0.9,262474.0),(159244.0,0.9,222558.0),(185432.0,0.9,188837.0),(183398.0,0.9,164959.0),(165913.0,0.9,150673.0),(192221.0,0.9,75572.0),(230054.0,0.9,77365.0),(230179.0,0.9,358210.0),(161.0,0.9,358111.0),(755.0,0.9,233576.0),(30336.0,0.9,229602.0),(55631.0,0.9,217230.0),(84031.0,0.9,214012.0),(110293.0,0.9,200406.0),(121516.0,0.9,210199.0),(115778.0,0.9,226024.0),(119800.0,0.9,242286.0),(131353.0,0.9,254862.0))
	public class MissionLogEventInfluenceAreaBoundary : MissionLogEventHeader
	{
		public Area Area { get; set; }

		public MissionLogEventInfluenceAreaBoundary(MissionLogEventHeader header)
			: base(header)
		{
			var id = this.RawParameters.GetInt("AID");
			var existingArea = this.Server.Areas[id];
			if (existingArea != null) {
				this.Server.Areas[id].SetBoundaries(this.RawParameters.GetVectorArray("BP"));
				this.Area = this.Server.Areas[id];
			} else {
				this.Area = new Area(this.RawParameters.GetVectorArray("BP")) {
					Id = this.RawParameters.GetInt("AID")
				};
			}
		}
	}
	//AType:13
	//Area info
	//T:0 AType:13 AID:16384 COUNTRY:201 ENABLED:1 BC(0,0,0)
	//T:0 AType:13 AID:18432 COUNTRY:101 ENABLED:1 BC(0,0,0)
	//T:92117 AType:13 AID:16384 COUNTRY:201 ENABLED:1 BC(0,1,0)

	public class MissionLogEventInfluenceAreaInfo : MissionLogEventHeader
	{
		public Area Area { get; set; }
		public int AreaId { get; set; }
		public Country Country { get; set; }
		public bool IsEnabled { get; set; }
		public List<CoalitionPlanesCount> PlanesByCoalition { get; set; }

		public MissionLogEventInfluenceAreaInfo(MissionLogEventHeader header)
			: base(header)
		{
			var country = new Country(this.RawParameters.GetInt("COUNTRY"));
			var coalition = this.Server.CoalitionIndexes.FirstOrDefault(c => c.Country.Id == country.Id);
			var coalitionIndex = coalition?.Index ?? 0;

			var id = this.RawParameters.GetInt("AID");
			var existingArea = this.Server.Areas[id];

			if (existingArea != null) {
				this.Server.Areas[id].Country = country;
				this.Server.Areas[id].Coalition = coalitionIndex;
				this.Server.Areas[id].IsEnabled = this.RawParameters.GetInt("ENABLED") == 1;
				this.Area = this.Server.Areas[id];
			} else {
				this.Area = new Area(this.RawParameters.GetInt("AID"), country, this.RawParameters.GetInt("ENABLED") == 1) {
								Coalition = coalitionIndex,
							};
			}

			this.PlanesByCoalition = new List<CoalitionPlanesCount>();
			var planesNumber = Util.SequenceToIntArray(this.RawParameters.GetString("BC"));
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
			GameInfo.ObjectsClassification.TryGetValue(this.RawParameters.GetString("TYPE"), out var purpose);

			if (purpose == null) {
				purpose = new GameObjectItem(GameObjectClass.Other, "Unknown");
			}

			this.Object = new GameObject(this.RawParameters.GetInt("ID"), this.RawParameters.GetString("TYPE")) {
				Classification = purpose.Classification.ToString("g"),
				Purpose = purpose.Purpose,
				Country = new Country(this.RawParameters.GetInt("COUNTRY")),
			};

			this.Object.CoalitionIndex = this.Server.GetCoalitionIndex(this.Object.Country);
			this.PlayerId = this.RawParameters.GetInt("PID");
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
			this.GroupId = this.RawParameters.GetInt("GID");
			this.ObjectIds = Util.SequenceToIntArray(this.RawParameters.GetString("IDS"));
			this.LeaderId = this.RawParameters.GetInt("LID");
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
	//inair: 0 - in air, 1 - air strip, 2 - parking
	public class MissionLogEventPlaneSpawn : MissionLogEventHeader
	{
		public Player Player { get; set; }
		public int Formation { get; set; }

		public MissionLogEventPlaneSpawn(MissionLogEventHeader header)
			: base(header)
		{
			GameObjectItem purpose;
			GameInfo.ObjectsClassification.TryGetValue(this.RawParameters.GetString("TYPE"), out purpose);
			if (purpose == null) {
				purpose = new GameObjectItem(GameObjectClass.Other, "Unknown");
			}

			this.Player = new Player() {
				Id = this.RawParameters.GetInt("PID"),
				Country = new Country(this.RawParameters.GetInt("COUNTRY")),
				IsInAir = this.RawParameters.GetInt("INAIR") == 0,
				IsOnline = true,
				LoginId = this.RawParameters.GetGuid("LOGIN"),
				NickId = this.RawParameters.GetGuid("IDS"),
				NickName = this.RawParameters.GetString("NAME"),
				Plane = new Plane(this.RawParameters.GetInt("PLID"), this.RawParameters.GetString("TYPE")) {
					Bullets = this.RawParameters.GetInt("BUL"),
					Bombs = this.RawParameters.GetInt("BOMB"),
					Classification = GameObjectClass.Plane.ToString("g"),
					Fuel = this.RawParameters.GetDouble("FUEL"),
					Payload = this.RawParameters.GetString("PAYLOAD"),
					Purpose = purpose.Purpose,
					Shells = this.RawParameters.GetInt("SH"),
					Skin = this.RawParameters.GetString("SKIN"),
					WeaponMods = this.RawParameters.GetString("WM"),
				},
				BotPilot = new GameObject(this.RawParameters.GetInt("PID"), "BotPilot"),
				SortieId = this.EventID,
			};
			this.Player.CoalitionIndex = this.Server.GetCoalitionIndex(this.Player.Country);
			this.Server.Players.PlayerSpawn(this.Player);
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
			this.AirField = new AirField() {
				Country = new Country(this.RawParameters.GetInt("COUNTRY")),
				Id = this.RawParameters.GetInt("AID"),
				Position = this.RawParameters.GetVector3D("POS"),
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
			this.ObjectiveId = this.RawParameters.GetInt("OBJID");
			this.Position = this.RawParameters.GetVector3D("POS");
			this.IsPrimary = this.RawParameters.GetInt("TYPE") == 1;
			this.CoalitionIndex = this.RawParameters.GetInt("COAL");
			this.IsCompleted = this.RawParameters.GetInt("RES") == 1;
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
			this.MissionEndTime = DateTime.UtcNow;
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
			this.PlayerId = this.RawParameters.GetInt("PID");
			this.Position = this.RawParameters.GetVector3D("POS");
			this.Player = this.Server.Players[this.PlayerId];
			if (this.Player == null) {
				this.Bot = this.Server.GameObjects[this.PlayerId];
			} else {
				this.Player.IsInAir = false;
			}
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
			this.PlayerId = this.RawParameters.GetInt("PID");
			this.Position = this.RawParameters.GetVector3D("POS");

			this.Player = this.Server.Players[this.PlayerId];
			if (this.Player == null) {
				this.Bot = this.Server.GameObjects[this.PlayerId];
			} else {
				this.Player.IsInAir = true;
			}
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
			this.PlaneId = this.RawParameters.GetInt("PLID");
			this.PlayerId = this.RawParameters.GetInt("PID");
			this.Bullets = this.RawParameters.GetInt("BUL");
			this.Shells = this.RawParameters.GetInt("SH");
			this.Bombs = this.RawParameters.GetInt("BOMB");
			this.Rockets = this.RawParameters.GetInt("RCT");

			this.Player = this.Server.Players[this.PlaneId] ?? this.Server.Players[this.PlayerId];
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
		public int TargetId { get; set; } //Plane/vehicle ID
		public Vector3D Position { get; set; }
		public Player AttackerPlayer { get; set; }
		public Player TargetPlayer { get; set; }
		public GameObject AttackerObject { get; set; }
		public GameObject TargetObject { get; set; }
		public bool IsTeamKill => this.AttackerCoalition == this.TargetCoalition;

		public int Hits
		{
			get
			{
				if (this.TargetObject != null && this.AttackerObject != null) {
					return this.TargetObject.GetHitsCountBy(this.AttackerObject);
				} else {
					return 0;
				}
			}
		}

		public double Damage
		{
			get
			{
				if (this.TargetObject != null && this.AttackerObject != null) {
					return this.TargetObject.GetDamageBy(this.AttackerObject);
				} else {
					return 0;
				}
			}
		}

		public int AttackerCoalition
		{
			get
			{
				if (this.AttackerPlayer != null) {
					return this.AttackerPlayer.CoalitionIndex;
				} else if (this.AttackerObject != null) {
					return this.AttackerObject.CoalitionIndex;
				} else {
					return 0;
				}
			}
		}

		public int TargetCoalition
		{
			get
			{
				if (this.TargetPlayer != null) {
					return this.TargetPlayer.CoalitionIndex;
				} else if (this.TargetObject != null) {
					return this.TargetObject.CoalitionIndex;
				} else {
					return 0;
				}
			}
		}

		public MissionLogEventKill(MissionLogEventHeader header)
			: base(header)
		{
			this.AttackerId = this.RawParameters.GetInt("AID");
			this.TargetId = this.RawParameters.GetInt("TID");
			this.Position = this.RawParameters.GetVector3D("POS");

			this.AttackerPlayer = this.Server.Players[this.AttackerId];
			this.AttackerObject = this.Server.GameObjects[this.AttackerId];

			this.TargetPlayer = this.Server.Players[this.TargetId];
			this.TargetObject = this.Server.GameObjects[this.TargetId];

			if (this.AttackerPlayer == null && this.AttackerObject == null && this.TargetPlayer != null &&
				this.TargetPlayer.HitsSources.Count > 0) {
				this.AttackerPlayer = this.TargetPlayer.MostDamageByPlayer();
				if (this.AttackerPlayer == null) {
					this.AttackerObject = this.TargetPlayer.MostDamageByObject();
				}
			}
		}
	}

	//AType:2
	//T:26263 AType:2 DMG:0.013 AID:612352 TID:311297 POS(123759.180,255.837,242953.906)
	//Damage
	public class MissionLogEventDamage : MissionLogEventHeader
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
			this.AttackerId = this.RawParameters.GetInt("AID");
			this.TargetId = this.RawParameters.GetInt("TID");
			this.Damage = this.RawParameters.GetDouble("DMG");
			this.Position = this.RawParameters.GetVector3D("POS");

			this.AttackerPlayer = this.Server.Players[this.AttackerId];
			if (this.AttackerPlayer == null) {
				this.AttackerObject = this.Server.GameObjects[this.AttackerId];
			}

			this.TargetPlayer = this.Server.Players[this.TargetId];
			if (this.TargetPlayer == null) {
				this.TargetObject = this.Server.GameObjects[this.TargetId];
			}
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
			this.AttackerId = this.RawParameters.GetInt("AID");
			this.TargetId = this.RawParameters.GetInt("TID");
			this.AmmoName = this.RawParameters.GetString("AMMO");

			this.AttackerPlayer = this.Server.Players[this.AttackerId];
			if (this.AttackerPlayer == null) {
				this.AttackerObject = this.Server.GameObjects[this.AttackerId];
			}

			this.TargetPlayer = this.Server.Players[this.TargetId];
			if (this.TargetPlayer == null) {
				this.TargetObject = this.Server.GameObjects[this.TargetId];
			}
		}
	}

	//AType:0
	//Mission start, when player spawned
	//T:0 AType:0 GDate:1942.11.19 GTime:14:30:0 MFile:Multiplayer/Ranked/BoS_MP_RD_Stalingrad.msnbin 
	//MID: GType:2 CNTRS:0:0,101:1,201:2,202:2 SETTS:1111110111101001000000011 MODS:0 PRESET:1 AQMID:0
	public class MissionLogEventStart : MissionLogEventHeader
	{
		public DateTime GameDateTime { get; set; }
		
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
			var gameDate = Regex.Match(this.RawParameters.GetString("GDate"), @"(\d+)\.(\d+)\.(\d+)");
			var gameTime = Regex.Match(this.RawParameters.GetString("GTime"), @"(\d+)\:(\d+)\:(\d+)");
			if (gameDate.Success && gameTime.Success) {
				this.GameDateTime = new DateTime(
					int.Parse(gameDate.Groups[1].Value),
					int.Parse(gameDate.Groups[2].Value),
					int.Parse(gameDate.Groups[3].Value),
					int.Parse(gameTime.Groups[1].Value),
					int.Parse(gameTime.Groups[2].Value),
					int.Parse(gameTime.Groups[3].Value));
			}

			this.MissionFile = this.RawParameters.GetString("MFile");
			this.MissionID = this.RawParameters.GetString("MID");
			this.GameType = this.RawParameters.GetInt("GType");
			this.Mods = this.RawParameters.GetInt("MODS");
			this.Preset = this.RawParameters.GetInt("Preset");
			this.AQMId = this.RawParameters.GetInt("AWMID");
			this.SettingsFlags = this.RawParameters.GetString("SETTS");
			var coalitions = this.RawParameters.GetString("CNTRS");

			var countryPairs = coalitions.Split(',').Select(p => p.Split(':')).ToArray();
			this.CoalitionIndexes = new List<CoalitionIndex>();

			for (var i = 0; i < countryPairs.Length; i++) {
				if (countryPairs[i].Length == 2 && int.TryParse(countryPairs[i][0], out var country) && int.TryParse(countryPairs[i][1], out var index)) {
					this.CoalitionIndexes.Add(new CoalitionIndex() {
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