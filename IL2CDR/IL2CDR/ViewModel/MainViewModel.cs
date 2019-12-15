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
using System.Collections.Specialized;
using System.ComponentModel;
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
			this.Config.PropertyChanged += this.Config_OnChanged; 

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

		private void DServers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.UpdateServerList();
		}


		private void Config_OnChanged(object sender, PropertyChangedEventArgs eventArgs)
		{
			if (Application.Current is App app) {
				app.AppLogDataService.MaxNumberOfLines = this.Config.ApplicationLogBufferSize; 
			}
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

		private void messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			//e.NewItems.Do(x => {
			//	var newLines = new string[x.Count + 1];
			//	newLines[0] = string.Empty;
			//	x.CopyTo(newLines, 1);
			//	UI.Dispatch(() => this.LogMessages += string.Join(Environment.NewLine, newLines));
			//});

			var ocLogLines = sender as ObservableCollection<string>;
			if (ocLogLines != null) {
				this.LogMessages = string.Join(Environment.NewLine, ocLogLines); 
			}
			
		}




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
				this.RaisePropertyChanged(nameof(this.CurrentScriptSettings));
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
				this.RaisePropertyChanged(nameof(this.ServerListMessage));
			}
		}



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
				this.RaisePropertyChanged(nameof(this.IsServerListMessageVisible));
			}
		}



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
				this.RaisePropertyChanged(nameof(this.IsWindowReopen));
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
				this.RaisePropertyChanged(nameof(this.Config));
			}
		}



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
				this.RaisePropertyChanged(nameof(this.LogMessages));
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
								var allMissionLogServerServices = this.dserverManager?.Servers?
																		.Select(server => server.MissionLogService)
																		.Where(mlds => mlds != null)
																		.ToList(); 


								//var missionLogService = (Application.Current as App)?.MissionLogDataService;
								if (allMissionLogServerServices == null) {
									return;
								}

								if (!this.Config.IsMissionLogMonitorEnabled) {
									allMissionLogServerServices.ForEach(mlds => mlds.Stop());
								} else {
									allMissionLogServerServices.ForEach(mlds => mlds.Start());
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
				this.RaisePropertyChanged(nameof(this.ServerList));
			}
		}

		////public override void Cleanup()
		////{
		////    // Clean up if needed

		////    base.Cleanup();
		////}
	}
}