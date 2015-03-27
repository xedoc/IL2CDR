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
        private DServerManager dserverManager;
        private ActionManager actionManager;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        [PreferredConstructor]
        public MainViewModel()
        {
            if( IsInDesignMode )
            {
                ServerList = new ObservableCollection<Server>(new List<Server>() { new Server("Xedoc playground", false, true) });
                return;
            }
            Config = Properties.Settings.Default.Config;
            
            dserverManager = (Application.Current as App).DServerManager;
            dserverManager.DServers.CollectionChanged += DServers_CollectionChanged;
            dserverManager.With(x => x.DServers).Do( x => {
                ServerList = x;
                UpdateServerList();
            });

            actionManager = (Application.Current as App).ActionManager;

            CurrentScriptSettings = Config.ScriptConfigs.FirstOrDefault();

            var messages = (Application.Current as App).AppLogDataService.LogMessages;
            LogMessages = String.Join( Environment.NewLine, messages);
            messages.CollectionChanged += messages_CollectionChanged;
        }

        void DServers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateServerList();
        }
        private void UpdateServerList()
        {
            UI.Dispatch(() =>
            {
                if (ServerList.Count == 0)
                {
                    IsServerListMessageVisible = true;
                    ServerListMessage = "Run some DServer to start log monitoring...";
                }
                else
                {
                    IsServerListMessageVisible = false;
                }
            });
        }
        void messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
                e.NewItems.Do( x => 
                {
                    var newLines = new string[x.Count + 1];
                    newLines[0] = String.Empty;
                    x.CopyTo(newLines, 1);
                    UI.Dispatch(() => LogMessages += String.Join(Environment.NewLine, newLines));
                });
        }


        /// <summary>
        /// The <see cref="CurrentScriptSettings" /> property's name.
        /// </summary>
        public const string CurrentScriptSettingsPropertyName = "CurrentScriptSettings";

        private ScriptConfig _currentScriptSettings = null;

        /// <summary>
        /// Sets and gets the CurrentScriptSettings property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ScriptConfig CurrentScriptSettings
        {
            get
            {
                return _currentScriptSettings;
            }

            set
            {
                if (_currentScriptSettings == value)
                {
                    return;
                }

                _currentScriptSettings = value;
                RaisePropertyChanged(CurrentScriptSettingsPropertyName);
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
                return _scriptSelectionChanged
                    ?? (_scriptSelectionChanged = new RelayCommand<ScriptConfig>(
                    (config) =>
                    {
                        CurrentScriptSettings.ConfigFields = config.ConfigFields;
                    }));
            }
        }

        /// <summary>
        /// The <see cref="ServerListMessage" /> property's name.
        /// </summary>
        public const string ServerListMessagePropertyName = "ServerListMessage";

        private string _serverListMessage = null;

        /// <summary>
        /// Sets and gets the ServerListMessage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ServerListMessage
        {
            get
            {
                return _serverListMessage;
            }

            set
            {
                if (_serverListMessage == value)
                {
                    return;
                }

                _serverListMessage = value;
                RaisePropertyChanged(ServerListMessagePropertyName);
            }
        }
        
        /// <summary>
        /// The <see cref="IsServerListMessageVisible" /> property's name.
        /// </summary>
        public const string IsServerListMessageVisiblePropertyName = "IsServerListMessageVisible";

        private bool _isServerListMessageVisible = false;

        /// <summary>
        /// Sets and gets the IsServerListMessageVisible property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsServerListMessageVisible
        {
            get
            {
                return _isServerListMessageVisible;
            }

            set
            {
                if (_isServerListMessageVisible == value)
                {
                    return;
                }

                _isServerListMessageVisible = value;
                RaisePropertyChanged(IsServerListMessageVisiblePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsWindowReopen" /> property's name.
        /// </summary>
        public const string IsWindowReopenPropertyName = "IsWindowReopen";

        private bool _isWindowReopen = false;

        /// <summary>
        /// Sets and gets the IsWindowReopen property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWindowReopen
        {
            get
            {
                return _isWindowReopen;
            }

            set
            {
                if (_isWindowReopen == value)
                {
                    return;
                }

                _isWindowReopen = value;
                RaisePropertyChanged(IsWindowReopenPropertyName);
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
                return _exitApplication
                    ?? (_exitApplication = new RelayCommand(
                    () =>
                    {
                        if (!IsWindowReopen)
                        {
                            Properties.Settings.Default.Save();
                            Application.Current.Shutdown();
                        }
                        IsWindowReopen = false;
                    }));
            }
        }

        /// <summary>
        /// The <see cref="Config" /> property's name.
        /// </summary>
        public const string ConfigPropertyName = "Config";

        private Config _config = null;

        /// <summary>
        /// Sets and gets the Config property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Config Config
        {
            get
            {
                return _config;
            }

            set
            {
                if (_config == value)
                {
                    return;
                }

                _config = value;
                RaisePropertyChanged(ConfigPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="LogMessages" /> property's name.
        /// </summary>
        public const string LogMessagesPropertyName = "LogMessages";

        private string _logMessages = null;

        /// <summary>
        /// Sets and gets the LogMessages property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LogMessages
        {
            get
            {
                return _logMessages;
            }

            set
            {
                if (_logMessages == value)
                {
                    return;
                }

                _logMessages = value;
                RaisePropertyChanged(LogMessagesPropertyName);
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
                return _enableChatLogMonitor
                    ?? (_enableChatLogMonitor = new RelayCommand(
                    () =>
                    {
                        
                    }));
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
                return _enableMissionLogMonitor
                    ?? (_enableMissionLogMonitor = new RelayCommand(
                    () =>
                    {
                        var missionLogService = (Application.Current as App).MissionLogDataService;
                        if (missionLogService == null)
                            return;

                        if( !Config.IsMissionLogMonitorEnabled )
                            missionLogService.Stop();
                        else
                            missionLogService.Start();
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
                return _scriptButtonClick
                    ?? (_scriptButtonClick = new RelayCommand<string>(
                    (buttonName) =>
                    {
                        Task.Factory.StartNew(() => actionManager.ProcessButtonClick(buttonName));                        
                    }));
            }
        }
        /// <summary>
        /// The <see cref="ServerList" /> property's name.
        /// </summary>
        public const string ServerListPropertyName = "ServerList";

        private ObservableCollection<Server> _serverList = null;

        /// <summary>
        /// Sets and gets the ServerList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Server> ServerList
        {
            get
            {
                return _serverList;
            }

            set
            {
                if (_serverList == value)
                {
                    return;
                }

                _serverList = value;
                RaisePropertyChanged(ServerListPropertyName);
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}