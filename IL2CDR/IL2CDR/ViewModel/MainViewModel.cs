using System;
using System.Windows;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using IL2CDR.Model;
using IL2CDR.Properties;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IL2CDR.ViewModel
{
	/// <summary>
	/// This class contains properties that the main View can data bind to.
	/// <para>
	/// See http://www.galasoft.ch/mvvm
	/// </para>
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		private readonly DServerManager dserverManager;
		private readonly ActionManager actionManager;
		private readonly ScriptManager scriptManager;

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		[PreferredConstructor]
		public MainViewModel()
		{
			if (this.IsInDesignMode) {
				this.ServerList = new ObservableCollection<Server>(new List<Server>()
					{new Server("Xedoc playground", false, true)});
				return;
			}

			this.Config = Settings.Default.Config;

			if (Application.Current is App app) {
				this.dserverManager = app.DServerManager;
				this.dserverManager.Servers.CollectionChanged += this.DServers_CollectionChanged;
				this.dserverManager.With(x => x.Servers).Do(x => {
					this.ServerList = x;
					this.UpdateServerList();
				});

				this.scriptManager = app.ScriptManager;
				this.actionManager = app.ActionManager;

				this.CurrentScriptSettings = this.Config.ScriptConfigs.FirstOrDefault();

				var messages = app.AppLogDataService.LogMessages;
				this.LogMessages = string.Join(Environment.NewLine, messages);
				messages.CollectionChanged += this.messages_CollectionChanged;
			}
		}

		private void DServers_CollectionChanged(object sender,
			System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			this.UpdateServerList();
		}

		private void UpdateServerList()
		{
			UI.Dispatch(() => {
				if (this.ServerList.Count == 0) {
					this.IsServerListMessageVisible = true;
					this.ServerListMessage = "Run some DServer to start log monitoring...";
				} else {
					this.IsServerListMessageVisible = false;
				}
			});
		}

		private void messages_CollectionChanged(object sender,
			System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			e.NewItems.Do(x => {
				var newLines = new string[x.Count + 1];
				newLines[0] = string.Empty;
				x.CopyTo(newLines, 1);
				UI.Dispatch(() => this.LogMessages += string.Join(Environment.NewLine, newLines));
			});
		}


		/// <summary>
		/// The <see cref="CurrentScriptSettings" /> property's name.
		/// </summary>
		public const string CURRENT_SCRIPT_SETTINGS_PROPERTY_NAME = "CurrentScriptSettings";

		private ScriptConfig _currentScriptSettings = null;

		/// <summary>
		/// Sets and gets the CurrentScriptSettings property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public ScriptConfig CurrentScriptSettings
		{
			get => this._currentScriptSettings;

			set
			{
				if (this._currentScriptSettings == value) {
					return;
				}

				this._currentScriptSettings = value;
				this.RaisePropertyChanged(CURRENT_SCRIPT_SETTINGS_PROPERTY_NAME);
			}
		}

		private RelayCommand<ScriptConfig> _scriptSelectionChanged;

		/// <summary>
		/// Gets the ScriptSelectionChanged.
		/// </summary>
		public RelayCommand<ScriptConfig> ScriptSelectionChanged
		{
			get
			{
				return this._scriptSelectionChanged
						?? (this._scriptSelectionChanged = new RelayCommand<ScriptConfig>(
							(config) => { this.CurrentScriptSettings.ConfigFields = config.ConfigFields; }));
			}
		}

		/// <summary>
		/// The <see cref="ServerListMessage" /> property's name.
		/// </summary>
		public const string SERVER_LIST_MESSAGE_PROPERTY_NAME = "ServerListMessage";

		private string _serverListMessage = null;

		/// <summary>
		/// Sets and gets the ServerListMessage property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public string ServerListMessage
		{
			get => this._serverListMessage;

			set
			{
				if (this._serverListMessage == value) {
					return;
				}

				this._serverListMessage = value;
				this.RaisePropertyChanged(SERVER_LIST_MESSAGE_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="IsServerListMessageVisible" /> property's name.
		/// </summary>
		public const string IS_SERVER_LIST_MESSAGE_VISIBLE_PROPERTY_NAME = "IsServerListMessageVisible";

		private bool _isServerListMessageVisible = false;

		/// <summary>
		/// Sets and gets the IsServerListMessageVisible property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public bool IsServerListMessageVisible
		{
			get => this._isServerListMessageVisible;

			set
			{
				if (this._isServerListMessageVisible == value) {
					return;
				}

				this._isServerListMessageVisible = value;
				this.RaisePropertyChanged(IS_SERVER_LIST_MESSAGE_VISIBLE_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="IsWindowReopen" /> property's name.
		/// </summary>
		public const string IS_WINDOW_REOPEN_PROPERTY_NAME = "IsWindowReopen";

		private bool _isWindowReopen = false;

		/// <summary>
		/// Sets and gets the IsWindowReopen property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public bool IsWindowReopen
		{
			get => this._isWindowReopen;

			set
			{
				if (this._isWindowReopen == value) {
					return;
				}

				this._isWindowReopen = value;
				this.RaisePropertyChanged(IS_WINDOW_REOPEN_PROPERTY_NAME);
			}
		}

		private RelayCommand _exitApplication;

		/// <summary>
		/// Gets the ExitApplication.
		/// </summary>
		public RelayCommand ExitApplication
		{
			get
			{
				return this._exitApplication
						?? (this._exitApplication = new RelayCommand(
							() => {
								if (!this.IsWindowReopen) {
									Settings.Default.Save();
									Application.Current.Shutdown();
								}

								this.IsWindowReopen = false;
							}));
			}
		}

		/// <summary>
		/// The <see cref="Config" /> property's name.
		/// </summary>
		public const string CONFIG_PROPERTY_NAME = "Config";

		private Config _config = null;

		/// <summary>
		/// Sets and gets the Config property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public Config Config
		{
			get => this._config;

			set
			{
				if (this._config == value) {
					return;
				}

				this._config = value;
				this.RaisePropertyChanged(CONFIG_PROPERTY_NAME);
			}
		}


		/// <summary>
		/// The <see cref="LogMessages" /> property's name.
		/// </summary>
		public const string LOG_MESSAGES_PROPERTY_NAME = "LogMessages";

		private string _logMessages = null;

		/// <summary>
		/// Sets and gets the LogMessages property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public string LogMessages
		{
			get => this._logMessages;

			set
			{
				if (this._logMessages == value) {
					return;
				}

				this._logMessages = value;
				this.RaisePropertyChanged(LOG_MESSAGES_PROPERTY_NAME);
			}
		}

		private RelayCommand _enableChatLogMonitor;

		/// <summary>
		/// Gets the EnableChatLogMonitor.
		/// </summary>
		public RelayCommand EnableChatLogMonitor
		{
			get
			{
				return this._enableChatLogMonitor
						?? (this._enableChatLogMonitor = new RelayCommand(
							() => { }));
			}
		}

		private RelayCommand _enableMissionLogMonitor;

		/// <summary>
		/// Gets the EnableMissionLogMonitor.
		/// </summary>
		public RelayCommand EnableMissionLogMonitor
		{
			get
			{
				return this._enableMissionLogMonitor
						?? (this._enableMissionLogMonitor = new RelayCommand(
							() => {
								var missionLogService = (Application.Current as App)?.MissionLogDataService;
								if (missionLogService == null) {
									return;
								}

								if (!this.Config.IsMissionLogMonitorEnabled) {
									missionLogService.Stop();
								} else {
									missionLogService.Start();
								}
							}));
			}
		}

		private RelayCommand<string> _scriptCheck;

		/// <summary>
		/// Gets the ScriptCheck.
		/// </summary>
		public RelayCommand<string> ScriptCheck
		{
			get
			{
				return this._scriptCheck
						?? (this._scriptCheck = new RelayCommand<string>(
							(scriptFile) => {
								if (this.scriptManager != null) {
									this.scriptManager.SwitchScript(scriptFile);
								}
							}));
			}
		}

		private RelayCommand<string> _scriptButtonClick;

		/// <summary>
		/// Gets the ScriptButtonClick.
		/// </summary>
		public RelayCommand<string> ScriptButtonClick
		{
			get
			{
				return this._scriptButtonClick
						?? (this._scriptButtonClick = new RelayCommand<string>(
							(buttonName) => {
								Task.Factory.StartNew(() => this.actionManager.ProcessButtonClick(buttonName));
							}));
			}
		}

		/// <summary>
		/// The <see cref="ServerList" /> property's name.
		/// </summary>
		public const string SERVER_LIST_PROPERTY_NAME = "ServerList";

		private ObservableCollection<Server> _serverList = null;

		/// <summary>
		/// Sets and gets the ServerList property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public ObservableCollection<Server> ServerList
		{
			get => this._serverList;

			set
			{
				if (this._serverList == value) {
					return;
				}

				this._serverList = value;
				this.RaisePropertyChanged(SERVER_LIST_PROPERTY_NAME);
			}
		}

		////public override void Cleanup()
		////{
		////    // Clean up if needed

		////    base.Cleanup();
		////}
	}
}