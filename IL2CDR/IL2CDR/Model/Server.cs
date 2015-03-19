using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace IL2CDR.Model
{
    public class Server : NotifyPropertyChangeBase
    {
        [ScriptIgnore]
        public RconConnection Rcon { get; set; }
        [ScriptIgnore]
        public MissionLogDataService MissionLogService { get; set; }
        [ScriptIgnore]
        public ProcessItem Process { get; set; }

        public string CurrentMissionId { get; set; }
        public int TimeZoneOffset { get; set; }

        public Server(RconConnection rcon, ProcessItem process)
        {
            Name = String.Format(@"PID: {0} {1}\DServer.exe", process.ProcessId, process.ProcessPath);
            Process = process;
            Rcon = rcon;
            MissionLogService = new MissionLogDataService(this);
            ServerId = default(Guid);
            IsConfigSet = false;
            IsRconConnected = false;
            TimeZoneOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
            Initialize();

        }
        public Server(string name, bool isConfigSet, bool isRconConnected )
        {
            Name = name;
            ServerId = GuidUtility.Create(GuidUtility.IsoOidNamespace, name); 
            IsConfigSet = isConfigSet;
            IsRconConnected = isRconConnected;
            Initialize();
        }
        private void Initialize()
        {
            Players = new PlayersCollection();
            GameObjects = new GameObjectsCollection();
            AirFields = new AirFieldCollection();
            CoalitionIndexes = new List<CoalitionIndex>();
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

        public int GetCoalitionIndex( Country country )
        {
            var coalition = CoalitionIndexes.FirstOrDefault(c => c.Country.Id == country.Id);
            if (coalition != null)
                return coalition.Index;
            else
                return -1;
        }

        /// <summary>
        /// The <see cref="CoalitionIndexes" /> property's name.
        /// </summary>
        public const string CoalitionIndexesPropertyName = "CoalitionIndexes";

        private List<CoalitionIndex> _coalitionIndexes;

        /// <summary>
        /// Sets and gets the CoalitionIndexes property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public List<CoalitionIndex> CoalitionIndexes
        {
            get
            {
                return _coalitionIndexes;
            }

            set
            {
                if (_coalitionIndexes == value)
                {
                    return;
                }

                _coalitionIndexes = value;
                RaisePropertyChanged(CoalitionIndexesPropertyName);
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

        private GameObjectsCollection _gameObjects;

        /// <summary>
        /// Sets and gets the GameObjects property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public GameObjectsCollection GameObjects
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

        private AirFieldCollection _airFields;

        /// <summary>
        /// Sets and gets the AirFields property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public AirFieldCollection AirFields
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

        private PlayersCollection _players;

        /// <summary>
        /// Sets and gets the Players property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public PlayersCollection Players
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
        [ScriptIgnore]
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
        [ScriptIgnore]
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

    public class PlayersCollection : Dictionary<int, Player>
    {
        private object lockList = new object();

        /// <summary>
        /// Get Player object by give bot/plane/player ID. 
        /// Set/add works by Player ID only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new Player this[int id]
        {
            get
            {
                Player result = null;
                //Search by player id
                lock (lockList)
                    this.TryGetValue(id, out result);

                //If search failed - search by plane/bot id
                if (result == null)
                {
                    result = this.Values.FirstOrDefault(player =>
                        player != null &&
                        (player.Plane != null && player.Plane.Id == id ) ||
                        (player.BotPilot != null && player.BotPilot.Id == id));
                }
                return result;
            }
            set
            {
                if (value == null || value.Id <= 0)
                    return;

                lock (lockList)
                {
                    var existing = this[value.Id];
                    if (existing == null)
                        this.Add(value.Id, value);
                    else
                        existing = value;
                }
            }
        }

        public void PlayerLeave( Guid nickId )
        {
            if (nickId != null ||nickId != default(Guid))
            {
                if (this.Any((pair) => pair.Value.NickId.Equals(nickId)))
                {
                    var player = this.FirstOrDefault((pair) => pair.Value.NickId.Equals(nickId)).Value;
                    if( player != null )
                    {
                        player.IsOnline = false;
                        player.IsInAir = false;
                    }
                   
                }
            }   
        }
        public void PlayerKilled( int id )
        {
            var player = this[id];
            if (player == null)
                return;

            player.IsInAir = false;
        }

    }
    public class GameObjectsCollection : Dictionary<int,GameObject>
    {
        private object lockList = new object();
        public new GameObject this[int id]
        {
            get
            {
                GameObject result = null;
                lock (lockList)
                    this.TryGetValue(id, out result);
                return result;
            }
            set
            {
                if (value == null || value.Id <= 0)
                    return;
                lock (lockList)
                {
                    var existing = this[value.Id];
                    if (existing == null)
                        this.Add(value.Id, value);
                    else
                        existing = value;
                }
            }
        }
    }

    public class AirFieldCollection : Dictionary<int,AirField>
    {
        private object lockList = new object();
        public new AirField this[int id]
        {
            get
            {
                AirField result = null;
                lock( lockList )
                    this.TryGetValue(id, out result);
                return result;
            }
            set
            {
                if (value == null || value.Id <= 0)
                    return;

                lock (lockList)
                {
                    var existing = this[value.Id];
                    if (existing == null)
                        this.Add(value.Id, value);
                    else
                        existing = value;
                }
            }
        }
    }
    
    
}
