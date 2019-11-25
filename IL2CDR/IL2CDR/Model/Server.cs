using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace IL2CDR.Model
{
	public class Server : NotifyPropertyChangeBase
	{
		private readonly object lockOnlinePlayers = new object();
		[JsonIgnore] public RconConnection Rcon { get; set; }
		[JsonIgnore] public MissionLogDataService MissionLogService { get; set; }
		[JsonIgnore] public ProcessItem Process { get; set; }
		[JsonIgnore] public Action<List<Player>, Server> OnPlayerListChange { get; set; }
		public string CurrentMissionId { get; set; }
		public int TimeZoneOffset { get; set; }

		private Timer timerPlayerList;

		public Server(RconConnection rcon, ProcessItem process)
		{
			this.Name = $@"PID: {process.Id} {process.Path}\DServer.exe";
			this.Process = process;
			this.Rcon = rcon;
			this.MissionLogService = new MissionLogDataService(this);
			this.ServerId = default(Guid);
			this.IsConfigSet = false;
			this.IsRconConnected = false;
			this.TimeZoneOffset = (int) TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
			this.Initialize();
		}

		public Server(string name, bool isConfigSet, bool isRconConnected)
		{
			this.Name = name;
			this.ServerId = GuidUtility.Create(GuidUtility.IsoOidNamespace, name);
			this.IsConfigSet = isConfigSet;
			this.IsRconConnected = isRconConnected;
			this.Initialize();
		}

		private void Initialize()
		{
			this.timerPlayerList = new Timer(this.UpdateOnlinePlayers, this, 0, 30000);
			this.Players = new PlayersCollection {
				OnPlayerJoin = (player) => {
					if (player == null) {
						return;
					}

					var existing = this.OnlinePlayers.FirstOrDefault(p => p.NickId.Equals(player.NickId));
					if (existing == null) {
						lock (this.lockOnlinePlayers) {
							this.OnlinePlayers.Add(player);
						}
					} else {
						existing.Country = player.Country;
					}
				},
				OnPlayerLeave = (player) => {
					if (player == null) {
						return;
					}

					lock (this.lockOnlinePlayers) {
						this.OnlinePlayers.RemoveAll(p => p.NickId.Equals(player.NickId));
					}
				}
			};


			this.GameObjects = new GameObjectsCollection();
			this.AirFields = new AirFieldCollection();
			this.CoalitionIndexes = new List<CoalitionIndex>();
		}


		private void UpdateOnlinePlayers(object sender)
		{
			var server = sender as Server;
			if (server == null || !server.IsRconConnected || this.Rcon == null) {
				return;
			}

			var newList = this.Rcon.GetPlayerList().Where(p => p != null).ToList();
			if (newList.Count <= 0) {
				lock (server.lockOnlinePlayers) {
					this.OnlinePlayers.Clear();
				}

				this.OnPlayerListChange?.Invoke(this.OnlinePlayers.ToList(), this);

				return;
			}

			var onlineIds = new HashSet<Guid>(newList.Select(player => player.NickId));

			lock (server.lockOnlinePlayers) {
				var playerListChanged = this.OnlinePlayers.Count(p => !onlineIds.Contains(p.NickId)) > 0;

				this.OnlinePlayers.RemoveAll(player => !onlineIds.Contains(player.NickId));

				foreach (var player in newList) {
					var existing = this.OnlinePlayers.FirstOrDefault(p => p.NickId.Equals(player.NickId));
					if (existing == null) {
						this.OnlinePlayers.Add(player);
						playerListChanged = true;
					} else {
						if (!playerListChanged) {
							playerListChanged = existing.Ping != player.Ping ||
												existing.Status != player.Status ||
												existing.CoalitionIndex != player.CoalitionIndex;
						}

						existing.Ping = player.Ping;
						existing.Status = player.Status;
						existing.ClientId = player.ClientId;
						existing.CoalitionIndex = player.CoalitionIndex;
					}
				}

				if (playerListChanged) {
					this.OnPlayerListChange?.Invoke(this.OnlinePlayers.ToList(), this);
				}
			}
		}

		public void Login()
		{
			while (true) {
				var result = this.Rcon.GetConsole();
				var newName = Re.GetSubString(result, @"Server name '(.*?)'");
				if (!string.IsNullOrWhiteSpace(newName)) {
					this.Name = newName;
					break;
				} else {
					Thread.Sleep(1000);
				}
			}
		}

		public void ResetMission()
		{
			this.Players.Clear();
			this.AirFields.Clear();
			this.GameObjects.Clear();
			this.Areas.Clear();
		}

		public int GetCoalitionIndex(Country country)
		{
			var coalition = this.CoalitionIndexes.FirstOrDefault(c => c.Country.Id == country.Id);
			if (coalition != null) {
				return coalition.Index;
			} else {
				return -1;
			}
		}

		/// <summary>
		/// The <see cref="OnlinePlayers" /> property's name.
		/// </summary>
		public const string ONLINE_PLAYERS_PROPERTY_NAME = "OnlinePlayers";

		private ObservableCollection<Player> _onlinePlayers = new ObservableCollection<Player>();

		/// <summary>
		/// Sets and gets the OnlinePlayers property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[JsonIgnore]
		public ObservableCollection<Player> OnlinePlayers
		{
			get => this._onlinePlayers;

			set
			{
				if (this._onlinePlayers == value) {
					return;
				}

				this._onlinePlayers = value;
				this.RaisePropertyChanged(ONLINE_PLAYERS_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="CoalitionIndexes" /> property's name.
		/// </summary>
		public const string COALITION_INDEXES_PROPERTY_NAME = "CoalitionIndexes";

		private List<CoalitionIndex> _coalitionIndexes;

		/// <summary>
		/// Sets and gets the CoalitionIndexes property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[JsonIgnore]
		public List<CoalitionIndex> CoalitionIndexes
		{
			get => this._coalitionIndexes;

			set
			{
				if (this._coalitionIndexes == value) {
					return;
				}

				this._coalitionIndexes = value;
				this.RaisePropertyChanged(COALITION_INDEXES_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="ServerId" /> property's name.
		/// </summary>
		public const string SERVER_ID_PROPERTY_NAME = "ServerId";

		private Guid _serverId;

		/// <summary>
		/// Sets and gets the ServerId property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public Guid ServerId
		{
			get => this._serverId;

			set
			{
				if (this._serverId == value) {
					return;
				}

				this._serverId = value;
				this.RaisePropertyChanged(SERVER_ID_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="GameObjects" /> property's name.
		/// </summary>
		public const string GAME_OBJECTS_PROPERTY_NAME = "GameObjects";

		private GameObjectsCollection _gameObjects;

		/// <summary>
		/// Sets and gets the GameObjects property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[JsonIgnore]
		public GameObjectsCollection GameObjects
		{
			get => this._gameObjects;

			set
			{
				if (this._gameObjects == value) {
					return;
				}

				this._gameObjects = value;
				this.RaisePropertyChanged(GAME_OBJECTS_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="AirFields" /> property's name.
		/// </summary>
		public const string AIR_FIELDS_PROPERTY_NAME = "AirFields";

		private AirFieldCollection _airFields;

		/// <summary>
		/// Sets and gets the AirFields property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[JsonIgnore]
		public AirFieldCollection AirFields
		{
			get => this._airFields;

			set
			{
				if (this._airFields == value) {
					return;
				}

				this._airFields = value;
				this.RaisePropertyChanged(AIR_FIELDS_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="Areas" /> property's name.
		/// </summary>
		public const string AREAS_PROPERTY_NAME = "Areas";

		private AreaCollection _areas = new AreaCollection();

		/// <summary>
		/// Sets and gets the Areas property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[JsonIgnore]
		public AreaCollection Areas
		{
			get => this._areas;

			set
			{
				if (this._areas == value) {
					return;
				}

				this._areas = value;
				this.RaisePropertyChanged(AREAS_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="Players" /> property's name.
		/// </summary>
		public const string PLAYERS_PROPERTY_NAME = "Players";

		private PlayersCollection _players;

		/// <summary>
		/// Sets and gets the Players property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[JsonIgnore]
		public PlayersCollection Players
		{
			get => this._players;

			set
			{
				if (this._players == value) {
					return;
				}

				this._players = value;
				this.RaisePropertyChanged(PLAYERS_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="Name" /> property's name.
		/// </summary>
		public const string NAME_PROPERTY_NAME = "Name";

		private string _name = null;

		/// <summary>
		/// Sets and gets the Name property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public string Name
		{
			get => this._name;

			set
			{
				if (this._name == value) {
					return;
				}

				this._name = value;
				this.RaisePropertyChanged(NAME_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="IsConfigSet" /> property's name.
		/// </summary>
		public const string IS_CONFIG_SET_PROPERTY_NAME = "IsConfigSet";

		private bool _isConfigSet = false;

		/// <summary>
		/// Sets and gets the IsConfigSet property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[JsonIgnore]
		public bool IsConfigSet
		{
			get => this._isConfigSet;

			set
			{
				if (this._isConfigSet == value) {
					return;
				}

				this._isConfigSet = value;
				this.RaisePropertyChanged(IS_CONFIG_SET_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="IsRconConnected" /> property's name.
		/// </summary>
		public const string IS_RCON_CONNECTED_PROPERTY_NAME = "IsRconConnected";

		private bool _IsRconConnected = false;

		/// <summary>
		/// Sets and gets the IsRconConnected property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[JsonIgnore]
		public bool IsRconConnected
		{
			get => this._IsRconConnected;

			set
			{
				if (this._IsRconConnected == value) {
					return;
				}

				this._IsRconConnected = value;
				this.RaisePropertyChanged(IS_RCON_CONNECTED_PROPERTY_NAME);
			}
		}

		public void DestroyObject(int id)
		{
			this.Players.PlayerKilled(id);
			this.GameObjects.ObjectDestroyed(id);
		}


		public void AddHit(MissionLogEventHit e)
		{
			if (e == null) {
				return;
			}

			if (e.TargetObject != null) {
				e.TargetObject.AddHit((object) e.AttackerPlayer ?? e.AttackerObject, e);
			} else {
				e.TargetPlayer?.AddHit((object) e.AttackerPlayer ?? e.AttackerObject, e);
			}

			if (e.AttackerPlayer != null) {
				e.AttackerPlayer.Hits++;
			}
		}

		public void AddDamage(MissionLogEventDamage e)
		{
			if (e == null) {
				return;
			}

			if (e.TargetObject != null) {
				e.TargetObject.AddDamage((object) e.AttackerPlayer ?? e.AttackerObject, e);
			} else {
				e.TargetPlayer?.AddDamage((object) e.AttackerPlayer ?? e.AttackerObject, e);
			}
		}
	}
}