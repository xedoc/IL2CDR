using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class Server : NotifyPropertyChangeBase
    {
        public RconConnection Rcon { get; set; }
        public MissionLogDataService MissionLogService { get; set; }
        public ProcessItem Process { get; set; }
        public Server(RconConnection rcon, ProcessItem process)
        {
            Name = String.Format(@"PID: {0} {1}\DServer.exe", process.ProcessId, process.ProcessPath);
            Process = process;
            Rcon = rcon;
            MissionLogService = new MissionLogDataService(this);
            ServerId = default(Guid);
            IsConfigSet = false;
            IsRconConnected = false;
        }
        public Server(string name, Guid guid, bool isConfigSet, bool isRconConnected )
        {
            Name = name;
            ServerId = guid;
            IsConfigSet = isConfigSet;
            IsRconConnected = isRconConnected;
        }
        public void Login()
        {
            while( true)
            {
                var result = Rcon.GetConsole();
                var newName = Re.GetSubString(result, @"Server name '(.*?)'");
                if (!String.IsNullOrWhiteSpace(newName))
                {
                    Name = newName;
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// The <see cref="ServerId" /> property's name.
        /// </summary>
        public const string ServerIdPropertyName = "ServerId";

        private Guid _serverId;

        /// <summary>
        /// Sets and gets the ServerId property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Guid ServerId
        {
            get
            {
                return _serverId;
            }

            set
            {
                if (_serverId == value)
                {
                    return;
                }

                _serverId = value;
                RaisePropertyChanged(ServerIdPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="GameObjects" /> property's name.
        /// </summary>
        public const string GameObjectsPropertyName = "GameObjects";

        private ObservableCollection<GameObject> _gameObjects;

        /// <summary>
        /// Sets and gets the GameObjects property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<GameObject> GameObjects
        {
            get
            {
                return _gameObjects;
            }

            set
            {
                if (_gameObjects == value)
                {
                    return;
                }

                _gameObjects = value;
                RaisePropertyChanged(GameObjectsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="AirFields" /> property's name.
        /// </summary>
        public const string AirFieldsPropertyName = "AirFields";

        private ObservableCollection<AirField> _airFields;

        /// <summary>
        /// Sets and gets the AirFields property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<AirField> AirFields
        {
            get
            {
                return _airFields;
            }

            set
            {
                if (_airFields == value)
                {
                    return;
                }

                _airFields = value;
                RaisePropertyChanged(AirFieldsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Players" /> property's name.
        /// </summary>
        public const string PlayersPropertyName = "Players";

        private ObservableCollection<Player> _players;

        /// <summary>
        /// Sets and gets the Players property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Player> Players
        {
            get
            {
                return _players;
            }

            set
            {
                if (_players == value)
                {
                    return;
                }

                _players = value;
                RaisePropertyChanged(PlayersPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = null;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }
        /// <summary>
        /// The <see cref="IsConfigSet" /> property's name.
        /// </summary>
        public const string IsConfigSetPropertyName = "IsConfigSet";

        private bool _isConfigSet = false;

        /// <summary>
        /// Sets and gets the IsConfigSet property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsConfigSet
        {
            get
            {
                return _isConfigSet;
            }

            set
            {
                if (_isConfigSet == value)
                {
                    return;
                }

                _isConfigSet = value;
                RaisePropertyChanged(IsConfigSetPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsRconConnected" /> property's name.
        /// </summary>
        public const string IsRconConnectedPropertyName = "IsRconConnected";

        private bool _IsRconConnected = false;

        /// <summary>
        /// Sets and gets the IsRconConnected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsRconConnected
        {
            get
            {
                return _IsRconConnected;
            }

            set
            {
                if (_IsRconConnected == value)
                {
                    return;
                }

                _IsRconConnected = value;
                RaisePropertyChanged(IsRconConnectedPropertyName);
            }
        }

    }
}
