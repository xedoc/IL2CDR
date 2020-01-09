using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;

namespace IL2CDR.Model
{
	public class Server : NotifyPropertyChangeBase
	{
		private readonly object lockOnlinePlayers = new object();

		[JsonIgnore] public IL2StartupConfig ServerConfig { get; set; }


		[JsonIgnore]
		public RconConnection Rcon
		{
			get => this._rcon; 
			set
			{
				if (this._rcon == value) {
					return;			// <-- nothing to do. 
				}

				// -- i) unassociate old instance: 
				if (this._rcon != null) {
					this._rcon.PropertyChanged -= this.OnRconPropertyChanged;
					this._rcon = null; 
				}


				// -- ii) associate new instance
				this._rcon = value;
				this._rcon.PropertyChanged += this.OnRconPropertyChanged; 

			}
		}

		private RconConnection _rcon;



		/// <summary>
		/// This is the proxy property to get, whether this server has a "valid configuration" (i.e., whether this.ServerConfig.IsConfigReady == true). 
		/// </summary>
		[JsonIgnore]
		public bool IsConfigSet => this.ServerConfig.IsConfigReady;


		/// <summary>
		/// Proxy property, that gets the IsConnected status of the Rcon service of this server. 
		/// </summary>
		[JsonIgnore]
		public bool IsRconConnected => this.Rcon.IsConnected;


		[JsonIgnore] public MissionLogDataService MissionLogService { get; set; }
		[JsonIgnore] public ProcessItem Process { get; set; }
		[JsonIgnore] public Action<List<Player>, Server> OnPlayerListChange { get; set; }
		public string CurrentMissionId { get; set; }
		public int TimeZoneOffset { get; set; }

		private Timer timerPlayerList;

		public Server(IL2StartupConfig serverConfig, RconConnection rcon, ProcessItem process)
		{
			this.Name = $@"PID: {process.Id} {process.Path}\DServer.exe";
			this.ServerId = default(Guid);
			this.Process = process;
			this.ServerConfig = serverConfig;
			this.Rcon = rcon;

			this.MissionLogService = new MissionLogDataService(this);

			this.TimeZoneOffset = (int) TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
			this.Initialize();
		}

		[Obsolete("This constructor is WEIRD! Please do NOT use it!", false)]
		public Server(string name, bool isConfigSet, bool isRconConnected)
		{
			this.Name = name;
			this.ServerId = GuidUtility.Create(GuidUtility.IsoOidNamespace, name);

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


		private void OnRconPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
		{
			if (eventArgs != null && (eventArgs.PropertyName == nameof(this.Rcon.IsConnected) || eventArgs.PropertyName == nameof(this.Rcon.IsRunning)) ) {
				this.RaisePropertyChanged(nameof(this.IsRconConnected));
			}

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
				this.RaisePropertyChanged(nameof(this.OnlinePlayers));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.CoalitionIndexes));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.ServerId));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.GameObjects));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.AirFields));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.Areas));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.Players));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.Name));
			}
		}



		public RelayCommand SwitchRconOnOff
		{
			get
			{
				if (this._switchRconOnOff == null) {
					this._switchRconOnOff = new RelayCommand(
							() => {
								if (this.Rcon.IsRunning) {
									this.Rcon.Stop();
								} else {
									this.Rcon.Start();
								}
							}
						);
				}

				return this._switchRconOnOff;
			}
		}

		private RelayCommand _switchRconOnOff; 




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