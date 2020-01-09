using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

//namespace IL2CDR.Scripts
//{
	public class GlobalStatistics : ActionScriptBase
	{

		public class ServerInfoPacketType
		{
			public int Type { get; set; }
			public Server Server { get; set; }
			public string ServerId { get; set; } 
			public string Token { get; set; }
			public ObjectInfoPacketType[] ObjectInfo { get; set; }
			public PlayerPacketType[] Players { get; set; }

		}	

		public class ObjectInfoPacketType
		{
			public Guid ObjectId { get; set; }
			public string Name { get; set; }
			public string Class { get; set; }
			public string Purpose { get; set; }

		}


		public class PlayerPacketType
		{
			public string NickId { get; set; }
			public int CountryId { get; set; }
			public int Ping { get; set; }
			public string Status { get; set; }
		}



	//private const string DOMAIN = "localhost";
	//private const string EVENTURL = "http://" + DOMAIN + ":49191/e/?XDEBUG_SESSION_START=55A2686E";
	//private const string PLAYERURL = "http://" + DOMAIN + ":49191/update/players/?XDEBUG_SESSION_START=55A2686E";
	//private const string EVENTURL = "http://" + DOMAIN + ":3992/e/?XDEBUG_SESSION_START=F3623ADB";
	//private const string PLAYERURL = "http://" + DOMAIN + ":3992/update/players/?XDEBUG_SESSION_START=55A2686E";

	private const string DOMAIN = "il2.info";
		private const string EVENTURL = "http://" + DOMAIN + "/e";
		private const string PLAYERURL = "http://" + DOMAIN + "/update/players";
		private const string BACKLOGFILE = "eventback.log";
		private ConcurrentQueue<object> events;
		private Timer sendTimer;
		private const int SENDTIMEOUT = 10000;
		private object lockSend = new object();
		private string lastPacket = string.Empty;
		private string token = string.Empty;
		private bool dictionarySent = false;
		private bool firstPacketSent = false;


		public GlobalStatistics()
		{
			this.events = new ConcurrentQueue<object>();
			this.sendTimer = new Timer(this.SendTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
		}

		public override ScriptConfig DefaultConfig
		{
			get
			{
				return new ScriptConfig() {
					//Enable events processing by this script
					IsEnabled = true,
					//Title for script list
					Title = "Global statistics",
					//Script description for GUI
					Description = "Send events to il2.info",
					//Settings in GUI
					ConfigFields = new ConfigFieldList() {
						//name, label, watermark, type, value, isVisible
						{"token", "Token", "Server authentication token", FieldType.Text, string.Empty, true},
					},
				};

			}
		}

		public override void OnServerLogStart(Server server)
		{
			var packet = new ServerInfoPacketType() {Type = 9999, Server = server};
			this.AddToQueue(packet);
		}

		public string Token
		{
			get
			{
				if (this.Config == null) {
					return string.Empty;
				}

				return this.Config.GetString("token");
			}
		}

		public override void OnApplicationStartup(object data)
		{
			this.sendTimer.Change(0, SENDTIMEOUT);
		}

		public override void OnApplicationShutdown(object data)
		{
			this.sendTimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		public void SendDataToServer(object sender)
		{
			var obj = sender as GlobalStatistics;
			if (obj == null) {
				return;
			}

			lock (obj.lockSend) {
				var result = "FAIL";
				if (string.IsNullOrWhiteSpace(obj.lastPacket)) {
					obj.lastPacket = this.GetNextPacket();
				}

				if (string.IsNullOrWhiteSpace(obj.lastPacket)) {
					return;
				}


				using (var webClient = new WebClientBase()) {
					var data = webClient.GZipBytes(obj.lastPacket);
					webClient.Timeout = 59000;
					webClient.ContentType = ContentType.JsonUTF8;
					webClient.KeepAlive = false;
					webClient.SetCookie("srvtoken", this.Token, DOMAIN);
					result = webClient.UploadCompressed(EVENTURL, obj.lastPacket);
					if (!string.IsNullOrWhiteSpace(result)) {
						if (result.Equals("OK", StringComparison.InvariantCultureIgnoreCase)) {
							if (!obj.firstPacketSent) {
								obj.firstPacketSent = true;
								Log.WriteInfo("Successfuly connected to il2.info");
							}

							obj.lastPacket = string.Empty;
						} else {
							Log.WriteInfo("Send result: {0}", result);
						}
					} else {
						Log.WriteInfo("Send result: FAIL (HTTP error)");
					}
				}
			}
		}

		public string GetNextPacket()
		{
			if (this.events.IsEmpty) {
				return null;
			}

			var content = string.Empty;
			var jsonPackets = new List<object>();

			var limit = 5;
			while (!this.events.IsEmpty) {
				limit--;
				if (limit <= 0) {
					break;
				}

				if (this.events.TryDequeue(out var obj)) {
					jsonPackets.Add(obj);
				}
			}

			return Json.Serialize(jsonPackets.ToList());
		}

		public void AddToQueue(object data)
		{
			if (string.IsNullOrWhiteSpace(this.Token)) {
				return;
			}

			if (data is MissionLogEventStart ||
				data is MissionLogEventMissionEnd ||
				data is MissionLogEventKill ||
				data is MissionLogEventPlayerAmmo ||
				data is MissionLogEventTakeOff ||
				data is MissionLogEventLanding ||
				data is MissionLogEventPlaneSpawn ||
				data is MissionLogEventGameObjectSpawn ||
				data is MissionLogEventObjectiveCompleted ||
				data is MissionLogEject ||
				data is MissionLogRemoveBot ||
				!(data is MissionLogEventHeader)) {
				//if( data is MissionLogEventHeader )
				//{
				//    var header = data as MissionLogEventHeader;
				//    Log.WriteInfo("{0} {1} {2}",header.MissionFile, header.EventTime, header.Type);
				//}
				if (data is MissionLogEventKill kill) {
					//Record kill only if player participate
					if (kill.TargetPlayer == null && kill.AttackerPlayer == null) {
						return;
					}
				}

				//var length = Json.Serialize(data).Length;
				//Log.WriteInfo("{0} JSON length: {1}", data.GetType(), length);
				this.events.Enqueue(data);
			}

			if (this.events.Count >= 5 || data is MissionLogEventMissionEnd || data is MissionLogEventPlayerAmmo) {
				Task.Factory.StartNew(this.SendDataToServer, this);
			}
		}

		private void SendTimerCallback(object sender)
		{
			if (this.events.Count > 0) {
				this.SendDataToServer(this);
			}
		}

		public override void OnHistory(object data)
		{
			if (data == null) {
				return;
			}

			if (!this.dictionarySent) {
				//Send game objects classification info
				this.AddGameObjectsToQueue();
			}

			this.AddToQueue(data);
			this.dictionarySent = true;
		}

		private void AddGameObjectsToQueue()
		{
			var packet = new ServerInfoPacketType() {
				Token = this.Token,
				Type = 9998,
				ObjectInfo = GameInfo.ObjectsClassification.Select(
					pair => new ObjectInfoPacketType() {
						ObjectId = GuidUtility.Create(GuidUtility.IsoOidNamespace, pair.Key),
						Name = pair.Key,
						Class = pair.Value.Classification.ToString("g"),
						Purpose = pair.Value.Purpose
					}).ToArray()
			};

			this.AddToQueue(packet);
		}

		public override void OnPlayerListChange(Server server, List<Player> players)
		{
			var packet = new ServerInfoPacketType() {
				ServerId = server.ServerId.ToString(),
				Players = players.Where(p => p != null && p.ClientId != 0 && p.NickId != null)
								.Select(p => new PlayerPacketType() {
											NickId = p.NickId.ToString(),
											CountryId = p.Country?.Id ?? 0,
											Ping = p.Ping,
											Status = p.Status.ToString(),
										})
								.ToArray()
			};
			var json = Json.Serialize(packet);
			using (var webClient = new WebClientBase()) {
				webClient.Timeout = 9000;
				webClient.ContentType = ContentType.JsonUTF8;
				webClient.KeepAlive = false;
				webClient.SetCookie("srvtoken", this.Token, DOMAIN);
				webClient.Upload(PLAYERURL, json);
			}
		}



	}
//}