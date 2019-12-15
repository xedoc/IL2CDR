using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace IL2CDR.Model
{
	public class MissionLogDataService : IMissionLogDataService, IStopStart
	{
		private readonly object lockHistory = new object();

		private List<object> missionHistory;

		private const string MASK = "missionReport(*)[*].txt";
		private string missionDateTimePrefix = string.Empty;

		private TextFileTracker tracker;
		private ActionManager actionManager;
		private readonly Server server;
		private DateTime lastEventTime;
		private long lastTick;

		private readonly Dictionary<EventType, Action<MissionLogEventHeader>> historyHandlers =
			new Dictionary<EventType, Action<MissionLogEventHeader>>() {
				{
					EventType.AirfieldInfo, (data) => data.With(x => x as MissionLogEventAirfieldInfo)
						.With(x => x.AirField).Do(x => data.Server.AirFields[x.Id] = x)
				}, {
					EventType.PlaneSpawn, (data) => data.With(x => x as MissionLogEventPlaneSpawn)
						.With(x => x.Player).Do(x => data.Server.Players[x.Id] = x)
				}, {
					EventType.GameObjectSpawn, (data) => data.With(x => x as MissionLogEventGameObjectSpawn)
						.With(x => x.Object).Do(x => data.Server.GameObjects[x.Id] = x)
				}, {
					EventType.Leave, (data) => data.With(x => x as MissionLogEventPlayerLeave)
						.Do(x => data.Server.Players.PlayerLeave(x.NickGuid))
				}, {
					EventType.Kill, (data) => data.With(x => x as MissionLogEventKill)
						.Do(x => data.Server.DestroyObject(x.TargetId))
				}, {
					EventType.MissionStart, (data) => data.With(x => x as MissionLogEventStart)
						.Do(x => data.Server.CoalitionIndexes = x.CoalitionIndexes)
				}, {
					EventType.Hit, (data) => data.With(x => x as MissionLogEventHit)
						.Do(x => data.Server.AddHit(x as MissionLogEventHit))
				}, {
					EventType.Damage, (data) => data.With(x => x as MissionLogEventDamage)
						.Do(x => data.Server.AddDamage(x as MissionLogEventDamage))
				}, {
					EventType.InfluenceAreaInfo, (data) => data.With(x => x as MissionLogEventInfluenceAreaInfo)
						.Do(x => data.Server.Areas.AddArea((x as MissionLogEventInfluenceAreaInfo).Area))
				}, {
					EventType.InfluenceAreaBoundary, (data) => data.With(x => x as MissionLogEventInfluenceAreaBoundary)
						.Do(x => data.Server.Areas.AddArea((x as MissionLogEventInfluenceAreaBoundary).Area))
				},
			};


		public List<object> MissionHistory
		{
			get => this.missionHistory;
			set => this.missionHistory = value;
		}


		public DateTime MissionStartDateTime { get; set; }
		public string MissionLogFolder { get; set; }

		public MissionLogDataService(Server server)
		{
			this.server = server;
			this.MissionLogFolder = server.Rcon.Config.MissionTextLogFolder;
			this.Initialize();
		}

		public MissionLogDataService(string missionLogFolder)
		{
			Log.WriteInfo("Start monitoring of {0} folder", missionLogFolder);
			this.MissionLogFolder = missionLogFolder;
			this.server = new Server("LogParser", true, true);
			this.missionHistory = new List<object>();
			// <--> intentionally NOT calling this.Initialize()? This constructor is called from script "LogParser" only to process historical logs. 
		}

		public void Initialize()
		{
			this.missionHistory = new List<object>();
			this.tracker = new TextFileTracker(this.MissionLogFolder, MASK);
			this.tracker.OnNewLine = (line) => {
				var data = MissionLogDataBuilder.GetData(line, this.MissionStartDateTime, this.GetCurrentEventNumber(),
					this.server);

				if (data != null && this.actionManager != null) {
					var header = data as MissionLogEventHeader;
					header.MissionFile = Path.GetFileName(this.tracker.CurrentFileName);

					this.AddHistory(data);
					this.actionManager.ProcessAction(data);
				}
			};

			this.tracker.OnFileCreation = (filePath) => {
				if (Regex.IsMatch(filePath, @"missionReport\([\d+|\-|_]+\)\[0\].txt", RegexOptions.IgnoreCase)) {
					//New mission log                    
					this.ClearHistory();
					this.StartNewMission(filePath);
				}
			};
		}

		private void StartNewMission(string logFilePath)
		{
			//Check if MissionEnd is sent
			lock (this.lockHistory) {
				if (this.missionHistory != null && this.missionHistory.Count > 0) {
					var existing = this.missionHistory.FirstOrDefault(data => data is MissionLogEventMissionEnd);
					if (existing == null) {
						var endMission = new MissionLogEventMissionEnd(
							new MissionLogEventHeader(string.Format("T:{0} AType:7", this.lastTick), this.lastEventTime));
						this.AddHistory(endMission);
						this.actionManager.ProcessAction(endMission);
					}
				}
			}

			Log.WriteInfo("New mission started {0}", logFilePath);
			this.missionDateTimePrefix = Re.GetSubString(logFilePath, @"missionReport\((.*)?\)\[0\]\.txt");

			if (string.IsNullOrWhiteSpace(this.missionDateTimePrefix)) {
				return;
			}

			this.server.ResetMission();

			this.server.CurrentMissionId = GuidUtility.Create(GuidUtility.IsoOidNamespace,
				string.Concat(this.server.ServerId, "_", this.missionDateTimePrefix)).ToString();
			this.MissionStartDateTime = Util.ParseDate(this.missionDateTimePrefix).ToUniversalTime();
		}

		public void ReadMissionHistory(string firstMissionLogFile = null)
		{
			//missionReport(2015-02-25_11-43-53)[0].txt

			if (firstMissionLogFile == null) {
				firstMissionLogFile = Util.GetNewestFilePath(this.MissionLogFolder, "missionReport(*)[0].txt");
				if (string.IsNullOrWhiteSpace(firstMissionLogFile)) {
					Log.WriteError("Mission log not found in {0}", this.MissionLogFolder);
					return;
				}
			} else {
				var file = firstMissionLogFile;
				firstMissionLogFile = this.With(x => Re.GetSubString(file, @"(missionReport\([\d+|\-|_]+\))\[\d+\].txt"))
										  .With(x => string.Concat(x, "[0].txt"));
			}


			if (string.IsNullOrWhiteSpace(firstMissionLogFile)) {
				Log.WriteError("Malformed log filename {0}", firstMissionLogFile);
				return;
			}

			Log.WriteInfo("Reading events history from {0}", firstMissionLogFile);


			this.StartNewMission(firstMissionLogFile);

			if (this.MissionStartDateTime.Equals(default(DateTime))) {
				return;
			}


			var missionFiles = Util.GetFilesSortedByTime(this.MissionLogFolder,
				$"missionReport({this.missionDateTimePrefix})[*].txt", true);

			var readException = Util.Try(() => {
				foreach (var file in missionFiles) {
					var fileInfo = new FileInfo(file);
					if (fileInfo.Length > 0 && !string.IsNullOrWhiteSpace(file)) {
						this.tracker?.AddFileOffset(file, fileInfo.Length);

						var lines = File.ReadAllLines(file);
						foreach (var line in lines) {
							var data = MissionLogDataBuilder.GetData(line, this.MissionStartDateTime,
								this.GetCurrentEventNumber(), this.server);
							if (data is MissionLogEventHeader header) {
								header.MissionFile = Path.GetFileName(file);
								this.AddHistory(header);
							}
						}
					}
				}
			});
		}

		private int GetCurrentEventNumber()
		{
			int result;
			lock (this.lockHistory) {
				result = this.missionHistory.Count;
			}

			return result;
		}

		private void AddHistory(object data)
		{
			if (data == null) {
				return;
			}

			lock (this.lockHistory) {
				this.missionHistory.Add(data);
			}

			if (data is MissionLogEventHeader header) {
				this.lastEventTime = header.EventTime;
				this.lastTick = header.Ticks;

				Action<MissionLogEventHeader> action;
				if (this.historyHandlers.TryGetValue(header.Type, out action)) {
					action(header);
				}
			}

			this.actionManager?.ProcessHistory(data);
		}

		private void ClearHistory()
		{
			lock (this.lockHistory) {
				this.missionHistory.Clear();
			}
		}

		public void Start()
		{
			if (string.IsNullOrWhiteSpace(this.MissionLogFolder)) {
				Log.WriteInfo("No mission folder specified!");
				return;
			}

			if (!Directory.Exists(this.MissionLogFolder)) {
				Log.WriteInfo("Specified mission folder doesn't exist {0}", this.MissionLogFolder);
				return;
			}

			this.ClearHistory();

			if (Application.Current != null && Application.Current is App app) {
				Log.WriteInfo("Initialize action manager for {0}", this.server.Name);
				this.actionManager = app.ActionManager;
				this.actionManager.ProcessServerLogStart(this.server);
			} else {
				var scriptManager = new ScriptManager();
				scriptManager.LoadScripts();
				scriptManager.Start();
				this.actionManager = new ActionManager(new ScriptManager());
			}

			this.server.OnPlayerListChange = (players, srv) => {
				this.actionManager.ProcessPlayerListChange(srv, players);
			};


			this.ReadMissionHistory();


			Log.WriteInfo("Starting MissionLogDataService.TextFileTracker for server '{0}' on directory '{1}'", this.server.Name, this.tracker?.Folder);
			this.tracker?.Start();
		}

		public void Stop()
		{
			this.tracker.Stop();
		}

		public void Restart()
		{
			this.Stop();
			this.Start();
		}
	}
}