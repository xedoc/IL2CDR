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


		private Task messagesRefresherTask;
		private int ignoredCalls = 0;
		private int executedCalls = 0; 

		private void messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// -- THROTTLING of events -- fire "ScrollingTextBox" refresh only once per 500ms: 

			if (this.messagesRefresherTask != null && !this.messagesRefresherTask.IsCompleted) {
				this.ignoredCalls++;
				return;		// <-- refresher task is stil "running", so the refresh is scheduled in a "near" future... don't do anything now. 
			}

			this.messagesRefresherTask = Task.Factory.StartNew(async () => {
				await Task.Delay(500);
				UI.Dispatch(() => {
					this.executedCalls++; 
					var ocLogLines = sender as ObservableCollection<string>;
					if (ocLogLines != null) {
						this.LogMessages = string.Join(Environment.NewLine, ocLogLines);
					}
				});
			});
			
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

		//private RelayCommand _enableChatLogMonitor;

		///// <summary>
		///// Gets the EnableChatLogMonitor.
		///// </summary>
		//public RelayCommand EnableChatLogMonitor
		//{
		//	get
		//	{
		//		return this._enableChatLogMonitor
		//				?? (this._enableChatLogMonitor = new RelayCommand(
		//					() => { }));
		//	}
		//}

		private RelayCommand _refreshEnabledDisabledStateOfServerComponentsCommand;

		/// <summary>
		/// Gets the RefreshEnabledDisabledStateOfServerComponentsCommand.
		/// </summary>
		public RelayCommand RefreshEnabledDisabledStateOfServerComponentsCommand
		{
			get
			{
				if (this._refreshEnabledDisabledStateOfServerComponentsCommand == null) {
					this._refreshEnabledDisabledStateOfServerComponentsCommand = new RelayCommand(
						() => {
							var allServers = this.dserverManager?.Servers?.Where(srv => srv != null).ToList();

							if (allServers == null) {
								return;
							}

							// -- i) MissionLogMonitor
							if (this.Config.IsMissionLogMonitorEnabled) {
								allServers.ForEach(server => server.MissionLogService?.Start());
							} else {
								allServers.ForEach(server => server.MissionLogService?.Stop());
							}

							// -- ii) ChatLogMonitor
							if (this.Config.IsChatMonitorEnabled) {
								// ??? where is ChatLogMonitor service? 
							} else {
								// ??? where is ChatLogMonitor service? 
							}

							// -- iii) RCon connection: 
							if (this.Config.IsRConEnabled) {
								allServers.ForEach(server => server.Rcon?.Start());
							} else {
								allServers.ForEach(server => server.Rcon?.Stop());
							}
						});
				}

				return this._refreshEnabledDisabledStateOfServerComponentsCommand;
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
															this.scriptManager?.SwitchScript(scriptFile);
														}
												)
						   );
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